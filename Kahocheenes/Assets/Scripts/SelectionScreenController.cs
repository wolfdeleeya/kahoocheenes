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

    public void OnPlayerCreated(GameObject playerController)
    {
        _currentVotes.Add(false);
        uiController.PlayerConnected();
        onVotesChanged.Invoke(MatchStartVotes);
        //dodaj inicijalizaciju početnih vozila i boja
    }

    public void ActionPressed(int playerID)
    {
        int playerIndex = playerID - 1;
        _currentVotes[playerIndex] = !_currentVotes[playerIndex];
        MatchStartVotes += _currentVotes[playerIndex] ? 1 : -1;
        _selectedCars.Add(0);
        
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
        int carIndex = (_selectedCars[playerIndex] + (isUp ? 1 : -1)) % carPrefabs.Count;
        _selectedCars[playerIndex] = carIndex;
        uiController.ChangeVehicle(playerIndex, isUp);
    }

    private void OnDestroy()
    {
        ClientManager.Instance.OnPlayerCreated.RemoveListener(OnPlayerCreated);
    }

    private int FindColorIndex(int startFromIndex, bool goRight)
    {
        int numberOfColors = availableColors.Count;
        int increment = goRight ? 1 : -1;
        for (int i = startFromIndex + 1; i != startFromIndex; i += increment)
        {
            i %= numberOfColors;
            if (!availableColors[i].IsSelected)
                return i;
        }

        return startFromIndex;
    }

    [Serializable]
    public class ColorSelection
    {
        public PlayerColorSO PlayerColor;
        [HideInInspector] public bool IsSelected;
    }
}