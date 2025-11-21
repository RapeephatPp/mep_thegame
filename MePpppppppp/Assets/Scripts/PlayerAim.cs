using UnityEngine;

public class PlayerAim : MonoBehaviour
{
    public Transform arm;                 
    public SpriteRenderer armRenderer;     
    public Sprite armRightSprite;          
    public Sprite armLeftSprite;           
    public SpriteRenderer playerRenderer;  
    
    [Header("Fire Point")]
    [SerializeField] private Transform firePoint;   // จุดยิง (ลูกกระสุน spawn)
    private Vector3 firePointRightLocalPos;   
    
    [Tooltip("Left offset")]
    [SerializeField] private Vector3 firePointLeftExtraOffset;// localPosition ตอนหันขวา
    
    
    private void Start()
    {
        
        if (firePoint != null)
        {
            // จำตำแหน่ง firePoint ตอนหันขวาไว้เป็นค่าอ้างอิง
            firePointRightLocalPos = firePoint.localPosition;
        }
    }
    
    void Update()
    {
        Vector3 mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 dir = mouse - arm.position;

        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        bool isRight = (mouse.x > transform.position.x);

        if (isRight)
        {   
            // ใช้ sprite แขนขวา
            armRenderer.sprite = armRightSprite;

            // หมุนปกติ
            arm.rotation = Quaternion.Euler(0, 0, angle);

            playerRenderer.flipX = false;
        }
        else
        {
            // เปลี่ยน sprite แขนเป็นด้านซ้าย
            armRenderer.sprite = armLeftSprite;

            // หมุนแขนแบบกลับซ้าย (เพิ่ม 180 องศา)
            arm.rotation = Quaternion.Euler(0, 0, angle + 180f);

            // ให้ตัวละครหันซ้าย
            playerRenderer.flipX = true;
        }
        
        if (firePoint != null)
        {
            // เริ่มจากตำแหน่งตอนหันขวา
            Vector3 localPos = firePointRightLocalPos;

            if (!isRight)
            {
                // กลับด้าน X ก่อน
                localPos.x = -localPos.x;

                // ⭐ แล้วบวก offset พิเศษสำหรับฝั่งซ้าย
                localPos += firePointLeftExtraOffset;
            }

            firePoint.localPosition = localPos;
        }
    }
}