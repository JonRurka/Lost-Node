using UnityEngine;
using System.Collections;

public class CameraScript : MonoBehaviour {
    public GameObject player;
    public float dampTime = 0.25f;
    public float dampVelocity = 5;

    public float distance;
    public Vector2 direction;
    public Vector2 force;

    private Rigidbody2D playerRigid;
    private Vector3 velocity = Vector3.zero;
    private Camera cam;

	// Use this for initialization
	void Start () {
        playerRigid = player.GetComponent<Rigidbody2D>();
        cam = GetComponent<Camera>();
	}
	
	// Update is called once per frame
	void Update () {
        Vector2 mousePos = Input.mousePosition;
        Vector2 center = cam.WorldToScreenPoint(player.transform.position);
        if (Input.GetMouseButton(0))
        {
            distance = Vector2.Distance(center, mousePos);
            direction = -(center - mousePos).normalized;

            /*Ray ray = cam.ScreenPointToRay(new Vector3(mousePos.x, mousePos.y, 0));
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 100))
            {
                Debug.DrawRay(hit.point, new Vector3(direction.x, direction.y, 0) * 0.5f, Color.red);
            }*/

            force = new Vector2(direction.x * distance / dampVelocity, direction.y * distance / dampVelocity);
            playerRigid.AddForce(force);
        }

        transform.position = new Vector3(player.transform.position.x, player.transform.position.y, transform.position.z);
        /*if (Vector2.Distance(new Vector2(transform.position.z, transform.position.y), new Vector2(player.transform.position.x, player.transform.position.y)) >= 0.01f)
        {
            Vector2 destination = player.transform.position;
            float x = Mathf.Lerp(transform.position.x, player.transform.position.x, 0);
            float y = Mathf.Lerp(transform.position.y, player.transform.position.y, 0);
            transform.position = new Vector3(x, y, transform.position.z);
            //transform.position = Mathf.SmoothDamp(transform.position, new Vector3(destination.x, destination.y, transform.position.z), ref velocity, dampTime);
        }*/
	}
}
