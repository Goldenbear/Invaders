using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Menu : MonoBehaviour
{
	private void Awake() {
		DontDestroyOnLoad(gameObject);
	}
	private void Update() {
		if( Input.GetKeyDown(KeyCode.Escape) ||
			(Input.GetMouseButtonDown(0) && (Input.mousePosition.x < 64f) && (Input.mousePosition.y > (Screen.height-64f))) )
			LoadMenu();
	}
	public void LoadMenu() {
		UnityEngine.SceneManagement.SceneManager.LoadScene("Menu");
		Destroy(this.gameObject);
	}
	public void LoadGame(string sceneName) {
		UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
		gameObject.GetComponent<Canvas>().enabled = false;
	}
}
