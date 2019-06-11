using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : BlockObject {
	public GameObject bulletPrefab;
	public float rechargeTime;
	public bool isOnWall;
	protected AudioSource sound;
	protected void SetPosition(){
		RaycastHit hit;
		if (isOnWall) {
			if (Physics.Linecast (transform.position, transform.position + Vector3.right, out hit)) {
				if (hit.transform.gameObject.layer == 8) {
					transform.rotation = Quaternion.Euler (0, 0, 180);
				}
			}else if (Physics.Linecast (transform.position, transform.position - Vector3.right, out hit)) {
				if (hit.transform.gameObject.layer == 8) {
					transform.rotation = Quaternion.Euler (0, 0, 0);
				}
			}
		}else if (Physics.Linecast (transform.position, transform.position + Vector3.up, out hit)) {
			if (hit.transform.gameObject.layer == 8) {
				transform.rotation = Quaternion.Euler (0, 0, -90);
			}
		}else if (Physics.Linecast (transform.position, transform.position - Vector3.up, out hit)) {
			if (hit.transform.gameObject.layer == 8) {
				transform.rotation = Quaternion.Euler (0, 0, 90);
			}
		}
	}
	protected Transform findBody(int childID){
		return transform.GetChild (childID);
	}
	protected Transform findSpawnPoint(Transform body){
		return body.Find ("Spawn");
	}
}
