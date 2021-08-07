using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{


    //NEAT settings

    //NEAT SETTINGS
    public static int YOUNG_BONUS_AGE_THRESHOLD = 10;
    public static double YOUNG_FITNESS_BONUS = 1.2;
    public static int OLD_AGE_THRESHOLD = 50;
    public static double OLD_AGE_PENALTY = 0.7;
    public static int NUM_GENS_ALLOWED_NO_IMPROVEMENT = 3;
    public static int NUM_AI = 30;
    public static double CROSSOVER_RATE = 0.7;
    public static int MAX_PERMITTED_NEURONS = 12;
    public static double CHANCE_ADD_NODE = 0.05;
    public static int NUM_TRYS_TO_FIND_OLD_LINK = 10;
    public static double CHANCE_ADD_LINK = 0.3;
    public static double CHANCE_ADD_RECURRENT_LINK = 0.05;
    public static int NUM_TRYS_TO_FIND_LOOPED_LINK = 0;
    public static int NUM_ADD_LINK_ATTEMPTS = 10;
    public static double MUTATION_RATE = 0.2;
    public static double PROBABILITY_WEIGHT_REPLACED = 0.1;
    public static double MAX_WEIGHT_PERTUBATION = 0.5;
    public static double ACTIVATION_MUTATION_RATE = 0.1;
    public static double MAX_ACTIVATION_PERTUBATION = 0.5;
    public static int NUM_INPUTS = 4;
    public static int NUM_OUTPUTS = 1;

    //prefabs and instances
    public Game gamePrefab;

    private List<Game> gameList;

    public Node nodePrefab;
    public Line linePrefab;

    public GameObject NNCenter;

    public Text scoreText;
    public Text genText;
    public Text bestScoreText;
    public Text bestGenText;
    public Text gamesLeft;
    public Text timeText;
    public Text speedText;
    public Text bestTimetext;

    private float bestScore = 0;
    private int bestGen = 0;
    private int fastestTime = 0;

    private float currentSpeed = 1;

    private int time;

    //Neat stuff

    private Ga NEAT;


    // Start is called before the first frame update
    void Start()
    {
        time = 0;
        NEAT = new Ga(NUM_AI, NUM_INPUTS, NUM_OUTPUTS); //create neat

        //create each invidivudal game
        gameList = new List<Game>();

        for(int i = 0; i < NUM_AI; i++)
        {
            Game game = Instantiate(gamePrefab) as Game;
            float x = ((i) + 1) * 32 - 55;
            float y = 0;
            game.SetPosition(new Vector2(x,y));
            gameList.Add(game);
        }

        //set text
        scoreText.text = "SCORE: 0";
        genText.text = "GENERATION: 1";
        bestScoreText.text = "BEST SCORE: 0";
        bestGenText.text = "BEST GENERATION: 1";
        gamesLeft.text = "GAMES LEFT: " + NUM_AI;
        timeText.text = "TIME: 0";
        bestTimetext.text = "BEST TIME: 0";
    }

    // Update is called once per frame
    void Update()
    {
        time++;
        if(AllDone())
        {
            Epoch();
           
        }
        else
        {

            //choose selected slider - the game with the highest score


            bool change = false;
            float highest = 0;
            int alive = 0;
            for (int z = 0; z < gameList.Count; z++)
            {
                if (!gameList[z].IsDead() && !gameList[z].AllDone()) //game still going
                {
                    alive += 1; //one more alive

                    if(highest <= gameList[z].GetScore()) //higher score
                    {
                        if (selectedSlider != z) //not the current game displayed, must change the display
                        {
                            change = true;
                        }
                        selectedSlider = z;
                        highest = gameList[z].GetScore();
                    }
                }
            }

            if(change)
            {
                for(int z = 0; z < gameList.Count; z++)
                {
                    //reset position to original

                    float x = (z + 1) * 32 - 55;
                    float y = 0;
                    gameList[z].SetPosition(new Vector2(x, y));

                    if(!gameList[z].IsDead()) //scale the velocity, so the balls hidden don't move too fast
                    {
                        gameList[z].ScaleVelocity(Ball.speed);
                    }

                    //now either enlargen or move
                    if (z == selectedSlider)
                    {
                        //move to the middle
                        gameList[z].SetPosition(new Vector2(0, 6.5f));
                        gameList[z].gameObject.transform.localScale = new Vector2(4.5f, 4.5f);

                        //increase the velocity

                        if(!gameList[z].IsDead())
                        {
                            gameList[z].ScaleVelocity(Ball.speed * 4.5f);
                        }

                    }
                    else
                    {
                        //move out of view
                        gameList[z].gameObject.transform.position += new Vector3(0, 100);
                        gameList[z].gameObject.transform.localScale = new Vector2(1, 1);

                    }
                }
            }

            //update each slider
            int c = 0;
            foreach(Game g in gameList)
            {
                if(g.IsDead())
                {
                    c++;
                    continue;
                }

                //get inputs
                List<double> inputs = g.getInputs();
                //forward propogate
                List<double> output = NEAT.UpdateMember(c, inputs);
                //Debug.Log((output[0] - 0.5)/10);
                //move slider
                g.MoveSlider((float)((output[0] - 0.5) / 1));
                c++;
            }
            //display a game still going


            DrawNN(); //draw the neural network

            //update text on screen
            scoreText.text = "SCORE: " + highest;
            gamesLeft.text = "GAMES LEFT: " + alive;
            timeText.text = "TIME: " + (time / 4);
        }


        if(Input.GetKeyDown(KeyCode.G)) //skip gen
        {
            Epoch();
        } else if(Input.GetKeyDown(KeyCode.S)) //speed up
        {
            ////change the var
            //currentSpeed += 1;
            //Ball.speed += 1f;
            //if (currentSpeed == 11)
            //{
            //    currentSpeed = 1;
            //    Ball.speed = 10;
            //}

            ////now speed up each ball

            //for(var c = 0; c < gameList.Count; c++)
            //{
            //    if(!gameList[c].IsDead())
            //    {
            //        if(c == selectedSlider)
            //        {
            //            gameList[c].ScaleVelocity(Ball.speed * 4.5f);
            //        } else
            //        {
            //            gameList[c].ScaleVelocity(Ball.speed);
            //        }
            //    }
            //}

            //speedText.text = "SPEED: " + currentSpeed + "x";
 

        } else if(Input.GetKeyDown(KeyCode.R)) //restart
        {
            //reset vars
            time = 0;
            NEAT = new Ga(NUM_AI, NUM_INPUTS, NUM_OUTPUTS); //create neat

            //reset text
            scoreText.text = "SCORE: 0";
            genText.text = "GENERATION: " + NEAT.generation;
            bestScoreText.text = "BEST SCORE: " + bestScore;
            bestGenText.text = "BEST GENERATION: " + bestGen;
            gamesLeft.text = "GAMES LEFT: " + NUM_AI;
            timeText.text = "TIME: 0";
            speedText.text = "SPEED: 1x";
            bestTimetext.text = "BEST TIME: 0";

            bestScore = 0;
            bestGen = 0;
            fastestTime = 0;
            Ball.speed = 10;

            currentSpeed = 1;
            //reset each game

            //reset all the games
            int counter = 0;
            foreach (Game g in gameList)
            {
                g.Restart(NEAT.getGenome(counter).starting[0], NEAT.getGenome(counter).starting[1]);
                counter++;
            }
        }

    }

    private void Epoch()
    {
        //first, figure out the highest score and get the fitnesses

        float highest = 0;
        int smallest = 0;
        List<double> fitnessList = new List<double>();

        foreach (Game g in gameList)
        {
            fitnessList.Add(g.GetFitness());

            if (g.GetScore() >= highest) //found a higher score
            {

                if(highest == 195) //highest already recorded
                {
                    if(g.GetTime() < smallest) //makes sure this games time is less than the one saved
                    {
                        highest = g.GetScore();
                        smallest = g.GetTime();
                    }
                } else
                {
                    highest = g.GetScore();
                    smallest = g.GetTime();
                }
            }

        }

        NEAT.Epoch(fitnessList);

        //reset all the games
        int counter = 0;
        foreach (Game g in gameList)
        {
            g.Restart(NEAT.getGenome(counter).starting[0], NEAT.getGenome(counter).starting[1]);
            counter++;
        }

        //update count

        if (highest >= bestScore)
        {

            if(highest == 195) //this is 195
            {
                if(bestScore == 195) //already recorded 195, need to check times
                {
                    if(fastestTime > smallest)
                    {
                        bestScore = highest;
                        bestGen = NEAT.generation - 1;
                        fastestTime = smallest;
                    }
                } else
                {
                    //first time game is won, can record
                    bestScore = highest;
                    bestGen = NEAT.generation - 1;
                    fastestTime = smallest;
                }
            } else
            { //haven't beat the game yet
                bestScore = highest;
                bestGen = NEAT.generation - 1;
                fastestTime = 0;
            }
        }

        scoreText.text = "SCORE: 0";
        genText.text = "GENERATION: " + NEAT.generation;
        bestScoreText.text = "BEST SCORE: " + bestScore;
        bestGenText.text = "BEST GENERATION: " + bestGen;
        gamesLeft.text = "GAMES LEFT: " + NUM_AI;
        timeText.text = "TIME: 0";
        bestTimetext.text = "BEST TIME: " + fastestTime;

        time = 0;
    }

    int selectedSlider = -1;
    Vector3 addition = new Vector3(0f, 0f);

    private void DrawNN()
    {
        //choose the slider

        //destroy any children of the gameboject
        foreach (Transform child in NNCenter.transform)
        {
            Destroy(child.gameObject);
        }

        //now we actually create the nn

        for (int i = 0; i < NEAT.getNNNodeSize(selectedSlider); i++) //for each of the nodes
        {
            //construct the node
            Node n = Instantiate(nodePrefab) as Node;

            int id = NEAT.getNNId(selectedSlider, i);

            n.transform.parent = NNCenter.transform;
            n.transform.localPosition = NEAT.getNNNodePosFromID(selectedSlider, id);


            //construct its connections

            List<PLink> connectionList = NEAT.getNNConnections(selectedSlider, i);

            foreach (PLink link in connectionList) //creates the lines
            {

                Line line = Instantiate(linePrefab) as Line;
                line.transform.parent = NNCenter.transform;

                line.Initialize(link.getWeight(), n.transform.localPosition, NEAT.getNNNodePosFromID(selectedSlider, link.getPOut().getNeuronId()));

                line.transform.localPosition += addition;

            }

            n.transform.position += addition;

        }
    }

    private bool AllDone()
    {
        foreach(Game g in gameList)
        {
            if(!g.IsDead() && !g.AllDone())
            {
                return false;
            }
        }
        return true;
    }
}


//problem:
