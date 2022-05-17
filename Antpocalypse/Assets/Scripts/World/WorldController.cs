using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WorldController : MonoBehaviour
{
    

    public int startSimSteps;
    public float timeBetweenSteps = 1f;

    public bool spawnEnemies;
    public bool spawnPickups;
    public GameObject hiveObject;
    public GameObject WallTile, FoodTile, HiveTile;


    private WorldModel worldModel;
    public float enemySpawnDistance = 10;
    public float hiveProximityDistance = 10;
    public float pickupSpawnDistance = 10;

    [Header("UI Objects")]
    public GameObject StartingSimProgressBar;
    public GameObject StartSimProgressBarBackground;
    public GameObject WinConditionProgressBar;
    public GameObject RemainingFoodText;

    [Header("Optimization Settings")]
    public float viewDistance = 10f;

    HiveModel hiveModel;
    HiveController hiveControl;
    bool isLevelReady;

    List<GameObject> WallObjects = new List<GameObject>();
    List<GameObject> FoodObjects = new List<GameObject>();
    List<GameObject> HiveObjects = new List<GameObject>();
    // Start is called before the first frame update
    void Start()
    {
        worldModel = GetComponent<WorldModel>();
        hiveModel = hiveObject.GetComponent<HiveModel>();
        hiveControl = hiveObject.GetComponent<HiveController>();


    }


    float elapsedEnemySpawnTime;
    float elapsedPickupSpawnTime;

    float elapsedSimulationTime;
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F)) // This is for testing purposes
        {
            StartCoroutine(GenerateLevel());
        }


        if (isLevelReady && worldModel.Players[0].activeInHierarchy == false) // This is where final initialization occurs
        {
           // hiveControl.InitializeAntAgents();    
            InitializePlayer(worldModel.Players[0], Camera.main.gameObject);
            StartingSimProgressBar.SetActive(false);
            StartSimProgressBarBackground.SetActive(false);

            hiveModel.hiveFoodTarget = hiveModel.totalStartingFood / 3;


            spawnEnemies = true;
        }

        RemainingFoodText.GetComponent<TextMeshProUGUI>().text = (hiveModel.hiveFoodTarget - hiveModel.foodInHive).ToString();

        if (isLevelReady && worldModel.Players[0].activeInHierarchy == true && Time.timeScale != 0f) // Simulate Hive , Effective Update of the Hive
        {
            elapsedSimulationTime += Time.deltaTime;
            if (elapsedSimulationTime >= timeBetweenSteps)
            {
                Vector2 playerDataPos = hiveControl.WorldToDataPosition(worldModel.Players[0].transform.position);
                List<Agent> activeAgents = new List<Agent>();

                // Determine which agents are active(enabled) and witch are not ----> activeAgents
                foreach (Agent agent in hiveModel.agents)
                {
                    if (Vector2.Distance(playerDataPos, agent.position) <= viewDistance) // Agent is in viewing radius
                    {
                        agent.AntAgent.SetActive(true);
                        activeAgents.Add(agent);
                    }
                    else // agent is not in viewing radius
                    {
                        agent.AntAgent.SetActive(false);
                    }
                }


                // Initialize AntAgents to their current positions as decided by Agent position
                foreach (Agent agent in activeAgents)
                {
                    agent.AntAgent.GetComponent<AntAgent>().InitializeAntAgent();
                }


                // Sense + Move Step
                hiveControl.SimulateStep();
                hiveObject.GetComponent<HiveView>().ShowDataMap();


                // Set Destinations for AntAgents to current Agent position
                foreach (Agent agent in activeAgents)
                {
                    agent.AntAgent.GetComponent<AntAgent>().SetDestination(timeBetweenSteps);
                }


               
                elapsedSimulationTime = 0;
            }
        }

        #region WinCondition

       
        WinConditionProgressBar.GetComponent<Image>().fillAmount = hiveModel.hiveBlocksDestroyed / (hiveModel.hiveBlocksTotal * hiveModel.hiveBlocksWinPercentage);


        #endregion

        #region AmbientEnemySpawn
        elapsedEnemySpawnTime += Time.deltaTime;
        if (elapsedEnemySpawnTime >= worldModel.ambientEnemySpawnTime)
        {
            // Spawn 1 enemy
            if (spawnEnemies)
            {
                List<GameObject> enemies = worldModel.Enemies;
                List<int> chances = worldModel.EnemiesSpawnChance;

                int min = 0; int max = chances[0];
                int random = Random.Range(0, 100);

                for (int i = 0; i < enemies.Count; i++)
                {
                    if (random > min && random <= max)
                    {
                        GameObject enemy = enemies[i];
                        SpawnAmbientEnemy(worldModel.Players[0].GetComponent<PlayerController>(), enemySpawnDistance, enemy);
                        break;
                    }
                    min += max;
                    max += chances[i + 1];

                }
            }
            
            //
            elapsedEnemySpawnTime = 0;
        }

        #endregion


        #region PickupSpawn

        elapsedPickupSpawnTime += Time.fixedDeltaTime;
        if (elapsedPickupSpawnTime >= worldModel.pickupSpawnTime)
        {
            if (spawnPickups)
            {
                // Spawn 1 pickup
                int random = Random.Range(0, 100);
                if (random <= worldModel.hpSpawnChance)
                {
                    SpawnPickup(worldModel.Players[0].GetComponent<PlayerController>(), pickupSpawnDistance, "HP");
                }
                else
                {
                    SpawnPickup(worldModel.Players[0].GetComponent<PlayerController>(), pickupSpawnDistance, "EXP");
                }
            }
            //
            elapsedPickupSpawnTime = 0;

        }


        #endregion


    }

    public void SpawnAmbientEnemy(PlayerController player, float distance, GameObject baseEnemy)
    {
        float dist = Vector2.Distance(hiveControl.WorldToDataPosition(player.transform.position), hiveModel.hivePosition);
        Vector3 spawnPos = new Vector3();
        if (worldModel.Enemies_GameO.Count < worldModel.targetNumberOfEnemies) // Dont allow creation of more than max number of enemies
        {
            if (dist > hiveProximityDistance)
            {



                Vector3 wCenter = hiveControl.DataToWorldPosition(hiveModel.mapResolution / 2, hiveModel.mapResolution / 2);
                float size = hiveModel.mapResolution;
                Camera playerCam = player.PlayerCamera;


                float width = playerCam.scaledPixelWidth;
                float height = playerCam.scaledPixelHeight;

                // Rework so its dependant on ScaledPixelHeight & ScaledPixelWidth
                float ringWidth = 2f;

                Vector3 center = new Vector3(0.5f, 0.5f, 0f);
                Vector3 centerVector = Camera.main.ViewportToWorldPoint(center);
                centerVector.z = 0;

                float minValue = distance;
                float maxValue = minValue + ringWidth;
                float x, y;
                //Vector3 spawnPos;
                Vector2Int spawnPosDataPoint;
                do
                {
                    float r = Random.Range(minValue, maxValue);
                    float angle = Random.Range(0f, Mathf.PI * 2);

                    y = r * Mathf.Sin(angle);
                    x = r * Mathf.Cos(angle);

                    spawnPos = new Vector3(x, y, 0f) + centerVector;
                    spawnPosDataPoint = hiveControl.WorldToDataPosition(spawnPos);

                    //spawnPosDataPoint = new Vector2Int(Mathf.RoundToInt(spawnPos.x), Mathf.RoundToInt(spawnPos.y));

                } while (spawnPos.x < wCenter.x - size / 2 || spawnPos.x > wCenter.x + size / 2 || spawnPos.y < wCenter.y - size / 2 || spawnPos.y > wCenter.y + size / 2 || !hiveModel.IsDataPointEmpty(spawnPosDataPoint));
            }
            else // Player is in proximity of the hive -> Spawn Enemies on the Hive Edge
            {
                Debug.LogError("Not Implemented Exception");
            }



            GameObject enemy = Instantiate(baseEnemy, spawnPos, Quaternion.identity);
            enemy.SetActive(true);
            enemy.GetComponent<Enemy>().playerFocus = player.gameObject;
            worldModel.Enemies_GameO.Add(enemy);

            //Debug.Log(worldModel.Enemies_GameO.Count);
        }
    }




    public void InitializePlayer(GameObject player, GameObject playerCamera)
    {
        // Working under assumption that there is only one player
        player.SetActive(true);
        player.transform.position = hiveControl.DataToWorldPosition(hiveModel.playerStartPosition.x, hiveModel.playerStartPosition.y);

        playerCamera.transform.position = hiveControl.DataToWorldPosition(hiveModel.playerStartPosition.x, hiveModel.playerStartPosition.y) + new Vector3(0, 0, -10f);

    }

    
    #region WallGeneration


    bool[,] alreadyPassedWall;
    bool SampleAlreadyPassedWall(int x, int y, HiveModel hiveModel)
    {
        if (x < 0 || x >= hiveModel.mapResolution || y < 0 || y >= hiveModel.mapResolution)
        {
            return false;
        }
        else
        {
            return alreadyPassedWall[x, y];
        }
    }

    bool wallsGenerated;
    public IEnumerator GenerateWalls(HiveModel hiveModel)
    {
        foreach (GameObject g in WallObjects)
        {
            Destroy(g);
        }

        int resolution = hiveModel.mapResolution;

        alreadyPassedWall = new bool[resolution, resolution];

        for (int x = 0; x < resolution; x++)
        {
            for (int y = 0; y < resolution; y++)
            {
                if (alreadyPassedWall[x, y])
                    continue;

                if (hiveModel.SampleDataMap(x, y) == 2) // there is a wall here
                {
                    GameObject novi = new GameObject();
                    novi.transform.SetParent(transform);
                    CompositeCollider2D coll = novi.AddComponent<CompositeCollider2D>();
                    coll.generationType = CompositeCollider2D.GenerationType.Manual;
                    novi.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
                    GenerateWallObject(x, y, novi, hiveModel);
                    coll.GenerateGeometry();
                    WallObjects.Add(novi);



                    yield return null;
                }


            }
        }

        wallsGenerated = true;
        yield return null;
    }
    void GenerateWallObject(int x, int y, GameObject parent, HiveModel hiveModel)
    {
        if (SampleAlreadyPassedWall(x, y, hiveModel) || hiveModel.SampleDataMap(x, y) != 2)
            return;


        // Vector3 position = new Vector3(x, y, 0);  // correct orientation : new Vector3(-y, x)
        //Vector3 position = new Vector3(-y, x, 0);
        Vector3 position = hiveModel.GetComponent<HiveController>().DataToWorldPosition(x, y);
       
        GameObject tile = Instantiate(WallTile, position, Quaternion.identity, parent.transform);
        alreadyPassedWall[x, y] = true;


        GenerateWallObject(x, y - 1, parent, hiveModel);
        GenerateWallObject(x, y + 1, parent, hiveModel);
        GenerateWallObject(x - 1, y, parent, hiveModel);
        GenerateWallObject(x + 1, y, parent, hiveModel);

    }


    #endregion
    

    #region FoodGeneration


    bool[,] alreadyPassedFood;
    bool SampleAlreadyPassedFood(int x, int y, HiveModel hiveModel)
    {
        if (x < 0 || x >= hiveModel.mapResolution || y < 0 || y >= hiveModel.mapResolution)
        {
            return false;
        }
        else
        {
            return alreadyPassedFood[x, y];
        }
    }

    bool foodGenerated;
   
    public IEnumerator GenerateFood(HiveModel hiveModel)
    {
        

        int resolution = hiveModel.mapResolution;

        alreadyPassedFood = new bool[resolution, resolution];
        //List<GameObject> newFoodObjects = new List<GameObject>();
        foreach (GameObject g in FoodObjects)
        {
            Destroy(g);
        }

        for (int x = 0; x < resolution; x++)
        {
            for (int y = 0; y < resolution; y++)
            {
                if (alreadyPassedFood[x, y])
                    continue;

                if (hiveModel.SampleFoodMap(x, y) != 0) // there is a food here
                {
                    GameObject novi = new GameObject();
                    novi.transform.SetParent(transform);
                    CompositeCollider2D coll = novi.AddComponent<CompositeCollider2D>();
                    coll.generationType = CompositeCollider2D.GenerationType.Manual;
                    novi.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;                    
                    GenerateFoodObject(x, y, novi, hiveModel);
                    coll.GenerateGeometry();
                    FoodObjects.Add(novi);



                    yield return null;
                }


            }
        }
        
        // FoodObjects = newFoodObjects;

        foodGenerated = true;
        yield return null;
    }
    void GenerateFoodObject(int x, int y, GameObject parent, HiveModel hiveModel)
    {
        if (SampleAlreadyPassedFood(x, y, hiveModel) || hiveModel.SampleFoodMap(x, y) == 0)
            return;


        // Vector3 position = new Vector3(x, y, 0);  // correct orientation : new Vector3(-y, x)
        //Vector3 position = new Vector3(-y, x, 0);
        Vector3 position = hiveModel.GetComponent<HiveController>().DataToWorldPosition(x, y);
        
        GameObject tile = Instantiate(FoodTile, position, Quaternion.identity, parent.transform);
        alreadyPassedFood[x, y] = true;


        GenerateFoodObject(x, y - 1, parent, hiveModel);
        GenerateFoodObject(x, y + 1, parent, hiveModel);
        GenerateFoodObject(x - 1, y, parent, hiveModel);
        GenerateFoodObject(x + 1, y, parent, hiveModel);

    }


    #endregion
    

    #region HiveGeneration


    bool[,] alreadyPassedHive;
    bool SampleAlreadyPassedHive(int x, int y, HiveModel hiveModel)
    {
        if (x < 0 || x >= hiveModel.mapResolution || y < 0 || y >= hiveModel.mapResolution)
        {
            return false;
        }
        else
        {
            return alreadyPassedHive[x, y];
        }
    }

    bool hiveGenerated;
    public IEnumerator GenerateHive(HiveModel hiveModel)
    {
        foreach (GameObject g in HiveObjects)
        {
            Destroy(g);
        }

        int resolution = hiveModel.mapResolution;

        alreadyPassedHive = new bool[resolution, resolution];

        for (int x = 0; x < resolution; x++)
        {
            for (int y = 0; y < resolution; y++)
            {
                if (alreadyPassedHive[x, y])
                    continue;

                if (hiveModel.SampleDataMap(x, y) == 4) // there is a hive here
                {
                    GameObject novi = new GameObject();
                    novi.transform.SetParent(transform);
                    CompositeCollider2D coll = novi.AddComponent<CompositeCollider2D>();
                    coll.generationType = CompositeCollider2D.GenerationType.Manual;
                    novi.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
                    GenerateHiveObject(x, y, novi, hiveModel);
                    coll.GenerateGeometry();
                    HiveObjects.Add(novi);

                    novi.name = "Hive Object";

                    yield return null;
                }


            }
        }

        hiveGenerated = true;
        yield return null;
    }
    void GenerateHiveObject(int x, int y, GameObject parent, HiveModel hiveModel)
    {
        if (SampleAlreadyPassedHive(x, y, hiveModel) || hiveModel.SampleDataMap(x, y) != 4)
            return;


        // Vector3 position = new Vector3(x, y, 0);  // correct orientation : new Vector3(-y, x)
        //Vector3 position = new Vector3(-y, x, 0);
        Vector3 position = hiveModel.GetComponent<HiveController>().DataToWorldPosition(x, y);

        GameObject tile = Instantiate(HiveTile, position, Quaternion.identity, parent.transform);
        hiveModel.hiveBlocksTotal++;
       
        alreadyPassedHive[x, y] = true;


        GenerateHiveObject(x, y - 1, parent, hiveModel);
        GenerateHiveObject(x, y + 1, parent, hiveModel);
        GenerateHiveObject(x - 1, y, parent, hiveModel);
        GenerateHiveObject(x + 1, y, parent, hiveModel);

    }


    #endregion


    IEnumerator GenerateLevel()
    {


        StartCoroutine(SimulateStartingCondition(hiveObject));
        StartCoroutine(GenerateWalls(hiveModel));
        StartCoroutine(GenerateFood(hiveModel));
        StartCoroutine(GenerateHive(hiveModel));

        while (!wallsGenerated || !foodGenerated || !hiveGenerated || !startingSimDone)
        {
            yield return null;
        }
       
        Debug.Log("Level has been generated.");
        isLevelReady = true;

        yield return null;
    }

    bool startingSimDone;
    IEnumerator SimulateStartingCondition(GameObject hiveObject)
    {
        startingSimDone = false;

        HiveView hView = hiveObject.GetComponent<HiveView>();
        HiveModel hiveModel = hiveObject.GetComponent<HiveModel>();
        HiveController hiveControl = hiveObject.GetComponent<HiveController>();
        Image progressBar = StartingSimProgressBar.GetComponent<Image>();

        while (hiveModel.currentStep < startSimSteps)
        {
            hiveControl.SimulateStep();
            progressBar.fillAmount = (float)hiveModel.currentStep / (float)startSimSteps;

            hView.ShowDataMap();

            yield return null;
        }
        yield return null;
        Debug.Log("Starting Condition Simulated");
        startingSimDone = true;
    }



  
    public GameObject SpawnPickup(PlayerController player, float distance, string type)
    {
        Camera playerCam = player.PlayerCamera;

        float width = playerCam.scaledPixelWidth;
        float height = playerCam.scaledPixelHeight;

        // Rework so its dependant on ScaledPixelHeight & ScaledPixelWidth


        Vector3 center = new Vector3(0.5f, 0.5f, 0f);
        Vector3 centerVector = Camera.main.ViewportToWorldPoint(center);
        centerVector.z = 0;

        float minValue = distance;
        float maxValue = minValue + distance / 2;

        float r = Random.Range(minValue, maxValue);
        float angle = Random.Range(0f, Mathf.PI * 2);

        float y = r * Mathf.Sin(angle);
        float x = r * Mathf.Cos(angle);

        Vector3 spawnPos = new Vector3(x, y, 0f);
        GameObject pickup = null;

        if (type.Equals("HP"))
        {
            pickup = Instantiate(worldModel.HealthPickup, spawnPos + centerVector, Quaternion.identity);
        }
        else if (type.Equals("EXP"))
        {
            pickup = Instantiate(worldModel.ExpPickup, spawnPos + centerVector, Quaternion.identity);
        }
        else
        {
            Debug.Log("Type of Pickup not recognized");
        }

        return pickup;
       
    }

    public GameObject GetClosestEnemy(PlayerModel playerModel, Camera playerCam, float defaultRange)
    {
        //Select all enemies in proximity
        GameObject closestEnemy = null;
        float minDistance = defaultRange;

        Collider2D[] proximityObjects = Physics2D.OverlapCircleAll(playerModel.transform.position, defaultRange);
        for (int i = 0; i < proximityObjects.Length; i++)
        {
            Enemy en = proximityObjects[i].GetComponent<Enemy>();
            if (en != null) // this object is enemy
            {
               
                Vector3 viewportPos = playerCam.WorldToViewportPoint(en.transform.position);
                if (viewportPos.x > 0 && viewportPos.x < 1 && viewportPos.y > 0 && viewportPos.y < 1) // Enemy is visible onScreen
                {
                    // Enemy en is on screen and valid for autoAttack
                    //validEnemies.Add(enemy);
                    float dist = Vector3.Distance(playerModel.transform.position, en.transform.position);
                    if (dist < minDistance)
                    {
                        minDistance = dist;
                        closestEnemy = en.gameObject;
                    }
                }
            }
        }

        return closestEnemy;
    }

}
