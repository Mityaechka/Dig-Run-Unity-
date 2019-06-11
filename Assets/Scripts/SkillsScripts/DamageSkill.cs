using UnityEngine;
using System.Collections;

public class DamageSkill : SkillEffect
{
	public override void applyEffect (AlivePlayer alive, GhostPlayer ghost)
	{
		base.applyEffect (alive, ghost);
		alive.myPhaseController.CmdGetDamage (1);
	}
	protected override IEnumerator cancelEffect (AlivePlayer alive, GhostPlayer ghost)
	{
		yield return new WaitForSeconds (alive.currentSkill.duration);
		alive.hasEffect = false;
		yield break;
	}
}

