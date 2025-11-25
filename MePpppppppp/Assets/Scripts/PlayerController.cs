using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance;
    
    public float jumpForce = 7f;
    public LayerMask groundLayer;
    public Transform groundCheck;
    
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;
    
    [Header("Mode")]
    [SerializeField] private bool canShoot = true;  
    
    private bool isGrounded;
    private Animator animator;
    private Rigidbody2D rb;
    private Camera cam;
    private WeaponSystem weapon;
    SpriteRenderer sr;
    
    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        cam = Camera.main;
        sr = GetComponent<SpriteRenderer>();
        weapon = GetComponent<WeaponSystem>();
        

        // อัปเดต UI ตอนเริ่ม
        if (PlayerHealth.Instance != null && UIManager.Instance != null)
            UIManager.Instance.UpdateHealthBar(PlayerHealth.Instance.CurrentHealth, PlayerHealth.Instance.MaxHealth);

        if (weapon != null && UIManager.Instance != null)
            UIManager.Instance.UpdateAmmo(weapon.MaxAmmo, weapon.MaxAmmo);
    }

    void Update()
    {   
        float move = Input.GetAxisRaw("Horizontal");

        // animation
        bool IsWalking = Mathf.Abs(move) > 0.01f;
        animator.SetBool("IsWalking", IsWalking);

        /*// เดินจริง ๆ
         
        Debug.Log("IsWalking = " + IsWalking);*/
        
        if (canShoot)
        {
            HandleAimingAndShooting();   // เฉพาะตัวที่ยิงได้เท่านั้น
        }
        
        HandleMovement(move);
        CheckGround();
        
    }
    
    private void HandleMovement(float move)
    {
        rb.linearVelocity = new Vector2(move * moveSpeed, rb.linearVelocity.y);
        
        if (!canShoot && sr != null)
        {
            if (move > 0.01f)
            {
                sr.flipX = false;   // หันขวา
            }
            else if (move < -0.01f)
            {
                sr.flipX = true;    // หันซ้าย
            }
        }
    }

    
    private void HandleAimingAndShooting()
    {
        if (weapon == null)
        {
            Debug.LogWarning("WeaponSystem not found on Player!");
            return;
        }

        // เมาส์ใน world space
        Vector3 mouseWorld3 = cam.ScreenToWorldPoint(Input.mousePosition);
        Vector2 mouseWorld2 = new Vector2(mouseWorld3.x, mouseWorld3.y);

        // ⭐ ใช้ตำแหน่งปากปืนเป็นต้นทิศ
        Vector2 firePos = (Vector2)weapon.FirePoint.position;
        Vector2 direction = (mouseWorld2 - firePos).normalized;

        if (Input.GetMouseButton(0))
        {
            weapon.Fire(direction);
        }
    }
    
    //Jummp
    void CheckGround()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
    }
    
}