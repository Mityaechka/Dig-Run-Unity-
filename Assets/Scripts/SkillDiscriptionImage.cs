using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
public class SkillDiscriptionImage : MonoBehaviour
{

	public Text nameText, discriptionText;
	float showTime = 6;
	public static SkillDiscriptionImage instance;
	Image label;

	void Awake(){
		instance = this;
		label = GetComponent<Image> ();
		setAlpha (0);

	}
	public IEnumerator show(string name,string discription){
		float newAlpha = 1;
		setAlpha (1);
		nameText.text = name;
		discriptionText.text = discription;
		yield return new WaitForEndOfFrame ();
		while (newAlpha >= 0) {
			setAlpha (newAlpha);
			newAlpha -= Time.fixedDeltaTime*0.25f;
			yield return new WaitForSeconds (Time.fixedDeltaTime);
		}

		yield break;
	}
	void setFabeAlpha(float alpha){
		label.CrossFadeAlpha(alpha, showTime, false);
		nameText.CrossFadeAlpha (alpha, showTime, false);
		discriptionText.CrossFadeAlpha (alpha, showTime, false);
	}
	void setAlpha(float alpha){
		label.color = new Color(1,1,1,alpha);
		nameText.color = new Color(0,0,0,alpha);
		discriptionText.color = new Color(0,0,0,alpha);
	}
}

