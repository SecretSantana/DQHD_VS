using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public interface ITween
{
    bool IsDone { get; }
    event Action OnDone;
    IEnumerator WaitUntilDone();

    // Only usable if not in a sequence, must kill the sequence instead
    void Kill(bool jumpToEnd);

    ulong Id { get; }
}

public interface ITweenStoryboard
{
    // Only usable on the frame that both the storyboard and tween were created
    void Append(ITween tween);
    void Insert(float time, ITween tween);

    bool IsDone { get; }
    event Action OnDone;
    IEnumerator WaitUntilDone();

    void Kill(bool jumpToEnd);

    ulong Id { get; }
}

public interface ITweenAction
{
    // Called immediately before starting to tween, after any delay or storyboard waiting
    // Here you can capture starting values for lerping
    void Begin();

    // Called every frame while running from t=0 until t=1
    // Here you can check state still valid, lerp your value, apply it to your target
    bool Set(float t);
}

public static class TweenExtensions
{
    public static ITween TweenColor(this Graphic graphic, Color end, float duration, Ease ease = Ease.Linear, object owner = null, float delay = 0f)
    {
        return TweenManager.Create(new TweenColorAction(graphic, end), duration, ease, owner, delay);
    }
    private class TweenColorAction : ITweenAction
    {
        private Graphic _graphic;
        private Color _start;
        private Color _end;
        public TweenColorAction(Graphic graphic, Color end)
        {
            _graphic = graphic;
            _end = end;
        }
        public void Begin()
        {
            _start = _graphic.color;
        }
        public bool Set(float t)
        {
            if (_graphic == null)
                return false;
            _graphic.color = Color.Lerp(_start, _end, t);
            return true;
        }
    }

    public static ITween TweenAlpha(this Graphic graphic, float end, float duration, Ease ease = Ease.Linear, object owner = null, float delay = 0f)
    {
        return TweenManager.Create(new TweenAlphaAction(graphic, end), duration, ease, owner, delay);
    }
    private class TweenAlphaAction : ITweenAction
    {
        private Graphic _graphic;
        private float _start;
        private float _end;
        private Color _current;
        public TweenAlphaAction(Graphic graphic, float end)
        {
            _graphic = graphic;
            _end = end;
        }
        public void Begin()
        {
            _current = _graphic.color;
            _start = _current.a;
        }
        public bool Set(float t)
        {
            if (_graphic == null)
                return false;
            _current.a = Mathf.Lerp(_start, _end, t);
            _graphic.color = _current;
            return true;
        }
    }

    public static ITween TweenAlpha(this CanvasGroup canvasGroup, float end, float duration, Ease ease = Ease.Linear, object owner = null, float delay = 0f)
    {
        return TweenManager.Create(new TweenAlphaCanvasGroupAction(canvasGroup, end), duration, ease, owner, delay);
    }
    private class TweenAlphaCanvasGroupAction : ITweenAction
    {
        private CanvasGroup _canvasGroup;
        private float _start;
        private float _end;
        public TweenAlphaCanvasGroupAction(CanvasGroup canvasGroup, float end)
        {
            _canvasGroup = canvasGroup;
            _end = end;
        }
        public void Begin()
        {
            _start = _canvasGroup.alpha;
        }
        public bool Set(float t)
        {
            if (_canvasGroup == null)
                return false;
            _canvasGroup.alpha = Mathf.Lerp(_start, _end, t);
            return true;
        }
    }

    public static ITween TweenLocalPosition(this Transform transform, Vector3 end, float duration, Ease ease = Ease.Linear, object owner = null, float delay = 0f)
    {
        return TweenManager.Create(new TweenLocalPositionAction(transform, end), duration, ease, owner, delay);
    }
    private class TweenLocalPositionAction : ITweenAction
    {
        private Transform _transform;
        private Vector3 _start;
        private Vector3 _end;
        public TweenLocalPositionAction(Transform transform, Vector3 end)
        {
            _transform = transform;
            _end = end;
        }
        public void Begin()
        {
            _start = _transform.localPosition;
        }
        public bool Set(float t)
        {
            if (_transform == null)
                return false;
            _transform.localPosition = Vector3.Lerp(_start, _end, t);
            return true;
        }
    }
}

public class TweenManager
{
    private static List<Tween> _tweens;
    private static List<Storyboard> _storyboards;
    private static ulong _nextId;

    public static ITween Create(ITweenAction tweenAction, float duration, Ease ease = Ease.Linear, object owner = null, float delay = 0f)
    {
        if (tweenAction == null)
            throw new ArgumentNullException("tweenAction");
        if (duration <= 0)
            throw new ArgumentException("duration must be greater than 0");
        if (delay < 0)
            throw new ArgumentException("delay must be greater than or equal to 0");

        Tween tween = new Tween()
        {
            Id = _nextId++,
            TweenAction = tweenAction,
            Duration = duration,
            Ease = ease,
            Owner = owner,
            Delay = delay,
            StartFrameCount = Time.frameCount,
            State = RunningState.Pending,
        };
        _tweens.Add(tween);
        return tween;
    }

    public static void Kill(ITween tween, bool jumpToEnd)
    {
        if (tween == null)
            throw new ArgumentNullException("tween");

        Tween thisTween;
        if (!FindTween(tween.Id, out thisTween))
        {
            if (FindTweenInStoryboard(tween.Id))
                throw new ArgumentException("can't directly kill a tween in a storyboard, can only kill the storyboard");
            throw new ArgumentException("tween was not found in manager");
        }

        KillInternal(thisTween, jumpToEnd);
        _tweens.Remove(thisTween);
    }

    public static int KillAll(object owner, bool jumpToEnd)
    {
        if (owner == null)
            throw new ArgumentNullException("owner");

        int tweens = KillAllTweens(owner, jumpToEnd);
        int storyboards = KillAllStoryboards(owner, jumpToEnd);
        return tweens + storyboards;
    }

    private static int KillAllTweens(object owner, bool jumpToEnd)
    {
        List<Tween> tweensToRemove = new List<Tween>();
        foreach (Tween tween in _tweens.Where(t => t.Owner == owner))
        {
            KillInternal(tween, jumpToEnd);
            tweensToRemove.Add(tween);
        }

        foreach (Tween tween in tweensToRemove)
        {
            _tweens.Remove(tween);
        }

        return tweensToRemove.Count;
    }

    public static void KillStoryboard(ITweenStoryboard storyboard, bool jumpToEnd)
    {
        if (storyboard == null)
            throw new ArgumentNullException("storyboard");
        Storyboard thisStoryboard;
        if (!FindStoryboard(storyboard.Id, out thisStoryboard))
            throw new ArgumentException("storyboard was not found in manager");

        KillStoryboardInternal(thisStoryboard, jumpToEnd);
        _storyboards.Remove(thisStoryboard);
    }

    private static int KillAllStoryboards(object owner, bool jumpToEnd)
    {
        if (owner == null)
            throw new ArgumentNullException("owner");

        List<Storyboard> storyboardsToRemove = new List<Storyboard>();
        foreach (Storyboard storyboard in _storyboards.Where(s => s.Owner == owner))
        {
            KillStoryboardInternal(storyboard, jumpToEnd);
            storyboardsToRemove.Add(storyboard);
        }

        foreach (Storyboard storyboard in storyboardsToRemove)
        {
            _storyboards.Remove(storyboard);
        }

        return storyboardsToRemove.Count;
    }

    public static ITweenStoryboard CreateStoryboard(object owner = null, bool repeat = false)
    {
        Storyboard storyboard = new Storyboard()
        {
            Id = _nextId++,
            Repeat = repeat,
            State = RunningState.Pending,
            Owner = owner,
        };
        _storyboards.Add(storyboard);
        return storyboard;
    }

    public static void StoryboardAppend(ITweenStoryboard storyboard, ITween tween)
    {
        if (storyboard == null)
            throw new ArgumentNullException("storyboard");
        Storyboard thisStoryboard;
        if (!FindStoryboard(storyboard.Id, out thisStoryboard))
            throw new ArgumentException("storyboard was not found in manager");
        if (thisStoryboard.State != RunningState.Pending)
            throw new ArgumentException("storyboard is already being processed");

        if (tween == null)
            throw new ArgumentNullException("tween");
        Tween thisTween;
        if (!FindTween(tween.Id, out thisTween))
        {
            if (FindTweenInStoryboard(tween.Id))
                throw new ArgumentException("tween is already in a storyboard");
            throw new ArgumentException("tween was not found in manager");
        }
        if (thisTween.State != RunningState.Pending)
            throw new ArgumentException("tween is already being processed");

        float maxEndTime = thisStoryboard.Tweens.Count == 0
            ? 0
            : thisStoryboard.Tweens.Max(t => t.Delay + t.Duration);
        thisTween.Delay += maxEndTime;
        thisStoryboard.Tweens.Add(thisTween);
        _tweens.Remove(thisTween);
    }

    public static void StoryboardInsert(ITweenStoryboard storyboard, ITween tween, float atTime)
    {
        if (storyboard == null)
            throw new ArgumentNullException("storyboard");
        Storyboard thisStoryboard;
        if (!FindStoryboard(storyboard.Id, out thisStoryboard))
            throw new ArgumentException("storyboard was not found in manager");
        if (thisStoryboard.State != RunningState.Pending)
            throw new ArgumentException("storyboard is already being processed");

        if (tween == null)
            throw new ArgumentNullException("tween");
        Tween thisTween;
        if (!FindTween(tween.Id, out thisTween))
        {
            if (FindTweenInStoryboard(tween.Id))
                throw new ArgumentException("tween is already in a storyboard");
            throw new ArgumentException("tween was not found in manager");
        }
        if (thisTween.State != RunningState.Pending)
            throw new ArgumentException("tween is already being processed");

        if (atTime < 0)
            throw new ArgumentException("atTime must be greater than or equal to 0");

        thisTween.Delay += atTime;
        thisStoryboard.Tweens.Add(thisTween);
        _tweens.Remove(thisTween);
    }

    private static void KillInternal(Tween tween, bool jumpToEnd)
    {
        if (jumpToEnd)
        {
            tween.TweenAction.Set(1f);
        }
        tween.State = RunningState.Done;
        tween.InvokeOnDone();
    }

    private static void KillStoryboardInternal(Storyboard storyboard, bool jumpToEnd)
    {
        IEnumerable<Tween> tweens = storyboard.Tweens;
        if (jumpToEnd && storyboard.Tweens.Count > 1)
        {
            tweens = tweens.OrderBy(t => t.Delay + t.Duration);
        }
        foreach (Tween tween in tweens)
        {
            if (jumpToEnd)
            {
                tween.TweenAction.Set(1f);
            }
            if (tween.State != RunningState.Done)
            {
                tween.State = RunningState.Done;
                tween.InvokeOnDone();
            }
        }

        storyboard.State = RunningState.Done;
        storyboard.InvokeOnDone();
    }

    private static bool FindTween(ulong id, out Tween tween)
    {
        tween = _tweens.Find(t => t.Id == id);
        return tween != null;
    }

    private static bool FindStoryboard(ulong id, out Storyboard storyboard)
    {
        storyboard = _storyboards.Find(s => s.Id == id);
        return storyboard != null;
    }

    private static bool FindTweenInStoryboard(ulong id)
    {
        return _storyboards.Exists(s => s.Tweens.Exists(t => t.Id == id));
    }

    private static void Update()
    {
        for (int index = _tweens.Count - 1; index >= 0; --index)
        {
            Tween tween = _tweens[index];
            ProcessTween(tween);
            if (tween.IsDone)
            {
                _tweens.RemoveAt(index);
            }
        }

        for (int index = _storyboards.Count - 1; index >= 0; --index)
        {
            Storyboard storyboard = _storyboards[index];
            ProcessStoryboard(storyboard);
            if (storyboard.IsDone)
            {
                _storyboards.RemoveAt(index);
            }
        }
    }

    private static void ProcessTween(Tween tween)
    {
        if (tween.State == RunningState.Pending)
        {
            tween.State = RunningState.Waiting;
        }

        if (tween.StartFrameCount == Time.frameCount)
            return;

        tween.Elapsed += Time.deltaTime;
        float activeElapsed = tween.Elapsed - tween.Delay;
        if (activeElapsed < 0f)
            return;

        if (tween.State == RunningState.Waiting)
        {
            tween.State = RunningState.Running;
            tween.TweenAction.Begin();
        }

        float t = Mathf.Clamp01(activeElapsed / tween.Duration);
        t = tween.Ease.Calculate(t);
        bool success = tween.TweenAction.Set(t);

        if (!success || activeElapsed >= tween.Duration)
        {
            tween.State = RunningState.Done;
            tween.InvokeOnDone();
        }
    }

    private static void ProcessStoryboard(Storyboard storyboard)
    {
        if (storyboard.State == RunningState.Pending)
        {
            storyboard.State = RunningState.Running;
        }

        bool allDone = true;
        for (int index = 0; index < storyboard.Tweens.Count; ++index)
        {
            Tween tween = storyboard.Tweens[index];
            if (tween.State != RunningState.Done)
            {
                ProcessTween(tween);
            }
            allDone &= tween.IsDone;
        }

        if (allDone)
        {
            if (storyboard.Repeat)
            {
                foreach (Tween tween in storyboard.Tweens)
                {
                    tween.Reset();
                }
            }
            else
            {
                storyboard.State = RunningState.Done;
                storyboard.InvokeOnDone();
            }
        }
    }

    static TweenManager()
    {
        _tweens = new List<Tween>();
        _storyboards = new List<Storyboard>();

        new GameObject("Tween Manager")
            .AddComponent<UpdateManager>()
            .OnUpdate += Update;
    }

    private class Tween : ITween
    {
        public float Duration { get; set; }
        public Ease Ease { get; set; }
        public object Owner { get; set; }
        public ulong Id { get; set; }
        public float Delay { get; set; }

        public bool IsDone { get { return State == RunningState.Done; } }

        public event Action OnDone;
        public ITweenAction TweenAction;
        public float Elapsed;
        public int StartFrameCount;
        public RunningState State;

        public IEnumerator WaitUntilDone()
        {
            return new WaitUntil(() => IsDone);
        }

        public void Kill(bool jumpToEnd)
        {
            TweenManager.Kill(this, jumpToEnd);
        }

        public void InvokeOnDone()
        {
            OnDone?.Invoke();
        }

        public void Reset()
        {
            Elapsed = 0;
            State = RunningState.Pending;
        }
    }

    private class Storyboard : ITweenStoryboard
    {
        public bool IsDone { get { return State == RunningState.Done; } }
        public bool Repeat;
        public RunningState State;
        public event Action OnDone;
        public ulong Id { get; set; }
        public object Owner;

        public List<Tween> Tweens = new List<Tween>();

        public void Append(ITween tween)
        {
            StoryboardAppend(this, tween);
        }

        public void Insert(float time, ITween tween)
        {
            StoryboardInsert(this, tween, time);
        }

        public void Kill(bool jumpToEnd)
        {
            KillStoryboard(this, jumpToEnd);
        }

        public IEnumerator WaitUntilDone()
        {
            return new WaitUntil(() => IsDone);
        }

        public void InvokeOnDone()
        {
            OnDone?.Invoke();
        }
    }

    private enum RunningState
    {
        Invalid,

        Pending, // created but hasn't been processed (can be storyboarded)
        Waiting, // waiting for delay to expire
        Running, // started and running now
        Done, // done
    }
}