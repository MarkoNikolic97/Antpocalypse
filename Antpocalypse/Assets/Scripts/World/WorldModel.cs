using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldModel : MonoBehaviour
{

    public List<GameObject> Players = new List<GameObject>();
    public List<GameObject> Enemies;
    public List<int> EnemiesSpawnChance; // When aggregated MUST equal 100!
    public float ambientEnemySpawnTime = 1f;


    public int targetNumberOfEnemies = 40;
    public List<GameObject> Enemies_GameO = new List<GameObject>();

    public GameObject HealthPickup;
    public GameObject ExpPickup;
    public float pickupSpawnTime = 1f;
    public float hpSpawnChance = 30; // ExpSpawnChane = 100 - hpSpawnChance


   

    // Start is called before the first frame update
    void Start()
    {
        Enemies_GameO.Add(Instantiate(Enemies[0], Vector3.zero, Quaternion.identity));
        Enemies_GameO[0].GetComponent<Ant>().playerFocus = Players[0];
    }

    // Update is called once per frame
    void Update()
    {
      //  Debug.Log(Enemies_GameO.Count);
    }


    public GameObject GetClosestEnemy()
    {
        return Enemies_GameO[0];

    }



}
