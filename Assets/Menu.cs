using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Menu : MonoBehaviour
{
	public void LoadMenu() {
		UnityEngine.SceneManagement.SceneManager.LoadScene("Menu");
	}
	public void LoadInvaders() {
		UnityEngine.SceneManagement.SceneManager.LoadScene("Invaders");
	}
	public void LoadBreakout() {
		UnityEngine.SceneManagement.SceneManager.LoadScene("Breakout");
	}
	public void LoadPacman() {
		UnityEngine.SceneManagement.SceneManager.LoadScene("Pacman");
	}
	public void LoadDefender() {
		UnityEngine.SceneManagement.SceneManager.LoadScene("Defender");
	}
}
