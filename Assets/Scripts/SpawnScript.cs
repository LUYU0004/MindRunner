using UnityEngine;
using System.Collections;

public class SpawnScript : MonoBehaviour {
	public static float spawnCycle = 0.5f;

	public GameObject obstacle;
	public GameObject powerup;
	
	float timeElapsed = 0;
	bool spawnPowerup = true;
	
	void Update () {
		if(spawnCycle > 0.001f) {
			timeElapsed += Time.deltaTime;
			if (timeElapsed > spawnCycle) {
				GameObject temp;
				if(spawnPowerup)
				{
					temp = (GameObject)Instantiate(powerup);
					Vector3 pos = temp.transform.position;
					temp.transform.position = new Vector3(Random.Range(-3, 4), pos.y, pos.z);
				}
				else
				{
					temp = (GameObject)Instantiate(obstacle);
					Vector3 pos = temp.transform.position;
					temp.transform.position = new Vector3(Random.Range(-3, 4), pos.y, pos.z);
				}
				
				//timeElapsed -= spawnCycle;
				timeElapsed = 0;
				spawnPowerup = !spawnPowerup;
			}
		}
	}
}