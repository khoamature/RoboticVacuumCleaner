using UnityEngine;

public class RobotController2D : MonoBehaviour
{
    public float speed = 5f;
    public bool canMoveDiagonally = true;
    public int maxCollisionBeforeRedirect = 3;
    public float stuckTimeThreshold = 2f;
    public float movementThreshold = 0.05f;

    private Rigidbody2D rb;
    private Vector2 direction;
    private int collisionCount = 0;

    private Vector2 lastPosition;
    private float stuckTimer = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;

        direction = GetRandomDirection();
        RotateRobot(direction.x, direction.y);

        lastPosition = transform.position;
    }

    void FixedUpdate()
    {
        rb.linearVelocity = direction * speed;

        float distanceMoved = Vector2.Distance(transform.position, lastPosition);
        if (distanceMoved < movementThreshold)
        {
            stuckTimer += Time.fixedDeltaTime;
            if (stuckTimer >= stuckTimeThreshold)
            {
                direction = GetRandomDirection();
                RotateRobot(direction.x, direction.y);
                stuckTimer = 0f;
                collisionCount = 0;
            }
        }
        else
        {
            stuckTimer = 0f;
        }

        lastPosition = transform.position;
    }

    private Vector2 GetRandomDirection()
    {
        float angle = Random.Range(0f, 360f);
        return new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad)).normalized;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        Vector2 collisionNormal = collision.contacts[0].normal;
        direction = Vector2.Reflect(direction, collisionNormal).normalized;

        if (!canMoveDiagonally)
        {
            if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
                direction = new Vector2(direction.x, 0).normalized;
            else
                direction = new Vector2(0, direction.y).normalized;
        }

        RotateRobot(direction.x, direction.y);

        collisionCount++;
        if (collisionCount >= maxCollisionBeforeRedirect)
        {
            direction = GetRandomDirection();
            RotateRobot(direction.x, direction.y);
            collisionCount = 0;
        }
    }

    void RotateRobot(float x, float y)
    {
        if (x == 0 && y == 0)
            return;

        float angle = Mathf.Atan2(y, x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }
}
