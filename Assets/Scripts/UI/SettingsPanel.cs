using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingsPanel : MonoBehaviour
{
    [SerializeField] Slider musicSlider;
    [SerializeField] Slider sfxSlider;
    [SerializeField] TMP_Text musicValue;
    [SerializeField] TMP_Text sfxValue;
    [SerializeField] TMP_Text savePathText;

    public void Refresh()
    {
        if (musicSlider != null) musicSlider.SetValueWithoutNotify(SaveSystem.Data.musicVolume);
        if (sfxSlider != null) sfxSlider.SetValueWithoutNotify(SaveSystem.Data.sfxVolume);
        UpdateLabels();
        if (savePathText != null) savePathText.text = "Save file: " + SaveSystem.FilePath;
    }

    public void OnMusicChanged(float value)
    {
        SaveSystem.SetMusicVolume(value);
        AudioManager.ApplyVolumes();
        UpdateLabels();
    }

    public void OnSfxChanged(float value)
    {
        SaveSystem.SetSfxVolume(value);
        AudioManager.ApplyVolumes();
        UpdateLabels();
    }

    void UpdateLabels()
    {
        if (musicValue != null) musicValue.text = "MUSIC " + Describe(SaveSystem.Data.musicVolume);
        if (sfxValue != null) sfxValue.text = "SOUND " + Describe(SaveSystem.Data.sfxVolume);
    }

    static string Describe(float volume)
    {
        int percent = Mathf.RoundToInt(volume * 100);
        if (percent == 0) return "OFF";
        return $"{percent}%  ({AudioManager.ToDecibels(volume):0.0} dB)";
    }
}
