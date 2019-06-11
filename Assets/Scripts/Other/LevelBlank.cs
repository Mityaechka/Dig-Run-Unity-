using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName ="NewBlank",menuName ="Level blanks/Level blank")]

public class LevelBlank : ScriptableObject {
	public List<Block> fields = new List<Block>();
	public List<Enemy> enemies = new List<Enemy>();
	public int xLength,yLength;

	public void addBlock(int x,int y,int id){
		
		fields.Add (new Block(x, y, id));
	}
	public void addEnemy(int x,int y,int id){

		enemies.Add (new Enemy(x, y, id));
	}
	public LevelBlank(){
		xLength = 0;
		yLength = 0;
	}
	public void set(int xL,int yL){
		xLength = xL;
		yLength = yL;
		fields.Clear ();
		//for (int i = 0; i < xLength*yLength; i++) {
				
		//}

}
	public int getIndex(int x,int y){
		int index = 0;
		if (x % xLength == 0)
			index = y * yLength;
		else
			index = y * yLength + x;
		return index;
	}
}