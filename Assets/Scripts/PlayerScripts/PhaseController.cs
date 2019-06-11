using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System.Collections.Generic;
using UnityEngine.UI;
public class PhaseController:NetworkBehaviour
{
	public ScoreContainer score;
	public  GhostPlayer ghost;
	public  AlivePlayer alive;
	public  GameObject aliveObject, ghostObject;
	public GameObject alivePrefab,ghostPrefab;
	public bool isAlive = true;
	public Vector3 deathPosition = Vector3.zero;

	public int id;
	bool isStartBuild;
	void Start(){
		MyNetManager.instance.addPlayer (this);
		MyNetManager.instance.getSpawnPoint (this.transform);
		GetMyId ();

		alive = aliveObject.GetComponent<AlivePlayer> ();ghost = ghostObject. GetComponent<GhostPlayer> ();

		alive.id = id;
		ghost.id = id;
		alive.transform.position = transform.position;ghost.transform.position = Vector3.zero;
		alive.myPhaseController = this;ghost.myPhaseController = this;
		if (hasAuthority) {
			MyNetManager.instance.attachCamera (alive);
			StartCoroutine ("waitBeforeSendBlanks");
			//FindObjectOfType<CameraPivot> ().player = aliveObject.transform;
			UIController.instance.alivePlyer = alive;
			UIController.instance.ghostPlayer = ghost;
		}

		ghostObject.SetActive (false);


	}

	IEnumerator waitBeforeSendBlanks(){
		yield return new WaitForSeconds (3);
		CmdGetBlanks ();
		yield break;
	}
	[Command]
	public void  CmdGetBlanks(){

		RpcGetBlanks (LobbyPackSelecter.instance.selectedBlanks.ToArray(),BlanksManager.instance.startBlankList);
		BlanksManager.instance.getBlanksCount++;
		if (BlanksManager.instance.getBlanksCount == Prototype.NetworkLobby.LobbyManager.s_Singleton._playerNumber) {
			
			RpcStartTimer ();
			Invoke ("waitStartTime", 4);
		}
		}
	[ClientRpc]
	void RpcStartTimer(){
		BlanksManager.instance.waring.SetActive (false);
		StartCoroutine ("minTimer");
	}
	void waitStartTime(){
		RpcStartGame ();
	}
	IEnumerator minTimer(){
		BlanksManager.instance.startTimer.SetActive (true);
		for (int i = 3; i >=0; i--) {
			BlanksManager.instance.startTimer.GetComponentInChildren<Text> ().text = i.ToString();
			yield return new WaitForSeconds (1);
			//yield return new  WaitForSeconds (1);
		}
		yield break;
	}
	[ClientRpc]
	public void  RpcStartGame(){
		BlanksManager.instance.startTimer.SetActive (false);
		MyNetManager.instance.myPLayer.alive.hasWait = true;
		MyNetManager.instance.myPLayer.ghost.hasWait = true;
		BlanksManager.instance.createStartServerBlanks ();

		for (int i = 0; i < MyNetManager.instance.playersOnServer.Count; i++) {
			MyNetManager.instance.playersOnServer [i].ghost.hasWait = true;
			MyNetManager.instance.playersOnServer [i].alive.hasWait = true;
		}
	}
	[ClientRpc]
	void RpcGetBlanks(int[] selectedBlanks,int[] start){
		LobbyPackSelecter.instance.selectedBlanks = new List<int>(selectedBlanks);
		LobbyPackSelecter.instance.getBlanksFromIndex (new List<int>(selectedBlanks));
		LobbyPackSelecter.instance.getBlanks ();
		BlanksManager.instance.startBlankList = start;
	}
	public override void OnStartLocalPlayer ()
	{
		base.OnStartLocalPlayer ();
		if (hasAuthority) {
			//BlanksManager.instance.createStart ();
		}


	}
	void GetMyId(){
		id = MyNetManager.instance.playerCount;
	}
	[Command]
	public void CmdGetScore(int id,int score){
		RpcGetScore (id, score);
	}
	[ClientRpc]
	void RpcGetScore(int id,int score){
		MyNetManager.instance.playersOnServer [id].score.Score = score;
	}
	//Получение эффекта игроком
	[Command]
	public void CmdGetEffect(int t,int playerID,int ghostID){
		MyNetManager.instance.playersOnServer[playerID-1]. RpcSetEffect (t,ghostID);
	}
	[ClientRpc]
	void RpcSetEffect(int t,int ghostId){
		if (hasAuthority) 
			alive.getEffect (t,ghostId);
	}
	//
	//Получение типа призрака
	[Command]
	public void CmdGetGhostType(int t,int playerID){
		MyNetManager.instance.playersOnServer[playerID-1].RpcSetGhostType (t);

	}
	[ClientRpc]
	void RpcSetGhostType(int t){
		ghost.ghostChar.setType (t);
		ghost.changeAlpha ();
	}
	//
	[Command]
	public void CmdGetSkillUsed(int playerID){
		MyNetManager.instance.playersOnServer[playerID-1].RpcSetUsedSkill (); 
	}
	[ClientRpc]
	void RpcSetUsedSkill(){
		ghost.changeAlpha ();
	}
	//

	//Падение сталактитов
	[Command]
	public void CmdTouchStalactite(int id){
		if(BlanksManager.createdBlock [id]!=null)
		RpcTouchStatalictite (id);
	}
	[ClientRpc]
	void RpcTouchStatalictite(int id){
		if(BlanksManager.createdBlock [id]!=null)
		BlanksManager.createdBlock [id].GetComponent<StalactiteBlock> ().gravityOn ();
	}
	//Остановка сталактитов
	[Command]
	public void CmdStopStalactite(int id){
		RpcStopStatalictite (id);
	}
	[ClientRpc]
	void RpcStopStatalictite(int id){
		BlanksManager.createdBlock [id].GetComponent<StalactiteBlock> ().stop ();
	}
	//Уничтожение сталоктитов
	[Command]
	public void CmdDestrStalactite(int id){
		RpcDestrStatalictite (id);
	}
	[ClientRpc]
	void RpcDestrStatalictite(int id){
		BlanksManager.createdBlock [id].GetComponent<StalactiteBlock> ().destroy ();
	}
	//
	[Command]
	public void CmdSendRequestForStartBlank(){

			RpcGetStartBlankList (BlanksManager.instance.startBlankList);

	}
	[ClientRpc]
	void RpcGetStartBlankList(int[] newList){
		BlanksManager.instance.startBlankList = newList;
	if (!BlanksManager.instance.isStartBuield){
		BlanksManager.instance.isStartBuield = true;
			for (int i = 0; i < BlanksManager.instance.blanksOnStart; i++) {
				BlanksManager.instance.blank = BlanksManager.instance.availableBlanks [BlanksManager.instance.startBlankList[i]];
				BlanksManager.instance.createBlankNoTimer ();
			}
		}
	}
	[Command]
	public void CmdSayServerBlockDig(int blockIndex){
		RpcGetDiggingBlock (blockIndex);
	}
	[ClientRpc]
	private void RpcGetDiggingBlock(int blockIndex){
		if(BlanksManager.createdBlock [blockIndex] !=null)
		BlanksManager.createdBlock [blockIndex].digThis ();
	}
	[Command]
	public void CmdSayLookDir(int dir){
		RpcSetLookDir (dir);
	}
	[ClientRpc]
	private void RpcSetLookDir(int dir){
		alive.lookDirection = dir;
		if (dir == 1)
			aliveObject.transform.rotation = Quaternion.Euler (0, 90, 0);
		else aliveObject.transform.rotation = Quaternion.Euler (0, -90, 0);
	}
	[Command]
	public void CmdGetDamage(int damage){
		if(!alive.isInvis)
		RpcSetDamage (damage);
		
	}
	[ClientRpc]
	public void RpcSetDamage(int damage){
		alive.takeDamege (damage);
		CmdGetScore (id-1, -5);
	}
	[Command]
	public void CmdGetHealthUp(int healthUp){
		RpcSetHealtUp (healthUp);
	}
	[ClientRpc]
	public void RpcSetHealtUp(int healthUp){
		alive.healthUp (healthUp);
	}


	[Command]
	public void CmdSaySwitch(Vector3 pos,bool stage,int sendBy){
		MyNetManager.instance.playersOnServer[sendBy-1].RpcSwitch (pos,stage); 
	}
	[ClientRpc]
	public void RpcSwitch(Vector3 pos,bool stage){
		IEnumerator c = switchPhase (pos);
		StartCoroutine (c);
	}
	//ОТКРЫТИЕ СУНДУКОВ
	[Command]
	public void CmdOpenChest(Vector3 dir,int index){
		RpcOpenChest (dir, index);
	}
	[ClientRpc]
	public void RpcOpenChest(Vector3 dir,int index){
		BlanksManager.instance.createdChest [index].GetComponent<Chest> ().open (dir);
	}
	//?//
	[Command]
	public void CmdSaySwitchPhase(){
		RpcSwitchPhase(); 
	}
	[ClientRpc]
	public void RpcSwitchPhase(){
		switchPhase ();
	}
	[Command]
	public void CmdPickUpEquipment(int id,int index){

		RpcPickUpEquipment (id,index);
	
	}
	[ClientRpc]
	public void RpcPickUpEquipment(int id,int index){

		alive.setEquipment (id,index);
	}


	[Command]
	public void CmdSetBootsParticle(bool stage){
		RpcSetBootsParticle (stage);
	}
	[ClientRpc]
	private void RpcSetBootsParticle(bool stage){
		alive.setBootsStage (stage);
	}


	[Command]
	public void CmdSetShield(bool val){
		RpcSetShield (val);
	}
	[ClientRpc]
	private void RpcSetShield(bool val){
		alive.setShield (val);
	}
	public void switchPhase(){
			if (isAlive) {
				isAlive = false;
				aliveObject.SetActive (false);
				ghostObject.SetActive (true);
			ghostObject.transform.position = aliveObject.transform.position;
			GetComponent<NetworkAnimator> ().animator = ghost.anim;
				GetComponent<NetworkTransformChild> ().target = ghostObject.transform;
			if (hasAuthority) 
				MyNetManager.instance.attachCamera (ghost);
			} else {
				isAlive = true;
				aliveObject.SetActive (true);
				ghostObject.SetActive (false);
			aliveObject.transform.position = ghostObject.transform.position;
			GetComponent<NetworkAnimator> ().animator = alive.anim;
				GetComponent<NetworkTransformChild> ().target = aliveObject.transform;
			if (hasAuthority)
			MyNetManager.instance.attachCamera (alive);
			}
	}
	public IEnumerator switchPhase(Vector3 newPosition){
		yield return new WaitForSeconds (0.1f);
		if (isAlive) {
			isAlive = false;
			aliveObject.SetActive (false);
			ghostObject.SetActive (true);
			ghostObject.transform.position = newPosition;
			GetComponent<NetworkAnimator> ().animator = ghost.anim;
			GetComponent<NetworkTransformChild> ().target = ghostObject.transform;
			if (hasAuthority) 
				MyNetManager.instance.attachCamera (ghost);
		} else {
			isAlive = true;
			aliveObject.SetActive (true);
			ghostObject.SetActive (false);
			aliveObject.transform.position = newPosition;
			GetComponent<NetworkAnimator> ().animator = alive.anim;
			GetComponent<NetworkTransformChild> ().target = aliveObject.transform;
			if (hasAuthority)
				MyNetManager.instance.attachCamera (alive);
		}
		yield break;
	}
}

