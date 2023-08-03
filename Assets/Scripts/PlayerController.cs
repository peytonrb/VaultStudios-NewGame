using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Basic Movement")]
    public CharacterController controller;
    public float speed;
    public float gravity = -9.81f;
    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;
    private Vector3 velocity;
    private bool isGrounded;

    [Header("Lover")]
    private GameObject loveInterest; // stores the GameObject of the love interest
    private PlayerListUI chosenInterest;
    private string interestName;
    public ChemistController chemist;
    public ForestcoreController forestcore;
    public GamerController gamer;
    public GrilldadController grilldad;
    public JojoController jojo;
    public OccultController occult;
    private bool isDateTime;
    private bool isSocialTime;
    private bool isMurderTime;
    private bool isIngredientTime;
    private int dateCount;

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

    [Header("Murder")]
    private string[] targets = new string[3];
    private string[] murderItems1 = new string[3];
    private string[] murderItems2 = new string[3];
    private string[] murderItems3 = new string[3];
    private GameObject target1;
    private GameObject target2;
    private GameObject target3;

    [Header("Other")]
    public GameObject inventory;
    public InventoryManager inventoryManager;

    /*
        i am writing this for any other programmers that may come across this program... to help understand, 
        targets[] and all the string[] murderItems variables are all associated. for example, targets[0] will
        ALWAYS CORRELATED TO murderItems1[]. targets[1] is always correlated to murderItems2[], etc.
    */

    // also i am like 99.9% sure theres a better way to do all of this but i have never done this type of 
    // gameplay coding before i am doing my best

    void Start()
    {
        isDateTime = true;
        isSocialTime = false;
        isMurderTime = false;
        isIngredientTime = false;
        dateCount = 0;
        interestName = PlayerListUI.loveInterest;
        loveInterest = GameObject.Find(interestName);
        // there will be a bug here if you do not start the test game from the PlayerCards scene
        loveInterest.gameObject.tag = "LoveInterest"; 
        assignMurders(interestName);

        target1 = GameObject.Find(targets[0]);
        target1.gameObject.tag = "Target";

        target2 = GameObject.Find(targets[1]);
        target2.gameObject.tag = "Target";              // <--- make sure these aren't null!!

        target3 = GameObject.Find(targets[2]);
        target3.gameObject.tag = "Target";

        // set active calls
        inventory.SetActive(false);
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
    }

    void Update()
    {
        inventoryContainsItems();

        // movement mechanics
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;
        controller.Move(move * speed * Time.deltaTime);

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
        // end movement mechanics

        // key presses
        if (Input.GetKeyDown(KeyCode.E))
        {
            inventory.SetActive(true);
            inventoryManager.listItems();
        }

        // character interaction key 
        if (Input.GetKeyDown(KeyCode.F))
        {
            if (isDateTime)
            {
                Collider[] hitColliders = Physics.OverlapSphere(gameObject.transform.position, 5f); // second number is radius
                foreach (var hitCollider in hitColliders)
                {
                    if (hitCollider.gameObject.tag == "LoveInterest")
                    {
                        if (interestName == "forestcore")
                        {
                            forestcoreDate.SetActive(true);             // date dialogue --> in love interest controllers
                            forestcoreSprite.SetActive(true);           // gifts during dates...
                        }
                        else if (interestName == "grilldad")
                        {
                            grilldadDate.SetActive(true);
                            grilldadSprite.SetActive(true);
                        }
                        else if (interestName == "chemist")
                        {
                            chemistDate.SetActive(true);
                            chemistSprite.SetActive(true);
                        }
                        else if (interestName == "gamer")
                        {
                            gamerDate.SetActive(true);
                            gamerSprite.SetActive(true);
                        }
                        else if (interestName == "jojo")
                        {
                            jojoDate.SetActive(true);
                            jojoSprite.SetActive(true);
                        }
                        else if (interestName == "occult")
                        {
                            occultDate.SetActive(true);
                            occultSprite.SetActive(true);
                        }

                        dateCount++;
                        isSocialTime = true;
                        isDateTime = false;
                    }
                    else
                    {
                        // text bubble from npc -- stored in npc controllers
                    }
                }
            }

            if (isSocialTime)
            {
                Collider[] hitColliders = Physics.OverlapSphere(gameObject.transform.position, 5f); // second number is radius
                foreach (var hitCollider in hitColliders)
                {
                    if (hitCollider.gameObject.tag == "Target") // this could break 
                    {
                       // initiate friend date
                    }
                    else
                    {
                        // text bubble from npc
                    }

                    isSocialTime = false;
                    isIngredientTime = true;
                    StartCoroutine(ingredientTimer());
                }
            }

            if (isIngredientTime)
            {
                // ingredient search mechanics
            }

            if (isMurderTime)
            {
                isIngredientTime = false;
                // guarantees player is within range of love interest AND player has items required
                Collider[] hitColliders = Physics.OverlapSphere(gameObject.transform.position, 5f); // second number is radius
                foreach (var hitCollider in hitColliders)
                {
                    if (hitCollider.gameObject.tag == "Target") // this is going to break --> maybe???? 
                    {
                        if (forestcore.playerHasItems)
                        {
                            forestcore.initiateMurderGame();
                        }
                        else if (grilldad.playerHasItems)
                        {
                            grilldad.initiateMurderGame();
                        }
                        else if (chemist.playerHasItems)
                        {   
                            chemist.initiateMurderGame();
                        }
                        else if (jojo.playerHasItems)
                        {
                            jojo.initiateMurderGame();
                        }
                        else if (gamer.playerHasItems)
                        {
                            gamer.initiateMurderGame();
                        }
                        else if (occult.playerHasItems)
                        {
                            occult.initiateMurderGame();
                        }

                        isMurderTime = false;
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
    }

    IEnumerator ingredientTimer()
    {
        yield return new WaitForSeconds(180);
        isMurderTime = true;
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
}
