using UnityEngine;

public enum CardEffectType
{
    DamageIncrease,
    HealthIncrease,
    FireRateIncrease,
    Heal,
    BaseHeal,
    BulletRange,
    BulletSpeed,
    BaseHealthIncrease,
    AmmoCapacity,
    ReloadSpeed,
    VampireShot,
    ShockRound,
    AdrenalineRush,
    
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
            
            case CardEffectType.BaseHeal:
                BaseController.Instance.HealBase((int)effectValue);
                break;

            case CardEffectType.DamageIncrease:
                weapon.AddDamage((int)effectValue);
                break;

            case CardEffectType.FireRateIncrease:
                weapon.AddFireRate(effectValue);
                break;
            
            case CardEffectType.BulletRange:
                BulletController.AddLifetime(effectValue);
                break;
            
            case CardEffectType.BulletSpeed:
                weapon.AddBulletSpeed(effectValue);
                break;
            
            case CardEffectType.BaseHealthIncrease:
                BaseController.Instance.AddMaxHealth((int)effectValue);
                break;
            
            case CardEffectType.AmmoCapacity:
                weapon.AddAmmoCapacity((int)effectValue);
                break;
            
            case CardEffectType.ReloadSpeed:
                weapon.AddReloadSpeed((int)effectValue);
                break;
            
            case CardEffectType.VampireShot:
                weapon.EnableVampireShot((int)effectValue);
                break;
            
            case CardEffectType.AdrenalineRush:
                weapon.EnableAdrenalineRush(effectValue, 3f); // multiplier = effectValue, 3 s
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