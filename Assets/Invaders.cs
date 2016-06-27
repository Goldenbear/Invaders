using UnityEngine;
using System.Collections;

public class Invaders : MonoBehaviour
{
	Light dirlight;
	GameObject[,] invaders = new GameObject[10, 5];
	GameObject[] invaderBullets = new GameObject[10];
	GameObject player;
	GameObject playerBullet;
	bool invadersMovingLeft = true;
	bool invadersMovingDown = false;
	int score = 0;
	bool gameOver = false;

	void Start()
	{
		gameObject.GetComponent<Camera>().backgroundColor = Color.black;
		dirlight = new GameObject("Light").AddComponent<Light>();
		dirlight.type = LightType.Directional;	//dirlight.color = Color.green;
		for(int i=0; i<invaders.GetLength(0); i++)
			for(int j=0; j<invaders.GetLength(1); j++)
			{
				invaders[i,j] = GameObject.CreatePrimitive(PrimitiveType.Cube);
				invaders[i,j].transform.position = new Vector3(i-5, j, 0);
				invaders[i,j].transform.localScale = new Vector3(0.75f, 0.5f, 0.5f);
				invaders[i,j].layer = 2;
			}
		for(int i=0; i<invaderBullets.Length; i++)
		{
			invaderBullets[i] = GameObject.CreatePrimitive(PrimitiveType.Cube);
			invaderBullets[i].transform.localScale = new Vector3(0.1f, 0.5f, 0.5f);
			invaderBullets[i].SetActive(false);
			invaderBullets[i].layer = 2;
		}
		player = GameObject.CreatePrimitive(PrimitiveType.Cube);
		player.transform.position = new Vector3(0, -4, 0);
		player.transform.localScale = new Vector3(0.75f, 0.5f, 0.5f);
		player.layer = 1;
		playerBullet = GameObject.CreatePrimitive(PrimitiveType.Cube);
		playerBullet.transform.localScale = new Vector3(0.1f, 0.5f, 0.5f);
		playerBullet.SetActive(false);
		playerBullet.layer = 1;
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
				}

			if(playerBullet.activeSelf)
			{
				Vector2 bulletPos = playerBullet.transform.position;
				bulletPos.y += 20f * Time.deltaTime;
				playerBullet.transform.position = bulletPos;

				Collider[] hits = Physics.OverlapBox(playerBullet.GetComponent<Collider>().bounds.center, playerBullet.GetComponent<Collider>().bounds.extents, playerBullet.transform.rotation, 1<<2);
				if((hits != null) && (hits.Length > 0))
				{
					playerBullet.SetActive(false);
					hits[0].gameObject.SetActive(false);
					score++;
				}

				if(bulletPos.y > 4)
					playerBullet.SetActive(false);
			}
				
			float xPos = player.transform.position.x + (Input.GetAxis("Horizontal") * 10f * Time.deltaTime);
			Vector3 playerPos = new Vector3(Mathf.Clamp(xPos, -7f, 7f), -4f, 0f);
			player.transform.position = playerPos;

			if(Input.GetButtonDown("Fire1") && !playerBullet.activeSelf)
			{
				playerBullet.transform.position = player.transform.position;
				playerBullet.SetActive(true);
			}
		}
	}
}
