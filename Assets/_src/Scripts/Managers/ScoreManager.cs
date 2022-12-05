using System;
using _src.Scripts.Core.EventDispatcher;
using UnityEngine;

namespace _src.Scripts.Managers
{
    public class ScoreManager : MonoBehaviour
    {
        public LevelData levelData;

        private void Start()
        {
            //Subscribe Events
            this.SubscribeListener(EventType.AddScore, param => AddScore((int) param));
        }

        private void AddScore(int amount)
        {
            levelData.score += amount * levelData.turnNumber;
            this.SendMessage(EventType.OnScoreChange, levelData.score);
        }
    }
}
