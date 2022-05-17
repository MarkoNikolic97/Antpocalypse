using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrostNova : Talent
{
    Animator skillAnimator;

    int totalEnemiesFrozen = 0;
    int enemiesFrozen_LastInstance;


    // Ranks are seperatly harcoded since, after experimentation, I realized it would actually be better if each rank could be completely independently modified
    [Header("Skill Specific Data")]
    public string frostNovaAnimController_Path = "Elementalist/FrostNovaAnimationObject";
    public string stunVisual_Path = "Elementalist/FrostNova_stun";

    [Header("Rank 1")]
    public float frequency;  //frequency at witch frostNova is activated
    public float stunDuration = 1f; // Duration of stun
    public float defaultRadius; //     This is the default radius that has to be tuned to match animation........................
    public float radiusModifier = 1f;


    [Header("Rank 2")]
    public float rank2_frequency; 
    public float rank2_Duration; 
    public float rank2_radiusModifier;

    [Header("Rank 3")]
    public float rank3_frequency;
    public float rank3_Duration;
    public float rank3_radiusModifier;

    [Header("Rank 4")]
    public float rank4_frequency;
    public float rank4_Duration;
    public float rank4_radiusModifier;

    [Header("Rank 5")]
    public float rank5_frequency;
    public float rank5_Duration;
    public float rank5_radiusModifier;


    float frequencyActual;
    float durationActual;
    float radiusModifierActual;


    private GameObject frostNovaAnimController;
    private GameObject stunVisual;
    // Start is called before the first frame update
    void Start()
    {
        frostNovaAnimController = Resources.Load<GameObject>(frostNovaAnimController_Path);
        stunVisual = Resources.Load<GameObject>(stunVisual_Path);
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

            StartCoroutine(FrostNovaActual());
            Debug.Log("FrostNova!");
            ///////
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
                durationActual = stunDuration;
                radiusModifierActual = radiusModifier;
                break;
            case 2:
                frequencyActual = rank2_frequency;
                durationActual = rank2_Duration;
                radiusModifierActual = rank2_radiusModifier;
                break;
            case 3:
                frequencyActual = rank3_frequency;
                durationActual = rank3_Duration;
                radiusModifierActual = rank3_radiusModifier;
                break;
            case 4:
                frequencyActual = rank4_frequency;
                durationActual = rank4_Duration;
                radiusModifierActual = rank4_radiusModifier;
                break;
            case 5:
                frequencyActual = rank5_frequency;
                durationActual = rank5_Duration;
                radiusModifierActual = rank5_radiusModifier;
                break;

        }

        if (skillAnimator == null)
        {

            GameObject animObj = Instantiate(frostNovaAnimController, player.transform.position, Quaternion.identity, player.transform);
              skillAnimator = animObj.GetComponent<Animator>();
           

            
        }
        // skillAnimator = playerControl.SkillAnimControl.GetComponent<Animator>();

    }

    public override void LevelUp()
    {
        base.LevelUp();
    }

    
    private IEnumerator FrostNovaActual() // Play Frost nova anim and then stun all enemies in range, as well as visually indicating each stunned enemy
    {
        yield return null;

        Vector3 previousLocalScale = skillAnimator.transform.localScale;

        skillAnimator.transform.localScale = new Vector3(radiusModifierActual, radiusModifierActual, radiusModifierActual);
        skillAnimator.Play("FrostNova", 0);

        int enemiesFrozen = 0;
        

        do
        {
            yield return null;
        } while (skillAnimator.GetCurrentAnimatorStateInfo(0).IsName("FrostNova"));

       // skillAnimator.Play("DefaultState", 0);

        Collider2D[] hitObjects = Physics2D.OverlapCircleAll(playerModel.transform.position, defaultRadius * radiusModifierActual);
        foreach (Collider2D collider in hitObjects)
        {
            Enemy enemy = collider.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemiesFrozen++;
                StartCoroutine(StunEnemy(enemy));
            }
        }

        enemiesFrozen_LastInstance = enemiesFrozen;
        totalEnemiesFrozen += enemiesFrozen;

        skillAnimator.transform.localScale = previousLocalScale;
        Debug.Log("frostNova has ended");
    }

    public IEnumerator StunEnemy(Enemy enemy)
    {
        Animator enemyAnim = enemy.GetComponentInChildren<Animator>();
        Debug.Log("Stun Enemy: " + enemy.name);
       

        GameObject stunIndicator = Instantiate(stunVisual, enemy.transform.position, Quaternion.identity);
       // enemy.controlEnabled = false;
       // enemyAnim.enabled = false;

       

        if (talentLevel == maxLevel) // Check if this is even balanced for higher lvl enemies like Shelly
        {
            bool destroyed;
            ((IDamageable)enemy).TakeDamage(10000,out destroyed);

            if (destroyed)
                TotalEnemiesKilled++;
        }
        else
        {
            StartCoroutine(enemy.CrowdControlled(durationActual));

            yield return new WaitForSeconds(durationActual);

            enemy.playerFocus = playerControl.gameObject;
            //enemy.controlEnabled = true;
            //enemyAnim.enabled = true;
        }
        yield return null;
        Destroy(stunIndicator);
    }


    public int TotalEnemiesFrozen { get => totalEnemiesFrozen;}
    public int EnemiesFrozen_LastInstance { get => enemiesFrozen_LastInstance;}
}
