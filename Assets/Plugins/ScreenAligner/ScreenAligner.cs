using System;
using System.Collections.Generic;
using UnityEngine;

namespace Alignment
{
    [RequireComponent(typeof(RectTransform))]
    public class ScreenAligner : MonoBehaviour
    {
        private RectTransform GetRectTransform() => _rectTransformCache == null
            ? _rectTransformCache = GetComponent<RectTransform>()
            : _rectTransformCache;
        private RectTransform _rectTransformCache = null;

        private Canvas GetCanvas() => _canvasCache == null
            ? _canvasCache = GetComponentInParent<Canvas>()
            : _canvasCache;
        private Canvas _canvasCache = null;

        public void AlignTarget(Bounds refBounds, RectTransform targetTrans, HorizontalData horizontalData, VerticalData verticalData)
        {
            Bounds screenBounds = GetRectTransform().GetBounds();
            Vector3 targetExtents = targetTrans.GetBounds().extents;
            Vector3 canvasScale = GetCanvas().transform.localScale;

            Vector3 newPos = targetTrans.position;
            newPos.x = AlignmentStrategies.GetValue(
                horizontalData.Alignment,
                horizontalData.Overflow,
                refBounds,
                targetExtents.x + horizontalData.ReferencePadding * canvasScale.x,
                screenBounds,
                targetExtents.x + horizontalData.ScreenPadding * canvasScale.x);
            newPos.y = AlignmentStrategies.GetValue(
                verticalData.Alignment,
                verticalData.Overflow,
                refBounds,
                targetExtents.y + verticalData.ReferencePadding * canvasScale.y,
                screenBounds,
                targetExtents.y + verticalData.ScreenPadding * canvasScale.y);

            targetTrans.position = newPos;
        }
    }

    [Serializable]
    public class HorizontalData
    {
        public HorizontalAlignment Alignment;
        public ScreenOverflowStrategy Overflow;
        public float ReferencePadding;
        public float ScreenPadding;
    }

    [Serializable]
    public class VerticalData
    {
        public VerticalAlignment Alignment;
        public ScreenOverflowStrategy Overflow;
        public float ReferencePadding;
        public float ScreenPadding;
    }

    public enum HorizontalAlignment
    {
        RightOuter,
        RightInner,
        Center,
        LeftInner,
        LeftOuter,
    }

    public enum VerticalAlignment
    {
        TopOuter,
        TopInner,
        Center,
        BottomInner,
        BottomOuter,
    }

    public enum ScreenOverflowStrategy
    {
        None,
        Clamp,
        TryOppositeThenClamp,
    }

    public static class AlignmentStrategies
    {
        // Shared
        private delegate float AlignmentStrategyFunc(Bounds refBounds, float refTargetExtents, Bounds screenBounds, float screenTargetExtents, ScreenOverflowStrategy overflow);

        private static float Clamp(float value, float min, float max, float targetExtents, ScreenOverflowStrategy overflow) =>
            overflow == ScreenOverflowStrategy.TryOppositeThenClamp || overflow == ScreenOverflowStrategy.Clamp
            ? Mathf.Clamp(value, min + targetExtents, max - targetExtents)
            : value;

        private static float GetPreferredValue(bool preferMax, float rawMin, float rawMax, float screenMin, float screenMax, float targetExtents, ScreenOverflowStrategy overflow)
        {
            float minValue = Clamp(rawMin, screenMin, screenMax, targetExtents, overflow);
            float minOverflow = minValue - rawMin;
            float maxValue = Clamp(rawMax, screenMin, screenMax, targetExtents, overflow);
            float maxOverflow = rawMax - maxValue;

            float preferredValue = preferMax ? maxValue : minValue;
            float preferredOverflow = preferMax ? maxOverflow : minOverflow;
            float oppositeValue = preferMax ? minValue : maxValue;
            float oppositeOverflow = preferMax ? minOverflow : maxOverflow;

            bool opposite = overflow == ScreenOverflowStrategy.TryOppositeThenClamp
                && preferredOverflow > 0
                && preferredOverflow > oppositeOverflow;
            return opposite ? oppositeValue : preferredValue;
        }

        // Horizontal
        private static readonly Dictionary<HorizontalAlignment, AlignmentStrategyFunc> _horizontalStrategies = new Dictionary<HorizontalAlignment, AlignmentStrategyFunc>()
            {
                { HorizontalAlignment.RightOuter, Horizontal.RightOuter },
                { HorizontalAlignment.RightInner, Horizontal.RightInner },
                { HorizontalAlignment.Center, Horizontal.Center },
                { HorizontalAlignment.LeftInner, Horizontal.LeftInner },
                { HorizontalAlignment.LeftOuter, Horizontal.LeftOuter },
            };

        public static float GetValue(HorizontalAlignment alignment, ScreenOverflowStrategy overflow, Bounds refBounds, float refTargetExtents, Bounds screenBounds, float screenTargetExtents)
        {
            return _horizontalStrategies[alignment](refBounds, refTargetExtents, screenBounds, screenTargetExtents, overflow);
        }

        private static class Horizontal
        {
            public static float RightOuter(Bounds refBounds, float refTargetExtents, Bounds screenBounds, float screenTargetExtents, ScreenOverflowStrategy overflow)
            {
                return GetPreferredValue(true,
                    refBounds.min.x - refTargetExtents,
                    refBounds.max.x + refTargetExtents,
                    screenBounds.min.x, screenBounds.max.x, screenTargetExtents, overflow);
            }

            public static float RightInner(Bounds refBounds, float refTargetExtents, Bounds screenBounds, float screenTargetExtents, ScreenOverflowStrategy overflow)
            {
                return GetPreferredValue(true,
                    refBounds.min.x + refTargetExtents,
                    refBounds.max.x - refTargetExtents,
                    screenBounds.min.x, screenBounds.max.x, screenTargetExtents, overflow);
            }

            public static float Center(Bounds refBounds, float refTargetExtents, Bounds screenBounds, float screenTargetExtents, ScreenOverflowStrategy overflow)
            {
                return Clamp(refBounds.center.x, screenBounds.min.x, screenBounds.max.x, screenTargetExtents, overflow);
            }

            public static float LeftInner(Bounds refBounds, float refTargetExtents, Bounds screenBounds, float screenTargetExtents, ScreenOverflowStrategy overflow)
            {
                return GetPreferredValue(false,
                    refBounds.min.x + refTargetExtents,
                    refBounds.max.x - refTargetExtents,
                    screenBounds.min.x, screenBounds.max.x, screenTargetExtents, overflow);
            }

            public static float LeftOuter(Bounds refBounds, float refTargetExtents, Bounds screenBounds, float screenTargetExtents, ScreenOverflowStrategy overflow)
            {
                return GetPreferredValue(false,
                    refBounds.min.x - refTargetExtents,
                    refBounds.max.x + refTargetExtents,
                    screenBounds.min.x, screenBounds.max.x, screenTargetExtents, overflow);
            }
        }

        // Vertical
        private static readonly Dictionary<VerticalAlignment, AlignmentStrategyFunc> _verticalStrategies = new Dictionary<VerticalAlignment, AlignmentStrategyFunc>()
            {
                { VerticalAlignment.TopOuter, Vertical.TopOuter },
                { VerticalAlignment.TopInner, Vertical.TopInner },
                { VerticalAlignment.Center, Vertical.Center },
                { VerticalAlignment.BottomInner, Vertical.BottomInner },
                { VerticalAlignment.BottomOuter, Vertical.BottomOuter },
            };

        public static float GetValue(VerticalAlignment alignment, ScreenOverflowStrategy overflow, Bounds refBounds, float refTargetExtents, Bounds screenBounds, float screenTargetExtents)
        {
            return _verticalStrategies[alignment](refBounds, refTargetExtents, screenBounds, screenTargetExtents, overflow);
        }

        private static class Vertical
        {
            public static float TopOuter(Bounds refBounds, float refTargetExtents, Bounds screenBounds, float screenTargetExtents, ScreenOverflowStrategy overflow)
            {
                return GetPreferredValue(true,
                    refBounds.min.y - refTargetExtents,
                    refBounds.max.y + refTargetExtents,
                    screenBounds.min.y, screenBounds.max.y, screenTargetExtents, overflow);
            }

            public static float TopInner(Bounds refBounds, float refTargetExtents, Bounds screenBounds, float screenTargetExtents, ScreenOverflowStrategy overflow)
            {
                return GetPreferredValue(true,
                    refBounds.min.y + refTargetExtents,
                    refBounds.max.y - refTargetExtents,
                    screenBounds.min.y, screenBounds.max.y, screenTargetExtents, overflow);
            }

            public static float Center(Bounds refBounds, float refTargetExtents, Bounds screenBounds, float screenTargetExtents, ScreenOverflowStrategy overflow)
            {
                return Clamp(refBounds.center.y, screenBounds.min.y, screenBounds.max.y, screenTargetExtents, overflow);
            }

            public static float BottomInner(Bounds refBounds, float refTargetExtents, Bounds screenBounds, float screenTargetExtents, ScreenOverflowStrategy overflow)
            {
                return GetPreferredValue(false,
                    refBounds.min.y + refTargetExtents,
                    refBounds.max.y - refTargetExtents,
                    screenBounds.min.y, screenBounds.max.y, screenTargetExtents, overflow);
            }

            public static float BottomOuter(Bounds refBounds, float refTargetExtents, Bounds screenBounds, float screenTargetExtents, ScreenOverflowStrategy overflow)
            {
                return GetPreferredValue(false,
                    refBounds.min.y - refTargetExtents,
                    refBounds.max.y + refTargetExtents,
                    screenBounds.min.y, screenBounds.max.y, screenTargetExtents, overflow);
            }
        }
    }

    public static class ScreenAlignerUtilities
    {
        public static Bounds GetBounds(this RectTransform rectTransform)
        {
            Vector3[] corners = new Vector3[4];
            rectTransform.GetWorldCorners(corners);
            Vector3 worldBounds = new Vector3(corners[2].x - corners[0].x, corners[1].y - corners[0].y, 0f);
            Vector3 center = new Vector3((corners[2].x + corners[0].x) * 0.5f, (corners[2].y + corners[0].y) * 0.5f, rectTransform.position.z);
            Bounds bounds = new Bounds(center, worldBounds);
            return bounds;
        }
    }
}