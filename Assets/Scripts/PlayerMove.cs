using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    public float Speed = 100f;

    private Rigidbody2D rb;
    private Vector2 moveDir = Vector2.zero;
    private bool isMoving = false;

    private Vector2 blockedDir = Vector2.zero;
    private Vector3 startPosition;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0;

        startPosition = transform.position; // 시작 위치 저장
    }

    void Update()
    {
        if (isMoving) return;

        Vector2 inputDir = Vector2.zero;

        if (Input.GetKeyDown(KeyCode.T)) inputDir = Vector2.up;
        else if (Input.GetKeyDown(KeyCode.G)) inputDir = Vector2.down;
        else if (Input.GetKeyDown(KeyCode.J)) inputDir = Vector2.left;
        else if (Input.GetKeyDown(KeyCode.K)) inputDir = Vector2.right;
        else return;

        if (inputDir == blockedDir) return;

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
            Debug.Log("Deadline 충돌 감지됨"); // ← 테스트 로그용
            // 🔧 임시 처리: 시작 위치로 리셋
            transform.position = startPosition;
            rb.velocity = Vector2.zero;
            isMoving = false;
            blockedDir = Vector2.zero;

            // 나중에 아래 라인을 게임 오버 UI로 교체
            // GameOverUI.Show();
        }
    }
}
