using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public static class Utilities
{
    public static void DestroyAllChildren(this Transform parent, bool immediate = false)
    {
        for (int index = parent.childCount - 1; index >= 0; --index)
        {
            if (immediate)
            {
                Object.DestroyImmediate(parent.GetChild(index).gameObject);
            }
            else
            {
                Object.Destroy(parent.GetChild(index).gameObject);
            }
        }
    }

    public static void SetAlpha(this Graphic graphic, float alpha)
    {
        Color color = graphic.color;
        color.a = alpha;
        graphic.color = color;
    }

    public static void Shuffle<T>(this List<T> orig)
    {
        for (int left = 0; left < orig.Count - 1; ++left)
        {
            int right = Random.Range(left, orig.Count);
            if (left != right)
            {
                T temp = orig[left];
                orig[left] = orig[right];
                orig[right] = temp;
            }
        }
    }

    public static List<T> ToShuffled<T>(this List<T> orig)
    {
        List<T> shuffled = new List<T>(orig);
        shuffled.Shuffle();
        return shuffled;
    }

    public static void SetInteractableAndShowing(this CanvasGroup group, bool interactableAndShowing)
    {
        TweenManager.KillAll(group, false);
        group.interactable = interactableAndShowing;
        group.blocksRaycasts = interactableAndShowing;
        group.alpha = interactableAndShowing ? 1f : 0f;
    }

    public static ITween AnimateInteractableAndShowing(this CanvasGroup group, bool interactableAndShowing, float duration, Ease ease)
    {
        TweenManager.KillAll(group, false);
        ITween tween = group.TweenAlpha(interactableAndShowing ? 1f : 0f, duration, ease, group);
        if (interactableAndShowing)
        {
            tween.OnDone += () =>
            {
                group.interactable = true;
                group.blocksRaycasts = true;
            };
        }
        else
        {
            group.interactable = false;
            group.blocksRaycasts = false;
        }
        return tween;
    }
}
