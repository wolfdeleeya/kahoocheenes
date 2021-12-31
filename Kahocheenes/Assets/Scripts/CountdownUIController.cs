using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CountdownUIController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI countdownDisplay;
    [SerializeField] private float minScale;
    [SerializeField] private float maxScale;
    [SerializeField] private string countdownOverText;
    [SerializeField] private AnimationCurve scaleCurve;
    [SerializeField] private AnimationCurve fadeCurve;
    [SerializeField] private RectTransform rectTransform;

    private Coroutine _coroutine;

    private void Start()
    {
        countdownDisplay.alpha = 0;
    }

    public void ChangeCountDisplay(int num)
    {
        bool isOver = num == 0;
        countdownDisplay.text = isOver ? countdownOverText : num.ToString();
        if (_coroutine != null)
            StopCoroutine(_coroutine);
        _coroutine = StartCoroutine(CountCoroutine(isOver));
    }

    private IEnumerator CountCoroutine(bool isFading)
    {
        float t = 0;
        countdownDisplay.alpha = 1;

        while (t < 1)
        {
            yield return null;
            t += Time.deltaTime;
            rectTransform.localScale = Vector3.one * Mathf.Lerp(minScale, maxScale, scaleCurve.Evaluate(t));
            if (isFading)
                countdownDisplay.alpha = Mathf.Lerp(0, 1, fadeCurve.Evaluate(t));
        }
    }
}