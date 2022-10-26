using UnityEngine;

namespace _src.Scripts.Core {
    public static class GetWorldSpace {
        private static float _screenHeight;
        private static float _screenWidth;

        /// <summary>
        /// Set Screen Height and Width and store it as variable.
        /// Set Once anywhere.
        /// Did this because of some fuckery 
        /// </summary>
        /// <param name="h">Height in pixels</param>
        /// <param name="w">Width in pixels</param>
        public static void SetDimension(float h, float w){
            _screenHeight = h;
            _screenWidth = w;
        }
    
        public static void SetPosition( Transform transformToSet, float offsetX, float offsetY){
            var vec3 = new Vector3(_screenWidth / 2f, _screenHeight / 2f);
            if (Camera.main == null) return;
            var position = Camera.main.ScreenToWorldPoint(vec3) + new Vector3(offsetX, offsetY, 0f);
            position.z = 0f;
            transformToSet.position = position;
        }
    }
}
