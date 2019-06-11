using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class AlivePlayer : PlayerControll {
	public LayerMask mask;
	public float forceRepulsion;
	public float speed, jumpSpeed,minJumpSpeed,maxJumpSpeed,kickForce,kickForceUp,gravityKof;
	bool jump;

	BlockObject blockStand;


	bool isDigging;
	public float digTime;
	bool isJumpPress;
	public float jumpKof;
	float timeAfterPress;


	public GameObject ghostObject;
	public KickInfo myKick;

	bool isFreeRight,isFreeLeft;

	public bool hasEffect;

	public int inverseKof = 1;

	public Image blackholeObject;
	public SkillData currentSkill;

	public Vector3 kickImpulse = new Vector3();

	public Vector3 lastPos;
	void Start(){
		StartController ();
		blackholeObject = BlanksManager.instance.mask;
		blackholeObject.enabled = true;
		lastPos = transform.position;
	}

	void Update(){
		UpdateFrame ();


		if (kickImpulse.x > 0)
			kickImpulse.x-=0.3f;
		if (kickImpulse.x < 0)
			kickImpulse.x+=0.3f;


		if (kickImpulse.x < 0.4f && kickImpulse.x > -0.4f)
			kickImpulse.x = 0;
	}
	void FixedUpdate()
	{
		//body.AddForce (Vector3.right* direction*speed*inverseKof,ForceMode.Force);
		Vector3 movment = new Vector3 (direction*speed*Time.fixedDeltaTime+kickImpulse.x, body.velocity.y, 0);
	    body.velocity = movment;
	}
	public void setKickStage(int stage){
		if (stage == 1)
			myKick._collider.enabled = true;
		else
			myKick._collider.enabled = false;
	}
	protected override void listenDirecrion ()
	{

		base.listenDirecrion ();
		RaycastHit hit;
		//Debug.DrawLine (myTransform - Vector3.right * 0.5f - Vector3.up 	, myTransform - Vector3.right * 0.5f + Vector3.up * 0.6f, Color.red, 5);

		if (inverseKof == 1) {
			if (Physics.Linecast (myTransform.position - Vector3.right * 0.5f - Vector3.up, myTransform.position - Vector3.right * 0.5f + Vector3.up * 0.6f,out hit, mask)) {
				if (hit.transform.tag != "Idol") {
					if (direction == -1)
						setDir (0);
					isFreeLeft = false;
				}
			} else
				isFreeLeft = true;
				if (Physics.Linecast (myTransform.position + Vector3.right * 0.5f - Vector3.up, myTransform.position + Vector3.right * 0.5f + Vector3.up * 0.6f,out hit, mask)) {
				if (hit.transform.tag != "Idol") {
					if (direction == 1)
						setDir (0);
					isFreeRight = false;
				}
			} else
				isFreeRight = true;
		} else {
				if (Physics.Linecast (myTransform.position - Vector3.right * 0.5f - Vector3.up, myTransform.position - Vector3.right * 0.5f + Vector3.up * 0.6f,out hit, mask)) {
				if (hit.transform.tag != "Idol") {
					if (direction == 1)
						setDir (0);
					isFreeRight = false;
				}
			} else
				isFreeLeft = true;
						if (Physics.Linecast (myTransform.position + Vector3.right * 0.5f - Vector3.up, myTransform.position + Vector3.right * 0.5f + Vector3.up * 0.6f,out hit, mask)) {
				if (hit.transform.tag != "Idol") {
					if (direction == -1)
						setDir (0);
					isFreeLeft = false;
				}
			} else
				isFreeRight = true;
		}

		if (Input.GetKeyDown (KeyCode.R))
			myPhaseController.CmdGetDamage (1);

		Debug.DrawLine (myTransform.position-transform.right*0.5f, myTransform.position -transform.right*0.5f - transform.up, Color.red, 0.1f);
		Debug.DrawLine (myTransform.position+transform.right*0.5f, myTransform.position +transform.right*0.5f - transform.up, Color.red, 0.1f);
		Debug.DrawLine (myTransform.position, myTransform.position  - transform.up, Color.red, 0.1f);

		bool checkGround = false;

		if (Physics.Linecast (myTransform.position, myTransform.position  - transform.up , out hit))
			checkGround = true; 
		else if (Physics.Linecast (myTransform.position+transform.right*0.3f, myTransform.position +transform.right*0.3f - transform.up, out hit))
			checkGround = true; 
		else if (Physics.Linecast (myTransform.position-transform.right*0.3f, myTransform.position -transform.right*0.3f - transform.up, out hit))
			checkGround = true; 
		
		if (checkGround) {
			if (hit.transform.tag == "Block") {
				blockStand = hit.transform.GetComponent<BlockObject> ();
				if (blockStand.canJump)
					jump = true;
			}
		} else {
			jump = false;
			blockStand = null;
		}
		if (inverseKof == 1) {
			if (Input.GetKeyDown (KeyCode.A) && isFreeLeft)
				setDir (-1);
			if (Input.GetKeyDown (KeyCode.D) && isFreeRight)
				setDir (1);
		} else {
			if (Input.GetKeyDown (KeyCode.A) && isFreeRight)
				setDir (-1);
			if (Input.GetKeyDown (KeyCode.D) && isFreeLeft)
				setDir (1);
		}
		if (Input.GetKeyUp (KeyCode.A) || Input.GetKeyUp (KeyCode.D)) {
			setDir (0);
		}

		if (Input.GetKeyDown (KeyCode.Space))
			isJumpPress = true;
		if (Input.GetKeyUp (KeyCode.Space)) {
			isJumpPress = false;
			Jump ();
			jumpSpeed = minJumpSpeed;
			timeAfterPress = 0;
		}
		if (isJumpPress)
			calcJumpSpeed ();

		if (Input.GetKeyDown (KeyCode.W) && !isDigging) {
			isDigging = true;
			anim.SetBool ("Digging", true);
			InvokeRepeating ("digBlock", 0, digTime);
		}

		if (Input.GetKeyUp (KeyCode.W)) {
			anim.SetBool ("Digging", false);
			isDigging = false;
			CancelInvoke ("digBlock");
		}
		if (Input.GetKeyUp (KeyCode.Q)) {
			anim.SetBool ("Kick", false);
		}
		if (Input.GetKeyDown (KeyCode.Q)) {
			anim.SetBool ("Kick", true);
		}
		Vector3 currentPos = transform.position;

		if (lastPos.x == transform.position.x) {
				anim.SetBool ("Move", true);
		} else if (jump && direction != 0)
			anim.SetBool ("Move", true);
		else anim.SetBool ("Move", false);

		lastPos = currentPos;
	}

	public void getEffect(int effectType,int ghostID){
		if (!hasEffect) {
			switch ((Type)effectType) {
			case Type.BlackDisplay:
				currentSkill = SkillsContainer.instance.blackSkill;
				break;
			case Type.Inverse:
				currentSkill = SkillsContainer.instance.inverseSkill;
				break;
			case Type.DamagePlayer:
				currentSkill = SkillsContainer.instance.damageSkill;
				break;
			case Type.TradeBody:
				currentSkill = SkillsContainer.instance.tradeSkill;
				break;
			}
			currentSkill.effect.applyEffect (this, MyNetManager.instance.playersOnServer[ghostID-1].ghost);
		}
	}

	public void setDirByUI(int dir){
		setDir (dir);
	}
	public void setDigStateByUI(bool state){
		if (state && !isDigging) {
			isDigging = true;
			anim.SetBool ("Digging", true);
			InvokeRepeating ("digBlock", 0, digTime);
		}else{
			anim.SetBool ("Digging", false);
			isDigging = false;
			CancelInvoke ("digBlock");
		}
	}
	public void setKickStateByUI(bool state){
			anim.SetBool ("Kick", state);

	}
	public void setJumpState(bool state){
		if (state)
			isJumpPress = true;
		else {
			isJumpPress = false;
			Jump ();
			jumpSpeed = minJumpSpeed;
			timeAfterPress = 0;
		}
	}
	void calcJumpSpeed(){
		timeAfterPress += Time.fixedDeltaTime;
//		Debug.Log (jumpSpeed+", "+timeAfterPress);
		jumpSpeed = minJumpSpeed+ Mathf.Pow(2.7f,timeAfterPress*jumpKof);
		if (jumpSpeed >= maxJumpSpeed) {
			isJumpPress = false;
			Jump ();
			jumpSpeed = minJumpSpeed;
			timeAfterPress = 0;
		}
	}
	void digBlock (){
		if (blockStand != null&&myPhaseController.hasAuthority) {
			if (blockStand.canBroken) {
				myPhaseController.CmdSayServerBlockDig (findInListByIndex(blockStand.index));
			}
		}
	}

	void OnTriggerEnter(Collider coll){
		if (coll.tag == "Kick") {
			kickImpulse += Vector3.right * coll.GetComponent<KickInfo> ().getDir () * kickForce ;
			//body.AddForce (Vector3.right * coll.GetComponent<KickInfo> ().getDir () * kickForce, ForceMode.Impulse);
			body.AddForce (Vector3.up * kickForceUp, ForceMode.Impulse);
		} 
	}
	void OnCollisionEnter(Collision collision) {
		if (collision.transform.gameObject.layer == 10) {
			if (myPhaseController.hasAuthority) {
				myPhaseController.CmdGetDamage (1);
				GetImpulse (collision);
			}
		}
	}
	void GetImpulse(Collision collision){
		body.AddForce (collision.contacts [0].normal * forceRepulsion, ForceMode.Impulse);
		if (-collision.contacts [0].point.y + myTransform.position.y > 0.0f)
			body.AddForce (Vector3.up * forceRepulsion, ForceMode.Impulse);
		else
			body.AddForce (-Vector3.up * forceRepulsion, ForceMode.Impulse);
	}
	public void setDir(int dir){
		direction = dir ;
		if (dir != 0) {
			myPhaseController.CmdSayLookDir (dir);
		}
	}
	void Jump(){
		if (jump) {
			body.AddForce(Vector3.up*jumpSpeed,ForceMode.Impulse);
			jump = false;
		}
	}
	public int getLookDir(){
		return lookDirection;
	}
	void OnCollisionExit(Collision coll) 
	{
		if(coll.transform.tag == "Block")
		{
			blockStand = null;
			jump = false;
		}
	}
	void OnEnable(){
		//health = maxHealth;
		hasEffect = false;
		if (!isFirstEnable) {
			if (myPhaseController.hasAuthority)
				UIController.instance.aliveLayerUi.SetActive (true);

		}
		MyNetManager.instance.alivePlayer.Add (gameObject);
		isFirstEnable = false;
	}
	void OnDisable(){
		if(myPhaseController.hasAuthority)
		UIController.instance.aliveLayerUi.SetActive (false);
		MyNetManager.instance.alivePlayer.Remove (gameObject);
	}
}
