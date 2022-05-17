using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPickup : MonoBehaviour
{
    public float maxDistance = 20f;
    public int minPickupLvl = 1, maxPickupLvl = 3;
    public float hpPerLvl = 5, scalePerLvl = 0.1f;

    public int pickupLvl;
    public float hpAmount;

    WorldModel worldModel;
    // Start is called before the first frame update
    void Start()
    {
        pickupLvl = Random.Range(minPickupLvl, maxPickupLvl);

        Vector3 scale = new Vector3(pickupLvl * scalePerLvl, pickupLvl * scalePerLvl, pickupLvl * scalePerLvl);
        hpAmount = hpPerLvl * pickupLvl;

        transform.localScale = scale;

        worldModel = GameObject.FindGameObjectWithTag("WORLD").GetComponent<WorldModel>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 scale = new Vector3(pickupLvl * scalePerLvl, pickupLvl * scalePerLvl, pickupLvl * scalePerLvl);
       

        transform.localScale = scale;

        if (Vector3.Distance(transform.position, worldModel.Players[0].transform.position) >= maxDistance)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        GameObject go = collision.gameObject;
        if (go.CompareTag("PLAYER"))
        {
            PlayerController p = go.GetComponent<PlayerController>();
            p.ApplyHealth(hpAmount);
            Destroy(gameObject);
        }
       
    }
}
