﻿using UnityEngine;
using UnityEngine.UI;

public class Breakout : MonoBehaviour
{
	static int score = 0;
	static int lives = 2;
	Light dirlight;
    GameObject ball;
    GameObject bat;
	GameObject borderTop;
	GameObject borderLeft;
	GameObject borderRight;
	GameObject[,] bricks = new GameObject[14, 8];
    Vector3 ballVelocity = new Vector3(1f, -1f, 0f);
	float ballSpeed = 1f;
	Canvas uiCanvas;
	Text uiScore;
	int numBricksDead = 0;
	float startTime = 0f;
	bool gameOver = false;
	Vector2 TouchJoy(int t) { return (Input.GetTouch(t).position - new Vector2(Screen.width-(Screen.height/4f), Screen.height/4f)) / (Screen.height/4f); }
	bool Fire { get { for(int t=0; t<Input.touchCount; t++) {if(TouchJoy(t).x<-2f) return true; } return false; } }

	void Start()
	{
		gameObject.GetComponent<Camera>().backgroundColor = Color.black;
		dirlight = new GameObject("Light").AddComponent<Light>();
		dirlight.type = LightType.Directional;  //dirlight.color = Color.green;
		ball = CreateObject("Ball", 0f, 0f, 0.2f, 0.1f, 0, Color.white);
 		bat = CreateObject("Bat", 0f, -4f, 0.6f, 0.2f, 1, Color.cyan);
 		borderTop = CreateObject("BorderTop", ((bricks.GetLength(0)/2) * 0.7f)-5.35f, (bricks.GetLength(1) * 0.25f)+2.5f, bricks.GetLength(0) * 0.7f, 0.2f, 2, Color.white);
 		borderLeft = CreateObject("BorderLeft", -((bricks.GetLength(0)/2) * 0.7f)-0.35f-0.1f, 0f, 0.2f, 10f, 3, Color.white);
 		borderRight = CreateObject("BorderRight", ((bricks.GetLength(0)/2) * 0.7f)-0.35f-0.1f, 0f, 0.2f, 10f, 4, Color.white);
		for (int i = 0; i < bricks.GetLength(0); i++)
			for (int j = 0; j < bricks.GetLength(1); j++) 
			{
 				bricks[i, j] = CreateObject("Brick", (i * 0.7f) - 5, (j * 0.25f) + 1, 0.6f, 0.15f, 5 + (j/2),
				j <= 1 ? Color.yellow : j <= 3 ? Color.green : j <= 5 ? new Color(1f, 0.6f, 0f, 1f) : Color.red);
			}
		uiCanvas = new GameObject("UI").AddComponent<Canvas>();
		uiCanvas.renderMode = RenderMode.WorldSpace;
		uiCanvas.GetComponent<RectTransform>().pivot = new Vector2(0f, 1f);
		uiCanvas.GetComponent<RectTransform>().localPosition = new Vector3(borderLeft.transform.position.x+0.5f, borderTop.transform.position.y-0.2f, 0);
		uiCanvas.GetComponent<RectTransform>().sizeDelta = new Vector2(1000f, 200f);
		uiCanvas.GetComponent<RectTransform>().localScale = new Vector3(0.01f, 0.01f, 1f);
		uiScore = uiCanvas.gameObject.AddComponent<Text>();
		uiScore.font = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;
		uiScore.fontSize = 50;
		startTime = Time.time + 3f;
	}

	GameObject CreateObject(string label, float px, float py, float sx, float sy, int layer, Color color) 
	{
        GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
		go.GetComponent<MeshRenderer>().material = new Material(Shader.Find("Sprites/Default"));
		go.GetComponent<Renderer>().material.color = color;
		go.transform.position = new Vector3(px, py, 0f);
		go.transform.localScale = new Vector3(sx, sy, 0.5f);
		go.layer = layer;
		return go;
	}

	void Update()
	{
		if((Time.time > startTime) && !gameOver)
		{
			int numBricksAlive = (bricks.GetLength(0)*bricks.GetLength(1)) - numBricksDead;
            ball.transform.position = ball.transform.position + (ballVelocity * Time.deltaTime * ballSpeed);
			Collider[] hits = Physics.OverlapBox(ball.GetComponent<Collider>().bounds.center, ball.GetComponent<Collider>().bounds.extents, ball.transform.rotation, ~(1<<0) );
			if((hits != null) && (hits.Length > 0))
            {
                if (hits[0].gameObject.layer == 1)			// Bat
                {
					float bathitX = (hits[0].ClosestPoint(ball.transform.position) - hits[0].gameObject.transform.position).x / hits[0].bounds.extents.x;
                    ballVelocity = new Vector3(bathitX, Mathf.Abs(ballVelocity.y), ballVelocity.z);				// Give upward velocity, left-right velocity based on hit position on bat
                }
				else if(hits[0].gameObject.layer == 2)      // Top border
				{
                    ballVelocity = new Vector3(ballVelocity.x, -Mathf.Abs(ballVelocity.y), ballVelocity.z);		// Give downward velocity
					bat.transform.localScale = new Vector3(0.3f, 0.2f, 0.5f);	// Bat halves in size if hit top
				}
                else if (hits[0].gameObject.layer == 3)     // Left border
                {
                    ballVelocity = new Vector3(Mathf.Abs(ballVelocity.x), ballVelocity.y, ballVelocity.z);		// Give rightward velocity
                }
				else if (hits[0].gameObject.layer == 4)     // Right border
				{
					ballVelocity = new Vector3(-Mathf.Abs(ballVelocity.x), ballVelocity.y, ballVelocity.z);		// Give leftward velocity
				}
				else if (hits[0].gameObject.layer >= 5)     // Brick
				{
					hits[0].gameObject.SetActive(false);														// Destroy brick so can't hit it again
					score += 1 + ((hits[0].gameObject.layer-5) * 2);
					numBricksDead++;
					ballVelocity = new Vector3(ballVelocity.x, -ballVelocity.y, ballVelocity.z);				// Reverse vertical velocity
					ballSpeed = Mathf.Max((hits[0].gameObject.layer>=6)?4f:(numBricksDead >= 12) ? 3f : (numBricksDead >= 4) ? 2f : 1f, ballSpeed);
				}
			}
			if(ball.transform.position.y < -4)
                if (--lives >= 0)
                {
					ball.transform.position = new Vector3(0, 0, 0);
					startTime = Time.time + 3f;
				}
                else
                    gameOver = true;                            												// Player dead = game over
			if (numBricksAlive == 0)
				UnityEngine.SceneManagement.SceneManager.LoadScene("Breakout");									// Reload scene for a new attack wave
		}
		else
		{
			if(Input.GetKeyDown(KeyCode.Space) || Fire)
			{
				score = 0;
				lives = 2;
				ballSpeed = 1f;
				bat.transform.localScale = new Vector3(0.6f, 0.2f, 0.5f);
				UnityEngine.SceneManagement.SceneManager.LoadScene("Breakout");									// Reload scene and reset score & lives for a new game
			}
		}
		//float newBatX = bat.transform.position.x + (Input.GetAxis("Horizontal") * 30f * Time.deltaTime);
		float newBatX = ((Input.mousePosition.x / Screen.width) * 10f) - 5f;
		bat.transform.position = new Vector3(Mathf.Clamp(newBatX, borderLeft.transform.position.x + 0.4f, borderRight.transform.position.x - 0.4f), -4f, 0f);
		uiScore.text = string.Format("{0}\n  {1:000}         {2}", 3-lives, score, gameOver ? "SPACE TO RESTART":"");
	}
}