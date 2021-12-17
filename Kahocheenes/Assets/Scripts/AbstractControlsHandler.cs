using UnityEngine;
using UnityEngine.Events;

public abstract class AbstractControlsHandler : MonoBehaviour
{
    [HideInInspector] public UnityEvent OnActionPressed = new UnityEvent();
    [HideInInspector] public UnityEvent OnActionReleased = new UnityEvent();
    
    [HideInInspector] public UnityEvent OnLeftPressed = new UnityEvent();
    [HideInInspector] public UnityEvent OnLeftReleased = new UnityEvent();

    [HideInInspector] public UnityEvent OnRightPressed = new UnityEvent();
    [HideInInspector] public UnityEvent OnRightReleased = new UnityEvent();
    
    [HideInInspector] public UnityEvent OnUpPressed = new UnityEvent();
    [HideInInspector] public UnityEvent OnUpReleased = new UnityEvent();
    
    [HideInInspector] public UnityEvent OnDownPressed = new UnityEvent();
    [HideInInspector] public UnityEvent OnDownReleased = new UnityEvent();

    public void HandleControls(ControlsPair pair)
    {
        switch (pair.Type)
        {
            case ControlsType.Up:
                UpCommand(pair.State);
                if (pair.State == ControlsState.Pressed)
                    OnUpPressed.Invoke();
                else
                    OnUpReleased.Invoke();
                break;

            case ControlsType.Down:
                DownCommand(pair.State);
                if (pair.State == ControlsState.Pressed)
                    OnDownPressed.Invoke();
                else
                    OnDownReleased.Invoke();
                break;

            case ControlsType.Right:
                RightCommand(pair.State);
                if (pair.State == ControlsState.Pressed)
                    OnRightPressed.Invoke();
                else
                    OnRightReleased.Invoke();
                break;

            case ControlsType.Left:
                LeftCommand(pair.State);
                if (pair.State == ControlsState.Pressed)
                    OnLeftPressed.Invoke();
                else
                    OnLeftReleased.Invoke();
                break;

            case ControlsType.Action:
                ActionCommand(pair.State);
                if (pair.State == ControlsState.Pressed)
                    OnActionPressed.Invoke();
                else
                    OnActionReleased.Invoke();
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