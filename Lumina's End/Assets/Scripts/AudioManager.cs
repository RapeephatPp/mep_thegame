using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Sources")]
    [SerializeField] private AudioSource bgmSource;
    [SerializeField] private AudioSource sfxSource;

    private const string BGM_KEY = "vol_bgm";
    private const string SFX_KEY = "vol_sfx";

    private void Awake()
    {
        // ทำเป็น Singleton + อยู่ข้าม Scene
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        float bgm = PlayerPrefs.GetFloat(BGM_KEY, 0.8f);
        float sfx = PlayerPrefs.GetFloat(SFX_KEY, 0.8f);
        SetBGMVolume(bgm);
        SetSFXVolume(sfx);
    }

    // ------------ BGM ------------
    public void PlayBGM(AudioClip clip, bool loop = true)
    {
        if (!bgmSource || clip == null) return;

        // ถ้าเล่นเพลงเดิมอยู่แล้ว ไม่ต้องสวิตช์
        if (bgmSource.clip == clip && bgmSource.isPlaying)
            return;

        bgmSource.clip = clip;
        bgmSource.loop = loop;
        bgmSource.Play();
    }

    public void StopBGM()
    {
        if (bgmSource) bgmSource.Stop();
    }

    public void SetBGMVolume(float value)
    {
        if (!bgmSource) return;
        value = Mathf.Clamp01(value);
        bgmSource.volume = value;
        PlayerPrefs.SetFloat(BGM_KEY, value);
    }

    // ------------ SFX ------------
    public void PlaySFX(AudioClip clip, float pitchMin = 1f, float pitchMax = 1f)
    {
        if (clip == null || sfxSource == null) return;
        if (!sfxSource.isActiveAndEnabled)    // เพิ่มเช็คว่ามันถูก disable ไหม
        {
            Debug.LogWarning("SFX source is disabled or inactive on " + sfxSource.gameObject.name);
            return;
        }

        float p = Random.Range(pitchMin, pitchMax);
        sfxSource.pitch = p;
        sfxSource.PlayOneShot(clip);
    }

    public void SetSFXVolume(float value)
    {
        if (!sfxSource) return;
        value = Mathf.Clamp01(value);
        sfxSource.volume = value;
        PlayerPrefs.SetFloat(SFX_KEY, value);
    }
}