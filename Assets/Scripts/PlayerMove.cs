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

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0;
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

        // 이전에 막힌 방향이면 무시
        if (inputDir == blockedDir) return;

        moveDir = inputDir;
        blockedDir = Vector2.zero; // 이동 성공 시 제한 해제
        isMoving = true;
        rb.velocity = moveDir * Speed;
    }

    void FixedUpdate()
    {
        // 이동 중인데 속도가 거의 0이면, 부딪혔다고 판단
        if (isMoving && rb.velocity.magnitude < 0.01f)
        {
            isMoving = false;
            blockedDir = moveDir; // 부딪힌 방향 저장
        }
    }
}
