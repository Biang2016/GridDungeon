using UnityEngine;
using System.Collections.Generic;
using BiangLibrary.GamePlay.UI;
using UnityEngine.UI;

public class SkillSelectPanel : BaseUIPanel
{
    void Awake()
    {
        UIType.InitUIType(
            false,
            false,
            false,
            UIFormTypes.Normal,
            UIFormShowModes.Normal,
            UIFormLucencyTypes.Penetrable);

        RegretButton.onClick.AddListener(LevelManager.Instance.RetractStep);
        GiveUpButton.onClick.AddListener(() =>
        {
            LevelManager.Instance.Failed();
            ResultPanel rp = UIManager.Instance.GetBaseUIForm<ResultPanel>();
            rp.SetGiveUp();
        });
    }

    public Transform SkillButtonContainer;
    private Dictionary<string, SkillButton> SkillButtonDict = new Dictionary<string, SkillButton>();

    [SerializeField]
    internal SkillGridIndicatorGroup SkillGridIndicatorGroup;

    [SerializeField]
    private Button RegretButton;

    [SerializeField]
    private Button GiveUpButton;

    public Text MoveCountText;
    public Text RewardText;
    public Text CurrentCoinText;
    public Text CurrentLevelText;

    public GameObject InGameUI;

    public void AddSkill(SkillConfig skillConfig)
    {
        SkillButton sb = GameObjectPoolManager.Instance.PoolDict[GameObjectPoolManager.PrefabNames.SkillButton].AllocateGameObject<SkillButton>(SkillButtonContainer);
        sb.Initialize(skillConfig, false, () =>
        {
            if (GameStateManager.Instance.GetState() == GameState.Playing)
            {
                LevelManager.Instance.CurrentSelectedSkillKey = skillConfig.SkillKey;
            }
        });
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

    public void SelectButton(string skillKey)
    {
        foreach (KeyValuePair<string, SkillButton> kv in SkillButtonDict)
        {
            bool selected = kv.Key.Equals(skillKey);
            kv.Value.transform.localScale = selected ? 1.2f * Vector3.one : Vector3.one;
            kv.Value.ButtonBGImage.enabled = selected;
        }
    }
}