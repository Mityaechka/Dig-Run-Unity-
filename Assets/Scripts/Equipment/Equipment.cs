using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum EquipmentType
{
	Helmet = 0,
	Boots = 1,
	Pickaxe = 2
}

public class Equipment:MonoBehaviour  {
	public EquipmentType type;
	public int id;

	public GameObject equipObject;

	public string name;
	[Multiline]
	public string discription;
	public virtual void pickUp(AlivePlayer player){

	}
	public virtual void drop(){
		Debug.Log ("DROP");
	}


}
