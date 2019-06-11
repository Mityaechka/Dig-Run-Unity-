using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockList : MonoBehaviour {
	public  GameObject[] allBlocks;
	public GameObject[] allEnemy;
	public GameObject chest;
	public int spawnChestRate;
	public static BlockList instance;
	void Awake(){
		instance = this;
	}
}
