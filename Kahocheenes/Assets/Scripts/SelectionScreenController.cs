using System;
using System.Collections.Generic;
using UnityEngine;

public class SelectionScreenController : MonoBehaviour
{
    [SerializeField] private UnityEventInteger onVotesChanged;

    [SerializeField] private List<ColorSelection> availableColors;
    [SerializeField] private List<GameObject> carPrefabs;
    [SerializeField] private SelectionScreenUIController uiController;

    private List<int> _selectedCars = new List<int>();
    private List<int> _selectedColors = new List<int>();
    private List<bool> _currentVotes = new List<bool>();
    private int _matchStartVotes;

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

    private void Start()
    {
        ClientManager.Instance.OnPlayerCreated.AddListener(OnPlayerCreated);
    }

    public void OnScreenChanged(int screenIndex)
    {
        if ((MenuController.UIComponentScreen) screenIndex == MenuController.UIComponentScreen.GameHub)
            uiController.DeactivateAllPanels();
        _currentVotes.Clear();
        _selectedCars.Clear();
        _selectedColors.Clear();
        _matchStartVotes = 0;
        foreach (var color in availableColors)
            color.IsSelected = false;
    }

    public void OnPlayerCreated(GameObject playerController)
    {
        _currentVotes.Add(false);
        _selectedCars.Add(0);
        _selectedColors.Add(0);
        SwitchColor(_selectedColors.Count, true);
        SwitchCar(_selectedColors.Count, true);
        uiController.PlayerConnected();
        onVotesChanged.Invoke(MatchStartVotes);
    }

    public void ActionPressed(int playerID)
    {
        int playerIndex = playerID - 1;
        _currentVotes[playerIndex] = !_currentVotes[playerIndex];
        MatchStartVotes += _currentVotes[playerIndex] ? 1 : -1;

        uiController.PlayerChangeVote(playerIndex, _currentVotes[playerIndex]);
    }


    public void SwitchColor(int playerID, bool isRight)
    {
        int playerIndex = playerID - 1;
        int colorIndex = FindColorIndex(_selectedColors[playerIndex], isRight);
        availableColors[_selectedColors[playerIndex]].IsSelected = false;
        _selectedColors[playerIndex] = colorIndex;
        availableColors[_selectedColors[playerIndex]].IsSelected = true;
        uiController.ChangeMaterial(playerIndex, availableColors[colorIndex].PlayerColor);
    }

    public void SwitchCar(int playerID, bool isUp)
    {
        int playerIndex = playerID - 1;
        int carIndex = TrueMod(_selectedCars[playerIndex] + (isUp ? 1 : -1), carPrefabs.Count);
        _selectedCars[playerIndex] = carIndex;
        uiController.ChangeVehicle(playerIndex, carIndex);
    }

    private void OnDestroy()
    {
        ClientManager.Instance.OnPlayerCreated.RemoveListener(OnPlayerCreated);
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