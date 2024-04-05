using System.Collections.Generic;

public class CardDB
{
    public CardDef GetCard(string id)
    {
        _cardDefs.TryGetValue(id, out var card);
        return card;
    }

    public IEnumerable<CardDef> GetAllCards()
    {
        return _cardDefs.Values;
    }

    private Dictionary<string, CardDef> _cardDefs = new Dictionary<string, CardDef>();

    public CardDB()
    {
        // Add new cards here
        // NOTE: also add CardResource to dynamic resources in editor
        // TODO: use an attribute instead of this list
        AddCard(new Card_Attack_1());
        AddCard(new Card_Attack_2());
        AddCard(new Card_Attack_3());

        void AddCard(CardDef def)
        {
            _cardDefs.Add(def.Id, def);
        }
    }
}
