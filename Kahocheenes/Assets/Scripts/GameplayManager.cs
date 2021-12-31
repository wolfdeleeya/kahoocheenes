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
    [SerializeField] private List<int> maxScores;
    [SerializeField] private List<ListInt> listOfScoreDeltas;
    [SerializeField] private PlayerPositionTracker positionTracker;

    private List<int> _scores;
    private int _playersLeft;
    private List<GameplayControlsHandler> _handlers;
    private int _maxScore;

    public static GameplayManager Instance { get; private set; }

    public int MaxScore => _maxScore;

    public List<int> CurrentScores => _scores;

    private void Awake()
    {
        if (Instance)
        {
            Destroy(Instance); //Destroy old Instance
        }

        Instance = this;

        DontDestroyOnLoad(gameObject);
        _handlers = new List<GameplayControlsHandler>();
        _scores = new List<int>();
        _maxScore = maxScores[ClientManager.Instance.NumOfPlayers - 2];
        SceneManager.Instance.OnSceneChanged.AddListener(SceneChanged);
    }

    public void SceneChanged(int sceneIndex)
    {
        if (sceneIndex == 0)
            Destroy(gameObject);
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
        _scores[playerIndex] += delta;
        _scores[playerIndex] = Mathf.Clamp(_scores[playerIndex], 0, _maxScore);
        OnScoreChanged.Invoke(_scores);
        if (_playersLeft == 1)
        {
            foreach (var handler in _handlers)
                if (handler.IsAlive)
                {
                    handler.DestroyPlayer();
                    break;
                }
        }
        else if (_playersLeft == 0)
        {
            RoundOver();
            for (int i = 0; i < _scores.Count; ++i)
                if (_scores[i] == _maxScore)
                    GameOver();
        }
    }

    public void RoundOver()
    {
        OnRoundOver.Invoke();
        StartRound(false);
    }

    public void GameOver()
    {
        OnGameOver.Invoke();
        SceneManager.Instance.ChangeScene(SceneManager.Scene.EndScreen);
    }

    public void StartRound(bool isFirstRound)
    {
        if (isFirstRound)
            _scores = new List<int>(new int[_handlers.Count]);
        var pos = isFirstRound ? positionTracker.StartPosition : positionTracker.AveragePosition;
        var forward = isFirstRound ? positionTracker.StartPositionForward : positionTracker.AveragePositionForward;
        foreach (var handler in _handlers)
        {
            handler.Spawn(pos, forward);
            handler.CanControl = false;
        }

        StartCoroutine(CountdownCRT());
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.Space))
            GameOver();
    }

    private void OnDestroy()
    {
        SceneManager.Instance.OnSceneChanged.RemoveListener(SceneChanged);
    }

    private IEnumerator CountdownCRT()
    {
        float t = countdownBetweenRounds;
        int lastCountdownNum = 0;

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