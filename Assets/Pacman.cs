using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
public class Pacman : MonoBehaviour {
	static int score = 0;
	static int lives = 2;
	static int level = 1;
	string[] maze = new string[] {	"3333333333333333333333333333","3111111111111331111111111113","3133331333331331333331333313","3133331333331331333331333313","3133331333331331333331333313",
									"3111111111111111111111111113","3133331331333333331331333313","3133331331333333331331333313","3111111331111331111331111113","3333331333330330333331333333",
									"0000031333330330333331300000","0000031330000400000331300000","0000031330333003330331300000","3333331330350607030331333333","0000001000300000030001000000",
									"3333331330300000030331333333","0000031330333333330331300000","0000031330000800000331300000","0000031330333333330331300000","3333331330333333330331333333",
									"3111111111111331111111111113","3133331333331331333331333313","3133331333331331333331333313","3111331111111211111111331113","3331331331333333331331331333",
									"3331331331333333331331331333","3111111331111331111331111113","3133333333331331333333333313","3133333333331331333333333313","3111111111111111111111111113","3333333333333333333333333333"};
	List<GameObject> pills = new List<GameObject>();
	GameObject pacman;
	Vector3 pacStart;
	int pacdir = 0;
	GameObject[] ghosts = new GameObject[4];
	int[] ghostdir = new int[4];
	GameObject[] playerLives = new GameObject[20];  // Max 20 lives
	GameObject[] uiObjects = new GameObject[10];
	Vector3 msgPos;
	bool gameOver = false;

    void Start() {
		gameObject.GetComponent<Camera>().backgroundColor = Color.black;
		new GameObject("Light").AddComponent<Light>().type = LightType.Directional;
		Physics.IgnoreLayerCollision(2, 1, true);
        for(int i=0; i<maze.Length; i++)
			for(int j=0; j<maze[i].Length; j++) {
				if(maze[i][j] == '1') {
					pills.Add( CreateMazeObject("Pill", PrimitiveType.Cube, j, i, 0.1f, 1, new Color(1f,0.6f,0.6f,1f)) );
				}
				else if(maze[i][j] == '2') {
					pacman = CreateMazeObject("Pacman", PrimitiveType.Sphere, j, i, 0.3f, 2, Color.yellow);
					pacStart = pacman.transform.position;
				}
				else if(maze[i][j] == '3') {
					CreateMazeObject("Wall", PrimitiveType.Cube, j, i, 0.25f, 3, Color.blue);
				}
				else if(maze[i][j] == '4') {
					ghosts[0] = CreateMazeObject("Blinky", PrimitiveType.Sphere, j, i, 0.3f, 4, Color.red);
				}
				else if(maze[i][j] == '5') {
					ghosts[1] = CreateMazeObject("Inky", PrimitiveType.Sphere, j, i, 0.3f, 5, Color.cyan);
				}
				else if(maze[i][j] == '6') {
					ghosts[2] = CreateMazeObject("Pinky", PrimitiveType.Sphere, j, i, 0.3f, 6, new Color(1f, 0.6f, 0.6f, 1f));
					CreateMazeObject("Exit", PrimitiveType.Cube, j, i, 0.25f, 20, Color.black, false);
				}
				else if(maze[i][j] == '7') {
					ghosts[3] = CreateMazeObject("Clyde", PrimitiveType.Sphere, j, i, 0.3f, 7, new Color(1f, 0.6f, 0f, 1f));
				}
				else if(maze[i][j] == '8') {
					msgPos = new Vector3(-4.5f+(j*0.3f)+0.15f, 4.5f-(i*0.3f), 0);
				}
			}
		for(int g=0; g<ghosts.Length; g++)
			ghostdir[g] = 1 + (int)(Random.value * 3.9999f);
		for (int i = 0; i < playerLives.Length; i++) {
			playerLives[i] = CreateMazeObject("Life", PrimitiveType.Sphere, 3+i, maze.Length, 0.3f, 2, Color.yellow, (i <= lives));
		}
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
		//go.SetActive(visible);
		go.name = label;
		go.transform.position = new Vector3(-4.5f+(x*0.3f), 4.5f-(y*0.3f), 0);
		go.transform.localScale = new Vector3(size, size, size);
		go.layer = layer;
		go.GetComponent<MeshRenderer>().materials[0].color = color;
		go.GetComponent<MeshRenderer>().enabled = visible;
		return go;
	}
	void KillPlayer() {
		playerLives[lives].SetActive(false);
		pacman.transform.position = pacStart;
		pacdir = 0;
		if (--lives < 0) {
			pacman.SetActive(false);
			gameOver = true;                            // Player dead = game over
		}
	}
	void Score(int add) {
		lives = (score / 10000) < ((score + add) / 10000) ? ((lives < (playerLives.Length - 1)) ? lives + 1 : lives) : lives;
		playerLives[lives].SetActive(true);
		score += add;
	}
    void Update()
    {
		uiObjects[1].GetComponent<Text>().text = string.Format("{0:00000}", score);
		uiObjects[2].GetComponent<Text>().text = gameOver ? "GAME OVER" : "";
		if(gameOver) {
			if (Input.GetKeyDown(KeyCode.Space)) {
				score = 0; lives = 2; level = 1;
				UnityEngine.SceneManagement.SceneManager.LoadScene("Pacman");      // Reload scene and reset score, lives & level for a new game
			}
			return;
		}
		pacdir = Input.GetKeyDown(KeyCode.LeftArrow) ? 1 : Input.GetKeyDown(KeyCode.RightArrow) ? 2 : Input.GetKeyDown(KeyCode.DownArrow) ? 3 : Input.GetKeyDown(KeyCode.UpArrow) ? 4 : pacdir;
		float newPacX = pacman.transform.position.x + 1f * Time.deltaTime * (pacdir == 1 ? -1f : pacdir == 2 ? 1f : 0f);
		float newPacY = pacman.transform.position.y + 1f * Time.deltaTime * (pacdir == 3 ? -1f : pacdir == 4 ? 1f : 0f);
		pacman.transform.position = new Vector3(newPacX, newPacY, 0f);
		Collider[] hits = Physics.OverlapBox(pacman.GetComponent<Collider>().bounds.center, pacman.GetComponent<Collider>().bounds.extents, pacman.transform.rotation, (1<<1)+(1<<3)+(1<<4)+(1<<5)+(1<<6)+(1<<7));
		if ((hits != null) && (hits.Length > 0)) {
			if(hits[0].gameObject.layer == 1) {
				hits[0].gameObject.SetActive(false);
				Score(10);
			}
			else if(hits[0].gameObject.layer == 3) {
				pacdir = 0;
				int x = Mathf.RoundToInt( (pacman.transform.position.x + 4.5f) / 0.3f );
				int y = Mathf.RoundToInt( (4.5f - pacman.transform.position.y) / 0.3f );
				pacman.transform.position = new Vector3(-4.5f+(x*0.3f), 4.5f-(y*0.3f), 0);
			}
			else if(hits[0].gameObject.layer >= 4) {
				KillPlayer();
			}
		}
		for(int g=0; g<ghosts.Length; g++) {
			float newX = ghosts[g].transform.position.x + 1f * Time.deltaTime * (ghostdir[g] == 1 ? -1f : ghostdir[g] == 2 ? 1f : 0f);
			float newY = ghosts[g].transform.position.y + 1f * Time.deltaTime * (ghostdir[g] == 3 ? -1f : ghostdir[g] == 4 ? 1f : 0f);
			ghosts[g].transform.position = new Vector3(newX, newY, 0f);
			Collider[] ghostHits = Physics.OverlapBox(ghosts[g].GetComponent<Collider>().bounds.center, ghosts[g].GetComponent<Collider>().bounds.extents, ghosts[g].transform.rotation, (1<<3)+(1<<20));
			if ((ghostHits != null) && (ghostHits.Length > 0)) {
				if( (ghostHits[0].gameObject.layer == 3) || (ghostHits[0].gameObject.layer == 20) ) {
					ghostdir[g] = (ghostHits[0].gameObject.layer == 20) ? 4 : 1 + (int)(Random.value * 3.9999f);
					int x = Mathf.RoundToInt( (ghosts[g].transform.position.x + 4.5f) / 0.3f );
					int y = Mathf.RoundToInt( (4.5f - ghosts[g].transform.position.y) / 0.3f );
					ghosts[g].transform.position = new Vector3(-4.5f+(x*0.3f), 4.5f-(y*0.3f), 0);
				}
			}
		}
    }
}
