using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KickStateSender : MonoBehaviour {
	public AlivePlayer player;
	public void setStage(int stage){
		player.setKickStage (stage);
	}
	public void playSound(string key){
		AudioManager.instance.playSound (key,player.id);
	}
}
