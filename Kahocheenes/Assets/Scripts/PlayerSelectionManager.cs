using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class PlayerSelectionManager : MonoBehaviour
{
    [HideInInspector] public UnityEvent OnSelectionAdded;
    [HideInInspector] public UnityEvent OnSelectionCleared;
    [HideInInspector] public UnityEventInteger OnPlayerVehicleChanged;
    [HideInInspector] public UnityEventInteger OnPlayerVehicleMaterialChanged;
    [HideInInspector] public UnityEventInteger OnPlayerVoteChanged;

    [SerializeField] private UnityEventInteger onVotesChanged;

    [SerializeField] private List<ColorSelection> availableColors;
    [SerializeField] private List<CarPrefabData> carPrefabs;

    private List<int> _selectedCars;
    private List<int> _selectedColors;
    private List<bool> _currentVotes;
    private int _matchStartVotes;

    public static PlayerSelectionManager Instance { get; private set; }

    public int MatchStartVotes
    {
        get { return _matchStartVotes; }

        set
        {
            _matchStartVotes = value;
            if (MatchStartVotes == _currentVotes.Count &&
                _currentVotes.Count > 1) //maybe add countdown before scene change
                SceneManager.Instance.ChangeScene(SceneManager.Scene.Gameplay);
            onVotesChanged.Invoke(MatchStartVotes);
        }
    }

    private void Awake()
    {
        if (Instance)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        
        OnSelectionAdded = new UnityEvent();
        OnSelectionCleared = new UnityEvent();
        OnPlayerVehicleChanged = new UnityEventInteger();
        OnPlayerVehicleMaterialChanged = new UnityEventInteger();
        OnPlayerVoteChanged = new UnityEventInteger();
        
        _selectedCars = new List<int>();
        _selectedColors = new List<int>();
        _currentVotes = new List<bool>();
    }

    public CarPrefabData GetPlayerCarPrefab(int playerId) => carPrefabs[_selectedCars[playerId - 1]];

    public int GetPlayerCarIndex(int playerId) => _selectedCars[playerId - 1];

    public PlayerColorSO GetPlayerColor(int playerId) => availableColors[_selectedColors[playerId - 1]].PlayerColor;

    public bool GetPlayerVote(int playerId) => _currentVotes[playerId - 1];
    
    public void ClearSelection()
    {
        OnSelectionCleared.Invoke();
        _currentVotes.Clear();
        _selectedCars.Clear();
        _selectedColors.Clear();
        _matchStartVotes = 0;
        foreach (var color in availableColors)
            color.IsSelected = false;
    }
    
    public void AddSelection(GameObject playerController)
    {
        _currentVotes.Add(false);
        _selectedCars.Add(0);
        _selectedColors.Add(0);
        OnSelectionAdded.Invoke();
        SwitchColor(_selectedColors.Count, true);
        SwitchCar(_selectedColors.Count, true);
        onVotesChanged.Invoke(MatchStartVotes);
    }

    public void ActionPressed(int playerID)
    {
        int playerIndex = playerID - 1;
        _currentVotes[playerIndex] = !_currentVotes[playerIndex];
        MatchStartVotes += _currentVotes[playerIndex] ? 1 : -1;
        OnPlayerVoteChanged.Invoke(playerID);
    }
    
    public void SwitchColor(int playerID, bool isRight)
    {
        int playerIndex = playerID - 1;
        int colorIndex = FindColorIndex(_selectedColors[playerIndex], isRight);
        availableColors[_selectedColors[playerIndex]].IsSelected = false;
        _selectedColors[playerIndex] = colorIndex;
        availableColors[_selectedColors[playerIndex]].IsSelected = true;
        OnPlayerVehicleMaterialChanged.Invoke(playerID);
    }

    public void SwitchCar(int playerID, bool isUp)
    {
        int playerIndex = playerID - 1;
        int carIndex = TrueMod(_selectedCars[playerIndex] + (isUp ? 1 : -1), carPrefabs.Count);
        _selectedCars[playerIndex] = carIndex;
        OnPlayerVehicleChanged.Invoke(playerID);
    }

    private int FindColorIndex(int startFromIndex, bool goRight)
    {
        int numberOfColors = availableColors.Count;
        int increment = goRight ? 1 : -1;
        for (int i = startFromIndex + increment; i != startFromIndex; i += increment)
        {
            i = TrueMod(i, numberOfColors);
            if (!availableColors[i].IsSelected)
                return i;
        }

        return startFromIndex;
    }


    private int TrueMod(int n, int m)
    {
        if (n >= 0)
            return n % m;

        while (n < 0)
            n += m;
        return n;
    }

    [Serializable]
    public class ColorSelection
    {
        public PlayerColorSO PlayerColor;
        [HideInInspector] public bool IsSelected;
    }
}