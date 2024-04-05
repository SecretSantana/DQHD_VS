using UnityEngine;

public class HomeController : MonoBehaviour
{
    public HomeView HomeView;

    private AppContext _app;

    public void Initialize(AppContext app)
    {
        _app = app;

        HomeView.AppNameText.text = _app.DynamicResources.AppName;
        HomeView.BackgroundImage.sprite = _app.DynamicResources.HomeBackground;
        HomeView.NewGameButton.onClick.AddListener(HomeView.HeroSelect.Show);
        HomeView.HeroSelect.Initialize(app,
            () => HomeView.HeroSelect.Hide(false),
            (heroDef) => _app.GotoGame(heroDef));
        HomeView.HeroSelect.Hide(true);
    }
}
