using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AIE;
public class GridBehaviour : MonoBehaviour
{
    /// <summary>
    /// the GameObject Container
    /// </summary>
    public List<GameObject> Children;

    /// <summary>
    /// The nodes
    /// </summary>
    public List<ScriptableNode> Nodes;
    public int Rows = 5;
    public int Cols = 5;
    public void Awake()
    {
        Nodes.ForEach(n => n.Walkable = true);
        Current = Nodes[0];
        Goal = Nodes[99];
    }
    public GameObject GetChild(ScriptableNode s)
    {
        if(Children[s.Id].GetComponent<NodeBehaviour>().Node != s)
        {
            Debug.LogError("node behaviour node mismatch");
            return null;
        }
        return Children[s.Id];
    }

    /*
        1. Find node closest to your position and declare it start node and put it on 
            the open list. 
        2. While there are nodes in the open list:
        3. Pick the node from the open list having the smallest F score. Put it on 
            the closed list (you don't want to consider it again).
        4. For each neighbor (adjacent cell) which isn't in the closed list:
        5. Set its parent to current node.
        6. Calculate G score (distance from starting node to this neighbor) and 
            add it to the open list
        7. Calculate F score by adding heuristics to the G value. 
     */
    public void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            StopCoroutine("Astar");
        }
        if(Input.GetKeyDown(KeyCode.F))
        {
            Clear();
            StartCoroutine(Astar(Current, Goal));
        }
    }

    public ScriptableNode Goal;
    public ScriptableNode Current;

    public List<ScriptableNode> Open = new List<ScriptableNode>();
    public List<ScriptableNode> Closed = new List<ScriptableNode>();

    public IEnumerator Astar(ScriptableNode start, ScriptableNode goal)
    {       
        Current = start;
        AddToOpen(Current);
        while(Open.Count > 0)
        {
            Open.Sort((a, b) => a.F.CompareTo(b.F));
            Current = Open[0];
            AddToClosed(Current);
            yield return new WaitForSeconds(.1f);
            foreach(var n in Current.Neighbors)
            {
                if(!Open.Contains(n) && n.Walkable)
                {
                    AddToOpen(n);
                    n.G = (int)Vector3.Distance(GetChild(n).transform.position, GetChild(Current).transform.position);
                    n.H = Utilities.ManhattanDistance(new Point(n.U, n.V), new Point(goal.U, goal.V));
                    n.Parent = Current;
                }
                else if(Open.Contains(n))
                {                    
                    AddToClosed(n);
                    if(n.G < Current.G)
                    {
                        n.Parent = Current;
                        n.G = (int)Vector3.Distance(GetChild(n).transform.position, GetChild(Current).transform.position);
                        Open.Sort((a, b) => a.F.CompareTo(b.F));
                    }
                }
            }
            if(Closed.Contains(goal))
                break;
            yield return null;
        }
    }
        
    public void AddToOpen(ScriptableNode s)
    {
        Open.Add(s);
        GetChild(s).GetComponent<MeshRenderer>().material.color = Color.cyan;
    }

    public void AddToClosed(ScriptableNode s)
    {
        Open.Remove(s);
        Closed.Add(s);
        GetChild(s).GetComponent<MeshRenderer>().material.color = Color.black;

    }

    public void SetGoal(ScriptableNode s)
    {
        Clear();
        Open.Clear();
        Closed.Clear();
        Goal = s;
        SetColor(GetChild(Goal), Color.green);
        StartCoroutine(Astar(Current, Goal));
    }

    public void SetStart(ScriptableNode s)
    {
        Current = s;
    }
    [ContextMenu("Create Nodes")]
    public void CreateNodes()
    {
        ClearNodes();
        Children = new List<GameObject>();
        Nodes = new List<ScriptableNode>();
        var positions = Utilities.CreateGrid(Rows, Cols);

        foreach(var p in positions)
        {
            var node = ScriptableObject.CreateInstance<ScriptableNode>();
            node.Create(new AstarNode(p.U, p.V, Nodes.Count));
            node.name = string.Format("Node {0}", Nodes.Count.ToString());
            Nodes.Add(node);
        }

        Nodes.ForEach(n => n.Neighbors = Neighbors(n));
        CreateGameObjects();

    }
    public void Clear()
    {
        Children.ForEach(child => SetColor(child, Color.white));
        foreach(var n in Nodes)
        {
            if(!n.Walkable)
                SetColor(GetChild(n), Color.red);
        }
        
            
        Nodes.ForEach(node => node.Parent = null);
    }

    public void SetColor(GameObject go, Color c)
    {
        go.GetComponent<MeshRenderer>().material.color = c;
    }

    public void SetColor(ScriptableNode s, Color c)
    {
        GameObject go = GetChild(s);
        go.GetComponent<MeshRenderer>().material.color = c;
    }

    [ContextMenu("Clear")]
    public void ClearNodes()
    {
        Children.ForEach(child => DestroyImmediate(child));
        Children.Clear();
        Nodes.ForEach(node => DestroyImmediate(node));
        Nodes.Clear();
    }

    public void CreateGameObjects()
    {
        foreach(var n in Nodes)
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            go.transform.SetParent(transform);
            go.transform.localPosition = new Vector3(n.U * 10, n.V * 10);
            go.transform.localScale = new Vector3(5, 5, 5);
            go.name = string.Format("Node {0}", n.Id);
            var nb = go.AddComponent<NodeBehaviour>();
            nb.Node = n;
            nb.grid = this;
            Children.Add(go);
        }
    }

    [ContextMenu("Set Camera")]
    public void SetCamera()
    {

    }

    public List<ScriptableNode> Neighbors(ScriptableNode node)
    {
        var neighbors = new List<ScriptableNode>();
        var dirs = new List<Point>()
            {
                new Point(1, 0),
                new Point(1, 1),
                new Point(0, 1),
                new Point(-1, 1),
                new Point(-1, 0),
                new Point(-1, -1),
                new Point(0, -1),
                new Point(1, -1),
            };

        foreach(var dir in dirs)
        {
            var nay = Nodes.Find(n => n.U == node.U + dir.U && n.V == node.V + dir.V);
            if(nay != null)
                neighbors.Add(nay);
        }

        return neighbors;
    }
}
