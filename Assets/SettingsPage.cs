using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

public class SettingsPage : MonoBehaviour
{
    [SerializeField] private Slider bgmSlider, sfxSlider;
    [SerializeField] private Button english, korean;

    private IEnumerator Start()
    {
        yield return new WaitUntil(()=>AudioManager.Instance != null);
        
        // 슬라이더 초기화
        bgmSlider.value = AudioManager.Instance.GetBgmVolume();
        sfxSlider.value = AudioManager.Instance.GetSfxVolume();

        // 슬라이더 이벤트 등록
        bgmSlider.onValueChanged.AddListener(OnBgmVolumeChanged);
        sfxSlider.onValueChanged.AddListener(OnSfxVolumeChanged);

        english.onClick.AddListener(() => ChangeLocale("en"));
        korean.onClick.AddListener(() => ChangeLocale("ko-KR"));
    }

    private void OnBgmVolumeChanged(float value)
    {
        AudioManager.Instance.SetBgmVolume(value);
    }

    private void OnSfxVolumeChanged(float value)
    {
        AudioManager.Instance.SetSfxVolume(value);
    }

    private void ChangeLocale(string localeCode)
    {
        StartCoroutine(SetLocale(localeCode));
    }

    private IEnumerator SetLocale(string code)
    {
        yield return LocalizationSettings.InitializationOperation;

        var locales = LocalizationSettings.AvailableLocales.Locales;
        foreach (var locale in locales)
        {
            if (locale.Identifier.Code == code)
            {
                LocalizationSettings.SelectedLocale = locale;
                Debug.Log($"Locale changed to {code}");
                yield break;
            }
        }

        Debug.LogWarning($"Locale {code} not found.");
    }
}