using Alignment;
using UnityEngine;

public class CardListView : MonoBehaviour
{
    public CanvasGroup CanvasGroup;
    public CardView CardViewPrefab;
    public Transform CardViewParent;

    public HorizontalData TooltipHoriz = new HorizontalData()
    {
        Alignment = HorizontalAlignment.RightOuter,
        Overflow = ScreenOverflowStrategy.TryOppositeThenClamp,
        ReferencePadding = 5,
        ScreenPadding = 5,
    };
    public VerticalData TooltipVert = new VerticalData()
    {
        Alignment = VerticalAlignment.Center,
        Overflow = ScreenOverflowStrategy.Clamp,
        ReferencePadding = 5,
        ScreenPadding = 5,
    };
}
