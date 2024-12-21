using System;
using System.Collections.Generic;
using System.Reflection;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;

namespace DraasGames.Core.Runtime.UI.Extensions
{
    public class CustomToggleAttributeProcessor : OdinAttributeProcessor<CustomToggle>
    {
        public override void ProcessChildMemberAttributes(InspectorProperty parentProperty, MemberInfo member, List<Attribute> attributes)
        {
            var hideIfNotNativeFields = new HashSet<string>
            {
                "m_Transition",
                "m_GroupsAllowInteraction"
            };
            
            foreach (var field in hideIfNotNativeFields)
            {
                if (member.Name == field)
                {
                    attributes.Add(new ShowIfAttribute("@_isNativeEffects"));
                }
            }

            if (member.Name == "m_TargetGraphic")
            {
                attributes.Add(new BoxGroupAttribute("Options/Native Effects"));
                attributes.Add(new ShowIfAttribute("@_isNativeEffects && " +
                                                   "(m_Transition == Transition.SpriteSwap || " +
                                                   "m_Transition == Transition.ColorTint)"));
            }
            
            if(member.Name == "m_Transition")
            {
                attributes.Add(new BoxGroupAttribute("Options"));
                //attributes.Add(new ShowIfAttribute("@m_Transition != Transition.None"));
            }
            
            if(member.Name == "m_Colors")
            {
                attributes.Add(new BoxGroupAttribute("Options/Native Effects"));
                attributes.Add(new ShowIfAttribute("@_isNativeEffects && m_Transition == Transition.ColorTint"));
            }
            
            if(member.Name == "m_SpriteState")
            {
                attributes.Add(new BoxGroupAttribute("Options/Native Effects"));
                attributes.Add(new ShowIfAttribute("@_isNativeEffects && m_Transition == Transition.SpriteSwap"));
            }
            
            if(member.Name == "m_AnimationTriggers")
            {
                attributes.Add(new BoxGroupAttribute("Options/Native Effects"));
                attributes.Add(new ShowIfAttribute("@_isNativeEffects && m_Transition == Transition.Animation"));
            }
            
            // These are the inherited fields from Selectable we want to conditionally show:
            var selectableFields = new HashSet<string>
            {
                "m_Navigation",
                "m_Transition",
                "m_Colors",
                "m_SpriteState",
                "m_AnimationTriggers",
                "m_Interactable",
                "m_TargetGraphic",
                "m_GroupsAllowInteraction"
            };

            if (member.Name == "m_Interactable")
            {
                attributes.Add(new BoxGroupAttribute("Options"));
            }
        }
    }
}