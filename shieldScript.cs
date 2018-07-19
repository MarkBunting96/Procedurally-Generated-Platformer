//Purpose: Controls shield collision, prevents the player from getting hit.

//Default Unity includes:
using UnityEngine;
using System.Collections;

//My includes:
//N/A

//class which includes an OnTriggerEnter2D function.
public class shieldScript : MonoBehaviour
{ 
    //OnTriggerEnter2D function which controls collision.
    void OnTriggerEnter2D(Collider2D other)
    {
        //If statement which checks if the other object is one of the two bullet types, then destroys that bullet
        //and calls the decreaseShieldHealth function from the gameManager.
        if (other.gameObject.CompareTag("EnemyBullet") || other.gameObject.CompareTag("MovingTurretBullet"))
        {
            other.gameObject.SetActive(false);
            Destroy(other);
            GameManager.instance.decreaseShieldHealth();
        }
    }
}
