using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneFader : MonoBehaviour
{
    public static SceneFader Instance { get; private set; }

    [Header("Fade Settings")]
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private float defaultDuration = 0.5f;

    bool isFading = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (canvasGroup == null)
            canvasGroup = GetComponent<CanvasGroup>();
    }

    private void Start()
    {
        // เริ่มด้วยเฟดเข้าจากดำ → ใส
        if (canvasGroup != null && canvasGroup.alpha > 0f)
        {
            StartCoroutine(Fade(1f, 0f, defaultDuration));
        }
    }

    // เรียกจากที่อื่นเวลาจะเปลี่ยน scene
    public void FadeToScene(string sceneName, float duration = -1f)
    {
        if (isFading) return;
        if (duration <= 0f) duration = defaultDuration;

        StartCoroutine(FadeToSceneRoutine(sceneName, duration));
    }

    IEnumerator FadeToSceneRoutine(string sceneName, float duration)
    {
        isFading = true;

        // เฟดจอให้มืดก่อน
        yield return Fade(0f, 1f, duration);

        // โหลดฉาก
        AsyncOperation op = SceneManager.LoadSceneAsync(sceneName);
        op.allowSceneActivation = false;

        while (op.progress < 0.9f)
            yield return null;

        op.allowSceneActivation = true;
        yield return null; // รอ 1 เฟรมให้ฉากใหม่เซ็ตตัว

        // เฟดออกให้เห็นฉากใหม่
        yield return Fade(1f, 0f, duration);

        isFading = false;
    }

    IEnumerator Fade(float from, float to, float duration)
    {
        if (canvasGroup == null) yield break;

        canvasGroup.blocksRaycasts = true;
        float t = 0f;
        canvasGroup.alpha = from;

        while (t < duration)
        {
            t += Time.unscaledDeltaTime;
            float a = Mathf.Lerp(from, to, t / duration);
            canvasGroup.alpha = a;
            yield return null;
        }

        canvasGroup.alpha = to;
        canvasGroup.blocksRaycasts = (to > 0.01f);
    }
}
