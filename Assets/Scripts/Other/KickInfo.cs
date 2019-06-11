using UnityEngine;
using System.Collections;

public class KickInfo : MonoBehaviour
{

	public AlivePlayer parent;
	public Collider _collider;
	void Start(){
		//parent = MyNetManager.instance.myPLayer.alive;
	}
	public int getDir(){
		return parent.getLookDir ();
	}
}

