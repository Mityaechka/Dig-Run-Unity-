using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentManager : MonoBehaviour {


	public static EquipmentManager instance;
	public GameObject[] equipmentObject;
	void Awake(){
		instance = this;
	}
/*	public void pickUp(int id,PhaseController player){
		Debug.Log ("2");
			player.CmdPickUpEquipment (id);
			if (currentHelmet != null) {
				currentHelmet.drop ();
			}
			currentHelmet = (Helmet)equipmentObject.GetComponent<EquipmentPickup>().equipment;
			currentHelmet.pickUp (player);
		}*/
	}
