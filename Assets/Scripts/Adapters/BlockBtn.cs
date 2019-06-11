using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class BlockBtn : MonoBehaviour {
	public int blockID;
	public void accept(int id,BlockAdapter par){
		blockID = id;
		GetComponent<Image> ().sprite = BlocksContainer.instanse.blocks [id].GetComponent<BlockObject> ().sprite;
		GetComponent<Button> ().onClick.AddListener (delegate {
			BuildManager.instance.currentId = id;
			//par.container.SetActive(false);
		});
	}
}
