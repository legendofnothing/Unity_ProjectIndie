using System;
using UnityEngine;
using EventType = Scripts.Core.EventDispatcher.EventType;
using UI.Components;
using Scripts.Core.EventDispatcher;

namespace Scripts.Core {
    public class GlobalDefines {
        [Serializable]
        public struct SpawnData {
            public GameObject prefab;
            public float chance; 
        }
    }

    public class FloatPair {
        public float float1;
        public float float2;

        public FloatPair(float float1, float float2) {
            this.float1 = float1;
            this.float2 = float2;
        }
    }
}

public static class UIStatic {
    public static void FireUIEvent(object type, float value, bool isBar = false) {
        var e = isBar ? EventType.OnBarUIChange : EventType.OnTextUIChange;
        object msg = isBar
            ? new BarUI.Message((BarUI.Type) type, value)
            : new TextUI.Message((TextUI.Type) type, value);
            
        EventDispatcher.instance.SendMessage(e,msg);
    }
}