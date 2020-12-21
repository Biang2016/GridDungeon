using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using BiangLibrary;
using BiangLibrary.GamePlay.UI;
using UnityEngine.UI;

public class NewSkillSelectPanel : BaseUIPanel
{
    public Animator PanelAnim;

    void Awake()
    {
        UIType.InitUIType(
            false,
            false,
            false,
            UIFormTypes.Normal,
            UIFormShowModes.Normal,
            UIFormLucencyTypes.Penetrable);
        ReturnButton.onClick.AddListener(() =>
        {
            Close();
            LevelManager.Instance.LoadNextLevel();
        });
    }

    public Text CurrentCoinText;

    [SerializeField]
    private Transform SkillButtonContainer;

    [SerializeField]
    private Button ReturnButton;

    public void RefreshSkillShop(int skillItemCount)
    {
        Clear();
        List<SkillConfig> validSkills = new List<SkillConfig>();

        foreach (KeyValuePair<string, SkillConfig> kv in LevelManager.Instance.SkillConfigDict)
        {
            if (LevelManager.Instance.MyCurrentSkills.ContainsKey(kv.Key)) continue;
            validSkills.Add(kv.Value.Clone());
        }

        List<SkillConfig> newSkills = CommonUtils.GetRandomWithProbabilityFromList(validSkills, skillItemCount);
        newSkills.Sort((a, b) => a.CoinCost.CompareTo(b.CoinCost));
        foreach (SkillConfig newSkill in newSkills)
        {
            AddSkill(newSkill);
        }

        RefreshAllSkillsAffordable();
    }

    private Dictionary<string, SkillButton> SkillButtonDict = new Dictionary<string, SkillButton>();

    public void RefreshAllSkillsAffordable()
    {
        CurrentCoinText.text = $"You have " + CommonUtils.AddHighLightColorToText(LevelManager.Instance.CurrentCoin.ToString(), "#FFFF00") + " coins";
        foreach (KeyValuePair<string, SkillButton> kv in SkillButtonDict)
        {
            kv.Value.Button.interactable = LevelManager.Instance.CurrentCoin >= kv.Value.SkillConfig.CoinCost;
            kv.Value.CoinCostText.color = kv.Value.Button.interactable ? Color.yellow : Color.gray;
        }
    }

    public void AddSkill(SkillConfig skillConfig)
    {
        SkillButton sb = GameObjectPoolManager.Instance.PoolDict[GameObjectPoolManager.PrefabNames.SkillButton].AllocateGameObject<SkillButton>(SkillButtonContainer);
        sb.Initialize(skillConfig, true, () =>
        {
            LevelManager.Instance.CurrentCoin -= skillConfig.CoinCost;
            RemoveSkill(skillConfig.SkillKey);
            StartCoroutine(ClientUtils.UpdateLayout((RectTransform) SkillButtonContainer.transform));
            LevelManager.Instance.AddSkill(skillConfig.SkillKey);
            RefreshAllSkillsAffordable();
        });

        StartCoroutine(ClientUtils.UpdateLayout((RectTransform) SkillButtonContainer.transform));
        SkillButtonDict.Add(skillConfig.SkillKey, sb);
    }

    public void RemoveSkill(string skillKey)
    {
        if (SkillButtonDict.TryGetValue(skillKey, out SkillButton button))
        {
            button.PoolRecycle();
        }

        SkillButtonDict.Remove(skillKey);
    }

    public void Clear()
    {
        foreach (KeyValuePair<string, SkillButton> kv in SkillButtonDict)
        {
            kv.Value.PoolRecycle();
        }

        SkillButtonDict.Clear();
    }

    public void Show()
    {
        PanelAnim.SetTrigger("Jump");
    }

    public void Close()
    {
        PanelAnim.SetTrigger("Hide");
    }
}