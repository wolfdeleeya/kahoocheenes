using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class GameplayManager : MonoBehaviour
{
    public UnityEvent OnGameOver;
    public UnityEventListInt OnScoreChanged;
    public UnityEvent OnRoundOver;
    public UnityEventInteger OnCountdownChanged;

    [SerializeField] private float countdownBetweenRounds;
    [SerializeField] private int maxScore;
    [SerializeField] private List<ListInt> listOfScoreDeltas;
    [SerializeField] private PlayerPositionTracker positionTracker;

    private List<int> _scores;
    private int _playersLeft;
    private List<GameplayControlsHandler> _handlers = new List<GameplayControlsHandler>();

    public static GameplayManager Instance { get; private set; }

    public int MaxScore => maxScore;

    private void Awake()
    {
        if (Instance)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void AddPlayer(GameplayControlsHandler handler)
    {
        positionTracker.AddHandler(handler);
        handler.OnPlayerDestroyed.AddListener(PlayerDestroyed);
        _handlers.Add(handler);
        ++_playersLeft;
        if (_playersLeft == ClientManager.Instance.NumOfPlayers)
            StartRound(true);
    }

    public void PlayerDestroyed(int playerID)
    {
        int playerIndex = playerID - 1;
        --_playersLeft;
        int numOfPlayers = _handlers.Count;

        var scoreDeltas = listOfScoreDeltas[numOfPlayers - 2].List; //there's no score deltas for 0 and 1 player games
        int delta = scoreDeltas[_playersLeft];
        _scores[playerIndex] = Mathf.Clamp(_scores[playerIndex], 0, maxScore);
        OnScoreChanged.Invoke(_scores);
        if (_playersLeft == 1)
        {
            RoundOver();
            for(int i = 0;i<_scores.Count;++i)
                if(_scores[i] == maxScore)
                    GameOver(i);
        }
    }

    public void RoundOver()
    {
        OnRoundOver.Invoke();
        Debug.Log("ROUND OVER");
    }

    public void GameOver(int winnerIndex)
    {
        OnGameOver.Invoke();
        Debug.Log("GAME OVER");
    }

    public void StartRound(bool isFirstRound)
    {
        var pos = isFirstRound ? positionTracker.StartPosition : positionTracker.AveragePosition;
        var forward = isFirstRound ? positionTracker.StartPositionForward : positionTracker.AveragePositionForward;
        foreach (var handler in _handlers)
        {
            handler.Spawn(pos, forward);
            handler.CanControl = false;
        }

        StartCoroutine(CountdownCRT());
    }

    private IEnumerator CountdownCRT()
    {
        float t = countdownBetweenRounds;
        int lastCountdownNum = (int) Mathf.Ceil(t);

        while (t > 0)
        {
            yield return null;
            t -= Time.deltaTime;

            int ceilT = (int) Mathf.Ceil(t);
            if (lastCountdownNum != ceilT)
            {
                lastCountdownNum = ceilT;
                OnCountdownChanged.Invoke(lastCountdownNum);
            }
        }

        foreach (var handler in _handlers)
            handler.CanControl = true;
        _playersLeft = _handlers.Count;
    }

    [Serializable]
    public class ListInt
    {
        public List<int> List;
    }
}