using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HiveView : MonoBehaviour
{
    protected HiveModel model;
    protected HiveController controller;

    public int foodWeight = 1;
    public bool isFoodWide;

    public GameObject ViewPlane;
    public GameObject toHomeTrailView;
    public GameObject toFoodTrailView;

    Renderer rend;
    // Start is called before the first frame update
    void Start()
    {
        rend = ViewPlane.GetComponent<MeshRenderer>();
        model = GetComponent<HiveModel>();
        controller = GetComponent<HiveController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            ShowHomeTrailMap();
        }

        if (Input.GetMouseButton(0)) // Painting the obsticle map
        {
           // Debug.Log("clck");
            Vector2Int clickedUVcoord = new Vector2Int();

            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit))
            {
                Transform objectHit = hit.transform;


                clickedUVcoord.x =Mathf.RoundToInt(hit.textureCoord.x * model.mapResolution); 
                clickedUVcoord.y = Mathf.RoundToInt(hit.textureCoord.y * model.mapResolution); 

                setObsticle(clickedUVcoord.x, clickedUVcoord.y);
                
            }
        }
        else if (Input.GetMouseButtonUp(0))
        {
            ShowDataMap();
        }


        if (Input.GetMouseButton(1)) // Painting the food map
        {
           // Debug.Log("clck");
            Vector2Int clickedUVcoord = new Vector2Int();

            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit))
            {
                Transform objectHit = hit.transform;


                clickedUVcoord.x = Mathf.RoundToInt(hit.textureCoord.x * model.mapResolution);
                clickedUVcoord.y = Mathf.RoundToInt(hit.textureCoord.y * model.mapResolution);

                SetFood(clickedUVcoord.x, clickedUVcoord.y, foodWeight, isFoodWide);

            }
        }
        else if (Input.GetMouseButtonUp(1))
        {
            ShowDataMap();
        }

    }

    public void ShowDataMap()
    {
        Color backgroundColor = Color.grey;
        Color agentColor = Color.red;
        Color obstacleColor = Color.black;
        Color foodColor = Color.green;
        Color hiveColor = Color.yellow;

        Texture2D tex = new Texture2D(model.mapResolution, model.mapResolution);
        for (int i = 0; i < model.mapResolution; i++)
        {
            for (int j = 0; j < model.mapResolution; j++)
            {
                if (model.dataMap[i,j] == 0)
                {
                    tex.SetPixel(i, j, backgroundColor);
                }
                else if (model.dataMap[i,j] == 1)
                {
                  //  colorAgent(i, j, tex, agentColor);

                }
                else if (model.dataMap[i,j] == 2)
                {
                    tex.SetPixel(i, j, obstacleColor);
                }
                else if (model.dataMap[i,j] == 4)
                {
                    tex.SetPixel(i, j, hiveColor);
                }

                if (model.foodMap[i,j] != 0)
                {
                    tex.SetPixel(i, j, foodColor);
                }
                if (model.dataMap[i,j] == 5)
                {
                    tex.SetPixel(i, j, Color.blue);
                    model.dataMap[i, j] = 0;
                }


            }
        }

        foreach (Agent agent in model.agents)
        {
            
            if (agent.phase == 2)
            {
                colorAgent(agent.dataPosition.x, agent.dataPosition.y, tex, foodColor);
            }
            else
            {
                colorAgent(agent.dataPosition.x, agent.dataPosition.y, tex, agentColor);
            }
            
        }
        /*
        // Show player
        GameObject player = GameObject.FindGameObjectWithTag("PLAYER");
        Vector2Int playerPos = controller.WorldToDataPosition(player.transform.position);

        colorAgent(playerPos.x, playerPos.y, tex, Color.blue);

        */

        tex.Apply();
        rend.material.SetTexture("_MainTex", tex);
        

    }

    public void ShowCurrentSensors(int agentID)
    {
        Agent agent = model.agents[agentID];

        Vector2 forward = agent.velocity.normalized;

        // Defining Sensors
        Vector2 leftSensorVector = Rotate(forward, model.sensorAngle) * model.sensorOffsetDistance;
        Vector2 rightSensorVector = Rotate(forward, -model.sensorAngle) * model.sensorOffsetDistance;
        Vector2 centerSensorVector = forward * model.sensorOffsetDistance;

        Vector2Int leftSensorPos = new Vector2Int(Mathf.RoundToInt(agent.position.x + leftSensorVector.x), Mathf.RoundToInt(agent.position.y + leftSensorVector.y));
        Vector2Int rightSensorPos = new Vector2Int(Mathf.RoundToInt(agent.position.x + rightSensorVector.x), Mathf.RoundToInt(agent.position.y + rightSensorVector.y));
        Vector2Int centerSensorPos = new Vector2Int(Mathf.RoundToInt(agent.position.x + centerSensorVector.x), Mathf.RoundToInt(agent.position.y + centerSensorVector.y));

        for (int i = leftSensorPos.x - model.sensorWidth / 2; i < leftSensorPos.x + model.sensorWidth / 2; i++)
        {
            for (int j = leftSensorPos.y - model.sensorWidth / 2; j < leftSensorPos.y + model.sensorWidth / 2; j++)
            {
                model.dataMap[i, j] = 5;
        
            }
        }
        for (int i = rightSensorPos.x - model.sensorWidth / 2; i < rightSensorPos.x + model.sensorWidth / 2; i++)
        {
            for (int j = rightSensorPos.y - model.sensorWidth / 2; j < rightSensorPos.y + model.sensorWidth / 2; j++)
            {
                model.dataMap[i, j] = 5;

            }
        }
        for (int i = centerSensorPos.x - model.sensorWidth / 2; i < centerSensorPos.x + model.sensorWidth / 2; i++)
        {
            for (int j = centerSensorPos.y - model.sensorWidth / 2; j < centerSensorPos.y + model.sensorWidth / 2; j++)
            {
                model.dataMap[i, j] = 5;

            }
        }


    }

    public void ShowHomeTrailMap()
    {
        float value;
      
        Texture2D tex = new Texture2D(model.mapResolution, model.mapResolution);
        for (int i = 0; i < model.mapResolution; i++)
        {
            for (int j = 0; j < model.mapResolution; j++)
            {
                value = model.SampleHomeTrailMap(i, j);
                tex.SetPixel(i, j, new Color(value, value, value));
            }
        }

        tex.Apply();
        toHomeTrailView.GetComponent<Renderer>().material.SetTexture("_MainTex", tex);
        


    }

    public void ShowFoodTrailMap()
    {
        float value;

        Texture2D tex = new Texture2D(model.mapResolution, model.mapResolution);
        for (int i = 0; i < model.mapResolution; i++)
        {
            for (int j = 0; j < model.mapResolution; j++)
            {
                value = model.SampleFoodTrailMap(i, j);
                tex.SetPixel(i, j, new Color(value, value, value));
            }
        }

        tex.Apply();
        toFoodTrailView.GetComponent<Renderer>().material.SetTexture("_MainTex", tex);



    }


    void colorAgent(int i, int j, Texture2D tex, Color agentColor)
    {
        tex.SetPixel(i, j, agentColor); // actuall agent position

        /*
        tex.SetPixel(i-1, j-1, agentColor);
        tex.SetPixel(i, j-1, agentColor);
        tex.SetPixel(i+1, j-1, agentColor);

        tex.SetPixel(i+1, j+1, agentColor);
        tex.SetPixel(i, j+1, agentColor);
        tex.SetPixel(i+1, j+1, agentColor);

        tex.SetPixel(i-1, j, agentColor);
        tex.SetPixel(i+1, j, agentColor);
        */
    }

    void setObsticle(int x, int y)
    {
        model.dataMap[x, y] = 2; // actual clicked obsticle

        model.dataMap[x-1, y-1] = 2;
        model.dataMap[x, y-1] = 2;
        model.dataMap[x+1, y-1] = 2;

        model.dataMap[x+1, y+1] = 2;
        model.dataMap[x, y+1] = 2;
        model.dataMap[x+1, y+1] = 2;

        model.dataMap[x-1, y] = 2;
        model.dataMap[x+1, y] = 2;
    }

    void SetFood(int x, int y, int weight, bool wide)
    {
        model.foodMap[x, y] = weight; 

        if (wide)
        {
            model.foodMap[x - 1, y - 1] = weight;
            model.foodMap[x, y - 1] = weight;
            model.foodMap[x + 1, y - 1] = weight;

            model.foodMap[x + 1, y + 1] = weight;
            model.foodMap[x, y + 1] = weight;
            model.foodMap[x + 1, y + 1] = weight;

            model.foodMap[x - 1, y] = weight;
            model.foodMap[x + 1, y] = weight;
        }
    }


    Vector2 Rotate(Vector2 v, float degrees)
    {
        float sin = Mathf.Sin(degrees * Mathf.Deg2Rad);
        float cos = Mathf.Cos(degrees * Mathf.Deg2Rad);

        float tx = v.x;
        float ty = v.y;
        v.x = (cos * tx) - (sin * ty);
        v.y = (sin * tx) + (cos * ty);
        return v;

    }

}
