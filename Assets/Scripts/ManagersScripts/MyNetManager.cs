using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyNetManager : MonoBehaviour {
	public static MyNetManager instance;
	public List<PhaseController> playersOnServer = new List<PhaseController> ();
	public List<GameObject> alivePlayer = new List<GameObject> ();
	public List<GameObject> aliveGhost = new List<GameObject> ();
	public int playerCount =0;
	public PhaseController myPLayer;
	public UnityStandardAssets.Cameras.FreeLookCam _camera;
	public Transform[] spawnPoints;
	public int currPoint = 0;

	public int timeEnd;
	int defTime;
	public void getSpawnPoint(Transform playerObj){
		playerObj.position = spawnPoints [currPoint].position;
		if (currPoint == spawnPoints.Length)
			currPoint = 0;
		else
			currPoint++;
	}
	public void attachCamera(PlayerControll player){
		if(myPLayer==null)
		myPLayer = player.myPhaseController;
		CameraPivot.instance.player = player.transform;
	}
	public void addPlayer(PhaseController newPlayer){
		playersOnServer.Add (newPlayer);
		playerCount++;
	}
	public void checkEndGame(){
		
		if (BlanksManager.instance.hasAuthority) {
			if (playerCount == aliveGhost.Count) {
				if (timeEnd >= 1) {
					minTime ();
				}
			} else {
				timeEnd = defTime;
				try{
					if(myPLayer.hasAuthority)
						BlanksManager.instance.RpcDisableEndGameTimer ();
				}catch{
				}
			}
		}
	}
	void minTime(){
		timeEnd--;
		if (timeEnd == 0) {
			BlanksManager.instance.RpcEndGame ();
			return;
		}
		BlanksManager.instance.RpcSetEndGameTimer (timeEnd);

		Invoke ("checkEndGame", 1);
	}
	public void EndGame(){
		EndTable.instance.timeTable.SetActive (false);
		EndTable.instance.anim.Play ("LastAnim");
		EndTable.instance.showTable ();
		_camera.Target = null;
		foreach(PhaseController player in MyNetManager.instance.playersOnServer){
			player.alive.transform.position = Vector3.zero;player.alive.gameObject.GetComponent<Rigidbody> ().isKinematic = true;
			player.ghost.transform.position = Vector3.zero;player.ghost.gameObject.GetComponent<Rigidbody> ().isKinematic = true;
		}
	}

	public void showEndTable(int time){
		EndTable.instance.gameObject.SetActive (true);
		EndTable.instance.timer.text = "Игра закончится через: " + time.ToString ();

	}
	public void disableEndTable(){
		EndTable.instance.timer.text = defTime.ToString();
		EndTable.instance.gameObject.SetActive (false);
	}
	void Awake(){
		instance = this;
		defTime = timeEnd;
		}
	void Start(){
		StartCoroutine ("mainThemeON");
	}
	IEnumerator mainThemeON(){
		yield return new WaitForSeconds (0.2f);
		//AudioManager.instance.playSound ("MainTheme", true,myPLayer.id);
		yield break;
	}
}
