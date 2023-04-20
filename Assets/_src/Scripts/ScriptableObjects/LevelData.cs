using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelData", menuName = "LevelData", order = 2)]
public class LevelData : ScriptableObject
{
    public int turnNumber;
    public int score;
    public int sceneIndex;
}
