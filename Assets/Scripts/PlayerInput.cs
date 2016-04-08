using UnityEngine;
using System.Collections;
using UnityStandardAssets.CrossPlatformInput;

public class PlayerInput : MonoBehaviour {
    private PlayerController player;
	
    // Use this for initialization
	void Awake () {
        player = GetComponent<PlayerController>();
	}
	
    void Update()
    {

    }

	// Update is called once per frame
	void FixedUpdate () {
        //Get x-axis input
        float x = 0f;
        x = (Input.GetKey(KeyCode.A) ? x - 1 : x);
        x = (Input.GetKey(KeyCode.D) ? x + 1 : x);

        //Get y-axis input
        float y = 0f;
        y = (Input.GetKey(KeyCode.S) ? y - 1 : y);
        y = (Input.GetKey(KeyCode.W) ? y + 1 : y);

        bool jump = Input.GetKey(KeyCode.Space);

        player.Move(x, 0f, jump);
    }
}
