//Purpose: The main code for this game. Contains lots of public variables  and functions 
//         which other objects within my game can access and use. 
//         Is a singleton so that only one instance of the game manager can exist at a time.
//         Controls the creation of the level generator script, instantiating the player and
//         the player children, and the camera.

//Default Unity includes:
using UnityEngine;
using System.Collections;

//My includes:
using UnityEngine.SceneManagement;                                                                    //To check and control scenes.

//class which contains an awake, initGame, loadPlatforms, loadPlayer, loadCamera
//getPlayerPosition, resetPlayer, resetGame, setActivePickup, decreaseShieldHealth
//Update and loadByIndex function.
public class GameManager : MonoBehaviour
{
    public static GameManager instance = null;                                                        //an instance of the game manager is initialised to NULL.

    public enum Pickup { DOUBLEJUMP, GUNPICKUP, SHIELDPICKUP, NONE };                                 //enum Pickup, contains all the pickups in the game.
        
    public Pickup activePickup;                                                                       //active pickup, which pickup is the active one.

    public int level = 1;                                                                             //the current level the player is on, initialised to 1.
    public int maxLevels = 10;                                                                        //maxLevels, the total number of levels the player needs to complete, initialised to 10.
    public int lives;                                                                                 //how many lives the player has.
    public int shieldHealth = 5;                                                                      //the health of the shield, initialised to 5.

    public bool levelStarted = false;                                                                 //boolean to declare if the level has started, initialised to false.
    public bool levelFinished = false;                                                                //boolean to declare if the level is finished, initialised to false.
    public bool loadingLevel = false;                                                                 //boolean to declare if the level is being loaded, initialised to false.
    public bool playerDead = false;                                                                   //boolean to declare if the player is dead, initiaised to false.
    public bool gameOver;                                                                             //boolean to declare if the game is over.
        
    public SpawnPlatforms levelGenerator;                                                             //the spawnplatforms script, used to spawn generate the levels.

    public Camera mainCam;                                                                            //main camera to be spawned.

    public PlayerController thePlayer;                                                                //thePlayer to be spawned

    public GameObject playerGun;                                                                      //playerGun, to be set to the players gun (child of player).
    public GameObject playerShield;                                                                   //same as above but for the shield.

    //Awake function which makes sure only on gameManager exists, sets
    //the manager to dontdestroyonload, and initialises some variables.
    void Awake()
    {
        //if statement that checks if instance is null, then 
        //sets the instance to this object.
        if (instance == null)
        {
            instance = this;
        }
        //else if statement that checks if the instance is not
        //this object, then destroys it.
        else if (instance != this)
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);                                                                //Sets this object to not be destroyed when a new scene is loaded.

        levelGenerator = GetComponent<SpawnPlatforms>();                                              //Sets the level generator to the spawnPlatforms component attached to this object.

        levelGenerator.enabled = false;                                                               //level generator is initialised to false, enabled when the right scene is loaded.
        activePickup = Pickup.NONE;                                                                   //active pickup is initialised to none.
        lives = 15;                                                                                   //lives is initialised to 15.
        level = 1;                                                                                    //level is initialised to 1.
        gameOver = false;                                                                             //gameOver is initialised to false.
        shieldHealth = 5;                                                                             //shieldhealth initialised to 5.
        maxLevels = 10;                                                                               //maxLevels initialised to 10.
    }

    //InitGame function, reinitialises the variables as awake is only called when
    //the game manager is first created, init game will be called everytime the
    //game scene is loaded.
    void InitGame()
    {
        levelGenerator.difficultyScale += level * 0.075f;                                             //difficultyScale in the levelGenerator is assigned using the level and a modifier.
        shieldHealth = 5;                                                                             //shieldHealth is reinitialised to 5.
        activePickup = Pickup.NONE;                                                                   //activePickup is reinitialised to none.
        LoadPlayer();                                                                                 //Load player is called.
        playerGun = GameObject.FindWithTag("PlayerGun");                                              //playerGun is assigned after the player has been loaded.
        playerGun.SetActive(false);                                                                   //gun is deactivated.
        playerShield = GameObject.FindWithTag("PlayerShield");                                        //playerShield is assigned after the player ha been loaded.
        playerShield.SetActive(false);                                                                //shield is deactivated.
        loadingLevel = false;                                                                         //loading level is assigned to false.
        playerDead = false;                                                                           //player dead is assigned to false.
        LoadCamera();                                                                                 //the camera is loaded.
        LoadPlatforms();                                                                              //the platforms are loaded.
    }

    //loadPlatforms function, re-enables the level generator and calls
    //initplatforms and initial platforms from the level generator.
    void LoadPlatforms()
    {
        levelGenerator.enabled = true;
        levelGenerator.InitPlatforms();
        levelGenerator.InitialPlatform();
    }

    //LoadPlayer is used to instantiate the player.
    void LoadPlayer()
    {
        Instantiate(thePlayer);
    }

    //LoadCamera is used to instantiate the main camera.
    void LoadCamera()
    {
        Instantiate(mainCam);
    }

    //GetPlayerPosition finds the player in the game and returns the transform.
    public Transform GetPlayerPosition()
    {
        Transform playerPos;

        playerPos = GameObject.FindWithTag("Player").transform;

        return playerPos;
    }

    //ResetPlayer first checks if the player still has lives, then decrements the amount of lives, 
    //lowers the difficultyScale, then checks if the player has ran out of lives, if so,
    //calls loadByIndex to load the gameOver scene, if not, sets player dead to true,
    //sets the players position back to the origin, zeroes the velocity, and calls
    //an invoke to reset game after 5 seconds.
    public void ResetPlayer()
    {
        if (lives > 0)
        {
            lives--;
            levelGenerator.difficultyScale -= 0.15f;

            if (lives <= 0)
            {
                LoadByIndex(2);
            }
            else
            {
                playerDead = true;
                GameObject.FindWithTag("Player").transform.position = new Vector3(0, 0, 0);
                GameObject.FindWithTag("Player").GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);

                Invoke("ResetGame", 5f);
            }
        }
    }

    //ResetGame function, checks if there is an active pickup, then sets it to none,
    //sets the pickup in level generator to active, and then deactivates the gun and shield.
    //It then sets pickup spawned in the level generator to false, then uses a for loop to
    //reactivate all the enemies in the level, then another one for the movingTurrets
    //then sets the player to alive.
    void ResetGame()
    {
        if (activePickup != Pickup.NONE)
        {
            activePickup = Pickup.NONE;
            levelGenerator.pickups[0].SetActive(true);
            playerGun.SetActive(false);
            playerShield.SetActive(false);
        }
        levelGenerator.pickupSpawned = false;
        for(int i = 0; i < levelGenerator.enemyCount; i++)
        {
            levelGenerator.enemies[i].SetActive(true);
        }

        for (int i = 0; i < levelGenerator.movingTurretCount; i++)
        {
            levelGenerator.movingTurrets[i].SetActive(true);
        }
        playerDead = false;
    }

    //SetActive pickup takes a pickup, then sets active pickup to set
    //and changes some variables depending on the pickup.
    public void SetActivePickup(Pickup set)
    {
        activePickup = set;

        //if the pickup is the gun, the gun obhect is set to active, else, gun is deactive.
        if (set == Pickup.GUNPICKUP)
        {
            playerGun.SetActive(true);
        }
        else
        {
            playerGun.SetActive(false);
        }

        //if pickup is shield, set shield to active, and the shield health to 5, else, shield deactive.
        if (set == Pickup.SHIELDPICKUP)
        {
            playerShield.SetActive(true);
            shieldHealth = 5;
        }
        else
        {
            playerShield.SetActive(false);
        }
    }

    //Decrease shield health decrements shield health, checks if life is <=0, and deactivates if so.
    public void decreaseShieldHealth()
    {
        shieldHealth--;

        if (shieldHealth <= 0)
        {
            playerShield.SetActive(false);
        }
    }


    //Update function handles what happens during what scene using the scene manager.
    void Update ()
    {
        //if active scene index =1 and level not started and not finished, level started
        //set to true, initgame called.
        if (SceneManager.GetActiveScene().buildIndex == 1 && !levelStarted && !levelFinished)
        {
            levelStarted = true;

            InitGame();            
        }

        //if active scene index = 1 and level not started and level finished, increment level
        // check if level has exceeded max levels, load game over scene, else invoke init game
        //after 1 second, assign levelstrted to ttue, level finished to false, and disable the
        //level generator.
        if (SceneManager.GetActiveScene().buildIndex == 1 && !levelStarted && levelFinished)
        {
            level++;

            if (level > maxLevels)
            {
                LoadByIndex(2);
            }
            else
            {
                Invoke("InitGame", 1);
                levelStarted = true;
                levelFinished = false;
                levelGenerator.enabled = false;
            }
        }

        //if active scene index != 1, disable the level generator.
        if (SceneManager.GetActiveScene().buildIndex != 1)
        {
            levelGenerator.enabled = false;
        }

        //if active scene index = 0 and lives are not = 15, set lives to 15, level to 1
        //level started and level finished to false.
        if (SceneManager.GetActiveScene().buildIndex == 0 && lives != 15)
        {
            lives = 15;
            level = 1;
            levelStarted = false;
            levelFinished = false;
        }
    }


    //Load the scene using the build settings index.
    public void LoadByIndex(int sceneIndex)
    {
        SceneManager.LoadScene(sceneIndex);
    }//LoadByIndex

};
