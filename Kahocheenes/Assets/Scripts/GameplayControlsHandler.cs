using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplayControlsHandler : AbstractControlsHandler
{
    [HideInInspector] public GameObject CarPrefabToSpawn; //SET BY PLAYER CONTROLLER
    [HideInInspector] public PlayerColorSO CurrentColor; //          -||-

    public Transform CarTransform { get; private set; }
    private BodyController _carController;

    public void DestroyPlayer()
    {
        Destroy(CarTransform.gameObject); //for now just destroy car game object   
    }

    public override void Initialize(int clientId)
    {
        throw new System.NotImplementedException();
    }

    public override void LeftCommand(ControlsState state)
    {
        _carController.SideInput = -1;
    }

    public override void RightCommand(ControlsState state)
    {
        _carController.SideInput = 1;
    }

    public override void UpCommand(ControlsState state)
    {
        _carController.ForwardInput = 1;
    }

    public override void DownCommand(ControlsState state)
    {
        _carController.ForwardInput = -1;
    }

    public override void ActionCommand(ControlsState state)
    {
        throw new System.NotImplementedException();
    }
}