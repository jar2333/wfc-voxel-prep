using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CreatePrototypes))]
class CreatePrototypesEditor : Editor {
    public override void OnInspectorGUI() {
        EditorGUILayout.LabelField( "Actions: ",   "initial functionality");
        if (GUILayout.Button("Create Sockets and then previews")) {
            CreatePrototypes myScript = (CreatePrototypes) target;
            myScript.TestSockets();
        }
        if (GUILayout.Button("Delete Previews and Reset Sockets")) {
            CreatePrototypes myScript = (CreatePrototypes) target;
            myScript.DeletePreviewsAndReset();
        }
        if (GUILayout.Button("Create Prototypes")) {
            Debug.Log("Not implemented");
        }
        EditorGUILayout.LabelField( "Merge 2 sockets: ",   "For incorrectly distinct labels");
        string toMerge1 = EditorGUILayout.TextField("");
        string toMerge2 = EditorGUILayout.TextField("");
        if (GUILayout.Button("Merge Sockets")) {
            Debug.Log("Not implemented");
        }
        EditorGUILayout.LabelField( "Fields ",   "");
        DrawDefaultInspector();
        
    }
}
