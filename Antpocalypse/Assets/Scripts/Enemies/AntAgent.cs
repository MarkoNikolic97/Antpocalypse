using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AntAgent : Enemy
{

    public GameObject HeadLookAtTarget;


    float maxMagnitudeDelta = 0f;
    public float rotationSpeed = 2f;
    public float movementSpeed = 1f;

    public HiveModel hiveModel;
    public HiveController hiveControl;


    bool isSimulated;  // isSimulated could be raplaced by the absence of PlayerFocus. Experiment later
    public Agent agent;

    Vector3 startPos, endPos;
    float journeyLenght;
    float speed;
    float startTime;

    float agentRotationSpeed = 1f;

    // Start is called before the first frame update
    public override void Start()
    {
        
    }

    // Update is called once per frame
    public override void Update()
    {
        if (controlEnabled)
        {
            if(isSimulated)
            {
                if (journeyLenght > 0)
                {


                    float distCovered = (Time.time - startTime) * speed;
                    float fractionOfJourney = distCovered / journeyLenght;


                    transform.position = Vector3.Lerp(startPos, endPos, fractionOfJourney);


                }

                float singleRotStep = agentRotationSpeed * Time.deltaTime;

                Vector3 newDirection = Vector3.RotateTowards(transform.right, agent.velocity.normalized, singleRotStep, 0.0f);
                transform.right = newDirection;

            }
            else // playerFocus mode
            {
                if (playerFocus == null)
                    return;
                // Implement PlayerFocus behaviour (Out of simulation behaviour)
                Vector3 toHeadDir = playerFocus.transform.position - transform.position;
                toHeadDir.Normalize();
                //HeadLookAtTarget.transform.position = playerFocus.transform.position; // LookAt Player

                transform.up = Vector3.RotateTowards(transform.up, toHeadDir, rotationSpeed * Time.deltaTime, maxMagnitudeDelta);
                //transform.position = transform.position + toHeadDir * movementSpeed * Time.smoothDeltaTime;
                GetComponent<Rigidbody2D>().velocity = toHeadDir * movementSpeed * Time.deltaTime;
            }
        }
        else
        {
           // GetComponent<Rigidbody2D>().velocity = Vector3.zero;
        }




      
    }

    

    public void InitializeAntAgent()
    {

        
       // this.hiveModel = hiveModel;
       // hiveControl = hiveModel.GetComponent<HiveController>();

        //this.agent = agent;
        isSimulated = true;

        transform.position = hiveControl.DataToWorldPosition(agent.position.x, agent.position.y);
        

       

    }

    public void SetDestination(float time)
    {

        startPos = transform.position;
        endPos = hiveControl.DataToWorldPosition(agent.position.x, agent.position.y);
        journeyLenght = Vector3.Distance(startPos, endPos);

        speed = journeyLenght / time;
        startTime = Time.time;

        //transform.position = hiveControl.DataToWorldPosition(agent.position.x,agent.position.y);


    }

    

    // Detach ant from simulation by removing it from antagents list as wall as its Agent counterpart from actual simulation(from Hive)
    public void DetachFromSimulation()
    {
        if (isSimulated)
        {
            agent.DropFood(transform.position);
            hiveModel.agents.Remove(agent);
           // hiveControl.AntAgents.Remove(gameObject);
            isSimulated = false;
        }
        

    }

    public override IEnumerator CrowdControlled(float duration)
    {
        Animator animAnt = GetComponentInChildren<Animator>();
        // Implement dropping of food on ant Crowd Control
        controlEnabled = false;
        DetachFromSimulation();
        animAnt.enabled = false;

        yield return new WaitForSeconds(duration);

        controlEnabled = true;
        animAnt.enabled = true;

        
        
    }

    

    private void OnDestroy()
    {
        DetachFromSimulation();
    }
}
