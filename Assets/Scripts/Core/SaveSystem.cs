using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

// ReSharper disable InconsistentNaming

namespace Scripts.Core {
    public static class DataKey {
        public const string Player = "PLAYER";
        public const string PlayerWeapon = "WEAPONSTAT";
        public const string PlayerEquippedWeapon = "WEAPON";
        public const string PlayerSkin = "SKIN";
        public const string Volume = "VOLUME";
        public const string FPS = "FPS";
    }

    public static class PlayerStatLevels {
        public const string HP = "HP";
        public const string DEF = "DEF";
        public const string ATK = "ATK";
        public const string CRIT = "CRIT";
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
        public Dictionary<string, int> PlayerLevels;
    }

    public static class SaveSystem{
        public static PlayerData playerData;
        public static CurrentLevelData currentLevelData;
        public static bool UseFancyUI = false;
        
        private static bool _init;
        public static void Init() {
            currentLevelData = new CurrentLevelData() {
                TurnNumber = 1,
                Score = 0
            };
            if (_init) return;
            _init = true;
            if (!PlayerPrefs.HasKey(DataKey.Player)) {
                playerData = new PlayerData {
                    Coin = 12345678,
                    LevelData = new Dictionary<string, DataLevel>(),
                    PlayerLevels = new Dictionary<string, int>() {
                        {PlayerStatLevels.HP,   5},
                        {PlayerStatLevels.ATK,  6},
                        {PlayerStatLevels.DEF,  7},
                        {PlayerStatLevels.CRIT, 8}
                    }
                };
                PlayerPrefs.SetString(DataKey.Player, JsonConvert.SerializeObject(playerData));
            }

            else {
                playerData = JsonConvert.DeserializeObject<PlayerData>(PlayerPrefs.GetString(DataKey.Player));
                currentLevelData = new CurrentLevelData() {
                    TurnNumber = 1,
                    Score = 0
                };
            }
        }

        public static void SaveData(string sceneName) {
            playerData.LevelData[sceneName] = new DataLevel {
                TurnNumber = currentLevelData.TurnNumber,
                Score = currentLevelData.Score
            };
            playerData.PreviousSceneName = sceneName;
            PlayerPrefs.SetString(DataKey.Player, JsonConvert.SerializeObject(playerData));
        }
        
        public static void SaveData() {
            PlayerPrefs.SetString(DataKey.Player, JsonConvert.SerializeObject(playerData));
        }

        public static void SaveDataFloat(string key, float value) {
            PlayerPrefs.SetFloat(key, value);
        }

        public static float GetDataFloat(string key) {
            return PlayerPrefs.HasKey(key) ? PlayerPrefs.GetFloat(key) : 1;
        }
    }
}