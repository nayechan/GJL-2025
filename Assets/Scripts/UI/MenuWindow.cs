using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuWindow : Window
{
    [SerializeField] private Window statWindow;
    
    public void OpenStatPage()
    {
        statWindow.Open();
        gameObject.SetActive(false);
        IsOpen = false;
    }
}
