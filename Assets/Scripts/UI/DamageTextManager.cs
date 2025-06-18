using UnityEngine;
using TMPro;
using Unity.VisualScripting;

public class DamageTextManager : MonoBehaviour
{
    public static DamageTextManager Instance;

    public GameObject damageTextPrefab;
    public Transform damageTextContainer;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;

        else
            Destroy(gameObject);
    }

    public void ShowDamage(long amount, Color color, Transform target, float yOffset = 0f)
    {
        // 데미지 텍스트 생성
        GameObject damageTextObj = Instantiate(damageTextPrefab, damageTextContainer);
        damageTextObj.GetComponent<DamageText>().Init(amount, color, target, yOffset);
    }
}