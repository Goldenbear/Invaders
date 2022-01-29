using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
public class Battlezone : MonoBehaviour {
	public class GameData : MonoBehaviour {
		public GameObject target;
	}
	static int score = 0, lives = 2, level = 1;
	GameObject[] uiObjects = new GameObject[10];
	List<GameObject> allObjects = new List<GameObject>(), playerLives = new List<GameObject>();
	float[,] playerGeom = new float[,] { {-0.5f, -0.5f, 0f}, { -0.3f, -0.3f, 0f}, { 0.5f, -0.3f, 0f}, { 0.5f, -0.2f, 0f}, { -0.3f, 0.2f, 0f}, { -0.4f, 0.5f, 0f}, { -0.5f, 0.5f, 0f}, { -0.5f, -0.5f, 0f} };
	float[,] bulletGeom = new float[,] { {-0.5f, 0f, 0f}, { 0.5f, 0f, 0f} };
	float[,] terrainGeom = new float[,] { { -20f, 0f, 0f }, { -18f, 2f, 0f }, { -15f, 0f, 0f }, { -12f, 0f, 0f }, { -9f, 2f, 0f }, { -6f, 0f, 0f }, { -3f, 0f, 0f }, { -2f, 1f, 0f }, { -1f, 0f, 0f }, { 2f, 0f, 0f }, { 3f, 1f, 0f }, { 4f, 0f, 0f }, { 5f, 0f, 0f }, { 8f, 2f, 0f }, { 11f, 0f, 0f }, { 15f, 0f, 0f }, { 16.5f, 0.5f, 0f }, { 17f, 0.3f, 0f }, { 17.5f, 0.8f, 0f }, { 18f, 0.6f, 0f }, { 18.5f, 1.1f, 0f }, { 19.5f, 0f, 0f }, { 20f, 0f, 0f }, { -20f, 0f, 0f } };
	float[,] cubeGeom = new float[,] { {-1f, -1f, -1f}, {-1f, -1f, 1f}, {1f, -1f, 1f}, {1f, -1f, -1f}, {-1f, -1f, -1f}, {-1f, 1f, -1f}, {-1f, 1f, 1f}, {1f, 1f, 1f}, {1f, 1f, -1f}, {-1f, 1f, -1f}, {-1f, -1f, -1f}, {-1f, -1f, 1f}, {-1f, 1f, 1f}, {1f, 1f, 1f}, {1f, -1f, 1f}, {1f, -1f, -1f}, {1f, 1f, -1f} };
	float camOffset = 0f, gameStateTimer = 0f;	// Both timer for losing life and auto-fire!
	int gameState = 0;
	Vector2 TouchJoy(int t) { return (Input.GetTouch(t).position - new Vector2(Screen.width-(Screen.height/4f), Screen.height/4f)) / (Screen.height/4f); }
	Vector2 KeyJoy { get { return (Input.GetKey(KeyCode.LeftArrow)?-Vector2.right:Vector2.zero)+(Input.GetKey(KeyCode.RightArrow)?Vector2.right:Vector2.zero)+(Input.GetKey(KeyCode.DownArrow)?-Vector2.up:Vector2.zero)+(Input.GetKey(KeyCode.UpArrow)?Vector2.up:Vector2.zero); } }
	Vector2 Joystick { get { for(int t=0; t<Input.touchCount; t++) {if(TouchJoy(t).magnitude<2f) return TouchJoy(t);} return KeyJoy; } }
	bool Fire { get { for(int t=0; t<Input.touchCount; t++) {if(TouchJoy(t).x<-2f) return true; } return Input.GetKey(KeyCode.LeftShift); } }
	void Start() {
		gameObject.GetComponent<Camera>().orthographic = false;				// 3D perspective camera!
		gameObject.GetComponent<Camera>().backgroundColor = Color.black;
 		CreateVectorObject("Terrain1", terrainGeom, -10f, 0f, 0f, 1f, 1f, 1f, 0, Color.green).transform.parent = transform;
		for (int i=0; i<20; i++) {
			playerLives.Add( CreateVectorObject("Life", playerGeom, 0.2f+i*0.5f, 5.2f, 0f, 0.3f, 0.1f, 1f, 0, Color.green, i<=lives, true, 0.1f) );
			playerLives[i].transform.parent = gameObject.transform;
		}
		for (int i=0; i<100; i++)
        	allObjects.Add( CreateVectorObject("Obstacle", cubeGeom, Random.Range(-40f, 40f), 0f, Random.Range(-40f, 40f), 0.5f, 0.5f, 0.5f, 0, Color.green) );
		uiObjects[0] = new GameObject("UICanvas");
		uiObjects[0].AddComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
		for(int i=0; i<4; i++) {
			uiObjects[1+i] = new GameObject("UIText");
			uiObjects[1+i].transform.parent = uiObjects[0].transform;
			uiObjects[1+i].AddComponent<Text>().font = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;
			uiObjects[1+i].GetComponent<Text>().fontSize = i == 1 ? Screen.width/50 : Screen.width/30;
			uiObjects[1+i].GetComponent<Text>().alignment = TextAnchor.MiddleLeft;		// Left align text within RectTransform
			uiObjects[1+i].GetComponent<RectTransform>().pivot = new Vector2(0f, 0.5f);	// Position left of RectTransform
			uiObjects[1+i].GetComponent<RectTransform>().localPosition = i == 0 ? new Vector3(0f, Screen.height/2.5f, 0f) : i == 1 ? new Vector3(0f, Screen.height/2.8f, 0f) : new Vector3(0f, 0f, 0f);
			uiObjects[1+i].GetComponent<RectTransform>().sizeDelta = new Vector2(800f, 800f);
			uiObjects[1+i].GetComponent<Text>().color = Color.green;
		}
    }
	GameObject CreateVectorObject(string label, float[,] geom, float pX, float pY, float pZ, float sX, float sY, float sZ, int layer, Color color, bool active = true, bool visible = true, float thickness=0.04f, Gradient gradient=null) {
		GameObject go = new GameObject(label);
		go.AddComponent<BoxCollider>().isTrigger = true;
		LineRenderer line = go.AddComponent<LineRenderer>();
		line.useWorldSpace = false;
		line.widthMultiplier = thickness;
		Vector3[] posVs = new Vector3[geom.GetLength(0)];
		for (int i = 0; i < posVs.Length; i++)
			posVs[i] = new Vector3(geom[i, 0], geom[i, 1], geom[i, 2]);
		line.positionCount = posVs.Length;
		line.SetPositions(posVs);
        line.colorGradient = gradient ?? line.colorGradient;
		go.SetActive(active);
		go.layer = layer;
		go.transform.position = new Vector3(pX, pY, pZ);
		go.transform.localScale = new Vector3(sX, sY, sZ);
		go.GetComponent<Renderer>().material = new Material(Shader.Find("Sprites/Default"));
		go.GetComponent<Renderer>().material.color = color;
		go.GetComponent<Renderer>().enabled = visible;
		go.AddComponent<GameData>();
		return go;
	}
	void KillPlayer() {
		playerLives[lives].SetActive(false);
		gameState = (--lives >= 0) ? 1 : 3;         // Player lost a life or player dead = game over?
		gameStateTimer = Time.unscaledTime + 3f;
	}
	void Score(int add) {
		lives = (score / 10000) < ((score + add) / 10000) ? ((lives < (playerLives.Count - 1)) ? lives + 1 : lives) : lives;
		playerLives[lives].SetActive(true);
		score += add;
		PlayerPrefs.SetInt("BattlezoneHighScore", score > PlayerPrefs.GetInt("BattlezoneHighScore") ? score : PlayerPrefs.GetInt("BattlezoneHighScore"));
	}
	void Update() {
		uiObjects[1].GetComponent<Text>().text = string.Format("SCORE     {0:00000}", score);
		uiObjects[2].GetComponent<Text>().text = string.Format("HIGH SCORE       {0:00000}", PlayerPrefs.GetInt("BattlezoneHighScore"));
		uiObjects[3].GetComponent<Text>().text = gameState == 2 ? "ATTACK WAVE " + level + "\n COMPLETED\n\nBONUS " + allObjects.Where(x => x.layer == 3).Count() + " X 100" : gameState == 3 ? "GAME OVER" : "";
		if (gameState > 0) {
			if ((gameState == 1) && (Time.unscaledTime > gameStateTimer)) {
				gameState = 0;
			}
			if ((gameState >= 2) && ((Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0)))) {
				level++;
				if (gameState == 3) { score = 0; lives = 2; level = 1; }                    // New game
				UnityEngine.SceneManagement.SceneManager.LoadScene("Battlezone");			// Reload scene for new level
			}
			return;
		}
		List<GameObject> destroyed = new List<GameObject>(), added = new List<GameObject>();
		if (Fire && (Time.unscaledTime > gameStateTimer)) {
			gameStateTimer = Time.unscaledTime + 0.2f;
		}
		transform.Rotate(new Vector3(0f, Joystick.x*10f*Time.deltaTime, 0f));
		transform.Translate(new Vector3(0f, 0f, Joystick.y*1f*Time.deltaTime));
		transform.GetChild(0).Translate(new Vector3(Joystick.x*-1f*Time.deltaTime, 0f, 0f));
    }
}