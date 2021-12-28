using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerPositionTracker : MonoBehaviour
{
    [SerializeField] private SplineController spline;

    private List<GameplayControlsHandler> _players = new List<GameplayControlsHandler>();

    public Vector3 AveragePosition { get; private set; }

    public Vector3 AveragePositionOnSpline => spline.FindClosestPointOnSpline(AveragePosition);

    public Vector3 AveragePositionForward => spline.FindForwardVectorForT(spline.FindTForPoint(AveragePosition));

    public Vector3 StartPosition => spline.GetPrecalculatedPoint(0);

    public Vector3 StartPositionForward => spline.FindForwardVectorForT(0);

    private void Start()
    {
        int numOfPlayers = ClientManager.Instance.NumOfPlayers;
        for (int i = 0; i < numOfPlayers; ++i)
            _players.Add(
                (GameplayControlsHandler) ClientManager.Instance.GetPlayerController(i).CurrentControlsHandler);
    }

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
    }
}