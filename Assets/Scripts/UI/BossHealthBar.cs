using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BossHealthBar : MonoBehaviour
{
    [SerializeField] Image fill;
    [SerializeField] TMP_Text label;
    [SerializeField] Color normalColor = new Color(0.89f, 0.29f, 0.29f);
    [SerializeField] Color enragedColor = new Color(1f, 0.6f, 0.1f);

    public void Show(string bossName)
    {
        gameObject.SetActive(true);
        if (label != null) label.text = bossName;
        if (fill != null) fill.color = normalColor;
        SetFraction(1f);
    }

    public void SetFraction(float value)
    {
        if (fill != null) fill.fillAmount = Mathf.Clamp01(value);
    }

    public void SetEnraged(string bossName)
    {
        if (fill != null) fill.color = enragedColor;
        if (label != null) label.text = bossName;
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
