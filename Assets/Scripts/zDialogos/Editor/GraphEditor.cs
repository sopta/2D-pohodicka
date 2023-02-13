using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using XNodeEditor;

[CustomEditor(typeof(DialogueNodeGraph))]
public class GraphEditor : Editor
{
    public override VisualElement CreateInspectorGUI()
    {
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
        btnClearAll.clickable.clicked += () => Debug.Log("clearing");
        
        root.Add(btn);
        root.Add(btnClearAll);
        return root;
    }
}
