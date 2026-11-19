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
        UpdateLabels();
    }

    public void OnSfxChanged(float value)
    {
        SaveSystem.SetSfxVolume(value);
        UpdateLabels();
    }

    void UpdateLabels()
    {
        if (musicValue != null) musicValue.text = $"MUSIC {Mathf.RoundToInt(SaveSystem.Data.musicVolume * 100)}%";
        if (sfxValue != null) sfxValue.text = $"SOUND {Mathf.RoundToInt(SaveSystem.Data.sfxVolume * 100)}%";
    }
}
