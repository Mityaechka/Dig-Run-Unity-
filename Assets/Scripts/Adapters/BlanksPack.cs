using UnityEngine;
using System.Collections;
using System.Collections.Generic;
[System.Serializable]
public class BlanksPack:IComparer<BlanksPack>
{
	public int maxBlanks = 5;
	public string packName;
	public List<ServerBlank> blanks = new List<ServerBlank>();
	public bool isPublish = false;
	public BlanksPack(string name){
		packName = name;
	}

	#region IComparer implementation

	public int Compare (BlanksPack x, BlanksPack y)
	{
		return x.packName.CompareTo (y.packName);
	}

	#endregion
}

