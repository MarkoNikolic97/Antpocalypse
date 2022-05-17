using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBreath : Talent
{
    Animator skillAnimator;

    // Ranks are seperatly harcoded since, after experimentation, I realized it would actually be better if each rank could be completely independently modified
    [Header("Skill Specific Data")]
    public string fireBreathController_Path = "Elementalist/FireBreathAnimObject";
    public float dmgTickFrequency;
    public float defaultAngle = 60; // 60 degrees in radians
    public float wideAngle = 90; // 90 degree in radians
    public float range; // max distance of enemy to player. This is  dependant on the animation

    [Header("Rank 1")]
    public float frequency; 
    public float damage = 1f; 
    public float duration;
    public bool isWide;


    [Header("Rank 2")]
    public float frequency_rank2;
    public float damage_rank2;
    public float duration_rank2;
    public bool isWide_rank2;

    [Header("Rank 3")]
    public float frequency_rank3;
    public float damage_rank3;
    public float duration_rank3;
    public bool isWide_rank3;

    [Header("Rank 4")]
    public float frequency_rank4;
    public float damage_rank4;
    public float duration_rank4;
    public bool isWide_rank4;

    [Header("Rank 5")]
    public float frequency_rank5;
    public float damage_rank5;
    public float duration_rank5;
    public bool isWide_rank5;


    float frequencyActual, damageActual, durationActual;
    bool isWideActual;

    private GameObject fireBreathAnimController;
    // Start is called before the first frame update
    void Start()
    {
        fireBreathAnimController = Resources.Load<GameObject>(fireBreathController_Path);
    }

    float elapsedTime;
    // Update is called once per frame
    void Update()
    {
        if (playerModel == null)
            return;
        // PlayerModel exists; that is, this talent is selected and applied to player

        elapsedTime += Time.deltaTime;
        if (elapsedTime >= frequencyActual)
        {

            StartCoroutine(FirebreathActual());

            ////
            elapsedTime = 0;
        }
        
    }

    public override void ApplyTalent(PlayerModel player)
    {
        base.ApplyTalent(player);

        switch (talentLevel)
        {
            case 1:
                frequencyActual = frequency;
                durationActual = duration;
                damageActual = damage;
                isWideActual = isWide;
                break;
            case 2:
                frequencyActual = frequency_rank2;
                durationActual = duration_rank2;
                damageActual = damage_rank2;
                isWideActual = isWide_rank2;
                break;
            case 3:
                frequencyActual = frequency_rank3;
                durationActual = duration_rank3;
                damageActual = damage_rank3;
                isWideActual = isWide_rank3;
                break;
            case 4:
                frequencyActual = frequency_rank4;
                durationActual = duration_rank4;
                damageActual = damage_rank4;
                isWideActual = isWide_rank4;
                break;
            case 5:
                frequencyActual = frequency_rank5;
                durationActual = duration_rank5;
                damageActual = damage_rank5;
                isWideActual = isWide_rank5;
                break;

        }

        if (skillAnimator == null)
        {

            GameObject animObj = Instantiate(fireBreathAnimController, player.transform.position, Quaternion.identity, player.transform);
            skillAnimator = animObj.GetComponent<Animator>();
          
        }
    }

    public override void LevelUp()
    {
        base.LevelUp();
    }

    public IEnumerator FirebreathActual()
    {
        skillAnimator.gameObject.SetActive(true);
        float targetAngle;

        if (isWideActual)
        {
            skillAnimator.Play("FireBreathWide", 0);
            targetAngle = wideAngle / 2;
        }
        else
        {
            skillAnimator.Play("FireBreath", 0);
            targetAngle = defaultAngle / 2;
        }

        float elapsedTime = 0;
        float elapsedDamageTime = 0;


        int enemiesKilled = 0, structuresDestroyed = 0;
        do
        {
            Vector3 playerDir = playerControl.playerDirection;

            elapsedDamageTime += Time.deltaTime;
            elapsedTime += Time.deltaTime;

            skillAnimator.transform.up = playerDir;

            if (elapsedDamageTime >= dmgTickFrequency)
            {
               
               

                Collider2D[] objectsInRange = Physics2D.OverlapCircleAll(playerModel.transform.position, range);
                

                foreach (Collider2D col in objectsInRange)
                {
                    GameObject obj = col.gameObject;
                    Vector3 objectDir = (obj.transform.position - playerModel.transform.position).normalized;
                    float angle = Vector3.Angle(objectDir, playerDir);

                    IDamageable damagableObject = obj.GetComponent<IDamageable>();
                    if (damagableObject != null)
                    {
                        if (angle <= targetAngle)
                        {
                            bool destroyed;
                            damagableObject.TakeDamage(damageActual, out destroyed);

                            if (damagableObject is Enemy && destroyed)
                                enemiesKilled++;
                            else if (destroyed)
                                structuresDestroyed++;
                            
                        }
                    }

                                                        
                }
                
                ///
                elapsedDamageTime = 0;
            }


            yield return null;
        } while (elapsedTime < durationActual);

        TotalEnemiesKilled += enemiesKilled; TotalStructuresDestroyed += structuresDestroyed;
        EnemiesKilled_LastInstance = enemiesKilled; StructuresDestroyed_LastInstance = structuresDestroyed;

        skillAnimator.gameObject.SetActive(false);

        Debug.Log("Enemies Killed Last Instance: " + EnemiesKilled_LastInstance);
        Debug.Log("Total Enemies Killed: " + TotalEnemiesKilled);

        yield return null;
    }
}
