using UnityEngine;

public class PlayerAim : MonoBehaviour
{
    public Transform arm;                 
    public SpriteRenderer armRenderer;     
    public Sprite armRightSprite;          
    public Sprite armLeftSprite;           
    public SpriteRenderer playerRenderer;  

    void Update()
    {
        Vector3 mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 dir = mouse - arm.position;

        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        bool isLeft = (mouse.x < transform.position.x);

        if (isLeft)
        {
            // เปลี่ยน sprite แขนเป็นด้านซ้าย
            armRenderer.sprite = armLeftSprite;

            // หมุนแขนแบบกลับซ้าย (เพิ่ม 180 องศา)
            arm.rotation = Quaternion.Euler(0, 0, angle + 180f);

            // ให้ตัวละครหันซ้าย
            playerRenderer.flipX = true;
        }
        else
        {
            // ใช้ sprite แขนขวา
            armRenderer.sprite = armRightSprite;

            // หมุนปกติ
            arm.rotation = Quaternion.Euler(0, 0, angle);

            playerRenderer.flipX = false;
        }
    }
}