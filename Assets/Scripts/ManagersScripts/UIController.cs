using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
public class UIController : MonoBehaviour {
	public GameObject aliveLayerUi,ghostLayerUI;
	public AlivePlayer alivePlyer;
	public GhostPlayer ghostPlayer;
	public static UIController instance;
	public Button effectBtn;
	public Text scoreText,depthText;
	public GameObject[] heartsImage,idolsImage;
	public SimpleInputNamespace.Joystick joystic;
	void Awake(){
		instance = this;
		if (Application.platform == RuntimePlatform.WindowsPlayer||Application.platform == RuntimePlatform.WindowsEditor) {
			for (int i = 0; i < aliveLayerUi.transform.childCount-2; i++) {
				aliveLayerUi.transform.GetChild (i).gameObject.SetActive (false);
			}
		}
	//	UpdateUIPlayer ();
	}
	public void UpdateUIPlayer(){
		for (int i = 0; i < heartsImage.Length; i++) {
			heartsImage [i].SetActive (false);
		}
		for (int i = 0; i < idolsImage.Length; i++) {
			idolsImage [i].SetActive (false);
		}

		for (int i = 0; i < alivePlyer.health; i++) {
			heartsImage [i].SetActive (true);
		}
		for (int i = 0; i < alivePlyer.idolsCount; i++) {
			idolsImage [i].SetActive (true);
		}
	}
	void Update(){
		if(alivePlyer!=null)
			depthText.text = "Depth: " + Mathf.Round(alivePlyer.transform.position.y-4).ToString();
	}
	public void  UpdateScore(){
		scoreText.text = "Score: " + alivePlyer.myPhaseController.score.Score;
	}
	public void setDirUI(int dir){
		alivePlyer.setDirByUI (dir);
	}

	public void setJumpState(bool state){
		alivePlyer.setJumpState (state);
	}

	public void setDigState(bool state){
		alivePlyer.setDigStateByUI (state);
	}
	public void setKickState(bool state){
		alivePlyer.setKickStateByUI (state);
	}
	public void setVertDirUI(int dir){
		ghostPlayer.setDirVert (dir);
	}
	public void setHorDirUI(int dir){
		ghostPlayer.setDirHor (dir);
	}
	public void setDirByJoystic(){
		ghostPlayer.setDirVert (joystic.Value.y);
		ghostPlayer.setDirHor (joystic.Value.x);
	}
	public void useEffect(){
		ghostPlayer.useEffect ();
	}
}
