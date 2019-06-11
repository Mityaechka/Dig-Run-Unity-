using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
public class BlanksManager : NetworkBehaviour {

	public BlanksList serverBlanks;
	public GameObject startTimer,waring;
	public LayerMask idolMask;
	string[] stringBlank = null;
	public GameObject defBlank;

	public Text blockIdText;
	public LevelBlank blank;
	public LevelBlank[] availableBlanks;
	public List<int> earlyBlank = new List<int>();
	public int maxBlanksInMemory;
	public Vector3 cursor;

	public int x,y;
	public int maxLength;

	public GameObject[] blocks;
	public GameObject emptyGM;

	public int blanksOnStart;

	public static BlanksManager instance;



	public int newBlank;

	public int[] startBlankList;

	bool isStartBuild;

	public float minDistToBorder;

	public static List<BlockObject> createdBlock = new List<BlockObject>();
	public  List<GameObject> createdChest = new List<GameObject>();
	public  List<GameObject> createdEquip = new List<GameObject>();
	public static List<EnemyObject> createdEnemies = new List<EnemyObject>();
	public float idolSpawnChace;
	public GameObject idolPrefab;
	public bool isStartBuield;

	public Image mask;

	public SimpleInputNamespace.Joystick joystic;

	bool checkPlaceChest;
	public CameraPivot pivot;

	public int getBlanksCount;
	void Awake(){

		DontDestroyOnLoad (defBlank);
	//	pivot = FindObjectOfType<CameraPivot> ();
		instance = this;
		DontDestroyOnLoad (gameObject);
	}

	public void createStart (){
		if (hasAuthority) {
			startBlankList = new int[blanksOnStart];
			for (int i = 0; i < blanksOnStart; i++) {
				startBlankList[i] = choiceBlank ();
			}
		} 
		//StartCoroutine("WaitFrame");
	}
	public void GetRequestToCreateBlank(PlayerControll sendBy){
		int r = choiceBlank ();
		RpcCreateNewBlank (r);
	}


	public void createStartServerBlanks(){
		AudioManager.instance.playSound ("MainTheme", true,MyNetManager.instance.myPLayer.id);
		defBlank.SetActive (false);
		for (int i = 0; i < startBlankList.Length; i++) {
			createServerBlankNoTimer (serverBlanks.serverBlanks [startBlankList [i]]);
		}
	}



	[Command]
		public void  CmdSayServerBorderReaced(int index){
		RpcCreateNewBlank (index);
	}

	[ClientRpc]
	void RpcCreateNewBlank(int index){
		//blank = availableBlanks [index];
		createBlank (index);
		//createBlank ();
	}
	[ClientRpc]
	public void RpcEndGame(){
		MyNetManager.instance.EndGame ();
	}
	public int choiceBlank(){
		bool r = true;
		int newBlank = 0;
		while (r) {
			bool t = true;
			newBlank = Random.Range (0, serverBlanks.serverBlanks.Count);
			for (int i = 0; i < earlyBlank.Count; i++) {
				if(t){
					if (earlyBlank [i] == newBlank)
						t = false;
					}
			}
			if(t==true)
			r = false;
		}

		if (earlyBlank.Count >= maxBlanksInMemory)
			earlyBlank.RemoveAt (0);
		earlyBlank.Add (newBlank);
		return newBlank;
	}

	IEnumerator WaitFrame(){
		//yield return new WaitForSeconds(0.25f);
		yield return new WaitForEndOfFrame();

			MyNetManager.instance.myPLayer.CmdSendRequestForStartBlank ();

		yield break;
	}
	public static void saveBlank(LevelBlank _blank,Vector3 _cursor,int x,int y){
		Vector3 startPos = _cursor;
		_blank.set(x,y);
		for (int i = 0; i < _blank.yLength; i++) {
			Vector3 r = startPos-Vector3.forward*2;
			for (int j = 0; j < _blank.xLength; j++) {
				RaycastHit _hit;
				if (Physics.Linecast (r, r + Vector3.forward * 10, out _hit)) {
					if (_hit.transform.gameObject.layer == 8) {
						_blank.addBlock (
							(int)(_hit.transform.position.x - _cursor.x),
							(int)(_hit.transform.position.y - _cursor.y),
							_hit.transform.GetComponent<BlockObject> ().id);
					}else if (_hit.transform.gameObject.layer == 10) {
						_blank.addEnemy (
							(int)(_hit.transform.position.x - _cursor.x),
							(int)(_hit.transform.position.y - _cursor.y),
							_hit.transform.GetComponent<EnemyObject> ().id);
					}
				} 
				r += Vector3.right;
			}
			startPos -= Vector3.up;
		}
	}
	public void createBlank(){
		IEnumerator courtine = createRow (0.002f);
		StartCoroutine (courtine);
		cursor -= Vector3.up * (blank.yLength-1);
		}
	public void createBlank(int blankId){
		IEnumerator courtine = createServerRow (0.002f,serverBlanks.serverBlanks[blankId]);
		StartCoroutine (courtine);
		cursor -= Vector3.up * (serverBlanks.serverBlanks[blankId].ySize-1);
	}
	void checkPlaceForIdol(GameObject go){
		
		float chance = Random.Range (0.0f, 1.0f);
		if (go.GetComponent<BlockObject>().blockType != BLockType.Rails&&hasAuthority) {
			if (!Physics.Raycast (go.transform.position,  transform.up,2,idolMask)&&chance < idolSpawnChace) {
				RpcCreateIdol (go.transform.position);
			}
		}
	}
	[ClientRpc]
	void RpcCreateChest(Vector3 blockPosition,int id){
		GameObject go =  Instantiate (BlockList.instance.chest, blockPosition + Vector3.up, Quaternion.identity);
		go.GetComponent<Chest> ().equipmentId = id;
		checkPlaceChest = false;
		go.GetComponent<BlockObject> ().index = createdChest.Count;
		createdChest.Add (go);
	}
	void checkPlaceForChest(GameObject go){

		if (go.GetComponent<BlockObject>().blockType!=BLockType.Rails&&hasAuthority&&checkPlaceChest) {
			if (!Physics.Raycast (go.transform.position,  transform.up,2)) {
				RpcCreateChest (go.transform.position,Random.Range(0,EquipmentManager.instance.equipmentObject.Length));
			}
		}
	}
	static public int findInListByIndex(int index){
		int r = 0,i = 0;
		foreach (BlockObject block in createdBlock) {
			if (block.index == index) {
				r = i;
				break;
			}
			i++;
		}
		return r;
	}
	[ClientRpc]
	void RpcCreateIdol(Vector3 blockPosition){
		Debug.DrawLine(blockPosition+Vector3.up,blockPosition+Vector3.up*2,Color.yellow,10);
		GameObject go = Instantiate (idolPrefab, blockPosition + Vector3.up, Quaternion.identity);
		//BlockObject.count++;
		//go.GetComponent<BlockObject> ().index = BlockObject.count;
		//Debug.Log (go.GetComponent<BlockObject> ().index);
		//createdBlock.Add (go.GetComponent<BlockObject> ());
	}

	public void createServerBlankNoTimer(ServerBlank blank){
		Transform parent = Instantiate (emptyGM, cursor, Quaternion.identity).transform;
		Vector3 r = cursor;
		for (int i = 0; i < blank.fields.Count; i++) {
			ServerBlock b = blank.fields [i];
			GameObject go = Instantiate (BlocksContainer.instanse.blocks[b.blockID], new Vector3 (r.x + b.x, r.y + b.y, 0), b.rotate,parent);
			//checkPlaceForIdol (go.transform.position);
			BlockObject.count++;
			go.GetComponent<BlockObject> ().index = BlockObject.count;
			createdBlock.Add (go.GetComponent<BlockObject> ());

		}
		cursor -= Vector3.up * (blank.ySize-1);
	}

	///<summary>
	/// dfsfsdf
	/// </summary>
	[ClientRpc]
	public void RpcSetEndGameTimer(int time){
		MyNetManager.instance.showEndTable (time);
	}
	[ClientRpc]
	public void RpcDisableEndGameTimer(){
		MyNetManager.instance.disableEndTable ();
	}
	public void loadBuildScene(){
		SceneManager.LoadScene (2);
	}
	public void createBlankNoTimer(){
		Transform parent = Instantiate (emptyGM, cursor, Quaternion.identity).transform;
		Vector3 r = cursor;
		LevelBlank lb = blank;
		for (int i = 0; i < lb.fields.Count; i++) {
			Block b = blank.fields [i];
			GameObject go = Instantiate (b.prefab, new Vector3 (r.x + b.x, r.y + b.y, 0), b.prefab.transform.rotation,parent);
				//checkPlaceForIdol (go.transform.position);
			BlockObject.count++;
			go.GetComponent<BlockObject> ().index = BlockObject.count;
			Debug.Log (go.GetComponent<BlockObject> ().index);
			createdBlock.Add (go.GetComponent<BlockObject> ());

		}
		for (int i = 0; i < lb.enemies.Count; i++) {
			Enemy e = blank.enemies [i];
			GameObject go = Instantiate (e.prefab, new Vector3 (r.x + e.x, r.y + e.y, 0), Quaternion.identity,parent);
			EnemyObject.count++;
			go.GetComponent<EnemyObject> ().index = EnemyObject.count;
			createdEnemies.Add (go.GetComponent<EnemyObject> ());
		}

		cursor -= Vector3.up * (blank.yLength-1);
	}
	public static void createBlankNoTimer(LevelBlank _blank,Vector3 _cursor,GameObject _emptyGM){
		Transform parent = Instantiate (_emptyGM, _cursor, Quaternion.identity).transform;
		Vector3 r = _cursor;
		LevelBlank lb = _blank;
		for (int i = 0; i < lb.fields.Count; i++) {
			Block b = _blank.fields [i];
			Instantiate (b.prefab, new Vector3 (r.x + b.x, r.y + b.y-1, 0), Quaternion.identity).transform.parent = parent;
		}
	}
	IEnumerator createRow(float delay){
		Transform parent = Instantiate (emptyGM, cursor, Quaternion.identity).transform;
		Vector3 r = cursor;
		LevelBlank _blank = blank;
		for (int i = 0; i < _blank.fields.Count; i++) {
			Block b = _blank.fields [i];
			GameObject go = Instantiate (b.prefab, new Vector3 (r.x + b.x, r.y + b.y, 0), b.prefab.transform.rotation,parent);
			if(b.prefab.gameObject.tag!="Spike")
			checkPlaceForIdol (go);
			if (BlockObject.count % BlockList.instance.spawnChestRate == 0) {
				checkPlaceChest = true;
			}
			if (checkPlaceChest) {
				checkPlaceForChest (go);
			}

			BlockObject.count++;

			go.GetComponent<BlockObject> ().index = BlockObject.count;
			createdBlock.Add (go.GetComponent<BlockObject> ());
			yield return new WaitForSeconds (delay);

			//yield return new WaitForEndOfFrame();
		}
		for (int i = 0; i < _blank.enemies.Count; i++) {
			Enemy e = _blank.enemies [i];
			GameObject go = Instantiate (e.prefab, new Vector3 (r.x + e.x, r.y + e.y, 0), Quaternion.identity,parent);
			EnemyObject.count++;
			go.GetComponent<EnemyObject> ().index = EnemyObject.count;
			createdEnemies.Add (go.GetComponent<EnemyObject> ());
		}
		yield break;
	}

	IEnumerator createServerRow(float delay,ServerBlank sBlank){
		Transform parent = Instantiate (emptyGM, cursor, Quaternion.identity).transform;
		Vector3 r = cursor;
		for (int i = 0; i < sBlank.fields.Count; i++) {
			ServerBlock b = sBlank.fields [i];
			GameObject go = Instantiate (BlocksContainer.instanse.blocks[b.blockID], new Vector3 (r.x + b.x, r.y + b.y-1, 0), b.rotate,parent);
			if(BlocksContainer.instanse.blocks[b.blockID].tag!="Spike")
				checkPlaceForIdol (go);
			if (BlockObject.count % BlockList.instance.spawnChestRate == 0) {
				checkPlaceChest = true;
			}
			if (checkPlaceChest) {
				checkPlaceForChest (go);
			}

			BlockObject.count++;

			go.GetComponent<BlockObject> ().index = BlockObject.count;
			createdBlock.Add (go.GetComponent<BlockObject> ());
			yield return new WaitForSeconds (delay);

			//yield return new WaitForEndOfFrame();
		}
		yield break;
	}

	public string[] StringBlank {
		get {
			return stringBlank;
		}
		set {
			stringBlank = value;
		}
	}
}
