using UnityEngine;
using System.Collections;
using UnityEngine.UI;
struct PlayerStr
{
	public int score;
	public string name;
	public Color color;
}
public class EndTable : MonoBehaviour
{
	
	public string lastPlayer;
	public PlayerAdapter adapter;
	public GameObject container;
	public Text timer,lastPlayerText,recordText,depthText;
	public GameObject timeTable;
	public Animator anim;
	public static EndTable instance;
	void Awake(){
		instance = this;
		gameObject.SetActive (false);

	}
	public void showTable(){
		PlayerStr [] players = new PlayerStr[Prototype.NetworkLobby.LobbyPlayerList._instance.Players.Count];

		for (int i = 0; i < players.Length; i++) {
			players[i].name = Prototype.NetworkLobby.LobbyPlayerList._instance.Players[i].playerName;
			players[i].color = Prototype.NetworkLobby.LobbyPlayerList._instance.Players[i].playerColor;
			players[i].score = MyNetManager.instance.playersOnServer [i].score.Score;
		}
		lastPlayerText.text = lastPlayer;
		players = sort (players);
		int lastDepth = PlayerPrefs.GetInt ("depth");
		int depth = 0;
		int.TryParse (UIController.instance.depthText.text.Substring(7), out depth);
		Debug.Log (UIController.instance.depthText.text.Substring(7) + ";" + depth);
		if (depth < lastDepth) {
			recordText.text = "НОВЫЙ РЕКОРД!\nМакс. глубина:";
			PlayerPrefs.SetInt ("depth", depth);
		}
		depthText.text = depth.ToString ();
		foreach (var player in players) {
			PlayerAdapter adap = Instantiate (adapter.Prefab, container.transform).GetComponent<PlayerAdapter> ();
			adap.apply (player.name, player.score.ToString(), player.color);
		}
	}
	PlayerStr [] sort(PlayerStr [] mas)
	{
		PlayerStr temp;
		for (int i = 0; i < mas.Length; i++)
		{
			for (int j = i + 1; j < mas.Length; j++)
			{
				if (mas[i].score < mas[j].score)
				{
					temp = mas[i];
					mas[i] = mas[j];
					mas[j] = temp;
				}                   
			}            
		}
		return mas;
	}

}

