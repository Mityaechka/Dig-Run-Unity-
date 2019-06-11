using UnityEngine;
using System.Collections;

public class TradeSkill : SkillEffect
{
	public override void applyEffect (AlivePlayer alive, GhostPlayer ghost)
	{
		base.applyEffect (alive, ghost);

		Vector3 lastPos = ghost.transform.position;
		ghost.isSkillAvaible = true;
		alive.hasEffect = false;

		alive.myPhaseController.CmdSaySwitch (alive.transform.position,true,ghost.id);
		alive.myPhaseController.CmdSaySwitch ( lastPos,false,alive.id);
	}
	protected override IEnumerator cancelEffect (AlivePlayer alive, GhostPlayer ghost)
	{
		yield return new WaitForSeconds (alive.currentSkill.duration);
		alive.hasEffect = false;
		yield break;
	}

}

