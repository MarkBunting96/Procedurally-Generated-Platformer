//Purpose: Controls the movement of the player, jumping, shooting
//and double jumping using input checks and also whether certain
//requirements are met using the game manager.

//Default Unity includes:
using UnityEngine;
using System.Collections;

//My includes:
//N/A

//class which includes an awake function, an update function, a fixed update function
//and a flip function. 
public class PlayerController : MonoBehaviour
{

    public bool facingRight = true;                                                                   //bool which says if the player is facing right, initialised to true.
    private bool grounded = false;                                                                    //bool which says if the player is grounded, initialised to false.
    private bool secondJump = false;                                                                  //bool which says if the player can perform a second jump, initialised to false.

    public float moveForce = 50f;                                                                     //move force which is applied when the player moves, initialised to 50.
    public float maxSpeed = 10f;                                                                      //max speed which is the fastest the player can move, initialised to 10.
    public float jumpForce = 300f;                                                                    //jumpForce which is the force applied when the player moves, initialised to 300.
    private float fireRate;                                                                           //the rate which the player can shoot.
    private float nextShot;                                                                           //when the player can next fire.

    public Transform backGroundCheck;                                                                 //transform which is assigned to an empty game object at the back end of the player, used to check if the player is grounded.
    public Transform frontGroundCheck;                                                                //same as above but for the front of the player.

    public GameObject shield;                                                                         //shield gameObject which is a child of the player.
    public GameObject bullet;                                                                         //bullet gameobject to be spawned when the player fires.

    private Rigidbody2D rb2d;                                                                         //the rigidbody2d of the player.

    private Vector3 groundCheckOffset;                                                                //offset which is used with the ground check objects for line casting.

    //Awake function which initialises some values and assigns some game objects.
    void Awake()
    {
        shield = GameObject.FindGameObjectWithTag("PlayerShield");                                    //shield is assigned to the player shield object which is a child of the player.
        fireRate = 0.5f;                                                                              //fire rate is set to 0.5.
        nextShot = fireRate;                                                                          //next shot is set to the fire rate.
        rb2d = GetComponent<Rigidbody2D>();                                                           //rb2d is assigned to the rigid body2d component of the player.
        groundCheckOffset = new Vector3((float)0.4, 0, 0);                                            //offset is assigned as a vector which is 0.4 infront of and behind the player.
    }

    //Update function which controls the jumping and firing of the player and if the player is 
    //grounded.
    void Update()
    {
        //if statement which checks if the player is alive using the game manager.
        if (!(GameManager.instance.playerDead))
        {
            //if statement which uses two linecasts to check if the player is grounded, casts a
            //straight line down from the front/back of the player to slightly below the player
            //and returns true if they collide with the ground.
            if (Physics2D.Linecast(transform.position - groundCheckOffset, backGroundCheck.position,
                                   1 << LayerMask.NameToLayer("Ground")) ||
                Physics2D.Linecast(transform.position + groundCheckOffset, frontGroundCheck.position,
                                   1 << LayerMask.NameToLayer("Ground")))
            {
                grounded = true;                                                                      //Sets grounded to true.                                                   
            }
            else
            {
                grounded = false;                                                                     //else, sets grounded to false.
            }

            //if statement which checks if the jump button has been pressed, and if the player 
            //if grounded, then zeroes out the y velocity, and adds the jump force to it, 
            //then sets second jump to true.
            if (Input.GetButtonDown("Jump") && grounded)
            {
                rb2d.velocity = new Vector2(rb2d.velocity.x, 0);
                rb2d.AddForce(new Vector2(0, jumpForce));
                secondJump = true;
            }

            //if statement which checks if the jump button has been pressed, the second jump bool
            //is set to true, and player is not grounded, and if the active pickup in the game
            //manager is the doublejump pickup, then sets double jump to false, and jumps again.
            if (Input.GetButtonDown("Jump") && secondJump && !grounded && 
                GameManager.instance.activePickup == GameManager.Pickup.DOUBLEJUMP)
            {
                secondJump = false;
                rb2d.velocity = new Vector2(rb2d.velocity.x, 0);
                rb2d.AddForce(new Vector2(0, jumpForce));
            }

            //if statement which chceks if the fire button has been pressed, and if the active pickup
            //in the game manager is the gun pickup.
            if (Input.GetButtonDown("Fire") &&  GameManager.instance.activePickup == 
                GameManager.Pickup.GUNPICKUP)
            {
                //if statement which checks if enough time has passed, gets the bullet offset, sets
                //a new next shot and spawns a bullet.
                if (Time.time > nextShot)
                {
                    Vector3 bulletOffset = new Vector3(0.2f, -0.05f, 0);
                    nextShot = Time.time + fireRate;
                    Instantiate(bullet, gameObject.transform.position + bulletOffset, 
                                Quaternion.identity);
                }
            }
        }
    }

    //fixed update function which controls the movement of the player
    void FixedUpdate()
    {
        //if statement which checks if the player is alive (prevents player movement 
        //when the death screen is active.
        if (!(GameManager.instance.playerDead))
        {
            float h = Input.GetAxisRaw("Horizontal");                                                 //float which keeps track of the horizontal input.                                     

            //if statement which checks if the player is at max speed, then keeps the speed
            //at max speed.
            if (Mathf.Abs(rb2d.velocity.x) > maxSpeed)
            {
                rb2d.velocity = new Vector2(Mathf.Sign(rb2d.velocity.x) 
                                            * maxSpeed, rb2d.velocity.y);
            }

            //if statement which checks if the player is below max speed, then 
            //applies the move force using the horizontal input.
            if (h * rb2d.velocity.x < maxSpeed)
            {
                rb2d.AddForce(Vector2.right * h * moveForce);
            }

            //if statement which checks if there is no input and the player
            //is still moving, then halts the player.
            if ((h == 0) && (rb2d.velocity.x != 0))
            {
                rb2d.velocity = new Vector2(0, rb2d.velocity.y);
            }

            //if statement which checks if the player is moving right and facing left, then
            //flips the player.
            if (h > 0 && !facingRight)
            {
                Flip();
            }
            //else if statement which does the opposite.
            else if (h < 0 && facingRight)
            {
                Flip();
            }
        }
        else
        {
            //if the player is facing left, flip the player to face right.
            if (!facingRight)
            {
                Flip();
            }
        }                
    }

    //Flip function which is used to switch booleans and flip the player sprite.
    void Flip()
    {
        facingRight = !facingRight;                                                                   //facing right is assigned to its opposite value.
        Vector3 theScale = transform.localScale;                                                      //the scale of the player is assigned to a vector3
        theScale.x *= -1;                                                                             //the scale is reversed.
        transform.localScale = theScale;                                                              //the player scale is assigned to the reverse scale.
    }
}