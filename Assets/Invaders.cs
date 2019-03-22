﻿using UnityEngine;
using UnityEngine.UI;

public class Invaders : MonoBehaviour
{
	static int score = 0;
	static int lives = 2;
	Light dirlight;
	GameObject player;
	GameObject[,] invaders = new GameObject[10, 5];
	GameObject[] bullets = new GameObject[4];		// Bullet 0 is player's, rest are invader's
	GameObject[] playerLives = new GameObject[2];
	Canvas uiCanvas;
	Text uiScore;
	bool invadersMovingLeft = true;
	bool invadersMovingDown = false;
	int numInvadersDead = 0;
	bool gameOver = false;
	Vector2 TouchJoy(int t) { return (Input.GetTouch(t).position - new Vector2(Screen.width-(Screen.height/4f), Screen.height/4f)) / (Screen.height/4f); }
	Vector2 Joystick { get { for(int t=0; t<Input.touchCount; t++) {if(TouchJoy(t).magnitude<2f) return TouchJoy(t);} return Vector2.zero; } }
	bool Fire { get { for(int t=0; t<Input.touchCount; t++) {if(TouchJoy(t).x<-2f) return true; } return false; } }

	void Start()
	{
		gameObject.GetComponent<Camera>().backgroundColor = Color.black;
		dirlight = new GameObject("Light").AddComponent<Light>();
		dirlight.type = LightType.Directional;	//dirlight.color = Color.green;
		player = CreateObject("Player", -7, -4, 0.75f, 0.5f, 1);
		for(int i=0; i<invaders.GetLength(0); i++)
			for(int j=0; j<invaders.GetLength(1); j++)
			{
				invaders[i,j] = CreateObject("Invader", i-5, (j*0.5f)+1, (j==(invaders.GetLength(1)-1))?0.4f:0.6f, 0.4f, 2);	// Top row are smaller and harder to hit
			}
		for(int b=0; b<4; b++)
			for(int i=0; i<5; i++)
				for(int j=0; j<4; j++)
				{
					CreateObject("BaseBrick", (b*2.2f)+(i*0.2f)-3.6f, (j*0.2f)-3, 0.2f, 0.2f, 3);
				}
		for(int i=0; i<bullets.Length; i++)
		{
			bullets[i] = CreateObject("Bullet", 0f, 0f, 0.1f, 0.5f, (i==0)?4:5);	// Bullet 0 is player bullet
		}
		for(int i=0; (i<playerLives.Length)&&(i<lives); i++)
		{
			playerLives[i] = CreateObject("Life", -6+(i*1f), -5, 0.75f, 0.5f, 0);
		}
		uiCanvas = new GameObject("UI").AddComponent<Canvas>();
		uiCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
		uiScore = uiCanvas.gameObject.AddComponent<Text>();
		uiScore.font = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;
		uiScore.fontSize = 50;
	}

	GameObject CreateObject(string label, float px, float py, float sx, float sy, int layer) 
	{
        GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
		go.GetComponent<MeshRenderer>().material = new Material(Shader.Find("Sprites/Default"));
		go.GetComponent<Renderer>().material.color = Color.white;
		go.transform.position = new Vector3(px, py, 0f);
		go.transform.localScale = new Vector3(sx, sy, 0.5f);
		go.layer = layer;
		return go;
	}

	void Update()
	{
		if(!gameOver)
		{
			bool moveLeftThisUpdate = invadersMovingLeft;
			bool moveDownThisUpdate = invadersMovingDown;
			invadersMovingDown = false;
			int numInvadersAlive = (invaders.GetLength(0)*invaders.GetLength(1)) - numInvadersDead;
			float invaderSpeed = 0.3f+((numInvadersDead/10)*0.1f)+((numInvadersAlive<=3)?((4-numInvadersAlive)*1f):0.0f);
			for(int i=0; i<invaders.GetLength(0); i++)
				for(int j=0; j<invaders.GetLength(1); j++)
					if(invaders[i,j].activeSelf)
					{
						Vector3 newPos = invaders[i,j].transform.position;
						if(moveDownThisUpdate)
							newPos.y -= 0.25f;
						if(newPos.y < -2f)
							gameOver = true;								// Invaders reached bases = game over
						newPos.x -= (moveLeftThisUpdate?invaderSpeed:-invaderSpeed) * Time.deltaTime;
						if( (invadersMovingLeft&&(newPos.x < -7.0f)) || ((!invadersMovingLeft)&&(newPos.x > 7.0f)) )
						{
							invadersMovingLeft = !invadersMovingLeft;
							invadersMovingDown = true;
						}
						invaders[i,j].transform.position = newPos;
						if( !bullets[(i%(bullets.Length-1))+1].activeSelf && ((j == 0) || !invaders[i,j-1].activeSelf) && (Random.value < 0.01f) )
						{
							bullets[(i%(bullets.Length-1))+1].transform.position = invaders[i,j].transform.position - (Vector3.up*0.5f);
							bullets[(i%(bullets.Length-1))+1].SetActive(true);
						}
					}
			for(int i=0; i<bullets.Length; i++)
				if(bullets[i].activeSelf)
				{
					float newBulletY = bullets[i].transform.position.y + ((i==0)?20f:-5f) * Time.deltaTime;
					bullets[i].transform.position = new Vector3(bullets[i].transform.position.x, newBulletY, bullets[i].transform.position.z);
					Collider[] hits = Physics.OverlapBox(bullets[i].GetComponent<Collider>().bounds.center, bullets[i].GetComponent<Collider>().bounds.extents, bullets[i].transform.rotation, ((i==0)?(1<<2)+(1<<5):(1<<1)+(1<<4))+(1<<3));
					if((hits != null) && (hits.Length > 0))
					{
						bullets[i].SetActive(false);
						hits[0].gameObject.SetActive(false);
						if(hits[0].gameObject.layer == 2)
						{
							score+=10;
							numInvadersDead++;
						}
						else if(hits[0].gameObject.layer == 1)
						{
							if(--lives >= 0)
							{
								playerLives[lives].SetActive(false);
								player.SetActive(true);
								player.transform.position = new Vector3(-7, -4, 0);
							}
							else
								gameOver = true;							// Player dead = game over
						}
					}
					if((bullets[i].transform.position.y < -4) || (bullets[i].transform.position.y > 4))
						bullets[i].SetActive(false);
				}
			float newPlayerX = player.transform.position.x + ((Input.GetAxis("Horizontal")+Joystick.x) * 10f * Time.deltaTime);
			player.transform.position = new Vector3(Mathf.Clamp(newPlayerX, -7f, 7f), -4f, 0f);
			if( (Input.GetKeyDown(KeyCode.LeftShift)||Fire) && !bullets[0].activeSelf)
			{
				bullets[0].transform.position = player.transform.position;
				bullets[0].SetActive(true);									// Fire a player bullet
			}
			if(numInvadersAlive == 0)
				UnityEngine.SceneManagement.SceneManager.LoadScene(0);		// Reload scene for a new attack wave
		}
		else
		{
			if(Input.GetKeyDown(KeyCode.Space))
			{
				score = 0;
				lives = 2;
				UnityEngine.SceneManagement.SceneManager.LoadScene(0);		// Reload scene and reset score & lives for a new game
			}
		}
		uiScore.text = string.Format("SCORE: {0:0000}         {1}", score, gameOver?"GAME OVER!\n   PRESS SPACE TO RESTART":"");
	}
}