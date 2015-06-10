using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using LibNoise;
using LibNoise.Models;
using LibNoise.Modifiers;

public class NodeController : MonoBehaviour {
    public static string NodeThreadName = "NodeThread";
    public int minNodesPerMeter = 1;
    public int maxNodesPerMeter = 2;
    public GameObject nodePrefab;
    public List<NodeScript> nodes;
    public GameObject player;
    
	// Use this for initialization
	void Start () {
        Loom.AddAsyncThread(NodeThreadName);
        nodes = new List<NodeScript>();
        PlaceNodes(100, 100);
        player.transform.position = new Vector3(100 / 2, 100 / 2, 0);
	}
	
	// Update is called once per frame
	void Update () {
	    
	}

    public void PlaceNodes(int width, int height)
    {
        int seed = Random.Range(int.MinValue, int.MaxValue);
        Loom.QueueAsyncTask(NodeThreadName, () =>
        {
            Perlin perlin = new Perlin();
            perlin.Seed = seed;
            perlin.Frequency = 20;
            Noise2D noiseMap = new Noise2D(width, height, perlin);
            noiseMap.GeneratePlanar(0, 1, 0, 1);
            Vector3[] points = GetNodePoints(noiseMap, seed);
            Loom.QueueOnMainThread(() =>
            {
                Debug.LogFormat("Spawning {0} nodes.", points.Length);
                SpawnNodes(points);
            });
        });
    }

    public Vector3[] GetNodePoints(Noise2D noiseMap, int seed)
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
        GameObject nodeObj = (GameObject)Instantiate(nodePrefab, position, Quaternion.identity);
        NodeScript nodeInst = nodeObj.AddComponent<NodeScript>();
        nodeObj.transform.parent = transform;
        nodes.Add(nodeInst);
    }
}
