using UnityEngine;
using System.Collections;

public class PlayerInput : MonoBehaviour {
    private PlayerController player;
	
    // Use this for initialization
	void Awake () {
        player = GetComponent<PlayerController>();
	}
	
    void Update()
    {

        //Get x-axis input
        float x = 0f;
        x = Mathf.Round(Mathf.Clamp(Input.GetAxis("Horizontal") * 3, -1, 1));

        //Get y-axis input
        float y = 0f;
        y = Mathf.Round(Mathf.Clamp(Input.GetAxis("Vertical") * 3, -1, 1));

        bool jump = (Input.GetAxis("Jump") != 0);

        player.Move(x, y, jump);
    }
}
