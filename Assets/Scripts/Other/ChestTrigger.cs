using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestTrigger : MonoBehaviour {

	public Chest parent;

	void OnTriggerEnter(Collider coll){
		if (coll.transform.tag =="P"&&!parent.isOpen) {
			if (coll.GetComponent<AlivePlayer> ().myPhaseController.hasAuthority) {
				coll.GetComponent<AlivePlayer> ().myPhaseController.CmdOpenChest (new Vector3 (0, 0.5f, 0), parent.index);
			}
		}
	}
}
