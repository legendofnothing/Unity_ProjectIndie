using System;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Components {
    public class ScalerCanvas : MonoBehaviour {
        private void Start() {
            var ratio = Screen.width / (float)Screen.height;
            if (Mathf.Approximately(ratio, 1080f / 1920f)) return;
            GetComponent<CanvasScaler>().matchWidthOrHeight = Screen.width / (float)Screen.height;
        }
    }
}
