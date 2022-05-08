using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerBowlingMovement : MonoBehaviour
{
    private GameObject player;
    public Vector3 newPosition;
    public Vector3 oldPosition;
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
        oldPosition = transform.position;
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
        oldPosition = transform.position;
        //Copy for calculations
        newPosition = transform.position;

        //determine direction player is moving in and set new destination
        if (Input.GetAxis("Vertical") != 0)
        {
            inputPolarity = Mathf.Sign(Input.GetAxis("Vertical"));
            newPosition.y += inputPolarity;
            cardinalDirection = new Vector2(0, inputPolarity);
        }
        else if (Input.GetAxis("Horizontal") != 0)
        {
            inputPolarity = Mathf.Sign(Input.GetAxis("Horizontal"));
            newPosition.x += inputPolarity;
            cardinalDirection = new Vector2(inputPolarity, 0);
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

    void FindStopPosition()
    {
        float leftMoveOffset = 0.8f;
        BoxCollider2D playerCol = gameObject.GetComponent<BoxCollider2D>();
        Vector2 playerColCenter = playerCol.bounds.center;
        Vector2 raycastDirection = new Vector2(cardinalDirection.x, cardinalDirection.y);
        RaycastHit2D rayHit = Physics2D.Raycast(playerColCenter, raycastDirection);

        if (rayHit)
        {
            Vector2 strikePoint = rayHit.point;
            if (cardinalDirection.x == -1)
            {
                strikePoint.x -= leftMoveOffset;
            }
            Debug.DrawLine(playerColCenter, rayHit.point, Color.black, 5f);
            if (rayHit.transform.CompareTag("Terrain"))
            {
                Vector3 cellPos = grid.WorldToCell(strikePoint);
                Vector3Int roundedCellPos = Vector3Int.FloorToInt(strikePoint);
                Vector3 cellCenter = grid.GetCellCenterWorld(roundedCellPos);
                print(cellPos);
                Debug.DrawLine(playerColCenter, cellCenter, Color.red, 5f);
            };
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
        FindStopPosition();
        yield return new WaitUntil(HasPlayerAdvanced);
    }

    //Cancels movement if player collides with terrain
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Terrain"))
        {
            CancelMovement();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            EndMovement();
        };
    }
}