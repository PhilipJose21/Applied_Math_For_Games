using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class PlayerRunner : MonoBehaviour
{
    public ItemSpawner spawner;

    [Header("Lane movement")]
    public float moveSpeed = 12f;

    [Header("Jump")]
    public float jumpSpeed = 9f;
    public float gravity = 25f;

    [Header("Look")]
    public float scaleMultiplier = 2.4f;

    [Header("Rotation")]
    public float maxLeanAngle = 20f;
    public float leanPerUnit = 6f;
    public float maxJumpTilt = -25f;
    public float rotationSmoothing = 10f;

    private float hitDepth = 40f;
    private bool autoHitbox = true;
    private float hitboxScale = 0.8f;
    private float manualHalfWidth = 0.6f;
    private float manualHalfHeight = 0.6f;

    [Header("Health")]
    public float maxHP = 100f;
    public float hitDamage = 5f;
    public float regenPerSecond = 1f;

    [Header("UI")]
    public Slider hpSlider;
    public GameObject gameOverScene;

    int lane = 1;
    float currentX;
    float groundY;
    float jumpHeight;
    float verticalVel;
    float hp;
    float hitTimer;
    bool gameOver;
    float laneAngle;
    float jumpAngle;
    float currentLean;
    float currentPitch;
    SpriteRenderer sr;

    Vector3 WorldPos
    {
        get { return new Vector3(currentX, groundY + jumpHeight, 0f); }
    }

    float PlayerHalfWidth()
    {
        if (autoHitbox && sr != null && sr.sprite != null)
            return sr.sprite.bounds.extents.x * Mathf.Abs(transform.localScale.x) * hitboxScale;
        return manualHalfWidth;
    }

    float PlayerHalfHeight()
    {
        if (autoHitbox && sr != null && sr.sprite != null)
            return sr.sprite.bounds.extents.y * Mathf.Abs(transform.localScale.y) * hitboxScale;
        return manualHalfHeight;
    }

    float LaneX(int l)
    {
        return spawner.spawnPoints[l].position.x;
    }

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        transform.localScale = transform.localScale * scaleMultiplier;
        hp = maxHP;
        groundY = spawner.groundY;
        currentX = LaneX(lane);
        if (gameOverScene != null) gameOverScene.SetActive(false);
        if (hpSlider != null) { hpSlider.minValue = 0f; hpSlider.maxValue = maxHP; }
    }

    void Update()
    {
        if (gameOver)
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                Time.timeScale = 1f;
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
            return;
        }

        laneSwitch();
        jump();
        collisionSystem();
        hpRegen();
        ApplyRotation();

        Vector2 screenPos = CameraComponent.Project(WorldPos);
        transform.position = new Vector3(screenPos.x, screenPos.y, 0f);
        if (sr != null) sr.sortingOrder = CameraComponent.sortingBase + 1000;
        
        UpdateUI();
    }

    public void laneSwitch()
    {
        if (Input.GetKeyDown(KeyCode.A))  
        {
            lane = Mathf.Max(0, lane - 1);
        }
        if (Input.GetKeyDown(KeyCode.D)) 
        {
            lane = Mathf.Min(2, lane + 1);
        }
        rotateObjectToLane(lane);
    }

    void hpRegen()
    {
        hp = Mathf.Min(maxHP, hp + regenPerSecond * Time.deltaTime);
        if (hp > maxHP) hp = maxHP;
        hitTimer = Mathf.Max(0f, hitTimer - Time.deltaTime);

        if (hp <= 0f)
        {
            hp = 0f;
            gameOver = true;
            Time.timeScale = 0f;
            if (gameOverScene != null) {
                gameOverScene.SetActive(true);
            }
        }
    }

    public void jump()
    {
        float targetX = LaneX(lane);
        float t = 1f - Mathf.Exp(-moveSpeed * Time.deltaTime);
        currentX = Mathf.Lerp(currentX, targetX, t);

        bool grounded = jumpHeight <= 0f && verticalVel <= 0f;
        if (grounded && Input.GetKeyDown(KeyCode.Space))
        {
            verticalVel = jumpSpeed;
        }

        verticalVel -= gravity * Time.deltaTime;
        jumpHeight += verticalVel * Time.deltaTime;
        if (jumpHeight <= 0f) 
        { 
            jumpHeight = 0f; 
            verticalVel = 0f; 
        }
        rotateObjectToJump();
    }

    void collisionSystem()
    {
        Vector3 pos = WorldPos;
        float playerHalfW = PlayerHalfWidth();
        float playerHalfH = PlayerHalfHeight();
        for (int i = 0; i < spawner.items.Count; i++)
        {
            Item item = spawner.items[i];
            if (item == null || item.hasHit) continue;

            Vector3 distance = item.itemPosition - pos;
            bool sameDepth = Mathf.Abs(distance.z) < hitDepth;
            float itemHalfW = autoHitbox ? item.HalfWidth * hitboxScale : manualHalfWidth;
            float itemHalfH = autoHitbox ? item.HalfHeight * hitboxScale : manualHalfHeight;
            bool sameLane = Mathf.Abs(distance.x) < playerHalfW + itemHalfW;
            bool tooLow = (pos.y - item.itemPosition.y) < playerHalfH + itemHalfH;

            if (sameDepth && sameLane && tooLow)
            {
                item.hasHit = true;
                Destroy(item.gameObject);
                hp -= hitDamage;
            }
        }
    }

    void UpdateUI()
    {
        if (hpSlider != null) 
        {
            hpSlider.value = hp;
        }
    }

    void rotateObjectToLane(int lane)
    {
        float dx = LaneX(lane) - currentX;
        laneAngle = Mathf.Clamp(-dx * leanPerUnit, -maxLeanAngle, maxLeanAngle);
    }

    void rotateObjectToJump()
    {
        float peakHeight = (jumpSpeed * jumpSpeed) / (2f * gravity);
        float height01 = peakHeight > 0f ? Mathf.Clamp01(jumpHeight / peakHeight) : 0f;
        jumpAngle = height01 * maxJumpTilt;
    }

    void ApplyRotation()
    {
        float t = 1f - Mathf.Exp(-rotationSmoothing * Time.deltaTime);
        currentLean = Mathf.LerpAngle(currentLean, laneAngle, t);
        currentPitch = Mathf.LerpAngle(currentPitch, jumpAngle, t);
        transform.rotation = Quaternion.Euler(currentPitch, 0f, currentLean);
    }
}