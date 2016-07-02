using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Invaders : MonoBehaviour
{
	Light dirlight;
	GameObject player;
	GameObject[,] invaders = new GameObject[10, 5];
	GameObject[] bullets = new GameObject[4];		// Bullet 0 is player's, rest are invader's
	Canvas uiCanvas;
	Text uiScore;
	bool invadersMovingLeft = true;
	bool invadersMovingDown = false;
	int score = 0;
	bool gameOver = false;

	void Start()
	{
		gameObject.GetComponent<Camera>().backgroundColor = Color.black;
		dirlight = new GameObject("Light").AddComponent<Light>();
		dirlight.type = LightType.Directional;	//dirlight.color = Color.green;
		player = GameObject.CreatePrimitive(PrimitiveType.Cube);
		player.transform.position = new Vector3(0, -4, 0);
		player.transform.localScale = new Vector3(0.75f, 0.5f, 0.5f);
		player.layer = 1;
		for(int i=0; i<invaders.GetLength(0); i++)
			for(int j=0; j<invaders.GetLength(1); j++)
			{
				invaders[i,j] = GameObject.CreatePrimitive(PrimitiveType.Cube);
				invaders[i,j].transform.position = new Vector3(i-5, (j*0.5f)+1, 0);
				invaders[i,j].transform.localScale = new Vector3((j==(invaders.GetLength(1)-1))?0.4f:0.6f, 0.4f, 0.5f);	// Top row are smaller and harder to hit
				invaders[i,j].layer = 2;
			}
		for(int b=0; b<4; b++)
			for(int i=0; i<5; i++)
				for(int j=0; j<4; j++)
				{
					GameObject baseBrick = GameObject.CreatePrimitive(PrimitiveType.Cube);
					baseBrick.transform.position = new Vector3((b*2.2f)+(i*0.2f)-3.6f, (j*0.2f)-3, 0);
					baseBrick.transform.localScale = new Vector3(0.2f, 0.2f, 0.5f);
					baseBrick.layer = 3;
				}
		for(int i=0; i<bullets.Length; i++)
		{
			bullets[i] = GameObject.CreatePrimitive(PrimitiveType.Cube);
			bullets[i].transform.localScale = new Vector3(0.1f, 0.5f, 0.5f);
			bullets[i].SetActive(false);
			bullets[i].layer = (i==0)?1:2;	// Bullet 0 is player bullet
		}
		uiCanvas = new GameObject("UI").AddComponent<Canvas>();
		uiCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
		uiScore = uiCanvas.gameObject.AddComponent<Text>();
		uiScore.font = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;
		uiScore.fontSize = 50;
	}

	void Update()
	{
		if(!gameOver)
		{
			bool moveLeftThisUpdate = invadersMovingLeft;
			bool moveDownThisUpdate = invadersMovingDown;
			invadersMovingDown = false;
			for(int i=0; i<invaders.GetLength(0); i++)
				for(int j=0; j<invaders.GetLength(1); j++)
				{
					if(!invaders[i,j].activeSelf)
						continue;
					Vector3 newPos = invaders[i,j].transform.position;
					if(moveDownThisUpdate)
					{
						newPos.y -= 0.25f;
						if(newPos.y < -4f)
							gameOver = true;
					}
					if(moveLeftThisUpdate)
					{
						newPos.x -= 0.5f * Time.deltaTime;
						if(newPos.x < -7.0f)
						{
							invadersMovingLeft = false;
							invadersMovingDown = true;
						}
					}
					else
					{
						newPos.x += 0.5f * Time.deltaTime;
						if(newPos.x > 7.0f)
						{
							invadersMovingLeft = true;
							invadersMovingDown = true;
						}
					}
					invaders[i,j].transform.position = newPos;
					if( !bullets[(i%(bullets.Length-1))+1].activeSelf && ((j == 0) || !invaders[i,j-1].activeSelf) && (Random.value < 0.01f) )
					{
						bullets[(i%(bullets.Length-1))+1].transform.position = invaders[i,j].transform.position - (Vector3.up*0.5f);
						bullets[(i%(bullets.Length-1))+1].SetActive(true);
					}
				}

			for(int i=0; i<bullets.Length; i++)
			{
				if(bullets[i].activeSelf)
				{
					Vector2 bulletPos = bullets[i].transform.position;
					bulletPos.y += ((i==0)?20f:-5f) * Time.deltaTime;
					bullets[i].transform.position = bulletPos;

					Collider[] hits = Physics.OverlapBox(bullets[i].GetComponent<Collider>().bounds.center, bullets[i].GetComponent<Collider>().bounds.extents, bullets[i].transform.rotation, ((i==0)?1<<2:1<<1)+(1<<3));
					if((hits != null) && (hits.Length > 0))
					{
						bullets[i].SetActive(false);
						hits[0].gameObject.SetActive(false);
						if(hits[0].gameObject.layer == 2)
							score+=10;
						else if(hits[0].gameObject.layer == 1)
							gameOver = true;
					}

					if((bulletPos.y < -4) || (bulletPos.y > 4))
						bullets[i].SetActive(false);
				}
			}
			float xPos = player.transform.position.x + (Input.GetAxis("Horizontal") * 10f * Time.deltaTime);
			Vector3 playerPos = new Vector3(Mathf.Clamp(xPos, -7f, 7f), -4f, 0f);
			player.transform.position = playerPos;

			if(Input.GetKeyDown(KeyCode.LeftShift) && !bullets[0].activeSelf)
			{
				bullets[0].transform.position = player.transform.position;
				bullets[0].SetActive(true);
			}
		}
		else
		{
			if(Input.GetKeyDown(KeyCode.Space))
				UnityEngine.SceneManagement.SceneManager.LoadScene(0);
		}
		uiScore.text = string.Format("SCORE: {0:0000} {1}", score, gameOver?"GAME OVER!":"");
	}
}
