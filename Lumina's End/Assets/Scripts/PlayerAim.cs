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
    [SerializeField] private Vector3 firePointLeftExtraOffset;
    
    
    [Header("Flashlight")]
    [SerializeField] private Transform flashLight;
    [SerializeField] private float flashLightAngleOffset = -90f;
    [SerializeField] private float flashLightDistanceFromFirePoint = 0.0f;
    
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
        if (arm == null) return;

        Vector3 mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouse.z = arm.position.z;

        // ทิศจากแขนไปหาเมาส์
        Vector2 dir = mouse - arm.position;

        // มุมจริงที่ชี้ไปหาเมาส์ (ไม่สนซ้ายขวา)
        float rawAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        bool isRight = (mouse.x > transform.position.x);

        // ---------- หมุนแขน + flip ตัวละคร ----------
        if (isRight)
        {
            armRenderer.sprite = armRightSprite;
            arm.rotation = Quaternion.Euler(0f, 0f, rawAngle);
            playerRenderer.flipX = false;
        }
        else
        {
            armRenderer.sprite = armLeftSprite;

            // sprite แขนซ้ายหันอีกทางเลยต้อง +180 ให้มันหันถูก
            arm.rotation = Quaternion.Euler(0f, 0f, rawAngle + 180f);
            playerRenderer.flipX = true;
        }

        // ---------- ขยับ FirePoint ----------
        if (firePoint != null)
        {
            // เริ่มจากตำแหน่งตอนหันขวา
            Vector3 localPos = firePointRightLocalPos;

            if (!isRight)
            {
                // กลับด้าน X
                localPos.x = -localPos.x;

                // แล้วบวก offset สำหรับฝั่งซ้าย
                localPos += firePointLeftExtraOffset;
            }

            firePoint.localPosition = localPos;
        }
        
        if (flashLight != null && firePoint != null)
        {
            // จุดเริ่มคือปากปืน
            Vector3 origin = firePoint.position;

            // ทิศจากปากปืน -> เมาส์
            Vector2 lightDir = (Vector2)(mouse - origin);
            if (lightDir.sqrMagnitude > 0.0001f)
            {
                lightDir.Normalize();

                // มุมจริง + มุม offset ที่ตั้งใน Inspector
                float lightAngle = Mathf.Atan2(lightDir.y, lightDir.x) * Mathf.Rad2Deg;
                flashLight.rotation = Quaternion.Euler(0f, 0f, lightAngle + flashLightAngleOffset);

                // ขยับตำแหน่งไฟฉายให้เลยปากปืนออกไปตามระยะที่กำหนด
                flashLight.position = origin + (Vector3)(lightDir * flashLightDistanceFromFirePoint);
            }
        }
        
    }

}