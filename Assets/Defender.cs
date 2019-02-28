using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Defender : MonoBehaviour
{
	static int score = 0;
	static int lives = 2;
	static int level = 1;
	GameObject player;
	List<GameObject> allObjects = new List<GameObject>();
	Vector3[] playerVs = { new Vector3(-0.3f, 0f, 0f), new Vector3(0.3f, 0f, 0f), new Vector3(0f, 0.1f, 0f), new Vector3(-0.3f, 0.1f, 0f), new Vector3(-0.3f, 0f, 0f) };
	Vector3[] bulletVs = { new Vector3(0f, 0f, 0f), new Vector3(1f, 0f, 0f) };
	Vector3[] terrainVs = { new Vector3(-20f, 0f, 0f), new Vector3(-18f, 2f, 0f), new Vector3(-15f, 0f, 0f), new Vector3(-12f, 0f, 0f), new Vector3(-9f, 2f, 0f), new Vector3(-6f, 0f, 0f), new Vector3(-3f, 0f, 0f), new Vector3(-2f, 1f, 0f), new Vector3(-1f, 0f, 0f), new Vector3(2f, 0f, 0f), new Vector3(3f, 1f, 0f), new Vector3(4f, 0f, 0f), new Vector3(5f, 0f, 0f), new Vector3(8f, 3f, 0f), new Vector3(11f, 0f, 0f), new Vector3(15f, 0f, 0f), new Vector3(16.5f, 0.5f, 0f), new Vector3(17f, 0.3f, 0f), new Vector3(17.5f, 0.8f, 0f), new Vector3(18f, 0.6f, 0f), new Vector3(18.5f, 1.1f, 0f), new Vector3(19.5f, 0f, 0f), new Vector3(20f, 0f, 0f) };
	Vector3[] sqrVs = { new Vector3(-0.1f, -0.1f, 0f), new Vector3(-0.1f, 0.1f, 0f), new Vector3(0.1f, 0.1f, 0f), new Vector3(0.1f, -0.1f, 0f), new Vector3(-0.1f, -0.1f, 0f) };
	float camOffset = 0f;
    void Start()
    {
		gameObject.GetComponent<Camera>().backgroundColor = Color.black;
		allObjects.Add( player = CreateVectorObject("Player", playerVs, 0f, 0f, 0f, 1f, 1f, 1f, 1, Color.white) );
        allObjects.Add( CreateVectorObject("Terrain1", terrainVs, -10f, -4f, 0f, 1f, 1f, 1f, 0, Color.red) );
        allObjects.Add( CreateVectorObject("Terrain2", terrainVs, -10f+40f, -4f, 0f, 1f, 1f, 1f, 0, Color.red) );
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
		GameObject go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
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

    void Update()
    {
		List<GameObject> destroyed = new List<GameObject>();
		if( Input.GetKeyDown(KeyCode.Space) )
        	allObjects.Add( CreateVectorObject("Bullet", bulletVs, player.transform.position.x+Mathf.Sign(player.transform.localScale.x)*0.3f, player.transform.position.y, player.transform.position.z, Mathf.Sign(player.transform.localScale.x), 1f, 1f, 2, Color.yellow) );
		camOffset = Mathf.Clamp(camOffset+Mathf.Sign(player.transform.localScale.x)*10f*Time.deltaTime, -4.5f, 4.5f);
		gameObject.transform.position = new Vector3(player.transform.position.x+camOffset, gameObject.transform.position.y, gameObject.transform.position.z);
		foreach(GameObject go in allObjects) {
			float pX = go.transform.position.x;
			float pY = go.transform.position.y;
			float sX = go.transform.localScale.x;
			bool destroy = false;
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
			}
			if(pX > player.transform.position.x+40f)
				pX -= 80f;
			else if(pX < player.transform.position.x-40f)
				pX += 80f;
			go.transform.position = new Vector3(pX, pY, go.transform.position.z);
			go.transform.localScale = new Vector3(sX, go.transform.localScale.y, go.transform.localScale.z);
			if(destroy) {
				allObjects.Remove(go);
				Destroy(go);
			}
		}
		foreach(GameObject go in destroyed) {
			allObjects.Remove(go);
			Destroy(go);
		}
    }
}
