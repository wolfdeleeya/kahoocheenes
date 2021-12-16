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
    Up,
    Down
}

public struct ControlsPair
{
    public ControlsType Type;
    public ControlsState State;
}