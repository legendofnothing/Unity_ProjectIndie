using UnityEditor;
using UnityEngine;

namespace _src.Scripts.Debug
{
    [CustomEditor(typeof(Bounds.DestroyBound))]
    public class EditorDestroyBound : Editor
    {
        public override void OnInspectorGUI(){
            DrawDefaultInspector();
            
            var script = (Bounds.DestroyBound)target;
            if (GUILayout.Button("Generate Bounds"))
            {
                script.GenerateBounds();
            }
        }
    }
}

