using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ItemDiscription : MonoBehaviour {

	public static ItemDiscription inctanse;
	public Text name, discrpiption;
	Equipment item;
	public float time;
	float maxTime;
	void Awake(){
		inctanse = this;
		maxTime = time;
		gameObject.SetActive (false);
		
	}
	

	void Update () {
		if (time > 0)
			time -= Time.deltaTime;
		if (time <= 0)
			gameObject.SetActive (false);
	}

	public Equipment Item {
		get {
			return item;
		}
		set {
			item = value;
			time = maxTime;
			name.text = item.name;
			discrpiption.text = item.discription;
			gameObject.SetActive (true);
		}
	}
	public void off(){
		gameObject.SetActive(false);
	}
}
