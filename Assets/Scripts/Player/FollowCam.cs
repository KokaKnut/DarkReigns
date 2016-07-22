using UnityEngine;
using System.Collections;

public class FollowCam : MonoBehaviour {

    public Transform target;
    public Vector2 lerpSpeed;
    public int zoom;
    public bool pixelFix;
	
	// Update is called once per frame
	void LateUpdate () {
        transform.position = new Vector3(Mathf.Lerp(transform.position.x, target.position.x, lerpSpeed.x), Mathf.Lerp(transform.position.y, target.position.y, lerpSpeed.y), transform.position.z);
        GetComponent<Camera>().orthographicSize = Screen.height / (2 * zoom);

        // Pixel fix
        if (pixelFix)
            transform.position = new Vector3(Mathf.Round(transform.position.x), Mathf.Round(transform.position.y), transform.position.z);
    }
}