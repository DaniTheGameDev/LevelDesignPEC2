using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewLevelData", menuName = "ScriptableObjects/Level Data", order = 1)]
public class LevelDataScriptableObject : ScriptableObject
{
    public List<LevelData> levels = new List<LevelData>();
}