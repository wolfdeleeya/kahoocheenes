using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplayCameraController : MonoBehaviour
{
    [SerializeField] private bool destroyPlayersByDistance;
    [SerializeField] private float distanceToDestroyPlayer;
    [SerializeField] private float moveLerpSpeed;
    [SerializeField] private float rotationLerpSpeed;
    [SerializeField] private PlayerPositionTracker positionTracker;

    private Vector3 _goalPos;

    private Transform _transform;

    private GameplayControlsHandler _lastPlayer;

    private void Awake()
    {
        _transform = transform;
    }

    private void Start()
    {
        positionTracker.OnLastPlayerChanged.AddListener(() => _lastPlayer = positionTracker.LastPlayer.ControlsHandler);        //pogledaj ocemo li samo gledati preko forward vektora
    }

    private void Update()
    {
        if (destroyPlayersByDistance && (_transform.position - _lastPlayer.CarTransform.position).magnitude <
            distanceToDestroyPlayer)
            _lastPlayer.DestroyPlayer();
    }

    private void LateUpdate()
    {
        Vector3 currentPos = _transform.position;
        Vector3 avgPosition = positionTracker.AveragePosition;
        Vector3 nextPos = positionTracker.AveragePositionOnSpline;

        Vector3 resultPos = Vector3.Lerp(currentPos, nextPos, moveLerpSpeed * Time.deltaTime);
        _transform.position = resultPos;

        Vector3 forward = (avgPosition - resultPos).normalized;
        Vector3 right = Vector3.Cross(forward, Vector3.up);
        Vector3 up = Vector3.Cross(right, forward);

        Quaternion goalRotation = Quaternion.LookRotation(forward, up);
        _transform.rotation = Quaternion.Slerp(_transform.rotation, goalRotation, rotationLerpSpeed * Time.deltaTime);
    }
}