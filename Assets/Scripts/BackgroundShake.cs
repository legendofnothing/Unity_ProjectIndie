using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class BackgroundShake : MonoBehaviour {
    private void Start() {
        transform
            .DOShakePosition(60f, new Vector3(2, 1), 1, 90,false, false)
            .SetLoops(-1);
    }
}
