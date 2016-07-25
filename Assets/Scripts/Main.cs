using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class Main : MonoBehaviour {

    public void Exit()
    {
        Application.Quit();
    }

	public void Reload()
    {
        SceneManager.LoadScene("workspace");
    }
}
