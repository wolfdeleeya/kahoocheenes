using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplayControlsHandler : AbstractControlsHandler
{
    [SerializeField] private float spawnDistanceBetweenCars;
    [SerializeField] private Vector3 spawnOffset;

    private BodyController _carController;

    private int _playerId;
    private PlayerPositionTracker _positionTracker;

    public Transform CarTransform { get; private set; }

    public void DestroyPlayer()
    {
        Destroy(CarTransform.gameObject); //for now just destroy car game object   
    }

    public override void Initialize(int clientId)
    {
        _positionTracker = FindObjectOfType<PlayerPositionTracker>();
        _playerId = clientId;
        Spawn(_positionTracker.StartPosition, _positionTracker.StartPositionForward);
    }

    public void Spawn(Vector3 position, Vector3 forward)
    {
        var playerSelManager = PlayerSelectionManager.Instance;
        GameObject carPrefabToSpawn = playerSelManager.GetPlayerCarPrefab(_playerId);
        var playerColor = playerSelManager.GetPlayerColor(_playerId);
        int playerCarIndex = playerSelManager.GetPlayerCarIndex(_playerId);

        forward = forward.normalized;
        var right = Vector3.Cross(forward, Vector3.up);
        Vector3 spawnPos = position + right.normalized * (_playerId - 3) * spawnDistanceBetweenCars + spawnOffset;

        CarTransform = Instantiate(carPrefabToSpawn, position, Quaternion.LookRotation(forward, Vector3.up)).transform;

        CarTransform.GetComponent<CarCustomizationController>().SetMaterial(playerColor.CarMaterials[playerCarIndex],
            playerColor.IndexesToSet[playerCarIndex].Indexes);
        _carController = CarTransform.GetComponent<BodyController>();
    }

    public override void LeftCommand(ControlsState state)
    {
        _carController.SideInput = state == ControlsState.Pressed ? -1 : 0;
    }

    public override void RightCommand(ControlsState state)
    {
        _carController.SideInput = state == ControlsState.Pressed ? 1 : 0;
    }

    public override void UpCommand(ControlsState state)
    {
        _carController.ForwardInput = state == ControlsState.Pressed ? 1 : 0;
    }

    public override void DownCommand(ControlsState state)
    {
        _carController.ForwardInput = state == ControlsState.Pressed ? -1 : 0;
    }

    public override void ActionCommand(ControlsState state)
    {
    }
}