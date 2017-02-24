using UnityEngine;
using System.Collections;

public class TrainingScript : MonoBehaviour 
{
	//public GUISkin myskin;  //custom GUIskin reference
	public string mainMenu;
	public string playerName = "";
	public int countRestMax = 5;  //max countdown number
	public int countMax = 15;
	public Texture progressBarFilled;
	public Texture progressBarEmpty;
	public Texture targetMark;

	private bool relaxationTraining = false;
	private bool attentionTraining = false;
	private bool resetTraining = false;
	private int trainingState = 0;
	private int countDown = 0;
	private float timeElapsed = 0;
	private float startTime = 0;
	private EmotivController emotiv = new EmotivController();
	
	private void OnGUI()
	{
		//GUI.skin=myskin;   //use the custom GUISkin
		
		if (PlayerInfo.name.Equals("") || PlayerInfo.name == null) {
			GUI.Box(new Rect(Screen.width/4, Screen.height/4-2*Screen.height/10+25, Screen.width/2, Screen.height/2+2*Screen.height/10+25), "MIND TRAINING");
			GUI.Label(new Rect(Screen.width/4+10, Screen.height/4+Screen.height/10+25, Screen.width/2-20, Screen.height/10), "Please enter your name...");
			playerName = GUI.TextField(new Rect(Screen.width/4+10, Screen.height/4+2*Screen.height/10+25, Screen.width/2-20, Screen.height/10), playerName, 25);
			if (GUI.Button(new Rect(Screen.width/4+10, Screen.height/4+3*Screen.height/10+25, Screen.width/2-20, Screen.height/10), "CONFIRM")){
				PlayerInfo.name = playerName;
				PlayerInfo.Load ();
			}
		} else {
			switch(trainingState) {
			case -1:
				GUI.Box(new Rect(Screen.width/4, Screen.height/4-2*Screen.height/10+25, Screen.width/2, Screen.height/2+2*Screen.height/10+25), "MIND TRAINING");
				GUI.Label(new Rect(Screen.width/4+10, Screen.height/4, Screen.width/2-20, Screen.height/10), "Please connect Emotiv EPOC Headset...");
				if (emotiv.isConnected()) {
					trainingState = 0;
				} else {
					emotiv = new EmotivController();
				}
				break;
			case 0:
				if (emotiv.isConnected()) {
					GUI.Box(new Rect(Screen.width/4, Screen.height/4-2*Screen.height/10+25, Screen.width/2, Screen.height/2+2*Screen.height/10+25), "MIND TRAINING");
					GUI.Label(new Rect(Screen.width/4+10, Screen.height/4+0*Screen.height/18, Screen.width/2-20, Screen.height/10), "Player Name: "+PlayerInfo.name);
					GUI.Label(new Rect(Screen.width/4+10, Screen.height/4+1*Screen.height/18, Screen.width/2-20, Screen.height/10), "Relaxation Threshold: "+PlayerInfo.relaxationThreshold);
					GUI.Label(new Rect(Screen.width/4+10, Screen.height/4+2*Screen.height/18, Screen.width/2-20, Screen.height/10), "Attention Threshold: "+PlayerInfo.attentionThreshold);
					if (GUI.Button(new Rect(Screen.width/4+10, Screen.height/4+Screen.height/10+25, Screen.width/2-20, Screen.height/10), "Relaxation Training")){
						trainingState = 1;
					}
					if (GUI.Button(new Rect(Screen.width/4+10, Screen.height/4+2*Screen.height/10+25, Screen.width/2-20, Screen.height/10), "Attention Training")){
						trainingState = 101;
					}
					if (GUI.Button(new Rect(Screen.width/4+10, Screen.height/4+3*Screen.height/10+25, Screen.width/2-20, Screen.height/10), "Reset Training")){
						trainingState = 201;
					}
					if (GUI.Button(new Rect(Screen.width/4+10, Screen.height/4+4*Screen.height/10+25, Screen.width/2-20, Screen.height/10), "Save and Return to Main Menu")){
						PlayerInfo.Save ();
						Application.LoadLevel(mainMenu);
					}
				} else {
					trainingState = -1;
				}
				break;
			case 201:
				GUI.Box(new Rect(Screen.width/4, Screen.height/4-2*Screen.height/10+25, Screen.width/2, Screen.height/2+2*Screen.height/10+25), "RESET TRAINING");
				GUI.Label(new Rect(Screen.width/4+10, Screen.height/4, Screen.width/2-20, Screen.height/10), "Are you sure?");
				GUI.Label(new Rect(Screen.width/4+10, Screen.height/4+1*Screen.height/18, Screen.width/2-20, Screen.height/10), "    All training data for "+PlayerInfo.name+" will be wiped!");
				if (GUI.Button(new Rect(Screen.width/4+10, Screen.height/4+4*Screen.height/10+25, (Screen.width/2-20)/2, Screen.height/10), "Continue")){
					PlayerInfo.ResetTraining();
					trainingState = 0;
				}
				if (GUI.Button(new Rect(Screen.width/4+10+(Screen.width/2-20)/2, Screen.height/4+4*Screen.height/10+25, (Screen.width/2-20)/2, Screen.height/10), "Return")){
					trainingState = 0;
				}
				break;
			case 1:
				GUI.Box(new Rect(Screen.width/4, Screen.height/4-2*Screen.height/10+25, Screen.width/2, Screen.height/2+2*Screen.height/10+25), "RELAXATION TRAINING");
				GUI.Label(new Rect(Screen.width/4+10, Screen.height/4, Screen.width/2-20, Screen.height/10), "Relaxation Training flow:");
				GUI.Label(new Rect(Screen.width/4+10, Screen.height/4+1*Screen.height/18, Screen.width/2-20, Screen.height/10), "    1. 5 seconds preparation to relax your mind.");
				GUI.Label(new Rect(Screen.width/4+10, Screen.height/4+2*Screen.height/18, Screen.width/2-20, Screen.height/10), "    2. 15 seconds to relax.");
				GUI.Label(new Rect(Screen.width/4+10, Screen.height/4+3*Screen.height/18, Screen.width/2-20, Screen.height/10), "    3. 5 seconds preparation to relax your mind.");
				GUI.Label(new Rect(Screen.width/4+10, Screen.height/4+4*Screen.height/18, Screen.width/2-20, Screen.height/10), "    4. 15 seconds to relax.");
				GUI.Label(new Rect(Screen.width/4+10, Screen.height/4+5*Screen.height/18, Screen.width/2-20, Screen.height/10), "    5. 5 seconds preparation to relax your mind.");
				GUI.Label(new Rect(Screen.width/4+10, Screen.height/4+6*Screen.height/18, Screen.width/2-20, Screen.height/10), "    6. 15 seconds to relax.");
				GUI.Label(new Rect(Screen.width/4+10, Screen.height/4+7*Screen.height/18, Screen.width/2-20, Screen.height/10), "If you redo this training the previous relaxation power threshold calculated will be used for averaging.");
				if (GUI.Button(new Rect(Screen.width/4+10, Screen.height/4+4*Screen.height/10+25, (Screen.width/2-20)/2, Screen.height/10), "Start Training")){
					trainingState++;
				}
				if (GUI.Button(new Rect(Screen.width/4+10+(Screen.width/2-20)/2, Screen.height/4+4*Screen.height/10+25, (Screen.width/2-20)/2, Screen.height/10), "Return")){
					trainingState = 0;
				}
				break;
			case 2:
			case 6:
			case 10:
				trainingState++;
				StartCoroutine(CountdownRest());
				break;
			case 3:
			case 7:
			case 11:
				GUI.Box(new Rect(Screen.width/4, Screen.height/4-2*Screen.height/10+25, Screen.width/2, Screen.height/2+2*Screen.height/10+25), "RELAXATION TRAINING");
				GUI.Label(new Rect(Screen.width/4+10, Screen.height/4, Screen.width/2-20, Screen.height/10), "Please be prepared to relax your mind: "+countDown.ToString());
				break;
			case 4:
			case 8:
			case 12:
				trainingState++;
				StartCoroutine(CountdownActual());
				StartCoroutine(GetRelaxationValues());
				startTime = Time.time;
				break;
			case 5:
			case 9:
			case 13:
				GUI.Box(new Rect(Screen.width/4, Screen.height/4-2*Screen.height/10+25, Screen.width/2, Screen.height/2+2*Screen.height/10+25), "RELAXATION TRAINING");
				GUI.Label(new Rect(Screen.width/4+10, Screen.height/4, Screen.width/2-20, Screen.height/10), "Relax your mind...: "+countDown.ToString());
				GUI.DrawTexture(new Rect(Screen.width/4+10, Screen.height/4+6*Screen.height/18, Screen.width/2-20, Screen.height/10), progressBarEmpty, ScaleMode.StretchToFill);
				timeElapsed = Time.time - startTime;
				GUI.DrawTexture(new Rect(Screen.width/4+10, Screen.height/4+6*Screen.height/18, (Screen.width/2-20) * timeElapsed/15, Screen.height/10), progressBarFilled, ScaleMode.StretchToFill);
				break;
			case 14:
				PlayerInfo.relaxationThreshold = PlayerInfo.CalculateThreshold (PlayerInfo.relaxationRatios);
				trainingState++;
				break;
			case 15:
				GUI.Box(new Rect(Screen.width/4, Screen.height/4-2*Screen.height/10+25, Screen.width/2, Screen.height/2+2*Screen.height/10+25), "RELAXATION TRAINING");
				GUI.Label(new Rect(Screen.width/4+10, Screen.height/4, Screen.width/2-20, Screen.height/10), "Relaxation Threshold: "+PlayerInfo.relaxationThreshold);
				if (GUI.Button(new Rect(Screen.width/4+10, Screen.height/4+4*Screen.height/10+25, Screen.width/2-20, Screen.height/10), "Return")){
					trainingState = 0;
				}
				break;
			case 101:
				GUI.Box(new Rect(Screen.width/4, Screen.height/4-2*Screen.height/10+25, Screen.width/2, Screen.height/2+2*Screen.height/10+25), "ATTENTION TRAINING");
				GUI.Label(new Rect(Screen.width/4+10, Screen.height/4, Screen.width/2-20, Screen.height/10), "Attention Training flow: **Please focus on Target Mark image during the 15 seconds training.");
				GUI.Label(new Rect(Screen.width/4+10, Screen.height/4+1*Screen.height/18, Screen.width/2-20, Screen.height/10), "    1. 5 seconds rest and preparation to focus your mind.");
				GUI.Label(new Rect(Screen.width/4+10, Screen.height/4+2*Screen.height/18, Screen.width/2-20, Screen.height/10), "    2. 15 seconds to focus.");
				GUI.Label(new Rect(Screen.width/4+10, Screen.height/4+3*Screen.height/18, Screen.width/2-20, Screen.height/10), "    3. 5 seconds rest and preparation to focus your mind.");
				GUI.Label(new Rect(Screen.width/4+10, Screen.height/4+4*Screen.height/18, Screen.width/2-20, Screen.height/10), "    4. 15 seconds to focus.");
				GUI.Label(new Rect(Screen.width/4+10, Screen.height/4+5*Screen.height/18, Screen.width/2-20, Screen.height/10), "    5. 5 seconds rest and preparation to focus your mind.");
				GUI.Label(new Rect(Screen.width/4+10, Screen.height/4+6*Screen.height/18, Screen.width/2-20, Screen.height/10), "    6. 15 seconds to focus.");
				GUI.Label(new Rect(Screen.width/4+10, Screen.height/4+7*Screen.height/18, Screen.width/2-20, Screen.height/10), "If you redo this training the previous attention power threshold calculated will be used for averaging.");
				if (GUI.Button(new Rect(Screen.width/4+10, Screen.height/4+4*Screen.height/10+25, (Screen.width/2-20)/2, Screen.height/10), "Start Training")){
					trainingState++;
				}
				if (GUI.Button(new Rect(Screen.width/4+10+(Screen.width/2-20)/2, Screen.height/4+4*Screen.height/10+25, (Screen.width/2-20)/2, Screen.height/10), "Return")){
					trainingState = 0;
				}
				break;
			case 102:
			case 106:
			case 110:
				trainingState++;
				StartCoroutine(CountdownRest());
				break;
			case 103:
			case 107:
			case 111:
				GUI.Box(new Rect(Screen.width/4, Screen.height/4-2*Screen.height/10+25, Screen.width/2, Screen.height/2+2*Screen.height/10+25), "ATTENTION TRAINING");
				GUI.Label(new Rect(Screen.width/4+10, Screen.height/4, Screen.width/2-20, Screen.height/10), "Please take a rest and be prepared to focus on the target image: "+countDown.ToString());
				break;
			case 104:
			case 108:
			case 112:
				trainingState++;
				StartCoroutine(CountdownActual());
				StartCoroutine(GetAttentionValues());
				startTime = Time.time;
				break;
			case 105:
			case 109:
			case 113:
				GUI.Box(new Rect(Screen.width/4, Screen.height/4-2*Screen.height/10+25, Screen.width/2, Screen.height/2+2*Screen.height/10+25), "ATTENTION TRAINING");
				GUI.Label(new Rect(Screen.width/4+10, Screen.height/4, Screen.width/2-20, Screen.height/10), "Focus on the Target...: "+countDown.ToString());
				GUI.DrawTexture(new Rect(Screen.width/2-36, Screen.height/2-63, 72, 63), targetMark, ScaleMode.StretchToFill);	
				GUI.DrawTexture(new Rect(Screen.width/4+10, Screen.height/4+6*Screen.height/18, Screen.width/2-20, Screen.height/10), progressBarEmpty, ScaleMode.StretchToFill);
				timeElapsed = Time.time - startTime;
				GUI.DrawTexture(new Rect(Screen.width/4+10, Screen.height/4+6*Screen.height/18, (Screen.width/2-20) * timeElapsed/15, Screen.height/10), progressBarFilled, ScaleMode.StretchToFill);
				break;
			case 114:
				PlayerInfo.attentionThreshold = PlayerInfo.CalculateThreshold (PlayerInfo.attentionRatios);
				trainingState++;
				break;
			case 115:
				GUI.Box(new Rect(Screen.width/4, Screen.height/4-2*Screen.height/10+25, Screen.width/2, Screen.height/2+2*Screen.height/10+25), "ATTENTION TRAINING");
				GUI.Label(new Rect(Screen.width/4+10, Screen.height/4, Screen.width/2-20, Screen.height/10), "Attention Threshold: "+PlayerInfo.attentionThreshold);
				if (GUI.Button(new Rect(Screen.width/4+10, Screen.height/4+4*Screen.height/10+25, Screen.width/2-20, Screen.height/10), "Return")){
					trainingState = 0;
				}
				break;
			}
		}
	}

	IEnumerator CountdownRest() {
		//start the countdown
		for(countDown = countRestMax; countDown>-1;countDown--){
			if(countDown!=0){
				yield return new WaitForSeconds(1);
			}
			else{
				trainingState++;  //set the flag to true
			}
		}
	}

	IEnumerator CountdownActual() {
		//start the countdown
		for(countDown = countMax; countDown>-1;countDown--){
			if(countDown!=0){
				yield return new WaitForSeconds(1);    
			}
			else{
				trainingState++;  //set the flag to true
			}
		}
	}

	IEnumerator GetRelaxationValues ()
	{
		int count; // 4 * 0.25s == 1 seconds
		ThetaBetaRatios ratio = new ThetaBetaRatios ("Relaxation");
		for ( count = countMax * 4; count > -1; count--) {
			ratio.values.Add (emotiv.GetAttention("r"));
			yield return new WaitForSeconds(0.250f);
		}
		ratio.CalculateAverages ();
		ratio.CalculateMedian ();
		ratio.CalculateMean ();
		PlayerInfo.relaxationRatios.Add (ratio);
	}

	IEnumerator GetAttentionValues ()
	{
		int count; // 4 * 0.25s == 1 seconds
		ThetaBetaRatios ratio = new ThetaBetaRatios ("Attention");
		for ( count = countMax * 4; count > -1; count--) {
			ratio.values.Add (emotiv.GetAttention("a"));
			yield return new WaitForSeconds(0.250f);
		}
		ratio.CalculateAverages ();
		ratio.CalculateMedian ();
		ratio.CalculateMean ();
		PlayerInfo.attentionRatios.Add (ratio);
	}
}