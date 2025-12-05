using UnityEngine;
using System.Collections;

public class CameraShake : MonoBehaviour
{
    public static CameraShake Instance;

    private Vector3 originalPos;

    void Awake()
    {
        Instance = this;
    }

    void OnEnable()
    {
        originalPos = transform.localPosition;
    }

    public IEnumerator Shake(float duration, float magnitude)
    {
        // แนะนำ: อัปเดตตำแหน่งเดิมล่าสุดทุกครั้งก่อนเริ่มสั่น เผื่อกล้องมีการขยับ
        originalPos = transform.localPosition; 
        
        float timer = 0f;

        while (timer < duration)
        {
            // ⭐ ส่วนที่เพิ่ม: เช็คว่าเกม Pause อยู่ไหม
            if (Time.timeScale == 0f)
            {
                // ดึงกล้องกลับมาที่เดิมให้ภาพนิ่งตอน Pause
                transform.localPosition = originalPos; 
                yield return null; // รอเฟรมถัดไป
                continue;          // ข้ามโค้ดข้างล่างไปเลย (ไม่เพิ่ม timer)
            }

            // โค้ดเดิม
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;

            transform.localPosition = originalPos + new Vector3(x, y, 0);

            timer += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = originalPos;
    }
}