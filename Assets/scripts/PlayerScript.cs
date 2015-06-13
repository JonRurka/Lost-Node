using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class PlayerScript : MonoBehaviour
{
    private NodeScript CurrentNode;
    public List<Vector3> nearbyNodes;
    public bool freeMode = true;

	// Use this for initialization
	void Start () {
        nearbyNodes = new List<Vector3>();
    }
	
	// Update is called once per frame
	void Update () {
	    if (Input.GetKeyDown(KeyCode.Space))
	    {
	        if (freeMode)
	            ConnectToNode(GetClosest());
	        else
	            DisconnectFromNode();
	    }
	}

    public Vector3 GetClosest()
    {
        return GetClosest(transform.position);
    }

    public Vector3 GetClosest(Vector3 currentLocations)
    {
        float closestDistance = float.MaxValue;
        Vector3[] nodeLocations = nearbyNodes.ToArray();
        Vector3 closestNode = Vector3.zero;
        for (int i = 0; i < nodeLocations.Length; i++)
        {
            float distance = Vector3.Distance(currentLocations, nodeLocations[i]);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestNode = nodeLocations[i];
            }
        }
        return closestNode;
    }

    public void ConnectToNode(Vector3 node)
    {
        if (NodeController.Instance.nodes.ContainsKey(node))
        {
            freeMode = false;
            CameraScript.Instance.SetFreeMode(freeMode);
            if (CurrentNode != null)
                CurrentNode.HideConnections();
            transform.position = node;
            CurrentNode = NodeController.Instance.nodes[node];
            CurrentNode.ShowConnections();
        }
        else
            Debug.Log("Could not find node " + node);
    }

    public void DisconnectFromNode()
    {
        freeMode = true;
        CameraScript.Instance.SetFreeMode(freeMode);
        if (CurrentNode != null)
            CurrentNode.HideConnections();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Found " + other.name);
        //if (other.gameObject.layer == LayerMask.NameToLayer("Node"))
        //{
            NodeScript nodeInst = other.GetComponent<NodeScript>();
            if (nodeInst != null && !nearbyNodes.Contains(nodeInst.position))
            {
                nearbyNodes.Add(nodeInst.position);
            }
        //}
    }

    void OnTriggerExit2D(Collider2D other)
    {
        //if (other.gameObject.layer == LayerMask.NameToLayer("Node"))
        //{
            NodeScript nodeInst = other.GetComponent<NodeScript>();
            if (nodeInst != null && nearbyNodes.Contains(nodeInst.position))
            {
                nearbyNodes.Remove(nodeInst.position);
            }
        //}
    }
}
