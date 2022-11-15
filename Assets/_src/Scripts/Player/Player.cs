using System;
using UnityEngine;
using _src.Scripts.Core;
using _src.Scripts.Grid;

namespace _src.Scripts.Player {
    public class Player : MonoBehaviour {
        protected Camera Camera;

        private void Awake()
        {
            Camera = Camera.main;
        }
    }
}
