using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
public class Asteroids : MonoBehaviour
{
	static int score = 0;
	static int lives = 2;
	static int level = 1;
	GameObject player;
	Rigidbody playerBody;
	GameObject saucer;
	List<GameObject> asteroids = new List<GameObject>();
	GameObject[] bullets = new GameObject[2];       // Bullet 0 is player's, 1 is saucer's
	GameObject[] playerLives = new GameObject[20];	// Max 20 lives
	Vector3[] playershape = { new Vector3(-1f, -1f, 0f), new Vector3(0f, -0.5f, 0f), new Vector3(1f, -1f, 0f), new Vector3(0f, 1f, 0f) };
	Vector3[] bulletshape = { new Vector3(0f, -1f, 0f), new Vector3(0f, 1f, 0f) };
	Vector3[] astAshape = { new Vector3(-1f, -0.4f, 0f), new Vector3(-0.6f, 0f, 0f), new Vector3(-1f, 0.4f, 0f), new Vector3(-0.2f, 0.8f, 0f), new Vector3(0f, 0.6f, 0f), new Vector3(0.2f, 0.8f, 0f), new Vector3(0.6f, 0.4f, 0f), new Vector3(0.4f, 0.2f, 0f), new Vector3(0.8f, 0f, 0f), new Vector3(0.4f, -0.8f, 0f), new Vector3(-0.2f, -0.6f, 0f), new Vector3(-0.3f, -0.7f, 0f) };
	Vector3[] astBshape = { new Vector3(-1f, 0.5f, 0f), new Vector3(-0.2f, 0.5f, 0f), new Vector3(-0.5f, 1f, 0f), new Vector3(0.2f, 1f, 0f), new Vector3(1f, 0.6f, 0f), new Vector3(1f, 0.2f, 0f), new Vector3(0f, 0f, 0f), new Vector3(1f, -0.5f, 0f), new Vector3(0.5f, -0.9f, 0f), new Vector3(0.2f, -0.7f, 0f), new Vector3(-0.5f, -1f, 0f), new Vector3(-0.8f, -0.4f, 0f) };
	Vector3[] astCshape = { new Vector3(-1f, -0.4f, 0f), new Vector3(-1f, 0.4f, 0f), new Vector3(-0.5f, 1f, 0f), new Vector3(0f, 0.6f, 0f), new Vector3(0.5f, 1f, 0f), new Vector3(0.8f, 0.6f, 0f), new Vector3(0.6f, 0.2f, 0f), new Vector3(1f, -0.4f, 0f), new Vector3(0.5f, -1f, 0f), new Vector3(-0.4f, -1f, 0f) };
	Vector3[] saucershape = { new Vector3(-1f, -0.4f, 0f), new Vector3(-0.5f, -0.8f, 0f), new Vector3(0.5f, -0.8f, 0f), new Vector3(1f, -0.4f, 0f), new Vector3(-1f, -0.4f, 0f), new Vector3(-0.4f, 0.1f, 0f), new Vector3(0.4f, 0.1f, 0f), new Vector3(1f, -0.4f, 0f), new Vector3(0.4f, 0.1f, 0f), new Vector3(0.2f, 0.4f, 0f), new Vector3(-0.2f, 0.4f, 0f), new Vector3(-0.4f, 0.1f, 0f) };
	Canvas uiCanvas;
	Text uiScore;
	bool gameOver = false;
	float saucerTime = 0f;
	Vector3[] RandomAsteroidShape { get { float r = Random.value; return r < 0.33f ? astAshape : r < 0.66f ? astBshape : astCshape; } }
	int RandomDirection { get { return (int)(Random.value * 3.9999f); } }
	Vector3 RandomPosition { get { return new Vector3((Random.value * 10f) - 5f, (Random.value * 10f) - 5f, 0); } }
	void Start() {
		gameObject.GetComponent<Camera>().backgroundColor = Color.black;
		player = CreateVectorObject("Player", playershape, Vector3.zero, 0.2f, 1);
		playerBody = player.AddComponent<Rigidbody>();
		playerBody.useGravity = false;
		playerBody.constraints = RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
		playerBody.drag = 0.5f;
		saucer = CreateVectorObject("Saucer", saucershape, Vector3.zero, 0.3f, 5, false);
		saucerTime = Time.time + 15f;// + Random.value * 30f;
		for (int i = 0; i < (2 + level * 2); i++) {
			GameObject asteroid = CreateVectorObject(RandomDirection.ToString(), RandomAsteroidShape, RandomPosition, 0.4f, 2);	// 0.4 = Large size asteroids
			asteroids.Add(asteroid);
		}
		bullets[0] = CreateVectorObject("PlayerBullet", bulletshape, Vector3.zero, 0.1f, 6, false);   // Bullet 0 is player bullet
		bullets[1] = CreateVectorObject("SaucerBullet", bulletshape, Vector3.zero, 0.1f, 7, false);   // Bullet 1 is saucer bullet
		for (int i = 0; i < playerLives.Length; i++) {
			playerLives[i] = CreateVectorObject("Life", playershape, new Vector3(-6 + (i * 0.4f), 4, 0), 0.2f, 0, (i <= lives));
		}
		uiCanvas = new GameObject("UI").AddComponent<Canvas>();
		uiCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
		uiScore = uiCanvas.gameObject.AddComponent<Text>();
		uiScore.font = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;
		uiScore.fontSize = 50;
	}
	GameObject CreateVectorObject(string label, Vector3[] shape, Vector3 pos, float radius, int layer, bool active=true) {
		GameObject go = new GameObject(label);
		go.SetActive(active);
		go.layer = layer;
		go.transform.position = pos;
		go.transform.localScale = new Vector3(radius, radius, radius);
		go.AddComponent<SphereCollider>().radius = 1f;
		LineRenderer line = go.AddComponent<LineRenderer>();
		line.useWorldSpace = false;
		line.widthMultiplier = 0.02f;
		line.material = new Material(Shader.Find("Sprites/Default"));
		line.positionCount = shape.Length;
		line.loop = true;
		line.SetPositions(shape);
		return go;
	}
	void KillPlayer() {
		playerLives[lives].SetActive(false);
		player.transform.position = RandomPosition;
		playerBody.velocity = Vector3.zero;
		if (--lives < 0) {
			player.SetActive(false);
			gameOver = true;                            // Player dead = game over
		}
	}
	void KillSaucer() {
		saucer.SetActive(false);
		saucerTime = Time.time + 5f + Random.value * 10f;
	}
	void Score(int add) {
		lives = (score / 10000) < ((score+add) / 10000) ? ((lives<(playerLives.Length-1)) ?lives+1:lives) : lives;
		playerLives[lives].SetActive(true);
		score += add;
	}
	void Update() {
		uiScore.text = string.Format("{0:00000}{1}", score, gameOver ? "\n\n\n\n                        GAME OVER" : "");
		if(gameOver) {
			if (Input.GetKeyDown(KeyCode.Space)) {
				score = 0;
				lives = 2;
				level = 1;
				UnityEngine.SceneManagement.SceneManager.LoadScene("Asteroids");      // Reload scene and reset score, lives & level for a new game
			}
			return;
		}
		for (int i = 0; i < asteroids.Count; i++)
			if (asteroids[i].activeSelf) {
				int dir = int.Parse(asteroids[i].name);	// Name = direction to travel out of 4 possible diagonals (0-3)
				float speed = asteroids[i].layer == 2 ? 0.5f : asteroids[i].layer == 3 ? 0.75f : 1f;
				float newX = asteroids[i].transform.position.x + ((dir % 2 == 0) ? speed : -speed) * Time.deltaTime;
				float newY = asteroids[i].transform.position.y + ((dir >= 2) ? speed : -speed) * Time.deltaTime;
				newX = Mathf.Abs(newX) < 5f ? newX : -newX;
				newY = Mathf.Abs(newY) < 5f ? newY : -newY;
				asteroids[i].transform.position = new Vector3(newX, newY, asteroids[i].transform.position.z);
				Collider[] hits = Physics.OverlapBox(asteroids[i].GetComponent<Collider>().bounds.center, asteroids[i].GetComponent<Collider>().bounds.extents, asteroids[i].transform.rotation, (1 << 1)+(1 << 5));
				if ((hits != null) && (hits.Length > 0)) {
					if (hits[0].gameObject.layer == 1)
						KillPlayer();
					else if (hits[0].gameObject.layer == 5)
						KillSaucer();
				}
			}
		for (int i = 0; i < bullets.Length; i++)
			if (bullets[i].activeSelf) {
				bullets[i].transform.position = bullets[i].transform.position + bullets[i].transform.up * ((i == 0) ? 20f : 5f) * Time.deltaTime;
				Collider[] hits = Physics.OverlapBox(bullets[i].GetComponent<Collider>().bounds.center, bullets[i].GetComponent<Collider>().bounds.extents, bullets[i].transform.rotation, (i == 0) ? (1 << 2) + (1 << 3) + (1 << 4) + (1<<5) : (1 << 1) + (1 << 2) + (1 << 3) + (1 << 4));
				if ((hits != null) && (hits.Length > 0)) {
					bullets[i].SetActive(false);
					if (hits[0].gameObject.layer == 5) {
						Score(1000);
						KillSaucer();
					}
					else if (hits[0].gameObject.layer >= 2) {
						if (hits[0].gameObject.layer < 4)
							for (int p = 0; p < 2; p++) {
								GameObject asteroid = CreateVectorObject(RandomDirection.ToString(), RandomAsteroidShape, hits[0].gameObject.transform.position, hits[0].gameObject.layer == 2 ? 0.2f : 0.1f, hits[0].gameObject.layer == 2 ? 3 : 4);	// 0.2 = medium size, 0.1 = small asteroids
								asteroids.Add(asteroid);
							}
						Score( hits[0].gameObject.layer == 2 ? 20 : hits[0].gameObject.layer == 3 ? 50 : 100 );
						asteroids.Remove(hits[0].gameObject);
						Destroy(hits[0].gameObject);
					}
					else if (hits[0].gameObject.layer == 1) {
						KillPlayer();
						saucerTime = saucer.activeSelf ? Time.time + (saucer.transform.localScale.x > 0.2f ? 1f : 3f) : saucerTime;  // Delay before firing at player. Small saucer waits longer as more accurate.
					}
				}
				if ((bullets[i].transform.position.x < -5) || (bullets[i].transform.position.x > 5) || (bullets[i].transform.position.y < -5) || (bullets[i].transform.position.y > 5))
					bullets[i].SetActive(false);
			}
		if (saucer.activeSelf) {
			saucer.transform.position = saucer.transform.position + saucer.transform.right * Time.deltaTime * (saucer.transform.position.y > 0f ? -1f : 1f);
			if (!bullets[1].activeSelf && (Time.time > saucerTime)) {
				bullets[1].transform.LookAt( (bullets[1].transform.position + Vector3.forward), Vector3.Normalize(player.transform.position - saucer.transform.position) );
				bullets[1].transform.Rotate(0f, 0f, (saucer.transform.localScale.x > 0.2f ? 10f : 3f) * (Mathf.RoundToInt(Random.value) == 0 ? 1f : -1f) );					// Higher angle = less accuracy
				bullets[1].transform.position = saucer.transform.position + bullets[1].transform.up * 0.4f;
				bullets[1].SetActive(true);                                 // Fire a saucer bullet
			}
			if ((saucer.transform.position.x < -5) || (saucer.transform.position.x > 5) || (saucer.transform.position.y < -5) || (saucer.transform.position.y > 5))
				KillSaucer();
		}
		else if (Time.time > saucerTime) {
			saucer.transform.position = Random.value < 0.5 ? new Vector3(4.9f, 3f, 0f) : new Vector3(-4.9f, -3f, 0f);
			saucer.transform.localScale = (score > (Random.value * 40000)) ? new Vector3(0.15f, 0.15f, 0.15f) : new Vector3(0.3f, 0.3f, 0.3f);
			saucer.SetActive(true);
			saucerTime = Time.time + (saucer.transform.localScale.x > 0.2f ? 1f : 3f);	// Delay before firing at player. Small saucer waits longer as more accurate.
		}
		playerBody.AddForce( Input.GetKeyDown(KeyCode.UpArrow) ? (player.transform.up * 5000f * Time.deltaTime) : Vector3.zero );
		float playerX = Mathf.Abs(player.transform.position.x) < 5f ? player.transform.position.x : Mathf.Clamp( -player.transform.position.x, -5f, 5f);
		float playerY = Mathf.Abs(player.transform.position.y) < 5f ? player.transform.position.y : Mathf.Clamp( -player.transform.position.y, -5f, 5f);
		player.transform.position = new Vector3(playerX, playerY, player.transform.position.z);
		player.transform.Rotate( 0f, 0f, -Input.GetAxis("Horizontal") * 500f * Time.deltaTime );
		if (Input.GetKeyDown(KeyCode.LeftShift) && !bullets[0].activeSelf) {
			bullets[0].transform.position = player.transform.position + player.transform.up * 0.6f;
			bullets[0].transform.rotation = player.transform.rotation;
			bullets[0].SetActive(true);                                 // Fire a player bullet
		}
		if (asteroids.Count == 0) {
			level++;
			UnityEngine.SceneManagement.SceneManager.LoadScene("Asteroids");      // Reload scene for a new attack wave
		}
	}
}