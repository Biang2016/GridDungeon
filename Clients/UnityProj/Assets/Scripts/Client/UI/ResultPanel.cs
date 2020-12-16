using UnityEngine;
using System.Collections;
using BiangStudio.GamePlay.UI;
using UnityEngine.UI;

public class ResultPanel : BaseUIPanel
{
    [SerializeField]
    private Button GiveUpButton;

    [SerializeField]
    private Button ResumeButton;

    [SerializeField]
    private Text GiveUpText;

    [SerializeField]
    private Text FailedTipText;

    void Awake()
    {
        UIType.InitUIType(
            false,
            false,
            false,
            UIFormTypes.Normal,
            UIFormShowModes.Normal,
            UIFormLucencyTypes.Penetrable);

        GiveUpButton.onClick.AddListener(LoadResultDetails);
        ResumeButton.onClick.AddListener(CloseUIForm);
    }

    public void SetFailed()
    {
        GiveUpText.gameObject.SetActive(false);
        FailedTipText.gameObject.SetActive(true);
    }

    public void SetGiveUp()
    {
        GiveUpText.gameObject.SetActive(true);
        FailedTipText.gameObject.SetActive(false);
    }

    public override void Display()
    {
        base.Display();
        ResultDetails.SetActive(false);
    }

    [SerializeField]
    private GameObject ResultDetails;

    [SerializeField]
    private Text TotalSkillCountText;

    [SerializeField]
    private Text LevelsBeatText;

    [SerializeField]
    private Button RestartButton;

    public void LoadResultDetails()
    {
        ResultDetails.SetActive(true);
        TotalSkillCountText.text = $"You have got {LevelManager.Instance.MyCurrentSkills.Count} skills...";
        LevelsBeatText.text = $"and have passed {LevelManager.Instance.LevelCount} levels...";
        RestartButton.onClick.AddListener(ClientGameManager.Instance.ReloadGame);
    }

    public override void Hide()
    {
        base.Hide();
        GameStateManager.Instance.SetState(GameState.Playing);
    }
}