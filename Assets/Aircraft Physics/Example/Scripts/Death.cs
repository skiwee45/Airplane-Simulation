using System.Collections;
using System.Collections.Generic;
using Aircraft_Physics.Example.Scripts;
using UnityEngine;
using UnityEngine.UI;

public class Death : MonoBehaviour
{
    private GameObject airplane;
    // Start is called before the first frame update
    void Start()
    {
        airplane = gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.y <= -1f)
        {
            Destroy(airplane);
        }
        
    }
}
