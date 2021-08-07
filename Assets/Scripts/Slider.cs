using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slider : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        this.transform.localPosition = new Vector2(0, -4.97f);
    }

    // Update is called once per frame
    void Update()
    {
        //if (Input.GetKeyDown(KeyCode.A))
        //{
            //transform.position = transform.position + new Vector3(-1, 0, 0);
        //} else if (Input.GetKeyDown(KeyCode.D))
        //{
            //transform.position = transform.position + new Vector3(1, 0, 0);
        //}
    }
}
