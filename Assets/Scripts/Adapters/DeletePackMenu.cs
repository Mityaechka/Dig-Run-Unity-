using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class DeletePackMenu : MonoBehaviour
{
	public Button cancelBtn,okBtn;
	public static DeletePackMenu instance;
	public int id;
	void Start(){
		instance = this;
		gameObject.SetActive (false);
	}
	public void cancel(){
		gameObject.SetActive (false);
	}
	public void ok(){
		BuildManager.instance.deletePack (id);
		gameObject.SetActive (false);
	}
}

