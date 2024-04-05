using System.Collections.Generic;

public abstract class HeroDef
{
    public abstract string Id { get; }
    public abstract int StartingMaxHealth { get; }
    public abstract int StartingMana { get; }
    public abstract int StartingActions { get; }
    public abstract int StartingHandSize { get; }
    public abstract List<string> StartingDeck { get; }
}

public class Hero_Warrior : HeroDef
{
    public override string Id => "HeroWarrior";
    public override int StartingMaxHealth => 30;
    public override int StartingMana => 2;
    public override int StartingActions => 1;
    public override int StartingHandSize => 2;
    public override List<string> StartingDeck => new List<string>()
    { "Attack_1", "Attack_1", "Attack_1", "Attack_1", "Attack_1",
      "Attack_2", "Attack_2", "Attack_2",
      "Attack_3" };
}

public class Hero_Rogue : HeroDef
{
    public override string Id => "HeroRogue";
    public override int StartingMaxHealth => 30;
    public override int StartingMana => 2;
    public override int StartingActions => 1;
    public override int StartingHandSize => 2;
    public override List<string> StartingDeck => new List<string>()
    { "Attack_1", "Attack_1", "Attack_1", "Attack_1", "Attack_1",
      "Attack_2", "Attack_2", "Attack_2",
      "Attack_3" };
}

public class Hero_Wizard : HeroDef
{
    public override string Id => "HeroWizard";
    public override int StartingMaxHealth => 30;
    public override int StartingMana => 2;
    public override int StartingActions => 1;
    public override int StartingHandSize => 2;
    public override List<string> StartingDeck => new List<string>()
    { "Attack_1", "Attack_1", "Attack_1", "Attack_1", "Attack_1",
      "Attack_2", "Attack_2", "Attack_2",
      "Attack_3" };
}

public class Hero_Priest : HeroDef
{
    public override string Id => "HeroPriest";
    public override int StartingMaxHealth => 30;
    public override int StartingMana => 2;
    public override int StartingActions => 1;
    public override int StartingHandSize => 2;
    public override List<string> StartingDeck => new List<string>()
    { "Attack_1", "Attack_1", "Attack_1", "Attack_1", "Attack_1",
      "Attack_2", "Attack_2", "Attack_2",
      "Attack_3" };
}

public class Hero_Paladin : HeroDef
{
    public override string Id => "HeroPaladin";
    public override int StartingMaxHealth => 30;
    public override int StartingMana => 2;
    public override int StartingActions => 1;
    public override int StartingHandSize => 2;
    public override List<string> StartingDeck => new List<string>()
    { "Attack_1", "Attack_1", "Attack_1", "Attack_1", "Attack_1",
      "Attack_2", "Attack_2", "Attack_2",
      "Attack_3" };
}

public class Hero_Assassin : HeroDef
{
    public override string Id => "HeroAssassin";
    public override int StartingMaxHealth => 30;
    public override int StartingMana => 2;
    public override int StartingActions => 1;
    public override int StartingHandSize => 2;
    public override List<string> StartingDeck => new List<string>()
    { "Attack_1", "Attack_1", "Attack_1", "Attack_1", "Attack_1",
      "Attack_2", "Attack_2", "Attack_2",
      "Attack_3" };
}