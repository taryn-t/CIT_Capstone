using System.Collections;
using UnityEngine;

public abstract class Node
{
    public Animator animator;
    public NodeStatus currentStatus;
    public abstract NodeStatus Execute( );
}

public enum NodeStatus
{
    Running,
    Success,
    Failure
}