using UnityEngine;
using System.Collections;

public class SkillEffect:MonoBehaviour
{
	public string skillName, skillDIscription;
	public virtual void applyEffect(AlivePlayer alive,GhostPlayer ghost){
		alive.hasEffect = true;
		IEnumerator corotine = cancelEffect (alive, ghost);
		StartCoroutine (corotine);

	}
	public void setDiscription(){
		IEnumerator routine = SkillDiscriptionImage.instance.show (skillName, skillDIscription);
		StartCoroutine(routine);
	}
	protected virtual IEnumerator cancelEffect(AlivePlayer alive,GhostPlayer ghost){
		yield  break;
	}
}

