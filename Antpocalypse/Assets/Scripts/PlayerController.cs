using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public GameObject Joystick;
    public GameObject UltSlot;
    public Camera PlayerCamera;
    public GameObject HealthBar, ExpBar;
    public GameObject DeathCanvas;
    // public GameObject SkillAnimControl;
    public float playerInvincibilityTime = 0.5f;

    public float timeBetweenAutoAttacks = 0.3f;
    public int movementControlStyle = 1; // 0 = Magic Survival   1 - Follow Style

    PlayerModel playerModel;
    WorldModel worldModel;
    public bool isInvincible;

    public Vector3 playerDirection;

    public GameObject ElementalistSkills, ElementalistUlts;
    public GameObject CommandoSkills, CommandoUlts;

    // Start is called before the first frame update
    void Start()
    {
        playerModel = GetComponent<PlayerModel>();
        worldModel = GameObject.FindGameObjectWithTag("WORLD").GetComponent<WorldModel>();

        // Set the loaded class /////

        string chosenClass = MenuController.ChosenClass;

        //chosenClass = "Commando";  For testing purposes

        if (chosenClass.Equals("Elementalist"))
        {
            playerModel.SetPlayerClass(new ElementalistClass(playerModel));
            SkillTalentManager = ElementalistSkills;
            UltTalentManager = ElementalistUlts;
            
        }
        else if (chosenClass.Equals("Commando"))
        {
            playerModel.SetPlayerClass(new CommandoClass(playerModel));
            SkillTalentManager = CommandoSkills;
            UltTalentManager = CommandoUlts;
        }

        /////////////////////////////

        GameObject ultObjectResource = playerModel.GetPlayerClass().GetUltUIResource();
        GameObject ultUIObject = Instantiate(ultObjectResource, UltSlot.transform);
        playerModel.GetPlayerClass().SetUltUIObject(ultUIObject);

        ///////////// TESTING TESTING TESTING //////////////////
        ///
        playerModel.SetHealth(100);
        ApplyDamage(66);
    }


    private Vector2 joystickStartPos, joystickDirection;
    //private Vector3 jsStartPos, jsDirection;



    float elapsedAutoattackTime;
    float elapsedBetweenAA = float.MaxValue;
    int currentShot;



    private void FixedUpdate()
    {
        ////////////////////////////////////////
        // PC TESTING MOVEMENT
        joystickDirection = Vector3.zero;
        Vector3 dirPC = Vector3.zero;
        if (Input.GetKey(KeyCode.W))
            dirPC += Vector3.up;
        if (Input.GetKey(KeyCode.A))
            dirPC += Vector3.left;
        if (Input.GetKey(KeyCode.S))
            dirPC += Vector3.down;
        if (Input.GetKey(KeyCode.D))
            dirPC += Vector3.right;
        dirPC.Normalize();

        //transform.Translate();
        Rigidbody2D rigidbody = GetComponent<Rigidbody2D>();
        rigidbody.MovePosition(transform.position + dirPC * playerModel.movementSpeed * Time.fixedDeltaTime);



        ////////////////////
        ///
        /// TESTING FOR FIRE BREATH ////////////


        #region PlayerMovement

        
        if (Input.touchCount > 0)
        {
            Touch joystickTouch = Input.touches[0];

            

            if (joystickTouch.tapCount == 2) // Double Click
            {
                //worldModel.GetComponent<WorldController>().SpawnAmbientEnemy(this, 10, worldModel.Enemies[0]);


                Vector3 clickPosition = PlayerCamera.ScreenToWorldPoint(joystickTouch.position);

                playerModel.GetPlayerClass().DoubleClick(clickPosition);

            }
            else if (joystickTouch.tapCount == 1)
            {


                if (movementControlStyle == 0)
                {

                    Vector3 jsWorldDirection = new Vector3();
                    GameObject stick = Joystick.transform.GetChild(0).gameObject;
                    Vector3 jsWorldStartPos = new Vector3();



                    if (joystickTouch.phase == TouchPhase.Began)
                    {



                        Joystick.SetActive(true);
                        joystickStartPos = joystickTouch.position;

                        jsWorldStartPos = new Vector3(PlayerCamera.ScreenToWorldPoint(joystickStartPos).x, PlayerCamera.ScreenToWorldPoint(joystickStartPos).y, 0f);
                        Joystick.transform.position = jsWorldStartPos;
                        Debug.Log("Touch Started");



                    }
                    else if (joystickTouch.phase == TouchPhase.Moved)
                    {




                        joystickDirection = joystickTouch.position - joystickStartPos;


                        jsWorldDirection = PlayerCamera.ScreenToWorldPoint(new Vector3(joystickDirection.x, joystickDirection.y, 0f));

                        jsWorldStartPos = new Vector3(PlayerCamera.ScreenToWorldPoint(joystickStartPos).x, PlayerCamera.ScreenToWorldPoint(joystickStartPos).y, 0f);
                        joystickDirection.Normalize();
                        stick.transform.position = jsWorldStartPos + new Vector3(joystickDirection.x, joystickDirection.y, 0f) * 0.5f;

                        // Move Player
                        transform.Translate(joystickDirection * playerModel.movementSpeed * Time.fixedDeltaTime);




                    }
                    else if (joystickTouch.phase == TouchPhase.Ended)
                    {

                        Joystick.SetActive(false);
                        //  stick.transform.localPosition = Vector3.zero;
                        Debug.Log("Touch Ended");



                    }
                    else if (joystickTouch.phase == TouchPhase.Stationary)
                    {
                        transform.Translate(joystickDirection * playerModel.movementSpeed * Time.fixedDeltaTime);
                    }
                }

                Vector3 directionStyle1 = Vector3.zero;
                if (movementControlStyle == 1) // Follow Finger
                {
                    Vector3 fingerTarget;
                    Vector3 target;

                    target = PlayerCamera.ScreenToWorldPoint(joystickTouch.position);
                    directionStyle1 = target - transform.position;
                    directionStyle1.z = 0;
                    directionStyle1.Normalize();

                    //Move Player
                    transform.Translate(directionStyle1 * playerModel.movementSpeed * Time.fixedDeltaTime);


                }

                





            }


        }

        /////;        Edge Scroll
        Vector3 pScreenPos = PlayerCamera.WorldToScreenPoint(transform.position);
        float per = 0.4f;
        float height = PlayerCamera.scaledPixelHeight;
        float width = PlayerCamera.scaledPixelWidth;

        if (pScreenPos.x < width * per)
        {
            PlayerCamera.transform.Translate(Vector3.left * 0.15f);
        }
        if (pScreenPos.x > width - width * per)
        {
            PlayerCamera.transform.Translate(Vector3.right * 0.15f);
        }
        if (pScreenPos.y < height * per)
        {
            PlayerCamera.transform.Translate(Vector3.down * 0.15f);
        }
        if (pScreenPos.y > height - height * per)
        {
            PlayerCamera.transform.Translate(Vector3.up * 0.15f);
        }



        #endregion
        // This must be changed for android build to: joystickDirection.sqrMagnitude > 0
        if (dirPC.sqrMagnitude > 0)
            playerDirection = dirPC.normalized;


    }

    // Update is called once per frame
    void Update()
    {
        
       


        // HealthBar Management
        HealthBar.GetComponent<Image>().fillAmount = playerModel.GetHP() / playerModel.maxHP;
        ExpBar.GetComponent<Image>().fillAmount = playerModel.GetCurrentExp() / playerModel.GetExpForLvlUp(playerModel.GetPlayerLvl() + 1);


        ///////////////// TESTING /////////////////////////
        ///
        if (Input.GetKeyDown(KeyCode.L))
        {
            LevelUp();
        }
        if (Input.GetMouseButtonDown(1))
        {
            Vector3 pressedLocation = PlayerCamera.ScreenToWorldPoint(Input.mousePosition);
            Vector2 pressedLoc = new Vector2(pressedLocation.x, pressedLocation.y);

            StartCoroutine(playerModel.GetPlayerClass().DoubleClick(pressedLoc));

        }
        /////////////////////////////////////////////


        #region AutoAttack

        elapsedAutoattackTime += Time.deltaTime;
        if (elapsedAutoattackTime >= playerModel.autoattackCooldown)
        {
            GameObject target = worldModel.GetComponent<WorldController>().GetClosestEnemy(playerModel, PlayerCamera, 15f);   // RANGE HERE
            if (target != null)
            {

                StartCoroutine(AutoAttack(target.transform.position));
            }
            elapsedAutoattackTime = 0;
        }

        #endregion

    }
    IEnumerator AutoAttack(Vector3 target)
    {

        for (int i = 0; i < playerModel.autoAttackNumber; i++)
        {
            playerModel.GetPlayerClass().AutoAttack(target);
            yield return new WaitForSeconds(0.3f);
        }
        yield return null;
    }

    public GameObject talentPanel;

    // Every X levels one/two/all? Class Talent Skill should be chosen and presented for Selection

    public GameObject generalTalentManager; // GeneralTalentManager should be loaded from Resources
    private GameObject SkillTalentManager; // Skill Talent List should be loaded from Resources according to the Player Class
    private GameObject UltTalentManager;

    Talent[] chosenTalents;

    public void ShowTalentOptions()
    {
        talentPanel.transform.parent.gameObject.SetActive(true);
        Time.timeScale = 0f;


        int n = 3;  // Number of talents

        if (playerModel.GetPlayerLvl() % playerModel.GetPlayerClass().skillTalentFrequency == 0) // SkillTalent should be presented
        {
            Talent[] ult = UltTalentManager.GetComponent<TalentManager>().GetChosenTalents(1);
            Talent[] skill = SkillTalentManager.GetComponent<TalentManager>().GetChosenTalents(3);

            

            chosenTalents[0] = skill[0];
            chosenTalents[1] = skill[1];

            if (ult.Length == 0)     // no more ult talents to choose from
                chosenTalents[2] = skill[2];
            else
                chosenTalents[2] = ult[0];
        }
        else // Only generalTalents should be presented to the player
        {
            chosenTalents = generalTalentManager.GetComponent<TalentManager>().GetChosenTalents(3);  // Return n talents from GeneralTalentList
        }

       

        for (int i = 0; i < n; i++)
        {
            GameObject choiceObject = talentPanel.transform.GetChild(i).gameObject;

            Text description = choiceObject.transform.GetChild(0).GetComponent<Text>();
            Text name = choiceObject.transform.GetChild(1).GetComponent<Text>();
            Text level = choiceObject.transform.GetChild(2).GetComponent<Text>();

            name.text = chosenTalents[i].Name;
            description.text = chosenTalents[i].Description;

            if(chosenTalents[i].GetCurrentTalentLevel() + 1 >= chosenTalents[i].maxLevel)
                level.text = "Max";
            else
                level.text = (chosenTalents[i].GetCurrentTalentLevel() + 1).ToString();




        }

    }

    public void TalentChosen(int talentIndex)
    {
        talentPanel.transform.parent.gameObject.SetActive(false);
        Time.timeScale = 1f;


        Talent chosenTalent = chosenTalents[talentIndex];

        
        List<Talent> talents = playerModel.GetEquippedTalents();
        // Check if talent is already chosen
        if (!talents.Contains(chosenTalent))
        {
            talents.Add(chosenTalent);
        }
       
        chosenTalent.LevelUp();
        

        //playerModel.GetEquippedTalents().Add(chosenTalent);


        //chosenTalent.ApplyTalent(playerModel);
        playerModel.CalculateStats();

        Debug.Log("You have chosen talent: " + chosenTalent.Name + " and stats have been recalculated.");
    }


    public void ApplyDamage(float dmg)
    {

        float trueDamage = dmg * (1f - playerModel.damageResistance);

        playerModel.health -= trueDamage;

        if (playerModel.health <= 0f)
        {
            // IMPLEMENT DEATH
            StartCoroutine(ShowDeathScreen());
        }

    }

    public void ApplyExp(float exp)
    {
        playerModel.currentExp += (exp * playerModel.EXPmodifier);
        if (playerModel.currentExp >= playerModel.GetExpForLvlUp(playerModel.playerLevel))
        {
            playerModel.currentExp = 0;
            LevelUp();
        }
    }
    public void ApplyHealth(float hp)
    {
        playerModel.health += hp;
        if (playerModel.health > playerModel.maxHP)
        {
            playerModel.health = playerModel.maxHP;
        }
    }

    public void LevelUp()
    {
        playerModel.playerLevel += 1;
        ShowTalentOptions();
        Debug.Log("Leveled Up - GJ");
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        Enemy enemy = collision.gameObject.GetComponent<Enemy>();
        if (enemy != null && !isInvincible) // Enemy has hit the player
        {
            ApplyDamage(enemy.playerDamage);
            StartCoroutine(OnHitInvincibility());
        }
    }
    private IEnumerator OnHitInvincibility()
    {
        isInvincible = true;
        yield return new WaitForSeconds(playerInvincibilityTime);
        isInvincible = false;
    }

    private IEnumerator ShowDeathScreen()
    {
        Time.timeScale = 0;
        float fadeTimeYouDied = 3f;
        float fadeTimeStats = 1f;
        float X = 100;

        // First YouDied fades in, then player stats fade in, and then the buttons appear
        TextMeshProUGUI YouDiedText = DeathCanvas.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI StatsText = DeathCanvas.transform.GetChild(2).GetComponent<TextMeshProUGUI>();
        GameObject buttonPanel = DeathCanvas.transform.GetChild(3).gameObject;

        YouDiedText.alpha = 0;
        StatsText.alpha = 0;
        
        buttonPanel.SetActive(false);

        Vector3 cameraPos = new Vector3(transform.position.x, transform.position.y, PlayerCamera.transform.position.z);
        PlayerCamera.transform.position = cameraPos;

        DeathCanvas.SetActive(true);

        /// Fade in X steps
        float fadeStep = fadeTimeYouDied / X;
        float Xs = 1f / X;
        for (float i = 0f; i <= 1f; i += Xs)
        {
            YouDiedText.alpha = i;
            yield return new WaitForSecondsRealtime(fadeStep);
        }
        fadeStep = fadeTimeStats / X;
        for (float i = 0f; i <= 1f; i += Xs)
        {
            StatsText.alpha = i;
            yield return new WaitForSecondsRealtime(fadeStep);
        }

        buttonPanel.SetActive(true);
        yield return null;
    }

}
