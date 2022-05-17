using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

public class HiveModel : MonoBehaviour
{
    [Header("Global Properties")]
    public int mapResolution;
    public string HiveMode = "collect";

    [HideInInspector]
    public int hiveFoodTarget;


    [Header("Stats")]
    [ReadOnly]public int currentStep;
    
    [ReadOnly] public int foodInHive;
    [ReadOnly] public int antsSpawned;
    [ReadOnly] public int antNumber;
    [ReadOnly] public int totalStartingFood;
    [ReadOnly] public int foodOnMap;
    [ReadOnly] public int hiveBlocksDestroyed;
    [ReadOnly] public int hiveBlocksTotal;
    [ReadOnly] public float hiveBlocksWinPercentage = 0.5f; // Destroy half of the hive


    [Header("Hive Values")]
    public Vector2Int hivePosition;
    public int hiveRadius = 4;
    public int agentStartNum = 200;
    public int foodPerSpawn;

    [Header("Generation Values")]
    public string noiseFunction = "perlin";
    public float cutoffDensity = 0; // noiseValue is in range -1 to 1
    public float noiseFrequency = 0.01f;
    public float noiseLacunarity = 2f;
    public float noiseGain = 0.5f;
    public int noiseOctaves = 8;
    public int hiveCaveRadius;
    public int foodPerPixel_Min = 2;
    public int foodPerPixel_Max = 20;
    public int foodPoints_Min = 2;
    public int foodPoints_Max = 6;
    public int foodPointRadius_Min = 2;
    public int foodPointRadius_Max = 5;
    public Vector2 playerStartPosition;
    // Maps hold discrete values that represent grid states; 0 - empty; 1 - Agent (not implemented); 2 - Obstacle; 4 - Hive ;
    // Food map is saparate so that each grid point can have multiple instances of food
    public int[,] dataMap;
    public int[,] foodMap;

    // TrailMaps hold weighted markers of trail data
    public float[,] toHomeTrailMap;
    public float[,] toFoodTrailMap;
    public List<Agent> agents = new List<Agent>();

   

    [Header("Agent Values")]
    public float speed = 1;
    public float steerStrenght = 0.5f;
    public float wanderStrenght = 0.1f;
    public float agentRotationAngle = 45f;
    public float toHome_DepositAmount = 1f;
    public float toFood_DepositAMount = 0.1f;
    

    [Header("Sensor Values")]
    public int sensorWidth = 2;
    public int sensorOffsetDistance = 6;
    public float sensorAngle = 45f;  //toLeft: sensorAngle; toRight: -sensorAngle

    [Header("ToHome Gaussian Diffsion Values")]
    public bool diffusion_Home;
    public int diffusionStepDelay_Home;
    public float DiffusionSigma_Home = 1f;
    public int kernelLenght_Home = 3;
    public float DiffusionMean_Home = 0f;

    [Header("ToHome Decay Values")]
    public bool decay_Home;
    public float decayAmount_Home;
    public int decayStepDelay_Home;


    [Header("ToFood Gaussian Diffsion Values")]
    public bool diffusion_Food;
    public int diffusionStepDelay_Food;
    public float DiffusionSigma_Food = 1f;
    public int kernelLenght_Food = 3;
    public float DiffusionMean_Food = 0f;

    [Header("ToFood Decay Values")]
    public bool decay_Food;
    public float decayAmount_Food;
    public int decayStepDelay_Food;



    // Start is called before the first frame update
    void Awake()
    {
        int size = MenuController.MapSize;
        if (size == 0)
            mapResolution = 100;
        else if (size == 1)
            mapResolution = 200;
        else if (size == 2)
            mapResolution = 300;
        else
            mapResolution = 240;
    }



    public float SampleHomeTrailMap(int x, int y)
    {
        if (x < 0 || x >= mapResolution || y < 0 || y >= mapResolution)
        {
            return 0;
        }
        else
        {
            return toHomeTrailMap[x, y];
        }
    }

    public float SampleFoodTrailMap(int x, int y)
    {
        if (x < 0 || x >= mapResolution || y < 0 || y >= mapResolution)
        {
            return 0;
        }
        else
        {
            return toFoodTrailMap[x, y];
        }
    }

    public int SampleFoodMap(int x, int y)
    {
        if (x < 0 || x >= mapResolution || y < 0 || y >= mapResolution)
        {
            return 0;
        }
        else
        {
            return foodMap[x, y];
        }
    }

    public int SampleDataMap(int x, int y)
    {
        if (x < 0 || x >= mapResolution || y < 0 || y >= mapResolution)
        {
            return 0;
        }
        else
        {
            return dataMap[x, y];
        }
    }

    public int TakeFood(int x, int y, int amount) // Returns the amount of food that has been taken
    {
        int value = 0;
        if (amount <= foodMap[x,y])
        {
            foodMap[x, y] -= amount;
            value = amount;
        }
        else // This foodPoint has been depleted
        {
            value = foodMap[x, y];
            foodMap[x, y] = 0;

           

        }

        if (foodMap[x,y] <= 0)
        {
            WorldController worldControl = GameObject.FindGameObjectWithTag("WORLD").GetComponent<WorldController>();
            StartCoroutine(worldControl.GenerateFood(this));
        }

        return value;

    }

    public void AddFood(int x, int y, int amount)
    {

        
        if (amount > 0)// food is put on map and foodObjects need to be regenerated
        {
            foodMap[x, y] += amount;
            WorldController worldControl = GameObject.FindGameObjectWithTag("WORLD").GetComponent<WorldController>();
            StartCoroutine(worldControl.GenerateFood(this));
            
        }
        
    }

    int foodSinceLastSpawn;
    public void ReturnFood(int amount, Vector2Int position)
    {
        
        foodInHive += amount;

        foodSinceLastSpawn += amount;
        if (foodSinceLastSpawn >= foodPerSpawn)
        {
            
            HiveController control = GetComponent<HiveController>();

            // Spawn an agent
            Vector2 velocity = position - hivePosition;
            velocity.Normalize();

            GameObject antAgentObj = Instantiate(control.AntAgentRef);
            Agent newAgent = new Agent(position, velocity, this, true, antAgentObj);


            agents.Add(newAgent);

            /* Spawn an ant
             * 
            
            
            antAgentObj.SetActive(true);
            AntAgent antAgent = antAgentObj.GetComponent<AntAgent>();
            antAgent.InitializeAntAgent(newAgent, this);
            control.AntAgents.Add(antAgentObj);


             * */

            antsSpawned++;
            ///////
            foodSinceLastSpawn = 0;
        }


    }



    public void ModifyHomeTrailMap(int x, int y, float amount)
    {
        toHomeTrailMap[x, y] += amount;
    }

    public void ModifyFoodTrailMap(int x, int y, float amount)
    {
        toFoodTrailMap[x, y] += amount;
    }

    public void RemoveObstacle(Vector2Int position)
    {
        if (dataMap[position.x, position.y] == 2)
        {
            dataMap[position.x, position.y] = 0;
            Debug.Log("Succesfully destroyed wall pixel at: " + position);
        }

    }
    public void RemoveHiveBlock(Vector2Int position)
    {
        if (dataMap[position.x, position.y] == 4)
        {
            hiveBlocksDestroyed++;
            dataMap[position.x, position.y] = 0;
            Debug.Log("Succesfully destroyed HIVE pixel");
        }

    }

    public bool IsDataPointEmpty(Vector2Int dp)
    {
        
        if (dataMap[dp.x, dp.y] == 0 && foodMap[dp.x, dp.y] == 0) // this point is not Obsticle, Hive and contains no food already
        {
            return true;
        }
        return false;
    }

    // Update is called once per frame
    void Update()
    {
        antNumber = agents.Count;
    }
}
