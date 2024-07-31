using UnityEngine;
using UnityEngine.UI;

public class PlayerController2 : MonoBehaviour
{
    [SerializeField] public float maxJumpForce = 15;
    [SerializeField] public float minSwipeDistance = 20f;
    [SerializeField] public float maxSwipeDistance = 200f;
    [SerializeField] public float swipeIndicatorRadius = 1f;
    float swipeDistance;
    float barSize;
    string terrainType;
    bool canJump;
    bool isGrounded;

    Rigidbody2D rb;
    Vector2 startTouchPosition, endTouchPosition, currentTouchPosition;
    Vector2 swipeVector;

    public Image swipeIndicatorImage;
    private RectTransform swipeIndicator;

    public LayerMask groundLayerMask;
    public LayerMask grassLayerMask;
    public LayerMask sandLayerMask;
    public LayerMask waterLayerMask;

    public float detectionRadius = 0.1f;
    public Transform groundCheck;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        swipeIndicator = swipeIndicatorImage.GetComponent<RectTransform>();
        swipeIndicatorImage.enabled = false;
        barSize = 1f / maxSwipeDistance;
        swipeIndicatorImage.fillAmount = barSize * swipeVector.magnitude;
        canJump = false;
    }

    void Update()
    {
        CheckGroundAndTerrain();

        if (Input.GetMouseButtonDown(0) && isGrounded)
        {
            startTouchPosition = Input.mousePosition;
            swipeIndicatorImage.enabled = true;
            swipeIndicatorImage.fillAmount = 0f;
            canJump = true;
        }

        if (Input.GetMouseButton(0) && isGrounded && canJump)
        {
            currentTouchPosition = Input.mousePosition;
            swipeVector = startTouchPosition - currentTouchPosition;
            swipeDistance = swipeVector.magnitude;

            swipeIndicatorImage.fillAmount = Mathf.Clamp(swipeDistance / maxSwipeDistance, 0, 1);
            swipeIndicatorImage.rectTransform.rotation = Quaternion.Euler(0, 0, (Mathf.Atan2(swipeVector.y, swipeVector.x) * Mathf.Rad2Deg) - 90);
            UpdateIndicatorPosition();
        }

        if (Input.GetMouseButtonUp(0) && isGrounded && canJump)
        {
            endTouchPosition = Input.mousePosition;
            swipeVector = startTouchPosition - endTouchPosition;
            swipeDistance = swipeVector.magnitude;
            Hit();
            swipeIndicatorImage.enabled = false;
            canJump = false;
        }
    }

    void Hit()
    {
        if (swipeDistance >= minSwipeDistance)
        {
            float swipeRatio = Mathf.Clamp(swipeDistance / maxSwipeDistance, 0, 1);
            Vector2 jumpForce = swipeVector.normalized * swipeRatio * maxJumpForce;
            rb.AddForce(jumpForce, ForceMode2D.Impulse);
        }
    }

    void UpdateIndicatorPosition()
    {
        if (swipeIndicatorImage.enabled)
        {
            Vector2 direction = swipeVector.normalized;
            Vector3 offset = new Vector3(direction.x, direction.y, 0) * swipeIndicatorRadius;
            Vector3 worldPositionWithOffset = transform.position + offset;
            Vector3 screenPosition = Camera.main.WorldToScreenPoint(worldPositionWithOffset);
            Vector2 localPoint;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                swipeIndicator.parent as RectTransform,
                screenPosition,
                swipeIndicatorImage.canvas.worldCamera,
                out localPoint
            );
            swipeIndicator.localPosition = localPoint;
        }
    }

    void CheckGroundAndTerrain()
    {
        LayerMask combinedLayerMask = groundLayerMask | grassLayerMask | sandLayerMask | waterLayerMask;

        Collider2D groundCollider = Physics2D.OverlapCircle(groundCheck.position, detectionRadius, combinedLayerMask);

        if (groundCollider != null)
        {
            isGrounded = true;
            DetectTerrainType(groundCollider);
        }
        else
        {
            isGrounded = false;
            terrainType = "None";
        }

        Debug.Log($"Is Grounded: {isGrounded}, Terrain Type: {terrainType}");
    }

    void DetectTerrainType(Collider2D groundCollider)
    {
        int groundLayer = groundCollider.gameObject.layer;

        if (IsInLayerMask(groundLayer, grassLayerMask))
        {
            terrainType = "Grass";
        }
        else if (IsInLayerMask(groundLayer, sandLayerMask))
        {
            terrainType = "Sand";
        }
        else if (IsInLayerMask(groundLayer, waterLayerMask))
        {
            terrainType = "Water";
        }
        else if (IsInLayerMask(groundLayer, groundLayerMask))
        {
            terrainType = "Ground";
        }
        else
        {
            terrainType = "Unknown";
        }
    }

    bool IsInLayerMask(int layer, LayerMask layerMask)
    {
        return (layerMask.value & (1 << layer)) > 0;
    }

    void OnDrawGizmos()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, detectionRadius);
        }
    }
}