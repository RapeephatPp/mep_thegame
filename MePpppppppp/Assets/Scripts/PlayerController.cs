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

        // ส่งค่า IsWalking เข้า Animator
        bool IsWalking = Mathf.Abs(move) > 0.01f;
        animator.SetBool("IsWalking", IsWalking);

        // เดินจริง ๆ
        Debug.Log("IsWalking = " + IsWalking);
        rb.linearVelocity = new Vector2(move * moveSpeed, rb.linearVelocity.y);

        HandleMovement();
        HandleAimingAndShooting();
        CheckGround();
        
    }
    
    private void HandleMovement()
    {
        float x = Input.GetAxisRaw("Horizontal");
        rb.linearVelocity = new Vector2(x * moveSpeed, rb.linearVelocity.y);
    }

    
    private void HandleAimingAndShooting()
    {
        if (weapon == null)
        {
            Debug.LogWarning("WeaponSystem not found on Player!");
            return;
        }

        // แปลงตำแหน่งเมาส์เป็น world space แล้วดึงเฉพาะ x,y เป็น Vector2
        Vector3 mouseWorld3 = cam.ScreenToWorldPoint(Input.mousePosition);
        Vector2 mouseWorld2 = new Vector2(mouseWorld3.x, mouseWorld3.y);

        // คำนวณทิศทางโดยใช้ 2D vectors เท่านั้น (จะไม่ถูก normalize ด้วย z component อีกต่อไป)
        Vector2 direction = (mouseWorld2 - (Vector2)transform.position).normalized;

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