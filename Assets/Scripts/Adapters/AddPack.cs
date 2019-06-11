using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class AddPack : MonoBehaviour {
	public InputField nameField;
	public Button cancelBtn,addBtn;
	public static AddPack instance;

	void Start(){
		instance = this;

	}
	public void cancel(){
		gameObject.SetActive (false);
	}
	public void add(){
		if (nameField.text != "") {
			BuildManager.instance.addNewPack (nameField.text);
			addBtn.interactable = false;
		}
	}
}
