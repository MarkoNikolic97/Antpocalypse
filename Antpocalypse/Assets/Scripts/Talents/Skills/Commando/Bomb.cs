using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : Talent
{
    [Header("Skill Specific Data")]
    public string BombAnimObject_Path = "Commando/BombAnimObject";
    public string ExplosionAnimObject_Path = "Commando/BombExplosionAnimObject";
    GameObject BombAnimObject;
    GameObject ExplosionAnimObject;

    public float baseAreaModifier; // Base area modifier, this must be tuned to fit the animation used

    [Header("Rank 1")]
    public float dmg_rank1;
    public float tickDurationSpeed_rank1;
    public float areaModifier_rank1;
    public float frequency_rank1;
    public float force_rank1;

    [Header("Rank 2")]
    public float dmg_rank2;
    public float tickDurationSpeed_rank2;
    public float areaModifier_rank2;
    public float frequency_rank2;
    public float force_rank2;

    [Header("Rank 3")]
    public float dmg_rank3;
    public float tickDurationSpeed_rank3;
    public float areaModifier_rank3;
    public float frequency_rank3;
    public float force_rank3;

    [Header("Rank 4")]
    public float dmg_rank4;
    public float tickDurationSpeed_rank4;
    public float areaModifier_rank4;
    public float frequency_rank4;
    public float force_rank4;

    [Header("Rank 5")]
    public float dmg_rank5;
    public float tickDurationSpeed_rank5;
    public float areaModifier_rank5;
    public float frequency_rank5;
    public float force_rank5;


    float frequencyActual, areaModifierActual, damageActual, tickDurationSpeedActual, forceActual;


    void Start()
    {
        // Initialize Resouces
        BombAnimObject = Resources.Load<GameObject>(BombAnimObject_Path);
        ExplosionAnimObject = Resources.Load<GameObject>(ExplosionAnimObject_Path);
    }

    float elapsedTime;
    void Update()
    {
        if (playerModel == null) // Not yet selected and applied to player
            return;

        Debug.Log("Enemies Killed Lst Instance: " + EnemiesKilled_LastInstance);
        Debug.Log("Totatl enemies Killed: " + TotalEnemiesKilled);

        elapsedTime += Time.deltaTime;
        if (elapsedTime >= frequencyActual)
        {

            StartCoroutine(BombActual(playerModel.transform.position));

            elapsedTime = 0;
        }


    }

    public IEnumerator BombActual(Vector3 bombPos)
    {
        // Phase1- Placeing and Ticking
        //bombPos -= new Vector3(3, 0, 0); // FOR TESTING PURPOSES

        GameObject bombObject = Instantiate(BombAnimObject, bombPos, Quaternion.identity);
        Animator bombAnimator = bombObject.GetComponent<Animator>();

       
        bombAnimator.Play("BombTicking", 0);
        bombAnimator.speed = tickDurationSpeedActual;
        do
        {
            yield return null;
        } while (bombAnimator.GetCurrentAnimatorStateInfo(0).IsName("BombTicking"));    // Wait until animation is finished

        Destroy(bombObject); // Will probably break

        GameObject expObject = Instantiate(ExplosionAnimObject, bombPos, Quaternion.identity);
        Animator expAnimator = expObject.GetComponent<Animator>();

        int enemiesKilled = 0, structuresDestroyed = 0;

        expObject.transform.localScale *= areaModifierActual; // Set explosion size

        // Apply dmg and force
        Collider2D[] hitObjects = Physics2D.OverlapCircleAll(bombPos, baseAreaModifier * areaModifierActual);
        foreach (Collider2D collider in hitObjects)
        {
            IDamageable damagable = collider.GetComponent<IDamageable>();
            if (damagable != null)
            {
                bool destroyed;
                damagable.TakeDamage(damageActual, out destroyed);
                if(damagable is Enemy)
                {
                    Enemy enemy = (Enemy)damagable;
                    // Apply force
                    Vector2 forceVector = (enemy.transform.position - bombPos).normalized;
                    //enemy.GetComponent<Rigidbody2D>().AddForce(forceVector * forceActual, ForceMode2D.Impulse);
                    StartCoroutine(enemy.ApplyForce(forceVector * forceActual, ForceMode2D.Impulse));
                    if (destroyed)
                        enemiesKilled++;

                }
                else if(destroyed)
                {
                    structuresDestroyed++;
                }

            }
            
        }
        TotalEnemiesKilled += enemiesKilled;
        EnemiesKilled_LastInstance = enemiesKilled;

        TotalStructuresDestroyed += structuresDestroyed;
        StructuresDestroyed_LastInstance = structuresDestroyed;

        //
        expAnimator.Play("Explosion", 0);

        do
        {
            yield return null;
        } while (!expAnimator.GetCurrentAnimatorStateInfo(0).IsName("Default State"));

        Destroy(expObject);

        yield return null;
    }


    public override void ApplyTalent(PlayerModel player)
    {
        base.ApplyTalent(player);

       

        switch (talentLevel)
        {
            case 1:
                frequencyActual = frequency_rank1;
                damageActual = dmg_rank1;
                areaModifierActual = areaModifier_rank1;
                tickDurationSpeedActual = tickDurationSpeed_rank1;
                forceActual = force_rank1;
                break;
            case 2:
                frequencyActual = frequency_rank2;
                damageActual = dmg_rank2;
                areaModifierActual = areaModifier_rank2;
                tickDurationSpeedActual = tickDurationSpeed_rank2;
                forceActual = force_rank2;
                break;
            case 3:
                frequencyActual = frequency_rank3;
                damageActual = dmg_rank3;
                areaModifierActual = areaModifier_rank3;
                tickDurationSpeedActual = tickDurationSpeed_rank3;
                forceActual = force_rank3;
                break;
            case 4:
                frequencyActual = frequency_rank4;
                damageActual = dmg_rank4;
                areaModifierActual = areaModifier_rank4;
                tickDurationSpeedActual = tickDurationSpeed_rank4;
                forceActual = force_rank4;
                break;
            case 5:
                frequencyActual = frequency_rank5;
                damageActual = dmg_rank5;
                areaModifierActual = areaModifier_rank5;
                tickDurationSpeedActual = tickDurationSpeed_rank5;
                forceActual = force_rank5;
                break;

        }
    }

    public override void LevelUp()
    {
        base.LevelUp();
    }

}
