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
        if (Constants.IS_CENTRAL_CLIENT)
        {
            this.gms.serverConn.SendClientMessageToServer(this.transform.position.ToString());   
        }   
    }

}
