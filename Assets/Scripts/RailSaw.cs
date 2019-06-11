using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RailSaw : MonoBehaviour {
	public Transform target,sawBlock;

	RailBlock nextBlock;
	RailDirectionEnum currentDirections;
	public List<RailDirection> avaibleDir = new List<RailDirection>();
	public float speed;
	public bool canMove=true;
	public void GetStartData(RailBlock block,RailDirectionEnum firstDir){
		nextBlock = block;
		target = block.transform;
		currentDirections = firstDir;
	}
	void FixedUpdate(){
		if (canMove) {
			transform.position = Vector3.MoveTowards (transform.position, target.position, speed * Time.fixedDeltaTime);

			if (transform.position == target.position) {
				findAvaibleDir ();
			}

		}
	}
	void findAvaibleDir(){
		canMove = false;
		avaibleDir = new List<RailDirection> ();
		RailDirectionEnum inverseDir = findInverseDir ();
		if (nextBlock.directions.Length != 1) {
			for (int i = 0; i < nextBlock.directions.Length; i++) {
				if (nextBlock.directions [i].direction != inverseDir && nextBlock.directions [i].NeighborBlock != null)
					avaibleDir.Add (nextBlock.directions [i]);
			}
			if (avaibleDir.Count == 0)
				Invoke ("findAvaibleDir", 0.5f);
			else
				findNewDirection ();
		} else {
			if (nextBlock.directions [0].NeighborBlock != null){
				avaibleDir.Add (nextBlock.directions [0]);
				findNewDirection();
			}
			else
				Invoke ("findAvaibleDir", 0.5f);
		}
	}
	void findNewDirection(){
		int randomDir = 0;
		if (nextBlock.directions.Length == 1 || avaibleDir.Count == 1)
			randomDir = 0;
		else {
			randomDir = Random.Range (0, avaibleDir.Count);
		}
		nextBlock = avaibleDir [randomDir].NeighborBlock;
		currentDirections = avaibleDir [randomDir].direction;
		target = avaibleDir [randomDir].NeighborBlock.transform;
		sawBlock.rotation = Quaternion.Euler (0, 0, 90*avaibleDir[randomDir].VecDirection.y);
		canMove = true;
	}
	RailDirectionEnum findInverseDir(){
		int temp = (int)currentDirections+2;
		if (temp >= 4)
			temp = temp-4;
		return (RailDirectionEnum)temp;
	}



	public RailDirectionEnum CurrentDirections {
		get {
			return currentDirections;
		}
		set {
			currentDirections = value;
		}
	}

	public RailBlock NextBlock {
		get {
			return nextBlock;
		}
		set {
			nextBlock = value;
		}
	}
}
