using UnityEngine;
using System.Collections;

public class PlayerInput : MonoBehaviour {
    private PlayerController1 player;
	
    // Use this for initialization
	void Awake () {
        player = GetComponent<PlayerController1>();
	}
	
    void FixedUpdate()
    {

        //Get x-axis input
        float x = 0f;
        x = (Mathf.Clamp(Input.GetAxis("Horizontal") * 3, -1, 1));

        //Get y-axis input
        float y = 0f;
        y = (Mathf.Clamp(Input.GetAxis("Vertical") * 3, -1, 1));

        bool jump = (Input.GetAxis("Jump") != 0);

        player.Move(x, y, jump);
    }
}
