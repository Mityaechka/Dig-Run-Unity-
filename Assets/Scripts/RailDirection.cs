using UnityEngine;
using System.Collections;
public enum RailDirectionEnum
{
	Up =0,
	Down = 2,
	Right = 1,
	Left = 3
}
[System.Serializable]
public class RailDirection
{
	RailBlock parent,neighborBlock;
	public RailDirectionEnum direction;
	RailDirectionEnum defDirection;
	Vector3 vecDirection;
	public Vector3 offset;
	public void SetDef(RailBlock block){
		defDirection = direction;
		parent = block;
	}




	public void setRotare(){
		int rotTime = (int)defDirection + (int)parent.transform.eulerAngles.z / 90;

		if (rotTime >= 4)
			rotTime = rotTime-4;

		direction = (RailDirectionEnum)rotTime;

		switch (direction) {
		case(RailDirectionEnum.Up):
			vecDirection = Vector3.up;
			break;
		case(RailDirectionEnum.Down):
			vecDirection = -Vector3.up;
			break;
		case(RailDirectionEnum.Right):
			vecDirection = Vector3.right;
			break;
		case(RailDirectionEnum.Left):
			vecDirection = -Vector3.right;
			break;
		}
	}
	 public void findNeighboringBlock(){
		RaycastHit hit;
		Debug.DrawLine (parent.transform.position+offset,parent.transform.position+vecDirection+offset, Color.red, 5);
		if (Physics.Raycast (parent.transform.position + offset, vecDirection + offset, out hit, 1)) {
			if (hit.transform.GetComponent<RailBlock> () != null) {
				neighborBlock = hit.transform.GetComponent<RailBlock> ();
				findInverseDirInNeighboringBlock (neighborBlock);

			}
		}
	}
	void findInverseDirInNeighboringBlock(RailBlock block){
		for (int i = 0; i < block.directions.Length; i++) {
			int temp = (int)block.directions[i].direction+2;
			if (temp >= 4)
				temp = temp-4;
			if (direction == (RailDirectionEnum)temp) {
				block.directions [i].neighborBlock = parent;
				return;
			}
		}
		neighborBlock = null;
		return;
	}
	
	public RailBlock NeighborBlock {
		get {
			return neighborBlock;
		}
		set {
			neighborBlock = value;
		}
	}

	public RailBlock Parent {
		get {
			return parent;
		}
		set {
			parent = value;
		}
	}

	public Vector3 VecDirection {
		get {
			return vecDirection;
		}
		set {
			vecDirection = value;
		}
	}
}

