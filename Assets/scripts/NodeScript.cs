using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NodeScript : MonoBehaviour {
    public Dictionary<Vector3, NodeScript> ConnectedNodes = new Dictionary<Vector3,NodeScript>();

    private SpriteRenderer spriteRend;

	// Use this for initialization
	void Start () {
        spriteRend = GetComponent<SpriteRenderer>();
        spriteRend.color = new Color(Random.value, Random.value, Random.value);
	}
	
	// Update is called once per frame
	void Update () {
	    
	}
}
