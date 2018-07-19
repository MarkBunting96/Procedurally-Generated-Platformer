//Purpose: Manages the spawning of the platforms, has many variables which are randomly generated 
//to keep the game as random as possible, has variables that make sure levels arent impossible.
//controls spawning of enemies and pickups and traps, manages activating/deactivating objects
//based on certain conditions.

//Default Unity includes:
using UnityEngine;
using System.Collections;

//My includes:
//N/A

//class which contains an awake, initplatforms, initial platform, changeplatformlimits
//genmaxplatforms, update, checkturretspawns, spawnpickup, checkmovingturret, checktrap
//spawn and a calculatedist function.
public class SpawnPlatforms : MonoBehaviour
{
    public int maxPlatforms;                                                                          //stores the max num of platforms
    public int movingTurretCount;                                                                     //counts the number of moving turrets
    public int movingTurretChance;                                                                    //will be a number between 0 and 100 to determine the chance of moving turrets
    public int enemyCount;                                                                            //counts the num of enemies
    private int enemyChance;                                                                          //same as above
    private int turretChance;                                                                         //same as above
    private int spikeChance;                                                                          //same as above
    public int maxPickups;                                                                            //max number of pickups
    private int pickupCount;                                                                          //counts the pickups
    private int prevPlatformType;                                                                     //will be assigned to a number between 0 and 4, stores the previous patform
    private int curPlatformType;                                                                      //same as above but for current platform type, both variables used to calculate platform distances
    private int platformCount;                                                                        //counts the platforms
    public int randPlatform;                                                                          //will contain a random number for which platform to spawn the pickup on

    public float minTurretHeight;                                                                     //minimum height for the turret
    public float maxTurretHeight;                                                                     //maximum height for the turret
    private float horizontalMin;                                                                      //minimum horizontal distance between platforms
    private float horizontalMax;                                                                      //maximum horizontal distance between platforms
    private float verticalMin;                                                                        //same as above but for vertical
    private float verticalMax;                                                                        //same
    public float difficultyScale;                                                                     //difficulty scale, will be a multiplier between 0.5 and 2.5, used to increase or decrease certain variables
    private float lowestPlatformHeight;                                                               //keeps track of the lowest platform height, so the game knows when the player has fallen too far
    private float minDifficulty;                                                                      //minimum difficulty stored
    private float maxDifficulty;                                                                      //same as above but max

    private bool increaseTurretSpawns;                                                                //bool to decide if turret spawns are increased
    public bool pickupSpawned;                                                                        //bool to decide if the pickup has been spawned

    public GameObject portal;                                                                         //all game object prefabs to be spawned
    public GameObject enemy;
    public GameObject spikeTrap;
    public GameObject turret;
    public GameObject doubleJumpPickup;
    public GameObject gunPickup;
    public GameObject shieldPickup;
    public GameObject movingTurret;

    public GameObject[] platformTypes;                                                                //array of platform type prefabs to be spawned
    public GameObject[] pickups;                                                                      //arrays to store a list of pickups, enemies, moving turrets and platforms
    public GameObject[] enemies;
    public GameObject[] movingTurrets;
    public GameObject[] platformArray;                                                                 


    private Vector2 originPosition;                                                                   //vector2 to store the origin position of this object, used and changed to declare where platforms spawn
    private Vector2 playerOriginPosition;                                                             //vector2 to store the players origin position, used to check how far the player has travelled and to spawn more platforms if necessary

    //Awake function which initialises the difficulty variables, intitial
    //difficulty set to 1.
    void Awake()
    {
        difficultyScale = 1f;
        minDifficulty = 0.5f;
        maxDifficulty = 2.5f;
    }

    //Initplatforms acts as the initialise function of the class.
    public void InitPlatforms()
    {
        //if active pickup is double jump, call changeplatform limits, else, initialise
        //platform variables and spikeChance.
        if (GameManager.instance.activePickup == GameManager.Pickup.DOUBLEJUMP)
        {
            changePlatformLimits();
        }
        else
        {
            horizontalMin = 3;                                                                        //values set after testing which values always produced possible gaps
            horizontalMax = 4.5f;
            verticalMin = -2.25f;
            verticalMax = 2.25f;

            spikeChance = (int)(10 * difficultyScale);                                                //spikechance set to 10 * difficulty scale, min 5, max 25
        }

        //if active pickup is gun, change moving turret chance and enemy chance
        //to higher values, else, no moving turrets, less enemy chance
        if (GameManager.instance.activePickup == GameManager.Pickup.GUNPICKUP)
        {
            movingTurretChance = (int)(25 * difficultyScale);                                         //min 13, max 63
            enemyChance = (int)(30 * difficultyScale);                                                //min 15, max 75
        }
        else
        {
            movingTurretChance = 0;
            enemyChance = (int)(15 * difficultyScale);                                                //min 7, max 37                                                                       
        }

        //if pickup is shield, increase turret chance, increase turret spawns, 
        //max turret height 9, min turret height set to 9, else
        //norm turret chance, no increased spawns, min height 3.5 and max 9
        if (GameManager.instance.activePickup == GameManager.Pickup.SHIELDPICKUP)
        {
            turretChance = (int)(35 * difficultyScale);                                               //min 18, max 88
            increaseTurretSpawns = true;                                                              
            maxTurretHeight = 9f;                                                                     //prevents extra turrets blocking platforms
            minTurretHeight = maxTurretHeight;
        }
        else
        {
           turretChance = (int)(10 * difficultyScale);                                                //min 5, max 25
           increaseTurretSpawns = false;
           minTurretHeight = 3.5f;
           maxTurretHeight = 9f;
        }

        enemyCount = 0;                                                                               //enemy count initialised to 0
        pickupSpawned = false;                                                                        //pickup spawned initialised to false
        movingTurretCount = 0;                                                                        //moving turret count initialised to 0
        lowestPlatformHeight = 0;                                                                     //lowest plat height initialised to 0 (initial platform is 0)
        platformCount = 0;                                                                            //platform count initialised to 0
        pickupCount = 0;                                                                              //pickup count initialised to 0
        maxPickups = 2;                                                                               //max pickups initialised to 2
        pickups = new GameObject[maxPickups];                                                         //pickups initialised to new array of gameobjects with the size of max pickups
        genMaxPlatforms();                                                                            //gen max platforms is called
        enemies = new GameObject[maxPlatforms];                                                       //enemies is assigned to a new array of gaeobjects with the size of max platforms (only 1 enemy per platform)
        movingTurrets = new GameObject[maxPlatforms + 3];                                             //movingturrets is assigned toa new array of gameobjects with the size of maxplatforms + 3 (1 per platform + 3 at the end if shield pickup active)

        randPlatform = (int)Random.Range(2, (int)(maxPlatforms / 2));                                 //randPlatform is assigned as a random number between 2 and half of the max platforms (pickup will spawn near the beginning of the level)

        platformArray = new GameObject[maxPlatforms + 1];                                             //platformArray is assigned as a new array of gameObjects with the size of maxplatforms + 1
        transform.position = new Vector3(0, -1, 0);                                                   //position is set to -1 below the origin (player will spawn on origin)
        originPosition = transform.position;                                                          //origin position is set to position
        playerOriginPosition = GameManager.instance.GetPlayerPosition().position;                     //player origin position is set to player pos using the function in the game manager
        prevPlatformType = 2;                                                                         //prevplatformtype is set to 2 (first platform is always of type 2
    }

    //function which increases the horiz max, and vert min and max so that the platforms
    //have larger gaps between them. also spike chance is increased. Only called when
    //doubleJump is active.
    public void changePlatformLimits ()
    {
        horizontalMax = 8.5f;
        verticalMax = 4f;
        verticalMin = -4f;

        spikeChance = (int)(30 * difficultyScale);                                                    //min 15, max 75
    }

    //randomly selects how many platforms spawned using a range which is modified using
    //difficulty scale variable
    public void genMaxPlatforms()
    {
        maxPlatforms = (int)Random.Range((int)(5 * difficultyScale), (int)(10 * difficultyScale));    //min range (3-13) max range (5-25)
    }

    //update function handles level management, resetting the player and difficulty clamping
    void Update()
    {
        //if level started
        if (GameManager.instance.levelStarted)
        {
            //if player position is more than origin position + 5, and there are platforms
            //still to spawn, call spawn and reset the originposition to current position
            if (GameManager.instance.GetPlayerPosition().position.x > playerOriginPosition.x + 5 &&
                platformCount < maxPlatforms)
            {
                Spawn();
                playerOriginPosition = GameManager.instance.GetPlayerPosition().position;
            }

            //if player y pos less than lowerst plat height - 5 and player is alive
            //reset player
            if (GameManager.instance.GetPlayerPosition().position.y < lowestPlatformHeight - 5 &&
                !(GameManager.instance.playerDead))
            {
                GameManager.instance.ResetPlayer();
            }
        }

        //if difficulty < min difficulty, difficulty = min difficulty
        if (difficultyScale < minDifficulty)
        {
            difficultyScale = minDifficulty;
        }

        //same but for max
        if (difficultyScale > maxDifficulty)
        {
            difficultyScale = maxDifficulty;
        }
    }

    //spawns the initial platform the the player spawns upon, adds it to the platform array
    //with the array index of platform count, sets lowest platform height, increments platform count
    //then calls spawn
    public void InitialPlatform()
    {
        platformArray[platformCount] = (GameObject)Instantiate(platformTypes[2], 
                                        new Vector2(originPosition.x, originPosition.y), 
                                        Quaternion.identity);
        lowestPlatformHeight = platformArray[platformCount].transform.position.y;
        platformCount++;

        Spawn();
    }

    //checks if a turret should be spawned using random num and turret chance
    private void CheckTurretSpawn()
    {
        int randNum = (int)Random.Range(1, 100);                                                      //random number between 1 and 100
        //if random number is less than or equal to turret chance
        if (randNum <= turretChance)
        {
            //if increaseturretspawns is active, max out the turret height
            if (increaseTurretSpawns)
            {
                minTurretHeight = maxTurretHeight;
            }

            int randxPos = (int)Random.Range(0 - curPlatformType/2, 0 + curPlatformType/2);           //randomx position between the width of the current platform
            float randHeight = Random.Range(minTurretHeight, maxTurretHeight);                        //random height between min and max turret height
            Instantiate(turret, originPosition + new Vector2(randxPos, randHeight),                   //spawn a turret and a spike on top using the random numbers
                        Quaternion.identity);
            Instantiate(spikeTrap, originPosition + new Vector2(randxPos, randHeight + 1),
                        Quaternion.identity);

            //if increased turret spawns, spawn 2 more turrets and spikes alongside the turret
            if (increaseTurretSpawns)
            {
                Instantiate(turret, originPosition + new Vector2(randxPos + 1.5f, maxTurretHeight),
                            Quaternion.identity);
                Instantiate(spikeTrap, originPosition + new Vector2(randxPos + 1.5f, maxTurretHeight + 1), 
                            Quaternion.identity);
                Instantiate(turret, originPosition + new Vector2(randxPos + 3 , maxTurretHeight),
                            Quaternion.identity);
                Instantiate(spikeTrap, originPosition + new Vector2(randxPos + 3, maxTurretHeight + 1), 
                            Quaternion.identity);
            }
        }
    }

    //spawns the pickup, takes a gameobject which is the pickup to spawn, 
    //adds it to the pickup array using pickup count as an index
    //sets spawned to true and increments pickup count.
    //then checks which pickup is being spawned and calls/sets 
    //appropriate functions/variables.
    private void SpawnPickup(GameObject pickupToSpawn)
    {
        pickups[pickupCount] = (GameObject)Instantiate(pickupToSpawn, originPosition + 
                                new Vector2(0, (float)1.5), Quaternion.identity);
        pickupSpawned = true;
        pickupCount++;
        //if pickup tag is doublejump, change platform limits
        if (pickupToSpawn.gameObject.CompareTag("DoubleJumpPickup"))
        {
            changePlatformLimits();
        }

        //if pickup tag is gun, change enemy chance and moving turret chance
        if (pickupToSpawn.gameObject.CompareTag("GunPickup"))
        {
            enemyChance = (int)(30 * difficultyScale);                                                //min 15, max 75
            movingTurretChance = (int)(25 * difficultyScale);                                         //min 13, max 63
        }

        //if tag is shielf, change turret chance, increase turret chance, 
        //max out max turret height
        if (pickupToSpawn.gameObject.CompareTag("ShieldPickup"))
        {
            turretChance = (int)(35 * difficultyScale);                                               //min 18, max 88
            increaseTurretSpawns = true;
            maxTurretHeight = 9f;
            minTurretHeight = maxTurretHeight;
        }
    }

    //checks if a trap should be spawned using random num and spike chance
    private void CheckTrapSpawn()
    {
        int randNum = (int)Random.Range(1, 100);                                                      //random number between 1 and 100
        //if randnum <= spike chance
        if (randNum <= spikeChance)
        {

            int randxPos = (int)Random.Range((0 - curPlatformType / 2)+1,                             //randomx pos between the middle of the platform (never on edges as could mean impossible jumps)
                                             (0 + curPlatformType / 2) - 1);

            Instantiate(spikeTrap, originPosition + new Vector2(randxPos, 0.87f),                     //spawn spikes in random position, just inside the platform
                        Quaternion.identity);
        }
    }

    //checks if a movingturret should be spawned using random num and movingturret chance
    private void CheckMovingTurret()
    {
        int randNum = (int)Random.Range(1, 100);                                                      //random number between 1 and 100

        Vector2 spawnPos = new Vector2(originPosition.x + ((curPlatformType + 1)/2) + 2,              //spawn pos set using the cur platform type, making it spawn infront of the platform
                                       originPosition.y + 2);

        //if randnum <= movingturretchance, spawn moving turret and increment count
        if (randNum <= movingTurretChance)
        {
            movingTurrets[movingTurretCount] = (GameObject)Instantiate(movingTurret, spawnPos, 
                                                Quaternion.identity);
            movingTurretCount++;
        }
    }
    
    //main spawning function, decides which platform to spawn and spanws them, 
    //using random numbers and certain variables to decide weather certain
    //features should be spawned.
    public void Spawn()
    {
        //for loop which loops through 5 times aslong as maxplatforms hasnt been
        //reached. spawns 5 platforms at a time.
        for (int i = 0; i < 5 && platformCount < maxPlatforms; i++)
        {
            //if statements which decide how big the platforms that can be spawned
            //are, usind the difficulty scale. higher the scale, narrower the platforms
            //and vice versa
            if (difficultyScale >= minDifficulty && difficultyScale < 0.8)
            {
                curPlatformType = Random.Range(4, platformTypes.Length);
            }
            else if (difficultyScale >= 0.8 && difficultyScale < 1.3)
            {
                curPlatformType = Random.Range(3, platformTypes.Length);
            }
            else if (difficultyScale >= 1.3 && difficultyScale < 1.5)
            {
                curPlatformType = Random.Range(2, platformTypes.Length);
            }
            else if (difficultyScale >= 1.5 && difficultyScale < 1.8)
            {
                curPlatformType = Random.Range(1, platformTypes.Length);
            }
            else if (difficultyScale >= 1.8 && difficultyScale < 2.3)
            {
                curPlatformType = Random.Range(0, platformTypes.Length -1);
            }
            else if (difficultyScale >= 2.3 && difficultyScale <= maxDifficulty)
            {
                curPlatformType = Random.Range(0, platformTypes.Length - 2);
            }

            //random position declared by adding a new vector2 with a random
            //range between horizontal min (modified by calculate dist) and
            //horizontal max (modified) for the x value, and a random range
            //between vert min and max for the y value. 
            Vector2 randomPosition = originPosition + new Vector2(Random.Range
                (horizontalMin * calculateDist(prevPlatformType, curPlatformType),
                horizontalMin * calculateDist(prevPlatformType, curPlatformType) +
                horizontalMax - horizontalMin - 1),
                Random.Range(verticalMin, verticalMax));

            prevPlatformType = curPlatformType;                                                       //prev platform type is set as curplatform type

            platformArray[platformCount] = (GameObject)Instantiate(platformTypes[curPlatformType],    //platform is instantiated and added to the array with the index of platform count
                                            randomPosition, Quaternion.identity);

            //ifstatement which checks if the new platform is lower than lowest platform height
            //and resets it if so
            if (platformArray[platformCount].transform.position.y < lowestPlatformHeight)
            {
                lowestPlatformHeight = platformArray[platformCount].transform.position.y;
            }

            platformCount++;                                                                          //platform count is incremented
            originPosition = randomPosition;                                                          //originpos is set to random pos

            CheckMovingTurret();                                                                      //checkmovingturret is called to check if a moving turret will be spawned

            //if platform count = randplatform and no pickup has been spawned and pickup count
            //is less than max pickups, pick a random pickup and call spawn pickup using that
            //game object
            if (platformCount == randPlatform && !pickupSpawned && pickupCount < maxPickups)
            {
                int randomPickup = (int)Random.Range(1, 100);
                if (randomPickup >=1 && randomPickup <= 30)
                {
                    SpawnPickup(gunPickup);
                }
                else if (randomPickup > 30 && randomPickup <= 70)
                {
                    SpawnPickup(doubleJumpPickup);
                }
                else if (randomPickup > 70 && randomPickup <= 100)
                {
                    SpawnPickup(shieldPickup);
                }
            }


            CheckTurretSpawn();                                                                       //checkturretspawn is called to check if a turret will be spawned

            //if curplatformtype = 4, 3 or 2 and it isnt the final platform, check trap spawn
            if (curPlatformType == 4 || curPlatformType == 3 || curPlatformType == 2 && 
                platformCount < maxPlatforms)
            {
                CheckTrapSpawn();                                                                     //checktrap spawn is called to check if a trap will be spawned

                //if curplattype = 2, and it is randplatform and pickup spawned is false, 
                //run code to spawn enemy
                if (!(curPlatformType == 2 && platformCount == randPlatform && pickupSpawned))
                {
                    int randNum = (int)Random.Range(1, 100);                                          //randnumber between 1 and 100

                    //if randnum < enemychance, spawn enemy, add to array using enemy count
                    //as index, increment enemy count
                    if (randNum <= enemyChance)
                    {
                        enemies[enemyCount] = (GameObject)Instantiate(enemy, originPosition + 
                                                new Vector2(0, (float)1.5), Quaternion.identity);
                        enemyCount++;
                    }
                }
            }

            //if platform count is morethan or equal to max platforms, spawn a portal above
            //the platform, then check increase turret spawns, if so, spawn 3 moving turrets after
            //final platform
            if (platformCount >= maxPlatforms)
            {
                Instantiate(portal, originPosition + new Vector2(0, (float)1.5), Quaternion.identity);

                if (increaseTurretSpawns)
                {
                    movingTurrets[movingTurretCount] = (GameObject)Instantiate(movingTurret, new Vector2
                        (originPosition.x + ((curPlatformType + 1) / 2) + 4, originPosition.y + 2), 
                         Quaternion.identity);

                    movingTurretCount++;

                    movingTurrets[movingTurretCount] = (GameObject)Instantiate(movingTurret, new Vector2
                        (originPosition.x + ((curPlatformType + 1) / 2)  + 4, originPosition.y + 7.5f),
                         Quaternion.identity);

                    movingTurretCount++;

                    movingTurrets[movingTurretCount] = (GameObject)Instantiate(movingTurret, new Vector2
                        (originPosition.x + ((curPlatformType + 1) / 2) + 4, originPosition.y - 3.5f),
                         Quaternion.identity);

                    movingTurretCount++;
                }
            }
        }
    }

    //calcdist returns a float after taking prev platform type and cur platform type
    //and calculating the minimum distance between platforms (also used for max
    //distance.
    private float calculateDist (int x, int y)
    {
        float ans = 0;

        ans = (x + y) * 0.25f + 1;

        return ans;
    }
}