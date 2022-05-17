using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Agent
{
    /* Agent Behaviour Model
    * 
    * Phase 1 - Agent is searching for food 
    *   IF no to_Food markers are detected, agent randomly wanders
    *   IF to_Food markers are detected, agent follows them
    * 
    * Phase 2 - Agent has taken food and is carrying it back to the Hive
    *   Agent follows to_Home markers
    * 
    */

    public GameObject AntAgent;
    public int phase = 1; // Phase 1 = Searching ; Phase 2 = Returning   Phase 3 = Patterning behaviour
  
    HiveModel model;

    public Vector2Int dataPosition;

    public Vector2 position;
    public Vector2 velocity;  // this, when normalized, is the forward direction
    public Vector2 desiredDirection;

    int carryCapacity = 1;
    int inventory;


    HiveController hiveControl;
    public Agent(Vector2Int dataPosition, Vector2 velocity, HiveModel model, bool collectMode, GameObject AntAgent)
    {
        this.dataPosition = dataPosition;
        this.velocity = velocity;
        this.model = model;

        hiveControl = model.GetComponent<HiveController>();

        position = dataPosition;
        desiredDirection = velocity;

        this.AntAgent = AntAgent;

        // Initialize AntAGent
        AntAgent.SetActive(false);
        AntAgent antAgent = AntAgent.GetComponent<AntAgent>();
        antAgent.hiveModel = model;
        antAgent.hiveControl = hiveControl;
        antAgent.agent = this;
        

        if (collectMode)
        {
            phase = 1;
        }
        else
        {
            phase = 3;
        }

        // Unity Instantiate only works from a monobehaviour object !
    }



    public void MoveStep()
    {
        //Debug.Log(phase);
        Vector2 newPos = new Vector2();
        do
        {
            desiredDirection = (desiredDirection + Random.insideUnitCircle * model.wanderStrenght).normalized;

            Vector2 desiredVelocity = desiredDirection * model.speed;
            Vector2 desiredSteeringForce = (desiredVelocity - velocity) * model.steerStrenght;
            Vector2 change = Vector2.ClampMagnitude(desiredSteeringForce, model.steerStrenght);

            velocity = Vector2.ClampMagnitude(velocity + change, model.speed);
            newPos = position + velocity;

        } while (TryMove(newPos, ref desiredDirection) == false);

        int newX = (int)newPos.x; int newY = (int)newPos.y;

        if (phase == 1) // Search Phase
        {
            
            if (model.SampleFoodMap(newX, newY) > 0) // Food has been found
            {              
                inventory = model.TakeFood(newX, newY, carryCapacity);
                phase = 2;

                velocity = model.hivePosition - dataPosition;
                velocity.Normalize();
                desiredDirection = velocity;

                return;
            }

            position = newPos;
            // Agent moved successfully

            model.dataMap[dataPosition.x, dataPosition.y] = 0;

            dataPosition.x = newX; // updated its Map position
            dataPosition.y = newY; // updated its Map position

            model.ModifyHomeTrailMap(dataPosition.x, dataPosition.y, model.toHome_DepositAmount); // updated toHomeTrailMap

            model.dataMap[dataPosition.x, dataPosition.y] = 1;

        }
        else if (phase == 2)
        {
           

            if (model.dataMap[newX, newY] == 4) // Food has been returned to the Hive
            {
                model.ReturnFood(inventory, dataPosition);
                inventory = 0;

                velocity = Rotate(velocity, 180);

                phase = 1;
                return;
            }

            position = newPos;
            // Agent moved successfully

            model.dataMap[dataPosition.x, dataPosition.y] = 0;

            dataPosition.x = newX; // updated its Map position
            dataPosition.y = newY; // updated its Map position

            model.ModifyFoodTrailMap(dataPosition.x, dataPosition.y, model.toFood_DepositAMount); // updated toFoodTrailMap

            model.dataMap[dataPosition.x, dataPosition.y] = 1;


        }
        else if (phase == 3)
        {
            position = newPos;
            // Agent moved successfully

            model.dataMap[dataPosition.x, dataPosition.y] = 0;

            dataPosition.x = newX; // updated its Map position
            dataPosition.y = newY; // updated its Map position

            model.ModifyHomeTrailMap(dataPosition.x, dataPosition.y, model.toHome_DepositAmount); // updated toFoodTrailMap

            model.dataMap[dataPosition.x, dataPosition.y] = 1;
        }

       // AntAgent.GetComponent<AntAgent>().SetDestination(GameObject.FindGameObjectWithTag("WORLD").GetComponent<WorldController>().timeBetweenSteps);


    }

    bool TryMove(Vector2 newPos, ref Vector2 desiredDir) // desiredDirection should be changed more intelligently than random but this works fine for now.
    {
        
        if ((int)newPos.x < 0 || (int)newPos.x >+ model.mapResolution || (int)newPos.y < 0 || (int)newPos.y >= model.mapResolution)
        {
            return false;
        }
        
        if (model.dataMap[(int)newPos.x, (int)newPos.y] == 2) 
        {
            velocity = Rotate(velocity, 180);
            return false;
        }
        if (phase == 1 && model.dataMap[(int)newPos.x, (int)newPos.y] == 4)
        {
           // desiredDir = Random.insideUnitCircle.normalized;
            return false;
        }
        

        return true;
    }


    public void SenseStep()
    {
        Vector2 forward = velocity.normalized;

        // Defining Sensors
        Vector2 leftSensorVector = Rotate(forward, model.sensorAngle) * model.sensorOffsetDistance;
        Vector2 rightSensorVector = Rotate(forward, -model.sensorAngle) * model.sensorOffsetDistance;
        Vector2 centerSensorVector = forward * model.sensorOffsetDistance;
        
        Vector2Int leftSensorPos = new Vector2Int(Mathf.RoundToInt(position.x + leftSensorVector.x), Mathf.RoundToInt(position.y + leftSensorVector.y));
        Vector2Int rightSensorPos = new Vector2Int(Mathf.RoundToInt(position.x + rightSensorVector.x), Mathf.RoundToInt(position.y + rightSensorVector.y));
        Vector2Int centerSensorPos = new Vector2Int(Mathf.RoundToInt(position.x + centerSensorVector.x), Mathf.RoundToInt(position.y + centerSensorVector.y));

        float leftSensor = 0, rightSensor = 0, centerSensor = 0;

        // Sample the sensor values
        if (phase == 1) // Searching Phase
        {
            leftSensor = UpdateSearchSensor(leftSensorPos);
            rightSensor = UpdateSearchSensor(rightSensorPos);
            centerSensor = UpdateSearchSensor(centerSensorPos);
        }
        else if (phase == 2) // Returning Phase
        {
            leftSensor = UpdateReturnSensor(leftSensorPos);
            rightSensor = UpdateReturnSensor(rightSensorPos);
            centerSensor = UpdateReturnSensor(centerSensorPos);
        }
        else if (phase == 3) // Patterning behaviour
        {
            leftSensor = UpdateReturnSensor(leftSensorPos);
            rightSensor = UpdateReturnSensor(rightSensorPos);
            centerSensor = UpdateReturnSensor(centerSensorPos);
        }
        
        // Agent Decision Logic
       


        string move = "----";
        if (centerSensor >= Mathf.Max(leftSensor, rightSensor)) // 
        {
            desiredDirection = velocity.normalized;
            move = "forward";
        }
        else if (leftSensor > rightSensor) // move left
        {
            desiredDirection = Rotate(velocity.normalized, model.agentRotationAngle);
            move = "left";
        }
        else if (rightSensor > leftSensor) // move right
        {
            desiredDirection = Rotate(velocity.normalized, -model.agentRotationAngle);
            move = "right";
        }


        // Check for small dropped pieces of food when searching for food
        Vector2Int closestFoodPos = Vector2Int.zero;
        float closestDist = float.MaxValue;
        if(phase == 1)
        {
            for (int i = dataPosition.x - 2; i < dataPosition.x +  2; i++)
            {
                for (int j = dataPosition.y -  2; j < dataPosition.y +  2; j++)
                {
                   
                    int food = model.SampleFoodMap(i, j);
                    if (food != 0)
                    {
                        Vector2Int foodPos = new Vector2Int(i, j);
                        float dist = Vector2Int.Distance(dataPosition, foodPos);

                        if (dist < closestDist)
                        {
                            closestDist = dist;
                            closestFoodPos = foodPos;
                        }
                        
                    }

                }
            }
            if (closestDist < 100000)
            {
                desiredDirection = (closestFoodPos - dataPosition);
                desiredDirection.Normalize();
                velocity = desiredDirection; // Perhaps not, investigate further, small behavioural change nontheless
            }
            


        }
        
 
    }

 


   



    /////////////

    public float UpdateSearchSensor(Vector2Int sensorPosition)
    {
        float totalSenseValue = 0;
        int totalFoodValue = 0;

        for (int i = sensorPosition.x - model.sensorWidth / 2; i < sensorPosition.x + model.sensorWidth / 2; i++)
        {
            for (int j = sensorPosition.y - model.sensorWidth / 2; j < sensorPosition.y + model.sensorWidth / 2; j++)
            {

                totalSenseValue += model.SampleFoodTrailMap(i, j);
                totalFoodValue += model.SampleFoodMap(i, j);



            }
        }

        if (totalFoodValue != 0)
            return totalFoodValue;

        return totalSenseValue;
    }

    public float UpdateReturnSensor(Vector2Int sensorPosition)
    {
        float totalSenseValue = 0;
        for (int i = sensorPosition.x - model.sensorWidth / 2; i < sensorPosition.x + model.sensorWidth / 2; i++)
        {
            for (int j = sensorPosition.y - model.sensorWidth / 2; j < sensorPosition.y + model.sensorWidth / 2; j++)
            {

                totalSenseValue += model.SampleHomeTrailMap(i, j);


            }
        }

        return totalSenseValue;
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




    public void DropFood(Vector3 antAgentPosition)
    {
        if (inventory == 0)
            return;

        antAgentPosition = new Vector3(Mathf.Round(antAgentPosition.x), Mathf.Round(antAgentPosition.y), 0);
        Vector2Int closestDataPos = hiveControl.WorldToDataPosition(antAgentPosition);
        model.AddFood(closestDataPos.x, closestDataPos.y, inventory);
        inventory = 0;

        phase = 1;

        Debug.Log("Food Dropped");

    }
}
