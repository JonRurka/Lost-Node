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
    public LayerMask linelayer;

    public float distance;
    public Vector2 direction;
    public Vector2 force;

    private PlayerScript playerInst;
    private Rigidbody2D playerRigid;
    private Vector3 velocity = Vector3.zero;
    private Camera cam;
    private LineRenderer lastLineRender;
    private Vector3 lastBoxPos = Vector3.zero;

    private bool MoveingPlayer;
    private Vector3 oldPlayerPosition;
    private Vector3 NewPlayerPosition;
    private float t;

    void Awake()
    {
        Instance = this;
        cam = GetComponent<Camera>();
    }

	// Use this for initialization
	void Start () {
        playerRigid = player.GetComponent<Rigidbody2D>();
	    playerInst = playerRigid.GetComponent<PlayerScript>();
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
	        if (Input.GetMouseButton(0))
	        {
	            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
	            RaycastHit hit;
	            if (Physics.Raycast(ray, out hit, 100, (1 << LayerMask.NameToLayer("LineCollider"))))
	            {
	                if (hit.transform.parent.GetComponent<LineRenderer>() != null)
	                {
	                    Vector3 colliderPosition = hit.transform.position;
	                    if (colliderPosition != lastBoxPos)
	                    {
	                        if (lastLineRender != null)
	                            lastLineRender.SetColors(Color.blue, Color.cyan);
	                        lastBoxPos = colliderPosition;
	                        lastLineRender = hit.transform.parent.GetComponent<LineRenderer>();
	                        lastLineRender.SetColors(Color.red, Color.yellow);
	                    }
	                }
	            }
	        }
	        if (MoveingPlayer)
	        {
	            playerRigid.position = Vector2.Lerp(oldPlayerPosition, NewPlayerPosition, t);
	            t += Time.deltaTime * 2;
	            if (t > 1)
	            {
	                MoveingPlayer = false;
                    Invoke("NewNode", 0.3f);
                    t = 0;
                }
	        }
	    }

        transform.position = new Vector3(player.transform.position.x, player.transform.position.y, transform.position.z);
    }

    void OnGUI()
    {
        if (!freeMode && lastLineRender != null)
        {
            if (GUI.Button(new Rect(Screen.width/2 - 50, Screen.height*9/10f, 100, 50), "Connect"))
            {
                MoveingPlayer = true;
                NodeScript curNode = lastLineRender.transform.parent.GetComponent<NodeScript>();
                oldPlayerPosition = playerRigid.position;
                NewPlayerPosition = curNode.GetEndLocation(lastLineRender);
            }
        }
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
            if (lastLineRender != null)
                lastLineRender.SetColors(Color.blue, Color.cyan);
            lastLineRender = null;
        }
    }

    void NewNode()
    {
        Vector3 closestNode = playerInst.GetClosest();
        NodeController.Instance.nodes[oldPlayerPosition].HideConnections();
        NodeController.Instance.nodes[closestNode].ShowConnections();
        playerInst.DisconnectFromNode();
        playerInst.ConnectToNode(closestNode);
        lastLineRender = null;
    }
}
