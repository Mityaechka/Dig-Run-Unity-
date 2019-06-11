using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StalactiteBlock : BlockObject {
	float speed = 8.5f;
	bool isTriggered,isFalled;
	float length = 0;
	Rigidbody rb;
	void Start(){
		InvokeRepeating ("UpdateObj", 0, 0.05f);
		rb = GetComponent<Rigidbody> ();
	}
	void FixedUpdate(){
		if (isTriggered && !isFalled) {
			rb.velocity = -Vector3.up * speed;
			speed += Time.fixedDeltaTime;
		}
	}
	void UpdateObj ()
	{
		if (length == 0) {
			RaycastHit firstHit;
			if (Physics.Linecast (transform.position - Vector3.up, transform.position - Vector3.up * 20, out firstHit)) {
				length = firstHit.distance;
			}
		} else if(!isTriggered) {
			RaycastHit secondHit;
			Debug.DrawLine (transform.position - Vector3.up, transform.position - Vector3.up * (length + 0.5f), Color.red, 2);
			if (Physics.Linecast (transform.position - Vector3.up, transform.position - Vector3.up * (length + 0.5f), out secondHit)) {
				if (secondHit.transform.tag == "P" && secondHit.transform.GetComponent<PlayerControll>().myPhaseController.hasAuthority) {
					//gravityOn ();
					secondHit.transform.GetComponent<PlayerControll>().myPhaseController.CmdTouchStalactite (BlanksManager.findInListByIndex (index));
				}
			}
		}
	}
	public void gravityOn(){
		rb.isKinematic = false;
		isTriggered = true;
	}
	public void stop(){
		isFalled = true;
		rb.isKinematic = true;
	}
	public void destroy(){
		Destroy (this.gameObject);
	}
	void OnCollisionEnter(Collision coll) {
		if (isTriggered && !isFalled) {
			
			if (coll.gameObject.layer == 8){
				stop ();
		}
				//coll.transform.GetComponent<PlayerControll> ().myPhaseController.CmdStopStalactite (BlanksManager.findInListByIndex (index));
			else if (coll.transform.tag == "P") {
				AlivePlayer player = coll.transform.GetComponent<AlivePlayer> ();
				if (player.myPhaseController.hasAuthority) {
					if (player.currentHelmet != null) {
						if(!player.currentHelmet.isProtect)
							player.myPhaseController.CmdGetDamage (1);
					}else player.myPhaseController.CmdGetDamage (1);
					player.myPhaseController.CmdDestrStalactite (BlanksManager.findInListByIndex (index));
				}
			}
		}
	}
}


 