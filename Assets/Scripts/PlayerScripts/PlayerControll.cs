using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
public class PlayerControll : MonoBehaviour {
	

	protected Rigidbody body;
	public ObjectNameView nameView;
	public int id;
	[HideInInspector]
	public bool isClient;
	public Animator anim;
	[HideInInspector]
	public bool firstFrame=true,hasWait =false;
	//[HideInInspector]
	public int direction,lookDirection;

	public int health ,maxHealth ;
	public float maxInvisTime;
	public bool isInvis,hasGhostProtect;
	public PhaseController myPhaseController;
	protected 	bool isFirstEnable = true;
	protected Transform myTransform;

	public int idolsCount,maxIdols = 3;
	public ParticleSystem shield;
	public  GameObject teleporParticle;
	protected void StartController(){
		var players = Prototype.NetworkLobby.LobbyPlayerList._instance.Players;
		nameView.text = players [myPhaseController.id-1].playerName;
		nameView.textColor = players [myPhaseController.id-1].playerColor;
		myTransform = transform;
		//TODO StartCoroutine ("waitBeforeStart");
		body = GetComponent<Rigidbody> ();
		//anim = GetComponent<Animator> ();
	}
	protected virtual void UpdateFrame(){
		BlanksManager b = BlanksManager.instance;
		if (myPhaseController.hasAuthority) {
			listenDirecrion ();
		}

		if (BlanksManager.instance.hasAuthority&&hasWait) {
			if (myTransform.position.y - b.cursor.y < b.minDistToBorder)
				b.GetRequestToCreateBlank (this);
		}
	}
	protected virtual void listenDirecrion(){}

	public void findFreePlace(){
		bool isFree = false;
		Vector3 newPos = Vector3.zero;
		for (int i = -3; i <= 3; i++) {
			Debug.Log (i);
			for (int j = 3; j < 20; j++) {
				Debug.Log (j);
				if (transform.position.x+i > 0 && transform.position.x+i < 17) {
					if (!Physics.Linecast (myTransform.position - Vector3.up * j + Vector3.right * i, myTransform.position - Vector3.up * (j - 2) + Vector3.right * i)) {
						isFree = true;
						newPos = new Vector3 (myTransform.position.x + i, myTransform.position.y - j, myTransform.position.z);
						break;
					}
				}
			}
			if (isFree)
				break;
		}
		Instantiate (teleporParticle, newPos, teleporParticle.transform.rotation);
		myTransform.position = newPos;
	}
	IEnumerator waitBeforeStart(){
		yield return new WaitForSeconds (1);
		hasWait = true;
		yield break;
	}
	public void setShield(bool val){
		hasGhostProtect = val;
		if (val) {
			shield.Play ();
		} else {
			shield.Stop ();
		}

	}
	public void takeDamege(int damage){
		if (!isInvis) {
			health -= damage;
			Debug.Log ("AAAA");

			if (health < 3) {
				myPhaseController.CmdSetShield (false);
			}
			Invoke ("enableShield", 0.15f);
			isInvis = true;
			Invoke ("waitInvis", maxInvisTime);
			idolsCount = 0;
			if (health <= 0) {
				health = 0;
				AudioManager.instance.playSound ("Death",id);
				myPhaseController.switchPhase ();
			} else
				AudioManager.instance.playSound ("Damage",id);
		}
		UIController.instance.UpdateUIPlayer ();
	}
	void enableShield(){
		shield.Play ();
	}
	void waitInvis(){
		isInvis = false;
		if (idolsCount <= 2)
			shield.Stop ();
	}
	public void healthUp(int healthUp){
		health+=healthUp;

		UIController.instance.UpdateUIPlayer ();
	}
	public void idolTouch(){
		if(myPhaseController.hasAuthority){
		if (myPhaseController.isAlive) {
				AudioManager.instance.playSound ("Idol",id);
				myPhaseController.CmdGetScore (id-1, 10);
				if (health < maxHealth)
					myPhaseController.CmdGetHealthUp (1);
				else if (idolsCount < maxIdols) {
					idolsCount++;

				} 
				if(idolsCount==maxIdols&&health>=3){
					myPhaseController.CmdSetShield (true);
				}
		} else {
				myPhaseController.CmdGetScore (id-1, 40);
				myPhaseController.CmdSaySwitchPhase();
				Debug.Log (maxHealth);
				myPhaseController.CmdGetHealthUp (maxHealth);
		}
		}
		UIController.instance.UpdateUIPlayer ();
	}
	protected int findInListByIndex(int index){
		int r = 0,i = 0;
		foreach (BlockObject block in BlanksManager.createdBlock) {
			if (block.index == index) {
				r = i;
				break;
			}
			i++;
		}
		return r;
	}

}
