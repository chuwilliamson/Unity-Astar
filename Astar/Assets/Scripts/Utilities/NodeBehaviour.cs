using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class NodeBehaviour : MonoBehaviour, IPointerClickHandler , IPointerEnterHandler
{
    public List<GameObject> Neighbors;    
    public ScriptableNode Node;
    public GridBehaviour grid;
    public Vector3 scale;
 
    private void Start()
    {
 
        scale = transform.localScale;
        Neighbors = new List<GameObject>();
        Node.Neighbors.ForEach(n => Neighbors.Add(grid.GetChild(n)));
        if(Block)
            Node.Walkable = false;
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
            grid.SetStart(Node);
        }
        eventData.Use();
    }
    public bool Block;
    
    private void Update()
    {     
        if(Node.Parent)
            Debug.DrawLine(transform.position, grid.GetChild(Node.Parent).transform.position);
        f = Node.F;
        g = Node.G;
        h = Node.H;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if(Input.GetMouseButton(1))
        {
            Node.Walkable = !Node.Walkable;
            if(!Node.Walkable)
                grid.SetColor(Node, Color.red);
            else
                grid.SetColor(Node, Color.white);
          
            StartCoroutine("TweenScale");           
        }

        eventData.Use();
    }
    public void Tween()
    {
        StopCoroutine("TweenScale");
        StartCoroutine("TweenScale");
    }
    private IEnumerator TweenScale()
    {
        float timer = 0;
        Vector3 oldScale = scale;
        
        while(timer < .5f)
        {            
            transform.localScale = Vector3.Lerp(oldScale, oldScale * 1.25f, timer /1f);
            timer += Time.deltaTime;
            yield return null;
        }
        transform.localScale = oldScale;
        yield return null;
    }

    public int f, g, h;
}











