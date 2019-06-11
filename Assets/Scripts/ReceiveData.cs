using UnityEngine;
using System.Collections;
[System.Serializable]
public class ReceiveData
{
	public string name,ipAdress;
	GameObject adapter;
	public ReceiveData(string name,string ipAdress){
		this.ipAdress = ipAdress;
		this.name = name;
	}


	public GameObject Adapter {
		get {
			return adapter;
		}
		set {
			adapter = value;
			adapter.GetComponent<DiscoveryAdapter> ().set (ipAdress,name);
		}
	}
}

