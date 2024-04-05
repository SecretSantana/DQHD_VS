using System.Collections.Generic;

public class EnemyDB
{
    public EnemyDef GetEnemy(string id)
    {
        _enemyDefs.TryGetValue(id, out var enemy);
        return enemy;
    }

    public IEnumerable<EnemyDef> GetAllEnemies()
    {
        return _enemyDefs.Values;
    }

    public List<EnemyDef> GetEnemies(EnemyType type, int level)
    {
        if (_lookup.TryGetValue(type, out var typeTable))
        {
            if (typeTable.TryGetValue(level, out var levelList))
            {
                return levelList;
            }
        }
        return new List<EnemyDef>();
    }

    private Dictionary<string, EnemyDef> _enemyDefs = new Dictionary<string, EnemyDef>();
    private Dictionary<EnemyType, Dictionary<int, List<EnemyDef>>> _lookup = new Dictionary<EnemyType, Dictionary<int, List<EnemyDef>>>();

    public EnemyDB()
    {
        // Add new enemies here
        // NOTE: also add EnemyResource to dynamic resources in editor
        // TODO: use an attribute instead of this list
        AddEnemy(new Enemy_Faerie_1());
        AddEnemy(new Enemy_Faerie_2());
        AddEnemy(new Enemy_Faerie_3());
        AddEnemy(new Enemy_Faerie_Elite_4());
        AddEnemy(new Enemy_Punisher_Boss_4());

        void AddEnemy(EnemyDef def)
        {
            _enemyDefs.Add(def.Id, def);
            if (!_lookup.TryGetValue(def.Type, out var typeTable))
            {
                _lookup.Add(def.Type, typeTable = new Dictionary<int, List<EnemyDef>>());
            }
            if (!typeTable.TryGetValue(def.Level, out var levelList))
            {
                typeTable.Add(def.Level, levelList = new List<EnemyDef>());
            }
            levelList.Add(def);
        }
    }
}
