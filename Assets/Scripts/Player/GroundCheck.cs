using UnityEngine;
using System.Collections;

public class GroundCheck : MonoBehaviour {

    private PlayerController1 player;

	void Start () {
        player = gameObject.GetComponentInParent<PlayerController1>();
	}
	
	void OnTriggerEnter2D(Collider2D collider)
    {
        player.grounded = true;
    }

    void OnTriggerStay2D(Collider2D collider)
    {
        player.grounded = true;
    }

    void OnTriggerExit2D(Collider2D collider)
    {
        player.grounded = false;
    }
}
