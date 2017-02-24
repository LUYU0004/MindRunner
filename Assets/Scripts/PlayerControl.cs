using UnityEngine;
using System.Collections;

public class PlayerControl : MonoBehaviour {
	// Use this for initialization
	public static float speed = 6.0f;
	public static float jumpSpeed = 8.0f;
	public static float gravity = 20.0f;

	public GameControlScript control;
	CharacterController controller;
	private Vector3 moveDirection = Vector3.zero;
	
	public CountdownScript count;  //CountdownScript instance
	public PauseMenuScript pause;  //PauseMenuScript instance
	//audio source reference variables
	public AudioSource powerupCollectSound;
	public AudioSource jumpSound;
	public AudioSource snagCollectSound;
	
	#if UNITY_ANDROID
	Vector3 zeroAcc;  //zero reference input.acceleration
	Vector3 currentAcc;  //In-game input.acceleration
	float sensitivityH = 3; //alter this to change the sensitivity of the accelerometer
	float smooth = 0.5f; //determines how smooth the acceleration(horizontal movement, in our case) control is
	float GetAxisH = 0;  //variable used to hold the value equivalent to Input.GetAxis("Horizontal")
	#endif
	
	//start 
	void Start () {
		//Debug.Log("Inside player control script start");
		controller = GetComponent<CharacterController>();
		#if UNITY_ANDROID
		zeroAcc = Input.acceleration;
		currentAcc = Vector3.zero;
		#endif
	}
	
	// Update is called once per frame
	void  Update (){
		//accelerometer and touch detection
		#if UNITY_ANDROID
		currentAcc = Vector3.Lerp(currentAcc, Input.acceleration-zeroAcc, Time.deltaTime/smooth);
		GetAxisH = Mathf.Clamp(currentAcc.x * sensitivityH, -1, 1);
		int fingerCount = 0;
		foreach (Touch touch in Input.touches) {
			if (touch.phase != TouchPhase.Ended && touch.phase != TouchPhase.Canceled)
				fingerCount++; 
		}
		#endif
		
		//check if grounded and countdown is done with
		if (controller.isGrounded && count.isCountDown  ) {
			// We are grounded, so recalculate
			// move direction directly from axes
			animation.Play("run");
			//check if game is paused
			if(pause.paused==false)
				gameObject.GetComponent<AudioSource>().enabled = true;
			else
				gameObject.GetComponent<AudioSource>().enabled = false;
			moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0, 0);
			#if UNITY_ANDROID
			moveDirection = new Vector3(GetAxisH, 0, 0);
			#endif
			moveDirection = transform.TransformDirection(moveDirection);
			moveDirection *= speed;
			
			jumpSound.Stop();
			#if UNITY_ANDROID
			if (fingerCount >= 1){
				animation.Stop("run");
				animation.Play("jump_pose");
				
				jumpSound.Play();
				gameObject.GetComponent<AudioSource>().enabled = false;
				moveDirection.y = jumpSpeed;
			}
			#endif
			if (Input.GetButton ("Jump")) {
				animation.Stop("run");
				animation.Play("jump_pose");
				
				jumpSound.Play();
				gameObject.GetComponent<AudioSource>().enabled = false;
				moveDirection.y = jumpSpeed;
			}
		}
		//disable run sound if game is over
		if(control.isGameOver){ 
			gameObject.GetComponent<AudioSource>().enabled = false;
		}
		// Apply gravity
		moveDirection.y -= gravity * Time.deltaTime;
		
		// Move the controller
		controller.Move(moveDirection * Time.deltaTime);
	}
	
	void OnTriggerEnter(Collider other){
		if(other.gameObject.name == "Powerup(Clone)")
		{
			powerupCollectSound.Play();  //play powerup collected sound
			control.PowerupCollected();
		}
		else if(other.gameObject.name == "Obstacle(Clone)")
		{
			snagCollectSound.Play();  //play snag collected sound
			control.AlcoholCollected(); 
		}
		Destroy(other.gameObject); 
	} 
}