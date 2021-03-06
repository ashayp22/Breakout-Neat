using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Line : MonoBehaviour
{
    private double weight; //weight

    public void Initialize(double w, Vector2 startPos, Vector2 endPos) //initialize
    {
        weight = w;

        if(weight < 0)
        {
            //blue for bias
            this.GetComponent<SpriteRenderer>().color = new Color32(0, 0, 255, 255);
        } else
        {
            //red for regular
            this.GetComponent<SpriteRenderer>().color = new Color32(255, 0, 0, 255);
        }

        //modifying the line(uses math)
        float distance = Mathf.Sqrt(Mathf.Pow(startPos.x - endPos.x, 2) + Mathf.Pow(startPos.y - endPos.y, 2)); //gets the distance between two points


        //setting rotation

        float multiplier = 1; //used for the rotation, where it is either rotated clockwise or counterclockwise(positive or negative)
        float xCoor = endPos.x;
        float yCoor = startPos.y;

        if (endPos.y < startPos.y)
        {
            multiplier = -1;
        }


        float side = Mathf.Sqrt(Mathf.Pow(startPos.x - xCoor, 2) + Mathf.Pow(startPos.y - yCoor, 2)); //gets the length of the side

        float rotation = multiplier * Mathf.Acos(side / distance) * (180 / Mathf.PI); //uses trig to get the rotation

        this.transform.rotation = Quaternion.Euler(0, 0, rotation); //rotates

        //setting size


        this.transform.localScale = new Vector3(distance / 20, (float)weight / 10, 1); //scales the line

        //setting position
        transform.localPosition = new Vector2((startPos.x + endPos.x) / 2, (startPos.y + endPos.y) / 2); //sets the position by making it in the middle of the two points

    }
}
