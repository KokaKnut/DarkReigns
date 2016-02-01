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
        bool jump = Input.GetKeyDown(KeyCode.Space);

        player.Move(0f, 0f, jump);
    }

	// Update is called once per frame
	void FixedUpdate () {
        float h = 0f;
        h = (Input.GetKey(KeyCode.A) ? h - 1 : h);
        h = (Input.GetKey(KeyCode.D) ? h + 1 : h);

        bool jump = Input.GetKeyDown(KeyCode.Space);

        player.Move(h, 0f, jump);
    }
}
