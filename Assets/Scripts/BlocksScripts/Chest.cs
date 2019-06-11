using UnityEngine;
using System.Collections;

public class Chest : BlockObject
{
	public int equipmentId;
	public Transform spawnPoint;
	public float spawnForce;
	public bool isOpen;


	
	public void open(Vector3 dir){
		isOpen = true;
		GameObject item = Instantiate (EquipmentManager.instance.equipmentObject [equipmentId], spawnPoint.position, Quaternion.identity);
		item.GetComponent<Rigidbody> ().AddForce (dir * spawnForce);
		item.GetComponent<EquipmentPickup> ().index = BlanksManager.instance.createdEquip.Count;
		BlanksManager.instance.createdEquip.Add (item);
	}
}

