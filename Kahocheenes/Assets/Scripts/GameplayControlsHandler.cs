using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplayControlsHandler : AbstractControlsHandler
{
    [HideInInspector] public UnityEventInteger OnPlayerDestroyed;

    [SerializeField] private float spawnDistanceBetweenCars;
    [SerializeField] private Vector3 spawnOffset;
    
    private BodyController _carController;

    public int PlayerID { get; private set; }

    public bool IsAlive { get; private set; }

    public bool CanControl { get; set; }
    
    public Transform CarTransform { get; private set; }

    private void Awake()
    {
        OnPlayerDestroyed = new UnityEventInteger();
    }

    public void DestroyPlayer()
    {
        Destroy(CarTransform.gameObject);
        IsAlive = false;
        OnPlayerDestroyed.Invoke(PlayerID);
    }

    public override void Initialize(int clientId)
    {
        PlayerID = clientId;
        GameplayManager.Instance.AddPlayer(this);
    }

    public void Spawn(Vector3 position, Vector3 forward)
    {
        IsAlive = true;
        var playerSelManager = PlayerSelectionManager.Instance;
        GameObject carPrefabToSpawn = playerSelManager.GetPlayerCarPrefab(PlayerID).GameplayPrefab;
        var playerColor = playerSelManager.GetPlayerColor(PlayerID);
        int playerCarIndex = playerSelManager.GetPlayerCarIndex(PlayerID);

        forward = forward.normalized;
        var right = Vector3.Cross(forward, Vector3.up);
        Vector3 spawnPos = position + right.normalized * (PlayerID - 3) * spawnDistanceBetweenCars + spawnOffset;

        CarTransform = Instantiate(carPrefabToSpawn, spawnPos, Quaternion.LookRotation(forward, Vector3.up)).transform;

        CarTransform.GetComponent<CarCustomizationController>().SetMaterial(playerColor.CarMaterials[playerCarIndex],
            playerColor.IndexesToSet[playerCarIndex].Indexes);
        _carController = CarTransform.GetComponent<BodyController>();
    }

    public override void LeftCommand(ControlsState state)
    {
        if (CanControl)
            _carController.SideInput = state == ControlsState.Pressed ? -1 : 0;
    }

    public override void RightCommand(ControlsState state)
    {
        if (CanControl)
            _carController.SideInput = state == ControlsState.Pressed ? 1 : 0;
    }

    public override void UpCommand(ControlsState state)
    {
        if (CanControl)
            _carController.ForwardInput = state == ControlsState.Pressed ? 1 : 0;
    }

    public override void DownCommand(ControlsState state)
    {
        if (CanControl)
            _carController.ForwardInput = state == ControlsState.Pressed ? -1 : 0;
    }

    public override void ActionCommand(ControlsState state)
    {
        if (CanControl)
            return;
    }
}