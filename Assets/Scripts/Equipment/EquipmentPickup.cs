using UnityEngine;
using System.Collections;

public class EquipmentPickup : MonoBehaviour
{
	public Equipment equipment;
	public int index;
	void Start(){
		equipment = GetComponent<Equipment> ();
	}
	void OnCollisionEnter(Collision coll) {
		if (coll.transform.tag == "P") {
			AlivePlayer player = coll.transform.GetComponent<AlivePlayer> ();
			if (player.myPhaseController.hasAuthority) {
				player.myPhaseController.CmdPickUpEquipment (equipment.id,index);
			}
		}
	}
}

