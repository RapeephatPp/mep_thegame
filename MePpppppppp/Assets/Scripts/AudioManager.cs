using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Sources")]
    [SerializeField] private AudioSource bgmSource;
    [SerializeField] private AudioSource sfxSource;

    const string BGM_KEY = "vol_bgm";
    const string SFX_KEY = "vol_sfx";

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        // โหลดค่า volume จาก PlayerPrefs
        float bgm = PlayerPrefs.GetFloat(BGM_KEY, 1f);
        float sfx = PlayerPrefs.GetFloat(SFX_KEY, 1f);
        SetBGMVolume(bgm);
        SetSFXVolume(sfx);
    }

    // --------- BGM ----------
    public void PlayBGM(AudioClip clip, bool loop = true)
    {
        if (!bgmSource || clip == null) return;

        if (bgmSource.clip == clip && bgmSource.isPlaying) return;

        bgmSource.clip = clip;
        bgmSource.loop = loop;
        bgmSource.Play();
    }

    public void StopBGM()
    {
        if (bgmSource) bgmSource.Stop();
    }

    public void SetBGMVolume(float v)
    {
        if (!bgmSource) return;
        v = Mathf.Clamp01(v);
        bgmSource.volume = v;
        PlayerPrefs.SetFloat(BGM_KEY, v);
    }

    // --------- SFX ----------
    public void PlaySFX(AudioClip clip)
    {
        if (!sfxSource || clip == null) return;
        sfxSource.PlayOneShot(clip);      // ใช้ volume จาก sfxSource.volume
    }

    public void SetSFXVolume(float v)
    {
        if (!sfxSource) return;
        v = Mathf.Clamp01(v);
        sfxSource.volume = v;
        PlayerPrefs.SetFloat(SFX_KEY, v);
    }
}