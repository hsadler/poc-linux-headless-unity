using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallScript : MonoBehaviour
{

    private float maxHeight = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float newHeight = this.transform.position.y;
        if(newHeight > this.maxHeight)
        {
            this.maxHeight = newHeight;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("Ball collided with floor. Max height was: " + this.maxHeight.ToString());
        this.maxHeight = 0;
    }
}
