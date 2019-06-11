using UnityEngine;
using System.Collections;
[System.Serializable]
public class ScoreContainer
{
	[SerializeField]
	int score=0;	
	public int Score {
		get {
			return score;
		}

		set {
			score += value;
			UIController.instance.UpdateScore ();
		}
	}
}

