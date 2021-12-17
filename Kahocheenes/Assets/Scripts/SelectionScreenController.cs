using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionScreenController : MonoBehaviour
{
    [SerializeField] private UnityEventInteger onVotesChanged;

    [SerializeField] private List<Tuple<bool, PlayerColorSO>> availableColors;
    
        
    private List<bool> _currentVotes = new List<bool>();
    private int _matchStartVotes;
    
    public int MatchStartVotes
    {
        get
        {
            return _matchStartVotes;
        }

        set
        {
            _matchStartVotes = value;
            if(MatchStartVotes == _currentVotes.Count)                          //maybe add countdown before scene change
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
        var player = playerController.GetComponent<PlayerController>();
        int playerID = player.Id;
        player.CurrentControlsHandler.OnActionPressed.AddListener(() => ActionPressed(playerID));
        _currentVotes.Add(false);
    }

    public void ActionPressed(int playerID)
    {
        int playerIndex = playerID - 1;
        _currentVotes[playerIndex] = !_currentVotes[playerIndex];
        MatchStartVotes += _currentVotes[playerIndex] ? 1 : -1;
    }

    public void LeftPressed(int playerID)
    {
        
    }
    
    public void RightPressed(int playerID)
    {
        
    }
    
    public void UpPressed(int playerID)
    {
        
    }
    
    public void DownPressed(int playerID)
    {
        
    }

    private void OnDestroy()
    {
        ClientManager.Instance.OnPlayerCreated.RemoveListener(OnPlayerCreated);
    }
}           //TODO: NAPRAVITI MIJENJANJE AUTA I BOJE NA INPUT