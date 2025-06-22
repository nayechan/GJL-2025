using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Serializable]
    struct FloorAudioData
    {
        public AudioClip clip;
        public int minFloor, maxFloor; // Main = 0
    }
    
    [SerializeField] private float karmaThreshold = 100.0f;
    [SerializeField] private float maxKarma = 120.0f;
    [SerializeField] private List<FloorAudioData> floorBgm;
    
    public static GameManager Instance { get; private set; }

    public bool IsGamePaused { get; private set; }
    private int currentFloor = 0;
    public int CurrentFloor
    {
        get => currentFloor;
        private set
        {
            currentFloor = value;
            OnFloorChange();
        }
    }

    private float karmaGauge = 0.0f;
    public bool IsKarmaOverflown { get; private set; } = false;
    
    public float KarmaGauge
    {
        get => karmaGauge;
        set
        {
            if (karmaGauge == value) return;
            
            karmaGauge = Mathf.Clamp(value, 0, maxKarma); // 최대값 제한
            onKarmaChange?.Invoke();

            if (karmaGauge >= karmaThreshold && !IsKarmaOverflown)
            {
                onKarmaOverflow?.Invoke();
                IsKarmaOverflown = true;
            }

            if (karmaGauge < karmaThreshold && IsKarmaOverflown)
                IsKarmaOverflown = false;
        } 
    }

    public UnityEvent onPause, onResume;
    public UnityEvent onKarmaChange, onKarmaOverflow;

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
        
        OnFloorChange();
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
        Time.timeScale = 1f;
        KarmaGauge = 0;
        SaveProgress();
        IsGamePaused = false;
        CurrentFloor = 0;
        SceneManager.LoadScene("Title");
    }

    public void ClearFloor()
    {
        ++CurrentFloor;
        KarmaGauge -= 30;
        SaveProgress();
        SceneManager.LoadScene("InGame");
        IsGamePaused = false;
    }

    public void SaveProgress()
    {
        PlayerPrefs.SetInt("CurrentFloor", CurrentFloor);
        PlayerPrefs.Save();
    }

    public void OnFloorChange()
    {
        AudioClip _floorBgm = floorBgm
            .First(audioData => audioData.minFloor <= CurrentFloor && CurrentFloor <= audioData.maxFloor)
            .clip;
        
        AudioManager.Instance.PlayBGM(_floorBgm);
    }
}