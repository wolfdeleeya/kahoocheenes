using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlacementColumnController : MonoBehaviour
{
    public UnityEvent RiseEvent;
    public UnityEvent OnRiseComplete;

    [SerializeField] private float riseDuration;
    [SerializeField, Range(0, 1)] private float eventRisePoint;
    [SerializeField] private float riseAmount;
    [SerializeField] private AnimationCurve riseCurve;
    [SerializeField] private Transform _spawnPoint;

    private bool _isEventInvoked;
    private Transform _transform;

    private void Awake()
    {
        _transform = transform;
    }

    public void Spawn(int playerID)
    {
        var selectionManager = PlayerSelectionManager.Instance;
        var carPrefab = selectionManager.GetPlayerCarPrefab(playerID).MainMenuPrefab;
        int carIndex = selectionManager.GetPlayerCarIndex(playerID);
        var playerColor = selectionManager.GetPlayerColor(playerID);

        var customization = Instantiate(carPrefab, _spawnPoint).GetComponent<CarCustomizationController>();
        customization.SetMaterial(playerColor.CarMaterials[carIndex], playerColor.IndexesToSet[carIndex].Indexes);
    }

    public void Rise()
    {
        StartCoroutine(RiseCRT());
    }

    private IEnumerator RiseCRT()
    {
        float t = 0;
        Vector3 startPos = _transform.position, endPos = startPos + Vector3.up * riseAmount;
        while (t < riseDuration)
        {
            yield return null;
            t += Time.deltaTime;
            float val = t / riseDuration;
            if (!_isEventInvoked && val > eventRisePoint)
            {
                RiseEvent.Invoke();
                _isEventInvoked = true;
            }

            _transform.position = Vector3.Lerp(startPos, endPos, riseCurve.Evaluate(val));
        }

        _transform.position = endPos;
        OnRiseComplete.Invoke();
    }
}