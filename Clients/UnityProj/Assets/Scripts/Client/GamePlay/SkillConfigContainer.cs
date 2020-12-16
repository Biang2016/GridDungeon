using BiangStudio.GameDataFormat.Grid;
using Sirenix.OdinInspector;
using UnityEngine;

public class SkillConfigContainer : MonoBehaviour
{
    public SkillConfig SkillConfig;
    public Color TintColor;

    [Button("刷新技能位置坐标")]
    private void GetSkillConfigDetailedData()
    {
        SkillConfig.SkillKey = name;
        SkillConfig.SkillColor = new Color(Random.value, Random.value, Random.value) * TintColor * 2f;
        SkillConfig.OccupiedPositions.Clear();
        SkillGridIndicator[] sgis = GetComponentsInChildren<SkillGridIndicator>();
        foreach (SkillGridIndicator sgi in sgis)
        {
            GridPos gp = new GridPos(Mathf.RoundToInt(sgi.RectTransform.anchoredPosition.x / 20f), Mathf.RoundToInt(sgi.RectTransform.anchoredPosition.y / 20f));
            SkillConfig.OccupiedPositions.Add(new SkillConfig.SkillGrid(gp, sgi.SkillGridType));
        }
    }
}