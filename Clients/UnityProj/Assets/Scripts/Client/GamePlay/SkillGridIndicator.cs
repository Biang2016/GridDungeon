using UnityEngine;
using BiangStudio.ObjectPool;
using Sirenix.OdinInspector;
using UnityEngine.UI;

public class SkillGridIndicator : PoolObject
{
    public RectTransform RectTransform;
    public Image Image;

    public Sprite TintWhiteSprite;
    public Sprite TintBlackSprite;
    public Sprite FlipSprite;

    [OnValueChanged("OnChangeSkillGridType")]
    [SerializeField]
    internal SkillGridType SkillGridType;

    public void Init(SkillConfig.SkillGrid skillGrid, float gridSizeFactor)
    {
        RectTransform.localScale = Vector3.one * gridSizeFactor;
        RectTransform.anchoredPosition = new Vector2(skillGrid.GridPos.x, skillGrid.GridPos.z) * 20f;

        SkillGridType = skillGrid.SkillGridType;
        OnChangeSkillGridType();
    }

    private void OnChangeSkillGridType()
    {
        switch (SkillGridType)
        {
            case SkillGridType.TintWhite:
            {
                Image.sprite = TintWhiteSprite;
                break;
            }
            case SkillGridType.TintBlack:
            {
                Image.sprite = TintBlackSprite;
                break;
            }
            case SkillGridType.Flip:
            {
                Image.sprite = FlipSprite;
                break;
            }
        }
    }
}

public enum SkillGridType
{
    TintWhite,
    TintBlack,
    Flip
}