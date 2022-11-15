using _src.Scripts.Grid;
using UnityEditor;
using UnityEngine;

namespace _src.Scripts.Debug {
    [CustomEditor(typeof(Grid.Grid))]
    public class EditorGrid : Editor
    {
        public override void OnInspectorGUI(){
            DrawDefaultInspector();
            
            var script = (Grid.Grid)target;
            if (GUILayout.Button("Generate Bounds"))
            {
                script.GenerateGrid();
            }
        }
    }
}
