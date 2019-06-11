using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class BlankAdapter : MainAdapter {
	public Text nameText;
	public int id;
	public Button delBtn, actionBtn;
	public override void set(int blankID){
		id = blankID;
		nameText.text = BuildManager.instance.user.packs[BuildManager.instance.openPackId].blanks[blankID].blankName;
		actionBtn.onClick.AddListener (delegate {
			BuildManager.instance.saveBLocks();
			BuildManager.instance.destroyGrid();
			BuildManager.instance.createBlank(blankID);
		});
		delBtn.onClick.AddListener (delegate {
			DeleteBlankMenu.instance.id = id;
			BuildManager.instance.deleteBlankMenu.SetActive(true);
		});
	}

}
