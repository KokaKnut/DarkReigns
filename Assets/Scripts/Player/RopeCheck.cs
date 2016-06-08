using UnityEngine;
using System.Collections;

public class RopeCheck : MonoBehaviour {

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
            other.gameObject.GetComponent<PlayerController>().rope = gameObject;
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
            other.gameObject.GetComponent<PlayerController>().rope = null;
    }
}
