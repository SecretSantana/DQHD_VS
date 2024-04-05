using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class AppController : MonoBehaviour
{
    [Header("Transition")]
    public Image TransitionBlocker;
    public float TransitionDuration = 0.25f;
    public Ease TransitionEase = Ease.In2;

    [Header("Scenes")]
    public Transform SceneParent;
    public HomeController HomePrefab;
    public GameController GamePrefab;

    [Header("Tooltips")]
    public TooltipController Tooltips;

    private DataSource _dataSource;
    private StaticResources _staticResources;
    private DynamicResources _dynamicResources;
    private AppContext _app;
    private GameObject _currentScene;

    public void Initialize(DataSource dataSource, StaticResources staticResources, DynamicResources dynamicResources)
    {
        _dataSource = dataSource;
        _staticResources = staticResources;
        _dynamicResources = dynamicResources;
        StartCoroutine(Coroutine_StartApp());
    }

    private IEnumerator Coroutine_StartApp()
    {
        TransitionBlocker.SetAlpha(1f);
        TransitionBlocker.raycastTarget = true;

        yield return null;

        Tooltips.Init(_staticResources, _dynamicResources);

        _app = new AppContext()
        {
            StaticResources = _staticResources,
            DynamicResources = _dynamicResources,
            HeroDB = new HeroDB(),
            CardDB = new CardDB(),
            EnemyDB = new EnemyDB(),
            GotoHome = GotoHome,
            GotoGame = GotoGame,
            Tooltips = Tooltips,
        };

        GotoHome();
    }

    private void GotoHome()
    {
        StartCoroutine(Coroutine_GotoScene(() =>
        {
            HomeController home = Instantiate(HomePrefab, SceneParent);
            home.Initialize(_app);
            _currentScene = home.gameObject;
        }));
    }

    private void GotoGame(HeroDef heroDef)
    {
        StartCoroutine(Coroutine_GotoScene(() =>
        {
            GameController game = Instantiate(GamePrefab, SceneParent);
            game.Initialize(_app, heroDef);
            _currentScene = game.gameObject;
        }));
    }

    private IEnumerator Coroutine_GotoScene(Action setCurrentScene)
    {
        TweenManager.KillAll(TransitionBlocker, false);

        if (_currentScene != null)
        {
            TransitionBlocker.raycastTarget = true;
            yield return TransitionBlocker
                .TweenAlpha(1f, TransitionDuration, TransitionEase, TransitionBlocker)
                .WaitUntilDone();
            Destroy(_currentScene);
        }

        setCurrentScene.Invoke();
        yield return null;

        yield return TransitionBlocker
            .TweenAlpha(0f, TransitionDuration, TransitionEase, TransitionBlocker)
            .WaitUntilDone();
        TransitionBlocker.raycastTarget = false;
    }
}

public class AppContext
{
    public StaticResources StaticResources;
    public DynamicResources DynamicResources;
    public HeroDB HeroDB;
    public CardDB CardDB;
    public EnemyDB EnemyDB;

    public Action GotoHome;
    public Action<HeroDef> GotoGame;

    public TooltipController Tooltips;
}