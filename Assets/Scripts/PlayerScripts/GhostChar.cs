using UnityEngine;
using System.Collections;


public enum Type
{
	Inverse = 0,
	BlackDisplay = 1,
	DamagePlayer = 2,
	TradeBody = 3
}
[System.Serializable]
public class GhostChar
{
	[SerializeField]
	float mainSpeed;
	[SerializeField]
	Type ghostType;

	public GhostPlayer ghost;

	public SkillData currentSkill;
	public GhostChar(){}

	public float MainSpeed {
		get {
			return mainSpeed*currentSkill.speedKof;
		}
	}
	public void setType(int t){
		ghostType = (Type)t;
		switch (ghostType) {
		case Type.BlackDisplay:
			currentSkill =SkillsContainer.instance.blackSkill;
			break;
		case Type.Inverse:
			currentSkill = SkillsContainer.instance.inverseSkill;
			break;
		case Type.DamagePlayer:
			currentSkill = SkillsContainer.instance.damageSkill;
			break;
		case Type.TradeBody:
			currentSkill = SkillsContainer.instance.tradeSkill;
			break;
		}
		ghost.spriteRender.sprite = currentSkill.skillSkin;	
		if(ghost.myPhaseController.hasAuthority)
		currentSkill.effect.setDiscription ();
	}
	public int getTypeInt(){
		return (int)ghostType;
	}

	public float MinDistanse {
		get {
			return currentSkill.minDistance;
		}
	}

	public Type GhostType {
		get {
			return ghostType;
		}
		set {
			ghostType = value;
		}
	}
}

