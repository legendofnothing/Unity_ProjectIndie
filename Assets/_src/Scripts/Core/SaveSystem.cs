using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
// ReSharper disable InconsistentNaming

namespace _src.Scripts.Core {
    public static class DataKey {
        public const string Player = "PLAYER";
    }
    
    public struct DataLevel {
        public int TurnNumber;
        public int Score;
    }

    public struct CurrentLevelData {
        public int TurnNumber;
        public int Score;
    }
    
    public class PlayerData {
        public int Coin;
        public string PreviousSceneName; 
        public Dictionary<string, DataLevel> LevelData;
    }

    public class SaveSystem : Singleton<SaveSystem> {
        public PlayerData playerData;
        public CurrentLevelData currentLevelData;
        
        private bool _init;
        public void Init() {
            currentLevelData = new CurrentLevelData() {
                TurnNumber = 1,
                Score = 0
            };
            if (_init) return;
            _init = true;
            DontDestroyOnLoad(gameObject);
            if (!PlayerPrefs.HasKey(DataKey.Player)) {
                playerData = new PlayerData {
                    Coin = 0,
                    LevelData = new Dictionary<string, DataLevel>()
                };
                PlayerPrefs.SetString(DataKey.Player, JsonConvert.SerializeObject(playerData));
            }

            else {
                playerData = JsonConvert.DeserializeObject<PlayerData>(PlayerPrefs.GetString(DataKey.Player));
            }
        }

        public void SaveData(string sceneName) {
            playerData.LevelData[sceneName] = new DataLevel() {
                TurnNumber = currentLevelData.TurnNumber,
                Score = currentLevelData.Score
            };
            playerData.PreviousSceneName = sceneName;
        }

        private void OnDestroy() {
            PlayerPrefs.SetString(DataKey.Player, JsonConvert.SerializeObject(playerData));
        }
    }
}