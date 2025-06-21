using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public bool IsGamePaused { get; private set; }
    public int CurrentFloor { get; private set; }

    public UnityEvent onPause, onResume;

    private void Awake()
    {
        onPause = new UnityEvent();
        onResume = new UnityEvent();
        
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
        CurrentFloor = 1;
        SaveProgress();
        SceneManager.LoadScene("InGame");
    }
    
    public void ContinueGame()
    {
        IsGamePaused = false;
        CurrentFloor = PlayerPrefs.GetInt("CurrentFloor", 1);
        SceneManager.LoadScene("InGame");
    }

    public void PauseGame()
    {
        IsGamePaused = true;
        Time.timeScale = 0f;
        onPause?.Invoke();
    }

    public void ResumeGame()
    {
        IsGamePaused = false;
        Time.timeScale = 1f;
        onResume?.Invoke();
    }

    public void EndGame()
    {
        SaveProgress();
        IsGamePaused = false;
        SceneManager.LoadScene("Title");
    }

    public void ClearFloor()
    {
        ++CurrentFloor;
        SaveProgress();
        SceneManager.LoadScene("InGame");
        IsGamePaused = false;
    }

    public void SaveProgress()
    {
        PlayerPrefs.SetInt("CurrentFloor", CurrentFloor);
        PlayerPrefs.Save();
    }
}