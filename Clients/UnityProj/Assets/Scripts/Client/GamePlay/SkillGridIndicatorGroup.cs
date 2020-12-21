using System.Collections.Generic;
using BiangLibrary.GameDataFormat.Grid;
using UnityEngine;

public class SkillGridIndicatorGroup : MonoBehaviour
{
    public RectTransform RectTransform;

    private List<SkillGridIndicator> SkillGridIndicators = new List<SkillGridIndicator>();

    public void Clear()
    {
        foreach (SkillGridIndicator sgi in SkillGridIndicators)
        {
            sgi.PoolRecycle();
        }

        SkillGridIndicators.Clear();
    }

    public void Init(List<SkillConfig.SkillGrid> skillGrids, float gridSizeFactor, bool autoCalculateScale, float wholeScale)
    {
        Clear();
        List<GridPos> gridPosList = new List<GridPos>();
        foreach (SkillConfig.SkillGrid sg in skillGrids)
        {
            SkillGridIndicator sgi = GameObjectPoolManager.Instance.PoolDict[GameObjectPoolManager.PrefabNames.SkillGridIndicator].AllocateGameObject<SkillGridIndicator>(RectTransform);
            sgi.Init(sg, gridSizeFactor);
            SkillGridIndicators.Add(sgi);
            gridPosList.Add(sg.GridPos);
        }

        GridRect rect = gridPosList.GetBoundingRectFromListGridPos();
        if (autoCalculateScale)
        {
            int longerSide = Mathf.Max(rect.size.x, rect.size.z);
            float sizePerGrid = RectTransform.rect.size.x / longerSide * 0.9f;
            float scale = sizePerGrid / 20f;
            RectTransform.localScale = Vector3.one * scale;
            RectTransform.anchoredPosition = new Vector2(-rect.center.x + ((rect.size.x % 2 == 0) ? 0.5f : 0), -rect.center.z + ((rect.size.z % 2 == 0) ? 0.5f : 0)) * 20f * scale;
        }
        else
        {
            RectTransform.localScale = Vector3.one * wholeScale;
            RectTransform.anchoredPosition = new Vector2(-rect.center.x, -rect.center.z) * 20f * wholeScale;
        }
    }
}