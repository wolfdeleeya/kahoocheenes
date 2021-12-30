using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.Events;

public class PlayerPositionTracker : MonoBehaviour
{
    [SerializeField] private SplineController spline;
    [SerializeField] private float distanceToDestroy;

    [Header("Destruction Detection Debug")] [SerializeField]
    private float averagePositionSphereRadius;

    [SerializeField] private Color averagePositionSphereColor;
    [SerializeField] private Color projectionLineColor;


    private List<GameplayControlsHandler> _players = new List<GameplayControlsHandler>();

    public Vector3 AveragePosition { get; private set; }

    public Vector3 AveragePositionOnSpline => spline.FindClosestPointOnSpline(AveragePosition);

    public Vector3 AveragePositionForward => spline.FindForwardVectorForT(spline.FindTForPoint(AveragePosition));

    public Vector3 StartPosition => spline.GetPrecalculatedPoint(0);

    public Vector3 StartPositionForward => spline.FindForwardVectorForT(0);

    public void AddHandler(GameplayControlsHandler handler) => _players.Add(handler);

    private void Update()
    {
        var averagePosition = Vector3.zero;
        int numOfActivePlayers = 0;
        foreach (var handler in _players)
        {
            if (handler.CarTransform)
            {
                averagePosition += handler.CarTransform.position;
                ++numOfActivePlayers;
            }
        }

        if (numOfActivePlayers > 0)
            AveragePosition = averagePosition / numOfActivePlayers;

        var backward = -AveragePositionForward;
        GameplayControlsHandler worstHandler = _players[0];
        float worstDistance = 0;
        
        var avPosOnSpline = AveragePositionOnSpline;
        
        foreach (var handler in _players)
        {
            if (handler.CarTransform)
            {
                var pos = handler.CarTransform.position;
                var delta = pos - avPosOnSpline;
                float backDistance = Vector3.Dot(backward, delta);
                if (backDistance > worstDistance)
                {
                    worstDistance = backDistance;
                    worstHandler = handler;
                }
            }
        }
        
        if(worstDistance > distanceToDestroy)
            worstHandler.DestroyPlayer();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = averagePositionSphereColor;
        var avPosOnLine = AveragePositionOnSpline;
        Gizmos.DrawSphere(avPosOnLine, averagePositionSphereRadius);

        Gizmos.color = projectionLineColor;
        Gizmos.DrawLine(avPosOnLine, avPosOnLine - AveragePositionForward * distanceToDestroy);
    }
}