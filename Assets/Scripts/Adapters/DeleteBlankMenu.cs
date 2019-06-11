using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class DeleteBlankMenu : MonoBehaviour {
	public Button cancelBtn,okBtn;
	public static DeleteBlankMenu instance;
	public int id;
	void Start(){
		instance = this;
		gameObject.SetActive (false);
	}
	public void cancel(){
		gameObject.SetActive (false);
	}
	public void ok(){
		BuildManager.instance.deleteBlank (id);
		gameObject.SetActive (false);
	}
}
