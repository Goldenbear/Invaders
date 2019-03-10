using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
public class Defender : MonoBehaviour {
	public class GameData : MonoBehaviour {
		public GameObject target;
	}
	static int score = 0, lives = 2, level = 1;
	GameObject explosion, player;
	GameObject[] uiObjects = new GameObject[10];
	List<GameObject> allObjects = new List<GameObject>();
	Vector3[] playerVs = { new Vector3(-0.3f, 0f, 0f), new Vector3(0.3f, 0f, 0f), new Vector3(0f, 0.1f, 0f), new Vector3(-0.3f, 0.1f, 0f), new Vector3(-0.3f, 0f, 0f) };
	Vector3[] bulletVs = { new Vector3(-0.5f, 0f, 0f), new Vector3(0.5f, 0f, 0f) };
	Vector3[] terrainVs = { new Vector3(-20f, 0f, 0f), new Vector3(-18f, 2f, 0f), new Vector3(-15f, 0f, 0f), new Vector3(-12f, 0f, 0f), new Vector3(-9f, 2f, 0f), new Vector3(-6f, 0f, 0f), new Vector3(-3f, 0f, 0f), new Vector3(-2f, 1f, 0f), new Vector3(-1f, 0f, 0f), new Vector3(2f, 0f, 0f), new Vector3(3f, 1f, 0f), new Vector3(4f, 0f, 0f), new Vector3(5f, 0f, 0f), new Vector3(8f, 2f, 0f), new Vector3(11f, 0f, 0f), new Vector3(15f, 0f, 0f), new Vector3(16.5f, 0.5f, 0f), new Vector3(17f, 0.3f, 0f), new Vector3(17.5f, 0.8f, 0f), new Vector3(18f, 0.6f, 0f), new Vector3(18.5f, 1.1f, 0f), new Vector3(19.5f, 0f, 0f), new Vector3(20f, 0f, 0f) };
	Vector3[] landerVs = { new Vector3(-0.5f, -0.5f, 0f), new Vector3(-0.2f, 0f, 0f), new Vector3(0f, 0f, 0f), new Vector3(0f, -0.5f, 0f), new Vector3(0f, -0.5f, 0f), new Vector3(0f, 0f, 0f), new Vector3(0.2f, 0f, 0f), new Vector3(0.5f, -0.5f, 0f), new Vector3(0.5f, -0.5f, 0f), new Vector3(0.2f, 0f, 0f), new Vector3(0f, 0f, 0f), new Vector3(-0.1f, 0f, 0f), new Vector3(-0.35f, 0.1f, 0f), new Vector3(-0.4f, 0.2f, 0f), new Vector3(-0.4f, 0.3f, 0f), new Vector3(-0.35f, 0.4f, 0f), new Vector3(-0.1f, 0.5f, 0f), new Vector3(0.1f, 0.5f, 0f), new Vector3(0.35f, 0.4f, 0f), new Vector3(0.4f, 0.3f, 0f), new Vector3(0.4f, 0.2f, 0f), new Vector3(0.35f, 0.1f, 0f), new Vector3(0.1f, 0f, 0f), new Vector3(0f, 0f, 0f), new Vector3(0f, 0.5f, 0f) };
	Vector3[] sqrVs = { new Vector3(-0.5f, -0.5f, 0f), new Vector3(-0.5f, 0.5f, 0f), new Vector3(0.5f, 0.5f, 0f), new Vector3(0.5f, -0.5f, 0f), new Vector3(-0.5f, -0.5f, 0f) };
	int[] sqrTs = new int[] {0, 1, 2, 0, 3, 2};
	float camOffset = 0f, gameStateTimer = 0f;
	int gameState = 0;
    void Start() {
		gameObject.GetComponent<Camera>().backgroundColor = Color.black;
		explosion = new GameObject("Explosion");
		explosion.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
		explosion.AddComponent<ParticleSystem>().Pause();
		explosion.GetComponent<ParticleSystemRenderer>().material = new Material(Shader.Find("Sprites/Default"));
		ParticleSystem.MainModule main = explosion.GetComponent<ParticleSystem>().main;
		main.startSize = 0.025f;
		main.startLifetime = 2f;
		main.startSpeed = 10f;
		main.scalingMode = ParticleSystemScalingMode.Shape;
		allObjects.Add( player = CreateVectorObject("Player", playerVs, 0f, 0f, 0f, 1f, 1f, 1f, 1, Color.white) );
        allObjects.Add( CreateVectorObject("Terrain1", terrainVs, -10f,     -4f, 0f, 1f, 1f, 1f, 0, new Color(0.6f, 0.3f, 0.1f)) );
        allObjects.Add( CreateVectorObject("Terrain2", terrainVs, -10f+40f, -4f, 0f, 1f, 1f, 1f, 0, new Color(0.6f, 0.3f, 0.1f)) );
		for(int i=0; i<100; i++)
        	allObjects.Add( CreateVectorObject("Star", sqrVs, Random.Range(-40f, 40f), Random.Range(-2f, 4f), 0f, 0.02f, 0.02f, 0.02f, 0, new Color(Random.value, Random.value, Random.value, Random.value)) );
		for(int i=0; i<10; i++)
        	allObjects.Add( CreateMeshObject("Human", sqrVs, sqrTs, Random.Range(-40f, 40f), -4.5f, 0f, 0.15f, 0.3f, 0.2f, 3, new Color(1f, 0.6f, 0.8f, 1f)) );
		for(int i=0; i<5+(level*5); i++)
        	allObjects.Add( CreateVectorObject("Lander", landerVs, Random.Range(4f, 40f)*(Random.value<0.5f?-1f:1f), 4f, 0f, 0.35f, 0.3f, 0.2f, 4, Color.green) );
		uiObjects[0] = new GameObject("UICanvas");
		uiObjects[0].AddComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
		for(int i=0; i<4; i++) {
			uiObjects[1+i] = new GameObject("UIText");
			uiObjects[1+i].transform.parent = uiObjects[0].transform;
			uiObjects[1+i].AddComponent<Text>().font = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;
			uiObjects[1+i].GetComponent<Text>().fontSize = 30;
			uiObjects[1+i].GetComponent<Text>().alignment = TextAnchor.MiddleCenter;
			uiObjects[1+i].GetComponent<RectTransform>().localPosition = i == 0 ? new Vector3(-400f, 250f, 0f) : i == 1 ? new Vector3(0f, 250f, 0f) : new Vector3(0f, 0f, 0f);
			uiObjects[1+i].GetComponent<RectTransform>().sizeDelta = new Vector2(800f, 800f);
		}
    }
	GameObject SetupObject(GameObject go, float pX, float pY, float pZ, float sX, float sY, float sZ, int layer, Color color, bool active, bool visible) {
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
	GameObject CreateMeshObject(string label, Vector3[] verts, int[] tris, float pX, float pY, float pZ, float sX, float sY, float sZ, int layer, Color color, bool active = true, bool visible = true) {
		GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);	// Adds MeshRenderer, MeshFilter and BoxCollider in one line!
		go.name = label;
		go.GetComponent<MeshFilter>().mesh = new Mesh();
		go.GetComponent<MeshFilter>().mesh.vertices = verts;
		go.GetComponent<MeshFilter>().mesh.triangles = tris;
		return SetupObject(go, pX, pY, pZ, sX, sY, sZ, layer, color, active, visible);
	}
	GameObject CreateVectorObject(string label, Vector3[] shape, float pX, float pY, float pZ, float sX, float sY, float sZ, int layer, Color color, bool active = true, bool visible = true) {
		GameObject go = new GameObject(label);
		go.AddComponent<BoxCollider>().isTrigger = true;
		LineRenderer line = go.AddComponent<LineRenderer>();
		line.useWorldSpace = false;
		line.widthMultiplier = 0.04f;
		line.positionCount = shape.Length;
		line.SetPositions(shape);
		return SetupObject(go, pX, pY, pZ, sX, sY, sZ, layer, color, active, visible);
	}
	void KillPlayer() {
		Explode(player, 50);
		//playerLives[lives].SetActive(false);
		gameState = 1;								// Player lost a life
		gameStateTimer = Time.unscaledTime + 3f;
		if (--lives < 0) {
			gameState = 3;							// Player dead = game over
		}
	}
	void Score(int add) {
		//lives = (score / 10000) < ((score + add) / 10000) ? ((lives < (playerLives.Length - 1)) ? lives + 1 : lives) : lives;
		//playerLives[lives].SetActive(true);
		score += add;
		PlayerPrefs.SetInt("DefenderHighScore", score > PlayerPrefs.GetInt("DefenderHighScore") ? score : PlayerPrefs.GetInt("DefenderHighScore"));
	}
	void Explode(GameObject go, int numParticles=20) {
		go.SetActive(false);
		explosion.transform.position = go.transform.position;
		explosion.GetComponent<Renderer>().material.color = go.GetComponent<Renderer>().material.color;
		explosion.GetComponent<ParticleSystem>().Emit(numParticles);
	}
    void Update() {
		uiObjects[1].GetComponent<Text>().text = string.Format("{0:00000}", score);
		uiObjects[2].GetComponent<Text>().text = string.Format("{0:00000}", PlayerPrefs.GetInt("DefenderHighScore"));
		uiObjects[3].GetComponent<Text>().text = gameState == 2 ? "ATTACK WAVE "+level+"\n COMPLETED\n\nBONUS "+allObjects.Where(x => x.layer == 3).Count()+" X 100" : gameState == 3 ? "GAME OVER" : "";
		if(gameState > 0) {
			if( (gameState == 1) && (Time.unscaledTime > gameStateTimer) ) {
				player.transform.position = Vector3.zero;
				player.SetActive(true);
				gameState = 0;
			}
			if( (gameState >= 2) && ((Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))) ) {
				level++;
				if(gameState == 3) { score = 0; lives = 2; level = 1; }					// New game
				UnityEngine.SceneManagement.SceneManager.LoadScene("Defender");			// Reload scene for new level
			}
			return;
		}
		List<GameObject> destroyed = new List<GameObject>();
		if( Input.GetKeyDown(KeyCode.LeftShift) )
        	allObjects.Add( CreateVectorObject("Bullet", bulletVs, player.transform.position.x+Mathf.Sign(player.transform.localScale.x)*0.3f, player.transform.position.y, player.transform.position.z, Mathf.Sign(player.transform.localScale.x), 0.1f, 1f, 2, Color.yellow) );
		camOffset = Mathf.Clamp(camOffset+Mathf.Sign(player.transform.localScale.x)*10f*Time.deltaTime, -4.5f, 4.5f);
		gameObject.transform.position = new Vector3(player.transform.position.x+camOffset, gameObject.transform.position.y, gameObject.transform.position.z);
		foreach(GameObject go in allObjects) {
			float pX = go.transform.position.x, pY = go.transform.position.y, sX = go.transform.localScale.x;
			switch(go.layer) {
/* Player */	case 1:	sX = Input.GetKey(KeyCode.LeftArrow) ? -Mathf.Abs(sX) : Input.GetKey(KeyCode.RightArrow) ? Mathf.Abs(sX) : sX;
						pX += ((Input.GetKey(KeyCode.LeftArrow) ? -10f : Input.GetKey(KeyCode.RightArrow) ? 10f : 0f) * Time.deltaTime);
						pY += ((Input.GetKey(KeyCode.DownArrow) ? -5f : Input.GetKey(KeyCode.UpArrow) ? 5f : 0f) * Time.deltaTime);
						pY = Mathf.Clamp(pY, -4.5f, 4f); 
				break;
/* Bullet */	case 2: pX += Mathf.Sign(sX)*30f*Time.deltaTime; 
						sX += Mathf.Sign(sX)*30f*Time.deltaTime;
						if( Mathf.Abs(pX - player.transform.position.x) > 10f )
							destroyed.Add(go);
				break;
/* Human */		case 3: pX = go.GetComponent<GameData>().target != null ? go.GetComponent<GameData>().target.transform.position.x : pX;
						pY += go.GetComponent<GameData>().target != null ? (go.GetComponent<GameData>().target.transform.position.y-0.3f)-pY : pY > -4.5f ? -1f*Time.deltaTime : 0f;
						if( (pY < -4.3f) && (pY > -4.4f) && (go.GetComponent<GameData>().target == null) ) {
							Explode(go);
							destroyed.Add(go);
						}
						else if( (pY <= -4.5f) && (go.GetComponent<GameData>().target == player) ) {
							go.GetComponent<GameData>().target = player.GetComponent<GameData>().target = null;	// Drop off human
							pY = -4.5f;
						}
				break;
/* Lander */	case 4: if( go.GetComponent<GameData>().target == null ) {										// If we dont have a human target
							float nearest = float.MaxValue;														// Target the nearest free human
							foreach(GameObject human in allObjects.Where(x => x.layer == 3 && x.GetComponent<GameData>().target == null)) {
								if(Mathf.Abs(human.transform.position.x-go.transform.position.x) < nearest) {
									nearest = Mathf.Abs(human.transform.position.x-go.transform.position.x);
									go.GetComponent<GameData>().target = human;
								}
							}
						}
						if(go.GetComponent<GameData>().target != null) {
							if(go.GetComponent<GameData>().target.GetComponent<GameData>().target == null) {	// If we havent reached our human
								Vector3 diff = go.GetComponent<GameData>().target.transform.position - go.transform.position;	// Move towards them
								diff.y = Mathf.Abs(diff.x) < 3f ? diff.y : ((go.GetComponent<Renderer>().material.color == Color.magenta ? 2f : 0f) + (Time.time%2f)) - go.transform.position.y;		// Stay high until near
								pX += Mathf.Sign(diff.x) * 1.0f*Time.deltaTime * (go.GetComponent<Renderer>().material.color == Color.magenta ? 2f : 1f);
								pY += Mathf.Sign(diff.y) * 0.8f*Time.deltaTime * (go.GetComponent<Renderer>().material.color == Color.magenta ? 2f : 1f);
							}
							else if(go.GetComponent<GameData>().target.GetComponent<GameData>().target == go) {	// If we have a human
								pY += pY < 4f ? 0.5f*Time.deltaTime : 0f;										// Go up
								if(pY >= 4f) {
									destroyed.Add(go.GetComponent<GameData>().target);							// Kill human
									go.GetComponent<GameData>().target = player;								// Target player
									go.GetComponent<Renderer>().material.color = Color.magenta;					// Turn Mutant
								}
							}
							else																				// No humans left to target
								go.GetComponent<GameData>().target = player;									// Target player
						}
				break;
			}
			Collider[] hits = Physics.OverlapBox(go.GetComponent<Collider>().bounds.center, go.GetComponent<Collider>().bounds.extents, go.transform.rotation, go.layer == 1 ? (1<<3) : go.layer == 3 ? (1<<2) : go.layer >= 4 ? (1<<1)+(1<<2)+(1<<3) : 0);
			for(int h=0; (hits != null) && (h < hits.Length); h++) {
				if(hits[h].gameObject.layer == 1) {							// Player
					KillPlayer();
				}
				else if(hits[h].gameObject.layer == 2) {					// Bullet
					Explode(go);
					destroyed.Add(go);
					destroyed.Add(hits[h].gameObject);
					Score(go.layer == 4 ? 150 : 0);
				}
				else if(hits[h].gameObject.layer == 3) {					// Human
					if( ((go == player) && (go.GetComponent<GameData>().target == null) && (hits[h].gameObject.transform.position.y > -4.4f)) || 
											(go.GetComponent<GameData>().target == hits[h].gameObject) ) {
						go.GetComponent<GameData>().target = hits[h].gameObject;
						hits[h].gameObject.GetComponent<GameData>().target = go;
					}
				}
			}
			pX += (pX < player.transform.position.x-40f) ? 80f : (pX > player.transform.position.x+40f) ? -80f : 0f;	// Wrap
			go.transform.position = new Vector3(pX, pY, go.transform.position.z);
			go.transform.localScale = new Vector3(sX, go.transform.localScale.y, go.transform.localScale.z);
		}
		foreach(GameObject dead in destroyed) {
			foreach(GameObject go in allObjects.Where(x => x.GetComponent<GameData>().target == dead))
				go.GetComponent<GameData>().target = null;
			allObjects.Remove(dead);
			Destroy(dead);
		}
		if(allObjects.Where(x => x.layer == 4).Count() == 0) {				// Level complete
			Score(allObjects.Where(x => x.layer == 3).Count() * 100);
			gameState = 2;
		}
    }
}