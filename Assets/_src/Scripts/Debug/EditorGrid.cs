using UnityEditor;
using UnityEngine;

namespace _src.Scripts.Debug {
    [CustomEditor(typeof(Managers.GridManager))]
    public class EditorGrid : Editor
    {
        public override void OnInspectorGUI(){
            DrawDefaultInspector();
            
            var script = (Managers.GridManager)target;
            if (GUILayout.Button("Generate Bounds"))
            {
                script.GenerateGrid();
            }
        }
    }
}
