using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HiveController : MonoBehaviour
{
    /* References:
 * https://sandipanweb.wordpress.com/2017/08/24/diffusion-and-variational-methods-in-image-processing-and-computer-vision/
 * "The following figures show that the Gaussian Blur on an image can be thought of as heat flow, since the solution of the (linear) diffusion equation is typically the Gaussian Kernel convolution,
 * as shown in the 1-D case, the diffusion equation has a unique solution with Gaussian convolution kernel G(0,σ=√(2t)),
 * where the bandwidth of the kernel changes proportional to the square root of the time."
 * 
 * 
 * https://www.youtube.com/watch?v=C_zFhWdM4ic
 * https://www.youtube.com/watch?v=SiJpkucGa1o&t=520s
 * https://stackoverflow.com/questions/14916085/simpliest-way-to-generate-a-1d-gaussian-kernel
 * https://hackernoon.com/how-to-implement-gaussian-blur-zw28312m
 * 
 * 
 */

 

    
    public GameObject AntAgentRef;

    protected HiveModel model;
    protected HiveView view;
    bool isSimulationRunning = false;

    public float secondsPerUpdate;

    protected float[] kernel1D_Home;
    protected float[] kernel1D_Food;
    // Start is called before the first frame update
    void Start()
    {
       

        model = GetComponent<HiveModel>();
        view = GetComponent<HiveView>();

        InitializeHive(model.HiveMode);

        //Initialize Diffusion kernel
        kernel1D_Home = CalculateGaussKernel1D(model.DiffusionSigma_Home, model.DiffusionMean_Home, model.kernelLenght_Home);
        kernel1D_Food = CalculateGaussKernel1D(model.DiffusionSigma_Food, model.DiffusionMean_Food, model.kernelLenght_Food);



    }

    float elapsedTime = 0f;

    // Update is called once per frame
    void Update()
    {

      


    }

    public void InitializeAntAgents()
    {
        foreach (Agent agent in model.agents)
        {
           // agent.AntAgent.GetComponent<AntAgent>().InitializeAntAgent(agent, model);
        }

       /* for (int i = 0; i < model.agents.Count; i++)
        {
            GameObject antAgentObj = Instantiate(AntAgentRef);
            antAgentObj.SetActive(true);
            AntAgent antAgent = antAgentObj.GetComponent<AntAgent>();
            antAgent.InitializeAntAgent(model.agents[i], model);

            AntAgents.Add(antAgentObj);
        }
       */
        Debug.Log("Ant agents are initialized");
    }

    

    void DecayMap(float[,] map,float decayValue, bool isDecay, int stepDelay)
    {
        if (isDecay == false)
        {
            return;
        }
        if (stepDelay != 0 && model.currentStep % stepDelay != 0)
        {
            return;
        }

        for (int x = 0; x < model.mapResolution; x++)
        {
            for (int y = 0; y < model.mapResolution; y++)
            {
                map[x, y] -= decayValue;
                if (map[x,y] < 0)
                {
                    map[x, y] = 0;
                }
            }
        }


    }
    float[,] DiffuseMap(float[,] map, float[] kernel, bool isDiffuse, int stepDelay)
    {
        if (isDiffuse == false)
        {
            return map;
        }
        if (stepDelay != 0 && model.currentStep % stepDelay != 0)
        {
            return map;
        }

        //kernel1D = CalculateGaussKernel1D(Mathf.Sqrt(2 * Time.smoothDeltaTime), 0, model.kernelLenght);

        int n = model.mapResolution;
        int kernelHalf = kernel.Length / 2;

        
        float[,] intermediateMap = new float[n, n];
        float[,] outputMap = new float[n, n];

        float newValue;

        // First 1D pass (X axis) of Gaussian Blur
        for (int x = 0; x < n; x++)
        {
            for (int y = 0; y < n; y++)
            {

                newValue = 0f;
                for (int i = 0; i < kernel.Length; i++)
                {
                    newValue += kernel[i] * SampleMapValue(x + i - kernelHalf, y, map, n);
                }
                intermediateMap[x, y] = newValue;
            }
        }
        // Second 1D pass (Y axis) of gaussian Blur
        for (int x = 0; x < n; x++)
        {
            for (int y = 0; y < n; y++)
            {
                newValue = 0f;
                for (int i = 0; i < kernel.Length; i++)
                {
                    newValue += kernel[i] * SampleMapValue(x, y + i - kernelHalf, intermediateMap, n);
                }
                outputMap[x, y] = newValue;

            }
        }

        return outputMap;
    }

    public void SimulateStep()
    {
        SimulateSenseStep();

        model.toHomeTrailMap = DiffuseMap(model.toHomeTrailMap, kernel1D_Home, model.diffusion_Home, model.diffusionStepDelay_Home);
        model.toFoodTrailMap = DiffuseMap(model.toFoodTrailMap, kernel1D_Food, model.diffusion_Food, model.diffusionStepDelay_Food);

        DecayMap(model.toHomeTrailMap, model.decayAmount_Home, model.decay_Home, model.decayStepDelay_Home);
        DecayMap(model.toFoodTrailMap, model.decayAmount_Food, model.decay_Food, model.decayStepDelay_Food);

        SimulateMoveStep();

        GetComponent<HiveView>().ShowHomeTrailMap();
        GetComponent<HiveView>().ShowFoodTrailMap();

        model.currentStep++;

    }

    public void SimulateMoveStep()
    {
        foreach (Agent agent in model.agents.ToArray())
        {
            
            agent.MoveStep();
        }
    }

    public void SimulateSenseStep()
    {
        foreach (Agent agent in model.agents.ToArray())
        {
            
            agent.SenseStep();
        }
    }

    float[] CalculatePascalArray(int n)
    {
        float[][] triangle = new float[n][];

        for (int row = 0; row < triangle.Length; row++)
        {
            triangle[row] = new float[row + 1];

            for (int i = 0; i < triangle[row].Length; i++)
            {
                if (i > 0 && i < triangle[row].Length - 1)
                {
                    triangle[row][i] = triangle[row - 1][i - 1] + triangle[row - 1][i];

                }
                else
                {
                    triangle[row][i] = 1;
                }


            }
        }

        return triangle[n - 1];
        
    }

    float[] CalculateGaussKernel1D(float sigma, float mean, int lenght)
    {
        float[] result = new float[lenght];
        int mid = lenght / 2;


        float sum = 0;
        int x = -mid;
        for (int i = 0; i < lenght; i++)
        {
            result[i] = (1 / (sigma * Mathf.Sqrt(2 * Mathf.PI))) * Mathf.Exp(-(Mathf.Pow(x - mean, 2) / 2 * Mathf.Pow(sigma, 2)));
            sum += result[i];
            x++;
        }

        for (int i = 0; i < lenght; i++)
        {
            result[i] /= sum;
        }
    

        return result;
    }

    float SampleMapValue(int x, int y, float[,] map, int n)
    {
        if (x < 0 || x >= n || y < 0 || y >= n)
        {
            return 0;
        }
        else
        {
            return map[x, y];
        }

    }

    public void InitializeHive(string mode)
    {
        model.dataMap = new int[model.mapResolution, model.mapResolution];
        model.foodMap = new int[model.mapResolution, model.mapResolution];

        model.toHomeTrailMap = new float[model.mapResolution, model.mapResolution];
        model.toFoodTrailMap = new float[model.mapResolution, model.mapResolution];

        // Set Hive Position
        int hivePosX = Random.Range(model.hiveRadius * 2, model.mapResolution - model.hiveRadius * 2);
        int hivePosY = Random.Range(model.hiveRadius * 2, model.mapResolution - model.hiveRadius * 2);
        model.hivePosition = new Vector2Int(hivePosX, hivePosY);

        // set bounderies
        for (int i = 0; i < model.mapResolution; i++)
            model.dataMap[i, 0] = 2;
        for (int i = 0; i < model.mapResolution; i++)
            model.dataMap[i, model.mapResolution - 1] = 2;
        for (int j = 0; j < model.mapResolution; j++)
            model.dataMap[0, j] = 2;
        for (int j = 0; j < model.mapResolution; j++)
            model.dataMap[model.mapResolution - 1, j] = 2;

        // Generate Obsticles and Hive Cave
        GenerateObsticles(model.hivePosition, model.hiveCaveRadius);


        if (mode.Equals("collect"))
        {

            for (int i = 0; i < model.agentStartNum; i++)
            {
                //Vector2Int startPos = new Vector2Int(Random.Range(10, mapResolution - 10), Random.Range(10, mapResolution - 10));
                Vector2 onCircle = Random.insideUnitCircle.normalized;

                Vector2Int startPos = new Vector2Int((int)(onCircle.x * (model.hiveRadius + 2)), (int)(onCircle.y * (model.hiveRadius + 2))) + model.hivePosition;


                // Vector2 startVelocity = Random.insideUnitCircle.normalized;
                Vector2 startVelocity = startPos - model.hivePosition;
                startVelocity.Normalize();

                GameObject antAgentObj = Instantiate(AntAgentRef);
                Agent agent = new Agent(startPos, startVelocity, model, true, antAgentObj);

                model.dataMap[startPos.x, startPos.y] = 1;

                model.agents.Add(agent);
            }

            // Generate Hive
           
            for (int i = -model.hiveRadius; i <= model.hiveRadius; i++)
            {
                for (int j = -model.hiveRadius; j <= model.hiveRadius; j++)
                {
                    int dataPosX = i + model.hivePosition.x;
                    int dataPosY = j + model.hivePosition.y;
                    if (dataPosX > 0 && dataPosX < model.mapResolution && dataPosY > 0 && dataPosY < model.mapResolution)
                    {

                        if (i * i + j * j <= model.hiveRadius * model.hiveRadius)
                        {
                            model.dataMap[dataPosX, dataPosY] = 4;
                            model.toHomeTrailMap[dataPosX, dataPosY] = 100000;
                           
                        }
                    }

                }
            }


        }
        else if (mode.Equals("pattern"))
        {
            for (int i = 0; i < model.agentStartNum; i++)
            {
                Vector2Int startPos = new Vector2Int(Random.Range(10, model.mapResolution - 10), Random.Range(10, model.mapResolution - 10));


                 Vector2 startVelocity = Random.insideUnitCircle.normalized;
                startVelocity.Normalize();

                //Agent agent = new Agent(startPos, startVelocity, model, false);
                model.dataMap[startPos.x, startPos.y] = 1;

              //  model.agents.Add(agent);
            }
        }

        /// Generate Food

        model.totalStartingFood = GenerateFood();

        // Determine Player Starting Position; Player must be spawned on point that is Empty and not inside Hive Cave
        Vector2Int playerStartPos; // 20 so that Player cannot see the EdgeOfMap on spawn
        do
        {
            playerStartPos = new Vector2Int(Random.Range(20, model.mapResolution - 20), Random.Range(20, model.mapResolution - 20));
        } while (!model.IsDataPointEmpty(playerStartPos) || IsPointInHiveCave(playerStartPos));

        model.playerStartPosition = playerStartPos;

        view.ShowDataMap();
    }


    public Vector2Int WorldToDataPosition(Vector3 worldPos)
    {
        int wX = (int)worldPos.x;
        int wY = (int)worldPos.y;

        int dataX = wY;
        int dataY = -wX;

        return new Vector2Int(dataX, dataY);
    }

    public Vector3 DataToWorldPosition(int dataX, int dataY)
    {

        int worldX = -dataY;
        int worldY = dataX;

        return new Vector3(worldX, worldY, 0);
    }

    public Vector3 DataToWorldPosition(float dataX, float dataY)
    {

        float worldX = -dataY;
        float worldY = dataX;

        return new Vector3(worldX, worldY, 0);
    }


    public void GenerateObsticles(Vector2Int hivePosition, int hiveCaveRadius)
    {
        // NoiseValue is in range from -1 to 1
        float cutoffDensity = model.cutoffDensity;
        int seed = Random.Range(1000, 1000000);
        FastNoiseLite noise = new FastNoiseLite(seed);
        if (model.noiseFunction.Equals("perlin"))
        {
            noise.SetNoiseType(FastNoiseLite.NoiseType.Perlin);
        }


        // Set noise values
        noise.SetFrequency(model.noiseFrequency);
        noise.SetFractalLacunarity(model.noiseLacunarity);
        noise.SetFractalGain(model.noiseGain);
        noise.SetFractalOctaves(model.noiseOctaves);

       

        for (int x = 0; x < model.mapResolution; x++)
        {
            for (int y = 0; y < model.mapResolution; y++)
            {
                float noiseValue = noise.GetNoise(x, y);
                if(noiseValue >= cutoffDensity)
                {
                    model.dataMap[x, y] = 2;
                }

                
            }
        }
        for (int x = -hiveCaveRadius; x < hiveCaveRadius; x++)
        {
            for (int y = -hiveCaveRadius; y < hiveCaveRadius; y++)
            {
                int dataPosX = x + model.hivePosition.x;
                int dataPosY = y + model.hivePosition.y;
                if (dataPosX > 1 && dataPosX < model.mapResolution - 1 && dataPosY > 1 && dataPosY < model.mapResolution - 1)
                {

                    if (x * x + y * y < hiveCaveRadius * hiveCaveRadius)
                    {
                        model.dataMap[dataPosX, dataPosY] = 0;
                       
                    }
                }
            }
        }

        
    }

    public int GenerateFood() // Returnes total food generated on the Map
    {
        int totalMapFood = 0;

        int foodPointNumber = Random.Range(model.foodPoints_Min, model.foodPoints_Max);
        int foodPointRadius, foodPerPixel;
        Vector2Int fpCenter;

        for (int i = 0; i < foodPointNumber; i++)
        {
            foodPointRadius = Random.Range(model.foodPointRadius_Min, model.foodPointRadius_Max);
            foodPerPixel = Random.Range(model.foodPerPixel_Min, model.foodPerPixel_Max);

            // After this block foodPoint Center will be determined and available           
            do
            {
                fpCenter = new Vector2Int(Random.Range(foodPointRadius, model.mapResolution - foodPointRadius), Random.Range(foodPointRadius, model.mapResolution - foodPointRadius));
            } while (!model.IsDataPointEmpty(fpCenter) || IsPointInHiveCave(fpCenter));
            //
            totalMapFood += GenerateFoodPoint(fpCenter, foodPointRadius, foodPerPixel);
            

        }

        return totalMapFood;
    }

    int GenerateFoodPoint(Vector2Int center, int radius, int foodAmount) // Returns total amount of food deposited
    {
        int totalFood = 0;
        for (int x = center.x - radius; x <= center.x + radius; x++)
        {
            for (int y = center.y - radius; y <= center.y + radius; y++)
            {

                if (x > 0 && x < model.mapResolution && y > 0 && y < model.mapResolution)
                {
                    Vector2Int point = new Vector2Int(x, y);
                    if (Vector2Int.Distance(center, point) <= radius)
                    {
                        model.foodMap[x, y] = foodAmount;
                        totalFood += foodAmount;
                        if (model.dataMap[x,y] != 0) // Food will overwrite any obsticle
                        {
                            model.dataMap[x, y] = 0;
                        }
                    }
                }
            }
        }

        return totalFood;
    }

    bool IsPointInHiveCave(Vector2Int point)
    {
        if (Vector2Int.Distance(model.hivePosition, point) <= model.hiveCaveRadius)
            return true;
        return false;

    }
}
