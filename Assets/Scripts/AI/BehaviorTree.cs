using UnityEngine;

public class BehaviorTree
{
    private Node root;

    public virtual void SetRoot(Node node)
    {
        root = node;
    }

    public virtual void Tick(MonoBehaviour mono)
    {
         mono.StartCoroutine(root?.Execute(mono));
                   
    }
    
}