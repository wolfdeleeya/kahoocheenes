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
        Debug.Log("Player " + PlayerID + (state == ControlsState.Released ? " RELEASED " : " PRESSED ") + " LEFT!");
    }

    public override void RightCommand(ControlsState state)
    {
        Debug.Log("Player " + PlayerID + (state == ControlsState.Released ? " RELEASED " : " PRESSED ") + " RIGHT!");
    }

    public override void UpCommand(ControlsState state)
    {
        Debug.Log("Player " + PlayerID + (state == ControlsState.Released ? " RELEASED " : " PRESSED ") + " UP!");
    }

    public override void DownCommand(ControlsState state)
    {
        Debug.Log("Player " + PlayerID + (state == ControlsState.Released ? " RELEASED " : " PRESSED ") + " DOWN!");
    }

    public override void ActionCommand(ControlsState state)
    {
        Debug.Log("Player " + PlayerID + (state == ControlsState.Released ? " RELEASED " : " PRESSED ") + " ACTION!");
    }
}
