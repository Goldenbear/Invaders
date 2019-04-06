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
	public void LoadInvaders() {
		UnityEngine.SceneManagement.SceneManager.LoadScene("Invaders");
		gameObject.GetComponent<Canvas>().enabled = false;
	}
	public void LoadBreakout() {
		UnityEngine.SceneManagement.SceneManager.LoadScene("Breakout");
		gameObject.GetComponent<Canvas>().enabled = false;
	}
	public void LoadAsteroids() {
		UnityEngine.SceneManagement.SceneManager.LoadScene("Asteroids");
		gameObject.GetComponent<Canvas>().enabled = false;
	}
	public void LoadPacman() {
		UnityEngine.SceneManagement.SceneManager.LoadScene("Pacman");
		gameObject.GetComponent<Canvas>().enabled = false;
	}
	public void LoadDefender() {
		UnityEngine.SceneManagement.SceneManager.LoadScene("Defender");
		gameObject.GetComponent<Canvas>().enabled = false;
	}
}
