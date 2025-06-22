using System;
using UnityEngine;
using UnityEngine.UI;

public class KarmaGaugeIndicator : MonoBehaviour
{
    [SerializeField] private Image image;
    private void Start()
    {
        GameManager.Instance.onKarmaChange.AddListener(OnKarmaChange);
        OnKarmaChange();
    }

    public void OnKarmaChange()
    {
        float fillAmount = Mathf.Clamp01(GameManager.Instance.KarmaGauge / 100.0f);
        image.fillAmount = fillAmount;
    }
}