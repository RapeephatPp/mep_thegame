using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class MainMenuManager : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private CanvasGroup mainPanel;
    [SerializeField] private CanvasGroup optionsPanel;

    [Header("Scene")]
    [SerializeField] private string gameSceneName = "SampleScene";

    [Header("Audio (optional)")]
    [SerializeField] private AudioSource uiAudio;
    [SerializeField] private AudioClip hoverClip;
    [SerializeField] private AudioClip clickClip;

    [Header("Fade")]
    [SerializeField] private float fadeTime = 0.4f;

    void Start()
    {
        Time.timeScale = 1f;
        // เริ่มด้วย mainPanel เฟดเข้า
        ShowPanel(mainPanel, true, instant:true);
        StartCoroutine(FadeIn(mainPanel, fadeTime));
        if (optionsPanel) ShowPanel(optionsPanel, false, instant:true);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && optionsPanel && optionsPanel.gameObject.activeSelf)
        {
            OnBackFromOptions();
        }
    }

    // ---------- Buttons ----------
    public void OnStartGame()
    {
        Play(clickClip);
        if (SceneFader.Instance != null)
        {
            SceneFader.Instance.FadeToScene(gameSceneName);
        }
        else
        {
            // ถ้าไม่มี SceneFader (กันพัง) ใช้วิธีเดิม
            StartCoroutine(LoadGameFlow());
        }
    }

    public void OnOpenOptions()
    {
        Play(clickClip);
        StartCoroutine(SwapPanels(mainPanel, optionsPanel));
    }

    public void OnBackFromOptions()
    {
        Play(clickClip);
        StartCoroutine(SwapPanels(optionsPanel, mainPanel));
    }

    public void OnQuit()
    {
        Play(clickClip);
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    // ---------- Panel helpers ----------
    IEnumerator SwapPanels(CanvasGroup from, CanvasGroup to)
    {
        yield return FadeOut(from, fadeTime);
        ShowPanel(from, false);
        ShowPanel(to, true, instant:true);
        yield return FadeIn(to, fadeTime);
    }

    IEnumerator LoadGameFlow()
    {
        // เฟดทั้งหน้าแล้วค่อยโหลด
        yield return FadeOut(mainPanel, fadeTime);
        AsyncOperation op = SceneManager.LoadSceneAsync(gameSceneName);
        op.allowSceneActivation = false;

        // ทำ black fade (ถ้าอยากดำทั้งจอ ให้ทำ GameObject Image ดำเต็มจอ แล้วเฟด alpha 0→1)
        yield return new WaitForSeconds(0.15f);
        op.allowSceneActivation = true;
    }

    void ShowPanel(CanvasGroup cg, bool show, bool instant=false)
    {
        if (!cg) return;
        cg.gameObject.SetActive(show);
        cg.alpha = show ? 1f : 0f;
        cg.interactable = show;
        cg.blocksRaycasts = show;
        if (instant) return;
    }

    IEnumerator FadeIn(CanvasGroup cg, float t)
    {
        if (!cg) yield break;
        cg.gameObject.SetActive(true);
        cg.interactable = true; cg.blocksRaycasts = true;
        for (float a = cg.alpha; a < 1f; a += Time.unscaledDeltaTime / t)
        {
            cg.alpha = a; yield return null;
        }
        cg.alpha = 1f;
    }

    IEnumerator FadeOut(CanvasGroup cg, float t)
    {
        if (!cg) yield break;
        cg.interactable = false; cg.blocksRaycasts = false;
        for (float a = cg.alpha; a > 0f; a -= Time.unscaledDeltaTime / t)
        {
            cg.alpha = a; yield return null;
        }
        cg.alpha = 0f; cg.gameObject.SetActive(false);
    }

    void Play(AudioClip clip)
    {
        if (uiAudio && clip) uiAudio.PlayOneShot(clip);
    }

    // ---------- UI events (optional) ----------
    public void OnButtonHover() => Play(hoverClip);
    public void OnButtonClick() => Play(clickClip);
}
