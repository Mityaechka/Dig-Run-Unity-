using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Constructor : MonoBehaviour {

	public LevelBlank blank;
	public Vector3 startPos;
	public int x,y;
	public GameObject parent;
	void Update () {
		if(Input.GetKeyDown(KeyCode.T))
			BlanksManager.saveBlank(blank,startPos,x,y);
		if(Input.GetKeyDown(KeyCode.Y)) 
			BlanksManager.createBlankNoTimer(blank,startPos,parent);
		if (Input.GetKeyDown (KeyCode.U)) {
			BlanksManager.createBlankNoTimer (blank, startPos, parent);
			startPos.y -= y;
		}
	}
}
