using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HelpManager : MonoBehaviour {
	public HelpTable[] tables;
	HelpTable lastTable;
	void Start(){
		//PlayerPrefs.DeleteAll ();
		if (PlayerPrefs.GetInt ("HELP") == 10) {
			clickBtn (false);
		} else {
			PlayerPrefs.SetInt ("HELP", 10);
			PlayerPrefs.Save ();
		}

	}
	public void enableTable(int id){
		if (lastTable != null) {
			lastTable.tableBtn.enabled = true;
			lastTable.table.SetActive (false);
		}
		tables[id].tableBtn.enabled = false;
		tables[id].table.SetActive (true);
		lastTable = tables [id];
	}
	public void clickBtn(bool flag){
		gameObject.SetActive (flag);
		if (flag)
			enableTable (0);
	}
	public void exit(){
		Application.Quit();
	}
}
