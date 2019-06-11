using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointTrigger : MonoBehaviour {

	int x;
	int y;
	public GameObject child;
	bool runOnPC;
	public void setPos(int x,int y){
		this.x = x;
		this.y = y;
	}
	void Awake(){
		if (Application.platform != RuntimePlatform.Android)
		{
			runOnPC = true;
		}
	}
	public void OnMouseOver()
	{
		if (!runOnPC) {
			if (BuildManager.instance.touchPhase == TouchPahaseConstr.Перетаскивание)
				BuildManager.instance.touch (this);
		} else {
			if (Input.GetKey (KeyCode.LeftAlt)) {
				BuildManager.instance.touchPhase = TouchPahaseConstr.Перетаскивание;
				if (!Input.GetKey (KeyCode.LeftShift)) {
					BuildManager.instance.phase = BuildPhase.Строительство;
				} else {
					BuildManager.instance.phase = BuildPhase.Разрушения;
				}
				BuildManager.instance.touch (this);
			}
		}
	}
	public void OnMouseUp()
	{
		if(!runOnPC)
		if(BuildManager.instance.touchPhase == TouchPahaseConstr.Касание)
			BuildManager.instance.touch (this);
	}
	public void OnMouseDown()
	{
		if (runOnPC) {
			BuildManager.instance.touchPhase = TouchPahaseConstr.Касание;
			if (!Input.GetKey (KeyCode.LeftAlt)) {
				if (!Input.GetKey (KeyCode.LeftShift)) {
					BuildManager.instance.phase = BuildPhase.Строительство;
				} else {
					BuildManager.instance.phase = BuildPhase.Разрушения;
				}
				BuildManager.instance.touch (this);
			}
		}
	}
	public int X {
		get {
			return x;
		}
	}

	public int Y {
		get {
			return y;
		}
	}
}
