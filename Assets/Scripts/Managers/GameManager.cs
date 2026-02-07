using System;
using System.Collections.Generic;
using UnityEngine;

public enum GameStateType
{
    None = 0,
    Intro = 1,
    Play = 2,
    Result = 3,
}

public class GameManager : MonoBehaviour
{
    static GameManager instance;
    static GameManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<GameManager>();
                if (instance == null)
                {
                    Debug.LogError("[GameManager] 场景中没有 GameManager！");
                }
            }
            return instance;
        }
    }

    [System.Serializable]
    public class GameObjectPart
    {
        public string name;
        public GameObject obj;
    }

    [Header("对象配置")]
    [SerializeField] private GameObjectPart[] gameObjectParts;
    private Dictionary<string, GameObjectPart> gameObjectPartDictionary;

    [Header("关卡积分需求阈值")]
    [SerializeField] private int[] scoreRequireParts;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        InitializeGameObjectParts();
        InitializeGameSettings();
    }

    void InitializeGameObjectParts()
    {
        gameObjectPartDictionary = new Dictionary<string, GameObjectPart>();

        foreach (var part in gameObjectParts)
        {
            if (!string.IsNullOrEmpty(part.name) && part.obj != null)
            {
                gameObjectPartDictionary[part.name] = part;
            }
        }
    }

    void InitializeGameSettings()
    {
        Time.fixedDeltaTime = 1.0f / 60.0f;
    }

    private static GameStateType CurrentGameState = GameStateType.None;
    public static void ChangeGameState(GameStateType target)
    {
        if (target != CurrentGameState)
        {
            OnGameStateChanged(CurrentGameState, target);
        }
    }
    public static GameStateType GetCurrentGameState()
    {
        return CurrentGameState;
    }

    private static int CurrentScore = 0;
    private static int CurrentLevel = 1;

    public static void IncreaseCurrentScore(int v)
    {
        CurrentScore += v;
        Instance.OnScoreChanged();

        int i2 = UnityEngine.Random.Range(1, 2);
        string sound = $"bird/point-{i2}";
        AudioManager.PlaySFX(sound);

        GameObject.Find("ScoreController").GetComponent<ScoreController>().SetValue(GameManager.GetCurrentScore());
    }

    public static int GetCurrentScore()
    {
        return CurrentScore;
    }

    public static int GetCurrentLevel()
    {
        return CurrentLevel;
    }

    void Start()
    {
        if (CurrentGameState == GameStateType.None)
        {
            ChangeGameState(GameStateType.Intro);
        }
    }

    static void ResetStates()
    {
        CurrentScore = 0;
        CurrentLevel = 1;
        Debug.Log("New Level! -> " + 1);
    }

    static void OnGameStateChanged(GameStateType gs_old, GameStateType gs_new)
    {
        GameObjectPart part;

        CurrentGameState = gs_new;
        Debug.Log("Change GameState: " + Type.GetType("GameStateType").GetEnumName(gs_new));

        if (gs_old == GameStateType.Intro)
        {
            part = Instance.gameObjectPartDictionary["part/intro"];
            part.obj.SetActive(false);
        }

        if (gs_new == GameStateType.Intro)
        {
            ResetStates();

            part = Instance.gameObjectPartDictionary["part/intro"];
            part.obj.SetActive(true);

            part = Instance.gameObjectPartDictionary["part/game"];
            part.obj.SetActive(false);

            FindObjectOfType<GUIManager>().ShowGameOverPanel(false);

            part = Instance.gameObjectPartDictionary["core/score"];
            part.obj.SetActive(false);

            part = Instance.gameObjectPartDictionary["core/pipe"];
            part.obj.GetComponent<PipeSystem>().CleanAll();
        }

        if (gs_new == GameStateType.Play)
        {
            part = Instance.gameObjectPartDictionary["part/game"];
            part.obj.SetActive(true);

            FindObjectOfType<BirdController>().ResetBirdPosition();

            part = Instance.gameObjectPartDictionary["core/score"];
            part.obj.GetComponent<ScoreController>().SetValue(0);
            part.obj.SetActive(true);

            Time.timeScale = 1f;
        }

        if (gs_new == GameStateType.Result)
        {
            Time.timeScale = 0f;

            FindObjectOfType<GUIManager>().ShowGameOverPanel(true);
        }
    }

    void OnScoreChanged()
    {
        int n = scoreRequireParts.Length;
        int level = CurrentLevel;
        int i = level - 1;
        if (i <= n - 1)
        {
            int next = scoreRequireParts[i];
            int score = CurrentScore;
            if (score >= next)
            {
                level += 1;
                CurrentLevel = level;
                OnLevelChanged(level);
            }
        }
        else
        {
            // skip
        }
    }

    void OnLevelChanged(int level)
    {
        Debug.Log("Meet New Level! -> " + level);
    }

    public static bool GetPaused()
    {
        return CurrentGameState == GameStateType.Result;
    }
    public static void Failed()
    {
        ChangeGameState(GameStateType.Result);
    }
}
