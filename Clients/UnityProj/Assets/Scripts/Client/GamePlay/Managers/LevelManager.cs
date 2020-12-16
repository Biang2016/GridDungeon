using System;
using System.Collections.Generic;
using System.IO;
using BiangStudio.GameDataFormat.Grid;
using BiangStudio.GamePlay.UI;
using BiangStudio.Singleton;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;
#if UNITY_EDITOR
using UnityEditor;

#endif

public class LevelManager : MonoSingleton<LevelManager>
{
    public Transform FragmentContainer;

    internal int CurrentLevelSideLength;
    internal bool[,] CurrentLevelMapData;
    internal bool[,] FragmentFrontMatrix;
    internal SquareFragment[,] FragmentMatrix;
    internal int SuccessFrontCount;

    internal int CurrentLevelIndex = 0;
    internal float CurrentScale = 1;

    public int StartCoins = 10;

    private int levelCount;

    public int LevelCount
    {
        get { return levelCount; }
        set
        {
            if (levelCount != value)
            {
                levelCount = value;
                SkillSelectPanel.CurrentLevelText.text = $"Level {levelCount}";
            }
        }
    }

    private int moveCount = -1;

    internal int MoveCount
    {
        get { return moveCount; }
        set
        {
            if (moveCount != value)
            {
                SkillSelectPanel.MoveCountText.text = $"Steps: {value}/{currentLevel.StepLimit}";
                moveCount = value;
            }
        }
    }

    private int currentFrontCount = -1;

    internal int CurrentFrontCount
    {
        get { return currentFrontCount; }
        set
        {
            if (currentFrontCount != value)
            {
                currentFrontCount = value;
            }
        }
    }

    private void LevelPass()
    {
        CurrentCoin += Reward;
        Reward = 0;
        CurrentSelectedSkillKey = String.Empty;
        GameStateManager.Instance.SetState(GameState.Waiting);
        NewSkillSelectPanel = UIManager.Instance.ShowUIForms<NewSkillSelectPanel>();
        NewSkillSelectPanel.RefreshSkillShop(currentLevel.ShopSkillItemCount);
        SkillSelectPanel.InGameUI.SetActive(false);
    }

    private int reward = -1;

    internal int Reward
    {
        get { return reward; }
        set
        {
            if (reward != value)
            {
                if (value >= 0)
                {
                    SkillSelectPanel.RewardText.text = $"Reward: {value}";
                }
                else
                {
                    SkillSelectPanel.RewardText.text = $"Punishment: {value}";
                }

                reward = value;
            }
        }
    }

    private int currentCoin = 0;

    public int CurrentCoin
    {
        get { return currentCoin; }
        set
        {
            if (currentCoin != value)
            {
                SkillSelectPanel.CurrentCoinText.text = $"Coins: {value}";
                NewSkillSelectPanel.RefreshAllSkillsAffordable();
                currentCoin = value;
            }
        }
    }

    [SerializeField]
    private LevelConfigContainer LevelConfigContainer;

    public SortedDictionary<int, LevelConfig> LevelConfigDict = new SortedDictionary<int, LevelConfig>(); // 干数据不修改

    [SerializeField]
    private List<SkillConfigContainer> SkillConfigContainers = new List<SkillConfigContainer>(); // 干数据不修改

    public Dictionary<string, SkillConfig> SkillConfigDict = new Dictionary<string, SkillConfig>(); // 干数据不修改

    [SerializeField]
    private List<SkillConfigContainer> StartSkillConfigContainers = new List<SkillConfigContainer>(); // 干数据不修改

    public Dictionary<string, SkillConfig> MyCurrentSkills = new Dictionary<string, SkillConfig>();

    private SkillGridIndicatorGroup SkillGridIndicatorGroup => SkillSelectPanel.SkillGridIndicatorGroup;

    private Stack<ArchivedStep> ArchivedSteps = new Stack<ArchivedStep>();

#if UNITY_EDITOR
    [Button("刷新数据")]
    public void RefreshData()
    {
        List<SkillConfigContainer> skillPrefabs = PrefabLoader.LoadAllPrefabsOfType<SkillConfigContainer>("Assets/Resources/Prefabs/Skills", SearchOption.AllDirectories);
        SkillConfigContainers = skillPrefabs;

        List<SkillConfigContainer> startSkillPrefabs = PrefabLoader.LoadAllPrefabsOfType<SkillConfigContainer>("Assets/Resources/Prefabs/Skills/StartSkills");
        StartSkillConfigContainers = startSkillPrefabs;
    }
#endif

    void Awake()
    {
        int levelIndex = 0;
        foreach (var lc in LevelConfigContainer.LevelConfigList)
        {
            levelIndex++;
            lc.LevelIndex = levelIndex;
            LevelConfigDict.Add(lc.LevelIndex, lc);
        }

        foreach (SkillConfigContainer scc in SkillConfigContainers)
        {
            SkillConfigDict.Add(scc.SkillConfig.SkillKey, scc.SkillConfig);
        }

        foreach (SkillConfigContainer scc in StartSkillConfigContainers)
        {
            MyCurrentSkills.Add(scc.SkillConfig.SkillKey, scc.SkillConfig);
        }
    }

    public void StartGame()
    {
        NewSkillSelectPanel = UIManager.Instance.ShowUIForms<NewSkillSelectPanel>();
        NewSkillSelectPanel.CloseUIForm();
        SkillSelectPanel = UIManager.Instance.ShowUIForms<SkillSelectPanel>();

        LoadSkills();
        CurrentCoin = StartCoins;
        LoadLevel(1);
    }

    private LevelConfig currentLevel;

    public void ClearLevel()
    {
        if (FragmentMatrix != null)
        {
            for (int col = 0; col < FragmentMatrix.GetLength(0); col++)
            {
                for (int row = 0; row < FragmentMatrix.GetLength(1); row++)
                {
                    FragmentMatrix[col, row].PoolRecycle();
                }
            }

            FragmentMatrix = null;
            CurrentFrontCount = 0;
        }

        SkillGridIndicatorGroup.Clear();
        ArchivedSteps.Clear();
        currentLevel = null;
        moveCount = -1;
        reward = -1;
    }

    private void LoadLevel(int levelIndex)
    {
        if (LevelConfigDict.TryGetValue(levelIndex, out LevelConfig levelConfig))
        {
            ClearLevel();
            currentLevel = levelConfig;
            CurrentLevelIndex = levelIndex;
            MoveCount = 0;
            Reward = levelConfig.StepLimit;
            CurrentLevelMapData = levelConfig.GetLevelMap();
            FragmentFrontMatrix = levelConfig.GetLevelMap();
            CurrentLevelSideLength = FragmentFrontMatrix.GetLength(0);
            CurrentScale = 21f / CurrentLevelSideLength;
            FragmentContainer.localScale = Vector3.one * CurrentScale;
            FragmentMatrix = new SquareFragment[CurrentLevelSideLength, CurrentLevelSideLength];
            SuccessFrontCount = levelConfig.SideLength * levelConfig.SideLength;
            CurrentFrontCount = 0;
            for (int col = 0; col < levelConfig.SideLength; col++)
            {
                for (int row = 0; row < levelConfig.SideLength; row++)
                {
                    if (FragmentFrontMatrix[col, row]) CurrentFrontCount++;
                    GenerateSquareFragment(col, row, FragmentFrontMatrix[col, row]);
                }
            }

            LevelCount++;
            GameStateManager.Instance.SetState(GameState.Playing);
        }
        else
        {
            LoadLevel(CurrentLevelIndex);
            GameStateManager.Instance.SetState(GameState.Playing);
        }

        SkillSelectPanel.InGameUI.SetActive(true);
    }

    public void LoadNextLevel()
    {
        LoadLevel(CurrentLevelIndex + 1);
    }

    private void GenerateSquareFragment(int col, int row, bool front)
    {
        SquareFragment fragment = GameObjectPoolManager.Instance.PoolDict[GameObjectPoolManager.PrefabNames.SquareFragment].AllocateGameObject<SquareFragment>(FragmentContainer);
        fragment.Initialize(new GridPos(col, row), front);
        Vector3 center = new Vector3((CurrentLevelSideLength) / 2f, -CurrentLevelSideLength / 2f, 0);
        fragment.transform.localPosition = new Vector3(col + 0.5f, -(CurrentLevelSideLength - row) + 0.5f) - center;
        FragmentMatrix[col, row] = fragment;
    }

    public void Update()
    {
        if (GameStateManager.Instance.GetState() == GameState.Playing)
        {
            //SkillGridIndicatorGroup.RectTransform.anchoredPosition = Input.mousePosition - new Vector3(Screen.width / 2f, Screen.height / 2f, 0);

            if (Input.GetKeyDown(KeyCode.R))
            {
                if (currentSkill != null)
                {
                    currentSkill.RotateOccupiedPositions();
                    SkillGridIndicatorGroup.Init(currentSkill.OccupiedPositions, SkillIndicatorGridSize, false, CurrentScale * SkillIndicatorScaleFactor);
                }
            }

            if ((!EventSystem.current.IsPointerOverGameObject() && Input.GetMouseButtonUp(0)) || Input.GetMouseButtonUp(1))
            {
                if (Reward + CurrentCoin > 0)
                {
                    Ray cursorRay = Camera.main.ScreenPointToRay(Input.mousePosition);
                    ArchivedStep archivedStep = new ArchivedStep();
                    if (currentSkill != null)
                    {
                        foreach (SkillConfig.SkillGrid sg in currentSkill.OccupiedPositions)
                        {
                            Ray ray = new Ray(cursorRay.origin + new Vector3(sg.GridPos.x, sg.GridPos.z, 0) * CurrentScale, cursorRay.direction);
                            if (Physics.Raycast(ray, out RaycastHit hit, 1000, LayerManager.Instance.LayerMask_Fragment))
                            {
                                FragmentCollider fc = hit.collider.gameObject.GetComponent<FragmentCollider>();
                                switch (sg.SkillGridType)
                                {
                                    case SkillGridType.Flip:
                                    {
                                        ArchivedStep.Step step = new ArchivedStep.Step(fc.SquireFragment.GridPos, fc.SquireFragment.Front);
                                        archivedStep.AllGridChanges.Add(step);
                                        fc.SquireFragment.FlipFragment();
                                        break;
                                    }
                                    case SkillGridType.TintWhite:
                                    {
                                        if (!fc.SquireFragment.Front)
                                        {
                                            ArchivedStep.Step step = new ArchivedStep.Step(fc.SquireFragment.GridPos, fc.SquireFragment.Front);
                                            archivedStep.AllGridChanges.Add(step);
                                            fc.SquireFragment.Front = true;
                                        }

                                        break;
                                    }
                                    case SkillGridType.TintBlack:
                                    {
                                        if (fc.SquireFragment.Front)
                                        {
                                            ArchivedStep.Step step = new ArchivedStep.Step(fc.SquireFragment.GridPos, fc.SquireFragment.Front);
                                            archivedStep.AllGridChanges.Add(step);
                                            fc.SquireFragment.Front = false;
                                        }

                                        break;
                                    }
                                }
                            }
                        }
                    }
                    else // no skill, can only flip one grid
                    {
                        if (Physics.Raycast(cursorRay, out RaycastHit hit, 1000, LayerManager.Instance.LayerMask_Fragment))
                        {
                            FragmentCollider fc = hit.collider.gameObject.GetComponent<FragmentCollider>();
                            ArchivedStep.Step step = new ArchivedStep.Step(fc.SquireFragment.GridPos, fc.SquireFragment.Front);
                            archivedStep.AllGridChanges.Add(step);
                            fc.SquireFragment.FlipFragment();
                        }
                    }

                    if (archivedStep.AllGridChanges.Count > 0)
                    {
                        CurrentSelectedSkillKey = String.Empty;
                        ArchivedSteps.Push(archivedStep);
                        MoveCount++;
                        Reward--;
                    }

                    if (CurrentFrontCount >= SuccessFrontCount)
                    {
                        LevelPass();
                    }
                }
                else
                {
                    Failed();
                }
            }

            if (Input.GetKeyUp(KeyCode.F9))
            {
                LoadNextLevel();
            }

            if (Input.GetKeyUp(KeyCode.Escape))
            {
                CurrentSelectedSkillKey = String.Empty;
            }
        }
    }

    public void RetractStep()
    {
        if (ArchivedSteps.Count > 0)
        {
            ArchivedStep archivedStep = ArchivedSteps.Pop();
            foreach (ArchivedStep.Step step in archivedStep.AllGridChanges)
            {
                FragmentMatrix[step.GP.x, step.GP.z].Front = step.BeforeIsFront;
            }

            MoveCount--;
            Reward++;
        }
    }

    #region SkillButton

    internal SkillSelectPanel SkillSelectPanel;

    public void LoadSkills()
    {
        foreach (KeyValuePair<string, SkillConfig> kv in MyCurrentSkills)
        {
            SkillSelectPanel.AddSkill(kv.Value.Clone());
        }
    }

    public void AddSkill(string skillKey)
    {
        SkillSelectPanel.AddSkill(SkillConfigDict[skillKey].Clone());
        MyCurrentSkills.Add(skillKey, SkillConfigDict[skillKey].Clone());
    }

    public float SkillIndicatorScaleFactor = 10f;
    public float SkillIndicatorGridSize = 0.6f;

    private SkillConfig currentSkill;

    internal string CurrentSelectedSkillKey
    {
        get { return currentSelectedSkillKey; }
        set
        {
            if (currentSelectedSkillKey != value)
            {
                currentSelectedSkillKey = value;
                if (SkillConfigDict.ContainsKey(value))
                {
                    currentSkill = SkillConfigDict[value].Clone();
                    SkillGridIndicatorGroup.Init(currentSkill.OccupiedPositions, SkillIndicatorGridSize, false, CurrentScale * SkillIndicatorScaleFactor);
                }
                else
                {
                    currentSkill = null;
                    SkillGridIndicatorGroup.Clear();
                }

                SkillSelectPanel.SelectButton(value);
            }
        }
    }

    private string currentSelectedSkillKey = String.Empty;

    #endregion

    #region NewSkillSelect

    internal NewSkillSelectPanel NewSkillSelectPanel;

    #endregion

    public void Failed()
    {
        ResultPanel resultPanel = UIManager.Instance.ShowUIForms<ResultPanel>();
        resultPanel.SetFailed();
        GameStateManager.Instance.SetState(GameState.ESC);
    }
}