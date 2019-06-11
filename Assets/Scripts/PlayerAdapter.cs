using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class PlayerAdapter : MonoBehaviour {
	public Text playerName, score;
	public GameObject Prefab;
	public void apply(string name,string score,Color color){
		this.playerName.text = name;
		this.playerName.color = color;
		this.score.text = score;
	}
}
