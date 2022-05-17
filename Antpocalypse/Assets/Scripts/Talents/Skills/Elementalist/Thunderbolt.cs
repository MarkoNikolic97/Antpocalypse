using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Thunderbolt : Talent
{

    float defaultRange = 25f; // We use this so we wouldnt have to pass thru all enemies to see shich one is onScreen

   

    // Ranks are seperatly harcoded since, after experimentation, I realized it would actually be better if each rank could be completely independently modified
    [Header("Skill Specific Data")]
    public string ThunderboltAnim_Path = "Elementalist/ThunderboltAnimObject";

    public float baseAreaModifier; // Base area modifier, this must be tuned to fit the animation used

    [Header("Rank 1")]
    public float frequency;
    public int nThunderbolts;
    public float areaModifier;
    public float damage;

    [Header("Rank 2")]
    public float frequency_rank2;
    public int nThunderbolts_rank2;
    public float areaModifier_rank2;
    public float damage_rank2;

    [Header("Rank 3")]
    public float frequency_rank3;
    public int nThunderbolts_rank3;
    public float areaModifier_rank3;
    public float damage_rank3;

    [Header("Rank 4")]
    public float frequency_rank4;
    public int nThunderbolts_rank4;
    public float areaModifier_rank4;
    public float damage_rank4;

    [Header("Rank 5")]
    public float frequency_rank5;
    public int nThunderbolts_rank5;
    public float areaModifier_rank5;
    public float damage_rank5;


    float frequencyActual, areaModifierActual, damageActual;
    int nThunderboltsActual;

    int[] enemiesKilledPerBolt_LastInstance;

    private GameObject ThunderboltAnimController;
    Camera playerCam;
    // Start is called before the first frame update
    void Start()
    {
        ThunderboltAnimController = Resources.Load<GameObject>(ThunderboltAnim_Path);
    }

    
    float elapsedTime;
    // Update is called once per frame
    void Update()
    {
        
        if (playerModel == null)
            return;
        // PlayerModel exists; that is, this talent is selected and applied to player

        Debug.Log("Enemies Killed LAst Instance: " + EnemiesKilled_LastInstance);
        if (enemiesKilledPerBolt_LastInstance != null)
        {
            for (int i = 0; i < nThunderboltsActual; i++)
            {
                Debug.Log("Bolt index " + i + " killed " + enemiesKilledPerBolt_LastInstance[i]);
            }
        }

        elapsedTime += Time.deltaTime;
        if (elapsedTime >= frequencyActual)
        {
            

            List<Enemy> validEnemies = new List<Enemy>();
            // Select enemies to thunderbolt and call ThunderboltActual on them
            ///////////////////////////////////////////////////////////////////
            //Select all enemies in proximity
            Collider2D[] proximityObjects = Physics2D.OverlapCircleAll(playerModel.transform.position, defaultRange);
            for (int i = 0; i < proximityObjects.Length; i++)
            {
                Enemy en = proximityObjects[i].GetComponent<Enemy>();
                if (en != null) // this object is enemy
                { 
                    // Filter all enemies that are not currently visible onScreen
                    Vector3 viewportPos = playerCam.WorldToViewportPoint(en.transform.position);
                    if (viewportPos.x > 0 && viewportPos.x < 1 && viewportPos.y > 0 && viewportPos.y < 1) // Enemy is visible onScreen
                    {
                        validEnemies.Add(en);
                    }
                }
            }
            proximityObjects = null;

            if (validEnemies.Count == 0) // there are no valid enemies on screen
            {
                elapsedTime = 0;
                return;
            }
            // if there is at least 1 valid enemy on screen reset stats
            enemiesKilledPerBolt_LastInstance = new int[nThunderboltsActual];
            EnemiesKilled_LastInstance = 0;
            StructuresDestroyed_LastInstance = 0;

            // All enemies that are valid for Thunderbolt are now in validEnemies list
            for (int i = 0; i < nThunderboltsActual; i++)
            {
                if (validEnemies.Count == 0)
                {
                    elapsedTime = 0;
                    return;
                }
                int r = Random.Range(0, validEnemies.Count - 1);
                Enemy chosen = validEnemies[r];
                validEnemies.RemoveAt(r);

                StartCoroutine(ThunderboltActual(chosen.gameObject, i));
            }
            

            //StartCoroutine(ThunderboltActual(TEST));



            ////////////////
            elapsedTime = 0;
        }
    }



   // public GameObject TEST;
    public IEnumerator ThunderboltActual(GameObject enemy, int boltIndex)
    {
        GameObject animObj = Instantiate(ThunderboltAnimController, enemy.transform.position, Quaternion.identity);
        Animator boltAnim = animObj.GetComponent<Animator>();

        GameObject boltArea = animObj.transform.GetChild(0).gameObject;

        Vector3 previousAreaScale = boltArea.transform.localScale;
        boltArea.transform.localScale = previousAreaScale * areaModifierActual;

        int enemiesKilled = 0, structuresDestroyed = 0;

        boltAnim.Play("Thunderbolt", 0);
        do
        {
            yield return null;
        } while (boltAnim.GetCurrentAnimatorStateInfo(0).IsName("Thunderbolt"));

        
        Collider2D[] hitObjects = Physics2D.OverlapCircleAll(animObj.transform.position, baseAreaModifier * areaModifierActual);
        foreach (Collider2D col in hitObjects)
        {
            IDamageable dmg = col.GetComponent<IDamageable>();
            if (dmg != null)// this object hit is an Enemy
            {
                bool destroyed;
                dmg.TakeDamage(damageActual, out destroyed);
                if (dmg is Enemy && destroyed)
                {
                    enemiesKilled++;
                }
                else if(destroyed) // by some chance player destroyed a structure with a bolt
                {
                    structuresDestroyed++;
                }

                
            }
        }
        enemiesKilledPerBolt_LastInstance[boltIndex] = enemiesKilled;
        TotalEnemiesKilled += enemiesKilled;
        TotalStructuresDestroyed += structuresDestroyed;

        EnemiesKilled_LastInstance += enemiesKilled;
        StructuresDestroyed_LastInstance += structuresDestroyed;

        do
        {
            yield return null;
        } while (boltAnim.GetCurrentAnimatorStateInfo(0).IsName("Thunderbolt_Hit"));

        Destroy(animObj);

       


        yield return null;
    }

    public override void ApplyTalent(PlayerModel player)
    {
        base.ApplyTalent(player);

        playerCam = playerControl.PlayerCamera;

        switch (talentLevel)
        {
            case 1:
                frequencyActual = frequency;
                damageActual = damage;
                areaModifierActual = areaModifier;
                nThunderboltsActual = nThunderbolts;
                break;
            case 2:
                frequencyActual = frequency_rank2;
                damageActual = damage_rank2;
                areaModifierActual = areaModifier_rank2;
                nThunderboltsActual = nThunderbolts_rank2;
                break;
            case 3:
                frequencyActual = frequency_rank3;
                damageActual = damage_rank3;
                areaModifierActual = areaModifier_rank3;
                nThunderboltsActual = nThunderbolts_rank3;
                break;
            case 4:
                frequencyActual = frequency_rank4;
                damageActual = damage_rank4;
                areaModifierActual = areaModifier_rank4;
                nThunderboltsActual = nThunderbolts_rank4;
                break;
            case 5:
                frequencyActual = frequency_rank5;
                damageActual = damage_rank5;
                areaModifierActual = areaModifier_rank5;
                nThunderboltsActual = nThunderbolts_rank5;
                break;

        }



    }

    public override void LevelUp()
    {
        base.LevelUp();
    }

}




/*
 void Update()
    {
        
        if (playerModel == null)
            return;
        // PlayerModel exists; that is, this talent is selected and applied to player

        elapsedTime += Time.deltaTime;
        if (elapsedTime >= frequencyActual)
        {
            List<Enemy> validEnemies = new List<Enemy>();
            // Select enemies to thunderbolt and call ThunderboltActual on them
            ///////////////////////////////////////////////////////////////////
            //Select all enemies in proximity
            Collider2D[] proximityObjects = Physics2D.OverlapCircleAll(playerModel.transform.position, defaultRange);
            List<Enemy> proximityEnemies = new List<Enemy>();
            for (int i = 0; i < proximityObjects.Length; i++)
            {
                Enemy en = proximityObjects[i].GetComponent<Enemy>();
                if (en != null) // this object is enemy
                {
                    proximityEnemies.Add(en);
                }
            }
            proximityObjects = null;
            // Filter all enemies that are not currently visible onScreen
            
            foreach (Enemy enemy in proximityEnemies)
            {
                Vector3 viewportPos = playerCam.WorldToViewportPoint(enemy.transform.position);
                if (viewportPos.x > 0 && viewportPos.x < 1 && viewportPos.y > 0 && viewportPos.y < 1) // Enemy is visible onScreen
                {
                    validEnemies.Add(enemy);
                }
            }
            proximityEnemies = null;
            // All enemies that are valid for Thunderbolt are now in validEnemies list
            for (int i = 0; i < nThunderboltsActual; i++)
            {
                int r = Random.Range(0, validEnemies.Count - 1);
                Enemy chosen = validEnemies[r];
                validEnemies.RemoveAt(r);

                StartCoroutine(ThunderboltActual(chosen.gameObject));
            }


            //StartCoroutine(ThunderboltActual(TEST_ENEMY));



            ////////////////
            elapsedTime = 0;
        }
    }
      
     
     */
