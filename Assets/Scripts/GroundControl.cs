using UnityEngine;
using System.Collections;

public class GroundControl : MonoBehaviour {
	//Material texture offset rate
	public static float speed = .5f;

	// Use this for initialization
	void Start () {
		
	}

	//Offset the material texture at a constant rate
	void Update () {
		float offset = Time.time * speed;                             
		renderer.material.mainTextureOffset = new Vector2(0, -offset); 
	}

}
