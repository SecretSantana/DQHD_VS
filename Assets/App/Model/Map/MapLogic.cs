using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class MapLogic
{
    public static bool AreaIsAvailable(Map map, Area area)
    {
        if (area.BlockedBy.Count == 0)
        {
            return true;
        }
        for (int index = 0; index < area.BlockedBy.Count; ++index)
        {
            if (area.BlockedBy[index].Enemy.Defeated)
            {
                return true;
            }
        }
        return false;
    }

    public static Map BuildMap(MapConfig config, EnemyDB enemyDB)
    {
        Map map = new Map();
        map.MapLevel = config.MapLevel;
        map.AreaRanks = new List<List<Area>>();

        // roll normal enemies and place into ranks
        List<int> normalEnemyLevels = config.NormalEnemyLevels.GetItem(Random.value);
        foreach (int normalLevel in normalEnemyLevels)
        {
            AddEnemyArea(config, enemyDB, map, EnemyType.Normal, normalLevel, 0, true);
        }

        // roll boss enemy and place into ranks (not in first rank or new rank, if possible)
        Area bossArea = AddEnemyArea(config, enemyDB, map, EnemyType.Boss, config.BossLevel, 1, false);
        bossArea.Rooms.Add(CreateNewRoom(RoomType.Stairs));

        // roll elites and place into ranks (not in first 2 ranks, if possible)
        List<int> eliteEnemyLevels = config.EliteEnemyLevels.GetItem(Random.value);
        foreach (int eliteLevel in eliteEnemyLevels)
        {
            AddEnemyArea(config, enemyDB, map, EnemyType.Elite, eliteLevel, 2, true);
        }

        // shuffle each rank (and assign files) before linking
        foreach (var rank in map.AreaRanks)
        {
            rank.Shuffle();
            for (int index = 0; index < rank.Count; ++index)
            {
                rank[index].File = index;
            }
        }

        // create BlockedBy links (paths)
        for (int index = 1; index < map.AreaRanks.Count; ++index)
        {
            map.AreaRanks[index] = LinkAreas(map.AreaRanks[index], map.AreaRanks[index - 1]);
        }

        // create bonus areas (just rooms without enemies, dead ends)
        int bonusAreaCount = config.BonusAreaCounts.GetItem(Random.value);
        for (int count = 0; count < bonusAreaCount; ++count)
        {
            AddBonusArea(config, map);
        }

        // assign rooms
        foreach ((RoomType roomType, ChanceList<int> roomCounts) in config.RoomCounts)
        {
            int total = roomCounts.GetItem(Random.value);
            for (int count = 0; count < total; ++count)
            {
                AddRoom(map, roomType);
            }
        }

        // shuffle rooms
        foreach (var rank in map.AreaRanks)
        {
            foreach (var area in rank)
            {
                area.Rooms.Shuffle();
            }
        }

        return map;
    }

    private static Area AddEnemyArea(MapConfig config, EnemyDB enemyDB, Map map, 
        EnemyType type, int level, int minRank, bool canAddNewRank)
    {
        int maxRank = canAddNewRank 
            ? map.AreaRanks.Count 
            : map.AreaRanks.Count - 1;
        maxRank = Mathf.Min(config.MaxRanks - 1, maxRank);
        List<int> validRanks = new List<int>();
        for (int r = minRank; r <= maxRank; ++r)
        {
            if (r >= map.AreaRanks.Count || map.AreaRanks[r].Count < config.MaxFiles)
            {
                validRanks.Add(r);
            }
        }
        int rank = validRanks.Count > 0 
            ? validRanks[Random.Range(0, validRanks.Count)] 
            : Mathf.Max(0, map.AreaRanks.Count - 1);

        if (rank == map.AreaRanks.Count)
        {
            map.AreaRanks.Add(new List<Area>());
        }
        List<EnemyDef> validEnemies = enemyDB.GetEnemies(type, level);
        Area area = new Area()
        {
            Rank = rank,
            Rooms = new List<Room>(),
            BlockedBy = new List<Area>(),
            Blocking = new List<Area>(),
            Enemy = new EnemyData()
            {
                EnemyDef = validEnemies[Random.Range(0, validEnemies.Count)],
            },
        };
        map.AreaRanks[rank].Add(area);
        return area;
    }

    private static Area AddBonusArea(MapConfig config, Map map)
    {
        List<Area> validBlockers = map.AreaRanks
            .SelectMany(r => r
            .Where(a => a.Rank < (config.MaxRanks - 1) && a.Enemy != null && a.Blocking.Count == 0))
            .ToList();
        if (validBlockers.Count == 0)
        {
            // uh oh... TODO: 
            return null;
        }
        Area blockingArea = validBlockers[Random.Range(0, validBlockers.Count)];
        int rank = blockingArea.Rank + 1;
        if (rank == map.AreaRanks.Count)
        {
            map.AreaRanks.Add(new List<Area>());
        }
        Area area = new Area()
        {
            Rank = rank,
            Rooms = new List<Room>(),
            BlockedBy = new List<Area>() { blockingArea },
            Blocking = new List<Area>(),
        };
        blockingArea.Blocking.Add(area);
        // this needs to insert into correct position according to what is blocking it
        int insertIndex = 0;
        for (; insertIndex < map.AreaRanks[rank].Count; ++insertIndex)
        {
            Area checkArea = map.AreaRanks[rank][insertIndex];
            int maxFile = checkArea.BlockedBy.Max(a => a.File);
            if (maxFile > blockingArea.File)
            {
                break;
            }
        }
        map.AreaRanks[rank].Insert(insertIndex, area);
        return area;
    }

    private static void AddRoom(Map map, RoomType roomType)
    {
        // empty bonus areas first
        List<Area> validAreas = AreasWhere(map, a => a.Enemy == null && a.Rooms.Count == 0);
        // then <2 rooms, then <3, etc.
        for (int lessThan = 2; validAreas.Count == 0; ++lessThan)
        {
            validAreas = AreasWhere(map, a => a.Rooms.Count < lessThan);
        }
        Area area = validAreas[Random.Range(0, validAreas.Count)];
        area.Rooms.Add(CreateNewRoom(roomType));

        static List<Area> AreasWhere(Map map, System.Func<Area, bool> filter)
        {
            List<Area> validAreas = new List<Area>();
            for (int rank = 0; rank < map.AreaRanks.Count; ++rank)
            {
                for (int file = 0; file < map.AreaRanks[rank].Count; ++file)
                {
                    Area area = map.AreaRanks[rank][file];
                    if (filter(area))
                    {
                        validAreas.Add(area);
                    }
                }
            }
            return validAreas;
        }
    }

    private static List<Area> LinkAreas(List<Area> areasToLink, List<Area> blockingAreas)
    {
        List<List<Area>> links = new List<List<Area>>(blockingAreas.Count);
        for (int i = 0; i < blockingAreas.Count; ++i)
        {
            links.Add(new List<Area>());
        }
        for (int i = 0; i < areasToLink.Count; ++i)
        {
            List<int> validBlockingIndices = new List<int>();
            for (int b = 0; b < blockingAreas.Count; ++b)
            {
                Area a = blockingAreas[b];
                if (a.Enemy == null || a.Enemy.EnemyDef.Type != EnemyType.Boss)
                {
                    validBlockingIndices.Add(b);
                };
            }
            int blockedByIndex = validBlockingIndices[Random.Range(0, validBlockingIndices.Count)];
            links[blockedByIndex].Add(areasToLink[i]);
            Area blockingArea = blockingAreas[blockedByIndex];
            areasToLink[i].BlockedBy.Add(blockingArea);
            blockingArea.Blocking.Add(areasToLink[i]);
        }
        List<Area> linkedAreas = new List<Area>();
        for (int i = 0; i < links.Count; ++i)
        {
            if (links[i].Count > 0)
            {
                linkedAreas.AddRange(links[i]);
            }
        }
        // TODO: something about blocked by multiple (30% chance on each level to have a double block?)
        return linkedAreas;
    }

    private static Room CreateNewRoom(RoomType type)
    {
        Room room = new Room() { Type = type };
        switch (type)
        {
            case RoomType.Heal: room.Data = new HealData(); break;
            case RoomType.HealingPool: room.Data = new HealingPoolData(); break;
            case RoomType.Chest: room.Data = new ChestData(); break;
            case RoomType.Shop: room.Data = new ShopData(); break;
            case RoomType.Chapel: room.Data = new ChapelData(); break;
            case RoomType.Blacksmith: room.Data = new BlacksmithData(); break;
            case RoomType.GoodieHut: room.Data=new GoodieHutData(); break;
            case RoomType.Stairs: room.Data = new StairsData(); break;
            default: throw new System.ArgumentException($"can't create Room for RoomType {type}");
        }
        return room;
    }

    public static void Debug_MapAreas(Map map)
    {
        System.Text.StringBuilder builder = new System.Text.StringBuilder();
        builder.AppendLine($"map has {map.AreaRanks.Count} areas:");
        for (int rank = 0; rank < map.AreaRanks.Count; ++rank)
        {
            builder.Append($":{rank}: ");
            for (int area = 0; area < map.AreaRanks[rank].Count; ++area)
            {
                if (map.AreaRanks[rank][area].Enemy == null)
                {
                    builder.Append("bonus");
                }
                else
                {
                    builder.Append(map.AreaRanks[rank][area].Enemy.EnemyDef.Id);
                }
                if (map.AreaRanks[rank][area].Rooms.Count > 0)
                {
                    builder.Append("(");
                    for (int room = 0; room < map.AreaRanks[rank][area].Rooms.Count; ++room)
                    {
                        builder.Append(map.AreaRanks[rank][area].Rooms[room].Type);
                        if (room < map.AreaRanks[rank][area].Rooms.Count - 1)
                        {
                            builder.Append(", ");
                        }
                    }
                    builder.Append(")");
                }
                if (area < map.AreaRanks[rank].Count - 1)
                {
                    builder.Append(", ");
                }
            }
            builder.AppendLine($" ({map.AreaRanks[rank].Count})");
        }
        Debug.Log(builder);
    }
}
