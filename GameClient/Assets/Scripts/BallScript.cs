using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallScript : MonoBehaviour
{

    public string entityUUID;

    void Start()
    {
        if(GameManagerScript.instance.isCentralClient)
        {
            Invoke("DestroySelf", 20);
        }
    }

    void Update()
    {
        
    }

    // INTERFACE METHODS

    // IMPLEMENTATION METHODS

    private void DestroySelf()
    {
        GameManagerScript.instance.DestroyGameBallById(this.entityUUID);
    }

}
