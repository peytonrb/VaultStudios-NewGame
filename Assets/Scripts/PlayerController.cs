using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Basic Movement")]
    public Rigidbody controller;
    public float speed;
    public float gravity = -9.81f;
    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;
    private Vector3 velocity;
    private bool isGrounded;

    [Header("Lover")]
    public ChemistController chemist;
    public ForestcoreController forestcore;
    public GamerController gamer;
    public GrilldadController grilldad;
    public JojoController jojo;
    public OccultController occult;
    public bool isDateTime;
    public bool isSocialTime;
    public bool isMurderTime;
    public bool isIngredientTime;
    private GameObject loveInterest; // stores the GameObject of the love interest
    private PlayerListUI chosenInterest;
    private string interestName;    
    private int dateCount;
    private int bodyCount;  // if player wins minigame --> increase body count

    [Header("UI")]
    public GameObject forestcoreDate;
    public GameObject grilldadDate;
    public GameObject chemistDate;
    public GameObject gamerDate;
    public GameObject jojoDate;
    public GameObject occultDate;
    public GameObject forestcoreSprite;
    public GameObject grilldadSprite;
    public GameObject chemistSprite;
    public GameObject gamerSprite;
    public GameObject jojoSprite;
    public GameObject occultSprite;
    public GameObject forestcoreShop;
    public GameObject grilldadShop;
    public GameObject chemistPharmacy;
    public GameObject gamerHouse;
    public GameObject jojoShop;
    public GameObject occultGrandmaHouse;
    public GameObject fcDialogueBox;
    public GameObject gdDialogueBox;
    public GameObject cDialogueBox;
    public GameObject gDialogueBox;
    public GameObject jDialogueBox;
    public GameObject oDialogueBox;
    public GameObject pauseMenu;
    private bool pauseInactive;
    public GameObject minigameMainMenu;
    public MinigameMenuUI minigameMenuUI;

    [Header("Dialogue")]
    public DialogueTriggerFC forestcoreDialogue;
    public DialogueTriggerGD grilldadDialogue;
    public DialogueTriggerC chemistDialogue;
    public DialogueTriggerG gamerDialogue;
    public DialogueTriggerJJ jojoDialogue;
    public DialogueTriggerO occultDialogue;
    public DialogueManager dialogueManager;

    [Header("Murder")]
    private string[] targets = new string[3];
    private string[] murderItems1 = new string[3];
    private string[] murderItems2 = new string[3];
    private string[] murderItems3 = new string[3];
    private GameObject target1;
    private GameObject target2;
    private GameObject target3;
    // public GameObject target1House;
    // public GameObject target2House;
    // public GameObject target3House;

    [Header("Other")]
    public GameObject inventory;
    public InventoryManager inventoryManager;
    public Material skybox;
    public Material skyboxNight;
    private static readonly int rotation = Shader.PropertyToID("_Rotation");
    private static readonly int exposure = Shader.PropertyToID("_Exposure");
    private float elapsedTime = 0f;
    private float timeScale = 2.5f;

    // regretting not using inheritance but it has been too long to go back

    void Start()
    {
        isDateTime = true;
        isSocialTime = false;
        isMurderTime = false;
        isIngredientTime = false;
        pauseInactive = true;
        dateCount = 0;
        bodyCount = 0;
        interestName = PlayerListUI.loveInterest;
        loveInterest = GameObject.Find(interestName);
        // there will be a bug here if you do not start the test game from the PlayerCards scene
        loveInterest.gameObject.tag = "LoveInterest"; 
        assignMurders(interestName);

        target1 = GameObject.Find(targets[0]);
        target1.gameObject.tag = "Target";

        target2 = GameObject.Find(targets[1]);
        target2.gameObject.tag = "Target";

        target3 = GameObject.Find(targets[2]);
        target3.gameObject.tag = "Target";

        // set active calls
        inventory.SetActive(false);
        setEverythingInactive();
        StartCoroutine(startingBuffer()); // the game does not allow u to begin dating until 4 minutes of gameplay have passed
    }

    void Update()
    {
        inventoryContainsItems();
        skyboxController();

        // movement mechanics
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 tempVect = new Vector3(x, 0f, z);
        tempVect = tempVect.normalized * speed * Time.deltaTime;
        controller.MovePosition(transform.position + tempVect);

        // key presses
        if (Input.GetKeyDown(KeyCode.E))
        {
            inventory.SetActive(true);
            inventoryManager.listItems();
        }

        if (Input.GetKeyDown(KeyCode.Return) && dialogueManager.isActive)
        {
            dialogueManager.DisplayNextSentence();
        }

        if (Input.GetKeyDown(KeyCode.Escape) && !inventory.activeSelf && pauseInactive)
        {
            pauseMenu.SetActive(true);
            pauseInactive = false;
        }

        if (Input.GetKeyDown(KeyCode.Escape) && !inventory.activeSelf && !pauseInactive)
        {
            pauseMenu.SetActive(false);
            pauseInactive = true;
        }

        // character interaction key 
        if (Input.GetKeyDown(KeyCode.F))
        {
            if (isDateTime)
            {
                RenderSettings.skybox = skybox;
                isMurderTime = false;
                Collider[] hitColliders = Physics.OverlapSphere(gameObject.transform.position, 5f); // second number is radius
                foreach (var hitCollider in hitColliders)
                {
                    if (hitCollider.gameObject.tag == "LoveInterest")
                    {
                        if (interestName == "forestcore")
                        {
                            forestcoreDate.SetActive(true);             
                            forestcoreSprite.SetActive(true);    
                            fcDialogueBox.SetActive(true);       // gifts during dates...?
                            forestcoreDialogue.dialogueStart();
                        }
                        else if (interestName == "grilldad")
                        {
                            grilldadDate.SetActive(true);
                            grilldadSprite.SetActive(true);
                            gdDialogueBox.SetActive(true);    
                            grilldadDialogue.dialogueStart();
                        }
                        else if (interestName == "chemist")
                        {
                            chemistDate.SetActive(true);
                            chemistSprite.SetActive(true);
                            cDialogueBox.SetActive(true);  
                            chemistDialogue.dialogueStart();
                        }
                        else if (interestName == "gamer")
                        {
                            gamerDate.SetActive(true);
                            gamerSprite.SetActive(true);
                            gDialogueBox.SetActive(true);  
                            gamerDialogue.dialogueStart();
                        }
                        else if (interestName == "jojo")
                        {
                            jojoDate.SetActive(true);
                            jojoSprite.SetActive(true);
                            jDialogueBox.SetActive(true); 
                            jojoDialogue.dialogueStart();
                        }
                        else if (interestName == "occult")
                        {
                            occultDate.SetActive(true);
                            occultSprite.SetActive(true);
                            oDialogueBox.SetActive(true); 
                            occultDialogue.dialogueStart();
                        }

                        dateCount++;
                        isSocialTime = true;

                        // if no dialogue is being played, stop the interaction
                        if (!dialogueManager.isActive)
                        {
                            setEverythingInactive();
                        }
                    }
                }
            }

            if (isSocialTime)
            {
                isDateTime = false;
                Collider[] hitColliders = Physics.OverlapSphere(gameObject.transform.position, 5f); // second number is radius
                foreach (var hitCollider in hitColliders)
                {
                    if (hitCollider.gameObject.tag == "Target")  
                    {
                        if (forestcore.isKillable)
                        {
                            forestcoreShop.SetActive(false);
                            forestcoreSprite.SetActive(true);
                            fcDialogueBox.SetActive(true);    
                            forestcoreDialogue.dialogueStart();
                        }
                        else if (grilldad.isKillable)
                        {
                            grilldadShop.SetActive(false);
                            grilldadSprite.SetActive(true);
                            gdDialogueBox.SetActive(true);  
                            grilldadDialogue.dialogueStart();
                        }
                        else if (chemist.isKillable)
                        {
                            chemistPharmacy.SetActive(false);
                            chemistSprite.SetActive(true);
                            cDialogueBox.SetActive(true);  
                            chemistDialogue.dialogueStart();
                        }
                        else if (gamer.isKillable)
                        {
                            gamerHouse.SetActive(false);
                            gamerSprite.SetActive(true);
                            gDialogueBox.SetActive(true);  
                            gamerDialogue.dialogueStart();
                        }
                        else if (jojo.isKillable)
                        {
                            jojoShop.SetActive(false);
                            jojoSprite.SetActive(true);
                            jDialogueBox.SetActive(true);  
                            jojoDialogue.dialogueStart();
                        }
                        else if (occult.isKillable)
                        {
                            occultGrandmaHouse.SetActive(false);
                            occultSprite.SetActive(true);
                            oDialogueBox.SetActive(true); 
                            occultDialogue.dialogueStart();
                        }
                    }

                    isIngredientTime = true;

                    // if no dialogue is being played, stop the interaction
                    if (!dialogueManager.isActive)
                    {
                        setEverythingInactive();
                        StartCoroutine(ingredientTimer());
                    }
                }
            }

            if (isIngredientTime)
            {
                // ingredient search mechanics - if any?
                isSocialTime = false;
            }

            if (isMurderTime)
            {
                RenderSettings.skybox = skyboxNight;
                
                // guarantees player is within range of love interest AND player has items required
                Collider[] hitColliders = Physics.OverlapSphere(gameObject.transform.position, 5f); // second number is radius
                foreach (var hitCollider in hitColliders)
                {
                    if (hitCollider.gameObject.tag == "Target") // should be their house but this is not my problem rn
                    {
                        if (forestcore.playerHasItems)
                        {
                            minigameMainMenu.SetActive(true); 
                            minigameMenuUI.nameOfTarget = "aspen";
                        }
                        else if (grilldad.playerHasItems)
                        {
                            minigameMainMenu.SetActive(true); 
                            minigameMenuUI.nameOfTarget = "davey";
                        }
                        else if (chemist.playerHasItems)
                        {   
                            minigameMainMenu.SetActive(true);
                            minigameMenuUI.nameOfTarget = "wesley";
                        }
                        else if (jojo.playerHasItems)
                        {
                            minigameMainMenu.SetActive(true); 
                            minigameMenuUI.nameOfTarget = "armani";
                        }
                        else if (gamer.playerHasItems)
                        {
                            minigameMainMenu.SetActive(true); 
                            minigameMenuUI.nameOfTarget = "carmen";
                        }
                        else if (occult.playerHasItems)
                        {
                            minigameMainMenu.SetActive(true); 
                            minigameMenuUI.nameOfTarget = "kai";
                        }

                        isDateTime = true;
                    }
                }
            }
        }

        if (inventory.activeSelf && Input.GetKeyDown(KeyCode.Escape))
        {
            inventory.SetActive(false);
        }
        // end key presses

        if (bodyCount == 3)
        {
            //game over --> win screen
        }
    }

    IEnumerator ingredientTimer()
    {
        yield return new WaitForSeconds(180);
        isMurderTime = true;
    }

    public void skyboxController()
    {
        skybox.SetFloat(rotation, elapsedTime * timeScale);
    }

    // do not change unless bugs
    private void inventoryContainsItems()
    {
        int counter1 = 0;
        int counter2 = 0;
        int counter3 = 0;

        // checks if player has required items
        for (int i = 0; i < murderItems1.Length; i++)
        {
            if (inventoryManager.contains(murderItems1[i]))
            {
                counter1++;
            }
        }

        for (int i = 0; i < murderItems2.Length; i++)
        {
            if (inventoryManager.contains(murderItems2[i]))
            {
                counter2++;
            }
        }

        for (int i = 0; i < murderItems3.Length; i++)
        {
            if (inventoryManager.contains(murderItems3[i]))
            {
                counter3++;
            }
        }

        // makes sure player has ALL required items
        if (counter1 == 3)
        {
            setToKill(1);
        }
        else if (counter2 == 3)
        {
            setToKill(2);
        }
        else if (counter3 == 3)
        {
            setToKill(3);
        }
    }

    // do not change unless there is bug
    private void setToKill(int index)
    {
        if (targets[index] == "forestcore")
        {
            forestcore.playerHasItems = true;
        }
        else if (targets[index] == "grilldad")
        {
            grilldad.playerHasItems = true;
        }
        else if (targets[index] == "chemist")
        {
            chemist.playerHasItems = true;
        }
        else if (targets[index] == "gamer")
        {
            gamer.playerHasItems = true;
        }
        else if (targets[index] == "jojo")
        {
            jojo.playerHasItems = true;
        }
        else if (targets[index] == "occult")
        {
            occult.playerHasItems = true;
        }
    }

    // assigns the murder targets to the variables - DO NOT CHANGE
    private void assignMurders(string interestName)
    {
        int iteration = 0;

        if (interestName == "forestcore")
        {
            // assigning the names to the variable local to this script
            for (int i = 0; i < 3; i++)
            {
                targets[i] = forestcore.friends[i];
            }

            // assigning the items to the variable local to this script
            foreach (string name in targets)
            {
                iteration++;
                assignMurderItems(name, iteration);
            }
        } else if (interestName == "grilldad")
        {
            // assigning the names to the variable local to this script
            for (int i = 0; i < 3; i++)
            {
                targets[i] = grilldad.friends[i];
            }

            // assigning the items to the variable local to this script
            foreach (string name in targets)
            {
                iteration++;
                assignMurderItems(name, iteration);
            }
        } else if (interestName == "chemist")
        {
            // assigning the names to the variable local to this script
            for (int i = 0; i < 3; i++)
            {
                targets[i] = chemist.friends[i];
            }

            // assigning the items to the variable local to this script
            foreach (string name in targets)
            {
                iteration++;
                assignMurderItems(name, iteration);
            }
        } else if (interestName == "gamer")
        {
            // assigning the names to the variable local to this script
            for (int i = 0; i < 3; i++)
            {
                targets[i] = gamer.friends[i];
            }

            // assigning the items to the variable local to this script
            foreach (string name in targets)
            {
                iteration++;
                assignMurderItems(name, iteration);
            }
        } else if (interestName == "occult")
        {
            // assigning the names to the variable local to this script
            for (int i = 0; i < 3; i++)
            {
                targets[i] = occult.friends[i];
            }

            // assigning the items to the variable local to this script
            foreach (string name in targets)
            {
                iteration++;
                assignMurderItems(name, iteration);
            }
        } else if (interestName == "jojo")
        {
            // assigning the names to the variable local to this script
            for (int i = 0; i < 3; i++)
            {
                targets[i] = jojo.friends[i];
            }

            // assigning the items to the variable local to this script
            foreach (string name in targets)
            {
                iteration++;
                assignMurderItems(name, iteration);
            }
        }
    }

    // assigns the items required to kill the targets to the variables -- DO NOT CHANGE
    private void assignMurderItems(string name, int iteration)
    {
        if (name == "forestcore")
        {
            if (iteration == 1)
            {
                for (int i = 0; i < 3; i++)
                {
                    murderItems1[i] = forestcore.requiredItems[i];
                }
            } else if (iteration == 2)
            {
                for (int i = 0; i < 3; i++)
                {
                    murderItems2[i] = forestcore.requiredItems[i];
                }
            } else if (iteration == 3)
            {
                for (int i = 0; i < 3; i++)
                {
                    murderItems3[i] = forestcore.requiredItems[i];
                }
            }
        } else if (name == "grilldad")
        {
            if (iteration == 1)
            {
                for (int i = 0; i < 3; i++)
                {
                    murderItems1[i] = grilldad.requiredItems[i];
                }
            } else if (iteration == 2)
            {
                for (int i = 0; i < 3; i++)
                {
                    murderItems2[i] = grilldad.requiredItems[i];
                }
            } else if (iteration == 3)
            {
                for (int i = 0; i < 3; i++)
                {
                    murderItems3[i] = grilldad.requiredItems[i];
                }
            }
        } else if (name == "chemist")
        {
            if (iteration == 1)
            {
                for (int i = 0; i < 3; i++)
                {
                    murderItems1[i] = chemist.requiredItems[i];
                }
            } else if (iteration == 2)
            {
                for (int i = 0; i < 3; i++)
                {
                    murderItems2[i] = chemist.requiredItems[i];
                }
            } else if (iteration == 3)
            {
                for (int i = 0; i < 3; i++)
                {
                    murderItems3[i] = chemist.requiredItems[i];
                }
            }
        } else if (name == "gamer")
        {
            if (iteration == 1)
            {
                for (int i = 0; i < 3; i++)
                {
                    murderItems1[i] = gamer.requiredItems[i];
                }
            } else if (iteration == 2)
            {
                for (int i = 0; i < 3; i++)
                {
                    murderItems2[i] = gamer.requiredItems[i];
                }
            } else if (iteration == 3)
            {
                for (int i = 0; i < 3; i++)
                {
                    murderItems3[i] = gamer.requiredItems[i];
                }
            }
        } else if (name == "occult")
        {
            if (iteration == 1)
            {
                for (int i = 0; i < 3; i++)
                {
                    murderItems1[i] = occult.requiredItems[i];
                }
            } else if (iteration == 2)
            {
                for (int i = 0; i < 3; i++)
                {
                    murderItems2[i] = occult.requiredItems[i];
                }
            } else if (iteration == 3)
            {
                for (int i = 0; i < 3; i++)
                {
                    murderItems3[i] = occult.requiredItems[i];
                }
            }
        } else if (name == "jojo")
        {
            if (iteration == 1)
            {
                for (int i = 0; i < 3; i++)
                {
                    murderItems1[i] = jojo.requiredItems[i];
                }
            } else if (iteration == 2)
            {
                for (int i = 0; i < 3; i++)
                {
                    murderItems2[i] = jojo.requiredItems[i];
                }
            } else if (iteration == 3)
            {
                for (int i = 0; i < 3; i++)
                {
                    murderItems3[i] = jojo.requiredItems[i];
                }
            }
        }
    }

    private void setEverythingInactive()
    {
        forestcoreDate.SetActive(false);
        grilldadDate.SetActive(false);
        chemistDate.SetActive(false);
        gamerDate.SetActive(false);
        jojoDate.SetActive(false);
        occultDate.SetActive(false);
        forestcoreSprite.SetActive(false);
        grilldadSprite.SetActive(false);
        chemistSprite.SetActive(false);
        gamerSprite.SetActive(false);
        jojoSprite.SetActive(false);
        occultSprite.SetActive(false);
        forestcoreShop.SetActive(false);
        grilldadShop.SetActive(false);
        chemistPharmacy.SetActive(false);
        gamerHouse.SetActive(false);
        jojoShop.SetActive(false);
        occultGrandmaHouse.SetActive(false);
        fcDialogueBox.SetActive(false);
        gdDialogueBox.SetActive(false);
        cDialogueBox.SetActive(false);
        gDialogueBox.SetActive(false);
        jDialogueBox.SetActive(false);
        oDialogueBox.SetActive(false);
        pauseMenu.SetActive(false);
        minigameMainMenu.SetActive(false);
    }

    IEnumerator startingBuffer()
    {
        yield return new WaitForSeconds(240);
        isDateTime = true;
    }
}
