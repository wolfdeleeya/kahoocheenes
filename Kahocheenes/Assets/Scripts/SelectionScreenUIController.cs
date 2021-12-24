using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SelectionScreenUIController : MonoBehaviour
{
    [SerializeField] private List<SelectionPanelController> playerSelectionPanels;
    [SerializeField] private TextMeshProUGUI voteCountText;

    private int _numOfPlayers;

    public void ChangeVehicle(int index, bool isUp) => playerSelectionPanels[index].ChangeVehicle(isUp);

    public void ChangeMaterial(int index, PlayerColorSO playerColor) =>
        playerSelectionPanels[index].ChangeMaterial(playerColor);

    public void PlayerChangeVote(int index, bool isFor) => playerSelectionPanels[index].ChangeVote(isFor);

    public void PlayerConnected() => playerSelectionPanels[_numOfPlayers++].Activate();

    public void OnVoteCountChanged(int count) => voteCountText.text = count + " / " + _numOfPlayers;
}