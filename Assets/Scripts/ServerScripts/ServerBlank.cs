using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class ServerBlank:IComparer<ServerBlank>
{
		public string blankName;
		public int xSize, ySize,idOnServer;
		public List<ServerBlock> fields = new List<ServerBlock>();
	public ServerBlank(string name,int x,int y){
		blankName = name;
		xSize = x;
		ySize = y;
	}
	public int Compare (ServerBlank x, ServerBlank y)
	{
		return x.blankName.CompareTo(y.blankName);
	}
}

