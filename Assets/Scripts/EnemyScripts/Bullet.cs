using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour
{
	public float speed,lifeTime;
	Vector3 direction;
	Rigidbody rb;
	void Start(){
		rb = GetComponent<Rigidbody> ();
		Destroy (gameObject, lifeTime);
	}
	void Update ()
	{
		rb.velocity = direction * speed * Time.fixedDeltaTime;
	}
	void OnTriggerEnter(Collider coll){
		if (coll.transform.gameObject.layer == 8) {
			Destroy (gameObject);
		} else if (coll.transform.tag=="P") {
			Destroy (gameObject);
			if (coll.transform.GetComponent<PlayerControll> ().myPhaseController.hasAuthority) {
				coll.transform.GetComponent<PlayerControll> ().myPhaseController.CmdGetDamage (1);

			}
		}
	}

	public Vector3 Direction {
		get {
			return direction;
		}
		set {
			direction = value;
		}
	}
}

