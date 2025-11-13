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
        float bgm = PlayerPrefs.GetFloat(BGM_KEY, 0.8f);
        float sfx = PlayerPrefs.GetFloat(SFX_KEY, 0.8f);
        if (bgmSlider) bgmSlider.value = bgm;
        if (sfxSlider) sfxSlider.value = sfx;
        RefreshLabels();
        AudioListener.volume = bgm; // ตัวอย่างง่าย ๆ ใช้รวม ถ้าจะละเอียดแยก Mixer ค่อยต่อยอด
    }

    public void OnChangeBGM(float v)
    {
        PlayerPrefs.SetFloat(BGM_KEY, v);
        AudioListener.volume = v;
        RefreshLabels();
    }

    public void OnChangeSFX(float v)
    {
        PlayerPrefs.SetFloat(SFX_KEY, v);
        RefreshLabels();
    }

    void RefreshLabels()
    {
        if (bgmLabel) bgmLabel.text = $"BGM: {(int)((bgmSlider?.value ?? 0)*100)}%";
        if (sfxLabel) sfxLabel.text = $"SFX: {(int)((sfxSlider?.value ?? 0)*100)}%";
    }
}