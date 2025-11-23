using UnityEngine;

public enum CardEffectType
{
    DamageIncrease,
    HealthIncrease,
    FireRateIncrease,
    Heal,
    BulletRange,
    BulletSpeed,
    BaseHealthIncrease,
    AmmoCapacity,
    
    
}

[CreateAssetMenu(fileName = "New Card", menuName = "Card")]
public class CardSO : ScriptableObject
{
    public Sprite cardImage; //Image of Card
    public string cardName; // Name of the Card
    public string cardText; // Text on the Card

    public CardEffectType effectType;   // Effect type of the card
    public float effectValue;           // Effect value of the card
    public bool isUnique;               // Only find One
    public int unlockLevel;             // Unlock which level

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
                BulletController.AddLifetime((int)effectValue);
                break;
            case CardEffectType.BulletSpeed:
                WeaponSystem.Instance.AddBulletSpeed((int)effectValue);
                break;
            case CardEffectType.BaseHealthIncrease:
                BaseController.Instance.AddMaxHealth((int)effectValue);
                break;
            case CardEffectType.AmmoCapacity:
                WeaponSystem.Instance.AddAmmoCapacity((int)effectValue);
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