using UnityEngine;
using System.Collections;

public class CameraScript : MonoBehaviour
{
    public static CameraScript Instance;
    public GameObject player;
    public float dampTime = 0.25f;
    public float dampVelocity = 5;
    public bool freeMode = true;
    public float zoomIn = 2;
    public float zoomOut = 5;

    public float distance;
    public Vector2 direction;
    public Vector2 force;

    private Rigidbody2D playerRigid;
    private Vector3 velocity = Vector3.zero;
    private Camera cam;

    void Awake()
    {
        Instance = this;
        cam = GetComponent<Camera>();
    }

	// Use this for initialization
	void Start () {
        playerRigid = player.GetComponent<Rigidbody2D>();
	}
	
	// Update is called once per frame
	void Update () {
	    if (freeMode)
	    {
	        Vector2 mousePos = Input.mousePosition;
	        Vector2 center = cam.WorldToScreenPoint(player.transform.position);
	        if (Input.GetMouseButton(0))
	        {
	            distance = Vector2.Distance(center, mousePos);
	            direction = -(center - mousePos).normalized;
	            force = new Vector2(direction.x*distance/dampVelocity, direction.y*distance/dampVelocity);
	            playerRigid.AddForce(force);
	        }
	        /*if (Vector2.Distance(new Vector2(transform.position.z, transform.position.y), new Vector2(player.transform.position.x, player.transform.position.y)) >= 0.01f)
            {
                Vector2 destination = player.transform.position;
                float x = Mathf.Lerp(transform.position.x, player.transform.position.x, 0);
                float y = Mathf.Lerp(transform.position.y, player.transform.position.y, 0);
                transform.position = new Vector3(x, y, transform.position.z);
                //transform.position = Mathf.SmoothDamp(transform.position, new Vector3(destination.x, destination.y, transform.position.z), ref velocity, dampTime);
            }*/
	    }
	    else
	    {
	        
	    }

        transform.position = new Vector3(player.transform.position.x, player.transform.position.y, transform.position.z);
    }

    public void SetFreeMode(bool freeMode)
    {
        this.freeMode = freeMode;
        if (freeMode)
            cam.orthographicSize = 5;
        else
        {
            cam.orthographicSize = 2;
            playerRigid.velocity = Vector2.zero;
        }
    }
}
