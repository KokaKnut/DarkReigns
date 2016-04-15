using UnityEngine;
using System.Collections;

public class CielingCheck : MonoBehaviour {

    private PlayerController player;

	void Start () {
        player = gameObject.GetComponentInParent<PlayerController>();
	}
	
	void OnTriggerEnter2D(Collider2D collider)
    {
        player.cielinged = true;
    }

    void OnTriggerStay2D(Collider2D collider)
    {
        player.cielinged = true;
    }

    void OnTriggerExit2D(Collider2D collider)
    {
        player.cielinged = false;
    }
}
