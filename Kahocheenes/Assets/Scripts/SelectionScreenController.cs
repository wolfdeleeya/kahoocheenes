using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionScreenController : MonoBehaviour
{
    [SerializeField] private UnityEventInteger onVotesChanged;
    
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
        }
    }

    private void Start()
    {
        NetworkControllerManager.Instance.OnClientConnected.AddListener(OnPlayerConnected);
    }

    public void OnPlayerConnected(int id)
    {
        
    }
}
