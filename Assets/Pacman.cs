using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
public class Pacman : MonoBehaviour {
	static int score = 0;
	static int lives = 2;
	static int level = 1;
	string[] maze = new string[] {	"[------------][------------]","|111111111111||111111111111|","|1[--]1[---]1||1[---]1[--]1|","|8|  |1|   |1||1|   |1|  |8|","|1<-->1<--->1<>1<--->1<-->1|",
									"|11111111111111111111111111|","|1[--]1[]1[------]1[]1[--]1|","|1<-->1||1<--][-->1||1<-->1|","|111111||1111||1111||111111|","<----]1|<--]0||0[-->|1[---->",
									"00000|1|[-->0<>0<--]|1|00000","00000|1||0000400000||1|00000","00000|1||0[--00--]0||1|00000","----->1<>0|000000|0<>1<-----","0000001000|506070|0001000000",
									"-----]1[]0|009000|0[]1[-----","00000|1||0<------>0||1|00000","00000|1||0000A00000||1|00000","00000|1||0[------]0||1|00000","[---->1<>0<--][-->0<>1<----]",
									"|111111111111||111111111111|","|1[--]1[---]1||1[---]1[--]1|","|1<-]|1<--->1<>1<--->1|[->1|","|811||1111111211111111||118|","<-]1||1[]1[------]1[]1||1[->",
									"[->1<>1||1<--][-->1||1<>1<-]","|111111||1111||1111||111111|","|1[----><--]1||1[--><----]1|","|1<-------->1<>1<-------->1|","|11111111111111111111111111|","<-------------------------->"};
	string wallChars = "3|-[]<>";
	Vector3[] wallV = { new Vector3(0f, -1f, 0f), new Vector3(0f, 1f, 0f) };
	Vector3[] wallH = { new Vector3(-1f, 0f, 0f), new Vector3(1f, 0f, 0f) };
	Vector3[] wallTL = { new Vector3(0f, -1f, 0f), new Vector3(0f, -0.7f, 0f), new Vector3(0.2f, -0.2f, 0f), new Vector3(0.7f, 0f, 0f), new Vector3(1f, 0f, 0f) };
	Vector3[] wallTR = { new Vector3(0f, -1f, 0f), new Vector3(0f, -0.7f, 0f), new Vector3(-0.2f, -0.2f, 0f), new Vector3(-0.7f, 0f, 0f), new Vector3(-1f, 0f, 0f) };
	Vector3[] wallBL = { new Vector3(0f, 1f, 0f), new Vector3(0f, 0.7f, 0f), new Vector3(0.2f, 0.2f, 0f), new Vector3(0.7f, 0f, 0f), new Vector3(1f, 0f, 0f) };
	Vector3[] wallBR = { new Vector3(0f, 1f, 0f), new Vector3(0f, 0.7f, 0f), new Vector3(-0.2f, 0.2f, 0f), new Vector3(-0.7f, 0f, 0f), new Vector3(-1f, 0f, 0f) };
	Vector3[] strawb = { new Vector3(0f, -1f, 0f), new Vector3(-1f, 0.5f, 0f), new Vector3(-0.5f, 1f, 0f), new Vector3(0f, 0.8f, 0f), new Vector3(0.5f, 1f, 0f), new Vector3(1f, 0.5f, 0f), new Vector3(0f, -1f, 0f) };
	List<GameObject> pills = new List<GameObject>();
	GameObject pacman;
	Vector3 pacStart;
	int pacdir = 0;
	GameObject[] ghosts = new GameObject[4];
	Vector3[] ghostStarts = new Vector3[4];
	int[] ghostdir = new int[4];
	int[] ghostState = new int[4];
	int[][] sideTry = { new int[3], new int[3], new int[3], new int[3] };     // An attempt to move sideways on a particular maze cell. [0] = dir, [1] = i, [2] = j
	GameObject ghostExit;
	List<GameObject> fruit = new List<GameObject>();
	float fruitTime = 0f;
	GameObject[] playerLives = new GameObject[20];  // Max 20 lives
	GameObject[] uiObjects = new GameObject[10];
	Vector3 msgPos;
	float blueTime = 0f;
	int blueScore = 200;
	int gameState = 0;
	int XToMazeJ(float x) { return Mathf.RoundToInt((x + 4.5f) / 0.3f); }
	int YToMazeI(float y) { return Mathf.RoundToInt((4.5f - y) / 0.3f); }
	float MazeJToX(int j) { return -4.5f + (j * 0.3f); }
	float MazeIToY(int i) { return 4.5f - (i * 0.3f); }
	bool NearCellCentre(Vector3 pos) { return (pos - new Vector3(MazeJToX(XToMazeJ(pos.x)), MazeIToY(YToMazeI(pos.y)), 0f)).magnitude < 0.05f; }
	char MazeChar(int i, int j) { return (i >= 0) && (i < maze.Length) && (j >= 0) && (j < maze[0].Length) ? maze[i][j] : (char)0; }
	char MazeChar(int i, int j, int dir) { return (dir == 1) ? MazeChar(i, j - 1) : (dir == 2) ? MazeChar(i, j + 1) : (dir == 3) ? MazeChar(i + 1, j) : MazeChar(i - 1, j); }
	void Start() {
		gameObject.GetComponent<Camera>().backgroundColor = Color.black;
		new GameObject("Light").AddComponent<Light>().type = LightType.Directional;
        for(int i=0; i<maze.Length; i++)
			for(int j=0; j<maze[i].Length; j++) {
				if(maze[i][j] == '1')
					pills.Add( CreateMazeObject("Pill", PrimitiveType.Cube, j, i, 0.1f, 1, new Color(1f, 0.6f, 0.4f, 1f)) );
				else if(maze[i][j] == '2')
					pacman = CreateMazeObject("Pacman", PrimitiveType.Sphere, j, i, 0.4f, 2, Color.yellow);
				else if(maze[i][j] == '3')
					CreateMazeObject("Wall", PrimitiveType.Cube, j, i, 0.25f, 3, Color.blue);
				else if(maze[i][j] == '4')
					ghosts[0] = CreateMazeObject("Blinky", PrimitiveType.Sphere, j, i, 0.4f, 4, Color.red);
				else if(maze[i][j] == '5')
					ghosts[1] = CreateMazeObject("Inky", PrimitiveType.Sphere, j, i, 0.4f, 5, Color.cyan);
				else if(maze[i][j] == '6')
					ghosts[2] = CreateMazeObject("Pinky", PrimitiveType.Sphere, j, i, 0.4f, 6, new Color(1f, 0.8f, 0.8f, 1f));
				else if(maze[i][j] == '7')
					ghosts[3] = CreateMazeObject("Clyde", PrimitiveType.Sphere, j, i, 0.4f, 7, new Color(1f, 0.6f, 0f, 1f));
				else if(maze[i][j] == '8')
					pills.Add( CreateMazeObject("Power", PrimitiveType.Sphere, j, i, 0.3f, 8, new Color(1f, 0.6f, 0.4f, 1f)) );
				else if(maze[i][j] == '9')
					ghostExit = CreateMazeObject("GhostExit", PrimitiveType.Cube, j, i, 0.3f, 9, Color.black, false);
				else if(maze[i][j] == 'A')
					msgPos = new Vector3(MazeJToX(j) + 0.15f, MazeIToY(i), 0);
				else if(maze[i][j] == '|')
					CreateVectorObject("WallV", wallV, j, i, 0.15f, 0.15f, 0.3f, 3, Color.blue);
				else if(maze[i][j] == '-')
					CreateVectorObject("WallH", wallH, j, i, 0.15f, 0.15f, 0.3f, 3, Color.blue);
				else if(maze[i][j] == '[')
					CreateVectorObject("WallTL", wallTL, j, i, 0.15f, 0.15f, 0.3f, 3, Color.blue);
				else if(maze[i][j] == ']')
					CreateVectorObject("WallTR", wallTR, j, i, 0.15f, 0.15f, 0.3f, 3, Color.blue);
				else if(maze[i][j] == '<')
					CreateVectorObject("WallBL", wallBL, j, i, 0.15f, 0.15f, 0.3f, 3, Color.blue);
				else if(maze[i][j] == '>')
					CreateVectorObject("WallBR", wallBR, j, i, 0.15f, 0.15f, 0.3f, 3, Color.blue);
			}
		pacStart = pacman.transform.position;
		for(int i=0; i<2; i++)
			fruit.Add( CreateVectorObject("Fruit", strawb, XToMazeJ(msgPos.x-0.15f), YToMazeI(msgPos.y), 0.15f, 0.15f, 0.3f, 10, Color.red, false, 0.15f) );
		for(int g=0; g<ghosts.Length; g++) {
			ghostStarts[g] = ghosts[g].transform.position;
			ghostdir[g] = 1 + (int)(Random.value * 3.9999f);
		}
		for (int i = 0; i < playerLives.Length; i++)
			playerLives[i] = CreateMazeObject("Life", PrimitiveType.Sphere, 3+i, maze.Length, 0.3f, 2, Color.yellow, (i <= lives));
		uiObjects[0] = new GameObject("UICanvas");
		uiObjects[0].AddComponent<Canvas>();
		uiObjects[0].GetComponent<Canvas>().renderMode = RenderMode.WorldSpace;
		uiObjects[0].GetComponent<RectTransform>().pivot = new Vector2(0f, 1f);						// Position = top-left
		uiObjects[0].GetComponent<RectTransform>().localPosition = new Vector3(-4.5f, 4.8f, 0);
		uiObjects[0].GetComponent<RectTransform>().sizeDelta = new Vector2(1000f, 1000f);
		uiObjects[0].GetComponent<RectTransform>().localScale = new Vector3(0.01f, 0.01f, 1f);
		for(int i=0; i<2; i++) {
			uiObjects[1+i] = new GameObject("UIText");
			uiObjects[1+i].AddComponent<Text>();
			uiObjects[1+i].transform.parent = uiObjects[0].transform;
			uiObjects[1+i].GetComponent<Text>().font = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;
			uiObjects[1+i].GetComponent<Text>().fontSize = 30;
			uiObjects[1+i].GetComponent<Text>().alignment = TextAnchor.MiddleCenter;
			uiObjects[1+i].GetComponent<RectTransform>().localPosition = i == 0 ? new Vector3(100f, 0f, 0) : new Vector3((msgPos.x+4.5f)*100f, (msgPos.y-4.8f)*100f, 0f);
			uiObjects[1+i].GetComponent<RectTransform>().localScale = new Vector3(1f, 1f, 1f);
			uiObjects[1+i].GetComponent<RectTransform>().sizeDelta = new Vector2(200f, 35f);
		}
    }
	GameObject CreateMazeObject(string label, PrimitiveType shape, int x, int y, float size, int layer, Color color, bool visible = true, float xOffset=0f, float yOffset=0f) {
		GameObject go = GameObject.CreatePrimitive(shape);
		go.name = label;
		go.transform.position = new Vector3(MazeJToX(x)+xOffset, MazeIToY(y)+yOffset, 0);
		go.transform.localScale = new Vector3(size, size, size);
		go.layer = layer;
		go.GetComponent<MeshRenderer>().materials[0].color = color;
		go.GetComponent<MeshRenderer>().enabled = visible;
		return go;
	}
	GameObject CreateVectorObject(string label, Vector3[] shape, int x, int y, float sX, float sY, float sZ, int layer, Color color, bool active = true, float xOffset=0f, float yOffset=0f) {
		GameObject go = new GameObject(label);
		go.SetActive(active);
		go.layer = layer;
		go.transform.position = new Vector3(MazeJToX(x)+xOffset, MazeIToY(y)+yOffset, 0);
		go.transform.localScale = new Vector3(sX, sY, sZ);
		go.AddComponent<BoxCollider>();
		LineRenderer line = go.AddComponent<LineRenderer>();
		line.useWorldSpace = false;
		line.widthMultiplier = 0.06f;
		line.material = new Material(Shader.Find("Sprites/Default"));
		line.material.color = color;
		line.positionCount = shape.Length;
		line.SetPositions(shape);
		return go;
	}
	void KillPlayer() {
		playerLives[lives].GetComponent<MeshRenderer>().enabled = false;
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
	void Score(int add) {
		lives = (score / 10000) < ((score + add) / 10000) ? ((lives < (playerLives.Length - 1)) ? lives + 1 : lives) : lives;
		playerLives[lives].GetComponent<MeshRenderer>().enabled = true;
		score += add;
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
		uiObjects[2].GetComponent<Text>().text = gameState == 0 ? "READY!" : gameState == 2 ? "GAME OVER" : "";
		if( (gameState == 0) || (gameState == 2) ) {
			fruitTime = Time.time + 15f;
			gameState = (gameState == 0) && Input.GetKeyDown(KeyCode.Space) ? 1 : gameState;
			if ((gameState == 2) && Input.GetKeyDown(KeyCode.Space)) {
				score = 0; lives = 2; level = 1;
				UnityEngine.SceneManagement.SceneManager.LoadScene("Pacman");      // Reload scene and reset score, lives & level for a new game
			}
			return;
		}
		int trydir = Input.GetKey(KeyCode.LeftArrow) ? 1 : Input.GetKey(KeyCode.RightArrow) ? 2 : Input.GetKey(KeyCode.DownArrow) ? 3 : Input.GetKey(KeyCode.UpArrow) ? 4 : pacdir;
		pacdir = NearCellCentre(pacman.transform.position) && (wallChars.IndexOf( MazeChar(YToMazeI(pacman.transform.position.y), XToMazeJ(pacman.transform.position.x), trydir) ) == -1) ? trydir : pacdir;
		float newPacX = pacman.transform.position.x + 1.5f * Time.deltaTime * (pacdir == 1 ? -1f : pacdir == 2 ? 1f : 0f);
		float newPacY = pacman.transform.position.y + 1.5f * Time.deltaTime * (pacdir == 3 ? -1f : pacdir == 4 ? 1f : 0f);
		newPacX = newPacX < MazeJToX(0) ? MazeJToX(maze[0].Length - 1) : newPacX > MazeJToX(maze[0].Length - 1) ? MazeJToX(0) : newPacX;	// Wrap around left-right
		pacman.transform.position = new Vector3((pacdir == 0) || (pacdir >= 3) ? MazeJToX(XToMazeJ(newPacX)) : newPacX, pacdir <= 2 ? MazeIToY(YToMazeI(newPacY)) : newPacY, 0);
		Collider[] hits = Physics.OverlapBox(pacman.GetComponent<Collider>().bounds.center, pacman.GetComponent<Collider>().bounds.extents, pacman.transform.rotation, (1<<1)+(1<<3)+(1<<4)+(1<<5)+(1<<6)+(1<<7)+(1<<8)+(1<<9)+(1<<10));
		for(int h=0; (hits != null) && (h < hits.Length); h++) {
			if(hits[h].gameObject.layer == 1) {
				hits[h].gameObject.SetActive(false);
				pills.Remove(hits[h].gameObject);
				Score(10);
			}
			else if( (hits[h].gameObject.layer == 3) || (hits[h].gameObject.layer == 9) ) {
				pacdir = 0;
				pacman.transform.position = new Vector3(MazeJToX(XToMazeJ(newPacX)), MazeIToY(YToMazeI(newPacY)), 0);
			}
			else if( (hits[h].gameObject.layer >= 4) && (hits[h].gameObject.layer <= 7) ) {
				if(ghostState[hits[h].gameObject.layer-4] == 0)
					KillPlayer();
				else if(ghostState[hits[h].gameObject.layer-4] == 1) {
					Score(blueScore);
					blueScore *= 2;
					ghostState[hits[h].gameObject.layer-4] = 2;
				}
			}
			else if(hits[h].gameObject.layer == 8) {
				hits[h].gameObject.SetActive(false);
				pills.Remove(hits[h].gameObject);
				Score(50);
				blueTime = Time.time + (level == 1 ? 6f : level == 2 ? 5f : level == 3 ? 4f : level == 4 ? 3f : level <= 8 ? 2f : level <= 16 ? 1f : 0f);
				blueScore = 200;
				for (int g=0; g<ghosts.Length; g++)
					ghostState[g] = 1;
			}
			else if(hits[h].gameObject.layer == 10) {
				hits[h].gameObject.SetActive(false);
				fruit.Remove(hits[h].gameObject);
				Score( level == 1 ? 100 : level == 2 ? 300 : level <= 4 ? 500 : level <= 6 ? 700 : level <= 8 ? 1000 : level <= 10 ? 2000 : level <= 12 ? 3000 : 5000 );
				fruitTime = Time.time + 15f;
			}
		}
		for(int g=0; g<ghosts.Length; g++) {
			float newX = ghosts[g].transform.position.x + ((ghostState[g] == 2) ? 2f : 1f) * Time.deltaTime * (ghostdir[g] == 1 ? -1f : ghostdir[g] == 2 ? 1f : 0f);
			float newY = ghosts[g].transform.position.y + ((ghostState[g] == 2) ? 2f : 1f) * Time.deltaTime * (ghostdir[g] == 3 ? -1f : ghostdir[g] == 4 ? 1f : 0f);
			newX = newX < MazeJToX(0) ? MazeJToX(maze[0].Length - 1) : newX > MazeJToX(maze[0].Length - 1) ? MazeJToX(0) : newX;    // Wrap around left-right
			ghosts[g].transform.position = new Vector3(ghostdir[g] >= 3 ? MazeJToX(XToMazeJ(newX)) : newX, ghostdir[g] <= 2 ? MazeIToY(YToMazeI(newY)) : newY, 0);
			ghosts[g].GetComponent<MeshRenderer>().materials[0].color = (ghostState[g] == 1) ? ( (Time.time < (blueTime-2.5f)) || ((Time.time % 0.5f) < 0.25f) ) ? Color.blue : Color.white : ghostState[g] == 2 ? Color.white : g == 0 ? Color.red : g == 1 ? Color.cyan : g == 2 ? new Color(1f, 0.6f, 0.6f, 1f) : new Color(1f, 0.6f, 0f, 1f);
			ghostState[g] = Time.time > blueTime ? 0 : ghostState[g];
			if (NearCellCentre(ghosts[g].transform.position) && ((YToMazeI(newY) != sideTry[g][1]) || (XToMazeJ(newX) != sideTry[g][2]))) { // if not attempted sideways move this cell then try it
				sideTry[g][0] = ChangeDirectionTo(ghosts[g], (Time.time > blueTime) ? pacman : ghostExit, ghostdir[g], (ghostState[g] == 2) ? 0.25f : 0.25f + (g * 0.25f));
				sideTry[g][1] = YToMazeI(newY);
				sideTry[g][2] = XToMazeJ(newX);
				if((sideTry[g][0] == DirectionTo(ghosts[g], (Time.time > blueTime) ? pacman : ghostExit)) && (wallChars.IndexOf( MazeChar(sideTry[g][1], sideTry[g][2], sideTry[g][0]) ) == -1) )
					ghostdir[g] = sideTry[g][0];
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