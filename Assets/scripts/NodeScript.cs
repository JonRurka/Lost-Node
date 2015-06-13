using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NodeScript : MonoBehaviour {
    public struct Line
    {
        public LineRenderer renderer;
        public List<Vector3> positions;

        public Line(LineRenderer renderer, List<Vector3> positions)
        {
            this.renderer = renderer;
            this.positions = new List<Vector3>(positions);
        }
    }

    public Dictionary<Vector3, NodeScript> connectedNodes = new Dictionary<Vector3,NodeScript>();
    public Vector3 position;
    public GameObject lineRenderPrefab;
    public bool connectionsShown;

    private SpriteRenderer _spriteRend;
    private List<Line> lines;
    private List<GameObject> colliderObjects; 

	// Use this for initialization
	void Start () {
        _spriteRend = GetComponent<SpriteRenderer>();
        _spriteRend.color = new Color(Random.value, Random.value, Random.value);
        lines = new List<Line>();
        colliderObjects = new List<GameObject>();
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
            if (distance < 1)
            {
                if (rand.NextDouble() < 1f)
                {
                    connectedNodes.Add(allPoints[i], NodeController.Instance.nodes[allPoints[i]]);
                    nodeCount++;
                }
            }
            else if (distance < 5)
            {
                if (rand.NextDouble() < 0.5f)
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
            List<Vector3> positions = new List<Vector3>
            {
                new Vector3(position.x, position.y, -1),
                new Vector3(pos.x, pos.y, -1)
            };
            LineRenderer line =
                ((GameObject)Instantiate(lineRenderPrefab, position, Quaternion.identity))
                    .GetComponent<LineRenderer>();
            line.transform.parent = transform;
            line.SetPosition(0, positions[0]);
            line.SetPosition(1, positions[1]);
            line.SetColors(Color.blue, Color.cyan);
            line.enabled = false;
            lines.Add(new Line(line, positions));
        }
    }

    public void ShowConnections()
    {
        foreach (Line line in lines)
        {
            line.renderer.enabled = true;
            GameObject lineCollider = new GameObject("lineCollider");
            lineCollider.transform.parent = line.renderer.transform;
            lineCollider.transform.rotation = Quaternion.LookRotation((line.positions[0] - line.positions[1]).normalized);
            lineCollider.transform.position = Vector3.Lerp(line.positions[0], line.positions[1], 0.5f);
            lineCollider.layer = LayerMask.NameToLayer("LineCollider");
            BoxCollider coll = lineCollider.gameObject.AddComponent<BoxCollider>();
            coll.size = new Vector3(0.01f, 0.01f, Vector3.Distance(line.positions[0], line.positions[1]));
            colliderObjects.Add(lineCollider);
        }
    }

    public void HideConnections()
    {
        foreach (Line line in lines)
        {
            line.renderer.enabled = false;
        }
        foreach (GameObject coll in colliderObjects)
        {
            Destroy(coll);
        }
        colliderObjects.Clear();
    }

    public Vector3 GetEndLocation(LineRenderer line)
    {
        Vector3 location = Vector3.zero;
        foreach (Line testLine in lines)
        {
            if (testLine.renderer == line)
                location = testLine.positions[1];
        }
        return location;
    }
}
