using System.Collections.Generic;

public class HeroDB
{
    public HeroDef GetHero(string id)
    {
        _heroDefs.TryGetValue(id, out var hero);
        return hero;
    }

    public IEnumerable<HeroDef> GetAllHeroes()
    {
        return _heroDefs.Values;
    }

    private Dictionary<string, HeroDef> _heroDefs = new Dictionary<string, HeroDef>();

    public HeroDB()
    {
        // Add new heroes here
        // NOTE: also add HeroResource to dynamic resources in editor
        // TODO: use attribute instead of this list
        AddHero(new Hero_Warrior());
        AddHero(new Hero_Rogue());
        AddHero(new Hero_Wizard());
        AddHero(new Hero_Priest());
        AddHero(new Hero_Paladin());
        AddHero(new Hero_Assassin());

        void AddHero(HeroDef def)
        {
            _heroDefs.Add(def.Id, def);
        }
    }
}
