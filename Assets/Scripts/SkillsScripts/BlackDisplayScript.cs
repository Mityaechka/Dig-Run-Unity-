using UnityEngine;
using System.Collections;

public class BlackDisplayScript : SkillEffect
{
	public override void applyEffect (AlivePlayer alive, GhostPlayer ghost)
	{
		base.applyEffect (alive, ghost);
		alive.blackholeObject.color = new Color (0, 0, 0, 1);
	}
	protected override IEnumerator cancelEffect (AlivePlayer alive, GhostPlayer ghost)
	{
		yield return new WaitForSeconds (alive.currentSkill.duration);
		alive.hasEffect = false;
		alive.blackholeObject.color = new Color (0, 0, 0,0.431f);
		yield break;
	}
}

