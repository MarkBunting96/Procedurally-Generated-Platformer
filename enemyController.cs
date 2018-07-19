//Purpose: Controls the movement of the enemy.

//Default Unity includes:
using UnityEngine;
using System.Collections;

//My includes:
//N/A

//Class which includes an Awake function, Update function, Flip function
//and an onTriggerEnter2D function
public class enemyController : MonoBehaviour
{

    private bool facingLeft = false;                                                                            //Bool which says if the enemy is facing left, assigned to false.
    private bool moveLeft = true;                                                                               //Bool which says if the enemy is moving left, assigned to true.
    private bool moveRight = false;                                                                             //Bool which says if the enemy is moving right, assigned to true.

    private float speed;                                                                                        //Float which controls the speed of the enemy.

    public Transform frontCheck;                                                                                //Public transform that is set to an empty game object which is a child
                                                                                                                //of the enemy and is infront of the enemy to check if the player 
                                                                                                                //is on the edge of the platform.

    private Vector3 frontCheckOffset;                                                                           //Vector3 which will contain the frontCheckOffset used with the frontCheck transform.
  
    private Rigidbody2D rb2d;                                                                                   //rigidbody2d will contain the rigidbody2d of the enemy.

    //Awake function which assigns initial values for the enemy.
    void Awake()
    {
        moveLeft = true;                                                                                        //moveLeft bool assigned to true.
        moveRight = false;                                                                                      //moveRight bool assigned to false.
        frontCheckOffset = new Vector3((float)0.5, 0, 0);                                                       //frontCheckOffset is assigned to a new vector which will be infront of the enemy.
        rb2d = GetComponent<Rigidbody2D>();                                                                     //rb2d is assigned the rigidbody2d component of the enemy.
        facingLeft = false;                                                                                     //facingLeft bool assigned to false.
        speed = 1.75f;                                                                                          //speed is assigned to 1.75.
    }

    //Update function which handles the enemy movement, makes sure the enemy doesnt fall of a platform.
    void Update()
    {
        //If statement that checks if the player is dead, using the game manager.
        if (!(GameManager.instance.playerDead))
        {
            //If statement which returns false if a line cast from the position - offset, 
            //to the frontCheck collides with the ground.
            if (!(Physics2D.Linecast(transform.position - frontCheckOffset, frontCheck.position,
                1 << LayerMask.NameToLayer("Ground"))))
            {
                Flip();                                                                                         //Flip function is called.
            }

            //If statement which returns true if moveLeft is true
            if (moveLeft)
            {
                rb2d.velocity = new Vector2(-2, 0) * speed;                                                     //velocity is assigned to move left.
            }
            //Else if statement which returns true if moveRight is true
            else if (moveRight)
            {
                rb2d.velocity = new Vector2(2, 0) * speed;                                                      //velocity is assigned to move right.
            }
        }
    }

    //Flip function which is used to switch booleans and flip the enemy sprite.
    void Flip()
    {
        facingLeft = !facingLeft;                                                                               //facingLeft is assigned to it's opposite value.
        moveRight = !moveRight;                                                                                 //moveRight is assigned to it's opposite value.
        moveLeft = !moveLeft;                                                                                   //moveLeft is assigned to it's opposite value.
        Vector3 theScale = transform.localScale;                                                                //The scale of the enemy is asigned to a vector3.
        theScale.x *= -1;                                                                                       //theScale's x value is multiplied by -1.
        transform.localScale = theScale;                                                                        //The new scale is assigned to the enemy scale.
    }

    //OnTriggerEnter2D is called when this object collides with another.
    void OnTriggerEnter2D(Collider2D other)
    {
        //if statement which checks if the other object is tagged as the player and if the player
        //is dead, using the game manager.
        if (other.gameObject.CompareTag("Player") && !(GameManager.instance.playerDead))
        {
            GameManager.instance.ResetPlayer();                                                                 //ResetPlayer is called from the game manager.
        }
    }
}
