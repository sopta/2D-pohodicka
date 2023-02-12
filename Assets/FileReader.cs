using System;
using System.IO;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;

class FileReader
{
    public static void ReadFile(string filePath, DialogueNodeGraph graph)
    {
        List<Tuple<string, string>> dialog = new List<Tuple<string, string>>();

        using (StreamReader reader = new StreamReader(filePath))
        {
            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine().Trim();
                if (line == String.Empty) continue;
                
                string name = line.Replace(":", string.Empty).Trim();
                string sentence = reader.ReadLine().Trim();

                dialog.Add(new Tuple<string, string>(name, sentence));
            }
        }
        
        float offset = graph.nodes.Count == 0 ? 0 : graph.nodes.Count * 330;
        foreach (Tuple<string, string> entry in dialog)
        {
            var node = graph.AddNode<DialogueActorNode>();
            node.name = "[none actor]";
            node.CZ.Value = entry.Item2;

            node.position.x = offset;
            node.position.y = 0;
            
            offset += 330;
            
            AssetDatabase.AddObjectToAsset(node, AssetDatabase.GetAssetPath(graph));
        }
        
        for (int i = 0; i < graph.nodes.Count; i++)
        {
            if ((i + 1) == graph.nodes.Count) break;
            
            graph.nodes[i].GetOutputPort("Next").Connect(graph.nodes[i+1].GetInputPort("Prev"));
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