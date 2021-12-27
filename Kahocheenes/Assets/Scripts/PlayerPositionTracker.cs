using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerPositionTracker : MonoBehaviour
{
    [HideInInspector] public UnityEvent OnLastPlayerChanged = new UnityEvent();
    [SerializeField] private SplineController spline;

    private List<PositionTracker> _players = new List<PositionTracker>();
    private PositionTracker _lastPlayer;

    public Vector3 AveragePosition { get; private set; }

    public Vector3 AveragePositionOnSpline => spline.FindClosestPointOnSpline(AveragePosition);

    public PositionTracker LastPlayer
    {
        get { return _lastPlayer; }
        private set
        {
            _lastPlayer = value;
            OnLastPlayerChanged.Invoke();
        }
    }

    private void Start()
    {
        int numOfPlayers = ClientManager.Instance.NumOfPlayers;
        for (int i = 0; i < numOfPlayers; ++i)
            _players.Add(
                new PositionTracker(
                    (GameplayControlsHandler) ClientManager.Instance.GetPlayerController(i)
                        .GetComponent<PlayerController>().CurrentControlsHandler, 0));
    }

    private void Update()
    {
        AveragePosition = Vector3.zero;
        int numOfActivePlayers = 0;
        foreach (var player in _players)
        {
            var handler = player.ControlsHandler;
            if (handler.CarTransform)
            {
                var position = handler.CarTransform.position;
                AveragePosition += position; //contribute to avg position
                ++numOfActivePlayers;

                float currentT = spline.FindTForPoint(position);
                float delta = currentT - player.CurrentT;
                if (delta > 0.5f) //passed lap
                    delta = -Mathf.Sign(delta) * (1 - Mathf.Abs(delta));
                player.CurrentT += delta;
                if (player != LastPlayer && player.CurrentT < LastPlayer.CurrentT)  //is last player
                    LastPlayer = player;
            }
        }

        if (numOfActivePlayers > 0)
            AveragePosition /= numOfActivePlayers;
    }

    public class PositionTracker
    {
        public GameplayControlsHandler ControlsHandler;
        public float CurrentT;

        public PositionTracker(GameplayControlsHandler handler, float currentT)
        {
            ControlsHandler = handler;
            CurrentT = currentT;
        }
    }
}