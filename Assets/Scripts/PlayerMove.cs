using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    [Header("이동 속도 조절")]
    public float Speed = 100f;
    public float MinSpeed = 20f;
    public float MaxSpeed = 300f;
    public float SpeedIncrease = 50f;
    public float SpeedPenalty = 50f;

    [Header("콤보 입력 유예 시간")]
    public float ComboWindow = 0.2f;

    private Rigidbody2D rb;
    private Vector2 moveDir = Vector2.zero;
    private bool isMoving = false;
    private Vector2 blockedDir = Vector2.zero;
    private Vector3 startPosition;

    private float lastWallHitTime = -999f; // 벽 충돌 시점 기록

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0;
        startPosition = transform.position;
    }

    void Update()
    {
        if (isMoving) return;

        Vector2 inputDir = Vector2.zero;

        if (Input.GetKeyDown(KeyCode.UpArrow)) inputDir = Vector2.up;
        else if (Input.GetKeyDown(KeyCode.DownArrow)) inputDir = Vector2.down;
        else if (Input.GetKeyDown(KeyCode.LeftArrow)) inputDir = Vector2.left;
        else if (Input.GetKeyDown(KeyCode.RightArrow)) inputDir = Vector2.right;
        else return;

        if (inputDir == blockedDir) return;

        // 콤보 입력 반응 속도 측정
        float timeSinceLastWall = Time.time - lastWallHitTime;
        if (timeSinceLastWall <= ComboWindow)
        {
            Speed += SpeedIncrease;
            Speed = Mathf.Min(Speed, MaxSpeed);
        }
        else
        {
            Speed -= SpeedPenalty;
            Speed = Mathf.Max(Speed, MinSpeed);
        }

        moveDir = inputDir;
        blockedDir = Vector2.zero;
        isMoving = true;
        rb.velocity = moveDir * Speed;
    }

    void FixedUpdate()
    {
        if (isMoving && rb.velocity.magnitude < 0.01f)
        {
            isMoving = false;
            blockedDir = moveDir;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Deadline"))
        {
            Debug.Log("Deadline 충돌 감지됨");
            transform.position = startPosition;
            rb.velocity = Vector2.zero;
            isMoving = false;
            blockedDir = Vector2.zero;

            // GameOverUI.Show(); ← 추후 UI 연결 예정
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            lastWallHitTime = Time.time;
        }
    }
}
