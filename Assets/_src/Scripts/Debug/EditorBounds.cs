using UnityEditor;
using UnityEngine;

namespace _src.Scripts.Debug {
    [CustomEditor(typeof(Bounds.Bounds))]
    public class EditorBounds : Editor {
        public override void OnInspectorGUI(){
            DrawDefaultInspector();
            
            var script = (Bounds.Bounds)target;
            if (GUILayout.Button("Generate Bounds"))
            {
                script.GenerateBounds();
            }
        }
    }
}
