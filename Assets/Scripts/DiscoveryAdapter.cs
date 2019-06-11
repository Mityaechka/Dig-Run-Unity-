using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class DiscoveryAdapter : MonoBehaviour {
	public int id;
	public Text adressText;
	public void set(string ipAdress,string name){
		adressText.text = ipAdress;
		Prototype.NetworkLobby.LobbyMainMenu.instance.OnClickJoin (ipAdress);
	}
}
