using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuUIAnimator : MonoBehaviour
{
    [SerializeField] private AnimationCurve animationCurve;
    [SerializeField] private float animationDuration;
    [SerializeField] private RectTransform uiTransformToAnimate;

    [SerializeField, Header("Positions for main menu, settings and HUB screens")]
    private List<Vector2> screenPositions;

    private Coroutine _animationCoroutine;

    public void AnimateScreenMovement(int screen)
    {
        if (_animationCoroutine != null)
            StopCoroutine(_animationCoroutine);
        _animationCoroutine = StartCoroutine(ScreenMovementCrt(screenPositions[screen]));
    }

    private IEnumerator ScreenMovementCrt(Vector2 newPos)
    {
        float t = 0;
        var startPos = uiTransformToAnimate.anchoredPosition;
        while (t < animationDuration)
        {
            yield return null;
            t += Time.deltaTime;
            uiTransformToAnimate.anchoredPosition =
                Vector2.Lerp(startPos, newPos, animationCurve.Evaluate(t / animationDuration));
        }
    }
}