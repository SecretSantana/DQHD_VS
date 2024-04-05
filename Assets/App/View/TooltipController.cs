using Alignment;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(ScreenAligner))]
public class TooltipController : MonoBehaviour
{
    public CardView CardFullPrefab;
    private CardView _cardFullInstance;

    private ScreenAligner GetScreenAligner() => _screenAlignerCache == null
            ? _screenAlignerCache = GetComponent<ScreenAligner>()
            : _screenAlignerCache;
    private ScreenAligner _screenAlignerCache = null;

    private Transform GetInstanceParent() => _instantParentCache == null
            ? _instantParentCache = GetScreenAligner().transform
            : _instantParentCache;
    private Transform _instantParentCache = null;

    private StaticResources _staticResources;
    private DynamicResources _dynamicResources;

    private RectTransform _refTrans;
    private RectTransform _tooltipTrans;

    public void Init(StaticResources staticResources, DynamicResources dynamicResources)
    {
        _staticResources = staticResources;
        _dynamicResources = dynamicResources;
        GetInstanceParent().DestroyAllChildren();
    }

    public void ShowCardFull(RectTransform refTrans, CardDef cardDef, HorizontalData horiz, VerticalData vert)
    {
        Hide();

        _refTrans = refTrans;

        if (_cardFullInstance == null)
        {
            _cardFullInstance = Instantiate(CardFullPrefab, GetInstanceParent());
            _cardFullInstance.gameObject.AddComponent<CanvasGroup>().blocksRaycasts = false;
        }
        _cardFullInstance.gameObject.SetActive(true);
        _cardFullInstance.InitFromDef(cardDef, _staticResources, _dynamicResources);

        _tooltipTrans = _cardFullInstance.GetRectTransform();
        LayoutRebuilder.ForceRebuildLayoutImmediate(_tooltipTrans);
        GetScreenAligner().AlignTarget(_refTrans.GetBounds(), _tooltipTrans, horiz, vert);
    }

    public void Hide(RectTransform refTrans)
    {
        if (_refTrans == refTrans)
        {
            Hide();
        }
    }

    public void Hide()
    {
        if (_tooltipTrans != null)
        {
            _tooltipTrans.gameObject.SetActive(false);
            _tooltipTrans = null;
        }
        _refTrans = null;
    }
}