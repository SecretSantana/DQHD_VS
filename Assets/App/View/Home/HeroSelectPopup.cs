using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HeroSelectPopup : MonoBehaviour
{
    public CanvasGroup CanvasGroup;
    public float FadeDuration = 0.3f;
    public Ease FadeEase = Ease.Out2;
    public HeroSelectView HeroSelectPrefab;
    public Transform HeroSelectParent;
    public Button CancelButton;
    public Button OKButton;
    public TMP_Text OKLabel;

    private AppContext _app;
    private HeroDef _selectedHeroDef;
    private HeroSelectView _selectedHeroView;

    public void Initialize(AppContext app, Action onCancel, Action<HeroDef> onOK)
    {
        _app = app;

        HeroSelectParent.DestroyAllChildren();
        foreach (HeroDef heroDef in _app.HeroDB.GetAllHeroes())
        {
            HeroDef hero = heroDef; // capture
            HeroResource heroResource = _app.DynamicResources.GetHero(hero.Id);
            HeroSelectView heroView = Instantiate(HeroSelectPrefab, HeroSelectParent);
            heroView.Image.sprite = heroResource.Portrait;
            heroView.NameText.text = heroResource.Name;
            heroView.Button.onClick.AddListener(() =>
            {
                _selectedHeroDef = hero;
                OKLabel.text = heroResource.Name;
                OKButton.interactable = true;

                if (_selectedHeroView != null)
                {
                    _selectedHeroView.AnimateSelected(false);
                }
                _selectedHeroView = heroView;
                _selectedHeroView.AnimateSelected(true);
            });
            heroView.SetSelected(false);
        }

        CancelButton.onClick.AddListener(() => onCancel());
        OKButton.onClick.AddListener(() => onOK(_selectedHeroDef));
    }

    public void Show()
    {
        TweenManager.KillAll(CanvasGroup, false);

        OKButton.interactable = false;
        OKLabel.text = "choose";
        if (_selectedHeroView != null)
        {
            _selectedHeroView.SetSelected(false);
            _selectedHeroView = null;
        }

        CanvasGroup
            .TweenAlpha(1f, FadeDuration, FadeEase, CanvasGroup)
            .OnDone += () => SetInteractable(true);
    }

    public void Hide(bool immediate = false)
    {
        TweenManager.KillAll(CanvasGroup, false);

        SetInteractable(false);

        if (immediate)
        {
            CanvasGroup.alpha = 0f;
        }
        else
        {
            CanvasGroup.TweenAlpha(0f, FadeDuration, FadeEase, CanvasGroup);
        }
    }

    private void SetInteractable(bool interactable)
    {
        CanvasGroup.blocksRaycasts = interactable;
        CanvasGroup.interactable = interactable;
    }
}
