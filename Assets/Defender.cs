using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
public class Defender : MonoBehaviour
{
	public class GameData : MonoBehaviour {
		public GameObject lander, human;
	}
	static int score = 0;
	static int lives = 2;
	static int level = 1;
	GameObject explosion, player;
	List<GameObject> allObjects = new List<GameObject>();
	Vector3[] playerVs = { new Vector3(-0.3f, 0f, 0f), new Vector3(0.3f, 0f, 0f), new Vector3(0f, 0.1f, 0f), new Vector3(-0.3f, 0.1f, 0f), new Vector3(-0.3f, 0f, 0f) };
	Vector3[] bulletVs = { new Vector3(-0.5f, 0f, 0f), new Vector3(0.5f, 0f, 0f) };
	Vector3[] terrainVs = { new Vector3(-20f, 0f, 0f), new Vector3(-18f, 2f, 0f), new Vector3(-15f, 0f, 0f), new Vector3(-12f, 0f, 0f), new Vector3(-9f, 2f, 0f), new Vector3(-6f, 0f, 0f), new Vector3(-3f, 0f, 0f), new Vector3(-2f, 1f, 0f), new Vector3(-1f, 0f, 0f), new Vector3(2f, 0f, 0f), new Vector3(3f, 1f, 0f), new Vector3(4f, 0f, 0f), new Vector3(5f, 0f, 0f), new Vector3(8f, 2f, 0f), new Vector3(11f, 0f, 0f), new Vector3(15f, 0f, 0f), new Vector3(16.5f, 0.5f, 0f), new Vector3(17f, 0.3f, 0f), new Vector3(17.5f, 0.8f, 0f), new Vector3(18f, 0.6f, 0f), new Vector3(18.5f, 1.1f, 0f), new Vector3(19.5f, 0f, 0f), new Vector3(20f, 0f, 0f) };
	Vector3[] sqrVs = { new Vector3(-0.5f, -0.5f, 0f), new Vector3(-0.5f, 0.5f, 0f), new Vector3(0.5f, 0.5f, 0f), new Vector3(0.5f, -0.5f, 0f), new Vector3(-0.5f, -0.5f, 0f) };
	int[] sqrTs = new int[] {0, 1, 2, 0, 3, 2};
	float camOffset = 0f;
	Canvas uiCanvas;
	Text uiScore;
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
		for(int i=0; i<10; i++)
        	allObjects.Add( CreateMeshObject("Lander", sqrVs, sqrTs, Random.Range(-40f, 40f), Random.Range(-2f, 4f), 0f, 0.3f, 0.3f, 0.2f, 4, Color.green) );
		uiCanvas = new GameObject("UI").AddComponent<Canvas>();
		uiCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
		uiScore = uiCanvas.gameObject.AddComponent<Text>();
		uiScore.font = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;
		uiScore.fontSize = 20;
    }
	GameObject SetupObject(GameObject go, float pX, float pY, float pZ, float sX, float sY, float sZ, int layer, Color color, bool active, bool visible) {
		go.SetActive(active);
		go.layer = layer;
		go.transform.position = new Vector3(pX, pY, pZ);
		go.transform.localScale = new Vector3(sX, sY, sZ);
		go.GetComponent<Renderer>().material.color = color;
		go.GetComponent<Renderer>().enabled = visible;
		go.AddComponent<GameData>();
		return go;
	}
	GameObject CreateMeshObject(string label, Vector3[] verts, int[] tris, float pX, float pY, float pZ, float sX, float sY, float sZ, int layer, Color color, bool active = true, bool visible = true) {
		GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);	// Adds MeshRenderer, MeshFilter and BoxCollider in one line!
		go.name = label;
		go.GetComponent<MeshRenderer>().material = new Material(Shader.Find("Sprites/Default"));
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
		line.material = new Material(Shader.Find("Sprites/Default"));
		line.positionCount = shape.Length;
		line.SetPositions(shape);
		return SetupObject(go, pX, pY, pZ, sX, sY, sZ, layer, color, active, visible);
	}
	void KillPlayer() {
		Explode(player, 50);
		//playerLives[lives].SetActive(false);
		//player.transform.position = RandomPosition;
		if (--lives < 0) {
			//gameOver = true;                            // Player dead = game over
		}
	}
	void Score(int add) {
		//lives = (score / 10000) < ((score + add) / 10000) ? ((lives < (playerLives.Length - 1)) ? lives + 1 : lives) : lives;
		//playerLives[lives].SetActive(true);
		score += add;
	}
	void Explode(GameObject go, int numParticles=20) {
		go.SetActive(false);
		explosion.transform.position = go.transform.position;
		explosion.GetComponent<Renderer>().material.color = go.GetComponent<Renderer>().material.color;
		explosion.GetComponent<ParticleSystem>().Emit(numParticles);
	}
    void Update() {
		uiScore.text = string.Format("{0:00000}", score);
		List<GameObject> destroyed = new List<GameObject>();
		int numAliens = 0;
		if( Input.GetKeyDown(KeyCode.Space) )
        	allObjects.Add( CreateVectorObject("Bullet", bulletVs, player.transform.position.x+Mathf.Sign(player.transform.localScale.x)*0.3f, player.transform.position.y, player.transform.position.z, Mathf.Sign(player.transform.localScale.x), 0.1f, 1f, 2, Color.yellow) );
		camOffset = Mathf.Clamp(camOffset+Mathf.Sign(player.transform.localScale.x)*10f*Time.deltaTime, -4.5f, 4.5f);
		gameObject.transform.position = new Vector3(player.transform.position.x+camOffset, gameObject.transform.position.y, gameObject.transform.position.z);
		foreach(GameObject go in allObjects) {
			float pX = go.transform.position.x;
			float pY = go.transform.position.y;
			float sX = go.transform.localScale.x;
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
/* Human */		case 3: pX = go.GetComponent<GameData>().lander != null ? go.GetComponent<GameData>().lander.transform.position.x : pX;
						pY += go.GetComponent<GameData>().lander != null ? (go.GetComponent<GameData>().lander.transform.position.y-0.3f)-pY : pY > -4.5f ? -1f*Time.deltaTime : 0f;
						if( (pY < -4.3f) && (pY > -4.4f) && (go.GetComponent<GameData>().lander == null) ) {
							Explode(go);
							destroyed.Add(go);
						}
						else if( (pY <= -4.5f) && (go.GetComponent<GameData>().lander == player) ) {
							go.GetComponent<GameData>().lander = null;
							player.GetComponent<GameData>().human = null;
							pY = -4.5f;
						}
				break;
/* Lander */	case 4: if( go.GetComponent<GameData>().human == null ) {
							float nearest = float.MaxValue;
							foreach(GameObject human in allObjects.Where(x => x.layer == 3 && x.GetComponent<GameData>().lander == null)) {
								if(Mathf.Abs(human.transform.position.x-go.transform.position.x) < nearest) {
									nearest = Mathf.Abs(human.transform.position.x-go.transform.position.x);
									go.GetComponent<GameData>().human = human;
								}
							}
						}
						if(go.GetComponent<GameData>().human != null) {
							if(go.GetComponent<GameData>().human.GetComponent<GameData>().lander == null) {
								Vector3 diff = go.GetComponent<GameData>().human.transform.position - go.transform.position;
								diff.y = Mathf.Abs(diff.x) < 3f ? diff.y : Random.Range(0f, 2f) - go.transform.position.y;
								pX += Mathf.Sign(diff.x) * 1.0f*Time.deltaTime;
								pY += Mathf.Sign(diff.y) * 0.8f*Time.deltaTime;
							}
							else if(go.GetComponent<GameData>().human.GetComponent<GameData>().lander == go)
								pY += pY < 4f ? 0.5f*Time.deltaTime : 0f;
							else
								go.GetComponent<GameData>().human = null;
						}
						numAliens++;
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
					Score(go.layer == 4 ? 10 : 0);
				}
				else if(hits[h].gameObject.layer == 3) {					// Human
					if( ((go == player) && (go.GetComponent<GameData>().human == null) && (hits[h].gameObject.transform.position.y > -4.4f)) || 
						(go.GetComponent<GameData>().human == hits[h].gameObject) ) {
						go.GetComponent<GameData>().human = hits[h].gameObject;
						hits[h].gameObject.GetComponent<GameData>().lander = go;
					}
				}
			}
			if(pX > player.transform.position.x+40f)
				pX -= 80f;
			else if(pX < player.transform.position.x-40f)
				pX += 80f;
			go.transform.position = new Vector3(pX, pY, go.transform.position.z);
			go.transform.localScale = new Vector3(sX, go.transform.localScale.y, go.transform.localScale.z);
		}
		foreach(GameObject go in destroyed) {
			foreach(GameObject human in allObjects.Where(x => x.layer == 3 && x.GetComponent<GameData>().lander == go))
				human.GetComponent<GameData>().lander = null;
			foreach(GameObject lander in allObjects.Where(x => x.layer == 4 && x.GetComponent<GameData>().human == go))
				lander.GetComponent<GameData>().human = null;
			allObjects.Remove(go);
			Destroy(go);
		}
		if (numAliens == 0) {
			level++;
			UnityEngine.SceneManagement.SceneManager.LoadScene("Defender");      // Reload scene for a new level
		}
    }
}
