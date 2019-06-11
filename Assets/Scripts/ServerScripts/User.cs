using UnityEngine;
using System.Collections;
using System.Collections.Generic;
[System.Serializable]
public class User
{
		public string idToken ;
		public string localId ;
		public string playerName;
		public int maxPackCount = 3;
	public bool isAdmin = false;
		public List<BlanksPack> packs = new List<BlanksPack>();
	public User(User temp){
		idToken = temp.idToken;
		localId = temp.localId;
		playerName = temp.playerName;
		maxPackCount = temp.maxPackCount;
		packs  = temp.packs;
		isAdmin = temp.isAdmin;
	}
	public User(string name,string idToken,string localId){
		playerName = name;
		this.idToken = idToken;
		this.localId = localId;

	}
}

