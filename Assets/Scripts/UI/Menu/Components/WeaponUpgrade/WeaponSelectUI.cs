using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;

namespace UI.Menu.Components.WeaponUpgrade {
    public class WeaponSelectUI : MonoBehaviour {
        public List<GameObject> gunPanels;
        public GameObject handler;

        private Tween _currTween;

        private void Start() {
            SelectGun(WeaponHandlerUI.instance.equippedWeapon);
        }

        public void SelectGun(int index) {
            if (WeaponHandlerUI.instance.isRunning) return;
            var dest = gunPanels[index];
            handler.transform.localPosition 
                = new Vector3(handler.transform.localPosition.x, dest.transform.localPosition.y, 0);
            WeaponHandlerUI.instance.SwitchWeapon(index);
        }
    }
}
