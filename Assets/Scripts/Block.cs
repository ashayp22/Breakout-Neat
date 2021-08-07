using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour
{
    private bool hit = false;
    public float pointValue;

    void OnCollisionEnter2D(Collision2D collision)
    {
       if(collision.collider.tag == "Ball")
        {
            this.gameObject.SetActive(false);
            hit = true;
        }
    }

    public void Reset()
    {
        hit = false;
    }

    public bool isHit()
    {
        return hit;
    }
}
