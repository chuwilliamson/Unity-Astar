using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class NodeBehaviour : MonoBehaviour, IPointerClickHandler
{
    public List<GameObject> Neighbors;    
    public ScriptableNode Node;
    public GridBehaviour grid;
    private void Start()
    {
        Neighbors = new List<GameObject>();
        Node.Neighbors.ForEach(n => Neighbors.Add(grid.GetChild(n)));
        if(!Node.Walkable)
            grid.SetColor(Node, Color.red);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if(eventData.button == PointerEventData.InputButton.Left)
        {
            eventData.selectedObject = gameObject;
            grid.SetGoal(Node);
        }
        if(eventData.button == PointerEventData.InputButton.Right)
        {
            Node.Walkable = false;
            grid.SetColor(Node, Color.red);
        }
        
    }

    private void Update()
    {     
        if(Node.Parent)
            Debug.DrawLine(transform.position, grid.GetChild(Node.Parent).transform.position);
    }


}










