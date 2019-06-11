using UnityEngine;
using System.Collections;

public class InverseSkill : SkillEffect
{
	public override void applyEffect (AlivePlayer alive, GhostPlayer ghost)
	{
		base.applyEffect (alive, ghost);
		alive.InverseKof = -1;
	}
	protected override IEnumerator cancelEffect (AlivePlayer alive, GhostPlayer ghost)
	{
		yield return new WaitForSeconds (alive.currentSkill.duration);
		alive.InverseKof = 1;
		alive.hasEffect = false;
		yield break;
	}
}

