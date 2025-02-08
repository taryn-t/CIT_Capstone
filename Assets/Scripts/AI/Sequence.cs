
using System.Collections;
using UnityEngine;
public class Sequence : Node
{
    private Node[] children;

    public Sequence(Node[] nodes)
    {
        children = nodes;
    }

    public override NodeStatus Execute()
    {
        foreach (Node child in children)
        {
            try{
               NodeStatus status = child.Execute( );  
            
                if (status == NodeStatus.Failure)
                {
                    return status; // Return failure if any child fails
                } 
            }
            catch{
                return NodeStatus.Failure;
            }
            
           
           
        }
        return NodeStatus.Success; // All children succeeded
    }
}