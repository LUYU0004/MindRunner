using UnityEngine;
using System.Collections;

public class GameControlScript : MonoBehaviour {
	
	public static float timeRemaining = 10;
	public static float timeExtension = 1f;
	public static float timeDeduction = 2f;
	public static float totalTimeElapsed = 0;
	public static float attentiveTime = 0;
	public static float semiAttentiveTime = 0;
	public static float semiRelaxedTime = 0;
	public static float relaxedTime = 0;
	public static float score=0f;
	public bool isGameOver = false;
	
	void Start(){
		Time.timeScale = 1;  // set the time scale to 1, to start the game world. This is needed if you restart the game from the game over menu
		timeRemaining = 10;
		timeExtension = 1f;
		timeDeduction = 2f;
		totalTimeElapsed = 0;
		attentiveTime = 0;
		semiAttentiveTime = 0;
		semiRelaxedTime = 0;
		relaxedTime = 0;
		score=0f;
		isGameOver = false;
	}
	
	void Update () { 
		if(isGameOver)
			return;
		
		totalTimeElapsed += Time.deltaTime;
		score = (totalTimeElapsed)*10+attentiveTime*15+semiAttentiveTime*5+semiRelaxedTime*1+relaxedTime*0;
		timeRemaining -= Time.deltaTime;
		AttentionControl attentionControl = gameObject.GetComponent<AttentionControl>();
		if (attentionControl.isAttentive()) attentiveTime += Time.deltaTime;
		else if (attentionControl.isSemiAttentive()) semiAttentiveTime += Time.deltaTime;
		else if (attentionControl.isSemiRelaxed()) semiRelaxedTime += Time.deltaTime;
		else if (attentionControl.isRelaxed()) relaxedTime += Time.deltaTime;
		if(timeRemaining <= 0){
			Score scoreObj = new Score();
			scoreObj.score = score;
			scoreObj.totalTimeElapsed = totalTimeElapsed;
			scoreObj.attentiveTime = attentiveTime;
			scoreObj.semiAttentiveTime = semiAttentiveTime;
			scoreObj.semiRelaxedTime = semiRelaxedTime;
			scoreObj.relaxedTime = relaxedTime;
			PlayerInfo.scores.Add(scoreObj);
			isGameOver = true;
		}
	}
	
	public void PowerupCollected()
	{
		timeRemaining += timeExtension;
	}
	
	public void AlcoholCollected()
	{
		timeRemaining -= timeDeduction;
	}
	
	void OnGUI()
	{
		//check if game is not over, if so, display the score and the time left
		if(!isGameOver)    
		{
			GUI.Label(new Rect(10, 10, Screen.width/5, Screen.height/6),"TIME LEFT: "+((int)timeRemaining).ToString());
			GUI.Label(new Rect(Screen.width-(Screen.width/6), 10, Screen.width/6, Screen.height/6), "SCORE: "+((int)score).ToString());
		}
		//if game over, display game over menu with score
		else
		{
			Time.timeScale = 0; //set the timescale to zero so as to stop the game world
			//display the final score
			GUI.Box(new Rect(Screen.width/4, Screen.height/8, Screen.width/2, Screen.height/2+Screen.height/4), "GAME OVER\nYOUR SCORE: "+(int)score);
			
			//restart the game on click
			if (GUI.Button(new Rect(Screen.width/4+10, Screen.height/4+Screen.height/10+10, Screen.width/2-20, Screen.height/10), "RESTART")){
				Application.LoadLevel(Application.loadedLevel);
			}

			//Save player info (for saving scores)
			if (GUI.Button(new Rect(Screen.width/4+10, Screen.height/4+2*Screen.height/10+10, Screen.width/2-20, Screen.height/10), "SAVE SCORE & PROGRESS")){
				PlayerInfo.Save ();
			}

			//load the main menu, which as of now has not been created
			if (GUI.Button(new Rect(Screen.width/4+10, Screen.height/4+3*Screen.height/10+10, Screen.width/2-20, Screen.height/10), "MAIN MENU")){
				Application.LoadLevel(0);
			}
			
			//exit the game
			if (GUI.Button(new Rect(Screen.width/4+10, Screen.height/4+4*Screen.height/10+10, Screen.width/2-20, Screen.height/10), "EXIT GAME")){
				Application.Quit();
			}
		}
	}
}