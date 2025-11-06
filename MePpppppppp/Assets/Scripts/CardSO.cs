using UnityEngine;
    
[CreateAssetMenu(fileName = "New Card", menuName = "Card")]
public class CardSO : ScriptableObject
{
    public Sprite cardImage; //Image of Card
    public string cardText; // Text on the Card
    public CardEffect effectType; // The Effect
    public float effectValue; // The Value of the Effect, (10%)
    public bool isUnique; // If unique, the card will not be randomized again if it's already selected.
    public int unlockLevel;
}

public enum CardEffect
{
    DamageIncrease,
    HealIncrease,
    ShootingSpeed,
}