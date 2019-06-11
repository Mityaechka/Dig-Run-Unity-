using UnityEngine;
using System.Collections;


[System.Serializable]
public class ServerBlock 
{
	public int x,y;
	public int blockID;
	public bool canDestroyInBuildMode;
	public Quaternion rotate;
	public  ServerBlock(int _x,int _y,int id,Quaternion val){
		x = _x;
		y = _y;
		blockID = id;
		rotate = val;
	}
}

