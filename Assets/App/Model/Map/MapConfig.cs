using System;
using System.Collections.Generic;

public class MapConfig
{
    public int MapLevel;
    public int BossLevel;
    public ChanceList<List<int>> NormalEnemyLevels;
    public ChanceList<List<int>> EliteEnemyLevels;
    public ChanceList<int> BonusAreaCounts;
    public Dictionary<RoomType, ChanceList<int>> RoomCounts;

    public int MaxFiles = 4;
    public int MaxRanks = 5;
    //public int MinRooms = 9;
    //public int MaxRooms = 15;

    public static MapConfig CreateLevel1()
    {
        ChanceList<List<int>> normalEnemies = new ChanceList<List<int>>();
        normalEnemies.LoadItem(10, new List<int>() { 1, 1, 2, 2, 2, 2, 3 });
        normalEnemies.LoadItem(10, new List<int>() { 1, 1, 2, 3, 3, 3 });
        normalEnemies.LoadItem(10, new List<int>() { 1, 2, 2, 2, 3, 3 });
        normalEnemies.LoadItem(10, new List<int>() { 2, 2, 2, 2, 2, 3 });
        normalEnemies.LoadItem(10, new List<int>() { 1, 3, 3, 3, 3 });
        normalEnemies.LoadItem(10, new List<int>() { 2, 2, 3, 3, 3 });
        ChanceList<List<int>> eliteEnemies = new ChanceList<List<int>>();
        eliteEnemies.LoadItem(70, new List<int>() { });
        eliteEnemies.LoadItem(30, new List<int>() { 4 });
        eliteEnemies.LoadItem(3, new List<int>() { 4, 4 });
        ChanceList<int> bonusAreas = new ChanceList<int>();
        bonusAreas.LoadItem(20, 0);
        bonusAreas.LoadItem(40, 1);
        bonusAreas.LoadItem(40, 2);
        Dictionary<RoomType, ChanceList<int>> roomCounts = new Dictionary<RoomType, ChanceList<int>>();
        roomCounts.Add(RoomType.Heal, new ChanceList<int>());
        roomCounts[RoomType.Heal].LoadItem(30, 2);
        roomCounts[RoomType.Heal].LoadItem(40, 3);
        roomCounts[RoomType.Heal].LoadItem(20, 4);
        roomCounts[RoomType.Heal].LoadItem(10, 5);
        roomCounts.Add(RoomType.HealingPool, new ChanceList<int>());
        roomCounts[RoomType.HealingPool].LoadItem(50, 0);
        roomCounts[RoomType.HealingPool].LoadItem(40, 1);
        roomCounts[RoomType.HealingPool].LoadItem(10, 2);
        roomCounts.Add(RoomType.Chest, new ChanceList<int>());
        roomCounts[RoomType.Chest].LoadItem(30, 2);
        roomCounts[RoomType.Chest].LoadItem(40, 3);
        roomCounts[RoomType.Chest].LoadItem(30, 4);
        roomCounts.Add(RoomType.Shop, new ChanceList<int>());
        roomCounts[RoomType.Shop].LoadItem(50, 2);
        roomCounts[RoomType.Shop].LoadItem(35, 3);
        roomCounts[RoomType.Shop].LoadItem(15, 4);
        roomCounts.Add(RoomType.Chapel, new ChanceList<int>());
        roomCounts[RoomType.Chapel].LoadItem(40, 0);
        roomCounts[RoomType.Chapel].LoadItem(55, 1);
        roomCounts[RoomType.Chapel].LoadItem(5, 2);
        roomCounts.Add(RoomType.Blacksmith, new ChanceList<int>());
        roomCounts[RoomType.Blacksmith].LoadItem(40, 0);
        roomCounts[RoomType.Blacksmith].LoadItem(55, 1);
        roomCounts[RoomType.Blacksmith].LoadItem(5, 2);
        roomCounts.Add(RoomType.GoodieHut, new ChanceList<int>());
        roomCounts[RoomType.GoodieHut].LoadItem(40, 0);
        roomCounts[RoomType.GoodieHut].LoadItem(55, 1);
        roomCounts[RoomType.GoodieHut].LoadItem(5, 2);
        MapConfig config = new MapConfig()
        {
            MapLevel = 1,
            BossLevel = 4,
            NormalEnemyLevels = normalEnemies,
            EliteEnemyLevels = eliteEnemies,
            BonusAreaCounts = bonusAreas,
            RoomCounts = roomCounts,
        };
        return config;
    }
}

public class ChanceList<T>
{
    private List<ChanceItem> _items = new List<ChanceItem>();
    private int _totalChance = 0;

    public void LoadItem(int chance, T item)
    {
        if (chance <= 0)
        {
            throw new InvalidOperationException($"ChanceList<{typeof(T)}>.LoadItem failed: chance ({chance}) must be greater than zero");
        }
        _items.Add(new ChanceItem() { Chance = chance, Item = item });
        _totalChance += chance;
    }

    public T GetItem(double percentZeroToOne)
    {
        if (_items.Count == 0)
        {
            throw new InvalidOperationException($"ChanceList<{typeof(T)}>.GetItem failed: no items");
        }
        int roll = (int)(percentZeroToOne * _totalChance);
        int index = 0;
        for ( ; index < _items.Count - 1; ++index)
        {
            var item = _items[index];
            if (roll < item.Chance)
            {
                break;
            }
            roll -= item.Chance;
        }
        return _items[index].Item;
    }

    private class ChanceItem
    {
        public int Chance;
        public T Item;
    }
}