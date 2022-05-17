using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ant : Enemy 
{



    public GameObject HeadLookAtTarget;
    

    public float maxMagnitudeDelta = 0f;
    public float rotationSpeed = 2f;
    public float movementSpeed = 1f;

    WorldModel worldModel;
    
    // Start is called before the first frame update
    public override void Start()
    {
        maxHP = 10;
        worldModel = GameObject.FindGameObjectWithTag("WORLD").GetComponent<WorldModel>();

        base.Start();
    }

   

    public override void Update()
    {
        base.Update();

        if (controlEnabled)
        {

            movementSpeed = 0.6f;

            Vector3 toHeadDir = playerFocus.transform.position - transform.position;
            toHeadDir.Normalize();
            HeadLookAtTarget.transform.position = playerFocus.transform.position; // LookAt Player

            transform.up = Vector3.RotateTowards(transform.up, toHeadDir, rotationSpeed * Time.deltaTime, maxMagnitudeDelta);
            //transform.position = transform.position + toHeadDir * movementSpeed * Time.smoothDeltaTime;
           // GetComponent<Rigidbody2D>().velocity = toHeadDir * movementSpeed * Time.deltaTime;
            GetComponent<Rigidbody2D>().MovePosition(transform.position + toHeadDir * movementSpeed * Time.deltaTime);
        }
        else
        {
            //GetComponent<Rigidbody2D>().velocity = Vector3.zero;

        }
    }


    private void OnDestroy()
    {
        if (worldModel.Enemies_GameO.Contains(gameObject))
        {
            worldModel.Enemies_GameO.Remove(gameObject);
        }
    }

    public override IEnumerator CrowdControlled(float duration)
    {
        Animator animAnt = GetComponentInChildren<Animator>();
        // Implement dropping of food on ant Crowd Control
        controlEnabled = false;
        animAnt.enabled = false;

        yield return new WaitForSeconds(duration);

        controlEnabled = true;
        animAnt.enabled = true;
    }

}
