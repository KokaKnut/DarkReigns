using UnityEngine;
using System.Collections;

public class CielingCheck : MonoBehaviour {

    private PlayerController1 player;

	void Start () {
        player = gameObject.GetComponentInParent<PlayerController1>();
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
