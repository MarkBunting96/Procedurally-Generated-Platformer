//Purpose: Controls the UI elements of the load menu, death menu and various text elements.

//Default Unity includes:
using UnityEngine;
using System.Collections;

//My includes:
using UnityEngine.UI;                                                                                 //To include text.

//Class which contains an Awake function, and an Update function.
public class LoadMenuScript : MonoBehaviour
{

    public GameObject loadMenu;                                                                       //loadMenu object which will be activated when a new level is being loaded.
    public GameObject deadMenu;                                                                       //deadMenu object which will be activated when the player has died.
    public GameObject pickup;                                                                         //pickup object which will handle activation/deactivation of the pickup for various menus.

    public GameObject doubleJumpPickup;                                                               //Stores the doubleJumpPickup icon.
    public GameObject gunPickup;                                                                      //Stores the gunPickup icon.
    public GameObject shieldPickup;                                                                   //Stores the shieldPickup icon.

    public Text pickupText;                                                                           //Stores the text for the relevant pickup.
    public Text levelText;                                                                            //Stores the text for the level number.
    public Text levelsToBeatText;                                                                     //Stores the text for how many levels are left.
    public Text livesText;                                                                            //Stores the text for how many lives the player has.

    //Awake function which initialises the text elements (apart from pickupText).
    void Awake()
    {
        livesText.text = "Lives: " + GameManager.instance.lives;                                      //Stores the number of lives, from gameManaager, in the livesText.
        levelsToBeatText.text = "Levels to beat: " + (GameManager.instance.maxLevels                  //Stores how many levels are left, using values from the gameManager, in the levelsToBeat text.
                                                      - GameManager.instance.level + 1);
        levelText.text = "Level " + GameManager.instance.level;                                       //Stores the level number in levelText.
    }

    //Update function which checks which state the game is in to deactivate/reactivate menus
    //and text.
    void Update()
    {
        //If statement which checks if the level is being loaded, if the game isnt over,
        //and if there are levels still available to play, then activates/deactivates
        //the appopriate elements.
        if (GameManager.instance.loadingLevel && !GameManager.instance.gameOver && GameManager.instance.level <= GameManager.instance.maxLevels)
        {
            loadMenu.gameObject.SetActive(true);
            pickup.gameObject.SetActive(false);
            pickupText.gameObject.SetActive(false);
            levelText.gameObject.SetActive(false);
            levelsToBeatText.gameObject.SetActive(true);
        }
        //else if statement that checks if the level has been loaded and if the player is alive,
        //then activates/deactivate shte appropriate elements.
        else if (!(GameManager.instance.loadingLevel && GameManager.instance.playerDead))
        {
            loadMenu.gameObject.SetActive(false);
            pickup.gameObject.SetActive(true);
            pickupText.gameObject.SetActive(true);
            levelsToBeatText.gameObject.SetActive(false);
            levelText.gameObject.SetActive(true);
        }

        //if statement that checks if the player is dead and if the game is still going,
        //then activates/deactivates the appopriate elements.
        if (GameManager.instance.playerDead && !GameManager.instance.gameOver)
        {
            deadMenu.gameObject.SetActive(true);
            pickup.gameObject.SetActive(false);
            levelsToBeatText.gameObject.SetActive(true);
        }
        //else if statement that checks if the player is alive and the level has loaded,
        //then activates/deactivates the appopriate elements.
        else if (!(GameManager.instance.playerDead && GameManager.instance.loadingLevel))
        {
            deadMenu.gameObject.SetActive(false);
        }

        //If statements that check what pickup is active and sets the appropriate
        //pickup text and activates the appropriate gameObject.
        if (GameManager.instance.activePickup == GameManager.Pickup.DOUBLEJUMP)
        {
            doubleJumpPickup.gameObject.SetActive(true);
            pickupText.text = "DOUBLE JUMP ACTIVE";
        }

        //Same as above.
        if (GameManager.instance.activePickup == GameManager.Pickup.GUNPICKUP)
        {
            gunPickup.gameObject.SetActive(true);
            pickupText.text = "GUN ACTIVE, RIGHT CLICK TO FIRE";
        }

        //Same as above.
        if (GameManager.instance.activePickup == GameManager.Pickup.SHIELDPICKUP)
        {
            shieldPickup.gameObject.SetActive(true);
            pickupText.text = "SHIELD ACTIVE, RESISTANCE TO BULLETS FOR FIVE HITS";
        }

        //Same as above. 
        if (GameManager.instance.activePickup == GameManager.Pickup.NONE)
        {
            doubleJumpPickup.gameObject.SetActive(false);
            gunPickup.gameObject.SetActive(false);
            shieldPickup.gameObject.SetActive(false);
            pickupText.text = "";
        }

        //Constant updates to the Lives, Levels to beat and Level text.
        livesText.text = "Lives: " + GameManager.instance.lives;
        levelsToBeatText.text = "Levels to beat: " + (GameManager.instance.maxLevels - GameManager.instance.level + 1);
        levelText.text = "Level " + GameManager.instance.level;
    }    
}
