using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuControlsHandler : AbstractControlsHandler
{
    public int PlayerID { get; private set; }

    public override void Initialize(int clientId)
    {
        PlayerID = clientId;
        Debug.Log("Player " + PlayerID + " connected.");
    }

    public override void LeftCommand(ControlsState state)
    {
        if (state == ControlsState.Pressed)
            PlayerSelectionManager.Instance.SwitchColor(PlayerID, false);
    }

    public override void RightCommand(ControlsState state)
    {
        if (state == ControlsState.Pressed)
            PlayerSelectionManager.Instance.SwitchColor(PlayerID, true);
    }

    public override void UpCommand(ControlsState state)
    {
        if (state == ControlsState.Pressed)
            PlayerSelectionManager.Instance.SwitchCar(PlayerID, true);
    }

    public override void DownCommand(ControlsState state)
    {
        if (state == ControlsState.Pressed)
            PlayerSelectionManager.Instance.SwitchCar(PlayerID, false);
    }

    public override void ActionCommand(ControlsState state)
    {
        if (state == ControlsState.Pressed)
            PlayerSelectionManager.Instance.ActionPressed(PlayerID);
    }
}