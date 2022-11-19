using UnityEditor;
using UnityEngine;

namespace _src.Scripts.Debug {
    [CustomEditor(typeof(Managers.Grid))]
    public class EditorGrid : Editor
    {
        public override void OnInspectorGUI(){
            DrawDefaultInspector();
            
            var script = (Managers.Grid)target;
            if (GUILayout.Button("Generate Bounds"))
            {
                script.GenerateGrid();
            }
        }
    }
}
