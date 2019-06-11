using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System.Collections.Generic;
using UnityEngine.UI;
public class MyNetDiscovery : NetworkDiscovery
{
	public int a;
	public List<ReceiveData> data = new List<ReceiveData> ();
	public GameObject discoveryAdapterPrefab,container,lobbyPackSelecter;
	public static MyNetDiscovery instance;

	public GameObject findGameObj;
	void Start(){
		instance = this;
		Initialize ();

	}
	public override void OnReceivedBroadcast (string fromAddress, string data)
	{
		
		base.OnReceivedBroadcast (fromAddress, data);
		Debug.Log (fromAddress);
		findGameObj.SetActive (false);
		ReceiveData temp = new ReceiveData (data, fromAddress);
		lobbyPackSelecter.SetActive (true);
		lobbyPackSelecter.GetComponent<Image> ().enabled = false;
		lobbyPackSelecter.GetComponent<LobbyPackSelecter> ().okBtn.SetActive (false);
		lobbyPackSelecter.transform.GetChild (0).gameObject.SetActive (false);
		lobbyPackSelecter.transform.GetChild (1).gameObject.SetActive (false);
		lobbyPackSelecter.GetComponent<LobbyPackSelecter> ().testModeToggle.isOn = false;
		if (!findInData (temp)) {
			temp.Adapter= Instantiate (discoveryAdapterPrefab, container.transform);
			this.data.Add (temp);
		}
	}
	bool findInData(ReceiveData newData){
		foreach (ReceiveData adrees in data) {
			if (adrees.ipAdress == newData.ipAdress)
				return true;
		}
		return false;
	}
	public void StartServer(){
		StartAsServer ();
	}
	public void StartClient(){
		StartAsClient ();
		findGameObj.SetActive (true);
	}
	public void closeClient(){
		StopBroadcast ();
		Initialize ();
		findGameObj.SetActive (false);
	}
}

