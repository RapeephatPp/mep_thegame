using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class OptionsManager : MonoBehaviour
{
    [SerializeField] private Slider bgmSlider;
    [SerializeField] private Slider sfxSlider;
    [SerializeField] private TextMeshProUGUI bgmLabel;
    [SerializeField] private TextMeshProUGUI sfxLabel;

    const string BGM_KEY = "vol_bgm";
    const string SFX_KEY = "vol_sfx";

    void OnEnable()
    {
        float bgm = PlayerPrefs.GetFloat(BGM_KEY, 1f);
        float sfx = PlayerPrefs.GetFloat(SFX_KEY, 1f);

        if (bgmSlider) bgmSlider.value = bgm;
        if (sfxSlider) sfxSlider.value = sfx;

        if (AudioManager.Instance)
        {
            AudioManager.Instance.SetBGMVolume(bgm);
            AudioManager.Instance.SetSFXVolume(sfx);
        }

        RefreshLabels();
    }

    public void OnChangeBGM(float v)
    {
        PlayerPrefs.SetFloat(BGM_KEY, v);
        if (AudioManager.Instance)
            AudioManager.Instance.SetBGMVolume(v);
        RefreshLabels();
    }

    public void OnChangeSFX(float v)
    {
        PlayerPrefs.SetFloat(SFX_KEY, v);
        if (AudioManager.Instance)
            AudioManager.Instance.SetSFXVolume(v);
        RefreshLabels();
    }


    void RefreshLabels()
    {
        if (bgmLabel) bgmLabel.text = $"BGM: {(int)((bgmSlider?.value ?? 0)*100)}%";
        if (sfxLabel) sfxLabel.text = $"SFX: {(int)((sfxSlider?.value ?? 0)*100)}%";
    }
}