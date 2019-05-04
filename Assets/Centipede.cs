using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
public class Centipede : MonoBehaviour {
	public class GameData : MonoBehaviour {
		public Vector2Int targetCell;
	}
	static int score = 0, lives = 2, level = 1;
	int[,] grid = new int[30,30];
	List<GameObject> allObjects = new List<GameObject>(), playerLives = new List<GameObject>(), centipede = new List<GameObject>(), uiObjects = new List<GameObject>();
	Vector3[] sphereVerts = { new Vector3(0f, 0f, 0f), new Vector3(-0.5f, -0.1f, 0f), new Vector3(-0.5f, 0.1f, 0f), new Vector3(-0.35f, 0.35f, 0f), new Vector3(-0.1f, 0.5f, 0f), new Vector3(0.1f, 0.5f, 0f), new Vector3(0.35f, 0.35f, 0f), new Vector3(0.5f, 0.1f, 0f), new Vector3(0.5f, -0.1f, 0f), new Vector3(0.35f, -0.35f, 0f), new Vector3(0.1f, -0.5f, 0f), new Vector3(-0.1f, -0.5f, 0f), new Vector3(-0.35f, -0.35f, 0f) };
	int[] sphereTris = new int[] {0, 12, 1, 0, 1, 2, 0, 2, 3, 0, 6, 7, 0, 7, 8, 0, 8, 9, 0, 9, 10, 0, 10, 11, 0, 11, 12, 0, 3, 4, 0, 4, 5, 0, 5, 6};
	Vector3[] bulletVerts = { new Vector3(-0.1f, -0.5f, 0f), new Vector3(-0.1f, 0.5f, 0f), new Vector3(0.1f, -0.5f, 0f), new Vector3(0.1f, 0.5f, 0f) };
	int[] bulletTris = new int[] {0, 1, 2, 1, 3, 2};
	int gameState = 0;
	float gameStateTimer = 0f;
	int XToGridJ(float x) { return Mathf.RoundToInt((x + 4.5f) / 0.3f); }
	int YToGridI(float y) { return Mathf.RoundToInt((4.5f - y) / 0.3f); }
	float GridJToX(int j) { return -4.5f + (j * 0.3f); }
	float GridIToY(int i) { return 4.5f - (i * 0.3f); }
	Vector2Int PosCell(Vector3 pos) { return new Vector2Int(XToGridJ(pos.x), YToGridI(pos.y)); }
	Vector3 CellPos(Vector2Int cell) { return new Vector3(GridJToX(cell.x), GridIToY(cell.y), 0f); }
	Vector3 SnapPos(Vector3 pos) { return CellPos(PosCell(pos)); }
	int CellLayer(Vector2Int cell) { return grid[cell.x, cell.y]; }
	Vector2 TouchJoy(int t) { return (Input.GetTouch(t).position - new Vector2(Screen.width-(Screen.height/4f), Screen.height/4f)) / (Screen.height/4f); }
	Vector2 KeyJoy { get { return (Input.GetKey(KeyCode.LeftArrow)?-Vector2.right:Vector2.zero)+(Input.GetKey(KeyCode.RightArrow)?Vector2.right:Vector2.zero)+(Input.GetKey(KeyCode.DownArrow)?-Vector2.up:Vector2.zero)+(Input.GetKey(KeyCode.UpArrow)?Vector2.up:Vector2.zero); } }
	Vector2 Joystick { get { for(int t=0; t<Input.touchCount; t++) {if(TouchJoy(t).magnitude<2f) return TouchJoy(t);} return KeyJoy; } }
	bool Fire { get { for(int t=0; t<Input.touchCount; t++) {if(TouchJoy(t).x<-2f) return true; } return Input.GetKeyDown(KeyCode.LeftShift); } }
	void Start() {
		gameObject.GetComponent<Camera>().backgroundColor = Color.black;
		allObjects.Add( CreateMeshObject("Player", sphereVerts, sphereTris, 15, 29, 0.3f, 4, Color.red) );
		for (int i=0; i<20; i++) {
			playerLives.Add( CreateMeshObject("Life", sphereVerts, sphereTris, i, 0, 0.3f, 0, Color.red, i<=lives) );
			playerLives[i].transform.parent = gameObject.transform;
		}
		for(int i=0; i<100; i++)
			allObjects.Add( CreateMeshObject("Mushroom", sphereVerts, sphereTris, (int)Random.Range(0, 30), (int)Random.Range(2, 28), 0.3f, 1, Color.green) );
		for(int i=0; i<10; i++)
			centipede.Add( CreateMeshObject("Centipede", sphereVerts, sphereTris, i, 1, 0.3f, i==0?10:i<9?11:13, Color.yellow, true, new Vector2Int(i+1,1)) );
		uiObjects.Add( new GameObject("UICanvas") );
		uiObjects[0].AddComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
		for(int i=0; i<2; i++) {
			uiObjects.Add( new GameObject("UIText") );
			uiObjects[1+i].transform.parent = uiObjects[0].transform;
			uiObjects[1+i].AddComponent<Text>().font = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;
			uiObjects[1+i].GetComponent<Text>().fontSize = Screen.width/30;
			uiObjects[1+i].GetComponent<Text>().alignment = TextAnchor.MiddleCenter;
			uiObjects[1+i].GetComponent<RectTransform>().localPosition = i == 0 ? new Vector3(-Screen.width/2.4f, Screen.height/2.2f, 0f) : i == 1 ? new Vector3(0f, Screen.height/2.2f, 0f) : new Vector3(0f, 0f, 0f);
			uiObjects[1+i].GetComponent<RectTransform>().sizeDelta = new Vector2(800f, 800f);
		}
	}
	GameObject SetupObject(GameObject go, int x, int y, float sX, float sY, float sZ, int layer, Color color, bool active, Vector2Int targetCell) {
		go.SetActive(active);
		go.layer = layer;
		go.transform.position = new Vector3(GridJToX(x), GridIToY(y), 0f);
		go.transform.localScale = new Vector3(sX, sY, sZ);
		go.GetComponent<Renderer>().material.color = color;
		go.AddComponent<GameData>().targetCell = targetCell;
		grid[x,y] = layer==1?layer:0;
		return go;
	}
	GameObject CreateMeshObject(string label, Vector3[] verts, int[] tris, int x, int y, float size, int layer, Color color, bool active = true, Vector2Int targetCell=new Vector2Int()) {
		GameObject go = GameObject.CreatePrimitive(PrimitiveType.Sphere);	// Adds MeshRenderer, MeshFilter and SphereCollider in one line!
		go.name = label;
		go.GetComponent<MeshRenderer>().material = new Material(Shader.Find("Sprites/Default"));
		go.GetComponent<MeshFilter>().mesh = new Mesh();
		go.GetComponent<MeshFilter>().mesh.vertices = verts;
		go.GetComponent<MeshFilter>().mesh.triangles = tris;
		return SetupObject(go, x, y, size, size, size, layer, color, active, targetCell);
	}
	GameObject CreateVectorObject(string label, Vector3[] shape, int x, int y, float sX, float sY, float sZ, int layer, Color color, bool active = true, Vector2Int targetCell=new Vector2Int()) {
		GameObject go = new GameObject(label);
		go.AddComponent<SphereCollider>();
		LineRenderer line = go.AddComponent<LineRenderer>();
		line.useWorldSpace = false;
		line.widthMultiplier = 0.06f;
		line.material = new Material(Shader.Find("Sprites/Default"));
		line.positionCount = shape.Length;
		line.SetPositions(shape);
		return SetupObject(go, x, y, sX, sY, sZ, layer, color, active, targetCell);
	}
	void KillPlayer() {
		//Explode(player, 50);
		playerLives[lives].SetActive(false);
		gameState = 1;								// Player lost a life
		gameStateTimer = Time.unscaledTime + 3f;
		if (--lives < 0) {
			gameState = 3;							// Player dead = game over
		}
	}
	void Score(int add) {
		lives = (score / 10000) < ((score + add) / 10000) ? ((lives < (playerLives.Count - 1)) ? lives + 1 : lives) : lives;
		playerLives[lives].SetActive(true);
		score += add;
		PlayerPrefs.SetInt("CentipedeHighScore", score > PlayerPrefs.GetInt("CentipedeHighScore") ? score : PlayerPrefs.GetInt("CentipedeHighScore"));
	}
    void Update() {
		uiObjects[1].GetComponent<Text>().text = string.Format("{0:00000}", score);
		uiObjects[2].GetComponent<Text>().text = string.Format("{0:00000}", PlayerPrefs.GetInt("CentipedeHighScore"));
		if(gameState > 0) {
			if( (gameState == 1) && (Time.unscaledTime > gameStateTimer) ) {
				gameState = 0;
			}
			if( (gameState >= 2) && ((Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))) ) {
				level++;
				if(gameState == 3) { score = 0; lives = 2; level = 1; }					// New game
				UnityEngine.SceneManagement.SceneManager.LoadScene("Centipede");		// Reload scene for new level
			}
			return;
		}
		List<GameObject> destroyed = new List<GameObject>(), added = new List<GameObject>();
		if(Fire)
        	allObjects.Add( CreateMeshObject("Bullet", bulletVerts, bulletTris, XToGridJ(allObjects[0].transform.position.x), YToGridI(allObjects[0].transform.position.y)-1, 0.3f, 5, Color.red) );
		foreach(GameObject go in allObjects) {
			switch(go.layer) {
				case 4:																												// Player
					go.transform.position += new Vector3(Joystick.x, Joystick.y, 0f) * Time.deltaTime;
					go.transform.position = new Vector3(Mathf.Clamp(go.transform.position.x, GridJToX(0), GridJToX(grid.GetUpperBound(0))), Mathf.Clamp(go.transform.position.y, GridIToY(grid.GetUpperBound(1)), GridIToY(grid.GetUpperBound(1)-10)), 0f);
				break;
				case 5:																												// Bullet
					go.transform.position += Vector3.up * 5f * Time.deltaTime;
				break;
			}
			Collider[] hits = Physics.OverlapSphere(go.GetComponent<Collider>().bounds.center, go.GetComponent<Collider>().bounds.extents.y, go.layer==5?(1<<1)+(1<<2)+(1<<3)+(1<<10)+(1<<11)+(1<<12)+(1<<13)+(1<<14)+(1<<15):0);
			for(int h=0; (hits != null) && (h < hits.Length); h++) {
				if(hits[h].gameObject.layer <= 2) {																					// Hit a mushroom
					hits[h].gameObject.layer++;
					destroyed.Add(go);
				}
				else if(hits[h].gameObject.layer == 3) {																			// Hit a mushroom
					destroyed.Add(hits[h].gameObject);
					destroyed.Add(go);
					Score(1);
				}
				else if(hits[h].gameObject.layer >= 10) {																			// Hit centipede
					int index = centipede.IndexOf(hits[h].gameObject);
					if(index > 0) {																									// Make prev segment a head
						centipede[index-1].layer = 14;																				// Move down one cell
						centipede[index-1].GetComponent<GameData>().targetCell = PosCell(go.transform.position) - Vector2Int.down;
					}
					centipede.Remove(hits[h].gameObject);
					destroyed.Add(hits[h].gameObject);
					destroyed.Add(go);
					Score(10);
				}
			}
		}
		for(int i=0; i<centipede.Count; i++) {
			GameObject go = centipede[i];
			Vector2Int goCell = PosCell(go.transform.position);
			Vector3 diff = CellPos(go.GetComponent<GameData>().targetCell) - go.transform.position;									// Vector to target cell
			if(diff.magnitude < 0.01f) {																							// Reached target cell?
				go.transform.position = CellPos(goCell);																			// Snap to centre of cell
				if(go.layer <= 11)																									// Tail/body
					go.GetComponent<GameData>().targetCell = PosCell(centipede[i+1].transform.position);							// Follow next segment
				else if(go.layer <= 13)	{																							// Head left/right
					go.GetComponent<GameData>().targetCell = goCell + (go.layer==12?Vector2Int.left:Vector2Int.right);				// Next cell left/right
					if( (XToGridJ(go.transform.position.x)==0) || (XToGridJ(go.transform.position.x)>=grid.GetUpperBound(0)) ||		// Reached edge of screen?
						(CellLayer(go.GetComponent<GameData>().targetCell) == 1) ) {												// Hit mushroom
						go.layer = go.layer==12?14:15;																				// Move down one cell
						go.GetComponent<GameData>().targetCell = goCell - (goCell.y<29?Vector2Int.down:Vector2Int.zero);
					}
				}
				else if(go.layer <= 15) {																							// Head up/down
					go.layer = go.layer==14?13:12;																					// Move Left/right in reverse direction from before moving down
					go.GetComponent<GameData>().targetCell = goCell + (go.layer==12?Vector2Int.left:Vector2Int.right);
				}
			}
			else
				go.transform.position += diff.normalized * 1.0f * Time.deltaTime;													// Move towards target
			Collider[] hits = Physics.OverlapSphere(go.GetComponent<Collider>().bounds.center, go.GetComponent<Collider>().bounds.extents.y, go.layer>=10&&go.layer<=15 ? (1<<4) : 0);
			for(int h=0; (hits != null) && (h < hits.Length); h++) {
				if(hits[h].gameObject.layer == 4) {																					// Hit the Player
					KillPlayer();
				}
			}
		}
		foreach(GameObject dead in destroyed) {
			allObjects.Remove(dead);
			Destroy(dead);
		}
		allObjects.AddRange(added);
	}
}
