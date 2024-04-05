using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "DynamicResources", menuName = "Scriptable Objects/DynamicResources", order = 1)]
public class DynamicResources : ScriptableObject
{
    [Header("Home")]
    public string AppName;
    public Sprite HomeBackground;

    [Header("Game")]
    public LevelResource[] LevelResources;

    [Header("Data")]
    public HeroResource[] HeroResources;
    public EnemyResource[] EnemyResources;
    public CardResource[] CardResources;

    private Dictionary<string, HeroResource> _heroResources;
    private Dictionary<string, EnemyResource> _enemyResources;
    private Dictionary<string, CardResource> _cardResources;
    private Dictionary<int, LevelResource> _levelResources;

    public HeroResource GetHero(string id)
    {
        if (_heroResources == null)
        {
            _heroResources = HeroResources.ToDictionary(res => res.Id, res => res);
        }
        _heroResources.TryGetValue(id, out var res);
        return res;
    }

    public EnemyResource GetEnemy(string id)
    {
        if (_enemyResources == null)
        {
            _enemyResources = EnemyResources.ToDictionary(res => res.Id, res => res);
        }
        _enemyResources.TryGetValue(id, out var res);
        return res;
    }

    public CardResource GetCard(string id)
    {
        if (_cardResources == null)
        {
            _cardResources = CardResources.ToDictionary(res => res.Id, res => res);
        }
        _cardResources.TryGetValue(id, out var res);
        return res;
    }

    public LevelResource GetLevel(int level)
    {
        if (_levelResources == null)
        {
            _levelResources = LevelResources.ToDictionary(res => res.Level, res => res);
        }
        _levelResources.TryGetValue(level, out var res);
        return res;
    }
}

[Serializable]
public class HeroResource
{
    public string Id;
    public string Name;
    public Sprite Portrait;
}

[Serializable]
public class EnemyResource
{
    public string Id;
    public string Name;
    public Sprite Portrait;
}

[Serializable]
public class CardResource
{
    public string Id;
    public string Name;
    public Sprite Image;
}

[Serializable]
public class LevelResource
{
    public int Level;
    public string Name;
    public Sprite Background;
}