//Purpose: Takes a stretched sprite in a sprite renderer and tiles that sprite across the 
//object instead of it appearing streched and contorted.

//Default Unity includes:
using UnityEngine;
using System.Collections;

//My includes:
//N/A

[RequireComponent(typeof(SpriteRenderer))]                                                            //SpriteRenderer is automatically added as a dependancy.

//class which contains an Awake function.
public class RepeatSpriteBoundary : MonoBehaviour
{
    private SpriteRenderer sprite;                                                                    //SpriteRenderer component will be stored in this variable.

    public GameObject pivot;                                                                          //GameObject which will be positioned at the top left of the sprite,
                                                                                                      //used for the pivot of this class.
    //Awake Function which tiles out the streched sprite.
    void Awake()
    {
        sprite = GetComponent<SpriteRenderer>();                                                      //The spriteRenderer component is stored in the sprite variable.

        Vector2 spriteSize = new Vector2(sprite.bounds.size.x / transform.localScale.x,               //Vector2 called spriteSize is assigned using the bounds and the
                                         sprite.bounds.size.y / transform.localScale.y);              //scale of the sprite.



        GameObject childPrefab = new GameObject();                                                    //childPrefab is declared to clone the sprite.
        SpriteRenderer childSprite = childPrefab.AddComponent<SpriteRenderer>();                      //SpriteRenderer added to the childPrefab
        childPrefab.transform.position = pivot.transform.position + new Vector3((float)0.5,           //The position of the prefab is assigned using the pivot.
                                                                                -(float)0.5, 0);

        childSprite.sprite = sprite.sprite;                                                           //The original sprite is assigned to the sprite of the child.

        
        GameObject child;                                                                             //New gameobject called child is declared.    

        //Nested for loops which loop through the x and y size and instantiate each 
        //small tile and assign new transforms
        for (int i = 0, l = (int)Mathf.Round(sprite.bounds.size.y); i < l; i++)
        {
            for (int j = 0, m = (int)Mathf.Round(sprite.bounds.size.x); j < m; j++)
            {
                child = Instantiate(childPrefab) as GameObject;
                child.transform.position = (pivot.transform.position + new Vector3((float)0.5, -(float)0.5, 0)) - ((new Vector3(-spriteSize.x * j, spriteSize.y * i, 0)));

                child.transform.parent = transform;
            }
        }
                                                                   
        childPrefab.transform.parent = transform;                                                     //Set the parent last on the prefab to prevent transform displacement
      
        sprite.enabled = false;                                                                       //Disable the currently existing sprite component since its now a repeated image
    }
}