using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Card : MonoBehaviour
{
    [SerializeField] public Image cardImageRenderer;        // ใส่ Image ของการ์ด
    [SerializeField] public TextMeshProUGUI cardTextRenderer; // ใส่ข้อความ/คำอธิบาย
    [SerializeField] private TextMeshProUGUI titleText;       // ถ้ามีชื่อการ์ด แยกจาก text

    private CardSO data;   

    public void Setup(CardSO data)
    {
        this.data = data;   
        cardImageRenderer.sprite = data.cardImage;
        cardTextRenderer.text = data.cardText;

        if (titleText != null)
            titleText.text = data.cardName;
    }

    public void OnCardSelected()
    {
        ApplyEffect();
        CardManager.Instance.OnCardPicked();
        Destroy(gameObject);
    }
    
    private void ApplyEffect()
    {
        switch (data.effectType)   
        {
            case CardEffectType.DamageIncrease:
                WeaponSystem.Instance.AddDamage((int)data.effectValue);
                break;

            case CardEffectType.HealthIncrease:
                PlayerHealth.Instance.AddMaxHealth((int)data.effectValue);
                break;

            case CardEffectType.FireRateIncrease:
                WeaponSystem.Instance.AddFireRate(data.effectValue);
                break;

            case CardEffectType.Heal:
                PlayerHealth.Instance.Heal((int)data.effectValue);
                break;
        }

        Debug.Log("Applied card: " + data.cardName);
    }
}