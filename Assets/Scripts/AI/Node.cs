public abstract class Node
{
    public abstract NodeStatus Execute();
}

public enum NodeStatus
{
    Running,
    Success,
    Failure
}