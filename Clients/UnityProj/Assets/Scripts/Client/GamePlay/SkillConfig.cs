using System;
using System.Collections.Generic;
using BiangStudio;
using BiangStudio.CloneVariant;
using BiangStudio.GameDataFormat;
using BiangStudio.GameDataFormat.Grid;
using UnityEngine;

[Serializable]
public class SkillConfig : IClone<SkillConfig>, Probability
{
    public string SkillKey;
    public int CoinCost;
    public int probability;
    public Color SkillColor;
    public List<SkillGrid> OccupiedPositions = new List<SkillGrid>();

    public void RotateOccupiedPositions()
    {
        List<GridPos> gps = new List<GridPos>();
        foreach (SkillGrid sg in OccupiedPositions)
        {
            gps.Add(sg.GridPos);
        }

        gps = GridPosR.TransformOccupiedPositions(new GridPosR(0, 0, GridPosR.Orientation.Right), gps);
        for (int index = 0; index < OccupiedPositions.Count; index++)
        {
            OccupiedPositions[index] = new SkillGrid(gps[index], OccupiedPositions[index].SkillGridType);
        }
    }

    [Serializable]
    public struct SkillGrid : IClone<SkillGrid>
    {
        public GridPos GridPos;
        public SkillGridType SkillGridType;

        public SkillGrid(GridPos gridPos, SkillGridType skillGridType)
        {
            GridPos = gridPos;
            SkillGridType = skillGridType;
        }

        public SkillGrid Clone()
        {
            SkillGrid newData = new SkillGrid();
            newData.GridPos = GridPos;
            newData.SkillGridType = SkillGridType;
            return newData;
        }
    }

    public SkillConfig Clone()
    {
        SkillConfig newData = new SkillConfig();
        newData.SkillKey = SkillKey;
        newData.CoinCost = CoinCost;
        newData.probability = probability;
        newData.SkillColor = SkillColor;
        newData.OccupiedPositions = OccupiedPositions.Clone();
        return newData;
    }

    public int Probability
    {
        get { return probability; }
        set { probability = value; }
    }

    public bool IsSingleton
    {
        get { return true; }
        set { }
    }

    public Probability ProbabilityClone()
    {
        return Clone();
    }
}