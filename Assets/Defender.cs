using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Defender : MonoBehaviour
{
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
    void Start()
    {
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
        	allObjects.Add( CreateMeshObject("Alien", sqrVs, sqrTs, Random.Range(-40f, 40f), Random.Range(-2f, 4f), 0f, 0.3f, 0.3f, 0.2f, 4, Color.green) );
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
		return go;
	}
	GameObject CreateMeshObject(string label, Vector3[] verts, int[] tris, float pX, float pY, float pZ, float sX, float sY, float sZ, int layer, Color color, bool active = true, bool visible = true) {
		GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
		go.name = label;
		go.GetComponent<MeshRenderer>().material = new Material(Shader.Find("Sprites/Default"));
		go.GetComponent<MeshFilter>().mesh = new Mesh();
		go.GetComponent<MeshFilter>().mesh.vertices = verts;
		go.GetComponent<MeshFilter>().mesh.triangles = tris;
		return SetupObject(go, pX, pY, pZ, sX, sY, sZ, layer, color, active, visible);
	}
	GameObject CreateVectorObject(string label, Vector3[] shape, float pX, float pY, float pZ, float sX, float sY, float sZ, int layer, Color color, bool active = true, bool visible = true) {
		GameObject go = new GameObject(label);
		go.AddComponent<BoxCollider>();
		LineRenderer line = go.AddComponent<LineRenderer>();
		line.useWorldSpace = false;
		line.widthMultiplier = 0.04f;
		line.material = new Material(Shader.Find("Sprites/Default"));
		line.positionCount = shape.Length;
		line.SetPositions(shape);
		return SetupObject(go, pX, pY, pZ, sX, sY, sZ, layer, color, active, visible);
	}
	void KillPlayer() {
		//playerLives[lives].SetActive(false);
		//player.transform.position = RandomPosition;
		if (--lives < 0) {
			player.SetActive(false);
			//gameOver = true;                            // Player dead = game over
		}
	}
	void Score(int add) {
		//lives = (score / 10000) < ((score + add) / 10000) ? ((lives < (playerLives.Length - 1)) ? lives + 1 : lives) : lives;
		//playerLives[lives].SetActive(true);
		score += add;
	}
    void Update()
    {
		uiScore.text = string.Format("{0:00000}", score);
		List<GameObject> destroyed = new List<GameObject>();
		if( Input.GetKeyDown(KeyCode.Space) )
        	allObjects.Add( CreateVectorObject("Bullet", bulletVs, player.transform.position.x+Mathf.Sign(player.transform.localScale.x)*0.3f, player.transform.position.y, player.transform.position.z, Mathf.Sign(player.transform.localScale.x), 0.1f, 1f, 2, Color.yellow) );
		camOffset = Mathf.Clamp(camOffset+Mathf.Sign(player.transform.localScale.x)*10f*Time.deltaTime, -4.5f, 4.5f);
		gameObject.transform.position = new Vector3(player.transform.position.x+camOffset, gameObject.transform.position.y, gameObject.transform.position.z);
		foreach(GameObject go in allObjects) {
			float pX = go.transform.position.x;
			float pY = go.transform.position.y;
			float sX = go.transform.localScale.x;
			switch(go.layer) {
				case 1:	sX = Input.GetKey(KeyCode.LeftArrow) ? -Mathf.Abs(sX) : Input.GetKey(KeyCode.RightArrow) ? Mathf.Abs(sX) : sX;
						pX += ((Input.GetKey(KeyCode.LeftArrow) ? -10f : Input.GetKey(KeyCode.RightArrow) ? 10f : 0f) * Time.deltaTime);
						pY += ((Input.GetKey(KeyCode.DownArrow) ? -5f : Input.GetKey(KeyCode.UpArrow) ? 5f : 0f) * Time.deltaTime);
						pY = Mathf.Clamp(pY, -4f, 4f); 
				break;
				case 2: pX += Mathf.Sign(sX)*30f*Time.deltaTime; 
						sX += Mathf.Sign(sX)*30f*Time.deltaTime;
						if( Mathf.Abs(pX - player.transform.position.x) > 10f )
							destroyed.Add(go);
				break;
				case 4: pY -= pY > -4f ? 0.1f*Time.deltaTime : 0f;
						Collider[] hits = Physics.OverlapBox(go.GetComponent<Collider>().bounds.center, go.GetComponent<Collider>().bounds.extents, go.transform.rotation, (1<<1)+(1<<2) );
						for(int h=0; (hits != null) && (h < hits.Length); h++) {
							if(hits[h].gameObject.layer == 2) {							// Bullet
								explosion.transform.position = go.transform.position;
								explosion.GetComponent<Renderer>().material.color = go.GetComponent<Renderer>().material.color;
								explosion.GetComponent<ParticleSystem>().Emit(20);
								destroyed.Add(go);
								Score(10);
							}
							else if(hits[h].gameObject.layer == 3) {					// Human
								
							}
						}
				break;
			}
			if(pX > player.transform.position.x+40f)
				pX -= 80f;
			else if(pX < player.transform.position.x-40f)
				pX += 80f;
			go.transform.position = new Vector3(pX, pY, go.transform.position.z);
			go.transform.localScale = new Vector3(sX, go.transform.localScale.y, go.transform.localScale.z);
		}
		foreach(GameObject go in destroyed) {
			allObjects.Remove(go);
			Destroy(go);
		}
    }
}
