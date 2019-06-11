using UnityEngine;
using System.Collections;
[System.Serializable]
public class Enemy 

{
	public int x,y;
	public GameObject prefab;
	public  Enemy(int _x,int _y,int id){
		x = _x;
		y = _y;
		prefab = BlockList.instance.allEnemy [id];
	}
}

