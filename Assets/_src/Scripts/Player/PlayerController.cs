using System;
using UnityEngine;

namespace _src.Scripts.Player {
    public class PlayerController : Player {
        private void Update(){
            RotatePlayer();
        }

        private void RotatePlayer() {
            switch (Input.touchCount)
            {
                case > 0: {
                    var touchInput = Input.GetTouch(0);

                    switch (touchInput.phase)
                    {
                        case TouchPhase.Moved: {
                            var touchPos = camera.ScreenToWorldPoint(touchInput.position);
                            var position = transform.position;
                            
                            var degreeDiff = Mathf.Atan2(touchPos.y - position.y, touchPos.x - position.x) * Mathf.Rad2Deg;
                            var angle = Mathf.Clamp(degreeDiff - 90f, -80f, 80f);
                            transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));

                            break;
                        }
                        case TouchPhase.Ended:
                            transform.rotation = Quaternion.Lerp(Quaternion.identity, Quaternion.Euler(Vector3.zero), 0.8f);
                            break;
                    }

                    break;
                }
            }
        }
    }
}
