//Purpose: Controls how the pickup moves and how they collide.

//Default Unity includes:
using UnityEngine;
using System.Collections;

//My includes:
//N/A

//Class which contains a Start, Update, and onTriggerEnter2D function.
public class pickupScript : MonoBehaviour
{
    private bool moveUp = true;                                                                       //moveUp boolean assigned to true.
    private bool moveDown = false;                                                                    //moveDown function assigned to false.

    public float speed = 0.02f;                                                                       //speed float assigned to 0.02.
    private float upperLimit;                                                                         //upperLimit, used to control how high the pickup moves.
    private float lowerLimit;                                                                         //lowerLimit, used to control how low the pickup moves.

    //Start function which initialises the upper and lower limit.
    void Start()
    {
        upperLimit = transform.position.y + 0.25f;                                                    //upperLimit asigned to 0.25 above the pickup.
        lowerLimit = transform.position.y - 0.25f;                                                    //lowerLimit assigned to 0.25 below the pickup.
    }
	
	//Update function which controls the movement of the pickup.
	void Update ()
    {
        //if statement to see if the pickup is moving up, assigns the transform to move up.
	    if (moveUp)
        {
            transform.position += new Vector3(0, 0.1f, 0) * speed;
        }
        //else if statement to see if the pickup is moving down, assignes the transform to move down.
        else if (moveDown)
        {
            transform.position -= new Vector3(0, 0.1f, 0) * speed;
        }

        //if statement to see if the position is above the upper limit, swaps move up and move down
        if (transform.position.y >= upperLimit)
        {
            moveDown = true;
            moveUp = false;
        }
        //else if which does the opposite of above.
        else if (transform.position.y <= lowerLimit)
        {
            moveDown = false;
            moveUp = true;
        }

    }

    //onTriggerEnter2D  which checks if the player has collided with the pickup, and which pickup it is.
    //The active pickup is then set in the game manager depending on which pickup is collided with.
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player") && this.gameObject.CompareTag("DoubleJumpPickup"))
        {
            GameManager.instance.SetActivePickup(GameManager.Pickup.DOUBLEJUMP);
            this.gameObject.SetActive(false);
        }
        if (other.gameObject.CompareTag("Player") && this.gameObject.CompareTag("GunPickup"))
        {
            GameManager.instance.SetActivePickup(GameManager.Pickup.GUNPICKUP);
            this.gameObject.SetActive(false);

        }
        if (other.gameObject.CompareTag("Player") && this.gameObject.CompareTag("ShieldPickup"))
        {
            GameManager.instance.SetActivePickup(GameManager.Pickup.SHIELDPICKUP);
            this.gameObject.SetActive(false);
        }
    }
}

