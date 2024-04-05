using System.Collections.Generic;

public class CombatState
{
    public CombatActorState HeroState;
    public CombatActorState EnemyState;
    // TODO: hero abilities
    public CombatVars Vars;
}

public class CombatVars
{
    public int RoundNumber;
    public bool IsHeroTurn;
}

public class CombatActorState
{
    // required to start
    public string Name;
    public string Resource;
    public int MaxHealth;
    public int Health;
    public int ActionsPerTurn;
    public int HandSize;
    public int Mana;
    public List<int> Deck;
    public List<int> Equipped;
    public Dictionary<EffectType, int> StartingEffectValues; // TODO: name sucks

    // calculated
    public int Actions;
    public List<CardInstance> Library;
    public List<CardInstance> Hand;
    public List<CardInstance> Equipment;
    public List<CardInstance> Battlefield;
    public List<CardInstance> Discard;
    public List<EffectInstance> Effects;
}

public class CardInstance
{
    public int InstanceId;
    public int CardId;
}

public class EffectInstance
{
    public EffectType Type;
    public int Value;
    public int Duration;
}

public enum EffectType
{
    None,

    Block,
}

public enum HeroAbilityType
{
    None,

    Heal,
}
