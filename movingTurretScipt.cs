//Purpose: Controls the way which the turret moves along its vertical platform,
//and how the turret fires.

//Default Unity includes:
using UnityEngine;
using System.Collections;

//My includes:
//N/A

//class which contains a start function, update function and an ontriggerenter2D function.
public class movingTurretScipt : MonoBehaviour
{
    public GameObject TurretBullet;                                                                   //Bullet which the turret will fire.
    public GameObject TurretPlatform;                                                                 //Vertical platform that the turret moves on.

    private bool moveUp = true;                                                                       //Bool which controls if the turret moves up, initialised to true.
    private bool moveDown = false;                                                                    //Bool which controls if the turret moves down, initialised to false.

    private float speed = 0;                                                                          //speed which the turret moves, assigned to 0 as it will be randomly assigned later.
    private float upperLimit;                                                                         //how high the turret can move.
    private float lowerLimit;                                                                         //how low the turret can move.
    private float minFireRate;                                                                        //min fire rate of the turret.
    private float maxFireRate;                                                                        //max fire rate of the turret.
    private float nextShot;                                                                           //when the turret will next fire.

    private Vector3 bulletOffset;                                                                     //Vector3 which will be the offset of the transform where the bullet will be spawned.

    //Start function which initialises the initial values of the attributes above.
    void Start()
    {
        speed = Random.Range(0.2f, 1);                                                                //speed is assigned a random value between 0.2 and 1.
        upperLimit = transform.position.y + 2f;                                                       //upper limit is assigned 2 above the y position.
        lowerLimit = transform.position.y - 2f;                                                       //lower limit is assigned as 2 below the y position.

        bulletOffset = transform.position - new Vector3(0.6f, 0, 0);                                  //bullet offset is assigned as slightly to the left of the transform.
        minFireRate = 1.5f;                                                                           //min fire rate is assigned 1.5.
        maxFireRate = 4f;                                                                             //max fire rate is assigned 4.
        nextShot = Random.Range(minFireRate, maxFireRate);                                            //next shot is assigned as a random range between min and max fire rate.
    }

    //Update function whiich controls the movement of the turret
    //and also when the turret fires.
    void Update()
    {
        //if statement that checks if the player is alive using the game manager.
        if (!(GameManager.instance.playerDead))
        {
            //if statement that checks if the turret is moving up, then moves the turret up 
            //using the speed variable.
            if (moveUp)
            {
                transform.position += new Vector3(0, 0.1f, 0) * speed;
            }
            //else if statement that chekcs if the player is moving down, then moves the turret
            //down using the speed variable.
            else if (moveDown)
            {
                transform.position -= new Vector3(0, 0.1f, 0) * speed;
            }

            //if statement that checks if the player has reached the upper limit, then flips
            //the move down and move up booleans.
            if (transform.position.y >= upperLimit)
            {
                moveDown = true;
                moveUp = false;
            }
            //else if statement that checks if the player has reached the lower limit then flips
            //the move down and move up booleans.
            else if (transform.position.y <= lowerLimit)
            {
                moveDown = false;
                moveUp = true;
            }

            //if statement that checks if enough time has passed, resets the next shot using
            //the min and max range, sets a new bullet offset for where the bullet is to be fired
            //from, then spawns the bullet.
            if (Time.time > nextShot)
            {
                nextShot = Time.time + Random.Range(minFireRate, maxFireRate);
                bulletOffset = transform.position - new Vector3(0.6f, 0, 0);
                Instantiate(TurretBullet, bulletOffset, Quaternion.identity);
            }
        } 
    }

    //on trigger enter2d function which checks if it is a friendlybullet object that has collided
    //with it, then sets the platform the turret is on to deactive.
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("FriendlyBullet"))
        {
            TurretPlatform.SetActive(false);
        }
    }
}
