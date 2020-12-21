using BiangLibrary.ObjectPool;
using UnityEngine.Events;
using UnityEngine.UI;

public class SkillButton : PoolObject
{
    public Image ButtonBGImage;
    public Button Button;
    public SkillConfig SkillConfig;
    public SkillGridIndicatorGroup SkillGridIndicatorGroup;
    public Text CoinCostText;

    public override void OnRecycled()
    {
        base.OnRecycled();
        SkillConfig = null;
        SkillGridIndicatorGroup.Clear();
    }

    public void Initialize(SkillConfig skillConfig, bool showCoin, UnityAction onClick)
    {
        SkillConfig = skillConfig;
        if (showCoin)
        {
            CoinCostText.text = $"{skillConfig.CoinCost} coins";
        }
        else
        {
            CoinCostText.text = "";
        }
        Button.image.color = skillConfig.SkillColor;
        Button.onClick.RemoveAllListeners();
        Button.onClick.AddListener(onClick);;
        SkillGridIndicatorGroup.Init(skillConfig.OccupiedPositions, 1f, true, 0f);
    }
}