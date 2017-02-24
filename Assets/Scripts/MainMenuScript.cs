using UnityEngine;
using System.Collections;

public class MainMenuScript : MonoBehaviour 
{
	//public GUISkin myskin;  //custom GUIskin reference
	public string gameLevel;
	public string trainingLevel;
	public string playerName;
	
	private void OnGUI()
	{
		//GUI.skin=myskin;   //use the custom GUISkin
		
		if (PlayerInfo.name.Equals("") || PlayerInfo.name == null) {
			GUI.Box(new Rect(Screen.width/4, Screen.height/4-2*Screen.height/10+25, Screen.width/2, Screen.height/2+2*Screen.height/10+25), "MIND RUNNER");
			GUI.Label(new Rect(Screen.width/4+10, Screen.height/4+Screen.height/10+25, Screen.width/2-20, Screen.height/10), "Please enter your name...");
			playerName = GUI.TextField(new Rect(Screen.width/4+10, Screen.height/4+2*Screen.height/10+25, Screen.width/2-20, Screen.height/10), playerName, 25);
			if (GUI.Button(new Rect(Screen.width/4+10, Screen.height/4+3*Screen.height/10+25, Screen.width/2-20, Screen.height/10), "CONFIRM")){
				PlayerInfo.name = playerName;
				PlayerInfo.Load ();
			}
		} else {
			GUI.Box(new Rect(Screen.width/4, Screen.height/4, Screen.width/2, Screen.height/2), "MAIN MENU");
			if (GUI.Button(new Rect(Screen.width/4+10, Screen.height/4+Screen.height/10+10, Screen.width/2-20, Screen.height/10), "TRAINING")){
				Application.LoadLevel(trainingLevel);
			}
			
			if (GUI.Button(new Rect(Screen.width/4+10, Screen.height/4+2*Screen.height/10+10, Screen.width/2-20, Screen.height/10), "PLAY")){
				if (!PlayerInfo.name.Equals ("") && PlayerInfo.name != null) {
					Application.LoadLevel(gameLevel);	
				}
			}
			
			if (GUI.Button(new Rect(Screen.width/4+10, Screen.height/4+3*Screen.height/10+10, Screen.width/2-20, Screen.height/10), "EXIT")){
				Application.Quit();
			}
		}
	}
}