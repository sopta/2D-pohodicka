using System;
using System.IO;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;

class DialogueTXTImporter
{
    public static void ImportFile(string filePath, DialogueNodeGraph graph)
    {
        List<(string, string)> dialog = new List<(string, string)>();

        using (StreamReader reader = new StreamReader(filePath))
        {
            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine().Trim();
                if (line == String.Empty) continue;
                
                string name = line.Replace(":", string.Empty).Trim();
                string sentence = reader.ReadLine().Trim();

                dialog.Add((name, sentence));
            }
        }
        
        float offset = graph.nodes.Count == 0 ? 0 : graph.nodes.Count * 330;
        foreach (var (actorName,sentence) in dialog)
        {
            var node = graph.AddNode<DialogueActorNode>();
            node.name = "[none actor]"; // todo @refactoring translate actor's name to ID
            //node.ActorID = ActorID.Ben;
            node.CZ.Value = sentence;

            node.position.x = offset;
            node.position.y = 0;
            
            offset += 330;
            
            AssetDatabase.AddObjectToAsset(node, AssetDatabase.GetAssetPath(graph));
        }
        
        for (int i = 0; i < graph.nodes.Count; i++)
        {
            if ((i + 1) == graph.nodes.Count) break;
            
            graph.nodes[i].GetOutputPort(DialogueActorNode.NextOutput).Connect(graph.nodes[i+1].GetInputPort(DialogueActorNode.PrevInput));
        }

        var settings = UnityEditor.AddressableAssets.AddressableAssetSettingsDefaultObject.Settings;
        var group = settings.FindGroup("Dialogues");

        if (InAddressable(group, graph) == false)
        {
            var guid = AssetDatabase.GUIDFromAssetPath(AssetDatabase.GetAssetPath(graph)).ToString();
        
            //settings.CreateAssetReference(guid);
            settings.CreateOrMoveEntry(guid, group);

            foreach (var entry in group.entries)
            {
                if (entry.AssetPath != AssetDatabase.GetAssetPath(graph)) continue;
                entry.address = graph.name.Replace(".asset", string.Empty);
            }
        }
    }

    private static bool InAddressable(AddressableAssetGroup group, DialogueNodeGraph graph)
    {
        foreach (var entry in group.entries)
        {
            if (entry.AssetPath == AssetDatabase.GetAssetPath(graph)) return true;
        }

        return false;
    }
}