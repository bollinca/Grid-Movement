using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    private GameObject player;
    public Vector3 newPosition;
    public Vector3 oldPosition;

    private float speed;
    private float inputPolarity;

    private bool movementCompleted;
    private bool isPlayerMoving;

    private float moveStartTime;
    private float moveCurrentTime;
    private float timeOutLimit;

    void Start()
    {
        player = gameObject;
        speed = 0.3f;
        isPlayerMoving = false;
        timeOutLimit = 0.32f;
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
        //Save previous position in case of obstacles
        oldPosition = transform.position;
        //determine direction player is moving in and set new destination
        if (Input.GetAxis("Vertical") != 0)
        {
            inputPolarity = Mathf.Sign(Input.GetAxis("Vertical"));
            newPosition = new Vector3(transform.position.x, transform.position.y + inputPolarity, transform.position.z);
        }
        else if (Input.GetAxis("Horizontal") != 0)
        {
            inputPolarity = Mathf.Sign(Input.GetAxis("Horizontal"));
            newPosition = new Vector3(transform.position.x + inputPolarity, transform.position.y, transform.position.z);
        }
    }

    // Moves player gradually
    void MoveTowardsTarget()
    {
        transform.position = Vector3.MoveTowards(transform.position, newPosition, Time.fixedDeltaTime * speed);
    }

    //Checks if movement is complete and repeats movement until destination is reached.
    bool IsMovementDone()
    {
        if (player.transform.position == newPosition)
        {
            movementCompleted = true;
            return movementCompleted;
        }
        else
        {
            MoveTowardsTarget();
            if (IsPlayerStuck())
            {
                CancelMovement();
            }
            return movementCompleted = false;
        }
    }

    void CancelMovement()
    {
        newPosition = oldPosition;
    }

    bool IsPlayerStuck()
    {
        moveCurrentTime = Time.time;
        float timeSpentMoving = moveCurrentTime - moveStartTime;
        if (timeSpentMoving > timeOutLimit)
        {
            return true;
        }
        else
        {
            return false;
        }
    }


    //Isolates movement from update function, preventing extra input and forcing movement to grid.
    IEnumerator Move()
    {
        isPlayerMoving = true;
        SetMoveTarget();
        moveStartTime = Time.time;
        yield return new WaitUntil(IsMovementDone);
        isPlayerMoving = false;
    }
}
