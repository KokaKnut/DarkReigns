using UnityEngine;
using System.Collections;

public class RopeCheck : MonoBehaviour {
    private PlayerController player;

    void Start()
    {
        player = gameObject.GetComponentInParent<PlayerController>();
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.CompareTag("player"))
            player.rope = gameObject;
    }

    void OnTriggerStay2D(Collider2D collider)
    {
        if (collider.gameObject.CompareTag("player"))
            player.rope = collider.gameObject;
    }

    void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.gameObject.CompareTag("player"))
            player.rope = null;
    }
}
