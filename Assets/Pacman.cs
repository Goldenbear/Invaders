using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
public class Pacman : MonoBehaviour {
	static int score = 0;
	static int lives = 2;
	static int level = 1;
	string[] maze = new string[] {	"3333333333333333333333333333","3111111111111331111111111113","3133331333331331333331333313","3833331333331331333331333383","3133331333331331333331333313",
									"3111111111111111111111111113","3133331331333333331331333313","3133331331333333331331333313","3111111331111331111331111113","3333331333330330333331333333",
									"0000031333330330333331300000","0000031330000400000331300000","0000031330333903330331300000","3333331330350607030331333333","0000001000300000030001000000",
									"3333331330300000030331333333","0000031330333333330331300000","0000031330000A00000331300000","0000031330333333330331300000","3333331330333333330331333333",
									"3111111111111331111111111113","3133331333331331333331333313","3133331333331331333331333313","3811331111111211111111331183","3331331331333333331331331333",
									"3331331331333333331331331333","3111111331111331111331111113","3133333333331331333333333313","3133333333331331333333333313","3111111111111111111111111113","3333333333333333333333333333"};
	List<GameObject> pills = new List<GameObject>();
	GameObject pacman;
	Vector3 pacStart;
	int pacdir = 0;
	GameObject[] ghosts = new GameObject[4];
	int[] ghostdir = new int[4];
	int[] ghostState = new int[4];
	GameObject ghostExit;
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
	void Start() {
		gameObject.GetComponent<Camera>().backgroundColor = Color.black;
		new GameObject("Light").AddComponent<Light>().type = LightType.Directional;
		Physics.IgnoreLayerCollision(2, 1, true);
        for(int i=0; i<maze.Length; i++)
			for(int j=0; j<maze[i].Length; j++) {
				if(maze[i][j] == '1')
					pills.Add( CreateMazeObject("Pill", PrimitiveType.Cube, j, i, 0.1f, 1, new Color(1f,0.6f,0.6f,1f)) );
				else if(maze[i][j] == '2')
					pacman = CreateMazeObject("Pacman", PrimitiveType.Sphere, j, i, 0.3f, 2, Color.yellow);
				else if(maze[i][j] == '3')
					CreateMazeObject("Wall", PrimitiveType.Cube, j, i, 0.25f, 3, Color.blue);
				else if(maze[i][j] == '4')
					ghosts[0] = CreateMazeObject("Blinky", PrimitiveType.Sphere, j, i, 0.3f, 4, Color.red);
				else if(maze[i][j] == '5')
					ghosts[1] = CreateMazeObject("Inky", PrimitiveType.Sphere, j, i, 0.3f, 5, Color.cyan);
				else if(maze[i][j] == '6')
					ghosts[2] = CreateMazeObject("Pinky", PrimitiveType.Sphere, j, i, 0.3f, 6, new Color(1f, 0.6f, 0.6f, 1f));
				else if(maze[i][j] == '7')
					ghosts[3] = CreateMazeObject("Clyde", PrimitiveType.Sphere, j, i, 0.3f, 7, new Color(1f, 0.6f, 0f, 1f));
				else if(maze[i][j] == '8')
					pills.Add( CreateMazeObject("Power", PrimitiveType.Sphere, j, i, 0.3f, 8, new Color(1f,0.6f,0.6f,1f)) );
				else if(maze[i][j] == '9')
					ghostExit = CreateMazeObject("GhostExit", PrimitiveType.Cube, j, i, 0.25f, 9, Color.black, false);
				else if(maze[i][j] == 'A')
					msgPos = new Vector3(MazeJToX(j) + 0.15f, MazeIToY(i), 0);
			}
		pacStart = pacman.transform.position;
		ghostExit.transform.position = new Vector3(ghostExit.transform.position.x + 0.15f, ghostExit.transform.position.y - 0.15f, 0f);
		for(int g=0; g<ghosts.Length; g++)
			ghostdir[g] = 1 + (int)(Random.value * 3.9999f);
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
			uiObjects[1+i].GetComponent<RectTransform>().localPosition = i == 0 ? new Vector3(100f, 0f, 0) : new Vector3((msgPos.x+4.5f)*100f, (msgPos.y-4.8f)*100f, 0f);
			uiObjects[1+i].GetComponent<RectTransform>().localScale = new Vector3(1f, 1f, 1f);
			uiObjects[1+i].GetComponent<RectTransform>().sizeDelta = new Vector2(200f, 35f);
		}
    }
	GameObject CreateMazeObject(string label, PrimitiveType shape, int x, int y, float size, int layer, Color color, bool visible = true) {
		GameObject go = GameObject.CreatePrimitive(shape);
		go.name = label;
		go.transform.position = new Vector3(MazeJToX(x), MazeIToY(y), 0);
		go.transform.localScale = new Vector3(size, size, size);
		go.layer = layer;
		go.GetComponent<MeshRenderer>().materials[0].color = color;
		go.GetComponent<MeshRenderer>().enabled = visible;
		return go;
	}
	void KillPlayer() {
		playerLives[lives].GetComponent<MeshRenderer>().enabled = false;
		pacman.transform.position = pacStart;
		pacdir = 0;
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
	int DirectionTo(GameObject objA, GameObject objB, int currDir, float randomness) {
		Vector3 diff = objB.transform.position - objA.transform.position;
		if(Random.value < randomness)
			return ((currDir == 1) || (currDir == 2)) ? ((Random.value < 0.5f) ? 3 : 4) : ((Random.value < 0.5f) ? 1 : 2);
		return ((currDir == 1) || (currDir == 2)) ? ((diff.y < 0f) ? 3 : 4) : ((diff.x < 0f) ? 1 : 2);
	}
    void Update() {
		uiObjects[1].GetComponent<Text>().text = string.Format("{0:00000}", score);
		uiObjects[2].GetComponent<Text>().text = gameState == 0 ? "READY!" : gameState == 2 ? "GAME OVER" : "";
		if( (gameState == 0) || (gameState == 2) ) {
			gameState = (gameState == 0) && Input.GetKeyDown(KeyCode.Space) ? 1 : gameState;
			if ((gameState == 2) && Input.GetKeyDown(KeyCode.Space)) {
				score = 0; lives = 2; level = 1;
				UnityEngine.SceneManagement.SceneManager.LoadScene("Pacman");      // Reload scene and reset score, lives & level for a new game
			}
			return;
		}
		pacdir = Input.GetKeyDown(KeyCode.LeftArrow) ? 1 : Input.GetKeyDown(KeyCode.RightArrow) ? 2 : Input.GetKeyDown(KeyCode.DownArrow) ? 3 : Input.GetKeyDown(KeyCode.UpArrow) ? 4 : pacdir;
		float newPacX = pacman.transform.position.x + 1.5f * Time.deltaTime * (pacdir == 1 ? -1f : pacdir == 2 ? 1f : 0f);
		float newPacY = pacman.transform.position.y + 1.5f * Time.deltaTime * (pacdir == 3 ? -1f : pacdir == 4 ? 1f : 0f);
		newPacX = newPacX < MazeJToX(0) ? MazeJToX(maze[0].Length - 1) : newPacX > MazeJToX(maze[0].Length - 1) ? MazeJToX(0) : newPacX;	// Wrap around left-right
		pacman.transform.position = new Vector3((pacdir == 0) || (pacdir >= 3) ? MazeJToX(XToMazeJ(newPacX)) : newPacX, pacdir <= 2 ? MazeIToY(YToMazeI(newPacY)) : newPacY, 0);
		Collider[] hits = Physics.OverlapBox(pacman.GetComponent<Collider>().bounds.center, pacman.GetComponent<Collider>().bounds.extents, pacman.transform.rotation, (1<<1)+(1<<3)+(1<<4)+(1<<5)+(1<<6)+(1<<7)+(1<<8));
		if ((hits != null) && (hits.Length > 0)) {
			if(hits[0].gameObject.layer == 1) {
				hits[0].gameObject.SetActive(false);
				pills.Remove(hits[0].gameObject);
				Score(10);
			}
			else if(hits[0].gameObject.layer == 3) {
				pacdir = 0;
				int x = Mathf.RoundToInt( (pacman.transform.position.x + 4.5f) / 0.3f );
				int y = Mathf.RoundToInt( (4.5f - pacman.transform.position.y) / 0.3f );
				pacman.transform.position = new Vector3(-4.5f+(x*0.3f), 4.5f-(y*0.3f), 0);
			}
			else if( (hits[0].gameObject.layer >= 4) && (hits[0].gameObject.layer <= 7) ) {
				if(ghostState[hits[0].gameObject.layer-4] == 0)
					KillPlayer();
				else if(ghostState[hits[0].gameObject.layer-4] == 1) {
					Score(blueScore);
					blueScore *= 2;
					ghostState[hits[0].gameObject.layer-4] = 2;
				}
			}
			else if(hits[0].gameObject.layer == 8) {
				hits[0].gameObject.SetActive(false);
				pills.Remove(hits[0].gameObject);
				Score(10);
				blueTime = Time.time + 6f;
				blueScore = 200;
				for (int g=0; g<ghosts.Length; g++)
					ghostState[g] = 1;
			}
		}
		for(int g=0; g<ghosts.Length; g++) {
			float newX = ghosts[g].transform.position.x + 1f * Time.deltaTime * (ghostdir[g] == 1 ? -1f : ghostdir[g] == 2 ? 1f : 0f);
			float newY = ghosts[g].transform.position.y + 1f * Time.deltaTime * (ghostdir[g] == 3 ? -1f : ghostdir[g] == 4 ? 1f : 0f);
			newX = newX < MazeJToX(0) ? MazeJToX(maze[0].Length - 1) : newX > MazeJToX(maze[0].Length - 1) ? MazeJToX(0) : newX;    // Wrap around left-right
			ghosts[g].transform.position = new Vector3(ghostdir[g] >= 3 ? MazeJToX(XToMazeJ(newX)) : newX, ghostdir[g] <= 2 ? MazeIToY(YToMazeI(newY)) : newY, 0);
			ghosts[g].GetComponent<MeshRenderer>().materials[0].color = ghostState[g] == 1 ? Color.blue : ghostState[g] == 2 ? Color.white : g == 0 ? Color.red : g == 1 ? Color.cyan : g == 2 ? new Color(1f, 0.6f, 0.6f, 1f) : new Color(1f, 0.6f, 0f, 1f);
			ghostState[g] = Time.time > blueTime ? 0 : ghostState[g];
			Collider[] ghostHits = Physics.OverlapBox(ghosts[g].GetComponent<Collider>().bounds.center, ghosts[g].GetComponent<Collider>().bounds.extents, ghosts[g].transform.rotation, (1<<3)+(1<<9));
			if ((ghostHits != null) && (ghostHits.Length > 0))
				ghostdir[g] = (ghostHits[0].gameObject.layer == 9) ? 4 : DirectionTo(ghosts[g], (Time.time > blueTime) ? pacman : ghostExit, ghostdir[g], 0.2f+(g*0.2f));
		}
		if (pills.Count == 0) {
			level++;
			UnityEngine.SceneManagement.SceneManager.LoadScene("Pacman");      // Reload scene for a new level
		}
    }
}