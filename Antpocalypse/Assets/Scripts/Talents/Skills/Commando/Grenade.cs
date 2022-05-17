using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grenade : Talent
{
    float defaultRange = 25;

    public string GrenadeAnimObject_Path = "Commando/";
    public string GrenadeExplosionObject_Path = "Commando/";

    GameObject GrenadeAnimObject;
    GameObject GrenadeExplosionObject;


    [Header("Skill Specific Data")]
    public float baseAreaModifier; // Base area modifier, this must be tuned to fit the animation used
    public List<Sprite> grenadeTravelSprites;
    public float travelSpeedActual = 0.2f;
    [Header("Rank 1")]
    public float dmg_rank1;
    public float areaModifier_rank1;
    public float frequency_rank1;
    public float force_rank1;

    [Header("Rank 2")]
    public float dmg_rank2;
    public float areaModifier_rank2;
    public float frequency_rank2;
    public float force_rank2;

    [Header("Rank 3")]
    public float dmg_rank3;
    public float areaModifier_rank3;
    public float frequency_rank3;
    public float force_rank3;

    [Header("Rank 4")]
    public float dmg_rank4;
    public float areaModifier_rank4;
    public float frequency_rank4;
    public float force_rank4;

    [Header("Rank 5")]
    public float dmg_rank5;
    public float areaModifier_rank5;
    public float frequency_rank5;
    public float force_rank5;


    float frequencyActual, areaModifierActual, damageActual, forceActual;
   

    Camera playerCam;

    // Start is called before the first frame update
    void Start()
    {
        GrenadeAnimObject = Resources.Load<GameObject>(GrenadeAnimObject_Path);
        GrenadeExplosionObject = Resources.Load<GameObject>(GrenadeExplosionObject_Path);
    }

    float elapsedTime;
    // Update is called once per frame
    void Update()
    {
        if (playerModel == null)
            return;

        elapsedTime += Time.deltaTime;
        if (elapsedTime >= frequencyActual)
        {
            List<Enemy> validEnemies = new List<Enemy>();
            //Select all enemies in proximity
            Collider2D[] proximityObjects = Physics2D.OverlapCircleAll(playerModel.transform.position, defaultRange);
            
            for (int i = 0; i < proximityObjects.Length; i++)
            {
                Enemy en = proximityObjects[i].GetComponent<Enemy>();
                if (en != null) // this object is enemy
                {
                    Vector3 viewportPos = playerCam.WorldToViewportPoint(en.transform.position);
                    if (viewportPos.x > 0 && viewportPos.x < 1 && viewportPos.y > 0 && viewportPos.y < 1) // Enemy is visible onScreen
                    {
                        validEnemies.Add(en);
                    }
                }
            }
            proximityObjects = null;
            // Filter all enemies that are not currently visible onScreen


            // All enemies that are valid for Targeting are now in validEnemies list
            if (validEnemies.Count == 0)
                return;

            int validIndex = Random.Range(0, validEnemies.Count);

            StartCoroutine(GrenadeActual(validEnemies[validIndex]));
            
           

            




            /////////////////////////
            elapsedTime = 0;
        }

    }

    public IEnumerator GrenadeActual(Enemy targetEnemy)
    {
        GameObject grenade = Instantiate(GrenadeAnimObject, playerControl.transform.position, Quaternion.identity);
        Vector3 startPos = grenade.transform.position;
        Vector3 endPos = targetEnemy.transform.position;

        float startTime = Time.time;
        float journeyLenght = Vector3.Distance(startPos, endPos);

        float fractionOfJourney = 0;
        float increment = 1f / grenadeTravelSprites.Count;

        int enemiesKilled = 0, structuresDestroyed = 0;

        while (fractionOfJourney <= 1)
        {
            float distCovered = (Time.time - startTime) * travelSpeedActual;
            fractionOfJourney = distCovered / journeyLenght;

            grenade.transform.position = Vector3.Lerp(startPos, endPos, fractionOfJourney);

            // Animation
            int i = Mathf.FloorToInt(fractionOfJourney / increment);

           
            if (i >= grenadeTravelSprites.Count)
                break;

            grenade.GetComponent<SpriteRenderer>().sprite = grenadeTravelSprites[i];

            yield return null;
        }

        // Apply dmg and force
        GameObject explosionObject = Instantiate(GrenadeExplosionObject, grenade.transform.position, Quaternion.identity);
        Animator explosionAnim = explosionObject.GetComponent<Animator>();
        explosionObject.transform.localScale = new Vector3(areaModifierActual, areaModifierActual, 0);
        Destroy(grenade);

        explosionAnim.Play("Explosion", 0);
        Collider2D[] objectsHit = Physics2D.OverlapCircleAll(explosionObject.transform.position, baseAreaModifier * areaModifierActual);
        foreach (Collider2D collider in objectsHit)
        {
            IDamageable damagable = collider.GetComponent<IDamageable>();
            if (damagable != null)
            {
                bool destroyed;
                damagable.TakeDamage(damageActual, out destroyed);
                if (damagable is Enemy)
                {
                    Enemy enemy = (Enemy)damagable;
                    // Apply force
                    Vector2 forceVector = (enemy.transform.position - explosionObject.transform.position).normalized;
                    enemy.ApplyForce(forceVector * forceActual, ForceMode2D.Impulse);
                    if (destroyed)
                        enemiesKilled++;

                }
                else if (destroyed)
                {
                    structuresDestroyed++;
                }

            }
        }
        TotalEnemiesKilled += enemiesKilled;
        EnemiesKilled_LastInstance = enemiesKilled;

        TotalStructuresDestroyed += structuresDestroyed;
        StructuresDestroyed_LastInstance = structuresDestroyed;
       
        yield return null;
    }


    public override void ApplyTalent(PlayerModel player)
    {
        base.ApplyTalent(player);
        playerCam = playerControl.PlayerCamera;


        switch (talentLevel)
        {
            case 1:
                frequencyActual = frequency_rank1;
                damageActual = dmg_rank1;
                areaModifierActual = areaModifier_rank1;
                forceActual = force_rank1;
                break;
            case 2:
                frequencyActual = frequency_rank2;
                damageActual = dmg_rank2;
                areaModifierActual = areaModifier_rank2;
                forceActual = force_rank2;
                break;
            case 3:
                frequencyActual = frequency_rank3;
                damageActual = dmg_rank3;
                areaModifierActual = areaModifier_rank3;
                forceActual = force_rank3;
                break;
            case 4:
                frequencyActual = frequency_rank4;
                damageActual = dmg_rank4;
                areaModifierActual = areaModifier_rank4;
                forceActual = force_rank4;
                break;
            case 5:
                frequencyActual = frequency_rank5;
                damageActual = dmg_rank5;
                areaModifierActual = areaModifier_rank5;
                forceActual = force_rank5;
                break;
        }

    }

    public override void LevelUp()
    {
        base.LevelUp();
    }
}
