using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardView : MonoBehaviour
{
    public TMP_Text NameText;
    public TMP_Text CostText;
    public Image TitleImage;
    public Image BorderImage;
    public Image ArtImage;
    public TMP_Text EffectText;

    public RectTransform GetRectTransform() => _rectTransformCache == null
            ? _rectTransformCache = GetComponent<RectTransform>()
            : _rectTransformCache;
    private RectTransform _rectTransformCache = null;

    public void InitFromDef(CardDef cardDef, StaticResources staticResources, DynamicResources dynamicResources)
    {
        CardResource cardRes = dynamicResources.GetCard(cardDef.Id);
        NameText.text = cardRes.Name;
        CostText.text = cardDef.Type == CardType.Spell || cardDef.Type == CardType.Action
            ? cardDef.Cost.ToString()
            : string.Empty;
        EffectText.text = ReplaceCardText(cardDef.Text);
        ArtImage.sprite = cardRes.Image;

        CardViewResource cardViewRes = staticResources.GetCard(cardDef.Type);
        TitleImage.color = cardViewRes.TextBackgroundColor;
        BorderImage.color = cardViewRes.FrameColor;
    }

    public static string ReplaceCardText(string text)
    {
        foreach ((string orig, string replacement) in _textReplacements)
        {
            if (text.Contains(orig))
            {
                text = text.Replace(orig, replacement);
            }
        }
        return text;
    }

    private static readonly Dictionary<string, string> _textReplacements = new Dictionary<string, string>()
    {
        { "{Damage_Fire}", "<sprite=\"DamageIcons\" name=\"Damage_Fire\">" },
        { "{Damage_Ice}", "<sprite=\"DamageIcons\" name=\"Damage_Ice\">" },
        { "{Damage_Elec}", "<sprite=\"DamageIcons\" name=\"Damage_Elec\">" },
        { "{Damage_Nature}", "<sprite=\"DamageIcons\" name=\"Damage_Nature\">" },
        { "{Damage_Phys}", "<sprite=\"DamageIcons\" name=\"Damage_Phys\">" },
        { "{Damage_Pierce}", "<sprite=\"DamageIcons\" name=\"Damage_Pierce\">" },
    };
}
