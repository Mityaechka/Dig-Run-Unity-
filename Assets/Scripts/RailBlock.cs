using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RailBlock : BlockObject {
	public RailDirection[] directions;
	public RailSaw saw;
	void Start(){

		for (int i = 0; i < directions.Length; i++) {
			directions [i].SetDef (this);
			directions [i].setRotare ();
			directions [i].findNeighboringBlock ();
		}

		if (saw != null) {
			saw.transform.position = transform.position;
			saw.GetStartData (this, directions [0].direction);
		}

	}
	public override void resetAfterRotate ()
	{
		base.resetAfterRotate ();

		for (int i = 0; i < directions.Length; i++) {
			
			if (directions [i].NeighborBlock != null) {
				 //Debug.Log (findInverseDirInNeighboringBlock (directions [i].NeighborBlock, directions [i].direction).direction);
				 findInverseDirInNeighboringBlock (directions [i].NeighborBlock, directions [i].direction).NeighborBlock = null;
			}
			directions [i].setRotare ();
			directions [i].NeighborBlock = null;
			directions [i].findNeighboringBlock ();
		}

		if (saw != null) {
			saw.transform.position = transform.position;
			saw.GetStartData (this, directions [0].direction);
		}
	}
	RailDirection findInverseDirInNeighboringBlock(RailBlock block,RailDirectionEnum direction){
		for (int i = 0; i < block.directions.Length; i++) {
			int temp = (int)block.directions[i].direction+2;
			if (temp >= 4)
				temp = temp-4;
			if (direction == (RailDirectionEnum)temp) {
				return block.directions [i];

			}
		}
		return null;
	}
}
