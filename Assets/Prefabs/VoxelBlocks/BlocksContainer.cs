using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlocksContainer : MonoBehaviour {
	public  GameObject[] blocks;

	public static BlocksContainer instanse;

	void Awake(){
		instanse = this;
	}
}
