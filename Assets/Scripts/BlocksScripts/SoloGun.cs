using UnityEngine;
using System.Collections;

public class SoloGun : Gun
{
	public Vector3 gunDirection;
	public bool isBodyMove,isRight;
	public float rotateSpeed;
	Transform body;
	Quaternion rotate;
	Transform bulletSpawn;
	void Start(){
		//Invoke ("SetPosition", 1);
		Invoke ("makeStart", 1);
		sound = GetComponent<AudioSource> ();
	}
	void makeStart(){
		body = findBody (0);
		bulletSpawn = findSpawnPoint (body);
		rotate = transform.rotation;//* Quaternion.Euler (gunDirection-new Vector3(0,0,90));
		body.rotation = rotate;
		InvokeRepeating ("shoot", 0, rechargeTime);
	}
	void calcRot(){
		if (isRight) {
			gunDirection.x += 15;
			if (gunDirection.x >= 15)
				isRight = false;
		} else {
			gunDirection.x -= 15;
			if (gunDirection.x <= -15)
				isRight = true;
		}
		rotate = transform.rotation * Quaternion.Euler (gunDirection);
		body.rotation = rotate;
	}
	protected override void beforeDestroy ()
	{
		CancelInvoke ();
	}

	void shoot(){
		if(PlayerPrefs.GetString("sound")=="True")
		sound.Play ();
		Instantiate (bulletPrefab, bulletSpawn.position,rotate).GetComponent<Bullet>().Direction = bulletSpawn.transform.up;
		if(isBodyMove)
		Invoke ("calcRot", rotateSpeed);
	}
}

