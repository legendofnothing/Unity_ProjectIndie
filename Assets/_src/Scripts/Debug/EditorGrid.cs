using _src.Scripts.Grid;
using UnityEditor;
using UnityEngine;

namespace _src.Scripts.Debug {
    [CustomEditor(typeof(SpawningGrid))]
    public class EditorGrid : Editor
    {
        public override void OnInspectorGUI(){
            DrawDefaultInspector();
            
            var script = (SpawningGrid)target;
            if (GUILayout.Button("Generate Bounds"))
            {
                script.GenerateGrid();
            }
        }
    }
}
