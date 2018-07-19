//Purpose: Controls activating and deactivating UI objects for either the gameOver condition, or the You Win condition.

//Default Unity includes:
using UnityEngine;
using System.Collections;

//My includes:
//N/A                                                                        

//Class which contains an Update function
public class endgameScript : MonoBehaviour
{
    public GameObject gameOver;                                                                       //GameObject which stores the gameOver text.
    public GameObject youWin;                                                                         //GameObject which stores the youWin text.
	
	//Update function which checks if the player has completed all of the levels and 
    //activates/deactivates the appropriate UI element.
	void Update ()
    {
        if (GameManager.instance.level > GameManager.instance.maxLevels)
        {
            gameOver.gameObject.SetActive(false);
            youWin.gameObject.SetActive(true);
        }
        else
        {
            gameOver.gameObject.SetActive(true);
            youWin.gameObject.SetActive(false);
        }
	}
}
