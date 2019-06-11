using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class AddBlankMenu : MonoBehaviour
{
	public InputField nameField,sizeField;
	public Button cancelBtn,addBtn;
	public static AddBlankMenu instance;

	void Start(){
		instance = this;
	}
	public void cancel(){
		gameObject.SetActive (false);
	}
	public void add(){
		if (nameField.text != "") {
			int y; int.TryParse (sizeField.text, out y);
			if (y < 40) {
				BuildManager.instance.addNewBlank (nameField.text,y);
				gameObject.SetActive (false);
			}
		}
	}
}

