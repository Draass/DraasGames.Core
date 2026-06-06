using System;

namespace DraasGames.Core.Runtime.Infrastructure.Logger
{
    /// <summary>
    /// A strongly-typed log tag. Defined in the package so <see cref="DLogger"/> can accept
    /// <c>params DLogTag[]</c>, while the concrete tag values are code-generated into a
    /// <c>DLogTags</c> class (e.g. <c>DLogTags.UI</c>) — giving contextual auto-complete at the call
    /// site without passing bare strings. The original tag text is preserved in <see cref="Name"/>.
    /// </summary>
    public readonly struct DLogTag : IEquatable<DLogTag>
    {
        private readonly string _name;

        public string Name => _name ?? string.Empty;

        public DLogTag(string name)
        {
            _name = name;
        }

        /// <summary>Creates a tag from a raw string (for dynamic tags not covered by DLogTags).</summary>
        public static DLogTag Of(string name) => new DLogTag(name);

        public override string ToString() => Name;

        public bool Equals(DLogTag other) => string.Equals(Name, other.Name, StringComparison.Ordinal);

        public override bool Equals(object obj) => obj is DLogTag other && Equals(other);

        public override int GetHashCode() => Name.GetHashCode();
    }
}
