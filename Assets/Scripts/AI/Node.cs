using System.Collections;
using UnityEngine;

public abstract class Node
{
    public Animator animator;
    public NodeStatus currentStatus;
    public abstract IEnumerator Execute(MonoBehaviour mono);
}

public enum NodeStatus
{
    Running,
    Success,
    Failure
}