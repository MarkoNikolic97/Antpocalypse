using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommandoShield : Talent
{

    Animator ShieldAnimator;

    public string ShieldAnimObject_Path = "Commando/";
    GameObject ShieldAnimObject;

    //   duration << frequency
    [Header("Rank 1")]
    public float frequency_rank1;
    public float hitsBlocked_rank1;
    public float duration_rank1;

    [Header("Rank 2")]
    public float frequency_rank2;
    public float hitsBlocked_rank2;
    public float duration_rank2;

    [Header("Rank 3")]
    public float frequency_rank3;
    public float hitsBlocked_rank3;
    public float duration_rank3;

    [Header("Rank 4")]
    public float frequency_rank4;
    public float hitsBlocked_rank4;
    public float duration_rank4;

    [Header("Rank 5")]
    public float frequency_rank5;
    public float hitsBlocked_rank5;
    public float duration_rank5;

    float frequencyActual, hitsBlockedActual, durationActual;


    public bool isShieldUp;
    int currentHitBlocked;
   // PlayerController playerControl;
    // Start is called before the first frame update
    void Start()
    {
        ShieldAnimObject = Resources.Load<GameObject>(ShieldAnimObject_Path);
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

            StartCoroutine(ShieldActual());

            ////////
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
                hitsBlockedActual = hitsBlocked_rank1;
                durationActual = duration_rank1;
                break;
            case 2:
                frequencyActual = frequency_rank2;
                hitsBlockedActual = hitsBlocked_rank2;
                durationActual = duration_rank2;
                break;
            case 3:
                frequencyActual = frequency_rank3;
                hitsBlockedActual = hitsBlocked_rank3;
                durationActual = duration_rank3;
                break;
            case 4:
                frequencyActual = frequency_rank4;
                hitsBlockedActual = hitsBlocked_rank4;
                durationActual = duration_rank4;
                break;
            case 5:
                frequencyActual = frequency_rank5;
                hitsBlockedActual = hitsBlocked_rank5;
                durationActual = duration_rank5;
                break;

        }

        if (ShieldAnimator == null)
        {
            ShieldAnimator = Instantiate(ShieldAnimObject, playerModel.transform.position, Quaternion.identity, playerModel.transform).GetComponent<Animator>();
            ShieldAnimator.gameObject.SetActive(false);
            ShieldAnimator.GetComponent<CommandoShieldHelper>().shield = this;
        }


    }

    public override void LevelUp()
    {
        base.LevelUp();
    }


    public IEnumerator ShieldActual()
    {

        isShieldUp = true;
        currentHitBlocked = 0;
        playerControl.isInvincible = true;

        ShieldAnimator.gameObject.SetActive(true);
        ShieldAnimator.Play("ShieldStatic",0);

        float elapsedTime = 0;
        while (elapsedTime < durationActual)
        {
            if (currentHitBlocked >= hitsBlockedActual)
            {
                break;
            }


            yield return null;
            elapsedTime += Time.deltaTime;
        }

        isShieldUp = false;
        playerControl.isInvincible = false;
        ShieldAnimator.gameObject.SetActive(false);

        yield return null;
    }


    public IEnumerator ShieldHit()
    {
        Debug.Log("Shield Was Hit");
        ShieldAnimator.Play("ShieldHit", 0);
        do
        {
            yield return null;
        } while (ShieldAnimator.GetCurrentAnimatorStateInfo(0).IsName("ShieldHit"));
        
        currentHitBlocked++;
    }
}
