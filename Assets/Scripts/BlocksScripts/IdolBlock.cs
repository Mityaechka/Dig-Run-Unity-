using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdolBlock : BlockObject {
	void OnTriggerEnter(Collider coll)
	{
		if (coll.transform.gameObject.layer == 9) {
			//if(coll.GetComponent<PlayerControll> ().myPhaseController.hasAuthority)
			coll.GetComponent<PlayerControll> ().idolTouch();
			gameObject.SetActive (false);
			//Destroy (this.gameObject);
		} 
	}
}
