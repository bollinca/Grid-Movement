using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBowlingMovement : MonoBehaviour
{
    private GameObject player;
    public Vector3 newPosition;
    public Vector3 oldPosition;

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
            movementCompleted = false;
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

    void CalcMoveTarget()
    {
        //Save previous position in case of movement cancellation
        oldPosition = transform.position;
        //Copy for calculations
        newPosition = transform.position;

        //determine direction player is moving in and set new destination
        if (Input.GetAxis("Vertical") != 0)
        {
            inputPolarity = Mathf.Sign(Input.GetAxis("Vertical"));
            newPosition.y += inputPolarity;
        }
        else if (Input.GetAxis("Horizontal") != 0)
        {
            inputPolarity = Mathf.Sign(Input.GetAxis("Horizontal"));
            newPosition.x += inputPolarity;
        }
    }

    // Moves player gradually
    void MoveTowardsTarget()
    {
        transform.position = Vector3.MoveTowards(transform.position, newPosition, Time.fixedDeltaTime * speed);
    }

    //Checks if movement is complete. (Used to repeat movement until destination is reached.)
    bool HasPlayerAdvanced()
    {
        MoveTowardsTarget();
        EndMovement();
        return movementCompleted;
    }

    void EndMovement()
    {
        if (player.transform.position == newPosition)
        {
            movementCompleted = true;
            isPlayerMoving = false;
        }
    }

    void CancelMovement()
    {
        newPosition = oldPosition;
    }

    //Isolates movement from update function, preventing extra input and forcing movement to grid.
    IEnumerator Move()
    {
        isPlayerMoving = true;
        CalcMoveTarget();
        yield return new WaitUntil(HasPlayerAdvanced);
    }

    //Cancels movement if player collides with terrain
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Terrain")
        {
            CancelMovement();
        }
    }


    //draft for bowling style
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            EndMovement();
        };
    }
}
