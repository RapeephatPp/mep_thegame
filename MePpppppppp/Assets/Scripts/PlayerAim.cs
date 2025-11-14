using UnityEngine;

public class PlayerAim : MonoBehaviour
{
    [Header("References")]
    public Transform arm;        // แขนของตัวละคร
    public Transform playerRoot; // ตัว Player หลัก

    void Update()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 dir = mousePos - arm.position;

        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        // หมุนแขน
        arm.rotation = Quaternion.Euler(0, 0, angle);

        // Flip ตัวละคร แต่ไม่ flip แขน
        if (angle > 90 || angle < -90)
        {
            // หันไปทางซ้าย
            playerRoot.localScale = new Vector3(-1, 1, 1);

            // แขนต้องกลับมาหันทางขวาตลอด (ป้องกันกลับหัว)
            arm.localScale = new Vector3(1, -1, 1); // พลิกแกน Y ของแขน
        }
        else
        {
            // หันขวา
            playerRoot.localScale = new Vector3(1, 1, 1);

            // แขนกลับมาเป็นปกติ
            arm.localScale = new Vector3(1, 1, 1);
        }
    }
}