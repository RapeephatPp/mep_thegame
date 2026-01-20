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
        
        /* Is really Walk Check
        Debug.Log("IsWalking = " + IsWalking);*/
        
        if (canShoot)
        {
            HandleAimingAndShooting(); // Only Shootable
            // R to Reload
            if (Input.GetKeyDown(KeyCode.R) && weapon != null)
            {
                weapon.TryManualReload();
            }
        }
        
        HandleMovement(move);
        CheckGround();
        
    }
    
    private void HandleMovement(float move)
    {
        rb.linearVelocity = new Vector2(move * moveSpeed, rb.linearVelocity.y);
        
        if (Mathf.Abs(move) > 0.1f && TutorialManager.Instance != null)
        {
            TutorialManager.Instance.OnPlayerMove();
        }
        
        if (!canShoot)
        {
            // ใช้การ Scale ติดลบเพื่อกลับด้านทั้ง Object
            if (move > 0.01f)
            {
                transform.localScale = new Vector3(1, 1, 1); // หันขวา (ปกติ)
            }
            else if (move < -0.01f)
            {
                transform.localScale = new Vector3(-1, 1, 1); // หันซ้าย (กลับด้าน)
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
            
            // --- เพิ่มตรงนี้: บอก Tutorial ว่ายิงแล้ว ---
            if (TutorialManager.Instance != null)
            {
                TutorialManager.Instance.OnPlayerShoot();
            }
            // ----------------------------------------
            
        }
    }
    
    //Jummp
    void CheckGround()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
    }
    
}