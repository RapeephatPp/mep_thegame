using UnityEngine;

public class PlayerHealth : CharacterBase, IDamageable
{
    protected override void Die()
    {
        Debug.Log("Player Died!");
        GameManager.Instance.OnPlayerDeath();
    }
}