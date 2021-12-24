using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuControlsHandler : AbstractControlsHandler
{
    private SelectionScreenController _selectionScreenController;
    public int PlayerID { get; private set; }

    public override void Initialize(int clientId)
    {
        PlayerID = clientId;
        _selectionScreenController = FindObjectOfType<SelectionScreenController>();
        Debug.Log("Player " + PlayerID + " connected.");
    }

    public override void LeftCommand(ControlsState state)
    {
        if (state == ControlsState.Pressed)
            _selectionScreenController.SwitchColor(PlayerID, false);
        Debug.Log("Player " + PlayerID + (state == ControlsState.Released ? " RELEASED " : " PRESSED ") + " LEFT!");
    }

    public override void RightCommand(ControlsState state)
    {
        if (state == ControlsState.Pressed)
            _selectionScreenController.SwitchColor(PlayerID, true);
        Debug.Log("Player " + PlayerID + (state == ControlsState.Released ? " RELEASED " : " PRESSED ") + " RIGHT!");
    }

    public override void UpCommand(ControlsState state)
    {
        if (state == ControlsState.Pressed)
            _selectionScreenController.SwitchCar(PlayerID, true);
        Debug.Log("Player " + PlayerID + (state == ControlsState.Released ? " RELEASED " : " PRESSED ") + " UP!");
    }

    public override void DownCommand(ControlsState state)
    {
        if (state == ControlsState.Pressed)
            _selectionScreenController.SwitchCar(PlayerID, false);
        Debug.Log("Player " + PlayerID + (state == ControlsState.Released ? " RELEASED " : " PRESSED ") + " DOWN!");
    }

    public override void ActionCommand(ControlsState state)
    {
        if (state == ControlsState.Pressed)
            _selectionScreenController.ActionPressed(PlayerID);
        Debug.Log("Player " + PlayerID + (state == ControlsState.Released ? " RELEASED " : " PRESSED ") + " ACTION!");
    }
}