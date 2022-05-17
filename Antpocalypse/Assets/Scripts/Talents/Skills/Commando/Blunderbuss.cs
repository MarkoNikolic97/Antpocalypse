using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blunderbuss : Talent
{
    public string BlunderbussAnimObject_Path = "Commando/";
    GameObject BlunderbussAnimObject;
    Animator skillAnimator;

    public float defaultAngle = 60; // 60 degrees in radians
    public float wideAngle = 90; // 90 degree in radians
    public float range; // max distance of enemy to player. This is  dependant on the animation

    [Header("Rank 1")]
    public float frequency_rank1;
    public float damage_rank1;
    public float knockbackForce_rank1;
    public bool isWide_rank1;


    [Header("Rank 2")]
    public float frequency_rank2;
    public float damage_rank2;
    public float knockbackForce_rank2;
    public bool isWide_rank2;

    [Header("Rank 3")]
    public float frequency_rank3;
    public float damage_rank3;
    public float knockbackForce_rank3;
    public bool isWide_rank3;

    [Header("Rank 4")]
    public float frequency_rank4;
    public float damage_rank4;
    public float knockbackForce_rank4;
    public bool isWide_rank4;

    [Header("Rank 5")]
    public float frequency_rank5;
    public float damage_rank5;
    public float knockbackForce_rank5;
    public bool isWide_rank5;


    float frequencyActual, damageActual, knockbackForceActual;
    bool isWideActual;
    // Start is called before the first frame update
    void Start()
    {
        BlunderbussAnimObject = Resources.Load<GameObject>(BlunderbussAnimObject_Path);
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

            StartCoroutine(BlunderbussActual());
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
                frequencyActual = frequency_rank1;
                knockbackForceActual = knockbackForce_rank1;
                damageActual = damage_rank1;
                isWideActual = isWide_rank1;
                break;
            case 2:
                frequencyActual = frequency_rank2;
                knockbackForceActual = knockbackForce_rank2;
                damageActual = damage_rank2;
                isWideActual = isWide_rank2;
                break;
            case 3:
                frequencyActual = frequency_rank3;
                knockbackForceActual = knockbackForce_rank3;
                damageActual = damage_rank3;
                isWideActual = isWide_rank3;
                break;
            case 4:
                frequencyActual = frequency_rank4;
                knockbackForceActual = knockbackForce_rank4;
                damageActual = damage_rank4;
                isWideActual = isWide_rank4;
                break;
            case 5:
                frequencyActual = frequency_rank5;
                knockbackForceActual = knockbackForce_rank5;
                damageActual = damage_rank5;
                isWideActual = isWide_rank5;
                break;

        }

        if (skillAnimator == null)
        {

            GameObject animObj = Instantiate(BlunderbussAnimObject, player.transform.position, Quaternion.identity, player.transform);
            skillAnimator = animObj.GetComponent<Animator>();

        }
    }
    
    public override void LevelUp()
    {
        base.LevelUp();
    }

    public IEnumerator BlunderbussActual()
    {
        //GameObject AnimObject = Instantiate(BlunderbussAnimObject, playerControl.transform.position, Quaternion.identity);
        //Animator skillAnimator = AnimObject.GetComponent<Animator>();

        skillAnimator.gameObject.SetActive(true);

        float targetAngle;
        if (isWideActual)
            targetAngle = wideAngle / 2;
        else
            targetAngle = defaultAngle / 2;

        Vector3 targetDir = playerControl.playerDirection;
        skillAnimator.transform.up = targetDir;

        if (isWideActual)
            skillAnimator.Play("Wide", 0);
        else
            skillAnimator.Play("Normal", 0);

        int enemiesKilled = 0, structuresDestroyed = 0;

        Collider2D[] proximityObjects = Physics2D.OverlapCircleAll(playerModel.transform.position, range);

        int HELPER = 0;

        foreach (Collider2D collider in proximityObjects)
        {
            Enemy e = collider.GetComponent<Enemy>();
            if (e != null)
            {
                Vector3 objDir = (collider.transform.position - playerControl.transform.position).normalized;
                float angle = Vector3.Angle(objDir, targetDir);
                if (angle <= targetAngle)
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
                            Vector2 forceVector = (enemy.transform.position - playerControl.transform.position).normalized;
                            enemy.ApplyForce(forceVector * knockbackForceActual, ForceMode2D.Impulse);
                            if (destroyed)
                                enemiesKilled++;

                        }
                        else if (destroyed)
                        {
                            structuresDestroyed++;
                        }

                        HELPER++;

                    }
                }
            }
        }
        Debug.Log(HELPER);

        AnimatorClipInfo[] m_CurrentClipInfo;
        m_CurrentClipInfo = skillAnimator.GetCurrentAnimatorClipInfo(0);
        yield return new WaitForSeconds(m_CurrentClipInfo[0].clip.length);

        skillAnimator.gameObject.SetActive(false);

        TotalEnemiesKilled += enemiesKilled;
        EnemiesKilled_LastInstance = enemiesKilled;

        TotalStructuresDestroyed += structuresDestroyed;
        StructuresDestroyed_LastInstance = structuresDestroyed;



        yield return null;
    }
}
