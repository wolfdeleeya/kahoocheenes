public enum ControlsType
{
    Down,
    Up,
    Left,
    Right,
    Action
}

public enum ControlsState
{
    Released,
    Pressed
}

public struct ControlsPair
{
    public ControlsType Type;
    public ControlsState State;
}