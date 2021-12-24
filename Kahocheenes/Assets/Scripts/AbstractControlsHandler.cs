using UnityEngine;
using UnityEngine.Events;

public abstract class AbstractControlsHandler : MonoBehaviour
{
    public void HandleControls(ControlsPair pair)
    {
        switch (pair.Type)
        {
            case ControlsType.Up:
                UpCommand(pair.State);
                break;

            case ControlsType.Down:
                DownCommand(pair.State);
                break;

            case ControlsType.Right:
                RightCommand(pair.State);
                break;

            case ControlsType.Left:
                LeftCommand(pair.State);
                break;

            case ControlsType.Action:
                ActionCommand(pair.State);
                break;
        }
    }

    public abstract void Initialize(int clientId);
    public abstract void LeftCommand(ControlsState state);
    public abstract void RightCommand(ControlsState state);
    public abstract void UpCommand(ControlsState state);
    public abstract void DownCommand(ControlsState state);
    public abstract void ActionCommand(ControlsState state);
}