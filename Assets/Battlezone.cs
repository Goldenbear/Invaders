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
	GameObject[] uiObjects = new GameObject[10];
	List<GameObject> allObjects = new List<GameObject>(), playerLives = new List<GameObject>();
	float[,] playerGeom = new float[,] { {-0.5f, -0.5f, 0f}, { -0.3f, -0.3f, 0f}, { 0.5f, -0.3f, 0f}, { 0.5f, -0.2f, 0f}, { -0.3f, 0.2f, 0f}, { -0.4f, 0.5f, 0f}, { -0.5f, 0.5f, 0f}, { -0.5f, -0.5f, 0f} };
	float[,] bulletGeom = new float[,] { {-1f, -1f, 0f}, { 1f, -1f, 0f}, { 1f, 1f, 0f}, { -1f, 1f, 0f}, { -1f, -1f, 0f}, { 0f, 0f, 1f}, { 1f, -1f, 0f}, { 0f, 0f, 1f}, { 1f, 1f, 0f}, { 0f, 0f, 1f}, { -1f, 1f, 0f} };
	float[,] terrainGeom = new float[,] { { -20f, 0f, 0f }, { -18f, 2f, 0f }, { -15f, 0f, 0f }, { -12f, 0f, 0f }, { -9f, 2f, 0f }, { -6f, 0f, 0f }, { -3f, 0f, 0f }, { -2f, 1f, 0f }, { -1f, 0f, 0f }, { 2f, 0f, 0f }, { 3f, 1f, 0f }, { 4f, 0f, 0f }, { 5f, 0f, 0f }, { 8f, 2f, 0f }, { 11f, 0f, 0f }, { 15f, 0f, 0f }, { 16.5f, 0.5f, 0f }, { 17f, 0.3f, 0f }, { 17.5f, 0.8f, 0f }, { 18f, 0.6f, 0f }, { 18.5f, 1.1f, 0f }, { 19.5f, 0f, 0f }, { 20f, 0f, 0f }, { -20f, 0f, 0f } };
	float[,] cubeGeom = new float[,] { {-1f, -1f, -1f}, {-1f, -1f, 1f}, {1f, -1f, 1f}, {1f, -1f, -1f}, {-1f, -1f, -1f}, {-1f, 1f, -1f}, {-1f, 1f, 1f}, {1f, 1f, 1f}, {1f, 1f, -1f}, {-1f, 1f, -1f}, {-1f, -1f, -1f}, {-1f, -1f, 1f}, {-1f, 1f, 1f}, {1f, 1f, 1f}, {1f, -1f, 1f}, {1f, -1f, -1f}, {1f, 1f, -1f} };
	float[,] tankGeom = new float[,] { {-0.4f, -0.1f, -0.8f}, {-0.4f, -0.1f, 0.8f}, {0.4f, -0.1f, 0.8f}, {0.4f, -0.1f, -0.8f}, {-0.5f, 0.1f, -1f}, {-0.5f, 0.1f, 1f}, {0.5f, 0.1f, 1f}, {0.5f, 0.1f, -1f}, {-0.4f, 0.3f, -0.8f}, {-0.4f, 0.3f, 0.5f}, {0.4f, 0.3f, 0.5f}, {0.4f, 0.3f, -0.8f}, {-0.3f, 0.6f, -0.7f}, {0.3f, 0.6f, -0.7f}, {-0.1f, 0.5f, -0.35f}, {-0.1f, 0.5f, 1f}, {0.1f, 0.5f, 1f}, {0.1f, 0.5f, -0.35f}, {-0.1f, 0.55f, -0.55f}, {-0.1f, 0.55f, 1f}, {0.1f, 0.55f, 1f}, {0.1f, 0.55f, -0.55f} };
	int[] tankGInd = new int[] {0, 1, 2, 3, 0, 4, 5, 1, 5, 6, 2, 6, 7, 3, 7, 4, 8, 9, 5, 9, 10, 6, 10, 11, 7, 11, 8, 12, 9, 10, 13, 11, 13, 12, 14, 15, 16, 17, 14, 18, 19, 15, 19, 20, 16, 20, 21, 17, 21, 18};
	int[][] tankExpGInd = new int[][] {new int[] {0, 1, 2, 3, 0, 4, 5, 1, 5, 6}, new int[] {4, 8, 9, 5, 9, 10, 6, 10, 11, 7, 11}, new int[] {8, 12, 9, 10, 13, 11, 13, 12, 14, 15, 16, 17}, new int[] {14, 18, 19, 15, 19, 20, 16, 20, 21, 17, 21, 18} };
	float[,] radarGeom = new float[,] { {-2f, -0.5f, 1f}, { -1f, -1f, 0f}, { -1f, 1f, 0f}, { -2f, 0.5f, 1f}, { -2f, -0.5f, 1f}, {-1f, -1f, 0f}, { 1f, -1f, 0f}, { 1f, 1f, 0f}, { -1f, 1f, 0f}, { -1f, -1f, 0f}, {1f, -1f, 0f}, { 2f, -0.5f, 1f}, { 2f, 0.5f, 1f}, { 1f, 1f, 0f}, { 1f, -1f, 0f}, {0f, -1f, 0f}, {0f, -1.5f, 0f} };
	float gameStateTimer = 0f;	// Both timer for losing life and auto-fire!
	int gameState = 0, enemyRadar = 0;
	Vector2 TouchJoy(int t) { return (Input.GetTouch(t).position - new Vector2(Screen.width-(Screen.height/4f), Screen.height/4f)) / (Screen.height/4f); }
	Vector2 KeyJoy { get { return (Input.GetKey(KeyCode.LeftArrow)?-Vector2.right:Vector2.zero)+(Input.GetKey(KeyCode.RightArrow)?Vector2.right:Vector2.zero)+(Input.GetKey(KeyCode.DownArrow)?-Vector2.up:Vector2.zero)+(Input.GetKey(KeyCode.UpArrow)?Vector2.up:Vector2.zero); } }
	Vector2 Joystick { get { for(int t=0; t<Input.touchCount; t++) {if(TouchJoy(t).magnitude<2f) return TouchJoy(t);} return KeyJoy; } }
	bool Fire { get { for(int t=0; t<Input.touchCount; t++) {if(TouchJoy(t).x<-2f) return true; } return Input.GetKey(KeyCode.LeftShift); } }
	void Start() {
		gameObject.GetComponent<Camera>().orthographic = false;				// 3D perspective camera!
		gameObject.GetComponent<Camera>().backgroundColor = Color.black;
		gameObject.AddComponent<BoxCollider>().isTrigger = true;			// Player collider
		gameObject.GetComponent<BoxCollider>().size = new Vector3(0.5f, 0.5f, 0.5f);
		gameObject.layer = 1;
		transform.Translate(new Vector3(0f, 0.3f, 0f));
 		CreateVectorObject("Terrain1", VertexArray(terrainGeom),    0f, 0f, 40f, (2f*Mathf.PI*40f)/40f, 5f, 1f, 0f, 0f, 0f, 0, Color.green, true, true, 0.1f).transform.parent = transform;
 		CreateVectorObject("Terrain2", VertexArray(terrainGeom), -200f, 0f, 40f, (2f*Mathf.PI*40f)/40f, 5f, 1f, 0f, 0f, 0f, 0, Color.green, true, true, 0.1f).transform.parent = transform;
		for (int i=0; i<20; i++) {
			playerLives.Add( CreateVectorObject("Life", VertexArray(playerGeom), 0.2f+i*0.5f, 5.5f, 0f, 0.3f, 0.1f, 1f, 0f, 0f, 0f, 0, Color.green, i<=lives, true, 0.1f) );
			playerLives[i].transform.parent = gameObject.transform;
		}
		for (int i=0; i<50; i++)
        	allObjects.Add( CreateVectorObject("Obstacle", VertexArray(cubeGeom), Random.Range(-40f, 40f), 0.25f, Random.Range(-40f, 40f), 0.5f, 0.25f, 0.5f, 0f, 0f, 0f, 4, Color.green) );
		uiObjects[0] = new GameObject("UICanvas");
		uiObjects[0].AddComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
		for(int i=0; i<5; i++) {
			uiObjects[1+i] = new GameObject("UIText");
			uiObjects[1+i].transform.parent = uiObjects[0].transform;
			uiObjects[1+i].AddComponent<Text>().font = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;
			uiObjects[1+i].GetComponent<Text>().fontSize = i == 1 ? Screen.width/50 : Screen.width/30;
			uiObjects[1+i].GetComponent<Text>().alignment = TextAnchor.MiddleLeft;		// Left align text within RectTransform
			uiObjects[1+i].GetComponent<RectTransform>().pivot = new Vector2(0f, 0.5f);	// Position left of RectTransform
			uiObjects[1+i].GetComponent<RectTransform>().localPosition = i == 0 ? new Vector3(0f, Screen.height/2.5f, 0f) : i == 1 ? new Vector3(0f, Screen.height/2.8f, 0f) : i == 2 ? new Vector3(-Screen.width/2.2f, Screen.height/2.5f, 0f) : i == 3 ? new Vector3(-Screen.width/2.2f, Screen.height/2.8f, 0f) : new Vector3(0f, 0f, 0f);
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
	GameObject CreateVectorObject(string label, Vector3[] geomVs, float pX, float pY, float pZ, float sX, float sY, float sZ, float rX, float rY, float rZ, int layer, Color color, bool active = true, bool visible = true, float thickness=0.01f) {
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
		go.SetActive(active);
		go.layer = layer;
		go.transform.position = new Vector3(pX, pY, pZ);
		go.transform.localScale = new Vector3(sX, sY, sZ);
		go.transform.Rotate(new Vector3(rX, rY, rZ), Space.Self);
		go.GetComponent<Renderer>().material = new Material(Shader.Find("Sprites/Default"));
		go.GetComponent<Renderer>().material.color = color;
		go.GetComponent<Renderer>().enabled = visible;
		go.AddComponent<GameData>().stateTimeout = Time.unscaledTime + 3f;			// Required by bullets
		go.GetComponent<GameData>().targetPos = go.transform.position;				// Required by tank AI so immediately retargets
		return go;
	}
	void KillPlayer() {
		playerLives[lives].SetActive(false);
		gameState = (--lives >= 0) ? 1 : 2;         // Player lost a life or player dead = game over?
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
		uiObjects[3].GetComponent<Text>().text = ((enemyRadar&1)!=0) ? "ENEMY IN RANGE" : "";
		uiObjects[4].GetComponent<Text>().text = ((enemyRadar&2)!=0) ? "ENEMY TO LEFT" : ((enemyRadar&4)!=0) ? "ENEMY TO RIGHT" : "";
		uiObjects[5].GetComponent<Text>().text = gameState == 2 ? "GAME OVER" : "";
		if (gameState > 0) {
			if ((gameState == 1) && (Time.unscaledTime > gameStateTimer)) {
				gameState = 0;																// Back to playing
			}
			if ((gameState == 2) && ((Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0)))) {
				score = 0; lives = 2; level = 1;                    						// New game
				UnityEngine.SceneManagement.SceneManager.LoadScene("Battlezone");			// Reload scene for new level
			}
			return;																			// Pause game = no update
		}
		List<GameObject> destroyed = new List<GameObject>(), added = new List<GameObject>();
		if (Fire && (Time.unscaledTime > gameStateTimer)) {
			Vector3 pos = transform.position + transform.forward*1f;
			allObjects.Add(CreateVectorObject("Bullet", VertexArray(bulletGeom), pos.x, pos.y, pos.z, 0.05f, 0.05f, 0.2f, 0f, transform.eulerAngles.y, 0f, 2, Color.green));
			gameStateTimer = Time.unscaledTime + 1f;
		}
		enemyRadar = 0;
		foreach(GameObject go in allObjects) {
			go.GetComponent<LineRenderer>().widthMultiplier = 0.005f + (Mathf.Clamp((go.transform.position-transform.position).magnitude, 1f, 10f)/10f) * 0.03f;	// Wider line width as get farther away
			switch(go.layer) {
/* Bullet */	case 2:	Collider[] hits = Physics.OverlapBox(go.GetComponent<Collider>().bounds.center, go.GetComponent<Collider>().bounds.extents, go.transform.rotation, (1<<1)+(1<<3)+(1<<4));
						for(int h=0; (hits != null) && (h < hits.Length); h++) {
							if(hits[h].gameObject.layer == 1) {					// Bullet hit player
								KillPlayer();
								destroyed.AddRange(allObjects.Where(x => x.layer == 2 || x.layer == 3));	// Remove all bullets and tanks from the field
							}
							else if (hits[h].gameObject.layer == 3) {			// Bullet hit a tank
								Score(100);
								for (int i=0; i<tankExpGInd.Length+1; i++) {	// Last explosion debris is the separate radar dish
									Vector3 p = hits[h].gameObject.transform.position;
        							GameObject ex = i<tankExpGInd.Length?CreateVectorObject("Explosion", VertexArray(tankGeom, tankExpGInd[i]), p.x, p.y, p.z, 0.5f, 0.5f, 0.5f, Random.Range(-180f,180f), Random.Range(-180f,180f), Random.Range(-180f,180f), 5, Color.green):CreateVectorObject("Explosion", VertexArray(radarGeom), p.x, p.y, p.z, 0.05f, 0.05f, 0.05f, Random.Range(-180f,180f), Random.Range(-180f,180f), Random.Range(-180f,180f), 5, Color.green);
									ex.GetComponent<GameData>().targetPos = new Vector3(Random.Range(-1f,1f),1f,Random.Range(-1f,1f));
									added.Add(ex);
								}
								destroyed.Add(go);
								destroyed.Add(hits[h].gameObject);
							}
							else if(hits[h].gameObject.layer == 4) {			// Bullet hit obstacle
								destroyed.Add(go);
							}
						}
						go.transform.Translate(go.transform.forward*10f*Time.deltaTime, Space.World);
						if((Time.unscaledTime > go.GetComponent<GameData>().stateTimeout))				// Bullets die after x seconds
							destroyed.Add(go);
				break;
/* Tank */		case 3: go.transform.GetChild(0).Rotate(new Vector3(0f, 60f*Time.deltaTime, 0f));		// Rotate radar
						Vector3 desiredDir = new Vector3(transform.position.x-go.transform.position.x, 0f, transform.position.z-go.transform.position.z);
						float driveSpeed = 1f;
						enemyRadar |= (desiredDir.magnitude < 10f) ? 1 : 0;
						enemyRadar |= ((Camera.main.WorldToViewportPoint(go.transform.position).x<0)||(Camera.main.WorldToViewportPoint(go.transform.position).x>1)) ? ((Vector3.Dot(desiredDir, transform.right) > 0f) ? 2 : 4) : 0;
						if(go.GetComponent<GameData>().state == 0) {			// Drive to the target position
							desiredDir = new Vector3(go.GetComponent<GameData>().targetPos.x-go.transform.position.x, 0f, go.GetComponent<GameData>().targetPos.z-go.transform.position.z);
							if(desiredDir.magnitude < 0.1f) {
								go.GetComponent<GameData>().state = Random.value > 0.5f ? 2 : 0;
								go.GetComponent<GameData>().targetPos = go.transform.position + new Vector3(Random.Range(-10f,10f),0f,Random.Range(-10f,10f));
								break;
							}
						}
						else if(go.GetComponent<GameData>().state == 1) {		// Back up
							desiredDir = go.transform.forward;
							driveSpeed = -1f;
							if(Time.unscaledTime > go.GetComponent<GameData>().stateTimeout) {
								go.GetComponent<GameData>().state = Random.value > 0.5f ? 2 : 0;
								go.GetComponent<GameData>().targetPos = go.transform.position + (go.transform.right * Random.Range(-10f,10f)) + (go.transform.forward * Random.Range(-10f,0));
								break;
							}
						}
						float angle = Quaternion.Angle(go.transform.rotation, Quaternion.LookRotation(desiredDir, Vector3.up));
						float whichWay = Vector3.Cross(go.transform.forward, desiredDir).y; // Left or right?
						angle = (whichWay < 0.0f) ? -angle : angle;
						if(Mathf.Abs(angle) < 3f)
							go.transform.Translate(go.transform.forward*driveSpeed*Time.deltaTime, Space.World);
						go.transform.Rotate(new Vector3(0f, Mathf.Clamp(angle, -30f*Time.deltaTime, 30f*Time.deltaTime), 0f));
						Collider[] ohits = Physics.OverlapBox(go.GetComponent<Collider>().bounds.center, go.GetComponent<Collider>().bounds.extents, go.transform.rotation, (1<<1)+(1<<3)+(1<<4));
						if((ohits != null) && (ohits.Length > 1)) {				// If hit an obstacle then back up and retarget (note always overlaps itself hence > 1)
							go.GetComponent<GameData>().state = 1;
							go.GetComponent<GameData>().stateTimeout = Time.unscaledTime + 1f;
						}
						if(go.GetComponent<GameData>().state == 2) {			// Shoot player
							if ( (desiredDir.magnitude < 10f) && (Mathf.Abs(angle) < 3f) && ((Time.unscaledTime > go.GetComponent<GameData>().stateTimeout)) ) {
								Vector3 pos = go.transform.position + go.transform.forward*1f + go.transform.up*0.3f;
								added.Add(CreateVectorObject("Bullet", VertexArray(bulletGeom), pos.x, pos.y, pos.z, 0.05f, 0.05f, 0.2f, 0f, go.transform.eulerAngles.y, 0f, 2, Color.green));
								go.GetComponent<GameData>().stateTimeout = Time.unscaledTime + 5f;	// Firing rate
							}
						}
				break;
/* Explosion */	case 5: go.transform.Translate(go.GetComponent<GameData>().targetPos*5f*Time.deltaTime, Space.World);
						go.GetComponent<GameData>().targetPos -= new Vector3(0f, 1f*Time.deltaTime, 0f);		// Apply fake gravity to the debris's vector
						go.transform.Translate(-Vector3.Cross(go.GetComponent<BoxCollider>().center, go.transform.localScale), Space.Self);	// Rotate around debris centre (use BoxCollider*scale)
						go.transform.Rotate(new Vector3(90f*Time.deltaTime, 720f*Time.deltaTime, 360f*Time.deltaTime), Space.Self);
						go.transform.Translate(Vector3.Cross(go.GetComponent<BoxCollider>().center, go.transform.localScale), Space.Self);
						if(go.transform.position.y < 0f)											// Explosions die when they hit the ground
							destroyed.Add(go);
				break;
			}
		}
		foreach(GameObject dead in destroyed) {
			allObjects.Remove(dead);
			Destroy(dead);
		}
		allObjects.AddRange(added);
		transform.Rotate(new Vector3(0f, Joystick.x*30f*Time.deltaTime, 0f));	// Player control - aka the camera
		transform.Translate(new Vector3(0f, 0f, Joystick.y*1f*Time.deltaTime));
		Collider[] phits = Physics.OverlapBox(gameObject.GetComponent<Collider>().bounds.center, gameObject.GetComponent<Collider>().bounds.extents, transform.rotation, (1<<3)+(1<<4));	// Hit tank or obstacle
		if((phits != null) && (phits.Length > 0)) {
			transform.Translate(new Vector3(0f, 0f, -Joystick.y*1f*Time.deltaTime));
		}
		transform.Find("Terrain1").transform.localPosition = new Vector3(((transform.eulerAngles.y / 180f)-1f) * -(2f*Mathf.PI*40f*0.5f), 0f, 40f);
		transform.Find("Terrain2").transform.localPosition = new Vector3(transform.GetChild(0).transform.localPosition.x+((transform.eulerAngles.y<180f)?-(2f*Mathf.PI*40f):(2f*Mathf.PI*40f)), 0f, 40f);	// Wrap second copy of terrain on end that needs it
		if(allObjects.Where(x => x.layer == 3).Count() == 0) {				// Wave complete
			level++;
			for (int i=0; i<((level <= 3)?1:2); i++) {
				Vector3 spawnCentre = transform.position+(transform.forward*10f);
    	    	allObjects.Add( CreateVectorObject("Tank", VertexArray(tankGeom, tankGInd), Random.Range(spawnCentre.x-10f, spawnCentre.x+10f), 0.05f, Random.Range(spawnCentre.z-10f, spawnCentre.z+10f), 0.5f, 0.5f, 0.5f, 0f, 0f, 0f, 3, Color.green) );
				CreateVectorObject("Radar", VertexArray(radarGeom), allObjects[allObjects.Count-1].transform.position.x, 0.42f, allObjects[allObjects.Count-1].transform.position.z-0.35f, 0.05f, 0.05f, 0.05f, 0f, 0f, 0f, 0, Color.green).transform.parent = allObjects[allObjects.Count-1].transform;
			}
		}
    }
}