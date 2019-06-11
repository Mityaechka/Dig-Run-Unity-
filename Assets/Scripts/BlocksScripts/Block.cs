using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class Block{

	public int x,y;
	public GameObject prefab;
	public  Block(int _x,int _y,int id){
		x = _x;
		y = _y;
		prefab = BlockList.instance.allBlocks [id];
	}
	public  Block(int _x,int _y,int id,bool temp){
		x = _x;
		y = _y;
		prefab = BlocksContainer.instanse.blocks [id];
	}
}
