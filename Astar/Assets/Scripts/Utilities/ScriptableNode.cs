using UnityEngine;
using System.Collections.Generic;
using AIE;
public class ScriptableNode : ScriptableObject
{
    public int Id;

    public int U;
    public int V;

    public int G;
    public int H;    
    public int F;
 

    public ScriptableNode Parent;
    public List<ScriptableNode> Neighbors;
    public enum NodeState
    {
        None = 0,
        Open = 1,
        Closed = 2,        
    }

    public void ChangeState(NodeState state)
    {
        CurrentState = state;
    }
    public NodeState CurrentState;
    private bool walkable;
    public bool Walkable
    {
        get
        {
            return walkable;
        }
        set
        {
            walkable = value;
            ChangeState(NodeState.Closed);
        }
    }
    public void Create(AstarNode n)
    {
        Parent = null;
        Neighbors = new List<ScriptableNode>();
        CurrentState = NodeState.None;
        Walkable = true;
        G = n.G;
        H = n.H;
        U = n.U;
        V = n.V;
        Id = n.Id;
    }


}
