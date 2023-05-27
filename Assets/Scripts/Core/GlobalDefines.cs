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
    
    public static string ConvertCost(float value) {
        if (value < 1000) return value.ToString("0");
        return value switch {
            >= 1000 and < 1000000 => (value / 1000).ToString("0.0") + "k",
            >= 1000000 and < 1000000000 => (value / 1000000).ToString("0.0") + "m",
            >= 1000000000 and < 1000000000000 => (value / 1000000000).ToString("0.0") + "b",
            >= 1000000000000 and < 1000000000000000 => (value / 1000000000).ToString("0.0") + "t",
            _ => "I'm guessing a lot"
        };
    }
}