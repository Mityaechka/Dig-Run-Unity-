using UnityEngine;
using System.Collections;
public class EnemyObject : BlockObject
{
	public static int count = 1;
	public float speed;
	 Vector3 nextPosToMove,nextPosToMoveCorner;
	public LayerMask mask;
	Vector3 lastDir, waitDir;
	bool isWall,isOnCorner;
	bool isWork;
	void Start(){
		if(BuildManager.instance!=null)
		gameObject.layer = 8;
		checkSpace (ref nextPosToMove);
		lastDir = Vector3.up;
		waitDir = Vector3.right;
		if (BuildManager.instance == null)
			isWork = true;
		else
			isWork = false;
	}
	void OnTriggerEnter(Collider coll){
		if (coll.tag == "Kick") {
			getDamage (1);
		}
	}
	void Update(){
		if (isWork) {
			if (isWall)
				transform.position = Vector3.MoveTowards (transform.position, nextPosToMove, speed * Time.deltaTime);
			else if (!isWall) {
				if (transform.position == nextPosToMoveCorner && isOnCorner) {
					isOnCorner = false;
				}
				if (isOnCorner) {
					transform.position = Vector3.MoveTowards (transform.position, nextPosToMoveCorner, speed * Time.deltaTime);
				} else {
					transform.position = Vector3.MoveTowards (transform.position, nextPosToMove, speed * Time.deltaTime);
				}
			} 
			if (transform.position == nextPosToMove) {
				checkSpace (ref nextPosToMove);
			}
		}
	}
	public void getDamage(int damage){
		health -= damage;
		if (health <= 0)
			Destroy (gameObject);
	}
	bool checkSpace(ref Vector3 nextPos){
		Vector3 startPos = transform.position;
		GameObject lastBlock = this.gameObject;
		bool findPlace = false;
		checkWall (startPos,ref nextPos ,ref findPlace, Vector3.up, -Vector3.right);
		if (!findPlace) {
			checkWall (startPos,ref nextPos,ref findPlace, Vector3.right, Vector3.up);
			//return true;
		}
		if (!findPlace) {
			checkWall (startPos,ref nextPos,ref findPlace, -Vector3.up, Vector3.right);
			//return true;
		}
		if (!findPlace) {
			checkWall (startPos,ref nextPos,ref findPlace, -Vector3.right, -Vector3.up);
			//return true;
		}
		if (!findPlace) {
			checkCorner (startPos,ref nextPos,ref findPlace,ref lastBlock, -Vector3.right, Vector3.up);
			//return true;
		}
		if (!findPlace) {
			checkCorner (startPos,ref nextPos,ref findPlace,ref lastBlock, -Vector3.up, -Vector3.right);
		}
		if (!findPlace) {
			checkCorner (startPos,ref nextPos,ref findPlace,ref lastBlock, Vector3.right, -Vector3.up);
		}
		if (!findPlace) {
			checkCorner (startPos,ref nextPos,ref findPlace,ref lastBlock, Vector3.up, Vector3.right);
		}
		if (!findPlace) {
			nextPos = startPos - lastDir;
			lastDir = -lastDir;
			findPlace = true;
		}
		if (findPlace)
			return true;
		else
			return false;
	}
	void checkCorner(Vector3 startPos,ref Vector3 nextPos,ref bool findPlace,ref GameObject lastBlock,Vector3 firstDir,Vector3 secondDir){
		RaycastHit hit;
		Vector3 direcrion = startPos + 0.8f*(firstDir + secondDir);
		bool move = false;
		if (Physics.Linecast (startPos, startPos+firstDir, out hit,mask)) {
			Debug.DrawLine (startPos, startPos+firstDir, Color.yellow, 5);
			Debug.DrawLine (startPos, direcrion, Color.yellow, 5);
			if (checkLayer (hit)) {
				if (!Physics.Linecast (startPos,  direcrion,mask)) {
						move = true;
				}
			}
		}
		if (move) {
			isOnCorner = true;
			isWall = false;
			nextPosToMoveCorner = startPos + secondDir;
			nextPos = startPos + (firstDir + secondDir);
			findPlace = true;

			lastDir = firstDir;
			if (firstDir == Vector3.right || firstDir == -Vector3.right)
				waitDir = Vector3.up;
			else
				waitDir = Vector3.right;
		}
	}
	void checkWall(Vector3 startPos,ref Vector3 nextPos,ref bool findPlace,Vector3 firstDir,Vector3 secondDir){
		RaycastHit hit;
		bool move = false;
		Vector3 direcrion = startPos + 0.8f*(firstDir + secondDir);
		//Debug.Log (firstDir);
		if (Physics.Linecast (startPos, startPos + secondDir, out hit, mask)) {
			if (checkLayer (hit)) {
				Debug.DrawLine (startPos, startPos+secondDir, Color.gray, 5);
				if (Physics.Linecast (startPos, direcrion, out hit, mask)) {
					Debug.DrawLine (startPos, direcrion, Color.blue, 5);
					Debug.DrawLine (startPos, startPos + firstDir, Color.blue, 5);
					if (checkLayer (hit)) {
						if (!Physics.Linecast (startPos, startPos + firstDir, mask)) {
							if (lastDir == firstDir) {
								move = true;
							} else if(waitDir==firstDir||waitDir==-firstDir) {
								move = true;
							}
						}
					}
				}
			}
		}
		if (move) {
			isWall = true;
			nextPos = startPos + firstDir;
			findPlace = true;
			lastDir = firstDir;
			if (firstDir == Vector3.right || firstDir == -Vector3.right)
				waitDir = Vector3.up;
			else
				waitDir = Vector3.right;
		}
	}
	bool checkLayer(RaycastHit hit){
		if (hit.transform.gameObject.layer == 8){
			return true;
		}
		else
			return false;
	}
}

/*if (Physics.Linecast (startPos, startPos - Vector3.right + Vector3.up, out hit)) {
	Debug.DrawLine (startPos, startPos - Vector3.right + Vector3.up, Color.blue, 5);
	Debug.DrawLine (startPos, startPos + Vector3.up, Color.blue, 5);
	if (checkLayer (hit, ref lastBlock)) {
		if (!Physics.Linecast (startPos, startPos + Vector3.up)) {
			nextPos = startPos + Vector3.up;
			findPlace = true;
			return true;
		}
	}
}*/