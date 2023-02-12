using System;
using DefaultNamespace;
using UnityEngine;
using UnityEngine.Localization.Settings;
using XNode;

[CreateAssetMenu]
public class DialogueNodeGraph : NodeGraph
{
    public DialogueActorNode StartAt;
    
    public override Node AddNode(Type type)
    {
        var node = base.AddNode(type);

        Guid g = Guid.NewGuid();
        
        (node as DialogueActorNode).ID = g.ToString().Split("-")[0];
        
        foreach (var locale in LocalizationSettings.AvailableLocales.Locales)
        {
            var dialogueEntryValue = new DialogEntryValue { Locale = locale.Identifier };
           
            switch (locale.Identifier.Code)
            {
                case "cs":
                    (node as DialogueActorNode).CZ = dialogueEntryValue;
                    break;
                case "en":
                    (node as DialogueActorNode).EN = dialogueEntryValue;
                    break;
            }
        }

        return node;
    }
}
