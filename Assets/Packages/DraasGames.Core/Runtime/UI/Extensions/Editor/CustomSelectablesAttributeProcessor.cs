using System;
using System.Collections.Generic;
using System.Reflection;
using JetBrains.Annotations;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;

namespace DraasGames.Core.Runtime.UI.Extensions.Editor
{
    [UsedImplicitly]
    public class CustomSelectablesAttributeProcessor : OdinAttributeProcessor<CustomSelectable>
    {
        public override void ProcessChildMemberAttributes(InspectorProperty parentProperty, MemberInfo member,
            List<Attribute> attributes)
        {
            // Fields that should only be visible if _isNativeEffects is true
            var hideIfNotNativeFields = new HashSet<string>
            {
                "m_Transition",
                "m_GroupsAllowInteraction"
            };

            // If the current field is in the list above, add a ShowIf attribute checking our _isNativeEffects
            foreach (var field in hideIfNotNativeFields)
            {
                if (member.Name == field)
                {
                    attributes.Add(new ShowIfAttribute("@_isNativeEffects"));
                }
            }

            if (member.Name == "m_Navigation")
            {
                attributes.Add(new BoxGroupAttribute("Navigation"));
            }

            // For TargetGraphic, we only show if using native color or sprite swap
            if (member.Name == "m_TargetGraphic")
            {
                attributes.Add(new BoxGroupAttribute("Options/Native Effects"));
                attributes.Add(new ShowIfAttribute(
                    "@_isNativeEffects && (m_Transition == Transition.SpriteSwap || m_Transition == Transition.ColorTint)"));
            }

            // Put transition in an "Options" group
            if (member.Name == "m_Transition")
            {
                attributes.Add(new BoxGroupAttribute("Options"));
                // If needed: attributes.Add(new ShowIfAttribute("@m_Transition != Transition.None"));
            }

            // If color tint is used, show colors
            if (member.Name == "m_Colors")
            {
                attributes.Add(new BoxGroupAttribute("Options/Native Effects"));
                attributes.Add(new ShowIfAttribute("@_isNativeEffects && m_Transition == Transition.ColorTint"));
            }

            // If sprite swap is used, show sprite state
            if (member.Name == "m_SpriteState")
            {
                attributes.Add(new BoxGroupAttribute("Options/Native Effects"));
                attributes.Add(new ShowIfAttribute("@_isNativeEffects && m_Transition == Transition.SpriteSwap"));
            }

            // If animation transition is used, show animation triggers
            if (member.Name == "m_AnimationTriggers")
            {
                attributes.Add(new BoxGroupAttribute("Options/Native Effects"));
                attributes.Add(new ShowIfAttribute("@_isNativeEffects && m_Transition == Transition.Animation"));
            }

            // Interactable is in "Options"
            if (member.Name == "m_Interactable")
            {
                attributes.Add(new BoxGroupAttribute("Options"));
            }
        }
    }
}