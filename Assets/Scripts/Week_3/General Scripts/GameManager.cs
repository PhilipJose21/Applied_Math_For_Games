using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager _instance;
    [SerializeField] private GameObject gameOverUI;
    [SerializeField] private GameObject winUI;
    [SerializeField] private GameObject finishArea;

    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {
        if (finishArea.GetComponent<TargetDetection>().IsTargetDetected())
        {
            FinishGame();
        }
    }

    public void GameOver()
    {
        gameOverUI.SetActive(true);
    }

    public void FinishGame()
    {
        winUI.SetActive(true);
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
