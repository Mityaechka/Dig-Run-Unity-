using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class LobbyPackAdapter : MonoBehaviour {
	public int id;
	public Text blankName;

	public Toggle toggle;
	void Awake(){
		toggle.onValueChanged.AddListener (delegate {
			LobbyPackSelecter.instance.OnPackSelect (toggle.isOn);
		});
	}
	public void apply(int blankId){
		id = blankId;
		blankName.text = LobbyPackSelecter.instance.packs [blankId].packName;
		
	}
}
