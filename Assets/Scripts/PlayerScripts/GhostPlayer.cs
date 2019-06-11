using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using UnityEngine.UI;
public class GhostPlayer : PlayerControll
{
	float vertDir,horDir;
	public float speed;
	public GameObject nearestPlayer;
	public GhostChar ghostChar;
	public LayerMask mask;

	public SpriteRenderer spriteRender;
	float curTime =0;

	public Button effectBtn;


	public bool isSkillAvaible;

	float slowKof = 1;
	void Start(){
		myPhaseController.CmdGetGhostType(Random.Range (0, 4),id);
		effectBtn = UIController.instance.effectBtn;
		StartController ();
		Collider coll = GetComponent<Collider> ();
		foreach (PhaseController player in MyNetManager.instance.playersOnServer) {

			Physics.IgnoreCollision(coll,player.aliveObject.GetComponent<Collider>());
		}

	}
	void Update(){
		UpdateFrame ();
	}
	void FixedUpdate()
	{
		Vector3 movment = new Vector3 (horDir*ghostChar.MainSpeed*slowKof*Time.fixedDeltaTime, vertDir*ghostChar.MainSpeed*slowKof*Time.fixedDeltaTime, 0);
		body.velocity = movment;
	}
	void findNearestPlayer(){
		List<GameObject> players = MyNetManager.instance.alivePlayer;
		List<GameObject> playersInView = new List<GameObject> ();
		List<float> distances = new List<float>();
		for (int i = 0; i < players.Count; i++) {
			Debug.DrawLine (transform.position, players [i].transform.position, Color.yellow, 1);
			if (!Physics.Linecast (transform.position, players [i].transform.position, mask)){
				if(!players[i].GetComponent<AlivePlayer>().hasGhostProtect||ghostChar.GhostType==Type.DamagePlayer)
				distances.Add (Vector3.Distance (transform.position, players [i].transform.position));
				playersInView.Add (players [i]);
			}
		}

		int minIndex = 0;

		float minDistance = float.MaxValue;
		for (int i = 0; i < distances.Count; i++) {
				if(distances[i]<minDistance){
				minIndex = i;
				minDistance = distances [i];
				}
		}
		if (minDistance <= ghostChar.MinDistanse) {
			nearestPlayer = playersInView [minIndex];

			if(isSkillAvaible) 
				effectBtn.interactable = true;
			else effectBtn.interactable = false;
		} else {
			nearestPlayer =	null;
			effectBtn.interactable = false;
		}

		}
	protected override void listenDirecrion ()
	{
		base.listenDirecrion ();
		if (Input.GetKeyDown (KeyCode.A))
			setDirHor (-1);
		if (Input.GetKeyDown (KeyCode.D))
			setDirHor (1);
		if (Input.GetKeyUp (KeyCode.A) || Input.GetKeyUp (KeyCode.D)) {
			setDirHor (0);
		}
		setDirHor (BlanksManager.instance.joystic.Value.x);
		setDirVert (BlanksManager.instance.joystic.Value.y);
		if (Input.GetKeyDown (KeyCode.Q)) {
			if (nearestPlayer != null&&isSkillAvaible) {
			myPhaseController.CmdGetSkillUsed(id);
				myPhaseController.CmdGetEffect (ghostChar.getTypeInt (), nearestPlayer.GetComponent<AlivePlayer> ().myPhaseController.id,id);
			}
		}
		if (Input.GetKeyDown (KeyCode.W))
			setDirVert (1);
		if (Input.GetKeyDown (KeyCode.S))
			setDirVert (-1);
		if (Input.GetKeyUp (KeyCode.W) || Input.GetKeyUp (KeyCode.S)) {
			setDirVert (0);
		}
	}

	public void setDirVert(int dir){
		vertDir = dir;
	}
	public void setDirHor(int dir){
		horDir = dir;
	}
	public void setDirVert(float dir){
		vertDir = dir;
	}
	public void setDirHor(float dir){
		horDir = dir;
	}
	public void useEffect(){
		if (nearestPlayer != null&&isSkillAvaible) {
			myPhaseController.CmdGetScore (id-1, 50);
			myPhaseController.CmdGetSkillUsed(id);
			myPhaseController.CmdGetEffect (ghostChar.getTypeInt (), nearestPlayer.GetComponent<AlivePlayer> ().myPhaseController.id,id);
		}
	}
	void OnEnable(){
		MyNetManager.instance.aliveGhost.Add (gameObject);
		horDir = 0;
			vertDir = 0;
		if (!isFirstEnable) {
			if (myPhaseController.hasAuthority) {
				Debug.Log("ACTIVATE");
				UIController.instance.ghostLayerUI.SetActive (true);
				myPhaseController.CmdGetGhostType(Random.Range (0, 4),id);
				InvokeRepeating ("findNearestPlayer", 0, 0.25f);

			}
			MyNetManager.instance.checkEndGame ();
		}
		slowKof = 1;
		isFirstEnable = false;


	}
	void OnDisable(){
		MyNetManager.instance.aliveGhost.Remove (gameObject);
		if (myPhaseController.hasAuthority) {
			UIController.instance.ghostLayerUI.SetActive (false);
			//BlanksManager.instance.joystic.thumbTR.localPosition = Vector2.zero;
			CancelInvoke ();
		}

		MyNetManager.instance.checkEndGame ();
		EndTable.instance.lastPlayer =  Prototype.NetworkLobby.LobbyPlayerList._instance.Players[id-1].playerName;
		EndTable.instance.lastPlayerText.color =  Prototype.NetworkLobby.LobbyPlayerList._instance.Players[id-1].playerColor;
		//UIController.instance.UpdateUIPlayer ();
	}



	public void changeAlpha(){
		curTime = 0;
		isSkillAvaible = false;
		spriteRender.color = new Color (1, 1, 1, 0);
		IEnumerator coritine = changeAlphaCorotine ();
		StartCoroutine (coritine);
	}
	IEnumerator changeAlphaCorotine(){
		while (curTime / ghostChar.currentSkill.rechargeTime < 1) {
			yield return new WaitForSeconds (0.05f);
			curTime += 0.05f;
			spriteRender.color = new Color (1, 1, 1, curTime);
		}
		isSkillAvaible = true;
		yield break;
	}

	public float SlowKof {
		get {
			return slowKof;
		}
		set {
			slowKof = value;
		}
	}
}

