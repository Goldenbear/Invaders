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
	float[,] bulletGeom = new float[,] { {-1f, -1f, 0f}, { 1f, -1f, 0f}, { 1f, 1f, 0f}, { -1f, 1f, 0f}, { -1f, -1f, 0f}, { 0f, 0f, 1f} };
	float[,] terrainGeom = new float[,] { { -20f, 0f, 0f }, { -18f, 2f, 0f }, { -15f, 0f, 0f }, { -12f, 0f, 0f }, { -9f, 2f, 0f }, { -6f, 0f, 0f }, { -3f, 0f, 0f }, { -2f, 1f, 0f }, { -1f, 0f, 0f }, { 2f, 0f, 0f }, { 3f, 1f, 0f }, { 4f, 0f, 0f }, { 5f, 0f, 0f }, { 8f, 2f, 0f }, { 11f, 0f, 0f }, { 15f, 0f, 0f }, { 16.5f, 0.5f, 0f }, { 17f, 0.3f, 0f }, { 17.5f, 0.8f, 0f }, { 18f, 0.6f, 0f }, { 18.5f, 1.1f, 0f }, { 19.5f, 0f, 0f }, { 20f, 0f, 0f }, { -20f, 0f, 0f } };
	float[,] cubeGeom = new float[,] { {-1f, -1f, -1f}, {-1f, -1f, 1f}, {1f, -1f, 1f}, {1f, -1f, -1f}, {-1f, -1f, -1f}, {-1f, 1f, -1f}, {-1f, 1f, 1f}, {1f, 1f, 1f}, {1f, 1f, -1f}, {-1f, 1f, -1f}, {-1f, -1f, -1f}, {-1f, -1f, 1f}, {-1f, 1f, 1f}, {1f, 1f, 1f}, {1f, -1f, 1f}, {1f, -1f, -1f}, {1f, 1f, -1f} };
	float[,] tankGeom = new float[,] { {-0.4f, -0.1f, -0.8f}, {-0.4f, -0.1f, 0.8f}, {0.4f, -0.1f, 0.8f}, {0.4f, -0.1f, -0.8f}, {-0.5f, 0.1f, -1f}, {-0.5f, 0.1f, 1f}, {0.5f, 0.1f, 1f}, {0.5f, 0.1f, -1f}, {-0.4f, 0.3f, -0.8f}, {-0.4f, 0.3f, 0.5f}, {0.4f, 0.3f, 0.5f}, {0.4f, 0.3f, -0.8f}, {-0.3f, 0.6f, -0.7f}, {0.3f, 0.6f, -0.7f}, {-0.1f, 0.5f, -0.35f}, {-0.1f, 0.5f, 1f}, {0.1f, 0.5f, 1f}, {0.1f, 0.5f, -0.35f}, {-0.1f, 0.55f, -0.55f}, {-0.1f, 0.55f, 1f}, {0.1f, 0.55f, 1f}, {0.1f, 0.55f, -0.55f} };
	int[] tankGInd = new int[] {0, 1, 2, 3, 0, 4, 5, 1, 5, 6, 2, 6, 7, 3, 7, 4, 8, 9, 5, 9, 10, 6, 10, 11, 7, 11, 8, 12, 9, 10, 13, 11, 13, 12, 14, 15, 16, 17, 14, 18, 19, 15, 19, 20, 16, 20, 21, 17, 21, 18};
	float camOffset = 0f, gameStateTimer = 0f;	// Both timer for losing life and auto-fire!
	int gameState = 0;
	Vector2 TouchJoy(int t) { return (Input.GetTouch(t).position - new Vector2(Screen.width-(Screen.height/4f), Screen.height/4f)) / (Screen.height/4f); }
	Vector2 KeyJoy { get { return (Input.GetKey(KeyCode.LeftArrow)?-Vector2.right:Vector2.zero)+(Input.GetKey(KeyCode.RightArrow)?Vector2.right:Vector2.zero)+(Input.GetKey(KeyCode.DownArrow)?-Vector2.up:Vector2.zero)+(Input.GetKey(KeyCode.UpArrow)?Vector2.up:Vector2.zero); } }
	Vector2 Joystick { get { for(int t=0; t<Input.touchCount; t++) {if(TouchJoy(t).magnitude<2f) return TouchJoy(t);} return KeyJoy; } }
	bool Fire { get { for(int t=0; t<Input.touchCount; t++) {if(TouchJoy(t).x<-2f) return true; } return Input.GetKey(KeyCode.LeftShift); } }
	void Start() {
		gameObject.GetComponent<Camera>().orthographic = false;				// 3D perspective camera!
		gameObject.GetComponent<Camera>().backgroundColor = Color.black;
		transform.Translate(new Vector3(0f, 0.3f, 0f));
 		CreateVectorObject("Terrain1", VertexArray(terrainGeom), -10f, 0f, 40f, 1f, 1f, 1f, 0f, 0f, 0f, 0, Color.green).transform.parent = transform;
		for (int i=0; i<20; i++) {
			playerLives.Add( CreateVectorObject("Life", VertexArray(playerGeom), 0.2f+i*0.5f, 5.2f, 0f, 0.3f, 0.1f, 1f, 0f, 0f, 0f, 0, Color.green, i<=lives, true, 0.1f) );
			playerLives[i].transform.parent = gameObject.transform;
		}
		for (int i=0; i<100; i++)
        	allObjects.Add( CreateVectorObject("Obstacle", VertexArray(cubeGeom), Random.Range(-40f, 40f), 0.25f, Random.Range(-40f, 40f), 0.5f, 0.25f, 0.5f, 0f, 0f, 0f, 0, Color.green) );
		for (int i=0; i<10; i++)
        	allObjects.Add( CreateVectorObject("Tank", VertexArray(tankGeom, tankGInd), Random.Range(-40f, 40f), 0.05f, Random.Range(-40f, 40f), 0.5f, 0.5f, 0.5f, 0f, 0f, 0f, 1, Color.green) );
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
	Vector3[] VertexArray(float[,] geom, int[] geomIndices=null) {
		Vector3[] geomVs = new Vector3[(geomIndices!=null) ? geomIndices.GetLength(0) : geom.GetLength(0)] ;
		for (int i = 0; i < geomVs.Length; i++)
			geomVs[i] = (geomIndices!=null) ? new Vector3(geom[geomIndices[i], 0], geom[geomIndices[i], 1], geom[geomIndices[i], 2]) : new Vector3(geom[i, 0], geom[i, 1], geom[i, 2]);
		return geomVs;
	}
	GameObject CreateVectorObject(string label, Vector3[] geomVs, float pX, float pY, float pZ, float sX, float sY, float sZ, float rX, float rY, float rZ, int layer, Color color, bool active = true, bool visible = true, float thickness=0.01f, Gradient gradient=null) {
		GameObject go = new GameObject(label);
		go.AddComponent<BoxCollider>().isTrigger = true;
		Bounds bounds = GeometryUtility.CalculateBounds(geomVs, Matrix4x4.identity);
		go.GetComponent<BoxCollider>().center = bounds.center;
		go.GetComponent<BoxCollider>().size = bounds.size;
		LineRenderer line = go.AddComponent<LineRenderer>();
		line.useWorldSpace = false;
		line.widthMultiplier = thickness;
		line.positionCount = geomVs.Length;
		line.SetPositions(geomVs);
        line.colorGradient = gradient ?? line.colorGradient;
		go.SetActive(active);
		go.layer = layer;
		go.transform.position = new Vector3(pX, pY, pZ);
		go.transform.localScale = new Vector3(sX, sY, sZ);
		go.transform.Rotate(new Vector3(rX, rY, rZ), Space.Self);
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
			Vector3 pos = transform.position + transform.forward*1f;
			allObjects.Add(CreateVectorObject("Bullet", VertexArray(bulletGeom), pos.x, pos.y, pos.z, 0.1f, 0.1f, 0.1f, 0f, transform.eulerAngles.y, 0f, 2, Color.green, true, true, 0.01f));
			gameStateTimer = Time.unscaledTime + 0.2f;
		}
		foreach(GameObject go in allObjects) {
			go.GetComponent<LineRenderer>().widthMultiplier = 0.005f + (Mathf.Clamp((go.transform.position-transform.position).magnitude, 1f, 10f)/10f) * 0.03f;	// Wider line width as get farther away
			switch(go.layer) {
/* Bullet */	case 2:	Collider[] hits = Physics.OverlapBox(go.GetComponent<Collider>().bounds.center, go.GetComponent<Collider>().bounds.extents, go.transform.rotation, 1<<1);
						for(int h=0; (hits != null) && (h < hits.Length); h++) {
							if (hits[h].gameObject.layer == 1) {
								Score(100);
								destroyed.Add(go);
								destroyed.Add(hits[h].gameObject);
							}
						}
						go.transform.Translate(go.transform.forward*10f*Time.deltaTime, Space.World);
						if((go.transform.position-transform.position).sqrMagnitude > (20f*20f))
							destroyed.Add(go);
				break;
			}
		}
		foreach(GameObject dead in destroyed) {
			allObjects.Remove(dead);
			Destroy(dead);
		}
		allObjects.AddRange(added);
		transform.Rotate(new Vector3(0f, Joystick.x*10f*Time.deltaTime, 0f));
		transform.Translate(new Vector3(0f, 0f, Joystick.y*1f*Time.deltaTime));
		transform.GetChild(0).Translate(new Vector3(Joystick.x*-1f*Time.deltaTime, 0f, 0f));
    }
}