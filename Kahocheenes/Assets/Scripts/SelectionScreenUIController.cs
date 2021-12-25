using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SelectionScreenUIController : MonoBehaviour
{
    [SerializeField] private List<SelectionPanelController> playerSelectionPanels;
    [SerializeField] private TextMeshProUGUI gameCodeText;

    private int _numOfPlayers;

    private void Start()
    {
        NetworkControllerManager.Instance.OnConnectionCodeReceived.AddListener(SetGameCode);
    }

    public void ChangeVehicle(int index, int carIndex) => playerSelectionPanels[index].ChangeVehicle(carIndex);

    public void ChangeMaterial(int index, PlayerColorSO playerColor) =>
        playerSelectionPanels[index].ChangeMaterial(playerColor);

    public void PlayerChangeVote(int index, bool isFor) => playerSelectionPanels[index].ChangeVote(isFor);

    public void PlayerConnected() => playerSelectionPanels[_numOfPlayers++].Activate();

    public void SetGameCode(string code) => gameCodeText.text = code;

    public void DeactivateAllPanels()
    {
        foreach (var panel in playerSelectionPanels)
            panel.Deactivate();
        _numOfPlayers = 0;
    }
    
    private void OnDestroy()
    {
        NetworkControllerManager.Instance.OnConnectionCodeReceived.RemoveListener(SetGameCode);
    }
}