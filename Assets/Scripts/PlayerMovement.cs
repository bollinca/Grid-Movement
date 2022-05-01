using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    private GameObject player;
    public Vector3 newPlayerPosition;
    
    private float speed;
    private float inputPolarity;
    
    private bool movementCompleted;
    private bool isPlayerMoving;

    void Start()
    {
        player = gameObject;
        speed = 0.3f;
        isPlayerMoving = false;
    }

    void FixedUpdate()
    {

        //Moves player by one space, but only if previous movement call has finished.
        if (PlayerTriedToMove() && isPlayerMoving == false)
        {
            StartCoroutine(Move());
        }
    }


    // Check for horizontal/vertical input.
    bool PlayerTriedToMove()
    {
        if (Input.GetAxis("Vertical") != 0 || Input.GetAxis("Horizontal") != 0)
        {
            return true;
        }
        else return false;
    }

    void SetMoveTarget()
    {
        //determine direction player is moving in and set new destination
        if (Input.GetAxis("Vertical") != 0)
        {
            inputPolarity = Mathf.Sign(Input.GetAxis("Vertical"));
            newPlayerPosition = new Vector3(transform.position.x, transform.position.y + inputPolarity, transform.position.z);
        }
        else if (Input.GetAxis("Horizontal") != 0)
        {
            inputPolarity = Mathf.Sign(Input.GetAxis("Horizontal"));
            newPlayerPosition = new Vector3(transform.position.x + inputPolarity, transform.position.y, transform.position.z);
        }
    }

    // Moves player gradually
    void MoveTowardsTarget()
    {
        transform.position = Vector3.MoveTowards(transform.position, newPlayerPosition, Time.fixedDeltaTime * speed);
    }

    //Checks if movement is complete and repeats movement until destination is reached.
    bool IsMovementDone()
    {
        if (player.transform.position == newPlayerPosition)
        {
            movementCompleted = true;
            return movementCompleted;
        }
        else
        {
            MoveTowardsTarget();
            return movementCompleted = false;
        }
    }


    //Isolates movement from update function, preventing extra input and forcing movement to grid.
    IEnumerator Move()
    {
        isPlayerMoving = true;
        SetMoveTarget();
        yield return new WaitUntil(IsMovementDone);
        isPlayerMoving = false;
    }
}
