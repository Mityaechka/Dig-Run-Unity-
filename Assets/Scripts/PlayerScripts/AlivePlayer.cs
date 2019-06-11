using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class AlivePlayer : PlayerControll {
	public int defaultPickaxeId;
	public Pickaxe currentPickaxe;
	public Helmet currentHelmet;
	public Boots currentBoots;

	public LayerMask mask;

	public float forceRepulsion;
	public float speed, jumpSpeed,minJumpSpeed,maxJumpSpeed;
	bool jump,isDigging,isJumpPress,canFly,isPlayerFly,isFirstBootsCheck;
	[HideInInspector]
	public bool isFreeRight = true,isFreeLeft = true;
	BlockObject blockStand;


	public float jumpKof;
	float timeAfterPress;


	public GameObject ghostObject;
	public KickInfo myKick;


	public bool hasEffect;
	[SerializeField]
	int inverseKof = 1;

	public Image blackholeObject;
	public SkillData currentSkill;

	public Vector3 kickImpulse = new Vector3();

	public Vector3 lastPos;
	float gravityKof = 1;
	public Transform helmetPosition,leftBootTransform,rightBootTransform,pickaxePosition;
	GameObject helmetObj,lBootObj,rBootObj,pickaxeObj;
	ParticleSystem boot1,boot2;

	public GameObject drill,pickaxe,shovel;
	bool isPress,pressLeft,pressRight;
	void Start(){


		StartController ();
		blackholeObject = BlanksManager.instance.mask;
		blackholeObject.enabled = true;
		lastPos = transform.position;

		lookDirection = 1;
		transform.rotation = Quaternion.Euler (0, 90, 0);
		setEquipment (defaultPickaxeId,-1);


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
		if (isJumpPress)
			calcJumpSpeed ();
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
		if (inverseKof == 1) {
			if (Input.GetKey (KeyCode.A))
				setDir (-1);
			if (Input.GetKey (KeyCode.D))
				setDir (1);
		} else {
			if (Input.GetKey (KeyCode.A) )
				setDir (-1);
			if (Input.GetKey (KeyCode.D))
				setDir (1);
		}
		if (isPress) {
			if (inverseKof == 1) {
				if (pressRight)
					setDir (1);
				if (pressLeft)
					setDir (-1);
			} else {
				if (pressRight)
					setDir (-1);
				if (pressLeft)
					setDir (1);
			}
		}

		//Debug.DrawLine ( myTransform.position - Vector3.right * 0.5f + Vector3.up * 0.8f,myTransform.position - Vector3.right * 0.5f - Vector3.up*0.5f, Color.red);
		//Debug.DrawLine ( myTransform.position + Vector3.right * 0.5f - Vector3.up,myTransform.position + Vector3.right * 0.5f + Vector3.up * 0.6f, Color.red);
		if (inverseKof == 1) {
			if (Physics.Linecast ( myTransform.position - Vector3.right * 0.5f + Vector3.up * 0.8f,myTransform.position - Vector3.right * 0.5f - Vector3.up*0.5f,out hit, mask)
				|| Physics.Linecast (myTransform.position - Vector3.right * 0.5f - Vector3.up,myTransform.position - Vector3.right * 0.5f + Vector3.up * 0.6f,out hit, mask)) {
					if (direction == -1)
						setDir (0);
			} else
				isFreeLeft = true;
			if (Physics.Linecast ( myTransform.position + Vector3.right * 0.5f + Vector3.up * 0.8f,myTransform.position + Vector3.right * 0.5f - Vector3.up*0.5f,out hit, mask)
					||Physics.Linecast (myTransform.position + Vector3.right * 0.5f - Vector3.up,myTransform.position + Vector3.right * 0.5f + Vector3.up * 0.6f,out hit, mask)) {
					if (direction == 1)
						setDir (0);
			} else
				isFreeRight = true;
		} else {
			if (Physics.Linecast ( myTransform.position - Vector3.right * 0.5f + Vector3.up * 0.8f,myTransform.position - Vector3.right * 0.5f - Vector3.up*0.5f,out hit, mask)
				|| Physics.Linecast (myTransform.position - Vector3.right * 0.5f - Vector3.up,myTransform.position - Vector3.right * 0.5f + Vector3.up * 0.6f,out hit, mask)){	
					if (direction == -1)
						setDir (0);
			} else
				isFreeLeft = true;
			if (Physics.Linecast ( myTransform.position + Vector3.right * 0.5f + Vector3.up * 0.8f,myTransform.position + Vector3.right * 0.5f - Vector3.up*0.5f,out hit, mask)
				||Physics.Linecast (myTransform.position + Vector3.right * 0.5f - Vector3.up,myTransform.position + Vector3.right * 0.5f + Vector3.up * 0.6f,out hit, mask)) {
					if (direction == 1)
						setDir (0);
			} else
				isFreeRight = true;
		}

		if (Input.GetKeyDown (KeyCode.R))
			myPhaseController.CmdGetDamage (1);
	
		bool checkGround = false;

		if (Physics.Linecast (myTransform.position, myTransform.position  - transform.up , out hit))
			checkGround = true; 
		else if (Physics.Linecast (myTransform.position+transform.forward*0.3f, myTransform.position +transform.forward*0.3f - transform.up, out hit))
			checkGround = true; 
		else if (Physics.Linecast (myTransform.position-transform.forward*0.3f, myTransform.position -transform.forward*0.3f - transform.up, out hit))
			checkGround = true; 
		
		if (checkGround) {
			if (hit.transform.tag == "Block") {
				blockStand = hit.transform.GetComponent<BlockObject> ();
				if (blockStand.canJump)
					jump = true;
				isPlayerFly = false;
			}
		} else {
			jump = false;
			blockStand = null;
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
			isPlayerFly = true;
			if (boot1 != null) {
				myPhaseController.CmdSetBootsParticle (false);
				isFirstBootsCheck = false;
			}
		}


		if (Input.GetKeyDown (KeyCode.W) && !isDigging) {
			isDigging = true;
			anim.SetBool ("Digging", true);
				StartCoroutine ("digBlock");
			}


		if (Input.GetKeyUp (KeyCode.W)) {
			anim.SetBool ("Digging", false);
			isDigging = false;
			StopCoroutine ("digBlock");
		}
		if (Input.GetKeyUp (KeyCode.Q)) {
			anim.SetBool ("Kick", false);
		}
		if (Input.GetKeyDown (KeyCode.Q)&&currentPickaxe.canKick) {
			anim.SetBool ("Kick", true);
		}
		Vector3 currentPos = transform.position;
		if (jump) {
			if (isFreeRight && direction == 1)
				anim.SetBool ("Move", true);
			else if (isFreeLeft && direction == -1)
				anim.SetBool ("Move", true);
			else
				anim.SetBool ("Move", false);
		}
		else
			anim.SetBool ("Move", false);


		lastPos = currentPos;
	}
	public void setBootsStage(bool stage){
		if(boot1!=null){
			if (stage) {
				boot1.Play ();
				boot2.Play ();
			} else {
				boot1.Stop ();
				boot2.Stop ();
			}
			}
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
		if (dir == 1) {
			pressRight = true;
			pressLeft = false;
			isPress = true;
		} else if (dir == -1) {
			pressRight = false;
			pressLeft = true;
			isPress = true;
		} else {
			pressRight = false;
			pressLeft = false;
			isPress = false;
		}
		setDir (dir);
	}
	public void setDigStateByUI(bool state){
		if (state && !isDigging) {
			isDigging = true;
			anim.SetBool ("Digging", true);
				StartCoroutine ("digBlock");
			if (currentPickaxe.id == 4) {
				AudioManager.instance.playSound ("Drill", true, id);
			}

		}else{
			anim.SetBool ("Digging", false);
			isDigging = false;
			StopCoroutine ("digBlock");
			if (currentPickaxe.id == 4) {
				AudioManager.instance.playSound ("Drill", true, id);
			}
		}
	}
	public void setKickStateByUI(bool state){
		if (currentPickaxe.canKick) {
			anim.SetBool ("Kick", state);
		}

	}
	public void setJumpState(bool state){
		if (state)
			isJumpPress = true;
		else {
			isJumpPress = false;
			Jump ();
			jumpSpeed = minJumpSpeed;
			timeAfterPress = 0;

			if (boot1 != null) {
				myPhaseController.CmdSetBootsParticle (false);
				isFirstBootsCheck = false;
			}
		}
	}
	void calcJumpSpeed(){
		timeAfterPress += Time.fixedDeltaTime;
//		Debug.Log (jumpSpeed+", "+timeAfterPress);
		if (currentBoots != null && canFly) {
			if (!isPlayerFly) {
				if (timeAfterPress < currentBoots.flyTime) {
					body.AddForce (Vector3.up * currentBoots.flyForce, ForceMode.Force);
					if (!isFirstBootsCheck) {

						myPhaseController.CmdSetBootsParticle (true);
						isFirstBootsCheck = true;
					}
				} else {

					isPlayerFly = true;
					isJumpPress = false;
					jumpSpeed = minJumpSpeed;
					timeAfterPress = 0;


					myPhaseController.CmdSetBootsParticle (false);
					isFirstBootsCheck = false;
				}
			}
		} else {
			jumpSpeed = minJumpSpeed + Mathf.Pow (2.7f, timeAfterPress * jumpKof);
			if (jumpSpeed >= maxJumpSpeed) {
				isJumpPress = false;
				Jump ();
				jumpSpeed = minJumpSpeed;
				timeAfterPress = 0;
			}
		}
	}
	IEnumerator digBlock (){


		yield return new WaitForSeconds (currentPickaxe.digTime);
		if (isDigging) {
			if (blockStand != null && myPhaseController.hasAuthority) {
				if (blockStand.canBroken) {
					myPhaseController.CmdGetScore (id-1, blockStand.scoreForDig);
					myPhaseController.CmdSayServerBlockDig (findInListByIndex (blockStand.index));
					
				}
			} else {
				StopCoroutine ("digBlock");
				StartCoroutine ("digBlock");
				yield break;
			}
			StartCoroutine ("digBlock");

		}
		yield break;

	}

	void OnTriggerEnter(Collider coll){
		if (coll.tag == "Kick") {
			KickInfo kick = coll.GetComponent<KickInfo> ();

			if (kick.parent.currentPickaxe.canKick) {
				kickImpulse += Vector3.right * kick.getDir() * kick.parent.currentPickaxe.kickForce;
				body.AddForce (Vector3.up * kick.parent.currentPickaxe.kickForceUp, ForceMode.Impulse);
			}
		}
		if (coll.transform.gameObject.layer == 10) {
			if (myPhaseController.hasAuthority) {
				myPhaseController.CmdGetDamage (1);
				GetImpulse (myTransform.position-coll.transform.position,1f);
			}
		}
	}
	void OnCollisionEnter(Collision collision) {
		if (collision.transform.gameObject.layer == 10) {

			if (myPhaseController.hasAuthority) {
				myPhaseController.CmdGetDamage (1);
				GetImpulse (collision,true);
			}
		}else if (collision.transform.tag=="Spike") {
			if (myPhaseController.hasAuthority) {
				bool flag = false;
				if (currentBoots == null)
					flag = true;
				else if (currentBoots.isProtect)
					flag = false;
				if (flag) {
					myPhaseController.CmdGetDamage (1);
					GetImpulse (collision, true);
				}
				}

			}
		}

	void GetImpulse(Collision collision,bool up){
		body.AddForce (collision.contacts [0].normal * forceRepulsion, ForceMode.Impulse);
		if (up) {
			if (-collision.contacts [0].point.y + myTransform.position.y > 0.0f)
				body.AddForce (Vector3.up * forceRepulsion, ForceMode.Impulse);
			else
				body.AddForce (-Vector3.up * forceRepulsion, ForceMode.Impulse);
		}
	}
	void GetImpulse(Vector3 dir,float k){
		body.AddForce (dir * forceRepulsion*k, ForceMode.Impulse);
	}
	public void setDir(int dir){
		direction = dir;
		if (dir != 0) //{
			myPhaseController.CmdSayLookDir (dir);
			//AudioManager.instance.playSound ("Walk", true);
		//} else
			//AudioManager.instance.playSound ("Walk", false);
	}
	void Jump(){
		if (jump) {
			AudioManager.instance.playSound ("Jump",id);
			body.AddForce(Vector3.up*jumpSpeed*gravityKof,ForceMode.Impulse);
			jump = false;
		}
	}
	public int getLookDir(){
		return lookDirection;
	}
	public void setEquipment(int id,int index){
		if(index!=-1)
		BlanksManager.instance.createdEquip [index].SetActive (false);
		//myPhaseController.CmdGetScore (id-1, 20);
		GameObject equipment = EquipmentManager.instance.equipmentObject [id];
		Quaternion rot;
		if (lookDirection == 1)
			rot = Quaternion.Euler (0, 0, 0);
		else rot = Quaternion.Euler (0, 180, 0);
		if (index != -1)
		if (myPhaseController.hasAuthority)
			ItemDiscription.inctanse.Item = equipment.GetComponent<Equipment> ();

		switch (equipment.GetComponent<Equipment>().type) {
		case(EquipmentType.Helmet):
			if (currentHelmet != null) {
				Destroy (helmetObj);
			}
			currentHelmet = (Helmet)equipment.GetComponent<EquipmentPickup> ().equipment;
			currentHelmet.pickUp (this);
			if (currentHelmet.increaseHearts)
				maxHealth = 4;
			else {
				maxHealth = 3;
				if (health == 4)
					health = 3;
				UIController.instance.UpdateUIPlayer ();
			}
			GameObject gm = Instantiate (equipment.GetComponent<Equipment> ().equipObject, helmetPosition.position, rot, helmetPosition);
			helmetObj = gm;


			break;

		case(EquipmentType.Pickaxe):

			currentPickaxe = (Pickaxe)equipment.GetComponent<EquipmentPickup> ().equipment;
			currentPickaxe.pickUp (this);

			//GameObject pickaxe = Instantiate (equipment.GetComponent<Equipment> ().equipObject, pickaxePosition.position, rot, pickaxePosition);

			anim.SetFloat ("PickaxeType", currentPickaxe.animationType);

			if (pickaxeObj != null)
				pickaxeObj.SetActive (false);
			switch (currentPickaxe.tag) {
			case "Pickaxe":
				pickaxeObj = pickaxe;
				break;
			case "Drill":
				pickaxeObj = drill;
				break;
			case "Shovel":
				pickaxeObj = shovel;
				break;
			}
			pickaxeObj.SetActive (true);
			myKick =  pickaxeObj.GetComponentInChildren<KickInfo> ();
			break;

		case(EquipmentType.Boots):
			if (currentBoots != null) {
				Destroy (lBootObj);
				Destroy (rBootObj);
			}
			currentBoots = (Boots)equipment.GetComponent<EquipmentPickup> ().equipment;
			currentBoots.pickUp (this);

			GameObject boot = Instantiate (equipment.GetComponent<Equipment> ().equipObject, leftBootTransform.position, rot, leftBootTransform);
			lBootObj = boot;
			boot1 = boot.GetComponentInChildren<ParticleSystem> ();

			boot = Instantiate (equipment.GetComponent<Equipment> ().equipObject, rightBootTransform.position,rot, rightBootTransform);
			rBootObj = boot;
			boot2 = boot.GetComponentInChildren<ParticleSystem> ();

			canFly = currentBoots.isFlying;
			if (currentBoots.isLowGravity)
				gravityKof = currentBoots.lowGravityKof;
			else
				gravityKof = 1;
			break;

		}

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
			setEquipment (currentPickaxe.id, -1);
			if (myPhaseController.hasAuthority) {
					/*if (Application.platform != RuntimePlatform.WindowsPlayer||Application.platform == RuntimePlatform.WindowsEditor) {
						for (int i = 0; i < UIController.instance.aliveLayerUi.transform.childCount-2; i++) {
						UIController.instance.aliveLayerUi.transform.GetChild (i).gameObject.SetActive (true);
						}
					}*/
				UIController.instance.aliveLayerUi.SetActive (true);
			}

		}
		if (UIController.instance.alivePlyer == null) {
			UIController.instance.alivePlyer = this;
			UIController.instance.UpdateUIPlayer ();
		}
		MyNetManager.instance.alivePlayer.Add (gameObject);
		InvokeRepeating ("findGHostInView", 0, 0.25f);
		isFirstEnable = false;
		isInvis = false;
	}
	void OnDisable(){
		if(myPhaseController.hasAuthority)
		UIController.instance.aliveLayerUi.SetActive (false);
		MyNetManager.instance.alivePlayer.Remove (gameObject);
		CancelInvoke ();
		setDir (0);
	}



	void findGHostInView(){
		if (currentHelmet != null) {
			if (currentHelmet.isTelepatic) {
				GameObject[] ghosts = MyNetManager.instance.aliveGhost.ToArray();
				for (int i = 0; i < ghosts.Length; i++) {
					Debug.DrawLine (transform.position, ghosts [i].transform.position, Color.yellow, 1);
					if (!Physics.Linecast (transform.position, ghosts [i].transform.position, mask)) {
						Debug.Log (Vector3.Distance (transform.position, ghosts [i].transform.position));
						if (Vector3.Distance (transform.position, ghosts [i].transform.position) < currentHelmet.radius) {
							Debug.Log ("AAA");
							ghosts [i].GetComponent<GhostPlayer> ().SlowKof = currentHelmet.speedSlowKof;
						} else
							ghosts [i].GetComponent<GhostPlayer> ().SlowKof = 1;
					}else ghosts [i].GetComponent<GhostPlayer> ().SlowKof = 1;
				}
			}
		}
	}

	public int InverseKof {
		get {
			return inverseKof;
		}
		set {
			inverseKof = value;
		}
	}
}
