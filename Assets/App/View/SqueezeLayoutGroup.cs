using Alignment;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SqueezeLayoutGroup : MonoBehaviour, ILayoutGroup
{
    public SqueezeAlignment Alignment = SqueezeAlignment.Center;
    public float Spacing = 0f;

    private RectTransform GetRectTransform() => _rectTransformCache == null
        ? _rectTransformCache = GetComponent<RectTransform>()
        : _rectTransformCache;
    private RectTransform _rectTransformCache = null;

    private RectTransform GetChildRectTransform(Transform child)
    {
        int id = child.GetInstanceID();
        if (!_childRectTransformCache.TryGetValue(id, out RectTransform rectTransform))
        {
            _childRectTransformCache.Add(id, rectTransform = child.GetComponent<RectTransform>());
        }
        return rectTransform;
    }
    private Dictionary<int, RectTransform> _childRectTransformCache = new Dictionary<int, RectTransform>();

    private Bounds _lastBounds;

    public void SetLayoutHorizontal()
    {
        RectTransform rectTransform = GetRectTransform();
        if (rectTransform.childCount == 0)
        {
            return;
        }

        Bounds bounds = rectTransform.GetBounds();
        if (bounds.size.x < bounds.size.y)
        {
            return; // this is a vertical layout
        }

        float totalChildWidth = 0f;
        for (int i = 0; i < rectTransform.childCount; ++i)
        {
            RectTransform child = GetChildRectTransform(rectTransform.GetChild(i));
            Bounds childBounds = child.GetBounds();
            totalChildWidth += childBounds.size.x;
            if (i > 0)
            {
                totalChildWidth += Spacing;
            }
        }

        float squeeze = 0f;
        if (totalChildWidth > bounds.size.x && rectTransform.childCount > 1)
        {
            squeeze = (totalChildWidth - bounds.size.x) / (rectTransform.childCount - 1);
        }
        Vector3 nextPos = Vector3.zero;
        switch (Alignment)
        {
            case SqueezeAlignment.Center: nextPos.x = 0f - (0.5f * Mathf.Min(totalChildWidth, bounds.size.x)); break;
            case SqueezeAlignment.LeftOrTop: nextPos.x = 0f - (0.5f * bounds.size.x); break;
            case SqueezeAlignment.RightOrBottom: nextPos.x = 0f + (0.5f * bounds.size.x); break;
        }
        for (int i = 0; i < rectTransform.childCount; ++i)
        {
            RectTransform child = GetChildRectTransform(rectTransform.GetChild(i));
            Bounds childBounds = child.GetBounds();
            nextPos.x += childBounds.extents.x;
            child.localPosition = nextPos;
            nextPos.x += childBounds.extents.x + Spacing - squeeze;
        }
    }

    public void SetLayoutVertical()
    {
        RectTransform rectTransform = GetRectTransform();
        if (rectTransform.childCount == 0)
        {
            return;
        }

        Bounds bounds = rectTransform.GetBounds();
        if (bounds.size.x >= bounds.size.y)
        {
            return; // this is a horizontal layout
        }

        float totalChildHeight = 0f;
        for (int i = 0; i < rectTransform.childCount; ++i)
        {
            RectTransform child = GetChildRectTransform(rectTransform.GetChild(i));
            Bounds childBounds = child.GetBounds();
            totalChildHeight += childBounds.size.y;
            if (i > 0)
            {
                totalChildHeight += Spacing;
            }
        }

        float squeeze = 0f;
        if (totalChildHeight > bounds.size.y && rectTransform.childCount > 1)
        {
            squeeze = (totalChildHeight - bounds.size.y) / (rectTransform.childCount - 1);
        }

        Vector3 nextPos = Vector3.zero;
        switch (Alignment)
        {
            case SqueezeAlignment.Center: nextPos.y = 0f + (0.5f * Mathf.Min(totalChildHeight, bounds.size.y)); break;
            case SqueezeAlignment.LeftOrTop: nextPos.y = 0f + (0.5f * bounds.size.y); break;
            case SqueezeAlignment.RightOrBottom: nextPos.y = 0f - (0.5f * bounds.size.y); break;
        }
        for (int i = 0; i < rectTransform.childCount; ++i)
        {
            RectTransform child = GetChildRectTransform(rectTransform.GetChild(i));
            Bounds childBounds = child.GetBounds();
            nextPos.y -= childBounds.extents.y;
            child.localPosition = nextPos;
            nextPos.y -= childBounds.extents.y + Spacing - squeeze;
        }
    }

    private void Update()
    {
        Bounds bounds = GetRectTransform().GetBounds();
        if (bounds.size.x != _lastBounds.size.x || bounds.size.y != _lastBounds.size.y)
        {
            _lastBounds = bounds;
            LayoutRebuilder.ForceRebuildLayoutImmediate(GetRectTransform());
        }
    }
}

public enum SqueezeAlignment
{
    Center,
    LeftOrTop,
    RightOrBottom,
}