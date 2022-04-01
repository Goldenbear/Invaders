using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
public class Battlezone : MonoBehaviour {
	public class GameData : MonoBehaviour {
		public int state = 0;
		public float stateTimeout = 0f;
		public Vector3 targetPos;
	}
	static int score = 0, lives = 2, level = 0;
	List<GameObject> allObjects = new List<GameObject>(), uiObjects = new List<GameObject>();
	float[,] radarHudGeom = new float[,] { {-1f,0f,0f}, {-0.8f,0f,0f}, {0.8f,0f,0f}, {1f,0f,0f}, {0f,-1f,0f}, {0f,-0.8f,0f}, {0f,0.8f,0f}, {0f,1f,0f}, {0f,0f,0f}, {-0.7f,0.7f,0f}, {0.7f,0.7f,0f}, {0f,1f,0f}, {-0.05f,-0.05f,0f}, {0.05f,-0.05f,0f}, {0.05f,0.05f,0f}, {-0.05f,0.05f,0f} };
	int[][] radarHudLines = new int[][] {new int[]{0,1}, new int[]{2,3}, new int[]{4,5}, new int[]{6,7}, new int[]{8,9}, new int[]{8,10}, new int[]{8,11}, new int[]{12,13,14,15,12} };
	float[,] xhairGeom = new float[,] { {-1f, -0.5f, 0f}, {-0.5f, -0.5f, 0f}, {-1f, 0f, 0f}, {0f, 0f, 0f}, {0f, 1f, 0f}, {1f, 0f, 0f}, {1f, -0.5f, 0f}, {0.5f, -0.5f, 0f} };
	int[][] xhairLines = new int[][] {new int[]{3, 4}, new int[]{0, 2, 5, 6}, new int[]{1, 2, 5, 7} };
	float[,] crackGeom = new float[,] { {0f,0f,0f}, {-0.2f,0.2f,0f}, {-0.4f,0.3f,0f}, {-0.45f,0.32f,0f}, {-0.6f,0.05f,0f}, {0f,0.35f,0f}, {-0.2f,0.5f,0f}, {-0.4f,0.65f,0f}, {-0.1f,0.65f,0f}, {0.2f, 0.2f,0f}, {0.25f,0.3f,0f}, {0.4f,0.3f,0f}, {0.6f,0.4f,0f}, {0.4f,0.5f,0f}, {-0.1f,-0.2f,0f}, {0.1f,-0.3f,0f}, {0.3f,-0.6f,0f} };
	int[][] crackLines = new int[][] {new int[]{0, 1, 2, 3}, new int[]{2, 4}, new int[]{1, 5, 6, 7}, new int[]{6, 8}, new int[]{0, 9, 10}, new int[]{9, 11, 12}, new int[]{11, 13}, new int[]{0, 14, 15, 16} };
	float[,] playerGeom = new float[,] { {-0.5f, -0.5f, 0f}, { -0.3f, -0.3f, 0f}, { 0.5f, -0.3f, 0f}, { 0.5f, -0.2f, 0f}, { -0.3f, 0.2f, 0f}, { -0.4f, 0.5f, 0f}, { -0.5f, 0.5f, 0f}, { -0.5f, -0.5f, 0f} };
	float[,] bulletGeom = new float[,] { {-1f,-1f,0f}, {1f,-1f,0f}, {1f,1f,0f}, {-1f,1f,0f}, {-1f,-1f,0f}, {0f,0f,1f}, {1f,-1f,0f}, {0f,0f,1f}, {1f,1f,0f}, {0f,0f,1f}, {-1f,1f,0f} };
	float[,] terrainGeom = new float[,] { { -20f, 0f, 0f }, { -18f, 2f, 0f }, { -15f, 0f, 0f }, { -12f, 0f, 0f }, { -9f, 2f, 0f }, { -6f, 0f, 0f }, { -3f, 0f, 0f }, { -2f, 1f, 0f }, { -1f, 0f, 0f }, { 2f, 0f, 0f }, { 3f, 1f, 0f }, { 4f, 0f, 0f }, { 5f, 0f, 0f }, { 8f, 2f, 0f }, { 11f, 0f, 0f }, { 15f, 0f, 0f }, { 16.5f, 0.5f, 0f }, { 17f, 0.3f, 0f }, { 17.5f, 0.8f, 0f }, { 18f, 0.6f, 0f }, { 18.5f, 1.1f, 0f }, { 19.5f, 0f, 0f }, { 20f, 0f, 0f }, { -20f, 0f, 0f } };
	float[,] cubeGeom = new float[,] { {-1f,0f,-1f}, {-1f,0f,1f}, {1f,0f,1f}, {1f,0f,-1f}, {-1f,0f,-1f}, {-1f,2f,-1f}, {-1f,2f,1f}, {1f,2f,1f}, {1f,2f,-1f}, {-1f,2f,-1f}, {-1f,0f,-1f}, {-1f,0f,1f}, {-1f,2f,1f}, {1f,2f,1f}, {1f,0f,1f}, {1f,0f,-1f}, {1f,2f,-1f} };
	float[,] tankGeom = new float[,] { {-0.4f, -0.1f, -0.8f}, {-0.4f, -0.1f, 0.8f}, {0.4f, -0.1f, 0.8f}, {0.4f, -0.1f, -0.8f}, {-0.5f, 0.1f, -1f}, {-0.5f, 0.1f, 1f}, {0.5f, 0.1f, 1f}, {0.5f, 0.1f, -1f}, {-0.4f, 0.3f, -0.8f}, {-0.4f, 0.3f, 0.5f}, {0.4f, 0.3f, 0.5f}, {0.4f, 0.3f, -0.8f}, {-0.3f, 0.6f, -0.7f}, {0.3f, 0.6f, -0.7f}, {-0.1f, 0.5f, -0.35f}, {-0.1f, 0.5f, 1f}, {0.1f, 0.5f, 1f}, {0.1f, 0.5f, -0.35f}, {-0.1f, 0.55f, -0.55f}, {-0.1f, 0.55f, 1f}, {0.1f, 0.55f, 1f}, {0.1f, 0.55f, -0.55f} };
	int[][] tankLines = new int[][] {new int[] {0, 1, 2, 3, 0, 4, 5, 1, 5, 6, 2, 6, 7, 3, 7, 4, 8, 9, 5, 9, 10, 6, 10, 11, 7, 11, 8, 12, 9, 10, 13, 11, 13, 12, 14, 15, 16, 17, 14, 18, 19, 15, 19, 20, 16, 20, 21, 17, 21, 18} };
	int[][] tankExpLines = new int[][] {new int[] {0, 1, 2, 3, 0, 4, 5, 1, 5, 6}, new int[] {4, 8, 9, 5, 9, 10, 6, 10, 11, 7, 11}, new int[] {8, 12, 9, 10, 13, 11, 13, 12, 14, 15, 16, 17}, new int[] {14, 18, 19, 15, 19, 20, 16, 20, 21, 17, 21, 18} };
	float[,] radarGeom = new float[,] { {-2f, -0.5f, 1f}, { -1f, -1f, 0f}, { -1f, 1f, 0f}, { -2f, 0.5f, 1f}, { -2f, -0.5f, 1f}, {-1f, -1f, 0f}, { 1f, -1f, 0f}, { 1f, 1f, 0f}, { -1f, 1f, 0f}, { -1f, -1f, 0f}, {1f, -1f, 0f}, { 2f, -0.5f, 1f}, { 2f, 0.5f, 1f}, { 1f, 1f, 0f}, { 1f, -1f, 0f}, {0f, -1f, 0f}, {0f, -1.5f, 0f} };
	float[,] ufoGeom = new float[,] { {0f,0.5f,0f}, {0f,0f,1f}, {0.7f,0f,0.7f}, {1f,0f,0f}, {0.7f,0f,-0.7f}, {0f,0f,-1f}, {-0.7f,0f,-0.7f}, {-1f,0f,0f}, {-0.7f,0f,0.7f}, {0f,-0.3f,0.2f}, {0.14f,-0.3f,0.14f}, {0.2f,-0.3f,0f}, {0.14f,-0.3f,-0.14f}, {0f,-0.3f,-0.2f}, {-0.14f,-0.3f,-0.14f}, {-0.2f,-0.3f,0f}, {-0.14f,-0.3f,0.14f} };
	int[][] ufoLines = new int[][] {new int[]{0,1,9},new int[]{0,2,10},new int[]{0,3,11},new int[]{0,4,12},new int[]{0,5,13},new int[]{0,6,14},new int[]{0,7,15},new int[]{0,8,16},new int[]{1,2,3,4,5,6,7,8,1},new int[]{9,10,11,12,13,14,15,16,9} };
	float[,] missileGeom = new float[,] { {-1f,0f,-1f},{-1f,0f,1f},{1f,0f,1f},{1f,0f,-1f},{-0.3f,1f,-0.3f},{-0.3f,1f,0.3f},{0.3f,1f,0.3f},{0.3f,1f,-0.3f},{-0.4f,1.4f,-1.5f},{0.4f,1.4f,-1.5f},{0.8f,2f,-1.5f},{0.4f,2.6f,-1.5f},{-0.4f,2.6f,-1.5f},{-0.8f,2f,-1.5f},{-1f,1f,0f},{1f,1f,0f},{1.5f,2f,0f},{1f,3f,0f},{-1f,3f,0f},{-1.5f,2f,0f},{0f,2f,8f},{0f,3f,0f},{0f,4f,0.2f},{-0.5f,2.5f,4f},{0.5f,2.5f,4f} };
	int[][] missileLines = new int[][] {new int[]{0,1,2,3,0,4,5,1,5,6,2,6,7,3,7,4},new int[]{8,9,10,11,12,13,8},new int[]{14,15,16,17,18,19,14},new int[]{8,14,20},new int[]{9,15,20},new int[]{10,16,20},new int[]{11,17,20},new int[]{12,18,20},new int[]{13,19,20},new int[]{21,22,23,21,22,24,21}};
	float gameStateTimer = 0f, fireTimer = 0f, waveTimer = 5f;
	int gameState = 0, enemyRadar = 0;
	Vector2 TouchJoy(int t) { return (Input.GetTouch(t).position - new Vector2(Screen.width-(Screen.height/4f), Screen.height/2f)) / (Screen.height/4f); }
	Vector2 KeyJoy { get { return (Input.GetKey(KeyCode.LeftArrow)?-Vector2.right:Vector2.zero)+(Input.GetKey(KeyCode.RightArrow)?Vector2.right:Vector2.zero)+(Input.GetKey(KeyCode.DownArrow)?-Vector2.up:Vector2.zero)+(Input.GetKey(KeyCode.UpArrow)?Vector2.up:Vector2.zero); } }
	Vector2 Joystick { get { for(int t=0; t<Input.touchCount; t++) {if(TouchJoy(t).magnitude<=1f) return TouchJoy(t); else if(TouchJoy(t).magnitude<=2f) return TouchJoy(t).normalized;} return KeyJoy; } }
	bool Fire { get { for(int t=0; t<Input.touchCount; t++) {if(TouchJoy(t).x<-2f) return true; } return Input.GetKey(KeyCode.LeftShift); } }
	void Start() {
		gameObject.GetComponent<Camera>().orthographic = false;				// 3D perspective camera!
		gameObject.GetComponent<Camera>().backgroundColor = Color.black;
		gameObject.AddComponent<BoxCollider>().isTrigger = true;			// Player collider
		gameObject.GetComponent<BoxCollider>().size = new Vector3(0.5f, 0.5f, 0.5f);
		gameObject.layer = 1;
		transform.position = new Vector3(0f, 0.3f, -10f);	// Z -10 so can see UI placed at Z 0
 		CreateVectorObject("Terrain1", MakeLines(terrainGeom),    0f, 0f, 40f, (2f*Mathf.PI*40f)/40f, 5f, 1f, 0f, 0f, 0f, 0, Color.green, true, 0.1f).transform.parent = transform;
 		CreateVectorObject("Terrain2", MakeLines(terrainGeom), -200f, 0f, 40f, (2f*Mathf.PI*40f)/40f, 5f, 1f, 0f, 0f, 0f, 0, Color.green, true, 0.1f).transform.parent = transform;
		for (int i=0; i<20; i++)
			CreateVectorObject("Life"+i, MakeLines(playerGeom), 2.8f+i*0.5f, 5.5f, 0f, 0.3f, 0.1f, 1f, 0f, 0f, 0f, 0, Color.green, i<=lives, 0.1f).transform.parent = transform;
 		CreateVectorObject("RadarHud", MakeLines(radarHudGeom, radarHudLines), 0f, 5.0f, 0f, 1f, 1f, 1f, 0f, 0f, 0f, 0, Color.green, true, 0.05f).transform.parent = transform;
 		CreateVectorObject("XHairTop", MakeLines(xhairGeom, xhairLines), 0f, 1.3f, 0f, 1f, 1f, 1f, 0f, 0f, 0f, 0, Color.green, true, 0.05f).transform.parent = transform;
 		CreateVectorObject("XHairBot", MakeLines(xhairGeom, xhairLines), 0f, -0.8f, 0f, 1f, -1f, 1f, 0f, 0f, 0f, 0, Color.green, true, 0.05f).transform.parent = transform;
 		CreateVectorObject("Crack", MakeLines(crackGeom, crackLines), 0f, 0f, 0f, 10f, 10f, 10f, 0f, 0f, 0f, 0, Color.green, false, 0.05f).transform.parent = transform;
		for (int i=0; i<50; i++)
        	allObjects.Add( CreateVectorObject("Obstacle", MakeLines((i<40)?cubeGeom:bulletGeom), Random.Range(-40f, 40f), 0f, Random.Range(-40f, 40f), (i>=40?0.25f:0.5f), (i>=40||Random.value<0.75f?0.25f:0.5f), 0.5f, (i<40)?0f:-90f, 0f, 0f, 4, Color.green) );
		uiObjects.Add( new GameObject("UICanvas") );
		uiObjects[0].AddComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
		for(int i=1; i<=5; i++) {
			uiObjects.Add( new GameObject("UIText") );
			uiObjects[i].transform.parent = uiObjects[0].transform;
			uiObjects[i].AddComponent<Text>().font = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;
			uiObjects[i].GetComponent<Text>().fontSize = i == 5 ? Screen.width/6 : i == 2 ? Screen.width/50 : Screen.width/30;
			uiObjects[i].GetComponent<Text>().alignment = TextAnchor.MiddleLeft;		// Left align text within RectTransform
			uiObjects[i].GetComponent<RectTransform>().pivot = new Vector2(0f, 0.5f);	// Position left of RectTransform
			uiObjects[i].GetComponent<RectTransform>().localPosition = i == 1 ? new Vector3(Screen.width/8f, Screen.height/2.5f, 0f) : i == 2 ? new Vector3(Screen.width/8f, Screen.height/2.8f, 0f) : i == 3 ? new Vector3(-Screen.width/2.2f, Screen.height/2.3f, 0f) : i == 4 ? new Vector3(-Screen.width/2.2f, Screen.height/3f, 0f) : new Vector3(-Screen.width/4f, 0f, 0f);
			uiObjects[i].GetComponent<RectTransform>().sizeDelta = new Vector2(Screen.width, 800);
			uiObjects[i].GetComponent<Text>().color = Color.green;
		}
    }
	Vector3[][] MakeLines(float[,] geom, int[][] geomIndexLines=null) {
		Vector3[][] vLines = new Vector3[(geomIndexLines!=null) ? geomIndexLines.GetLength(0) : 1][];		// No line indices then default to 1 line
		for(int i = 0; i < vLines.GetLength(0); i++) {
			vLines[i] = new Vector3[(geomIndexLines!=null) ? geomIndexLines[i].Length : geom.GetLength(0)];	// No line indices then default to all geometry
			for (int v = 0; v < vLines[i].Length; v++)
				vLines[i][v] = (geomIndexLines!=null) ? new Vector3(geom[geomIndexLines[i][v], 0], geom[geomIndexLines[i][v], 1], geom[geomIndexLines[i][v], 2]) : new Vector3(geom[v, 0], geom[v, 1], geom[v, 2]);
		}
		return vLines;
	}
	GameObject CreateVectorObject(string label, Vector3[][] vLines, float pX, float pY, float pZ, float sX, float sY, float sZ, float rX, float rY, float rZ, int layer, Color color, bool active = true, float thickness=0.01f) {
		GameObject go = new GameObject(label);
		Bounds bounds = GeometryUtility.CalculateBounds(vLines[0], Matrix4x4.identity);
		for(int i=0; i<vLines.GetLength(0); i++) {
			GameObject linego = (i==0) ? go : new GameObject(label+"Child"+i);					// Create a child object for every additional line
			linego.transform.parent = (i==0) ? go.transform.parent : go.transform;
			LineRenderer linerend = linego.AddComponent<LineRenderer>();
			linerend.useWorldSpace = false;
			linerend.widthMultiplier = thickness;
			linerend.positionCount = vLines[i].Length;
			linerend.SetPositions(vLines[i]);
			linerend.material = new Material(Shader.Find("Sprites/Default"));
			linerend.material.color = color;
			bounds.Encapsulate(GeometryUtility.CalculateBounds(vLines[i], Matrix4x4.identity));	// Expand parent's bounding box collider to include child lines
		}
		go.AddComponent<BoxCollider>().isTrigger = true;
		go.GetComponent<BoxCollider>().center = bounds.center;
		go.GetComponent<BoxCollider>().size = bounds.size;
		go.SetActive(active);
		go.layer = layer;
		go.transform.position = new Vector3(pX, pY, pZ);
		go.transform.localScale = new Vector3(sX, sY, sZ);
		go.transform.Rotate(new Vector3(rX, rY, rZ), Space.Self);
		go.AddComponent<GameData>().stateTimeout = Time.unscaledTime + 3f;			// Required by bullets
		go.GetComponent<GameData>().targetPos = go.transform.position;				// Required by tank AI so immediately retargets
		return go;
	}
	void KillPlayer(List<GameObject> destroyed) {
		transform.Find("Life"+lives).gameObject.SetActive(false);
		gameState = (--lives >= 0) ? 1 : 2;         // Player lost a life or player dead = game over?
		gameStateTimer = Time.unscaledTime + 3f;
		destroyed.AddRange(allObjects.Where(x => x.layer==2 || x.layer==3 || x.layer==6 || x.layer==7));	// Remove all bullets, tanks, UFOs, missiles from the field
	}
	void Score(int add) {
		lives = (score / 50000) < ((score + add) / 50000) ? ((lives < 19) ? lives + 1 : lives) : lives;
		transform.Find("Life"+lives).gameObject.SetActive(true);
		score += add;
		PlayerPrefs.SetInt("BattlezoneHighScore", score > PlayerPrefs.GetInt("BattlezoneHighScore") ? score : PlayerPrefs.GetInt("BattlezoneHighScore"));
	}
	void Update() {
		uiObjects[1].GetComponent<Text>().text = string.Format("SCORE     {0:00000}", score);
		uiObjects[2].GetComponent<Text>().text = string.Format("HIGH SCORE       {0:00000}", PlayerPrefs.GetInt("BattlezoneHighScore"));
		uiObjects[3].GetComponent<Text>().text = ((enemyRadar&1)!=0) ? "ENEMY IN RANGE" : "";
		uiObjects[4].GetComponent<Text>().text = ((enemyRadar&2)!=0) ? "ENEMY TO LEFT" : ((enemyRadar&4)!=0) ? "ENEMY TO RIGHT" : "";
		uiObjects[5].GetComponent<Text>().text = gameState == 2 ? "GAME OVER" : "";
		transform.Find("RadarHud/RadarHudChild6").Rotate(new Vector3(0f, 0f, -90f*Time.deltaTime));	// Rotate radar second hand
		transform.Find("RadarHud/RadarHudChild7").gameObject.SetActive((enemyRadar & 1)!=0);		// Only show enemy dot on radar if within range
		transform.Find("XHairTop/XHairTopChild1").gameObject.SetActive((enemyRadar & 8)==0);		// Show XHair 1 if no enemy centred on screen
		transform.Find("XHairBot/XHairBotChild1").gameObject.SetActive((enemyRadar & 8)==0);
		transform.Find("XHairTop/XHairTopChild2").gameObject.SetActive((enemyRadar & 8)!=0);		// Show XHair 2 if enemy centred on screen
		transform.Find("XHairBot/XHairBotChild2").gameObject.SetActive((enemyRadar & 8)!=0);
		if (gameState > 0) {
			transform.Find("Crack").gameObject.SetActive(true);
			if ((gameState == 1) && (Time.unscaledTime > gameStateTimer)) {
				transform.Find("Crack").gameObject.SetActive(false);
				gameState = 0;																// Back to playing
			}
			if ((gameState == 2) && ((Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0)))) {
				score = 0; lives = 2; level = 1;                    						// New game
				UnityEngine.SceneManagement.SceneManager.LoadScene("Battlezone");			// Reload scene for new level
			}
			return;																			// Pause game = no update
		}
		List<GameObject> destroyed = new List<GameObject>(), added = new List<GameObject>();
		if (Fire && (Time.unscaledTime > fireTimer)) {
			Vector3 pos = transform.position + transform.forward*1f;
			allObjects.Add(CreateVectorObject("Bullet", MakeLines(bulletGeom), pos.x, pos.y, pos.z, 0.05f, 0.05f, 0.2f, 0f, transform.eulerAngles.y, 0f, 2, Color.green));
			fireTimer = Time.unscaledTime + 1f;
		}
		enemyRadar = 0;
		foreach(GameObject go in allObjects) {
			foreach(LineRenderer lr in go.GetComponentsInChildren<LineRenderer>())
				lr.widthMultiplier = 0.005f + (Mathf.Clamp((go.transform.position-transform.position).magnitude, 1f, 10f)/10f) * 0.03f;	// Wider line width as get farther away
			switch(go.layer) {
/* Bullet */	case 2:	Collider[] hits = Physics.OverlapBox(go.GetComponent<Collider>().bounds.center, go.GetComponent<Collider>().bounds.extents, go.transform.rotation, (1<<1)+(1<<3)+(1<<4)+(1<<6)+(1<<7));
						for(int h=0; (hits != null) && (h < hits.Length); h++) {
							if(hits[h].gameObject.layer == 1) {					// Bullet hit player
								KillPlayer(destroyed);
							}
							else if(hits[h].gameObject.layer == 3 || hits[h].gameObject.layer == 6 || hits[h].gameObject.layer == 7) {	// Bullet hit a tank or UFO or missile
								if(hits[h].gameObject.layer == 3 || hits[h].gameObject.layer == 7)				// Tanks explode into debris pieces
									for (int i=0; i<tankExpLines.Length+1; i++) {	// Last explosion debris is the separate radar dish
										Vector3 p = hits[h].gameObject.transform.position;
	        							added.Add(i<tankExpLines.Length?CreateVectorObject("Explosion", MakeLines(tankGeom, new int[1][]{tankExpLines[i]}), p.x, p.y, p.z, 0.5f, 0.5f, 0.5f, Random.Range(-180f,180f), Random.Range(-180f,180f), Random.Range(-180f,180f), 5, Color.green):CreateVectorObject("Explosion", MakeLines(radarGeom), p.x, p.y, p.z, 0.05f, 0.05f, 0.05f, Random.Range(-180f,180f), Random.Range(-180f,180f), Random.Range(-180f,180f), 5, Color.green));
										added[added.Count-1].GetComponent<GameData>().targetPos = new Vector3(Random.Range(-1f,1f),1f,Random.Range(-1f,1f));
									}
								Score( hits[h].gameObject.layer == 3 ? 1000 : hits[h].gameObject.layer == 7 ? 2000 : 5000);
								destroyed.Add(hits[h].gameObject);				// Destroy tank/UFO/missile
							}
						}
						go.transform.Translate(go.transform.forward*10f*Time.deltaTime, Space.World);
						if(((hits!=null)&&(hits.Length>0)) || (Time.unscaledTime > go.GetComponent<GameData>().stateTimeout))	// Bullets die if hit something or after x seconds
							destroyed.Add(go);
				break;
/* Tank */		case 3: go.transform.Find("Radar").Rotate(new Vector3(0f, 60f*Time.deltaTime, 0f));		// Rotate radar
						Vector3 desiredDir = new Vector3(transform.position.x-go.transform.position.x, 0f, transform.position.z-go.transform.position.z);
						Vector3 radarDir = transform.InverseTransformPoint(go.transform.position) * 0.05f;
						if((desiredDir.magnitude < 20f) && ( ((enemyRadar&1)==0) || (radarDir.magnitude < transform.Find("RadarHud/RadarHudChild7").localPosition.magnitude)))
							transform.Find("RadarHud/RadarHudChild7").localPosition = new Vector3(radarDir.x, radarDir.z, 0f);
						enemyRadar |= (desiredDir.magnitude < 20f) ? 1 : 0;
						enemyRadar |= ((Camera.main.WorldToViewportPoint(go.transform.position).x<0)||(Camera.main.WorldToViewportPoint(go.transform.position).x>1)) ? ((Vector3.Dot(desiredDir, transform.right) > 0f) ? 2 : 4) : 0;
						enemyRadar |= ((Camera.main.WorldToViewportPoint(go.transform.position).x>0.4f)&&(Camera.main.WorldToViewportPoint(go.transform.position).x<0.6f)) ? 8 : 0;
						if(go.GetComponent<GameData>().state == 0) {									// Aim at target position, retarget if reached
							desiredDir = new Vector3(go.GetComponent<GameData>().targetPos.x-go.transform.position.x, 0f, go.GetComponent<GameData>().targetPos.z-go.transform.position.z);
							if(desiredDir.magnitude < 0.1f) {
								go.GetComponent<GameData>().state = (/*(Random.value > 0.5f) && */(Time.unscaledTime > go.GetComponent<GameData>().stateTimeout)) ? 2 : 0;		// Target player (not straight away after spawn) or random position
								go.GetComponent<GameData>().targetPos = go.transform.position + new Vector3(Random.Range(-5f,5f),0f,Random.Range(-5f,5f));
								break;
							}
						}
						else if(go.GetComponent<GameData>().state == 1) {								// Back up
							go.transform.Translate(-go.transform.forward*Time.deltaTime, Space.World);	// Drive backwards
							if(Time.unscaledTime > go.GetComponent<GameData>().stateTimeout) {			// Until timeout
								go.GetComponent<GameData>().state = 0;									// Drive to a position off to one side
								go.GetComponent<GameData>().targetPos = go.transform.position + (go.transform.right * Random.Range(-5f,5f)) + (go.transform.forward * Random.Range(-1f,0f));
							}
							break;	// If backing-up dont do normal driving or collision handling
						}
						float angle = Quaternion.Angle(go.transform.rotation, Quaternion.LookRotation(desiredDir, Vector3.up));	// Angle to desiredDir
						float whichWay = Vector3.Cross(go.transform.forward, desiredDir).y; 			// Left or right?
						angle = (whichWay < 0.0f) ? -angle : angle;
						if(Mathf.Abs(angle) < 3f)
							go.transform.Translate(go.transform.forward*Time.deltaTime, Space.World);	// Drive if facing desiredDir
						go.transform.Rotate(new Vector3(0f, Mathf.Clamp(angle, -20f*Time.deltaTime, 20f*Time.deltaTime), 0f));	// Face desiredDir
						Collider[] ohits = Physics.OverlapBox(go.GetComponent<Collider>().bounds.center, go.GetComponent<Collider>().bounds.extents, go.transform.rotation, (1<<1)+(1<<3)+(1<<4));
						if((ohits != null) && (ohits.Length > 1)) {		// If hit an obstacle then back up and retarget (note always overlaps itself hence > 1)
							go.GetComponent<GameData>().state = 1;
							go.GetComponent<GameData>().stateTimeout = Time.unscaledTime + 1f;
						}
						if(go.GetComponent<GameData>().state == 2) {			// Attack player (desiredDir defaults to player direction)
							if ( (desiredDir.magnitude < 10f) && (Mathf.Abs(angle) < 3f) && ((Time.unscaledTime > go.GetComponent<GameData>().stateTimeout)) ) {
								Vector3 pos = go.transform.position + go.transform.forward*1f + go.transform.up*0.3f;
								added.Add(CreateVectorObject("Bullet", MakeLines(bulletGeom), pos.x, pos.y, pos.z, 0.05f, 0.05f, 0.2f, 0f, go.transform.eulerAngles.y, 0f, 2, Color.green));
								go.GetComponent<GameData>().stateTimeout = Time.unscaledTime + 5f;		// Firing rate
							}
						}
				break;
/* Explosion */	case 5: go.transform.Translate(go.GetComponent<GameData>().targetPos*5f*Time.deltaTime, Space.World);	// Move debris
						go.GetComponent<GameData>().targetPos -= new Vector3(0f, 1f*Time.deltaTime, 0f);	// Apply fake gravity to the debris's vector
						go.transform.RotateAround(go.transform.TransformPoint(go.GetComponent<BoxCollider>().center), new Vector3(1f,2f,3f), 720f*Time.deltaTime);// Rotate around debris centre
						if(go.transform.position.y < 0f)													// Explosions die when they hit the ground
							destroyed.Add(go);
				break;
/* UFO */		case 6: go.transform.Rotate(new Vector3(0f, 90f*Time.deltaTime, 0f));
						go.transform.Translate(new Vector3(Mathf.Sin(Time.time*0.1f), 0f, Mathf.Cos(Time.time*0.1f))*1f*Time.deltaTime, Space.World);
				break;
/* Missile */	case 7: go.transform.rotation = Quaternion.RotateTowards(go.transform.rotation, Quaternion.AngleAxis(Mathf.Sin(5f*Time.time)*(level<15?0f:30f)*Mathf.Clamp01((transform.position-go.transform.position).magnitude*0.1f), Vector3.up)*Quaternion.LookRotation(transform.position-go.transform.position), 90f*Time.deltaTime);
						go.transform.Translate(go.transform.position.y>0f?-Vector3.up*0.1f:new Vector3(go.transform.forward.x,0f,go.transform.forward.z)*10f*Time.deltaTime, Space.World);
						if((transform.position-go.transform.position).magnitude < 1f)
							KillPlayer(destroyed);
				break;
			}
		}
		foreach(GameObject dead in destroyed) {
			allObjects.Remove(dead);
			Destroy(dead);
		}
		allObjects.AddRange(added);
		transform.Rotate(new Vector3(0f, Joystick.x*20f*Time.deltaTime, 0f));	// Player control - aka the camera
		transform.Translate(new Vector3(0f, 0f, Joystick.y*1f*Time.deltaTime));
		Collider[] phits = Physics.OverlapBox(gameObject.GetComponent<Collider>().bounds.center, gameObject.GetComponent<Collider>().bounds.extents, transform.rotation, (1<<3)+(1<<4));	// Hit tank or obstacle
		if((phits != null) && (phits.Length > 0))
			transform.Translate(new Vector3(0f, 0f, -Joystick.y*1f*Time.deltaTime));
		transform.Find("Terrain1").transform.localPosition = new Vector3(((transform.eulerAngles.y / 180f)-1f) * -(2f*Mathf.PI*40f*0.5f), 0f, 40f);
		transform.Find("Terrain2").transform.localPosition = new Vector3(transform.GetChild(0).transform.localPosition.x+((transform.eulerAngles.y<180f)?-(2f*Mathf.PI*40f):(2f*Mathf.PI*40f)), 0f, 40f);	// Wrap second copy of terrain on end that needs it
		if( (allObjects.Where(x => x.layer == 3 || x.layer == 7).Count() == 0) && ((waveTimer-=Time.deltaTime)<0f) ) {	// Wave complete when all tanks and missiles are dead
			level++;
			for (int i=0; i<(((score>30000)&&(Random.value<0.2f))?0:(level<=30)?1:2); i++) {	// Missiles start at 30K (dont create any tanks)
    	    	allObjects.Add( CreateVectorObject("Tank", MakeLines(tankGeom, tankLines), transform.position.x+Random.Range(-10f, 10f), 0.05f, transform.position.z+Random.Range(-10f, 10f), 0.5f, 0.5f, 0.5f, 0f, 0f, 0f, 3, Color.green) );
				CreateVectorObject("Radar", MakeLines(radarGeom), allObjects[allObjects.Count-1].transform.position.x, 0.42f, allObjects[allObjects.Count-1].transform.position.z-0.35f, 0.05f, 0.05f, 0.05f, 0f, 0f, 0f, 0, Color.green).transform.parent = allObjects[allObjects.Count-1].transform;
			}
			if(allObjects.Where(x => x.layer == 3).Count() == 0)			// If no tanks created then create a missile
				allObjects.Add( CreateVectorObject("Missile", MakeLines(missileGeom, missileLines), transform.position.x+transform.forward.x*20f, 10f, transform.position.z+transform.forward.z*20f, 0.2f, 0.2f, 0.2f, -10f, Quaternion.LookRotation(-transform.forward).eulerAngles.y, 0f, 7, Color.green) );
			if(allObjects.Where(x => x.layer == 6).Count() == 0)			// Generate a new UFO if one doesnt exist
				allObjects.Add( CreateVectorObject("UFO", MakeLines(ufoGeom, ufoLines), Random.Range(-10f, 10f), 0.5f, Random.Range(-10f, 10f), 0.5f, 0.5f, 0.5f, 0f, 0f, 0f, 6, Color.green) );
			waveTimer = 5f;	// Time between attack waves
		}
    }
}