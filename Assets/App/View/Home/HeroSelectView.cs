using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HeroSelectView : MonoBehaviour
{
    public Image Image;
    public TMP_Text NameText;
    public Button Button;
    public Image SelectedBorder;
    public float SelectDuration = 0.1f;
    public Ease SelectEase = Ease.InOut2;

    public void AnimateSelected(bool selected)
    {
        TweenManager.KillAll(SelectedBorder, false);
        SelectedBorder.TweenAlpha(selected ? 1f : 0f, SelectDuration, SelectEase, SelectedBorder);
    }

    public void SetSelected(bool selected)
    {
        TweenManager.KillAll(SelectedBorder, false);
        SelectedBorder.SetAlpha(selected ? 1f : 0f);
    }
}
