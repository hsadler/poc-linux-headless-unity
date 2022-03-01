using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallScript : MonoBehaviour
{

    [SerializeField]
    private GameManagerScript gms;

    void Start()
    {
       
    }

    void Update()
    {
        if (gms.isCentralClient)
        {
            this.gms.serverConn.SendClientMessageToServer(this.transform.position.ToString());
        }
    }

}
