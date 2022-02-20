using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("PrintLog", 0f, 1f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void PrintLog()
    {
        Debug.Log("I'm the unity client!");
    }

}
