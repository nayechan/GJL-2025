using System.Collections.Generic;
using UnityEngine;

public class WindowManager : MonoBehaviour
{
    public static WindowManager Instance { get; private set; }
    private Dictionary<string, Window> windows;
    
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        
        windows = new Dictionary<string, Window>();
        foreach (Transform child in transform)
        {
            Window window = child.GetComponent<Window>();
            if(window != null)
                windows[child.name] = window;
        }
    }

    public Window GetWindow(string _name)
    {
        return windows.GetValueOrDefault(_name, null);
    }
}