using System;
using _src.Scripts.Core;
using _src.Scripts.Core.EventDispatcher;
using UnityEngine;

namespace _src.Scripts.Managers
{
    public class ScoreManager : Singleton<ScoreManager>
    {
        private void Start() {
            //Subscribe Events
            this.SubscribeListener(EventType.AddScore, param => AddScore((int) param));
        }

        private void AddScore(int amount) {
            SaveSystem.instance.currentLevelData.Score += amount 
                                                          * SaveSystem.instance.currentLevelData.TurnNumber;
            this.SendMessage(EventType.OnScoreChange, SaveSystem.instance.currentLevelData.Score);
        }
    }
}
