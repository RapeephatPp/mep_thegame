using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance;
    
    public float jumpForce = 7f;
    public LayerMask groundLayer;
    private bool isGrounded;
    public Transform groundCheck;
    
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;

    private Rigidbody2D rb;
    private Vector2 moveInput;
    private Camera cam;
    private WeaponSystem weapon;
    
    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        cam = Camera.main;
        weapon = GetComponent<WeaponSystem>();

        // อัปเดต UI ตอนเริ่ม
        if (PlayerHealth.Instance != null && UIManager.Instance != null)
            UIManager.Instance.UpdateHealthBar(PlayerHealth.Instance.CurrentHealth, PlayerHealth.Instance.MaxHealth);

        if (weapon != null && UIManager.Instance != null)
            UIManager.Instance.UpdateAmmo(weapon.MaxAmmo, weapon.MaxAmmo);
    }

    void Update()
    {
        HandleMovement();
        HandleAimingAndShooting();
        CheckGround();

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }

    }

    private void HandleMovement()
    {
        moveInput = new Vector2(Input.GetAxisRaw("Horizontal"), 0);
        rb.linearVelocity = new Vector2(moveInput.x * moveSpeed, rb.linearVelocity.y);

        // พลิกตัวหันซ้าย/ขวา
        if (moveInput.x > 0) transform.localScale = Vector3.one;
        else if (moveInput.x < 0) transform.localScale = new Vector3(-1, 1, 1);
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