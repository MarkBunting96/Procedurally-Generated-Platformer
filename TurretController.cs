//Purpose: Controls the way which the turret fires.

//Default Unity includes:
using UnityEngine;
using System.Collections;

//My includes:
//N/A

//turretcontroller class which contains a start, and an update function.
public class TurretController : MonoBehaviour
{
    public GameObject TurretBullet;                                                                   //bullet gameObject which will be spawned

    private float minFireRate;                                                                        //minimum fire rate.
    private float maxFireRate;                                                                        //maximum fire rate.
    private float nextShot;                                                                           //time when the next shot will happen.

    private Vector3 bulletOffset;                                                                     //Vector3 offset for where the bullet will be spawned. 
           
    //Start function which initialises the offset, the fire rate range and the next shot
    //using those ranges.
    void Start()
    {
        bulletOffset = transform.position - new Vector3(0, 0.6f, 0);                                  //Bullet offset is set to slightly below the transform of this object.
        minFireRate = 1.5f;                                                                           //min fire rate is assigned as 1.5.
        maxFireRate = 4f;                                                                             //max fire rate is assigned as 4
        nextShot = Random.Range(minFireRate, maxFireRate);                                            //next shot is assigned as a random range between the min and max fire rate.
    }

    //Update function which shoots a bullet when time has expired next shot, when the player is 
    //alive.
    void Update()
    {
        //if statement which checks if the player is alive using the game manager
        if (!(GameManager.instance.playerDead))
        {
            //if statement which checks if enough time has passed, then sets the new next
            //shot value using the range again, then spawns a bullet.
            if (Time.time > nextShot)
            {
                nextShot = Time.time + Random.Range(minFireRate, maxFireRate);
                Instantiate(TurretBullet, bulletOffset, Quaternion.identity);
            }
        }
    }
}
