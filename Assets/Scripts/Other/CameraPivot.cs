using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraPivot : MonoBehaviour
{
    public Transform player;
    public float centrerX, centerZ, leftBorder, rightBorder;
    public static CameraPivot instance;
    public UnityStandardAssets.Cameras.FreeLookCam _camera;
    Vector3 startPos = new Vector3(7, 5, 0);
    bool inFocus = false;
    void Awake()
    {
        instance = this;
        Invoke("enableCamera", 0.1f);
    }
    void enableCamera()
    {
        _camera.enabled = true;
    }
    void Update()
    {
        /*if (player == null) {
			if (MyNetManager.instance.myPLayer.isAlive) {
				player = MyNetManager.instance.myPLayer.aliveObject.transform;
					} else 	player = MyNetManager.instance.myPLayer.ghost.transform;
		}*/
        if (player != null)
        {
            Vector3 newPos = new Vector3();
            if (player.position.x > leftBorder && player.position.x < rightBorder)
            {
                newPos = new Vector3(player.position.x, player.position.y, centerZ);
                if (!inFocus)
                    inFocus = true;
            }
            else
            {
                newPos = new Vector3(transform.position.x, player.position.y, centerZ);
            }
            if (!inFocus)
                newPos = new Vector3(7.5f, player.position.y, centerZ);

            transform.position = newPos;
        }
    }
}
