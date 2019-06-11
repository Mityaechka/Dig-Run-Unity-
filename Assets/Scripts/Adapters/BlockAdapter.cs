using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class BlockAdapter : MainAdapter {
	public int blockID;
	public Text nameText;
	public List<GameObject> avaibleBlocks = new List<GameObject> ();
	public BLockType collectedType;
	public GameObject container,blockBtnPref;
	public override void set(int blockID){
		nameText.text = BlocksContainer.instanse.blocks [blockID].GetComponent<BlockObject> ().blockName;
		GetComponent<Button> ().onClick.AddListener (delegate {
			BuildManager.instance.selectBlock(blockID);
		});
	}
	public void accept(){
		nameText.text = collectedType.ToString();
		for (int i = 0; i < avaibleBlocks.Count; i++) {
			Instantiate (blockBtnPref,container.transform.position,Quaternion.identity,container.transform).GetComponent<BlockBtn>().accept(avaibleBlocks[i].GetComponent<BlockObject>().id,this);
		}
		container.SetActive (false);
		GetComponent<Button> ().onClick.AddListener (delegate {
			bool temp = container.activeSelf;
			foreach (BlockAdapter adapter  in BuildManager.instance.typesAdapters.Values) {
				adapter.container.SetActive(false);
			}
			container.SetActive (!temp);
		});
	}

}
