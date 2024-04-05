using System;
using UnityEngine;

[CreateAssetMenu(fileName = "StaticResources", menuName = "Scriptable Objects/StaticResources", order = 1)]
public class StaticResources : ScriptableObject
{
    [Header("Areas")]
    public AreaDifficultyResource EasyDifficulty;
    public AreaDifficultyResource FairDifficulty;
    public AreaDifficultyResource HardDifficulty;
    public AreaDifficultyResource VeryHardDifficulty;

    [Header("Cards")]
    public CardViewResource WhiteCard;
    public CardViewResource BlueCard;
    public CardViewResource BlackCard;
    public CardViewResource RedCard;
    public CardViewResource GreenCard;
    public CardViewResource ColorlessCard;

    public CardViewResource GetCard(CardType type)
    {
        switch (type)
        {
            case CardType.Prayer: return WhiteCard;
            case CardType.Mana: return BlueCard;
            case CardType.Spell: return BlackCard;
            case CardType.Attack: return RedCard;
            case CardType.Action: return GreenCard;
            case CardType.Equipment: return ColorlessCard;
        }

        Debug.LogError($"could not find CardViewResource for CardType: {type}");
        return null;
    }
}

[Serializable]
public class AreaDifficultyResource
{
    public Color FrameColor;
    public Color TextBackgroundColor;
}

[Serializable]
public class CardViewResource
{
    public Color FrameColor;
    public Color TextBackgroundColor;
}