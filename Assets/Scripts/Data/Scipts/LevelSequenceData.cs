using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelSequenceData", menuName = "Scriptable Objects/LevelSequenceData")]
public class LevelSequenceData : ScriptableObject
{
    public List<LevelData> levelsSequence;

    public LevelData GetFirstLevel()
    {
        if (levelsSequence != null && levelsSequence.Count > 0)
            return levelsSequence[0];

        return null;
    }

    public LevelData GetLevel(int currentLevelId)
    {
        if (currentLevelId < levelsSequence.Count)
        {
            return levelsSequence[currentLevelId];
        }
        return null;
    }

    public LevelData GetNextLevel(int currentLevelId)
    {
        if (currentLevelId + 1 < levelsSequence.Count)
        {
            return levelsSequence[currentLevelId + 1];
        }
        return null; // Кампания пройдена до конца
    }
}
