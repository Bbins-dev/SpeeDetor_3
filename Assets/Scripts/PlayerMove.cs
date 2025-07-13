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

    [Header("색상 피드백")]
    public SpriteRenderer spriteRenderer; // 플레이어 SpriteRenderer 참조
    public Color comboSuccessColor = Color.green;
    public Color comboFailColor = Color.red;
    public float flashDuration = 0.2f;

    private Rigidbody2D rb;
    private Vector2 moveDir = Vector2.zero;
    private bool isMoving = false;
    private Vector2 blockedDir = Vector2.zero;
    private Vector3 startPosition;

    private float lastWallHitTime = -999f;
    private Color originalColor;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0;
        startPosition = transform.position;

        if (spriteRenderer == null)
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        originalColor = spriteRenderer.color;
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

        // 🔽 콤보 판정 & 피드백 처리
        HandleComboTiming();

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
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            lastWallHitTime = Time.time;
        }
    }

    // 🔷 콤보 타이밍 판정 및 속도/색상 처리
    void HandleComboTiming()
    {
        float timeSinceWall = Time.time - lastWallHitTime;

        if (timeSinceWall <= ComboWindow)
        {
            Speed += SpeedIncrease;
            Speed = Mathf.Min(Speed, MaxSpeed);
            StartCoroutine(FlashColor(comboSuccessColor));
        }
        else
        {
            Speed -= SpeedPenalty;
            Speed = Mathf.Max(Speed, MinSpeed);
            StartCoroutine(FlashColor(comboFailColor));
        }
    }

    // 🔷 색상 변경 후 원상 복귀 처리
    IEnumerator FlashColor(Color flashColor)
    {
        spriteRenderer.color = flashColor;
        yield return new WaitForSeconds(flashDuration);
        spriteRenderer.color = originalColor;
    }
}
