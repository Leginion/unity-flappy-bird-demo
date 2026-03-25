using UnityEngine;
using UnityEngine.UI;

public class GUIManager : MonoBehaviour
{
    [SerializeField] private GameObject GameOverPanel;
    [SerializeField] private MedalController medalController;
    [SerializeField] private ScoreController scoreMain;
    [SerializeField] private ScoreController scoreNow;
    [SerializeField] private ScoreController scoreBest;
    [SerializeField] private Button playAgain;
    [SerializeField] private GameObject newBest;

    void Awake()
    {
        playAgain.onClick.AddListener(OnClickPlayAgain);
    }

    public void ShowGameOverPanel(bool is_show)
    {
        GameOverPanel.SetActive(is_show);
        scoreMain.gameObject.SetActive(!is_show);

        if (is_show)
        {
            int score = GameManager.GetCurrentScore();
            int best = GameManager.GetBestScore();

            int level = Mathf.Min(4, score / 5);
            medalController.SetMedalId(level);

            scoreNow.SetValue(score);
            scoreBest.SetValue(Mathf.Max(score, best));

            if (score > best)
            {
                newBest.SetActive(true);
                GameManager.SetBestScore(score);
            }
            else
            {
                newBest.SetActive(false);
            }
        }
    }

    private void Update()
    {
        if (GameOverPanel.activeSelf)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                GameManager.ChangeGameState(GameStateType.Intro);
            }
        }
    }

    private void OnClickPlayAgain()
    {
        GameManager.ChangeGameState(GameStateType.Intro);
    }
}