using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

using System;
using System.Threading;
using System.Threading.Tasks;
using Proyecto26;
using UnityEngine.UI;

public enum BuildPhase
{
	Строительство = 0,
	Разрушения = 1
}
public enum MirrorPhase
{
	X = 0,
	Y = 1,
	XAndY = 2,
	None = 3
}
public enum TouchPahaseConstr
{
	Касание  = 0,
	Перетаскивание = 1
}
public class BuildManager : MonoBehaviour {
	public Color publishColor,notPublishColor;
	public GameObject signMask, packsMask,signError,packsError,menuPackBtn,menuPackObj,savePackBtn,savePackObj,saveErr,saveMsg,publishBtn,publishObj;
	public LayerMask mask;
	bool firstDraw;
	public Button[] touchModeBtn;
	public Dropdown buildDropdown,touchPhaseDropdown;
	public Toggle[] mirrorTogles;
	public MirrorPhase mirrorPhase;
	[HideInInspector]
	public TouchPahaseConstr touchPhase;
	public InputField emailField,passwordField,usernameField,emailEnterField,passwordEnerField;
	public Toggle rememberToggle;
	public GameObject signMenu,greetingMenu,addBLankMenu;
	public int zPos,currentId;
	public GameObject pointPrefab,blankAdapterPrefab,packAdapterPrefab,blockAdapterBrefab,emptyPrefab,addMenu,addBlankBtn,backPacksBtn,deletePackMenu,deleteBlankMenu;
	GameObject blankObject,blankInBuild;
	Vector3 startPos;
	LineRenderer borderLine;
	public static BuildManager instance;

	public GameObject addPackBtnPref, addPackBtnObj,addBlankObj,backPacksObj;
	public BuildPhase phase;
	public Button saveBtn,saveNewBtn,buildBtn,destroyBtn;
	public ServerBlank blank;

	public InputField blankNameField;

	public GameObject blankAdaptersContainer,blockAdaptersContainer;

	public BlankAdapter[] adapters;
	public BlockAdapter[] blocksAdapters;
	public PackAdapter[] packAdapters;
	public ServerBlank[] dowloadBlanks;

	bool isFresh;
	public int selectedId;

	private string AuthKey = "AIzaSyB1K-946AMEPdf724mccOkvF84cbKCZ4gg";

	[HideInInspector]
	public int openPackId;

	string packName;
	public User user;

	List<GameObject> createdBlocks = new List<GameObject>();
	GameObject[,] gridBlocks;

	List<int> blanksForDel = new List<int>();

	public Dictionary<BLockType, BlockAdapter> typesAdapters = new Dictionary<BLockType, BlockAdapter>();


	void Start () {
		
		List<string> buildPhaseString = new List<string>(Enum.GetNames (typeof(BuildPhase)));
		buildDropdown.AddOptions (buildPhaseString);

		buildDropdown.onValueChanged.AddListener (delegate {
			phase = (BuildPhase) buildDropdown.value;	
		});

		List<string> touchPhaseString = new List<string>(Enum.GetNames (typeof(TouchPahaseConstr)));
		touchPhaseDropdown.AddOptions (touchPhaseString);

		touchPhaseDropdown.onValueChanged.AddListener (delegate {
			touchPhase = (TouchPahaseConstr) touchPhaseDropdown.value;	
		});


		if (Prototype.NetworkLobby.LobbyManager.s_Singleton != null)
			Destroy (Prototype.NetworkLobby.LobbyManager.s_Singleton.gameObject);

		if(GameObject.Find ("DefBlank")!=null)
		GameObject.Find ("DefBlank").SetActive (false);
		gridBlocks = new GameObject[1, 1];
		instance = this;

		startPos = transform.position;
		borderLine = GetComponent<LineRenderer>();
		if (PlayerPrefs.GetString ("EMAIL") != null) {
			emailEnterField.text = PlayerPrefs.GetString ("EMAIL");
			passwordEnerField.text = PlayerPrefs.GetString ("PASSWORD");
		}
		if (UserDataPool.signUpUser != null) {
			user = UserDataPool.signUpUser;

			user.localId = UserDataPool.resp.localId;
			user.idToken = UserDataPool.resp.idToken;

			closeSignMenu();
		}
	}
	public void OnEditMirror(){
		if (mirrorTogles [0].isOn&&mirrorTogles [1].isOn)
			mirrorPhase = MirrorPhase.XAndY;
		else if (mirrorTogles [0].isOn&&!mirrorTogles [1].isOn)
			mirrorPhase = MirrorPhase.X;
		else if (!mirrorTogles [0].isOn&&mirrorTogles [1].isOn)
			mirrorPhase = MirrorPhase.Y;
		else mirrorPhase = MirrorPhase.None;
	}
	public void loadMainScene(){
		SceneManager.LoadScene (0);
	}
	
	public void touchPhasePress(int id){
		for (int i = 0; i < touchModeBtn.Length; i++) {
			if(!touchModeBtn [i].interactable)
				touchModeBtn [i].interactable = true;
		}
		touchModeBtn [id].interactable = false;
		touchPhase = (TouchPahaseConstr) id;
	}
	public void CreateBLockAdapters(){

		cleanAdapters (blocksAdapters);	

		blocksAdapters = new BlockAdapter[BlocksContainer.instanse.blocks.Length];

		for (int i = 0; i < 8; i++) {
			GameObject go =  Instantiate (blockAdapterBrefab, blockAdaptersContainer.transform.position, Quaternion.identity, blockAdaptersContainer.transform);
			go.GetComponent<BlockAdapter> ().collectedType = (BLockType)i;
			typesAdapters.Add((BLockType)i,go.GetComponent<BlockAdapter>());
		}


		for (int i = 0; i < BlocksContainer.instanse.blocks.Length; i++) {
			typesAdapters [BlocksContainer.instanse.blocks [i].GetComponent<BlockObject> ().blockType].avaibleBlocks.Add (BlocksContainer.instanse.blocks[i]);
		}

		foreach (BlockAdapter adapter  in typesAdapters.Values) {
			adapter.accept ();
		}
	}
	public void CreateAdapters(List<ServerBlank> blanks,BlanksPack pack){
		if (menuPackObj != null)
			Destroy (menuPackObj);

		cleanAdapters (adapters);
		adapters = new BlankAdapter[blanks.Count];
		if (backPacksObj != null)
			Destroy (backPacksObj);
		if (addBlankObj != null)
			Destroy (addBlankObj);
		if (savePackObj != null)
			Destroy (savePackObj);
		if (publishObj != null)
			Destroy (publishObj);
		backPacksObj =  Instantiate (backPacksBtn, blankAdaptersContainer.transform.position, Quaternion.identity, blankAdaptersContainer.transform);
		backPacksObj.GetComponent<Button> ().onClick.AddListener (delegate {
			backToPackMenu();	
		});
		for (int i = 0; i < blanks.Count; i++) {
			if(!hasInListForDel(i)){
			GameObject go =  Instantiate (blankAdapterPrefab, blankAdaptersContainer.transform.position, Quaternion.identity, blankAdaptersContainer.transform);
			adapters [i] = go.GetComponent<BlankAdapter> ();
			adapters [i].set (i);
			}
		}

		savePackObj = Instantiate (savePackBtn, blankAdaptersContainer.transform.position, Quaternion.identity, blankAdaptersContainer.transform);
		savePackObj.GetComponent<Button> ().onClick.AddListener (delegate {
			saveEdit();
		});
		publishObj = Instantiate (publishBtn, blankAdaptersContainer.transform.position, Quaternion.identity, blankAdaptersContainer.transform);
		if (pack.isPublish) {
			publishObj.GetComponent<Image> ().color = publishColor;
			publishObj.transform.GetChild(0).GetComponent<Text> ().text = "ОПУБЛИКОВАНО";
		} else {
			publishObj.GetComponent<Image> ().color = notPublishColor;
			publishObj.transform.GetChild(0).GetComponent<Text> ().text = "ОПУБЛИКОВАТЬ";
		}
		if (pack.blanks.Count <= 2) {
			publishObj.GetComponent<Button> ().interactable = false;
		}
		publishObj.GetComponent<Button> ().onClick.AddListener (delegate {
			clickPublish (pack);
		});
		if (blanks.Count < 10) {
			addBlankObj = Instantiate (addBlankBtn, blankAdaptersContainer.transform.position, Quaternion.identity, blankAdaptersContainer.transform);
			addBlankObj.GetComponent<Button> ().onClick.AddListener (delegate {
				addBLankMenu.SetActive (true);
			});
		}
	}
	void clickPublish(BlanksPack pack){
		pack.isPublish = !pack.isPublish;
		if (pack.isPublish) {
			publishObj.GetComponent<Image> ().color = publishColor;
			publishObj.transform.GetChild(0).GetComponent<Text> ().text = "ОПУБЛИКОВАНО";
		} else {
			publishObj.GetComponent<Image> ().color = notPublishColor;
			publishObj.transform.GetChild(0).GetComponent<Text> ().text = "ОПУБЛИКОВАТЬ";
		}
	}
	public void saveEdit(){
		blanksForDel.Sort ();
		blanksForDel.Reverse ();
		for (int i = 0; i < blanksForDel.Count; i++) {
			user.packs [openPackId].blanks.RemoveAt (blanksForDel [i]);
		}
		blanksForDel = new List<int> ();
		saveBLocks ();
		if (user.packs [openPackId].blanks.Count <= 2) {
			publishObj.GetComponent<Button> ().interactable = false;
		}
		RestClient.Put<SignResponse> ("https://shovel-database.firebaseio.com/Users/" + user.localId + ".json?auth=" + user.idToken,
			user).Then (arg => {
				StartCoroutine(showSaveMsg());
			}).Catch(err=>StartCoroutine( saveErrMsg()));
	}
	IEnumerator saveErrMsg(){
		saveErr.SetActive (true);
		yield return new WaitForSeconds (5);
		saveErr.SetActive (false);
		yield break;
	}
	IEnumerator showSaveMsg(){
		saveMsg.SetActive (true);
		yield return new WaitForSeconds (5);
		saveMsg.SetActive (false);
		yield break;
	}
	bool hasInListForDel(int id){
		for (int i = 0; i < blanksForDel.Count; i++) {
			if (id == blanksForDel [i])
				return true;
		}
		return false;
	}
	void backToPackMenu(){
		blanksForDel.Sort ();
		blanksForDel.Reverse ();
		for (int i = 0; i < blanksForDel.Count; i++) {
			user.packs [openPackId].blanks.RemoveAt (blanksForDel [i]);
		}
		blanksForDel = new List<int> ();
		cleanAdapters (adapters);
		saveBLocks ();
		foreach (GameObject go in createdBlocks) {
			Destroy (go);
		}		
		if (publishObj != null)
			Destroy (publishObj);
		if (savePackObj != null)
			Destroy (savePackObj);
		destroyGrid ();
		createdBlocks.Clear ();
		if (backPacksObj != null)
			Destroy (backPacksObj);
		if (addBlankObj != null)
			Destroy (addBlankObj);

		packsMask.SetActive (true);
		for (int i = 0; i < user.packs.Count; i++) {
			if (user.packs [i].blanks.Count < 2)
				user.packs [i].isPublish = false;
		}

	
		RestClient.Put<SignResponse> ("https://shovel-database.firebaseio.com/Users/" + user.localId + ".json?auth=" + user.idToken,
			user).Then (arg => {
				packsMask.SetActive (false);
			UpdateUserData (CreatePackAdapters);
			}).Catch(err=>onPackErr());
	}
	public void ReloadPacks(){
		UpdateUserData (CreatePackAdapters,onPackErr);
		packsMask.SetActive (true);
		packsError.SetActive(false);
	}

	void onPackErr(){
		packsError.SetActive(true);	
		packsMask.SetActive (false);
	}
	public void destroyGrid(){
		if (gridBlocks[0,0] !=null) {
			
			for (int i = 0; i < gridBlocks.GetLength(0); i++) {
				for (int j = 0; j < gridBlocks.GetLength(1); j++) {
					if (gridBlocks [i, j].GetComponent<PointTrigger> ().child != null)
						Destroy (gridBlocks [i, j].GetComponent<PointTrigger> ().child);
					Destroy (gridBlocks [i, j]);
				}
			}
			Vector3[] emptyPoint = new Vector3[4];
			GetComponent<LineRenderer> ().SetPositions (emptyPoint);
		}
	}
	public void selectBlock(int _id){
		currentId = _id;
	}
	void buildGrid(){
		int x = user.packs [openPackId].blanks [selectedId].xSize;
		int y = user.packs [openPackId].blanks [selectedId].ySize;
		gridBlocks = new GameObject[x,y];
		Transform go = Instantiate (emptyPrefab, Vector3.zero, Quaternion.identity).transform;
		for (int i = 0; i < x; i++) {
			for (int j = 0; j <y; j++) {
				GameObject temp = Instantiate (pointPrefab, new Vector3 (startPos.x + i, startPos.y - j, 0), Quaternion.identity, go.transform);
				temp.GetComponent<PointTrigger> ().setPos (i, j);
				gridBlocks[i,j] = temp;
				}
		}
	}
	 void drawBorder(){
		Vector3[] points = new Vector3[4];
		points [0] = new Vector3(startPos.x- 0.5f,startPos.y+0.5f,zPos);
		points [1] = new Vector3(startPos.x+blank.xSize-0.5f,startPos.y+0.5f,zPos);
		points [2] = new Vector3(startPos.x+blank.xSize -0.5f,startPos.y-blank.ySize +0.5f,zPos);
		points [3] = new Vector3(startPos.x-0.5f,startPos.y-blank.ySize+0.5f,zPos);
		borderLine.SetPositions (points);
	}
	public void touch(PointTrigger point){

		if (blankInBuild == null) {
			blankInBuild = Instantiate (emptyPrefab, Vector3.zero, Quaternion.identity);
		}



		if (phase == BuildPhase.Строительство) {
			if (point.child != null) {
				rotateBlock (point,1);
				point.child.GetComponent<BlockObject> ().resetAfterRotate ();
				/*if (mirrorPhase == MirrorPhase.X) {
					int x = point.X;
					PointTrigger mirrorPoint = gridBlocks [blank.xSize - x-1, point.Y].GetComponent<PointTrigger>();
					rotateBlock (mirrorPoint,-1);
				}else if (mirrorPhase == MirrorPhase.Y) {
					int y = point.Y;

					PointTrigger mirrorPoint = gridBlocks [point.X,blank.ySize - y-1].GetComponent<PointTrigger>();
					rotateBlock (mirrorPoint,1);
				}else if (mirrorPhase == MirrorPhase.XAndY) {

					int x = point.X;
					int y = point.Y;

					PointTrigger mirrorPoint = gridBlocks [point.X,blank.ySize - y-1].GetComponent<PointTrigger>();
					rotateBlock (mirrorPoint,-1);

					mirrorPoint = gridBlocks [blank.xSize - x-1,blank.ySize - y-1].GetComponent<PointTrigger>();
					rotateBlock (mirrorPoint,-1);

					mirrorPoint = gridBlocks [blank.xSize - x-1,point.Y].GetComponent<PointTrigger>();
					rotateBlock (mirrorPoint,-1);
				}*/
			} else {
				createBlock (point);
				if (mirrorPhase == MirrorPhase.X) {
					int x = point.X;
					PointTrigger mirrorPoint = gridBlocks [blank.xSize - x-1, point.Y].GetComponent<PointTrigger>();
					createBlock (mirrorPoint);
				}else if (mirrorPhase == MirrorPhase.Y) {
					int y = point.Y;

					PointTrigger mirrorPoint = gridBlocks [point.X,blank.ySize - y-1].GetComponent<PointTrigger>();
					createBlock (mirrorPoint);
				}else if (mirrorPhase == MirrorPhase.XAndY) {

					int x = point.X;
					int y = point.Y;

					PointTrigger mirrorPoint = gridBlocks [point.X,blank.ySize - y-1].GetComponent<PointTrigger>();
					createBlock (mirrorPoint);

					mirrorPoint = gridBlocks [blank.xSize - x-1,blank.ySize - y-1].GetComponent<PointTrigger>();
					createBlock (mirrorPoint);

					mirrorPoint = gridBlocks [blank.xSize - x-1,point.Y].GetComponent<PointTrigger>();
					createBlock (mirrorPoint);
				}
			}
		} else {

			if (mirrorPhase == MirrorPhase.X) {
				int x = point.X;
				PointTrigger mirrorPoint = gridBlocks [blank.xSize - x-1, point.Y].GetComponent<PointTrigger>();
				destroyBlock (mirrorPoint);
			}else if (mirrorPhase == MirrorPhase.Y) {
				int y = point.Y;

				PointTrigger mirrorPoint = gridBlocks [point.X,blank.ySize - y-1].GetComponent<PointTrigger>();
				destroyBlock (mirrorPoint);
			}else if (mirrorPhase == MirrorPhase.XAndY) {

				int x = point.X;
				int y = point.Y;

				PointTrigger mirrorPoint = gridBlocks [point.X,blank.ySize - y-1].GetComponent<PointTrigger>();
				destroyBlock (mirrorPoint);

				mirrorPoint = gridBlocks [blank.xSize - x-1,blank.ySize - y-1].GetComponent<PointTrigger>();
				destroyBlock (mirrorPoint);

				mirrorPoint = gridBlocks [blank.xSize - x-1,point.Y].GetComponent<PointTrigger>();
				destroyBlock (mirrorPoint);
			}

			destroyBlock (point);
		}

	}
	void createBlock(PointTrigger point){
		if (point.child == null) {
			point.child = Instantiate (BlocksContainer.instanse.blocks [currentId], new Vector3 (startPos.x + point.X, startPos.y - point.Y, zPos), BlocksContainer.instanse.blocks [currentId].transform.rotation, blankInBuild.transform);
			point.child.GetComponent<BlockObject> ().canDestroyInBuildMode = true;
		}
	}
	void destroyBlock(PointTrigger point){
		if (point.child != null) {
			if (point.child.GetComponent<BlockObject> ().canDestroyInBuildMode) {
				Destroy (point.child);
			}
		}
	}
	void rotateBlock(PointTrigger point,int direction){
		if (point.child.GetComponent<BlockObject> ().canRotate)
		if (touchPhase == TouchPahaseConstr.Касание) {
			if (point.child.GetComponent<BlockObject> ().rotType == RotType.Z) {
				point.child.transform.Rotate (Vector3.forward, 90*direction, Space.Self);
			} else point.child.transform.Rotate (Vector3.right, 90*direction, Space.Self);
		}
	}
	void CreatePackAdapters(){
		packsMask.SetActive (false);
		cleanAdapters (packAdapters);
		if (addPackBtnObj != null)
			Destroy (addPackBtnObj);

		if (menuPackObj != null)
			Destroy (menuPackObj);
		
		menuPackObj = Instantiate (menuPackBtn, blankAdaptersContainer.transform.position, Quaternion.identity, blankAdaptersContainer.transform);
		menuPackObj.GetComponent<Button> ().onClick.AddListener (delegate {
			loadMainScene()	;
		});

		packAdapters = new PackAdapter[user.packs.Count];
		for (int i = 0; i < user.packs.Count; i++) {
			GameObject go = Instantiate (packAdapterPrefab, blankAdaptersContainer.transform.position, Quaternion.identity, blankAdaptersContainer.transform);
			packAdapters [i] = go.GetComponent<PackAdapter> ();
			packAdapters [i].set (i);
		}
		if (user.packs.Count < BuildManager.instance.user.maxPackCount) {
			addPackBtnObj = Instantiate (addPackBtnPref, blankAdaptersContainer.transform.position, Quaternion.identity, blankAdaptersContainer.transform);
			addPackBtnObj.GetComponent<Button> ().onClick.AddListener (delegate {
				addMenu.SetActive (true);	
			});
		}
	}
	public void deletePack(int id){
		user.packs.RemoveAt (id);
		updatePacksMenu ();
	}
	public void deleteBlank(int id){
		blanksForDel.Add (id);
		Debug.Log (id);
		Destroy(adapters[id].gameObject);
	}

	void updatePacksMenu(){
		cleanAdapters (packAdapters);
		if(addPackBtnObj!=null)
			Destroy (addPackBtnObj);
			RestClient.Put<SignResponse> ("https://shovel-database.firebaseio.com/Users/" + user.localId + ".json?auth=" + user.idToken,
				user).Then (arg => {
					UpdateUserData (CreatePackAdapters);
				}).Catch(err=>Debug.Log(err));
		}

	void removeInArray<T> (T[] array,int index,ref T[] newArray){
		 newArray = new T[array.Length-1];
		int temp = 0;
		for (int i = 0; i < array.Length; i++) {
			if (i != index) {
				newArray [temp] = array [i];
				temp++;
			}
		}
	}
	public void addNewBlank(string name,int ySize){

		user.packs [openPackId].blanks.Add ( new ServerBlank (name,blank.xSize,ySize));
		selectedId = user.packs [openPackId].blanks.Count - 1;
		createBlank (selectedId);
		openBlankMenu (openPackId);
		//blanksForDel.Remove (user.packs [openPackId].blanks.Count-blanksForDel.Count);
	}
	public void addNewPack(string name){
		BlanksPack pack = new BlanksPack (name);
		user.packs.Add (pack);
		int tempID = user.packs.Count;
		RestClient.Put<SignResponse>("https://shovel-database.firebaseio.com/Users/" + user.localId + ".json?auth=" + user.idToken,
			user).Then(arg =>{
				AddPack.instance.addBtn.interactable  =true;
				addMenu.SetActive(false);
				openBlankMenu(pack);
				UpdateUserData();
			}).Catch(err => AddPack.instance.addBtn.interactable=true);
		user.packs.Remove (pack);
	}
	public void openBlankMenu(int id){
		openPackId = id;
		cleanAdapters (packAdapters);
		CreateAdapters(user.packs[id].blanks,user.packs[id]);
		if(addPackBtnObj!=null)
			Destroy (addPackBtnObj);
		if (user.packs [id].blanks.Count > 0)
			createBlank (0);
	}
	public void openBlankMenu(BlanksPack pack){
		openPackId = user.packs.Count;
		cleanAdapters (packAdapters);
		CreateAdapters(pack.blanks,pack);
		if(addPackBtnObj!=null)
			Destroy (addPackBtnObj);

	}
	void changeBuildMode(BuildPhase val){
		phase = val;
	}
	public void SignUpNewUser(){
		SignUpUser (emailField.text, usernameField.text, passwordField.text);
	}
	public void SignInUser(){
		SignInUser (emailEnterField.text, passwordEnerField.text);
	}
	public void saveBLocks(){
		if (user.packs [openPackId].blanks.Count != 0) {
			blank.fields.Clear ();
			Vector3 startPos = this.startPos;
			for (int i = 0; i < blank.ySize; i++) {
				Vector3 r = new Vector3 (startPos.x, startPos.y, 5);
				for (int j = 0; j < blank.xSize; j++) {
					RaycastHit _hit;
					Debug.DrawLine (r, r + Vector3.forward * 50, Color.red, 2);
					if (Physics.Linecast (r, r + Vector3.forward * 50, out _hit,mask)) {

						if (_hit.transform.gameObject.layer == 8||_hit.transform.gameObject.layer == 10) {
							blank.fields.Add (new ServerBlock (
								(int)(_hit.transform.position.x - this.startPos.x),
								(int)(_hit.transform.position.y - this.startPos.y),
								_hit.transform.GetComponent<BlockObject> ().id,
								_hit.transform.rotation));
						}
					} 
					r += Vector3.right;
				}
				startPos -= Vector3.up;
			}
			user.packs [openPackId].blanks [selectedId] = blank;
		}
	}

	public void cleanAdapters(MainAdapter[] adapters){
		for (int i = 0; i < adapters.Length; i++) {
			if(adapters[i]!=null)
			Destroy (adapters[i].transform.gameObject);
		}
	}


	public void createBlank(int blankId){
		createdBlocks.Clear ();
		destroyGrid ();
		blank =user.packs[openPackId].blanks[blankId];
		selectedId = blankId;
		drawBorder ();

		buildGrid ();


		if (blankInBuild != null)
			Destroy (blankInBuild);
		if (blankObject != null)
			Destroy (blankObject);
		blankObject = Instantiate (emptyPrefab, Vector3.zero, Quaternion.identity);
		Vector3 r = startPos;

		for (int i = 0; i < blank.fields.Count; i++) {
			ServerBlock b = blank.fields [i];
			GameObject go = Instantiate (BlocksContainer.instanse.blocks [b.blockID], new Vector3 (r.x + b.x, r.y + b.y, zPos), b.rotate, blankObject.transform);
			gridBlocks [b.x, -b.y].GetComponent<PointTrigger>().child = go;
			createdBlocks.Add (go);
			}
		if(gridBlocks[0,0].GetComponent<PointTrigger>().child==null){
			for (int i = 0; i < blank.xSize; i++) {
				for (int j = 0; j < blank.ySize; j++) {
					if (i == 0 || i == blank.xSize-1) {
						GameObject go = Instantiate (BlocksContainer.instanse.blocks [0], new Vector3 (r.x + i, r.y - j, zPos), BlocksContainer.instanse.blocks [0].transform.rotation, blankObject.transform);
						gridBlocks [i, j].GetComponent<PointTrigger>().child = go;
						createdBlocks.Add (go);
					}
				}
			}
		}
	}

	private void SignUpUser(string email, string username, string password)
	{
		string userData = "{\"email\":\"" + email + "\",\"password\":\"" + password + "\",\"returnSecureToken\":true}";
		signMask.SetActive(true);
		RestClient.Post<SignResponse>("https://www.googleapis.com/identitytoolkit/v3/relyingparty/signupNewUser?key=" + AuthKey, userData).Then(
			response =>
			{
				user = new User(username,response.idToken,response.localId);

				closeSignMenu();
				RestClient.Put("https://shovel-database.firebaseio.com/Users/" + user.localId + ".json?auth=" + user.idToken, user);

				RestClient.Get<UsersIdResponse>("https://shovel-database.firebaseio.com/UsersId" + ".json?auth=" + user.idToken).Then(stringResp =>
					{
						stringResp.id.Add(user.localId);
						RestClient.Put("https://shovel-database.firebaseio.com/UsersId" + ".json?auth=" + user.idToken,stringResp);

					});
			}).Catch(error =>
				{
					StartCoroutine(showError(signError,signMask));
				});
	}
	void addToArray<T>(ref T[] array,T element){
		T[] newArray = new T[array.Length + 1];
		for (int i = 0; i < array.Length; i++) {
			newArray [i] = array [i];
		}
		newArray [array.Length] = element;
		array = newArray;
	}
	private void SignInUser(string email, string password)
	{
		string userData = "{\"email\":\"" + email + "\",\"password\":\"" + password + "\",\"returnSecureToken\":true}";
		signMask.SetActive(true);
		RestClient.Post<SignResponse>("https://www.googleapis.com/identitytoolkit/v3/relyingparty/verifyPassword?key=" + AuthKey, userData).Then(
			response =>
			{
				GetUserData(response);
				if(rememberToggle.isOn){
					PlayerPrefs.SetString("EMAIL",email);
					PlayerPrefs.SetString("PASSWORD",password);
				}else{
					PlayerPrefs.DeleteKey("EMAIL");
					PlayerPrefs.DeleteKey("PASSWORD");
				}

			}).Catch(error =>
				{
					StartCoroutine(showError(signError,signMask));
				});
	
	}
	IEnumerator showError(GameObject errType,GameObject mask){
		errType.SetActive (true);
		mask.SetActive (false);
		yield return new WaitForSeconds (4);
		errType.SetActive (false);
		yield break;
	}

	private void GetUserData(SignResponse resp)
	{
		RestClient.Get<User>("https://shovel-database.firebaseio.com/Users/" + resp.localId + ".json?auth=" + resp.idToken).Then(response =>
			{
				user = response;
				UserDataPool.signUpUser = user;
				UserDataPool.resp = resp;
				user.localId = resp.localId;
				user.idToken = resp.idToken;
				closeSignMenu();
			});

	}
	public void UpdateUserData()
	{
		RestClient.Get<User>("https://shovel-database.firebaseio.com/Users/" + user.localId + ".json?auth=" + user.idToken).Then(response =>
			{
				user = response;
			});

	}
	public void UpdateUserData(Action method,Action onErr = null )
			{
				RestClient.Get<User>("https://shovel-database.firebaseio.com/Users/" + user.localId + ".json?auth=" + user.idToken).Then(response =>
					{
				user = response;
				method();
			}).Catch(error =>
				{
					onErr();
				});

			}
	void debugErr(){
		Debug.Log ("ERROR");
	}
	void closeSignMenu(){
		signMenu.SetActive (false);
		greetingMenu.SetActive (true);
		greetingMenu.transform.GetChild(0).GetComponent<Text>().text = "Добро пожаловать, " + user.playerName;
		//Invoke ("closeGreet", 3);
		closeGreet();
	}
	void closeGreet(){
		greetingMenu.SetActive (false);
		CreateBLockAdapters ();
		CreatePackAdapters ();
	}
}
