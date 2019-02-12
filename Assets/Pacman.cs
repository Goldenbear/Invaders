using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
public class Pacman : MonoBehaviour {
	static int score = 0, lives = 2, level = 1;
	string[] maze = new string[] {	"[------------][------------]","|111111111111||111111111111|","|1[--]1[---]1||1[---]1[--]1|","|8|  |1|   |1||1|   |1|  |8|","|1<-->1<--->1<>1<--->1<-->1|",
									"|11111111111111111111111111|","|1[--]1[]1[------]1[]1[--]1|","|1<-->1||1<--][-->1||1<-->1|","|111111||1111||1111||111111|","<----]1|<--]0||0[-->|1[---->",
									"00000|1|[-->0<>0<--]|1|00000","00000|1||0000400000||1|00000","00000|1||0[--00--]0||1|00000","----->1<>0|000000|0<>1<-----","0000001000|506070|0001000000",
									"-----]1[]0|009000|0[]1[-----","00000|1||0<------>0||1|00000","00000|1||0000300000||1|00000","00000|1||0[------]0||1|00000","[---->1<>0<--][-->0<>1<----]",
									"|111111111111||111111111111|","|1[--]1[---]1||1[---]1[--]1|","|1<-]|1<--->1<>1<--->1|[->1|","|811||1111111211111111||118|","<-]1||1[]1[------]1[]1||1[->",
									"[->1<>1||1<--][-->1||1<>1<-]","|111111||1111||1111||111111|","|1[----><--]1||1[--><----]1|","|1<-------->1<>1<-------->1|","|11111111111111111111111111|","<-------------------------->"};
	string wallChars = "|-[]<>";
	Color[] fruitColors = { new Color(0.5f, 0f, 0f, 1f), new Color(1f, 0.6f, 0f, 1f), new Color(1f, 0f, 0f, 1f), new Color(0f, 1f, 0f, 1f), new Color(0f, 0f, 1f, 1f), new Color(1f, 0.9f, 0f, 1f), new Color(1f, 1f, 0f, 1f) };
	Vector3[] wallV = { new Vector3(0f, -1f, 0f), new Vector3(0f, 1f, 0f) };
	Vector3[] wallH = { new Vector3(-1f, 0f, 0f), new Vector3(1f, 0f, 0f) };
	Vector3[] wallTL = { new Vector3(0f, -1f, 0f), new Vector3(0f, -0.7f, 0f), new Vector3(0.2f, -0.2f, 0f), new Vector3(0.7f, 0f, 0f), new Vector3(1f, 0f, 0f) };
	Vector3[] wallTR = { new Vector3(0f, -1f, 0f), new Vector3(0f, -0.7f, 0f), new Vector3(-0.2f, -0.2f, 0f), new Vector3(-0.7f, 0f, 0f), new Vector3(-1f, 0f, 0f) };
	Vector3[] wallBL = { new Vector3(0f, 1f, 0f), new Vector3(0f, 0.7f, 0f), new Vector3(0.2f, 0.2f, 0f), new Vector3(0.7f, 0f, 0f), new Vector3(1f, 0f, 0f) };
	Vector3[] wallBR = { new Vector3(0f, 1f, 0f), new Vector3(0f, 0.7f, 0f), new Vector3(-0.2f, 0.2f, 0f), new Vector3(-0.7f, 0f, 0f), new Vector3(-1f, 0f, 0f) };
	Vector3[] strawbVerts = { new Vector3(0f, -1f, 0f), new Vector3(-0.3f, -0.9f, 0f), new Vector3(-1f, 0f, 0f), new Vector3(-1f, 0.5f, 0f), new Vector3(-0.5f, 1f, 0f), new Vector3(0f, 0.8f, 0f), new Vector3(0.5f, 1f, 0f), new Vector3(1f, 0.5f, 0f), new Vector3(1f, 0f, 0f), new Vector3(0.3f, -0.9f, 0f) };
	int[] strawbTris = new int[] {0, 1, 2, 0, 2, 3, 0, 3, 4, 0, 4, 5, 0, 5, 6, 0, 6, 7, 0, 7, 8, 0, 8, 9};
	Vector3[] sphereVerts = { new Vector3(0f, 0f, 0f), new Vector3(-0.5f, -0.1f, 0f), new Vector3(-0.5f, 0.1f, 0f), new Vector3(-0.35f, 0.35f, 0f), new Vector3(-0.1f, 0.5f, 0f), new Vector3(0.1f, 0.5f, 0f), new Vector3(0.35f, 0.35f, 0f), new Vector3(0.5f, 0.1f, 0f), new Vector3(0.5f, -0.1f, 0f), new Vector3(0.35f, -0.35f, 0f), new Vector3(0.1f, -0.5f, 0f), new Vector3(-0.1f, -0.5f, 0f), new Vector3(-0.35f, -0.35f, 0f) };
	int[] sphereTris = new int[] {0, 12, 1, 0, 1, 2, 0, 2, 3, 0, 6, 7, 0, 7, 8, 0, 8, 9, 0, 9, 10, 0, 10, 11, 0, 11, 12, 0, 3, 4, 0, 4, 5, 0, 5, 6};
	int[] pacLTris   = new int[] {0,  0, 0, 0, 0, 0, 0, 0, 0, 0, 6, 7, 0, 7, 8, 0, 8, 9, 0, 9, 10, 0, 10, 11, 0, 11, 12, 0, 3, 4, 0, 4, 5, 0, 5, 6};
	int[] pacRTris   = new int[] {0, 12, 1, 0, 1, 2, 0, 2, 3, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 9, 10, 0, 10, 11, 0, 11, 12, 0, 3, 4, 0, 4, 5, 0, 5, 6};
	int[] pacUTris   = new int[] {0, 12, 1, 0, 1, 2, 0, 2, 3, 0, 6, 7, 0, 7, 8, 0, 8, 9, 0, 0,  0, 0,  0,  0, 0,  0,  0, 0, 3, 4, 0, 4, 5, 0, 5, 6};
	int[] pacDTris   = new int[] {0, 12, 1, 0, 1, 2, 0, 2, 3, 0, 6, 7, 0, 7, 8, 0, 8, 9, 0, 9, 10, 0, 10, 11, 0, 11, 12, 0, 0, 0, 0, 0, 0, 0, 0, 0};
	Vector3[] ghostVerts = { new Vector3(0f, 0f, -1f), new Vector3(-0.5f, -0.1f, -1f), new Vector3(-0.5f, 0.1f, -1f), new Vector3(-0.35f, 0.35f, -1f), new Vector3(-0.1f, 0.5f, -1f), new Vector3(0.1f, 0.5f, -1f), new Vector3(0.35f, 0.35f, -1f), new Vector3(0.5f, 0.1f, -1f), new Vector3(0.5f, -0.1f, -1f), new Vector3(0.5f, -0.5f, -1f), new Vector3(0.25f, -0.35f, -1f), new Vector3(0f, -0.5f, -1f), new Vector3(-0.25f, -0.35f, -1f), new Vector3(-0.5f, -0.5f, -1f) };
	int[] ghostTris = new int[] {0, 13, 1, 0, 1, 2, 0, 2, 3, 0, 6, 7, 0, 7, 8, 0, 8, 9, 0, 9, 10, 0, 10, 11, 0, 11, 12, 0, 12, 13, 0, 3, 4, 0, 4, 5, 0, 5, 6};
	int[] ghostEyesTris = new int[] { 0, 3, 4, 0, 5, 6 };
	List<GameObject> pills = new List<GameObject>();
	List<GameObject> fruit = new List<GameObject>();
	GameObject pacman, ghostExit;
	Vector3 pacStart, msgPos;
	Vector3[] ghostStarts = new Vector3[4];
	int pacdir = 0, blueScore = 200, gameState = 0;
	float fruitTime = 0f, blueTime = 0f, pauseTime = 0f;
	GameObject[] ghosts = new GameObject[4];
	GameObject[] playerLives = new GameObject[20];  // Max 20 lives
	GameObject[] uiObjects = new GameObject[10];
	int[] ghostdir = new int[4];
	int[] ghostState = new int[4];
	int[][] ghostSideTry = { new int[3], new int[3], new int[3], new int[3] };     // Ghosts' attempts to move sideways on a particular maze cell. [0] = dir, [1] = i, [2] = j
	int XToMazeJ(float x) { return Mathf.RoundToInt((x + 4.5f) / 0.3f); }
	int YToMazeI(float y) { return Mathf.RoundToInt((4.5f - y) / 0.3f); }
	float MazeJToX(int j) { return -4.5f + (j * 0.3f); }
	float MazeIToY(int i) { return 4.5f - (i * 0.3f); }
	bool NearCellCentre(Vector3 pos) { return (pos - new Vector3(MazeJToX(XToMazeJ(pos.x)), MazeIToY(YToMazeI(pos.y)), 0f)).magnitude < 0.05f; }
	char MazeChar(int i, int j) { return (i >= 0) && (i < maze.Length) && (j >= 0) && (j < maze[0].Length) ? maze[i][j] : (char)0; }
	char MazeChar(int i, int j, int dir) { return (dir == 1) ? MazeChar(i, j - 1) : (dir == 2) ? MazeChar(i, j + 1) : (dir == 3) ? MazeChar(i + 1, j) : MazeChar(i - 1, j); }
	void Start() {
		gameObject.GetComponent<Camera>().backgroundColor = Color.black;
        for(int i=0; i<maze.Length; i++)
			for(int j=0; j<maze[i].Length; j++) {
				switch (maze[i][j]) {
					case '1': pills.Add( CreateMeshObject("Pill", sphereVerts, sphereTris, j, i, 0.1f, 1, new Color(1f, 0.6f, 0.4f, 1f)) ); break;
					case '2': pacman = CreateMeshObject("Pacman", sphereVerts, pacLTris, j, i, 0.4f, 2, Color.yellow); break;
					case '3': msgPos = new Vector3(MazeJToX(j) + 0.15f, MazeIToY(i), 0); break;
					case '4': ghosts[0] = CreateMeshObject("Blinky", ghostVerts, ghostTris, j, i, 0.4f, 4, Color.red); break;
					case '5': ghosts[1] = CreateMeshObject("Inky", ghostVerts, ghostTris, j, i, 0.4f, 5, Color.cyan); break;
					case '6': ghosts[2] = CreateMeshObject("Pinky", ghostVerts, ghostTris, j, i, 0.4f, 6, new Color(1f, 0.8f, 1f, 1f)); break;
					case '7': ghosts[3] = CreateMeshObject("Clyde", ghostVerts, ghostTris, j, i, 0.4f, 7, new Color(1f, 0.6f, 0f, 1f)); break;
					case '8': pills.Add( CreateMeshObject("Power", sphereVerts, sphereTris, j, i, 0.3f, 8, new Color(1f, 0.6f, 0.4f, 1f)) ); break;
					case '9': ghostExit = CreateVectorObject("GhostExit", wallH, j, i, 0.15f, 0.15f, 0.3f, 9, Color.black, true, false); break;
					case '|': CreateVectorObject("WallV", wallV, j, i, 0.15f, 0.15f, 0.3f, 3, Color.blue); break;
					case '-': CreateVectorObject("WallH", wallH, j, i, 0.15f, 0.15f, 0.3f, 3, Color.blue); break;
					case '[': CreateVectorObject("WallTL", wallTL, j, i, 0.15f, 0.15f, 0.3f, 3, Color.blue); break;
					case ']': CreateVectorObject("WallTR", wallTR, j, i, 0.15f, 0.15f, 0.3f, 3, Color.blue); break;
					case '<': CreateVectorObject("WallBL", wallBL, j, i, 0.15f, 0.15f, 0.3f, 3, Color.blue); break;
					case '>': CreateVectorObject("WallBR", wallBR, j, i, 0.15f, 0.15f, 0.3f, 3, Color.blue); break;
				}
			}
		pacStart = pacman.transform.position;
		for(int i=0; i<2; i++)
			fruit.Add( CreateMeshObject("Fruit", strawbVerts, strawbTris, XToMazeJ(msgPos.x-0.15f), YToMazeI(msgPos.y), 0.2f, 10, fruitColors[System.Math.Min((level-1)/2, fruitColors.Length-1)], false, true, 0.15f) );
		for(int g=0; g<ghosts.Length; g++) {
			ghostStarts[g] = ghosts[g].transform.position;
			ghostdir[g] = 1 + (int)(Random.value * 3.9999f);
		}
		for (int i = 0; i < playerLives.Length; i++)
			playerLives[i] = CreateMeshObject("Life", sphereVerts, pacLTris, 3+(i*2), maze.Length, 0.4f, 2, Color.yellow, (i <= lives));
		uiObjects[0] = new GameObject("UICanvas");
		uiObjects[0].AddComponent<Canvas>().renderMode = RenderMode.WorldSpace;
		uiObjects[0].GetComponent<RectTransform>().pivot = new Vector2(0f, 1f);						// Position = top-left
		uiObjects[0].GetComponent<RectTransform>().localPosition = new Vector3(-4.5f, 4.8f, 0);
		uiObjects[0].GetComponent<RectTransform>().sizeDelta = new Vector2(1000f, 1000f);
		uiObjects[0].GetComponent<RectTransform>().localScale = new Vector3(0.01f, 0.01f, 1f);
		for(int i=0; i<4; i++) {
			uiObjects[1+i] = new GameObject("UIText");
			uiObjects[1+i].transform.parent = uiObjects[0].transform;
			uiObjects[1+i].AddComponent<Text>().font = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;
			uiObjects[1+i].GetComponent<Text>().fontSize = 30;
			uiObjects[1+i].GetComponent<Text>().alignment = TextAnchor.MiddleCenter;
			uiObjects[1+i].GetComponent<RectTransform>().localPosition = i <= 1 ? new Vector3(100f+i*300f, 0f, 0) : new Vector3((msgPos.x+4.5f)*100f, (msgPos.y-4.8f)*100f, 0f);
			uiObjects[1+i].GetComponent<RectTransform>().localScale = new Vector3(1f, 1f, 1f);
			uiObjects[1+i].GetComponent<RectTransform>().sizeDelta = new Vector2(200f, 35f);
		}
    }
	GameObject CreateMeshObject(string label, Vector3[] verts, int[] tris, int x, int y, float size, int layer, Color color, bool active = true, bool visible = true, float xOffset=0f, float yOffset=0f, float zOffset=0f) {
		GameObject go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
		go.name = label;
		go.SetActive(active);
		go.layer = layer;
		go.transform.position = new Vector3(MazeJToX(x)+xOffset, MazeIToY(y)+yOffset, zOffset);
		go.transform.localScale = new Vector3(size, size, size);
		go.GetComponent<MeshRenderer>().material = new Material(Shader.Find("Sprites/Default"));
		go.GetComponent<MeshRenderer>().materials[0].color = color;
		go.GetComponent<MeshRenderer>().enabled = visible;
		go.GetComponent<MeshFilter>().mesh = new Mesh();
		go.GetComponent<MeshFilter>().mesh.vertices = verts;
		go.GetComponent<MeshFilter>().mesh.triangles = tris;
		return go;
	}
	GameObject CreateVectorObject(string label, Vector3[] shape, int x, int y, float sX, float sY, float sZ, int layer, Color color, bool active = true, bool visible = true, float xOffset=0f, float yOffset=0f, float zOffset=0f) {
		GameObject go = new GameObject(label);
		go.SetActive(active);
		go.layer = layer;
		go.transform.position = new Vector3(MazeJToX(x)+xOffset, MazeIToY(y)+yOffset, zOffset);
		go.transform.localScale = new Vector3(sX, sY, sZ);
		go.AddComponent<BoxCollider>();
		LineRenderer line = go.AddComponent<LineRenderer>();
		line.useWorldSpace = false;
		line.widthMultiplier = 0.06f;
		line.material = new Material(Shader.Find("Sprites/Default"));
		line.material.color = color;
		line.enabled = visible;
		line.positionCount = shape.Length;
		line.SetPositions(shape);
		return go;
	}
	void KillPlayer() {
		playerLives[lives].SetActive(false);
		pacman.transform.position = pacStart;
		for(int g=0; g<ghosts.Length; g++)
			ghosts[g].transform.position = ghostStarts[g];
		pacdir = 0;
		gameState = 0;
		if (--lives < 0) {
			pacman.SetActive(false);
			gameState = 2;                            // Player dead = game over
		}
	}
	void Score(int add, GameObject go=null) {
		lives = (score / 10000) < ((score + add) / 10000) ? ((lives < (playerLives.Length - 1)) ? lives + 1 : lives) : lives;
		playerLives[lives].SetActive(true);
		score += add;
		PlayerPrefs.SetInt("HighScore", score > PlayerPrefs.GetInt("HighScore") ? score : PlayerPrefs.GetInt("HighScore"));
		uiObjects[4].GetComponent<RectTransform>().localPosition = go != null ? new Vector3((go.transform.position.x+4.5f)*100f, (go.transform.position.y-4.8f)*100f, 0f) : uiObjects[4].GetComponent<RectTransform>().localPosition;
		uiObjects[4].GetComponent<Text>().text = go != null ? string.Format("{0}", add) : uiObjects[4].GetComponent<Text>().text;
	}
	int ChangeDirectionTo(GameObject objA, GameObject objB, int currDir, float randomness) {
		Vector3 diff = objB.transform.position - objA.transform.position;
		if(Random.value < randomness)
			return ((currDir == 1) || (currDir == 2)) ? ((Random.value < 0.5f) ? 3 : 4) : ((Random.value < 0.5f) ? 1 : 2);
		return ((currDir == 1) || (currDir == 2)) ? ((diff.y < 0f) ? 3 : 4) : ((diff.x < 0f) ? 1 : 2);
	}
	int DirectionTo(GameObject objA, GameObject objB) { 
		Vector3 diff = objB.transform.position - objA.transform.position;
		return Mathf.Abs(diff.y) > Mathf.Abs(diff.x) ? ((diff.y < 0f) ? 3 : 4) : ((diff.x < 0f) ? 1 : 2);
	}
	void Update() {
		uiObjects[1].GetComponent<Text>().text = string.Format("{0:00000}", score);
		uiObjects[2].GetComponent<Text>().text = string.Format("{0:00000}", PlayerPrefs.GetInt("HighScore"));
		uiObjects[3].GetComponent<Text>().text = gameState == 0 ? "READY!" : gameState == 2 ? "GAME OVER" : "";
		if( (gameState == 0) || (gameState == 2) ) {
			fruitTime = Time.time + 15f;
			gameState = (gameState == 0) && Input.anyKeyDown ? 1 : gameState;
			if ((gameState == 2) && Input.GetKeyDown(KeyCode.Space)) {
				score = 0; lives = 2; level = 1;
				UnityEngine.SceneManagement.SceneManager.LoadScene("Pacman");      // Reload scene and reset score, lives & level for a new game
			}
			return;
		}
		Time.timeScale = Time.realtimeSinceStartup > pauseTime ? 1f : 0f;
		uiObjects[4].GetComponent<Text>().text = (pacdir == 0) ? "" : uiObjects[4].GetComponent<Text>().text;
		if(pacdir > 0)
			pacman.GetComponent<MeshFilter>().mesh.triangles = ((Time.time % 0.2f) < 0.1f) ? (pacdir == 1 ? pacLTris : pacdir == 2 ? pacRTris : pacdir == 3 ? pacUTris : pacDTris) : sphereTris;
		int trydir = Input.GetKey(KeyCode.LeftArrow) ? 1 : Input.GetKey(KeyCode.RightArrow) ? 2 : Input.GetKey(KeyCode.DownArrow) ? 3 : Input.GetKey(KeyCode.UpArrow) ? 4 : pacdir;
		pacdir = NearCellCentre(pacman.transform.position) && (wallChars.IndexOf( MazeChar(YToMazeI(pacman.transform.position.y), XToMazeJ(pacman.transform.position.x), trydir) ) == -1) ? trydir : pacdir;
		float newPacX = pacman.transform.position.x + 1.5f * Time.deltaTime * (pacdir == 1 ? -1f : pacdir == 2 ? 1f : 0f);
		float newPacY = pacman.transform.position.y + 1.5f * Time.deltaTime * (pacdir == 3 ? -1f : pacdir == 4 ? 1f : 0f);
		newPacX = newPacX < MazeJToX(0) ? MazeJToX(maze[0].Length - 1) : newPacX > MazeJToX(maze[0].Length - 1) ? MazeJToX(0) : newPacX;	// Wrap around left-right
		pacman.transform.position = new Vector3((pacdir == 0) || (pacdir >= 3) ? MazeJToX(XToMazeJ(newPacX)) : newPacX, pacdir <= 2 ? MazeIToY(YToMazeI(newPacY)) : newPacY, 0);
		Collider[] hits = Physics.OverlapBox(pacman.GetComponent<Collider>().bounds.center, pacman.GetComponent<Collider>().bounds.extents, pacman.transform.rotation, (1<<1)+(1<<3)+(1<<4)+(1<<5)+(1<<6)+(1<<7)+(1<<8)+(1<<9)+(1<<10));
		for(int h=0; (hits != null) && (h < hits.Length); h++) {
			if(hits[h].gameObject.layer == 1) {												// Pill
				hits[h].gameObject.SetActive(false);
				pills.Remove(hits[h].gameObject);
				Score(10);
			}
			else if( (hits[h].gameObject.layer == 3) || (hits[h].gameObject.layer == 9) ) {	// Wall
				pacdir = 0;
				pacman.transform.position = new Vector3(MazeJToX(XToMazeJ(newPacX)), MazeIToY(YToMazeI(newPacY)), 0);
			}
			else if( (hits[h].gameObject.layer >= 4) && (hits[h].gameObject.layer <= 7) ) {	// Ghost
				if(ghostState[hits[h].gameObject.layer-4] == 0)
					KillPlayer();
				else if(ghostState[hits[h].gameObject.layer-4] == 1) {
					Score(blueScore, hits[h].gameObject);
					blueScore *= 2;
					ghostState[hits[h].gameObject.layer-4] = 2;
					pauseTime = Time.realtimeSinceStartup + 0.5f;
				}
			}
			else if(hits[h].gameObject.layer == 8) {										// Power pill
				hits[h].gameObject.SetActive(false);
				pills.Remove(hits[h].gameObject);
				Score(50);
				blueTime = Time.time + (level == 1 ? 6f : level == 2 ? 5f : level == 3 ? 4f : level == 4 ? 3f : level <= 8 ? 2f : level <= 16 ? 1f : 0f);
				blueScore = 200;
				for (int g=0; g<ghosts.Length; g++)
					ghostState[g] = 1;
			}
			else if(hits[h].gameObject.layer == 10) {										// Fruit
				hits[h].gameObject.SetActive(false);
				fruit.Remove(hits[h].gameObject);
				Score( level == 1 ? 100 : level == 2 ? 300 : level <= 4 ? 500 : level <= 6 ? 700 : level <= 8 ? 1000 : level <= 10 ? 2000 : level <= 12 ? 3000 : 5000, hits[h].gameObject );
				fruitTime = Time.time + 15f;
			}
		}
		for(int g=0; g<ghosts.Length; g++) {
			ghosts[g].GetComponent<MeshFilter>().mesh.triangles = (ghostState[g] == 2) ? ghostEyesTris : ghostTris;
			float newX = ghosts[g].transform.position.x + ((ghostState[g] == 2) ? 3f : 1f) * Time.deltaTime * (ghostdir[g] == 1 ? -1f : ghostdir[g] == 2 ? 1f : 0f);
			float newY = ghosts[g].transform.position.y + ((ghostState[g] == 2) ? 3f : 1f) * Time.deltaTime * (ghostdir[g] == 3 ? -1f : ghostdir[g] == 4 ? 1f : 0f);
			newX = newX < MazeJToX(0) ? MazeJToX(maze[0].Length - 1) : newX > MazeJToX(maze[0].Length - 1) ? MazeJToX(0) : newX;    // Wrap around left-right
			ghosts[g].transform.position = new Vector3(ghostdir[g] >= 3 ? MazeJToX(XToMazeJ(newX)) : newX, ghostdir[g] <= 2 ? MazeIToY(YToMazeI(newY)) : newY, 0);
			ghosts[g].GetComponent<MeshRenderer>().materials[0].color = (ghostState[g] == 1) ? ( (Time.time < (blueTime-2.5f)) || ((Time.time % 0.5f) < 0.25f) ) ? Color.blue : Color.white : ghostState[g] == 2 ? Color.white : g == 0 ? Color.red : g == 1 ? Color.cyan : g == 2 ? new Color(1f, 0.8f, 1f, 1f) : new Color(1f, 0.6f, 0f, 1f);
			ghostState[g] = Time.time > blueTime ? 0 : ghostState[g];
			if (NearCellCentre(ghosts[g].transform.position) && ((YToMazeI(newY) != ghostSideTry[g][1]) || (XToMazeJ(newX) != ghostSideTry[g][2]))) { // if not attempted sideways move this cell then try it
				ghostSideTry[g][0] = ChangeDirectionTo(ghosts[g], (Time.time > blueTime) ? pacman : ghostExit, ghostdir[g], (ghostState[g] == 2) ? 0.25f : 0.25f + (g * 0.25f));
				ghostSideTry[g][1] = YToMazeI(newY);
				ghostSideTry[g][2] = XToMazeJ(newX);
				if((ghostSideTry[g][0] == DirectionTo(ghosts[g], (Time.time > blueTime) ? pacman : ghostExit)) && (wallChars.IndexOf( MazeChar(ghostSideTry[g][1], ghostSideTry[g][2], ghostSideTry[g][0]) ) == -1) )
					ghostdir[g] = ghostSideTry[g][0];
			}
			Collider[] ghostHits = Physics.OverlapBox(ghosts[g].GetComponent<Collider>().bounds.center, ghosts[g].GetComponent<Collider>().bounds.extents, ghosts[g].transform.rotation, (1<<3)+(1<<9));
			for (int h = 0; (ghostHits != null) && (h < ghostHits.Length); h++) {
				ghostState[g] = (ghostHits[h].gameObject.layer == 9) ? 0 : ghostState[g];
				ghostdir[g] = (ghostHits[h].gameObject.layer == 9) ? 4 : ChangeDirectionTo(ghosts[g], (Time.time > blueTime) ? pacman : ghostExit, ghostdir[g], (ghostState[g] == 2) ? 0.25f : 0.25f + (g * 0.25f));
			}
		}
		if(Time.time > fruitTime)
			if(fruit.Count > 0)
				fruit[0].gameObject.SetActive(true);		// Activate next fruit if any remaining
		if (pills.Count == 0) {
			level++;
			UnityEngine.SceneManagement.SceneManager.LoadScene("Pacman");      // Reload scene for a new level
		}
    }
}