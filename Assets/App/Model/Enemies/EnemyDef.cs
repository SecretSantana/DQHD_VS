public abstract class EnemyDef
{
    public abstract string Id { get; }
    public abstract int Level { get; }
    public abstract EnemyType Type { get; }
}

public enum EnemyType
{
    None,
    Normal,
    Elite,
    Boss,
}

public class Enemy_Faerie_1 : EnemyDef
{
    public override string Id => "Faerie_1";
    public override int Level => 1;
    public override EnemyType Type => EnemyType.Normal;
}

public class Enemy_Faerie_2 : EnemyDef
{
    public override string Id => "Faerie_2";
    public override int Level => 2;
    public override EnemyType Type => EnemyType.Normal;
}

public class Enemy_Faerie_3 : EnemyDef
{
    public override string Id => "Faerie_3";
    public override int Level => 3;
    public override EnemyType Type => EnemyType.Normal;
}

public class Enemy_Faerie_Elite_4 : EnemyDef
{
    public override string Id => "Faerie_Elite_4";
    public override int Level => 4;
    public override EnemyType Type => EnemyType.Elite;
}

public class Enemy_Punisher_Boss_4 : EnemyDef
{
    public override string Id => "Punisher_Boss_4";
    public override int Level => 4;
    public override EnemyType Type => EnemyType.Boss;
}