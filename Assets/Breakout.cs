using UnityEngine;
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

	void Start()
	{
		gameObject.GetComponent<Camera>().backgroundColor = Color.black;
		dirlight = new GameObject("Light").AddComponent<Light>();
		dirlight.type = LightType.Directional;  //dirlight.color = Color.green;
        ball = GameObject.CreatePrimitive(PrimitiveType.Cube);
        ball.transform.position = new Vector3(0, 0, 0);
        ball.transform.localScale = new Vector3(0.2f, 0.1f, 0.5f);
        ball.layer = 0;
        bat = GameObject.CreatePrimitive(PrimitiveType.Cube);
		bat.transform.position = new Vector3(0, -4, 0);
		bat.transform.localScale = new Vector3(0.6f, 0.2f, 0.5f);
		bat.layer = 1;
        borderTop = GameObject.CreatePrimitive(PrimitiveType.Cube);
        borderTop.transform.position = new Vector3( ((bricks.GetLength(0)/2) * 0.7f)-5.35f, (bricks.GetLength(1) * 0.25f)+2.5f, 0);
        borderTop.transform.localScale = new Vector3( bricks.GetLength(0) * 0.7f, 0.2f, 0.5f);
        borderTop.layer = 2;
        borderLeft = GameObject.CreatePrimitive(PrimitiveType.Cube);
        borderLeft.transform.position = new Vector3(-((bricks.GetLength(0)/2) * 0.7f)-0.35f-0.1f, 0, 0);
        borderLeft.transform.localScale = new Vector3(0.2f, 10f, 0.5f);
        borderLeft.layer = 3;
        borderRight = GameObject.CreatePrimitive(PrimitiveType.Cube);
        borderRight.transform.position = new Vector3(((bricks.GetLength(0)/2) * 0.7f)-0.35f-0.1f, 0, 0);
        borderRight.transform.localScale = new Vector3(0.2f, 10f, 0.5f);
        borderRight.layer = 4;
		for (int i = 0; i < bricks.GetLength(0); i++)
			for (int j = 0; j < bricks.GetLength(1); j++)
			{
				bricks[i, j] = GameObject.CreatePrimitive(PrimitiveType.Cube);
				bricks[i, j].transform.position = new Vector3((i * 0.7f) - 5, (j * 0.25f) + 1, 0);
				bricks[i, j].transform.localScale = new Vector3(0.6f, 0.15f, 0.5f);
				bricks[i, j].layer = 5 + (j/2);
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
                    ballVelocity = new Vector3(ballVelocity.x, Mathf.Abs(ballVelocity.y), ballVelocity.z);		// Give upward velocity
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
			if(Input.GetKeyDown(KeyCode.Space))
			{
				score = 0;
				lives = 2;
				ballSpeed = 1f;
				bat.transform.localScale = new Vector3(0.6f, 0.2f, 0.5f);
				UnityEngine.SceneManagement.SceneManager.LoadScene("Breakout");									// Reload scene and reset score & lives for a new game
			}
		}
		float newBatX = bat.transform.position.x + (Input.GetAxis("Horizontal") * 20f * Time.deltaTime);
		bat.transform.position = new Vector3(Mathf.Clamp(newBatX, borderLeft.transform.position.x + 0.4f, borderRight.transform.position.x - 0.4f), -4f, 0f);
		uiScore.text = string.Format("{0}\n  {1:000}         {2}", 3-lives, score, gameOver ? "SPACE TO RESTART":"");
	}
}