public enum Ease
{
    Linear,
    In2, Out2, InOut2,
    In3, Out3, InOut3,
    In4, Out4, InOut4,
    In5, Out5, InOut5,
}

public static class EaseFunctions
{
    public static float Calculate(this Ease ease, float t)
    {
        switch (ease)
        {
            case Ease.In2: return t * t;
            case Ease.Out2: return t * (2 - t);
            case Ease.InOut2: return t < .5 ? 2 * t * t : -1 + (4 - 2 * t) * t;

            case Ease.In3: return t * t * t;
            case Ease.Out3: return (--t) * t * t + 1;
            case Ease.InOut3: return t < .5 ? 4 * t * t * t : (t - 1) * (2 * t - 2) * (2 * t - 2) + 1;

            case Ease.In4: return t * t * t * t;
            case Ease.Out4: return 1 - (--t) * t * t * t;
            case Ease.InOut4: return t < .5 ? 8 * t * t * t * t : 1 - 8 * (--t) * t * t * t;

            case Ease.In5: return t * t * t * t * t;
            case Ease.Out5: return 1 + (--t) * t * t * t * t;
            case Ease.InOut5: return t < .5 ? 16 * t * t * t * t * t : 1 + 16 * (--t) * t * t * t * t;

            default:
            case Ease.Linear: return t;
        }
    }
}