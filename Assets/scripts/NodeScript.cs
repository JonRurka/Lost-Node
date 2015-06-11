using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NodeScript : MonoBehaviour {
    public Dictionary<Vector3, NodeScript> connectedNodes = new Dictionary<Vector3,NodeScript>();
    public Vector3 position;
    public GameObject lineRenderPrefab;
    public bool connectionsShown;

    private SpriteRenderer _spriteRend;
    private List<LineRenderer> lines; 

	// Use this for initialization
	void Start () {
        _spriteRend = GetComponent<SpriteRenderer>();
        _spriteRend.color = new Color(Random.value, Random.value, Random.value);
        lines = new List<LineRenderer>();
    }
	
	// Update is called once per frame
	void Update () {

	}

    public void Init(Vector3 position, GameObject lineRenderPrefab)
    {
        this.position = position;
        this.lineRenderPrefab = lineRenderPrefab;
    }

    public int Connect()
    {
        int nodeCount = 0;
        List<Vector3> allPoints = new List<Vector3>(NodeController.Instance.nodes.Keys);
        System.Random rand = NodeController.Instance.rand;
        for (int i = 0; i < allPoints.Count; i++)
        {
            float distance = Vector3.Distance(position, allPoints[i]);
            if (distance < 3)
            {
                if (rand.NextDouble() < 1f)
                {
                    connectedNodes.Add(allPoints[i], NodeController.Instance.nodes[allPoints[i]]);
                    nodeCount++;
                }
            }
            else if (distance < 100)
            {
                if (rand.NextDouble() < 0.0001f)
                {
                    connectedNodes.Add(allPoints[i], NodeController.Instance.nodes[allPoints[i]]);
                    nodeCount++;
                }
            }
        }
        return nodeCount;
    }

    public void CreateLines()
    {
        foreach (Vector3 pos in connectedNodes.Keys)
        {
            LineRenderer line =
                ((GameObject)Instantiate(lineRenderPrefab, position, Quaternion.identity))
                    .GetComponent<LineRenderer>();
            line.transform.parent = transform;
            line.SetPosition(0, new Vector3(position.x, position.y, -1));
            line.SetPosition(1, new Vector3(pos.x, pos.y, -1));
            line.SetColors(Color.blue, Color.cyan);
            line.enabled = false;
            lines.Add(line);
        }
    }

    public void ShowConnections()
    {
        foreach (LineRenderer line in lines)
        {
            line.enabled = true;
        }
    }

    public void HideConnections()
    {
        foreach (LineRenderer line in lines)
        {
            line.enabled = false;
        }
    }
}
