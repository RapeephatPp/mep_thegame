using UnityEngine;

// Enum ประกาศประเภทศัตรู
public enum EnemyType
{
    Normal,
    Speed,
    Tank,
    Flying,
    Bomber,
    Shieldler,
    Ranger,
}

[CreateAssetMenu(fileName = "New Enemy Data", menuName = "Game/Enemy Data")]
public class EnemySO : ScriptableObject
{
    [Header("General Stats")]
    public string enemyName;
    public EnemyType enemyType;
    public float maxHp = 100f;
    public float moveSpeed = 2f;
    public float damage = 10f;

    [Header("Attack Settings")]
    public float attackRange = 1.5f;
    public float attackCooldown = 1f;

    [Header("Special: Bomber")]
    public float explosionRadius = 3f;
    public int explosionDamage = 50;
    public GameObject explosionEffect; // ลาก Prefab ระเบิดมาใส่

    [Header("Special: Shieldler")]
    public float shieldHp = 50f;
    
    // Flying เราจะใช้เช็คจาก Type เอา หรือจะเพิ่ม bool แยกก็ได้
    public bool IsFlying => enemyType == EnemyType.Flying;
}