public abstract class CardDef
{
    public abstract string Id { get; }
    public abstract string Text { get; }
    public abstract CardType Type { get; }
    public virtual int Cost => 0;
}

public enum CardType
{
    None,

    Attack,
    Action,
    Spell,
    Prayer,
    Mana,
    Equipment,
}

public class Card_Attack_1 : CardDef
{
    public override string Id => "Attack_1";
    public override string Text => "Deal 1{Damage_Phys} damage.";
    public override CardType Type => CardType.Attack;
}

public class Card_Attack_2 : CardDef
{
    public override string Id => "Attack_2";
    public override string Text => "Deal 2{Damage_Phys} damage.";
    public override CardType Type => CardType.Attack;
}

public class Card_Attack_3 : CardDef
{
    public override string Id => "Attack_3";
    public override string Text => "Deal 3{Damage_Phys} damage.";
    public override CardType Type => CardType.Attack;
}