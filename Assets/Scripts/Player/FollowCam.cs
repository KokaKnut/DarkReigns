using UnityEngine;
using System.Collections;

public class FollowCam : MonoBehaviour {

    public Transform target;
    public float lerpSpeed;
    public int zoom;
	
	// Update is called once per frame
	void Update () {
        transform.position = new Vector3(Mathf.Lerp(transform.position.x, target.position.x, lerpSpeed), Mathf.Lerp(transform.position.y, target.position.y, lerpSpeed), transform.position.z);
        GetComponent<Camera>().orthographicSize = Screen.height / (2 * zoom);

        // Pixel fix
        transform.position = new Vector3(Mathf.Round(transform.position.x), Mathf.Round(transform.position.y), transform.position.z);
    }
}
