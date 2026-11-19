using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelSelectButton : MonoBehaviour
{
    [SerializeField] int levelNumber = 1;
    [SerializeField] Button button;
    [SerializeField] TMP_Text label;
    [SerializeField] Color unlockedColor = Color.white;
    [SerializeField] Color lockedColor = new Color(0.45f, 0.45f, 0.5f);

    public void Refresh()
    {
        bool unlocked = SaveSystem.IsUnlocked(levelNumber);
        if (button != null)
        {
            button.interactable = unlocked;
            if (button.targetGraphic != null) button.targetGraphic.color = unlocked ? unlockedColor : lockedColor;
        }
        if (label == null) return;
        int best = SaveSystem.GetLevelBest(levelNumber);
        label.text = unlocked
            ? $"LEVEL {levelNumber}\n<size=60%>BEST {best}</size>"
            : $"LEVEL {levelNumber}\n<size=60%>LOCKED</size>";
    }
}
