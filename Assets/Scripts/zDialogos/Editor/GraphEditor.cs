using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

[CustomEditor(typeof(DialogueNodeGraph))]
public class GraphEditor : Editor
{
    public override VisualElement CreateInspectorGUI()
    {
        var root = new VisualElement();
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
