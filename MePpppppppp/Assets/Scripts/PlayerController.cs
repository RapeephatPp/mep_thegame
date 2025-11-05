using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance;

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
    }

    void Update()
    {
        HandleMovement();
        HandleAimingAndShooting();
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

        Vector3 mouseWorldPos = cam.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = (mouseWorldPos - transform.position).normalized;

        if (Input.GetMouseButton(0))
        {
            weapon.Fire(direction);
        }
    }

    // สำหรับระบบอัปเกรด
    public WeaponSystem GetWeaponSystem() => weapon;
}