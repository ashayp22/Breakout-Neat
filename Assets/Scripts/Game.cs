using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{

    public Ball ballPrefab;
    public Slider sliderPrefab;
    public Block blockPrefab;

    private Ball ball;
    private Slider slider;
    private List<Block> blockList;

    public GameObject skull;
    public GameObject crown;

    private bool setDead = false;
    private bool setDone = false;

    private Color32[] colorList = { new Color32(255, 0, 0, 255), new Color32(255, 165, 0, 255), new Color32(255, 255, 0, 255)
           ,new Color32(0, 255, 0, 255), new Color32(0, 0, 255, 255) };

    // Start is called before the first frame update
    void Start()
    {

        //create the game
        ball = Instantiate(ballPrefab) as Ball;
        slider = Instantiate(sliderPrefab) as Slider;

        ball.transform.parent = this.transform;
        slider.transform.parent = this.transform;

        blockList = new List<Block>();

        for(int i = 0; i < 5; i++)
        {

            for(int j = 0; j < 13; j++)
            {
                Block block = Instantiate(blockPrefab) as Block;
                block.pointValue = i + 1;
                block.transform.parent = this.transform;
                block.transform.localPosition = new Vector2((j-2) * 1.65f - 6.6f, (i * .9f) + 2.5f);
                block.name = "block " + i + ' ' + j;
                block.GetComponent<SpriteRenderer>().color = colorList[i];
                blockList.Add(block);
            }
        }

    }

    public void SetPosition(Vector2 pos)
    {
        transform.position = pos;
    }

    public void MoveSlider(float change)
    {

        if(change > 0 && slider.transform.localPosition.x < 9.4)
        {
            slider.transform.localPosition = new Vector2(slider.transform.localPosition.x + change, slider.transform.localPosition.y);
        } else if(change < 0 && slider.transform.localPosition.x > -9.4)
        {
            slider.transform.localPosition = new Vector2(slider.transform.localPosition.x + change, slider.transform.localPosition.y);
        }
    }

    public float GetFitness()
    {
        return ball.GetFitness();
    }

    public void Update()
    {
        if(IsDead() && !setDead)
        {
            skull.SetActive(true);
            setDead = true;
        }

        if(AllDone() && !setDone)
        {
            ball.SetDead();
            setDone = true;
            crown.SetActive(true);
        }
    }

    public bool IsDead() //game over
    {
        return ball.IsDead();
    }

    public bool AllDone()
    {
        foreach(Block b in blockList)
        {
            if(!b.isHit())
            {
                return false;
            }
        }
        return true;
    }

    public float GetScore()
    {
        return ball.getScore();
    }

    public List<double> getInputs()
    {
        List<double> inputs = new List<double>();

        //ball - slider x pos, ball - slider y pos, ball velocity (x,y)

        inputs.Add((ball.transform.localPosition.x - slider.transform.localPosition.x) / 22);
        inputs.Add((ball.transform.localPosition.y - slider.transform.localPosition.y) / 14);


        //velocity

        //have to normalize, so we will get the angle and then multiply it by 1

        inputs.Add(ball.GetComponent<Rigidbody2D>().velocity.x / ball.GetCurrentSpeed());
        inputs.Add(ball.GetComponent<Rigidbody2D>().velocity.y / ball.GetCurrentSpeed());

        return inputs;
    }

    public void Restart(float ballAngle, float startPos)
    {
        //reset the slider

        slider.transform.localPosition = new Vector2(startPos, -4.97f);

        //reset the ball

        ball.transform.localPosition = new Vector2(startPos, -4f);
        ball.Respawn(ballAngle);

        //reset skull and crown
        skull.SetActive(false);
        setDead = false;
        setDone = false;
        crown.SetActive(false);

        //reset the blocks
        foreach (Block b in blockList)
        {
            b.gameObject.SetActive(true);
            b.Reset();
        }
    }

    public void ScaleVelocity(float scale)
    {
        ball.SetCurrentSpeed(scale); //set the current speed of the ball
        Vector2 currentVec = ball.GetComponent<Rigidbody2D>().velocity;
        
        if(currentVec.x == 0 || currentVec.y == 0)
        {
            return;
        }

        float angle = Mathf.Atan(currentVec.y / currentVec.x); //ranging from -180 to 180, so we need to add 180 based on the quadrant

        //convert to degrees

        angle *= (180 / Mathf.PI);

        //Debug.Log(angle);

        if (currentVec.x > 0 && currentVec.y > 0) //first quadrant
        {
            angle *= (Mathf.PI / 180);
            Vector2 newVec = new Vector2(scale * Mathf.Cos(angle), scale * Mathf.Sin(angle));

            ball.GetComponent<Rigidbody2D>().velocity = newVec;
        } else if (currentVec.x < 0 && currentVec.y > 0) //second quadrant
        {
            angle += 180;
            angle *= (Mathf.PI / 180);
            Vector2 newVec = new Vector2(scale * Mathf.Cos(angle), scale * Mathf.Sin(angle));

            ball.GetComponent<Rigidbody2D>().velocity = newVec;
        } else if (currentVec.x < 0 && currentVec.y < 0) //third quadrant
        {
            angle += 180;
            angle *= (Mathf.PI / 180);

            Vector2 newVec = new Vector2(scale * Mathf.Cos(angle), scale * Mathf.Sin(angle));

            ball.GetComponent<Rigidbody2D>().velocity = newVec;
        } else if (currentVec.x > 0 && currentVec.y < 0) //fourth quadrant
        {
            angle *= (Mathf.PI / 180);

            Vector2 newVec = new Vector2(scale * Mathf.Cos(angle), scale * Mathf.Sin(angle));

            ball.GetComponent<Rigidbody2D>().velocity = newVec;
        }

    }

    public int GetTime()
    {
        return ball.GetTime() / 4;
    }
}
