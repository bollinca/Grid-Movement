using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerBowlingMovement : MonoBehaviour
{
    private GameObject player;
    public Vector3 moveDirection;
    public Vector3 oldPos;
    private Vector3 finalPos;
    public Grid grid;

    private float speed;
    private float inputPolarity;
    private Vector2 cardinalDirection;

    private bool movementCompleted;
    private bool isPlayerMoving;

    void Start()
    {
        player = gameObject;
        speed = 0.3f;
        isPlayerMoving = false;
        oldPos = transform.position;
    }

    void FixedUpdate()
    {
        //Moves player by one space, but only if previous movement call has finished.
        if (PlayerTriedToMove() && !isPlayerMoving)
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
        oldPos = transform.position;
        //Copy for calculations
        moveDirection = transform.position;

        //determine direction player is moving in and set new destination
        if (Input.GetAxis("Vertical") != 0)
        {
            inputPolarity = Mathf.Sign(Input.GetAxis("Vertical"));
            moveDirection.y += inputPolarity;
            cardinalDirection = new Vector2(0, inputPolarity);
        }
        else if (Input.GetAxis("Horizontal") != 0)
        {
            inputPolarity = Mathf.Sign(Input.GetAxis("Horizontal"));
            moveDirection.x += inputPolarity;
            cardinalDirection = new Vector2(inputPolarity, 0);
        }
    }

    // Moves player gradually
    void MoveTowardsTarget()
    {
        transform.position = Vector3.MoveTowards(transform.position, finalPos, Time.fixedDeltaTime * speed);
    }

    //Checks if movement is complete. (Used to repeat movement until destination is reached.)
    bool IsMovementComplete()
    {
        MoveTowardsTarget();
        EndMovement();
        return movementCompleted;
    }

    void EndMovement()
    {
        if (player.transform.position == finalPos)
        {
            movementCompleted = true;
            isPlayerMoving = false;
        }
    }

    void FindStopPosition()
    {
        float leftDownOffset = 0.8f;
        BoxCollider2D playerCol = gameObject.GetComponent<BoxCollider2D>();
        Vector2 playerColCenter = playerCol.bounds.center;
        Vector2 raycastDirection = new Vector2(cardinalDirection.x, cardinalDirection.y);
        RaycastHit2D rayHit = Physics2D.Raycast(playerColCenter, raycastDirection);

        if (rayHit)
        {
            Vector2 strikePoint = rayHit.point;
            if (cardinalDirection.x == -1)
            {
                strikePoint.x -= leftDownOffset;
            }
            if (cardinalDirection.y == -1)
            {
                strikePoint.y -= leftDownOffset;
            }
            //Debug.DrawLine(playerColCenter, rayHit.point, Color.black, 5f);

            Vector3 cellPos = grid.WorldToCell(strikePoint);
            Vector3Int intCellPos = Vector3Int.FloorToInt(cellPos);
            Vector3 cellCenter = grid.GetCellCenterWorld(intCellPos);

            //Debug.DrawLine(playerColCenter, cellCenter, Color.red, 5f);

            if (rayHit.transform.CompareTag("Terrain"))
            {
                finalPos = new Vector2(cellCenter.x - raycastDirection.x, cellCenter.y - raycastDirection.y);
                //Debug.DrawLine(playerColCenter, finalPos, Color.green, 5f)
                //print("TERRAIN BLOCK: " + cellCenter + ", Player Destination: " + finalPos + ", CURRENT POSITION: " + player.transform.position);
            }
            else if (rayHit.transform.CompareTag("Enemy"))
            {
                finalPos = cellCenter;
                Debug.DrawLine(playerColCenter, finalPos, Color.green, 5f);
            }
        }
    }

    //Isolates movement from update function, preventing extra input and forcing movement to grid.
    IEnumerator Move()
    {
        isPlayerMoving = true;
        CalcMoveTarget();
        FindStopPosition();
        yield return new WaitUntil(IsMovementComplete);
    }
}