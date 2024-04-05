using System.Collections.Generic;

public class Map
{
    public int MapLevel;
    public List<List<Area>> AreaRanks;
}

public class Area
{
    public int Rank;
    public int File;
    public EnemyData Enemy;
    public List<Room> Rooms;
    public List<Area> BlockedBy;
    public List<Area> Blocking;
}

public class Room
{
    public RoomType Type;
    public object Data;
}

public enum RoomType
{
    None,

    Heal,
    HealingPool,
    Chest,
    Shop,
    Chapel,
    Blacksmith,
    GoodieHut,
    //Altar,
    Stairs,
}

public class EnemyData
{
    public EnemyDef EnemyDef;
    public bool Defeated;
}

public class HealData
{
    public bool Initialized;
}

public class HealingPoolData
{
    public bool Initialized;
}

public class ChestData
{
    public bool Initialized;
}

public class ShopData
{
    public bool Initialized;
}

public class ChapelData
{
    public bool Initialized;
}

public class BlacksmithData
{
    public bool Initialized;
}

public class GoodieHutData
{
    public bool Initialized;
}

public class StairsData
{
    public bool Initialized;
}
