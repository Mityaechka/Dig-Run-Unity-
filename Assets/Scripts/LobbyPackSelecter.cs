using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Proyecto26;
using UnityEngine.UI;
using UnityEngine.Networking;

public class LobbyPackSelecter : NetworkBehaviour {
	public GameObject blanksContainer,blankAdapterPref;
	public List<GameObject> blankAdapters = new List<GameObject>();
	public UsersIdResponse usersId;
	public List<BlanksPack> packs = new List<BlanksPack>();
	public List<BlanksPack> sortedBlanks = new List<BlanksPack>();
	public List<ServerBlank> blanks = new List<ServerBlank>();
	//[SyncVar]
	public List<int> selectedBlanks;
	public GameObject openBtn;
	int sendRequist,getRequist;
	public GameObject loadingMask,okBtn;
	public static LobbyPackSelecter instance;

	private string[] separatedBlanks ;
	public int minPack = 1;


	public InputField emailField,passwordField;
	public Toggle rememberToggle,testModeToggle;
	public GameObject signMask,signError;
	private string AuthKey = "AIzaSyB1K-946AMEPdf724mccOkvF84cbKCZ4gg";

	int curCount;

	public User user;
	void Awake(){
		instance = this;
	}

	public void checkUser(){
		if (PlayerPrefs.GetString ("mode") == null) {
			testModeToggle.isOn = false;
		} else {
			if (PlayerPrefs.GetString ("mode") == "true")
				testModeToggle.isOn = true;
			else
				testModeToggle.isOn = false;
		}
		if (PlayerPrefs.GetString ("EMAIL") != null) {
			emailField.text = PlayerPrefs.GetString ("EMAIL");
			passwordField.text = PlayerPrefs.GetString ("PASSWORD");
			SignInUser (emailField.text, passwordField.text);
		}

	}
	public void showSignMenu(GameObject go){
		go.SetActive (!go.activeSelf);
	}
	public void SignInUser(){
		SignInUser (emailField.text, passwordField.text);
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
	private void GetUserData(SignResponse resp)
	{
		RestClient.Get<User>("https://shovel-database.firebaseio.com/Users/" + resp.localId + ".json?auth=" + resp.idToken).Then(response =>
			{
				user = response;
				user.localId = resp.localId;
				user.idToken = resp.idToken;
				signMask.SetActive(false);
			});

	}

	IEnumerator showError(GameObject errType,GameObject mask){
		errType.SetActive (true);
		mask.SetActive (false);
		yield return new WaitForSeconds (4);
		errType.SetActive (false);
		yield break;
	}







	public void OnPackSelect(bool flag){
		if (flag)
			curCount++;
		else
			curCount--;
		if (curCount >= minPack)
			okBtn.GetComponent<Button> ().enabled = true;
		else
			okBtn.GetComponent<Button> ().enabled = false;
	}
	void Start(){
		if (!testModeToggle.isOn) {
			UpdateUserData ();
			PlayerPrefs.SetString ("mode", "false");
		} else {
			PlayerPrefs.SetString ("mode", "true");
			sendRequist = 1;
			getPacks (user);
			Prototype.NetworkLobby.LobbyManager.s_Singleton.maxPlayers = 1;
		}
		PlayerPrefs.Save ();
		if(!testModeToggle.isOn)
			loadingMask.SetActive (true);
		okBtn.GetComponent<Button> ().enabled = false;
	}

	public void  UpdateUserData()
	{
		usersId = null;
		sendRequist = 0;
		getRequist = 0;
		packs = new List<BlanksPack>();
		sortedBlanks = new List<BlanksPack>();
		blanks = new List<ServerBlank>();

		RestClient.Get<UsersIdResponse>("https://shovel-database.firebaseio.com/UsersId.json" ).Then(stringResp =>
			{
				usersId = stringResp;
				getUsers();
			}).Catch(err=>UpdateUserData());
	}

	void getUsers(){
		foreach (string userId in usersId.id) {
			sendRequist++;
			RestClient.Get<User>("https://shovel-database.firebaseio.com/Users/" + userId + ".json").Then(response =>
				{
					getPacks(response);
				}).Catch(err=>Debug.Log(err));
		}
	}
	void getPacks(User user){
		getRequist++;
		foreach (BlanksPack pack in user.packs) {
			if(pack.isPublish||testModeToggle.isOn)
			packs.Add (pack);
		}
		if (getRequist == sendRequist) {
			sort (ref packs);
			createBlanksAdapters ();
			loadingMask.SetActive (false);
		}

	}
	void createBlanksAdapters(){
		foreach (var go in blankAdapters) {
			Destroy (go);
		}
		blankAdapters = new List<GameObject>();
		for (int i = 0; i < packs.Count; i++) {
			if (packs [i].isPublish||testModeToggle.isOn) {
				GameObject go = Instantiate (blankAdapterPref, blanksContainer.transform);
				blankAdapters.Add(go);
				go.GetComponent<LobbyPackAdapter> ().apply (i);
			}
		}
	}

	public void blanksAccept(){
		openBtn.SetActive (true);
		int i = 0;
		selectedBlanks = new List<int>();
		sortedBlanks = new List<BlanksPack> ();
		foreach (var packAdapter in blankAdapters) {
			if (packAdapter.GetComponent<LobbyPackAdapter> ().toggle.isOn) {
				sortedBlanks.Add (packs [packAdapter.GetComponent<LobbyPackAdapter> ().id]);
				selectedBlanks.Add (i);
			}
			i++;
		}
		getBlanks ();
		gameObject.SetActive (false);
	}
	public void Open(){
		gameObject.SetActive (true);
	}
	public void getBlanks(){
		
		blanks = new List<ServerBlank> ();
		foreach (var pack in sortedBlanks) {
			foreach (var blank in pack.blanks) {
				blanks.Add (blank);	
			}
		}
		BlanksManager.instance.serverBlanks.serverBlanks = blanks;
		separatedBlanks = new string[blanks.Count];
		for (int i = 0; i < blanks.Count; i++) {
			separatedBlanks[i] =  JsonUtility.ToJson (blanks[i]);
		}
		BlanksManager.instance.StringBlank= separatedBlanks;
		BlanksManager.instance.createStart ();
	}
	public void sort(ref List<BlanksPack> packsForSort){
		BlanksPack temp;
		for (int i = 0; i < packsForSort.Count-1; i++)
		{
			for (int j = i + 1; j < packsForSort.Count; j++)
			{
				if (packsForSort[i].packName.CompareTo(packsForSort[j].packName) >0)
				{
					temp = packsForSort[i];
					packsForSort[i] = packsForSort[j];
					packsForSort[j] = temp;
				}
			}

	}
	}
	public void getBlanksFromIndex(List<int> blankIds){
		sortedBlanks = new List<BlanksPack> ();
		for (int i = 0; i < blankIds.Count; i++) {
			sortedBlanks.Add (packs [blankIds[i]]);
		}

	}

}
