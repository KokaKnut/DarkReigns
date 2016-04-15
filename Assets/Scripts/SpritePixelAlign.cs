using UnityEngine;
using System.Collections;

public class SpritePixelAlign : MonoBehaviour {

    private SpriteRenderer sprite;

    void Start () {
        sprite = GetComponent<SpriteRenderer>();
    }
	
	void LateUpdate () {
        sprite.transform.position = new Vector3(Mathf.Round(transform.parent.position.x), Mathf.Round(transform.parent.position.y), transform.position.z);
    }
}
