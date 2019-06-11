using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RedLineScript : MonoBehaviour {
	public float speed;
	public float k,devideK;
	Rigidbody rb;
	void Start(){
		rb = GetComponent<Rigidbody> ();
	}
	void Update(){
		//transform.position = Vector3.MoveTowards (transform.position, target, speed * Time.deltaTime);
		rb.velocity = -Vector3.up * speed * Time.fixedDeltaTime*k;
		if(k<3)
		k += Time.deltaTime/devideK;
	}

	void OnTriggerEnter(Collider coll) 
	{
		try{
		if (coll.transform.gameObject.layer == 8 || coll.transform.gameObject.layer == 11) {
			coll.transform.GetComponent<BlockObject> ().destroyThis ();
		} else if (coll.transform.gameObject.layer == 10) {
			coll.transform.gameObject.SetActive (false);
		} else if (coll.transform.gameObject.layer == 9) {
			PlayerControll p = coll.transform.GetComponent<PlayerControll> ();
			if (p.myPhaseController.hasAuthority) {
				p.findFreePlace ();
				if (p.myPhaseController.isAlive)
					p.myPhaseController.CmdGetDamage (1);
			}
		} else if (coll.transform.gameObject.layer == 13)
			coll.gameObject.SetActive (false);
		}catch{
		
	}
	}
}
