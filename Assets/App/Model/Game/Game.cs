using System.Collections.Generic;

public class Game
{
    public HeroData Hero => _hero;
    public Map CurrentMap => _maps[_currentMapIndex];

    private HeroData _hero;
    private List<Map> _maps;
    private int _currentMapIndex;

    public Game(EnemyDB enemyDB, HeroDef hero)
    {
        _hero = new HeroData()
        {
            Id = hero.Id,
            Level = 1,
            MaxHealth = hero.StartingMaxHealth,
            Health = hero.StartingMaxHealth,
            Mana = hero.StartingMana,
            Actions = hero.StartingActions,
            HandSize = hero.StartingHandSize,
            Deck = new List<string>(hero.StartingDeck),
        };
        _maps = new List<Map>()
        {
            MapLogic.BuildMap(MapConfig.CreateLevel1(), enemyDB),
        };
    }
}

public class HeroData
{
    public string Id;
    public int Level;
    public int CurrentXP;
    public int MaxHealth;
    public int Health;
    public int Mana;
    public int Actions;
    public int HandSize;
    public List<string> Deck;

    private static Dictionary<int, int> _xpToNextLevel = new Dictionary<int, int>()
    {
        { 1, 2 },
        { 2, 4 },
        { 3, 7 },
        { 4, 12 },
        { 5, 17 },
        { 6, 23 },
        { 7, 30 },
        { 8, 38 },
        { 9, 47 },
    };
    public static int GetXpToNextLevel(int currentLevel)
    {
        _xpToNextLevel.TryGetValue(currentLevel, out int xp);
        return xp;
    }
}