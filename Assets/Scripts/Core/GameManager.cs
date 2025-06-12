using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public bool IsGamePaused { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void StartGame()
    {
        IsGamePaused = false;
        Debug.Log("Game Started");
        // 게임 시작 초기화 코드
    }

    public void PauseGame()
    {
        IsGamePaused = true;
        Time.timeScale = 0f;
        Debug.Log("Game Paused");
    }

    public void ResumeGame()
    {
        IsGamePaused = false;
        Time.timeScale = 1f;
        Debug.Log("Game Resumed");
    }

    public void EndGame()
    {
        IsGamePaused = true;
        Debug.Log("Game Ended");
        // 게임 종료 처리
    }
}