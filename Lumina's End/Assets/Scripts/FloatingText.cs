using System.Collections;
using UnityEngine;
using TMPro;

public class FloatingText : MonoBehaviour
{
    [Header("UI Components")]
    [SerializeField] private TextMeshProUGUI tmpText;
    [SerializeField] private CanvasGroup canvasGroup;

    [Header("Settings")]
    [SerializeField] private float typingSpeed = 0.05f;
    [SerializeField] private float letterBounceAmount = 1.2f; // ขยายใหญ่ตอนตัวอักษรโผล่
    [SerializeField] private Vector3 offset = new Vector3(0, -1f, 0); //position
    
    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip popSound;

    private Transform targetFollow;
    private Coroutine displayRoutine;

    public void Show(string message, Transform target)
    {
        targetFollow = target;
        gameObject.SetActive(true);
        
        if (displayRoutine != null) StopCoroutine(displayRoutine);
        displayRoutine = StartCoroutine(AnimateText(message));
    }

    public void Hide()
    {
        // Fade Out แล้วปิด
        StartCoroutine(FadeOut());
    }

    private IEnumerator AnimateText(string message)
    {
        tmpText.text = message;
        tmpText.maxVisibleCharacters = 0; // เริ่มจากไม่เห็นตัวอักษรเลย
        canvasGroup.alpha = 1;

        int totalChars = message.Length;

        for (int i = 0; i <= totalChars; i++)
        {
            tmpText.maxVisibleCharacters = i;

            // --- Juicy Effect: เด้งดึ๋ง! ---
            if (i > 0)
            {
                // เล่นเสียง Pop
                if (audioSource && popSound) 
                    audioSource.PlayOneShot(popSound, Random.Range(0.8f, 1.2f)); // สุ่ม Pitch นิดหน่อยให้ไม่น่าเบื่อ

                // กระตุก Scale เล็กน้อยให้ดูมีแรง
                StartCoroutine(PunchScale());
            }

            yield return new WaitForSeconds(typingSpeed);
        }
    }

    private IEnumerator PunchScale()
    {
        // ขยายวูบเดียวแล้วกลับมาปกติ
        transform.localScale = Vector3.one * letterBounceAmount;
        float t = 0;
        while (t < 1)
        {
            t += Time.deltaTime * 10f; // ความเร็วในการหดกลับ
            transform.localScale = Vector3.Lerp(transform.localScale, Vector3.one, t);
            yield return null;
        }
        transform.localScale = Vector3.one;
    }

    private IEnumerator FadeOut()
    {
        float t = 1;
        while (t > 0)
        {
            t -= Time.deltaTime * 2f;
            canvasGroup.alpha = t;
            yield return null;
        }
        gameObject.SetActive(false);
    }

    private void LateUpdate()
    {
        // ลอยตามเป้าหมาย (Player)
        if (targetFollow != null)
        {
            transform.position = targetFollow.position + offset;
        }
    }
}