using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{

    public Rigidbody2D rb;
    public static float speed = 10f;
    private float currentSpeed = 10f;
    private int blockHit = 0;
    private int sliderHit = 0;
    private float alive = 0;
    private bool isDead = false;
    private float topBonus = 0;

    private float score = 0;

    private Block target; //ref to the block it is targeting

    private Vector2 lastVec;

    // Start is called before the first frame update
    void Start()
    {
        this.transform.localPosition = new Vector2(0, -4f);
        Respawn(Random.Range(1f, 179f));
        lastVec = GetComponent<Rigidbody2D>().velocity;
    }

    float minV = 1f;

    void Update()
    {
        if(!isDead)
        {
            alive += 1f;
            Vector2 vec = GetComponent<Rigidbody2D>().velocity;
            if (Mathf.Abs(vec.x) <= minV)
            {
                Debug.Log(vec.x);
                GetComponent<Rigidbody2D>().velocity = new Vector2(lastVec.x * -1, vec.y);
            }
            else if (Mathf.Abs(vec.y) <= minV)
            {
                Debug.Log(vec.y);
                GetComponent<Rigidbody2D>().velocity = new Vector2(vec.x, lastVec.y * -1);

            }

            if (Mathf.Abs(GetComponent<Rigidbody2D>().velocity.x) > minV && Mathf.Abs(GetComponent<Rigidbody2D>().velocity.y) > minV)
            {
                lastVec = GetComponent<Rigidbody2D>().velocity;
            }
        }
    }

    public void SetTarget(Block b)
    {
        target = b;
    }

    public void Respawn(float angle)
    {
        isDead = false;
        blockHit = 0;
        sliderHit = 0;
        alive = 0;
        score = 0;
        topBonus = 0;
        currentSpeed = 10f;
        Vector2 initialVelocity;
        
        //random angle 30-80 degrees, or 100-170 degrees
        //float angle;

        do
        {
            angle = Random.Range(1f, 179f);
        } while (angle < 30 || (angle > 70 && angle < 110) || (angle > 160));

        //convert to radians
        angle *= Mathf.PI / 180;

        initialVelocity = new Vector2(speed * Mathf.Cos(angle), speed * Mathf.Sin(angle));

        GetComponent<Rigidbody2D>().velocity = initialVelocity;
    }

    public void SetCurrentSpeed(float s)
    {
        currentSpeed = s;
    }

    public float GetCurrentSpeed()
    {
        return currentSpeed;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.collider.tag == "Bottom")
        {
            SetDead();
        }
        else if (collision.collider.tag == "Block")
        {
            blockHit += 1;
            Block b = (Block)collision.collider.GetComponent<Block>();
            score += b.pointValue;
        } else if(collision.collider.tag == "Slider")
        {
            sliderHit += 1;
        } else if(collision.collider.tag == "Top")
        {
            topBonus += (16000 - alive) / 100;
        }
    }

    private float distance(Vector2 a, Vector2 b)
    {
        return Mathf.Sqrt(Mathf.Pow(a.x - b.x, 2) + Mathf.Pow(a.y - b.y, 2));
    }

    public void SetDead()
    {
        isDead = true;
        GetComponent<Rigidbody2D>().velocity = Vector2.zero;
    }

    public bool IsDead()
    {
        return isDead;
    }

    public int GetTime()
    {
        return (int)alive;
    }

    public float GetFitness()
    {
        float p1 = (blockHit / alive) * 40;
        float p2 = sliderHit * 100;
        float p3 = score * 10;

        float p4 = ((score * 10 + sliderHit * 100) / (alive / 4)) * 100;

        //Debug.Log(p1 + " " + p2 + " " + p3);
        float fitness = ((blockHit + score) * 1000 / alive) + topBonus;
        Debug.Log(((blockHit + score) * 1000 / alive) + " " + topBonus);

        return fitness;
    }

    public float getScore()
    {
        return score;
    }
}
