using UnityEngine;
using System.Collections;

public class MultiGun : Gun
{
	public Vector3[] gunDirection;
	Transform[] body;
	Transform[] bulletSpawn;
	Quaternion[] rotate;
	int gunCount;
	void Start(){
		Invoke ("SetPosition", 1);
		Invoke ("makeStart", 1);

	}
	void makeStart(){
		gunCount = gunDirection.Length;

		rotate = new Quaternion[gunCount];
		body = new Transform[gunCount];
		bulletSpawn = new Transform[gunCount];

		for (int i = 0; i < gunDirection.Length; i++) {
			body [i] = findBody (i);
			bulletSpawn [i] = findSpawnPoint (body [i]);

			rotate[i] = transform.rotation * Quaternion.Euler (gunDirection[i]-new Vector3(0,0,90));
			body[i].rotation = rotate[i];
		}

		InvokeRepeating ("shoot", 0, rechargeTime);
	}
	protected override void beforeDestroy ()
	{
		CancelInvoke ();
	}
	void shoot(){
		for (int i = 0; i < body.Length; i++) {
			Instantiate (bulletPrefab, bulletSpawn[i].position, rotate[i]);
		}
	}
}

