using UnityEngine;
using System.Collections;

public class ObstacleScript : MonoBehaviour {
	
	public GameControlScript control;
	public static float objectSpeed = 20f;
	
	
	void Update () {
		
		transform.Translate(new Vector3(0, -objectSpeed, 0)*Time.deltaTime);
		
	}
}