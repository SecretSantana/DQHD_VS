using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public GameView GameView;

    private AppContext _app;
    private Game _game;

    public void Initialize(AppContext app, HeroDef heroDef)
    {
        _app = app;

        // this spins up a new randomized instance of maps
        _game = new Game(app.EnemyDB, heroDef);

        ResetToState();
    }

    private void ResetToState()
    {
        GameView.NonCombatView.CanvasGroup.SetInteractableAndShowing(true);

        LevelResource levelResource = _app.DynamicResources.GetLevel(_game.CurrentMap.MapLevel);
        GameView.Background.sprite = levelResource.Background;

        // map
        var mapView = GameView.NonCombatView.MapView;
        mapView.LevelName.text = levelResource.Name;
        mapView.CanvasGroup.SetInteractableAndShowing(true);
        mapView.RankParent.DestroyAllChildren();
        for (int rank = 0; rank < _game.CurrentMap.AreaRanks.Count; ++rank)
        {
            Transform rankInstance = Instantiate(mapView.RankPrefab, mapView.RankParent);
            for (int file = 0; file < _game.CurrentMap.AreaRanks[rank].Count; ++file)
            {
                Area area = _game.CurrentMap.AreaRanks[rank][file];
                AreaView areaInstance = Instantiate(mapView.AreaPrefab, rankInstance);

                bool areaIsAvailable = MapLogic.AreaIsAvailable(_game.CurrentMap, area);
                areaInstance.Blocker.gameObject.SetActive(!areaIsAvailable);

                if (area.Enemy == null || !areaIsAvailable)
                {
                    areaInstance.EnemyImage.gameObject.SetActive(false);
                }
                else
                {
                    areaInstance.EnemyImage.gameObject.SetActive(true);
                    EnemyResource enemyResource = _app.DynamicResources.GetEnemy(area.Enemy.EnemyDef.Id);
                    areaInstance.EnemyImage.sprite = enemyResource.Portrait;
                    AreaDifficultyResource areaResource = GetAreaResource(_game.Hero.Level, area.Enemy.EnemyDef);
                    areaInstance.EnemyFrameImage.color = areaResource.FrameColor;
                    areaInstance.LevelBackgroundImage.color = areaResource.TextBackgroundColor;
                    areaInstance.LevelText.text = $"{area.Enemy.EnemyDef.Level}";
                    areaInstance.EnemyButton.onClick.AddListener(() => { Debug.Log($"// TODO: begin combat with {area.Enemy.EnemyDef.Id}"); });
                }

                areaInstance.RoomParent.DestroyAllChildren();
                for (int room = 0; room < area.Rooms.Count; ++room)
                {
                    RoomView roomInstance = Instantiate(areaInstance.RoomPrefab, areaInstance.RoomParent);
                    // TODO: init room instance
                }
                areaInstance.RoomParent.gameObject.SetActive(areaIsAvailable);
            }
        }

        // hero
        var heroView = GameView.NonCombatView.HeroView;
        heroView.CanvasGroup.SetInteractableAndShowing(true);
        HeroResource heroResource = _app.DynamicResources.GetHero(_game.Hero.Id);
        heroView.PortraitImage.sprite = heroResource.Portrait;
        heroView.NameText.text = heroResource.Name;
        heroView.LevelText.text = $"Level {_game.Hero.Level}";
        heroView.XPText.text = $"{_game.Hero.CurrentXP}/{HeroData.GetXpToNextLevel(_game.Hero.Level)}";
        heroView.HealthText.text = $"{_game.Hero.Health}/{_game.Hero.MaxHealth}";
        heroView.HandSizeText.text = $"{_game.Hero.HandSize}";
        heroView.ActionsText.text = $"{_game.Hero.Actions}";
        heroView.ManaText.text = $"{_game.Hero.Mana}";

        // deck
        var deckView = GameView.NonCombatView.DeckView;
        deckView.CanvasGroup.SetInteractableAndShowing(true);
        deckView.CardViewParent.DestroyAllChildren();
        var sortedDeck = _game.Hero.Deck
            .Select(id => _app.CardDB.GetCard(id))
            .OrderBy(def => def.Id); // TODO: real sort
        foreach (CardDef cardDef in sortedDeck)
        {
            CardView cardInstance = Instantiate(deckView.CardViewPrefab, deckView.CardViewParent);
            cardInstance.InitFromDef(cardDef, _app.StaticResources, _app.DynamicResources);

            CardDef capture = cardDef;
            PointerEventHandler eventHandler = cardInstance.gameObject.AddComponent<PointerEventHandler>();
            eventHandler.HandlePointerEnter += () => _app.Tooltips.ShowCardFull(cardInstance.GetRectTransform(), capture, deckView.TooltipHoriz, deckView.TooltipVert);
            eventHandler.HandlePointerExit += () => _app.Tooltips.Hide(cardInstance.GetRectTransform());
        }

        // combat (off)
        GameView.CombatView.CanvasGroup.SetInteractableAndShowing(false);
    }

    private AreaDifficultyResource GetAreaResource(int heroLevel, EnemyDef enemyDef)
    {
        int diff = enemyDef.Level - heroLevel;
        if (enemyDef.Type == EnemyType.Elite) diff += 1;
        else if (enemyDef.Type == EnemyType.Boss) diff += 2;
        if (diff < 0) return _app.StaticResources.EasyDifficulty;
        if (diff == 0) return _app.StaticResources.FairDifficulty;
        if (diff == 1) return _app.StaticResources.HardDifficulty;
        return _app.StaticResources.VeryHardDifficulty;
    }
}