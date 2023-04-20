using System;
using _src.Scripts.Core;
using _src.Scripts.Core.EventDispatcher;
using UnityEngine;

namespace _src.Scripts.Managers
{
    public class ScoreManager : Singleton<ScoreManager>
    {
        private void Start()
        {
            //Subscribe Events
            this.SubscribeListener(EventType.AddScore, param => AddScore((int) param));
        }

        private void AddScore(int amount)
        {
            LevelManager.instance.levelData.score += amount * LevelManager.instance.levelData.turnNumber;
            this.SendMessage(EventType.OnScoreChange, LevelManager.instance.levelData.score);
        }
    }
}
