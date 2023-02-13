using System;
using System.Linq;
using UnityEditor;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using XNodeEditor;

[CustomEditor(typeof(DialogueNodeGraph))]
public class GraphEditor : Editor
{
    private bool _CanBeAdded;

    private AddressableAssetSettings _AddressableAssetSettings;
    private AddressableAssetGroup _AddressableAssetGroup;
    
    private void OnEnable()
    {
        _AddressableAssetSettings = UnityEditor.AddressableAssets.AddressableAssetSettingsDefaultObject.Settings;
        _AddressableAssetGroup = _AddressableAssetSettings.FindGroup("Dialogues");

        _CanBeAdded = !InAddressable(_AddressableAssetGroup, target as DialogueNodeGraph);
    }

    public override VisualElement CreateInspectorGUI()
    {
        DialogueNodeGraph graph = target as DialogueNodeGraph;

        var root = new VisualElement();
        
        var showGraphBtn = new Button();
        showGraphBtn.text = "Edit graph";
        showGraphBtn.style.height = 40;
        showGraphBtn.clickable.clicked += () =>
        {
            NodeEditorWindow.Open(serializedObject.targetObject as XNode.NodeGraph);
        };
        
        root.Add(showGraphBtn);
        
        InspectorElement.FillDefaultInspector(root, serializedObject, this);

        var btn = new Button();
        btn.text = "Import";
        btn.clickable.clicked += () => Debug.Log("yeee");
        
        var btnClearAll = new Button();
        btnClearAll.text = "Clear all nodes";
        btnClearAll.clickable.clicked += () =>
        {
            foreach (var node in graph.nodes.ToList()) // we need to make a copy
            {
                graph.RemoveNode(node);
                AssetDatabase.RemoveObjectFromAsset(node);
            }

            graph.StartAt = null;
            
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        };

        var addToAddressableBtn = new Button();
        addToAddressableBtn.text = "Add to Addressable";
        addToAddressableBtn.visible = _CanBeAdded;
        addToAddressableBtn.clickable.clicked += () =>
        {
            var guid = AssetDatabase.GUIDFromAssetPath(AssetDatabase.GetAssetPath(graph)).ToString();
    
            _AddressableAssetSettings.CreateOrMoveEntry(guid, _AddressableAssetGroup);

            foreach (var entry in _AddressableAssetGroup.entries)
            {
                if (entry.AssetPath != AssetDatabase.GetAssetPath(graph)) continue;
                entry.address = graph.name.Replace(".asset", string.Empty);
            }
        };
        
        root.Add(btn);
        root.Add(btnClearAll);
        root.Add(addToAddressableBtn);
        return root;
    }
    
    private bool InAddressable(AddressableAssetGroup group, DialogueNodeGraph graph)
    {
        foreach (var entry in group.entries)
        {
            if (entry.AssetPath == AssetDatabase.GetAssetPath(graph)) return true;
        }

        return false;
    }
}
