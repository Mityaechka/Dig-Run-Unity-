using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class PackAdapter : MainAdapter
{
	public Text packName;
	public Button delBtn,openBtn;
	public int id;
	public override void set (int id)
	{
		this.id = id;
		BlanksPack pack =  BuildManager.instance.user.packs [id];
		packName.text = pack.packName;


		openBtn.onClick.AddListener (delegate {
			BuildManager.instance.openBlankMenu(id);
		});
		delBtn.onClick.AddListener (delegate {
			DeletePackMenu.instance.gameObject.SetActive(true);
			DeletePackMenu.instance.id = id;
		});
	}
}

