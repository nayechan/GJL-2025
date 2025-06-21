using System;
using System.Collections.Generic;
using UnityEngine;

public class Window : MonoBehaviour
{
    private Dictionary<string, object> values;
    [SerializeField] private bool shouldPause = false;

    private void Awake()
    {
        values = new Dictionary<string, object>();
    }

    public object GetValue(string key)
    {
        return values.GetValueOrDefault(key, null);
    }

    public T GetValue<T>(string key)
    {
        return values.TryGetValue(key, out var val) && val is T casted ? casted : default;
    }

    public void SetValue(string key, object value)
    {
        values[key] = value;
    }

    public void Open()
    {
        gameObject.SetActive(true);
        if(shouldPause)
            GameManager.Instance.PauseGame();
    }

    public void Close()
    {
        gameObject.SetActive(false);
        if(shouldPause)
            GameManager.Instance.ResumeGame();
    }
}