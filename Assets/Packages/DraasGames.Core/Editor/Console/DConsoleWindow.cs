#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using DraasGames.Core.Runtime.Infrastructure.Logger;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Packages.DraasGames.Core.Editor.Console
{
    /// <summary>
    /// A Unity-console-like window for <see cref="DLogger"/>: a virtualized list of captured messages
    /// with toggle filters per level, free-text search, a Collapse mode that groups identical messages
    /// with an occurrence count, and a detail pane with a clickable stack trace. Double-clicking a row
    /// jumps to its source line; clicking a frame in the detail pane jumps to that exact line. Tag
    /// filters appear automatically once entries carry tags.
    /// </summary>
    internal sealed class DConsoleWindow : EditorWindow
    {
        private const int LevelCount = 4; // Info, Warning, Error, Exception
        private const float RowHeight = 22f;

        private const string CollapsePrefKey = "DraasGames.DConsole.Collapse";
        private const string AutoScrollPrefKey = "DraasGames.DConsole.AutoScroll";

        private readonly List<DConsoleRow> _rows = new();
        private readonly Dictionary<(int Level, string Sender, string Message), int> _collapseIndex = new();
        private readonly bool[] _levelEnabled = { true, true, true, true };
        private readonly HashSet<string> _activeTags = new();

        private Texture[] _levelIcons;
        private Button[] _levelButtons;
        private Label[] _levelCountLabels;

        private ListView _list;
        private VisualElement _detailContainer;
        private VisualElement _tagContainer;
        private List<string> _knownTags = new();

        private string _search = string.Empty;
        private bool _autoScroll = true;
        private bool _collapse;
        private bool _dirty;

        [MenuItem("Window/DraasGames/Console")]
        public static void Open()
        {
            var window = GetWindow<DConsoleWindow>();
            window.titleContent = new GUIContent("DConsole");
            window.Show();
        }

        private void CreateGUI()
        {
            _collapse = EditorPrefs.GetBool(CollapsePrefKey, false);
            _autoScroll = EditorPrefs.GetBool(AutoScrollPrefKey, true);

            // Full-size console icons (32px) instead of the .sml 16px variants: downscaling to the row
            // height stays crisp and does not pixelate.
            _levelIcons = new[]
            {
                IconOrNull("console.infoicon"),
                IconOrNull("console.warnicon"),
                IconOrNull("console.erroricon"),
                IconOrNull("console.erroricon") // Exception reuses the error icon
            };
            _levelButtons = new Button[LevelCount];
            _levelCountLabels = new Label[LevelCount];

            var root = rootVisualElement;
            root.Add(BuildToolbar());

            var split = new TwoPaneSplitView(1, 160f, TwoPaneSplitViewOrientation.Vertical);
            split.style.flexGrow = 1;
            root.Add(split);

            _list = BuildList();
            split.Add(_list);
            split.Add(BuildDetail());

            // Coalesce high-frequency recorder changes into one refresh per tick.
            DConsoleRecorder.Changed -= OnRecorderChanged;
            DConsoleRecorder.Changed += OnRecorderChanged;
            root.schedule.Execute(Tick).Every(100);

            Rebuild();

            // The built-in empty label is created on the first empty render, possibly a frame later.
            root.schedule.Execute(HideEmptyLabel).StartingIn(50);
        }

        private void OnDisable()
        {
            DConsoleRecorder.Changed -= OnRecorderChanged;
        }

        private VisualElement BuildToolbar()
        {
            var toolbar = new Toolbar();

            toolbar.Add(new ToolbarButton(OnClearClicked) { text = "Clear" });

            var collapse = new ToolbarToggle { text = "Collapse", value = _collapse };
            collapse.RegisterValueChangedCallback(evt =>
            {
                _collapse = evt.newValue;
                EditorPrefs.SetBool(CollapsePrefKey, _collapse);
                _dirty = true;
            });
            toolbar.Add(collapse);

            var clearOnPlay = new ToolbarToggle { text = "Clear on Play", value = DConsoleRecorder.ClearOnPlay };
            clearOnPlay.RegisterValueChangedCallback(evt => DConsoleRecorder.ClearOnPlay = evt.newValue);
            toolbar.Add(clearOnPlay);

            var autoScroll = new ToolbarToggle { text = "Auto-scroll", value = _autoScroll };
            autoScroll.RegisterValueChangedCallback(evt =>
            {
                _autoScroll = evt.newValue;
                EditorPrefs.SetBool(AutoScrollPrefKey, _autoScroll);
            });
            toolbar.Add(autoScroll);

            var search = new ToolbarSearchField();
            search.style.flexGrow = 1;
            search.style.marginLeft = 4;
            search.style.marginRight = 4;
            search.RegisterValueChangedCallback(evt =>
            {
                _search = evt.newValue ?? string.Empty;
                _dirty = true;
            });
            toolbar.Add(search);

            _tagContainer = new VisualElement { style = { flexDirection = FlexDirection.Row, alignItems = Align.Center } };
            _tagContainer.style.display = DisplayStyle.None;
            toolbar.Add(_tagContainer);

            for (var i = 0; i < LevelCount; i++)
            {
                toolbar.Add(CreateLevelFilter(i, (DLogLevel)i));
            }

            return toolbar;
        }

        private VisualElement CreateLevelFilter(int index, DLogLevel level)
        {
            var button = new Button(() =>
            {
                _levelEnabled[index] = !_levelEnabled[index];
                UpdateLevelFilterVisual(index);
                _dirty = true;
            })
            {
                tooltip = level.ToString()
            };

            button.style.flexDirection = FlexDirection.Row;
            button.style.alignItems = Align.Center;
            button.style.marginLeft = 0;
            button.style.marginRight = 0;
            button.style.paddingLeft = 4;
            button.style.paddingRight = 4;

            if (_levelIcons[index] != null)
            {
                var icon = new Image { image = _levelIcons[index], scaleMode = ScaleMode.ScaleToFit };
                icon.style.width = 16;
                icon.style.height = 16;
                icon.style.marginRight = 2;
                button.Add(icon);
            }

            var count = new Label("0");
            count.style.unityTextAlign = TextAnchor.MiddleLeft;
            button.Add(count);

            _levelButtons[index] = button;
            _levelCountLabels[index] = count;
            UpdateLevelFilterVisual(index);
            return button;
        }

        private void UpdateLevelFilterVisual(int index)
        {
            _levelButtons[index].style.opacity = _levelEnabled[index] ? 1f : 0.4f;
        }

        private ListView BuildList()
        {
            var list = new ListView
            {
                fixedItemHeight = RowHeight,
                selectionType = SelectionType.Single,
                showAlternatingRowBackgrounds = AlternatingRowBackground.ContentOnly,
                itemsSource = _rows,
                makeItem = MakeRow,
                bindItem = BindRow
            };
            list.style.flexGrow = 1;
            list.selectionChanged += OnSelectionChanged;
            list.itemsChosen += OnItemsChosen;
            return list;
        }

        private static VisualElement MakeRow()
        {
            var row = new VisualElement();
            row.style.flexDirection = FlexDirection.Row;
            row.style.alignItems = Align.Center;
            row.style.paddingLeft = 2;
            row.style.paddingRight = 4;

            var icon = new Image { name = "icon", scaleMode = ScaleMode.ScaleToFit };
            icon.style.width = 16;
            icon.style.height = 16;
            icon.style.marginRight = 4;
            icon.style.flexShrink = 0;
            row.Add(icon);

            var message = new Label { name = "message" };
            message.style.flexGrow = 1;
            message.style.overflow = Overflow.Hidden;
            message.style.textOverflow = TextOverflow.Ellipsis;
            message.style.whiteSpace = WhiteSpace.NoWrap;
            message.style.unityTextAlign = TextAnchor.MiddleLeft;
            row.Add(message);

            var meta = new Label { name = "meta" };
            meta.style.flexShrink = 0;
            meta.style.marginLeft = 8;
            meta.style.unityTextAlign = TextAnchor.MiddleRight;
            meta.style.color = new Color(0.6f, 0.6f, 0.6f);
            row.Add(meta);

            var badge = new Label { name = "badge" };
            badge.style.flexShrink = 0;
            badge.style.marginLeft = 6;
            badge.style.paddingLeft = 6;
            badge.style.paddingRight = 6;
            badge.style.backgroundColor = new Color(0.32f, 0.32f, 0.32f);
            badge.style.color = Color.white;
            badge.style.unityTextAlign = TextAnchor.MiddleCenter;
            badge.style.borderTopLeftRadius = 9;
            badge.style.borderTopRightRadius = 9;
            badge.style.borderBottomLeftRadius = 9;
            badge.style.borderBottomRightRadius = 9;
            badge.style.display = DisplayStyle.None;
            row.Add(badge);

            return row;
        }

        private void BindRow(VisualElement element, int index)
        {
            if (index < 0 || index >= _rows.Count)
            {
                return;
            }

            var row = _rows[index];
            var entry = row.Entry;

            var icon = element.Q<Image>("icon");
            if (icon != null)
            {
                var iconIndex = Mathf.Clamp((int)entry.Level, 0, _levelIcons.Length - 1);
                icon.image = _levelIcons[iconIndex];
            }

            var message = element.Q<Label>("message");
            if (message != null)
            {
                message.text = SingleLine(entry.Message);
                message.style.color = ColorFor(entry.Level);
            }

            var meta = element.Q<Label>("meta");
            if (meta != null)
            {
                meta.text = BuildMeta(entry);
            }

            var badge = element.Q<Label>("badge");
            if (badge != null)
            {
                if (_collapse && row.Count > 1)
                {
                    badge.text = row.Count.ToString();
                    badge.style.display = DisplayStyle.Flex;
                }
                else
                {
                    badge.style.display = DisplayStyle.None;
                }
            }
        }

        private VisualElement BuildDetail()
        {
            // Vertical-only scroll so long lines wrap to the pane width instead of scrolling sideways.
            var scroll = new ScrollView(ScrollViewMode.Vertical);
            scroll.style.flexGrow = 1;

            _detailContainer = new VisualElement();
            _detailContainer.style.paddingLeft = 6;
            _detailContainer.style.paddingTop = 4;
            _detailContainer.style.paddingRight = 6;
            _detailContainer.style.paddingBottom = 4;
            scroll.Add(_detailContainer);

            return scroll;
        }

        private void OnClearClicked()
        {
            DConsoleRecorder.Clear();
        }

        private void OnRecorderChanged()
        {
            _dirty = true;
        }

        private void Tick()
        {
            if (!_dirty)
            {
                return;
            }

            _dirty = false;
            Rebuild();
        }

        private void Rebuild()
        {
            _rows.Clear();

            var source = DConsoleRecorder.Snapshot;

            if (_collapse)
            {
                _collapseIndex.Clear();
                for (var i = 0; i < source.Count; i++)
                {
                    var entry = source[i];
                    if (!PassesFilter(entry))
                    {
                        continue;
                    }

                    var key = ((int)entry.Level, entry.Sender ?? string.Empty, entry.Message ?? string.Empty);
                    if (_collapseIndex.TryGetValue(key, out var rowIndex))
                    {
                        var existing = _rows[rowIndex];
                        existing.Count++;
                        _rows[rowIndex] = existing;
                    }
                    else
                    {
                        _collapseIndex[key] = _rows.Count;
                        _rows.Add(new DConsoleRow { Entry = entry, Count = 1 });
                    }
                }
            }
            else
            {
                for (var i = 0; i < source.Count; i++)
                {
                    var entry = source[i];
                    if (PassesFilter(entry))
                    {
                        _rows.Add(new DConsoleRow { Entry = entry, Count = 1 });
                    }
                }
            }

            _list.RefreshItems();
            HideEmptyLabel();
            UpdateCounts();
            UpdateTagFilters();

            if (_autoScroll && _rows.Count > 0)
            {
                _list.ScrollToItem(_rows.Count - 1);
            }
        }

        private void HideEmptyLabel()
        {
            if (_list == null)
            {
                return;
            }

            // ListView shows a built-in "List is empty" label when the source is empty; hide it.
            var empty = _list.Q<Label>(className: "unity-collection-view__empty-label")
                        ?? _list.Q<Label>(className: "unity-list-view__empty-label");
            if (empty != null)
            {
                empty.style.display = DisplayStyle.None;
            }
        }

        private bool PassesFilter(DConsoleEntry entry)
        {
            var levelIndex = (int)entry.Level;
            if (levelIndex >= 0 && levelIndex < _levelEnabled.Length && !_levelEnabled[levelIndex])
            {
                return false;
            }

            if (!string.IsNullOrEmpty(_search))
            {
                var inMessage = entry.Message != null &&
                                entry.Message.IndexOf(_search, StringComparison.OrdinalIgnoreCase) >= 0;
                var inSender = entry.Sender != null &&
                               entry.Sender.IndexOf(_search, StringComparison.OrdinalIgnoreCase) >= 0;
                if (!inMessage && !inSender)
                {
                    return false;
                }
            }

            if (_activeTags.Count > 0)
            {
                var hit = false;
                for (var i = 0; i < entry.Tags.Count; i++)
                {
                    if (_activeTags.Contains(entry.Tags[i]))
                    {
                        hit = true;
                        break;
                    }
                }

                if (!hit)
                {
                    return false;
                }
            }

            return true;
        }

        private void UpdateCounts()
        {
            for (var i = 0; i < LevelCount; i++)
            {
                if (_levelCountLabels[i] != null)
                {
                    _levelCountLabels[i].text = DConsoleRecorder.GetCount((DLogLevel)i).ToString();
                }
            }
        }

        /// <summary>
        /// Rebuilds the tag-filter row from the union of tags present in the buffer. Stays hidden while
        /// no entry carries tags, so it lights up automatically once DLogger starts attaching them —
        /// no further window changes required.
        /// </summary>
        private void UpdateTagFilters()
        {
            var seen = new HashSet<string>();
            var source = DConsoleRecorder.Snapshot;
            for (var i = 0; i < source.Count; i++)
            {
                var tags = source[i].Tags;
                for (var t = 0; t < tags.Count; t++)
                {
                    seen.Add(tags[t]);
                }
            }

            if (seen.Count == _knownTags.Count && seen.SetEquals(_knownTags))
            {
                return;
            }

            _knownTags = new List<string>(seen);
            _knownTags.Sort(StringComparer.OrdinalIgnoreCase);

            _tagContainer.Clear();
            _activeTags.RemoveWhere(tag => !seen.Contains(tag));

            foreach (var tag in _knownTags)
            {
                var capturedTag = tag;
                var toggle = new ToolbarToggle { text = tag, value = _activeTags.Contains(tag) };
                toggle.RegisterValueChangedCallback(evt =>
                {
                    if (evt.newValue)
                    {
                        _activeTags.Add(capturedTag);
                    }
                    else
                    {
                        _activeTags.Remove(capturedTag);
                    }

                    _dirty = true;
                });
                _tagContainer.Add(toggle);
            }

            _tagContainer.style.display = _knownTags.Count > 0 ? DisplayStyle.Flex : DisplayStyle.None;
        }

        private void OnSelectionChanged(IEnumerable<object> selection)
        {
            foreach (var item in selection)
            {
                if (item is DConsoleRow row)
                {
                    ShowDetail(row.Entry, row.Count);
                    return;
                }
            }

            ShowDetail(null);
        }

        private void OnItemsChosen(IEnumerable<object> chosen)
        {
            foreach (var item in chosen)
            {
                if (item is DConsoleRow row && DConsoleRecorder.TryOpenInEditor(row.Entry))
                {
                    return;
                }
            }
        }

        private void ShowDetail(DConsoleEntry entry, int count = 1)
        {
            _detailContainer.Clear();

            if (entry == null)
            {
                return;
            }

            var header = new StringBuilder();
            if (count > 1)
            {
                header.Append('x').Append(count).Append("   ");
            }

            if (!string.IsNullOrEmpty(entry.Sender))
            {
                header.Append('[').Append(entry.Sender).Append("]  ");
            }

            header.Append(entry.Message);
            _detailContainer.Add(MakeDetailLine(header.ToString(), null, 0));

            if (string.IsNullOrEmpty(entry.StackTrace))
            {
                return;
            }

            // Render each stack frame as its own line; frames carrying "(at path:line)" become
            // clickable links that jump straight to that line.
            var lines = entry.StackTrace.Split('\n');
            foreach (var rawLine in lines)
            {
                var line = rawLine.TrimEnd('\r');
                if (line.Length == 0)
                {
                    continue;
                }

                var match = StackLineRegex.Match(line);
                if (match.Success)
                {
                    int.TryParse(match.Groups[2].Value, out var lineNumber);
                    _detailContainer.Add(MakeDetailLine(line, match.Groups[1].Value, lineNumber));
                }
                else
                {
                    _detailContainer.Add(MakeDetailLine(line, null, 0));
                }
            }
        }

        private VisualElement MakeDetailLine(string text, string path, int line)
        {
            var label = new Label(text);
            label.style.whiteSpace = WhiteSpace.Normal; // wrap long lines to the pane width
            label.selection.isSelectable = true;

            if (string.IsNullOrEmpty(path))
            {
                return label;
            }

            label.style.color = new Color(0.45f, 0.6f, 1f);
            label.RegisterCallback<ClickEvent>(_ => DConsoleRecorder.OpenFile(path, line));
            label.RegisterCallback<MouseEnterEvent>(_ => label.style.unityFontStyleAndWeight = FontStyle.Bold);
            label.RegisterCallback<MouseLeaveEvent>(_ => label.style.unityFontStyleAndWeight = FontStyle.Normal);
            return label;
        }

        private static string SingleLine(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return string.Empty;
            }

            var breakIndex = text.IndexOfAny(NewLineChars);
            return breakIndex >= 0 ? text.Substring(0, breakIndex) : text;
        }

        private static string BuildMeta(DConsoleEntry entry)
        {
            var time = entry.Time.ToString("HH:mm:ss");
            return string.IsNullOrEmpty(entry.Sender) ? time : entry.Sender + "   " + time;
        }

        private static Color ColorFor(DLogLevel level)
        {
            switch (level)
            {
                case DLogLevel.Warning:
                    return new Color(1f, 0.78f, 0.2f);
                case DLogLevel.Error:
                case DLogLevel.Exception:
                    return new Color(1f, 0.42f, 0.42f);
                default:
                    return new Color(0.85f, 0.85f, 0.85f);
            }
        }

        private static Texture IconOrNull(string iconName)
        {
            var content = EditorGUIUtility.IconContent(iconName);
            return content?.image;
        }

        private static readonly char[] NewLineChars = { '\n', '\r' };

        private static readonly Regex StackLineRegex = new(@"\(at (.+?):(\d+)\)", RegexOptions.Compiled);

        private struct DConsoleRow
        {
            public DConsoleEntry Entry;
            public int Count;
        }
    }
}
#endif
