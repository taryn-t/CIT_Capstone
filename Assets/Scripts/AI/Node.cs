using UnityEngine;

public abstract class Node
{
    public Animator animator;
    public abstract NodeStatus Execute();
}

public enum NodeStatus
{
    Running,
    Success,
    Failure
}