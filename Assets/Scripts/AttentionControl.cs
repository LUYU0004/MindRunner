using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AttentionControl : MonoBehaviour {
	public GameObject character;
	public GameObject ground;
	public GameObject wall1;
	public GameObject wall2;
	public Texture attentionBarFilled;
	public Texture attentionBarEmpty;
	public bool feedback;

	private EmotivController emotiv;
	private List<double> attentionValues;
	private double attentionValue = 0;

	public bool isAttentive() {
		return (attentionValue < PlayerInfo.attentionThreshold);
	}
	
	public bool isSemiAttentive() {
		return (attentionValue < PlayerInfo.attentionThreshold + (PlayerInfo.relaxationThreshold - PlayerInfo.attentionThreshold)/2 && attentionValue > PlayerInfo.attentionThreshold);
	}
	
	public bool isSemiRelaxed() {
		return (attentionValue > PlayerInfo.attentionThreshold + (PlayerInfo.relaxationThreshold - PlayerInfo.attentionThreshold)/2 && attentionValue < PlayerInfo.relaxationThreshold);
	}
	
	public bool isRelaxed() {
		return (attentionValue > PlayerInfo.relaxationThreshold);
	}

	// Use this for initialization
	void Start () {
		emotiv = new EmotivController ();
		attentionValues = new List<double>();
		attentionValue = 0;
		StartCoroutine (UpdateAttention());
	}
	
	// Update is called once per frame
	void Update () {
	/*
		if (isAttentive()) {
			gameObject.GetComponent<SpawnScript>().spawnCycle = 0.5f;
			wall1.GetComponent<GroundControl>().speed = 0.5f;
			wall2.GetComponent<GroundControl>().speed = 0.5f;
			ground.GetComponent<GroundControl>().speed = 0.5f;
			character.GetComponent<PlayerControl>().speed = 6.0f;
			character.GetComponent<PlayerControl>().jumpSpeed = 8.0f;
			character.GetComponent<PlayerControl>().gravity = 20.0f;
			character.GetComponent<Animation>().Play("run");
			ObstacleScript.objectSpeed = 20f;
			PowerupScript.objectSpeed = 20f;
		} else {
			gameObject.GetComponent<SpawnScript>().spawnCycle = 0.0f;
			wall1.GetComponent<GroundControl>().speed = 0.0f;
			wall2.GetComponent<GroundControl>().speed = 0.0f;
			ground.GetComponent<GroundControl>().speed = 0.0f;
			character.GetComponent<PlayerControl>().speed = 0.0f;
			character.GetComponent<PlayerControl>().jumpSpeed = 0.0f;
			character.GetComponent<PlayerControl>().gravity = 0.0f;
			character.GetComponent<Animation>().Stop();
			ObstacleScript.objectSpeed = 0f;
			PowerupScript.objectSpeed = 0f;
		}
	*/
		float modifier = 0;
		Animation animation = character.GetComponent<Animation>();
	/*
		if (isAttentive()) {
			modifier = 1.0f;
			animation["run"].speed = 1.0f * modifier;
		} else if (isSemiAttentive()) {
			modifier = 2.0f/3.0f;
			animation["run"].speed = 1.0f * modifier;
		} else if (isSemiRelaxed()) {
			modifier = 1.0f/3.0f;
			animation["run"].speed = 1.0f * modifier;
		} else if (isRelaxed()) {
			modifier = 0.0f;
			animation.Stop();
		}
	*/
		if (!isRelaxed()) {
			modifier = (float)((PlayerInfo.relaxationThreshold - attentionValue) / (PlayerInfo.relaxationThreshold - PlayerInfo.attentionThreshold));
			modifier = (modifier > 1.0f) ? 1 : modifier;
			animation["run"].speed = 1.0f * modifier;
		} else {
			modifier = 0.0f;
			animation.Stop();
		}
		SpawnScript.spawnCycle = 0.5f * 1.0f/modifier;
		GroundControl.speed = 0.5f * modifier;
		PlayerControl.speed = 6.0f * modifier;
		PlayerControl.jumpSpeed = 8.0f * modifier;
		PlayerControl.gravity = 20.0f * modifier;
		ObstacleScript.objectSpeed = 20f * modifier;
		PowerupScript.objectSpeed = 20f * modifier;
		
		if (Input.GetButton ("Fire1")) {
			//if (emotiv.debug) PlayerInfo.attentionThreshold++;
		}
		if (Input.GetButton ("Fire2")) {
			//if (emotiv.debug) PlayerInfo.attentionThreshold--;
		}
	}

	IEnumerator UpdateAttention() {
		double sum = 0;
		double temp = 0;
		while (!gameObject.GetComponent<GameControlScript> ().isGameOver) {
			attentionValues.Add(emotiv.GetAttention("a"));
			if (attentionValues.Count >= 4) {
				sum = 0;
				foreach(double value in attentionValues) sum += value;
				temp = sum / attentionValues.Count;
				if (!double.IsNaN(temp)) attentionValue = temp; 
				attentionValues.RemoveRange(0, attentionValues.Count);
				Debug.Log ("AttentionValue: " + attentionValue + ", Threshold: " + PlayerInfo.attentionThreshold);
			}
			yield return new WaitForSeconds(0.250f);
		}
	}
	
	void OnGUI() {
		if (feedback) {
			GUI.DrawTexture(new Rect(8*Screen.width/9, Screen.height/4, 40, Screen.height/2), attentionBarFilled, ScaleMode.StretchToFill);
			if (!isAttentive()) {
				if (!isRelaxed()) {
					GUI.DrawTexture(new Rect(8*Screen.width/9, Screen.height/4, 40, (float)(Screen.height/2*(attentionValue - PlayerInfo.attentionThreshold)/(PlayerInfo.relaxationThreshold-PlayerInfo.attentionThreshold))), attentionBarEmpty, ScaleMode.StretchToFill);
				} else {
					GUI.DrawTexture(new Rect(8*Screen.width/9, Screen.height/4, 40, Screen.height/2), attentionBarEmpty, ScaleMode.StretchToFill);
				}
			}
		}
	}
}
