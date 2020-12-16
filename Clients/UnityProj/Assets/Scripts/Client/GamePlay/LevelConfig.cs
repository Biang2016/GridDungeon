using System;
using UnityEngine;
using Random = UnityEngine.Random;

[Serializable]
public class LevelConfig
{
    [HideInInspector]
    public int LevelIndex;
    public int SideLength;
    public int StepLimit;
    public int ShopSkillItemCount;

    public bool[,] GetLevelMap()
    {
        bool[,] levelMap = new bool[SideLength, SideLength];
        for (int col = 0; col < SideLength; col++)
        {
            for (int row = 0; row < SideLength; row++)
            {
                levelMap[col, row] = Random.value > 0.5f;
            }
        }

        return levelMap;
    }
}