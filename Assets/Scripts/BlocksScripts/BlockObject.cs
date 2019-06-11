using UnityEngine;
using System.Collections;
public enum BLockType{
	Ground =0,
	Collum =1,
	Special =2,
	Gun = 3,
	IronBlock = 4,
	Rails = 5,
	Enemy = 6,
	Spikes = 7
}

public enum RotType{
	Z =0,
	X =1
}
public class BlockObject : MonoBehaviour
{
	public int scoreForDig;
	public BLockType blockType;
	public RotType rotType;
	public Sprite sprite;
	public bool canDestroyInBuildMode,canRotate;
	public string blockName;
	public static int count = 1;
	public int id,health=1,index;
	public bool canBroken,canJump;
	public MeshFilter meshFilter;
	public Mesh[] statesMesh;
	public bool isEnemy;
	void Awake(){
		if(meshFilter==null)
		meshFilter = GetComponent<MeshFilter> ();
	}
	public void digThis(){
		if (!isEnemy) {
			BlanksManager.instance.blockIdText.text = index.ToString ();
			health--;
			if (health < 0)
				Destroy (this.gameObject);
			else
				meshFilter.mesh = statesMesh [health];
		}
	}
	protected virtual void beforeDestroy(){}
	public virtual void resetAfterRotate(){
	}
	public void destroyThis(){
		beforeDestroy ();
		//BlanksManager.createdBlock.Remove (this);
		gameObject.SetActive(false);
		//Destroy (gameObject,1);
	}
}

