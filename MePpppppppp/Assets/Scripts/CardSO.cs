using UnityEngine;

public enum CardEffectType
{
    DamageIncrease,
    HealthIncrease,
    FireRateIncrease,
    Heal,
    BulletRange,
    BulletSpeed,
    BaseMaxHp
    
}

[CreateAssetMenu(fileName = "New Card", menuName = "Card")]
public class CardSO : ScriptableObject
{
    public Sprite cardImage; //Image of Card
    public string cardName; // Name of the Card
    public string cardText; // Text on the Card

    public CardEffectType effectType;   // ✅ ประเภทเอฟเฟกต์
    public float effectValue;           // ✅ ค่าที่จะเพิ่มตามการ์ด
    public bool isUnique;               // ✅ ใช้ได้ครั้งเดียวหรือไม่
    public int unlockLevel;             // ✅ ปลดล็อกตอนเลเวลไหน

    public void ApplyCardEffect()
    {
        PlayerController player = FindFirstObjectByType<PlayerController>();
        WeaponSystem weapon = FindFirstObjectByType<WeaponSystem>();

        switch (effectType)
        {
            case CardEffectType.HealthIncrease:
                PlayerHealth.Instance.AddMaxHealth((int)effectValue);
                break;

            case CardEffectType.Heal:
                PlayerHealth.Instance.Heal((int)effectValue);
                break;

            case CardEffectType.DamageIncrease:
                weapon.AddDamage((int)effectValue);
                break;

            case CardEffectType.FireRateIncrease:
                weapon.AddFireRate(effectValue);
                break;
            
            case CardEffectType.BulletRange:
                //Add Effect
                break;
            case CardEffectType.BulletSpeed:
                //Add Effect
                break;
            case CardEffectType.BaseMaxHp:
                //Add Effect
                break;
        }

        Debug.Log("Applied card: " + cardName);
    }
}



/*public enum CardEffect
{
    DamageIncrease,
    HealIncrease,
    ShootingSpeed,
}*/