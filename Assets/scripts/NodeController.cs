using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using LibNoise;
using LibNoise.Models;
using LibNoise.Modifiers;

public class NodeController : MonoBehaviour {
    public static string NodeThreadName = "NodeThread";
    public static NodeController Instance;
    public int minNodesPerMeter = 1;
    public int maxNodesPerMeter = 2;
    public GameObject cameraObj;
    public GameObject nodePrefab;
    public GameObject lineRenderPrefab;
    public Dictionary<Vector3, NodeScript> nodes;
    public GameObject playerPrefab;
    public GameObject player;
    public PlayerScript playerInstance;
    public System.Random rand;
    public int seed;
    public Vector3 playerPosition;

    private System.Diagnostics.Stopwatch watch;

    void Awake()
    {
        Instance = this;
    }

	// Use this for initialization
	void Start () {
        Loom.AddAsyncThread(NodeThreadName);
        nodes = new Dictionary<Vector3, NodeScript>();
        watch = new System.Diagnostics.Stopwatch();
        seed = Random.Range(int.MinValue, int.MaxValue);
        rand = new System.Random(seed);
        PlaceNodes(100, 100);
	}
	
	// Update is called once per frame
	void Update ()
	{
        if (player != null)
	        playerPosition = player.transform.position;
	}

    public void PlaceNodes(int width, int height)
    {
        Loom.QueueAsyncTask(NodeThreadName, () =>
        {
            Perlin perlin = new Perlin();
            perlin.Seed = seed;
            perlin.Frequency = 20;
            Noise2D noiseMap = new Noise2D(width, height, perlin);
            noiseMap.GeneratePlanar(0, 1, 0, 1);
            Vector3[] points = GetNodePoints(noiseMap);
            Loom.QueueOnMainThread(() =>
            {
                Debug.LogFormat("Spawning {0} nodes.", points.Length);
                SpawnNodes(points);
                Loom.QueueAsyncTask(NodeThreadName, ConnectNodes);
            });
        });
    }

    public Vector3[] GetNodePoints(Noise2D noiseMap)
    {
        List<Vector3> points = new List<Vector3>();
        System.Random rand = new System.Random(seed);
        float[,] values = noiseMap.GetData();
        for (int x = 0; x < noiseMap.Width; x++)
        {
            for (int y = 0; y < noiseMap.Height; y++)
            {
                if (values[x, y] > 0.25f)
                {
                    for (int lx = 0; lx < rand.Next(minNodesPerMeter, maxNodesPerMeter); lx++)
                    {
                        float xPos = x + (((float)rand.NextDouble() / 1) - 0.5f);
                        float yPos = y + (((float)rand.NextDouble() / 1) - 0.5f);
                        points.Add(new Vector3(xPos, yPos, 0));
                    }
                }
            }
        }
        return points.ToArray();
    }

    public void SpawnNodes(Vector3[] points)
    {
        for (int i = 0; i < points.Length; i++)
        {
            SpawnNode(points[i]);
        }
    }

    public void SpawnNode(Vector3 position)
    {
        position = new Vector3(position.x, position.y, 0);
        GameObject nodeObj = (GameObject)Instantiate(nodePrefab, position, Quaternion.identity);
        NodeScript nodeInst = nodeObj.AddComponent<NodeScript>();
        nodeObj.transform.parent = transform;
        nodeInst.Init(position, lineRenderPrefab);
        nodes.Add(position, nodeInst);
    }

    public void ConnectNodes()
    {
        SafeDebug.Log("Connecting nodes.");
        int totalNodes = 0;
        watch.Start();
        NodeScript[] nodeInstances = nodes.Values.ToArray();
        for (int i = 0; i < nodeInstances.Length; i++)
        {
            totalNodes += nodeInstances[i].Connect();
            Loom.QueueOnMainThread(nodeInstances[i].CreateLines);
        }
        watch.Stop();
        SafeDebug.Log("Finished connecting nodes: " + watch.Elapsed);
        SafeDebug.Log("Total Connections proccesed: " + totalNodes + "/" + nodeInstances.Length * nodeInstances.Length);
        Loom.QueueOnMainThread(() =>
        {
            Vector3[] nodePositions = nodes.Keys.ToArray();
            Vector3 playerPos = nodePositions[Random.Range(0, nodePositions.Length)];
            player = (GameObject)Instantiate(playerPrefab, playerPos, Quaternion.identity);
            CameraScript camSc = cameraObj.AddComponent<CameraScript>();
            camSc.player = player;
            camSc.dampVelocity = 15;
            playerInstance = player.GetComponent<PlayerScript>();
            Loom.QueueOnMainThread(() => playerInstance.ConnectToNode(playerPos));
        });
    }
}
