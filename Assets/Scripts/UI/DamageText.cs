using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class DamageText : MonoBehaviour
{
    private Transform target;
    private Vector3 targetPos;
    public void Init(long amount, Color color, Transform _target, float yOffset)
    {
        // 텍스트 설정 (TextMeshPro 기준)
        var text = GetComponent<TMP_Text>();
        if (text != null)
        {
            text.text = amount.ToString();
            text.color = color;
        }
        
        target = _target;
        targetPos = target.position + yOffset * Vector3.up;
    }

    private void Update()
    {
        var text = GetComponent<TMP_Text>();
        Color color = text.color;

        color.a -= Time.deltaTime;
        
        if(color.a <= 0)
            Destroy(gameObject);
        
        text.color = color;
        transform.position = Camera.main.WorldToScreenPoint(targetPos);
    }
}
