using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[Serializable]
public static class Interp
{
    public enum Type
    {
        Linear, // x = t
        InSin, // follow a sin-ish curve
        OutSin,
        InOutSin,
        InSinSquared, // more aggressive sin-ish curve
        OutSinSquared,
        InOutSinSquared,
        InSquared, // quadratic curve
        OutSquared,
        InOutSquared,
        InCubed, // steep quadratic curve
        OutCubed,
        InOutCubed,
        InQuad, // steepest quadratic curve
        OutQuad,
        InOutQuad,
        InElastic, // kind of "sproing" into place
        OutElastic,
        InOutElastic,
        InBack, // pull outside the range, then slide into place
        OutBack, // or move past the target, then return to final place
        InOutBack,
    }

    /// <summary>
    /// Scale value between 0 and 1 based on interpolation type.
    /// </summary>
    /// <param name="type">Interpolation type to use.</param>
    /// <param name="t">value between 0 and 1 to move along curve.
    /// Values outside this range will be clamped.</param>
    /// <returns>Scaled value. Note that this value isn't guaranteed
    /// to be between 0 and 1 for all interpolation types.</returns>
    public static float GetValue(Type type, float t)
    {
        return GetValueUnclamped(type, Mathf.Clamp01(t));
    }

    /// <summary>
    /// Scale a value based on interpolation type.
    /// </summary>
    /// <param name="type">Interpolation type to use.</param>
    /// <param name="t">Value along curve. 0 represents the beginning,
    /// 1 represents the end. Values outside this range are allowed but
    /// may behave unpredictably for some interp types.</param>
    /// <returns>Scaled value.</returns>
    public static float GetValueUnclamped(Type type, float t)
    {
        return type switch
        {
            Type.Linear => t,
            Type.InSin => 1f - Mathf.Sin(Mathf.PI / 2f + Mathf.PI / 2f * t),
            Type.OutSin => Mathf.Sin(Mathf.PI / 2f * t),
            Type.InOutSin => (Mathf.Sin(Mathf.PI * t - Mathf.PI / 2f) + 1f) / 2f,
            Type.InSinSquared => 1f - Mathf.Sin(Mathf.PI / 2f + Mathf.PI / 2f * t * t),
            Type.OutSinSquared => Mathf.Sin(Mathf.PI / 2f * t * t),
            Type.InOutSinSquared => (Mathf.Sin(Mathf.PI * t * t - Mathf.PI / 2f) + 1f) / 2f,
            Type.InSquared => t * t,
            Type.OutSquared => 2 * t - t * t,
            Type.InOutSquared => t < 0.5f ? t * t * 2 : -1f + 4f * t - 2f * t * t,
            Type.InCubed => t * t * t,
            Type.OutCubed => 1f - (1f - t) * (1f - t) * (1f - t),
            Type.InOutCubed => t < 0.5f ? 4 * t * t * t : (
                    1f - (1f - (t * 2 - 1f)) * (1f - (t * 2 - 1f)) * (1f - (t * 2 - 1f))
                    ) / 2f + 0.5f,
            Type.InQuad => t * t * t * t,
            Type.OutQuad => 1f - (1f - t) * (1f - t) * (1f - t) * (1f - t),
            Type.InOutQuad => t < 0.5f ? 8 * t * t * t * t : (1f - 
                (1f - (t * 2 - 1f)) * (1f - (t * 2 - 1f)) * (1f - (t * 2 - 1f))
                * (1f - (t * 2 - 1f))) / 2f + 0.5f,
            Type.InElastic => -Mathf.Pow(2f, 10 * t - 10) * Mathf.Sin((t * 10 - 10.75f) * (2 * Mathf.PI) / 3f),
            Type.OutElastic => Mathf.Pow(2f, -10 * t) * Mathf.Sin((t * 10 - 0.75f) * (2 * Mathf.PI) / 3f) + 1f,
            Type.InOutElastic => t < 0.5f ? -Mathf.Pow(2f, 20 * t - 10) * Mathf.Sin((20 * t - 11.125f) * (2 * Mathf.PI) / 4.5f) / 2f :
                Mathf.Pow(2f, -20 * t + 10) * Mathf.Sin((20 * t - 11.125f) * (2 * Mathf.PI) / 4.5f) / 2f + 1f,
            Type.InBack => t * t * ((1.7f + 1f) * t - 1.7f),
            Type.OutBack => (t - 1f) * (t - 1f) * ((1.7f + 1f) * (t - 1f) + 1.7f) + 1f,
            Type.InOutBack => t < 0.5f ? 2 * t * t * ((1.7f + 1f) * t * 2 - 1.7f) :
                ((2 * t - 2f) * (2 * t - 2f) * ((1.7f + 1f) * (2 * t - 2f) + 1.7f)
                 + 1f) / 2f + 0.5f, // somehow this feels even worse
            _ => t
        };
    }

    /// <summary>
    /// Interpolate between two values along a specified curve.
    /// </summary>
    /// <param name="type">Interpolation type to use.</param>
    /// <param name="start">Value for when t = 0.</param>
    /// <param name="end">Value for when t = 1.</param>
    /// <param name="t">Value between 0 and 1 to move along curve.
    /// If it's outside this range, it will be clamped.</param>
    /// <returns>Scaled value. Note for some interp types, this
    /// value could fall outside the range of [start, end].</returns>
    public static float Erp(Type type, float start, float end, float t)
    {
        return Mathf.LerpUnclamped(start, end, GetValue(type, t));
    }

    /// <summary>
    /// Interpolate between two values along a specified curve.
    /// </summary>
    /// <param name="type">Interpolation type to use.</param>
    /// <param name="start">Value for when t = 0.</param>
    /// <param name="end">Value for when t = 1.</param>
    /// <param name="t">Value along curve. 0 represents the beginning,
    /// 1 represents the end. Values outside this range are allowed but
    /// may behave unpredictably for some interp types.</param>
    /// <returns>Scaled value.</returns>
    public static float ErpUnclamped(Type type, float start, float end, float t)
    {
        return Mathf.LerpUnclamped(start, end, GetValueUnclamped(type, t));
    }
}

[Serializable]
public class Transition
{
    public enum Style
    {
        MoveX,
        MoveY,
        Fade,
        RotateZ,
        Wait,
    }

    [SerializeField] public Style action;
    [SerializeField] public float time;
    [Tooltip("The amount to change the value by over the duration of the animation.\n" +
             "For example, put \'-0.5\' for Fade to make target fade away by 50%.")]
    [SerializeField] public float amount;
    [Tooltip("The target to control for this animator.\n" +
             "If none is specified, it will default to whatever object ran the animation.")]
    [SerializeField] public GameObject target;
    [SerializeField] public Interp.Type interpolation;
    [Tooltip("If true, this will happen at the same time as the previous item in the list.\n" +
             "Otherwise it will happen after it finishes.")]
    [SerializeField] public bool withPrevious = false;

    private float _timeElapsed;

    public bool IsAnimating => _timeElapsed > 0;

    /// <summary>
    /// Coroutine to animate this
    /// Calls onFinish on the frame the animation reaches its end
    /// </summary>
    public IEnumerator Animate(Action onFinish = null)
    {
        _timeElapsed = 0;
        switch (action)
        {
            case Style.MoveX:
            {
                var t = target.transform;
                var startPos = t.localPosition.x;
                var endPos = startPos + amount;
                yield return null;
                while (_timeElapsed < time)
                {
                    _timeElapsed += Time.deltaTime;
                    var newPos = Interp.Erp(interpolation, startPos, endPos,
                        _timeElapsed / time);
                    var oldPos = t.localPosition;
                    oldPos.x = newPos;
                    t.localPosition = oldPos;
                    yield return null;
                }

                break;
            }
            case Style.MoveY:
            {
                var t = target.transform;
                var startPos = t.localPosition.y;
                var endPos = startPos + amount;
                yield return null;
                while (_timeElapsed < time)
                {
                    _timeElapsed += Time.deltaTime;
                    var newPos = Interp.Erp(interpolation, startPos, endPos,
                        _timeElapsed / time);
                    var oldPos = t.localPosition;
                    oldPos.y = newPos;
                    t.localPosition = oldPos;
                    yield return null;
                }

                break;
            }
            case Style.Fade:
            {
                var sp = target.GetComponent<SpriteRenderer>();
                var startAlpha = sp.color.a;
                var endAlpha = startAlpha + amount;
                yield return null;
                while (_timeElapsed < time)
                {
                    _timeElapsed += Time.deltaTime;
                    var newAlpha = Interp.Erp(interpolation, startAlpha, endAlpha,
                        _timeElapsed / time);
                    var oldColor = sp.color;
                    oldColor.a = newAlpha;
                    sp.color = oldColor;
                    yield return null;
                }
                break;
            }
            case Style.RotateZ:
            {
                var t = target.transform;
                var startRot = t.localRotation.eulerAngles.z;
                var endRot = startRot + amount;
                yield return null;
                while (_timeElapsed < time)
                {
                    _timeElapsed += Time.deltaTime;
                    var newRot = Interp.Erp(interpolation, startRot, endRot,
                        _timeElapsed / time);
                    var oldRot = t.localRotation;
                    var oldRotEuler = oldRot.eulerAngles;
                    oldRotEuler.z = newRot;
                    oldRot.eulerAngles = oldRotEuler;
                    t.localRotation = oldRot;
                    yield return null;
                }
                break;
            }
            case Style.Wait:
            {
                yield return null;
                while (_timeElapsed < time)
                {
                    _timeElapsed += Time.deltaTime;
                    yield return null;
                }

                break;
            }
        }

        _timeElapsed = 0;
        onFinish?.Invoke();
        yield break;
    }
}

[Serializable]
public class TransitionList
{
    [SerializeField] private List<Transition> list = new();

    public bool IsPlaying { get; private set; } = false;

    private int curIdx = -1;
    
    /// <summary>
    /// Start the animation list. It must not already be playing.
    /// </summary>
    /// <returns>Unity coroutine IEnumerator</returns>
    public IEnumerator Play(GameObject self, Action onFinish = null)
    {
        if (IsPlaying)
        {
            Debug.LogError("Cannot start transition list that\'s already playing!");
            yield break;
        }
        
        IsPlaying = true;
        curIdx = 0;
        while (curIdx < list.Count)
        {
            if (list[curIdx].target == null)
            {
                list[curIdx].target = self;
            }
            yield return list[curIdx].Animate();
            list[curIdx].target = null;
            curIdx++;
        }

        IsPlaying = false;
        onFinish?.Invoke();
    }

    /// <summary>
    /// Add a new transition to the list.
    /// </summary>
    /// <param name="newTransition">The transition to add. It must not be animating already.</param>
    public void Add(Transition newTransition)
    {
        if (newTransition.IsAnimating)
        {
            Debug.LogError("Failed to add new transition! (It was already animating, did you try adding it in multiple places by mistake?)");
            return;
        }
        list.Add(newTransition);
    }

    
    /// <summary>
    /// Remove a transition from the list. If the list is currently playing, this could have unexpected effects.
    /// </summary>
    /// <param name="transition">The transition to remove.</param>
    public void Remove(Transition transition)
    {
        var itemIdx = list.IndexOf(transition);
        if (itemIdx == -1)
        {
            Debug.LogError("Could not find transition in list!");
            return;
        }
        else if (itemIdx <= curIdx)
        {
            Debug.LogWarning("Tried to remove an item from the list that we already passed over, this will have consequences!");
        }
        list.RemoveAt(itemIdx);
    }

    /// <summary>
    /// Clear the list.
    /// </summary>
    public void Clear()
    {
        list.Clear();
    }
}