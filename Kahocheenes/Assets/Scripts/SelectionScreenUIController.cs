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

        var playerSelManager = PlayerSelectionManager.Instance;
        playerSelManager.OnSelectionCleared.AddListener(DeactivateAllPanels);
        playerSelManager.OnSelectionAdded.AddListener(PlayerConnected);
        playerSelManager.OnPlayerVehicleChanged.AddListener(ChangeVehicle);
        playerSelManager.OnPlayerVehicleMaterialChanged.AddListener(ChangeMaterial);
        playerSelManager.OnPlayerVoteChanged.AddListener(PlayerChangeVote);
    }

    public void ChangeVehicle(int playerId) =>
        playerSelectionPanels[playerId - 1].ChangeVehicle(PlayerSelectionManager.Instance.GetPlayerCarIndex(playerId));

    public void ChangeMaterial(int playerId) =>
        playerSelectionPanels[playerId - 1].ChangeMaterial(PlayerSelectionManager.Instance.GetPlayerColor(playerId));

    public void PlayerChangeVote(int playerId) => playerSelectionPanels[playerId - 1]
        .ChangeVote(PlayerSelectionManager.Instance.GetPlayerVote(playerId));

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
        
        var playerSelManager = PlayerSelectionManager.Instance;
        playerSelManager.OnSelectionCleared.RemoveListener(DeactivateAllPanels);
        playerSelManager.OnSelectionAdded.RemoveListener(PlayerConnected);
        playerSelManager.OnPlayerVehicleChanged.RemoveListener(ChangeVehicle);
        playerSelManager.OnPlayerVehicleMaterialChanged.RemoveListener(ChangeMaterial);
        playerSelManager.OnPlayerVoteChanged.RemoveListener(PlayerChangeVote);
    }
}