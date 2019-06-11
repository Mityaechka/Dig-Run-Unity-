using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour
{
	Dictionary<string,AudioSource> sounds;
	public AudioSource[] soundSource, musicSource;
	public GameObject musicCross, soundCross;
	public static AudioManager instance;
	bool music,sound;
	public LobbyPackSelecter selecter;
	void Awake ()
	{
		try{
		instance = this;
		if (PlayerPrefs.GetString ("music") != null) {
			if (PlayerPrefs.GetString ("music") == "True")
				music = false;
			else
				music = true;
			clickMusic ();
		} else {
			music = false;
			clickMusic ();
		}
		if (PlayerPrefs.GetString ("sound") != null) {
			if (PlayerPrefs.GetString ("sound") == "True")
				sound = false;
			else
				sound = true;
			clickSound ();
		}else {
			sound = false;
			clickSound ();
		}
		sounds = new Dictionary<string, AudioSource>();
		sounds.Add ("MainTheme",GetComponent<AudioSource> ());
		sounds.Add ("Jump", transform.GetChild (0).GetComponent<AudioSource> ());
		sounds.Add ("Walk", transform.GetChild (1).GetComponent<AudioSource> ());
		sounds.Add ("Dig", transform.GetChild (2).GetComponent<AudioSource> ());
		sounds.Add ("Kick", transform.GetChild (3).GetComponent<AudioSource> ());
		sounds.Add ("Idol", transform.GetChild (4).GetComponent<AudioSource> ());
		sounds.Add ("Damage", transform.GetChild (5).GetComponent<AudioSource> ());
		sounds.Add ("Death", transform.GetChild (6).GetComponent<AudioSource> ());
		sounds.Add ("Drill", transform.GetChild (7).GetComponent<AudioSource> ());

			selecter.checkUser ();
		} catch{
		
		}
	}
	public void playSound(string key,int playerID){
		if(MyNetManager.instance.myPLayer.id==playerID)
		sounds [key].Play ();
	}
	public void playSound(string key,bool flag,int playerID){

		if (MyNetManager.instance.myPLayer.id ==playerID) {
			if (flag)
			if (!sounds [key].isPlaying)
				sounds [key].Play ();
			else if (sounds [key].isPlaying)
				sounds [key].Stop ();
		}
	}
	public void clickMusic(){
		try{
		music = (!music);
		musicCross.SetActive (!music);
		Debug.Log (music.ToString ());
		PlayerPrefs.SetString ("music", music.ToString ());
		PlayerPrefs.Save ();
		for (int i = 0; i < musicSource.Length; i++) {
			musicSource [i].mute = !music;
		}
		}
		catch{
			Debug.Log ("ОШИБКА");
		}
	}
	public void clickSound(){
		try{
		sound = (!sound);
		soundCross.SetActive (!sound);
		PlayerPrefs.SetString ("sound", sound.ToString ());
		PlayerPrefs.Save ();
		for (int i = 0; i < soundSource.Length; i++) {
			soundSource [i].mute = !sound;
		}
		}
		catch{
			Debug.Log ("ОШИБКА");
		}
	}
}

