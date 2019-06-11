using UnityEngine;
using System.Collections;

public class SkillsContainer : MonoBehaviour
{
	public SkillData blackSkill, inverseSkill, damageSkill, tradeSkill;

	public static SkillsContainer instance;


	void Awake(){
		instance = this;
	}


/*

	void invokeBlacked(){
		isDisplayBlack = false;
		Debug.Log ("Not black");
	}
	void invokeInversed(){
		isInversed = false;
		inverseKof = 1;
		Debug.Log ("Not inverse");
	}
	void invokeDamaged(){
		isDamaged = false;
		Debug.Log ("Not damaged");
	}
	*/
}

