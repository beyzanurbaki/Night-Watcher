using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Hareket Ayarlari")]
    public float moveSpeed = 5f;

    [Header("Etkilesim Ayarlari")]
    public float interactionRange = 1.5f;
    public LayerMask npcLayer;

    [Header("Fener")]
    public GameObject flashlight;

    private Rigidbody2D rb;
    private Vector2 movement;
    private GameObject nearbyNPC;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // Hareket
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");

        // NPC kontrolu
        CheckNearbyNPC();

        // E tusu ile etkilesim
        if (Input.GetKeyDown(KeyCode.E) && nearbyNPC != null)
        {
            Interact();
        }

        // Tab ile hafiza paneli
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            UIManager.Instance.ToggleMemoryPanel();
        }

        // F ile fener ac/kapa
        if (Input.GetKeyDown(KeyCode.F))
        {
            if (flashlight != null)
            {
                flashlight.SetActive(!flashlight.activeSelf);
            }
        }
    }

    void FixedUpdate()
    {
        rb.MovePosition(rb.position + movement.normalized * moveSpeed * Time.fixedDeltaTime);
    }

    void CheckNearbyNPC()
    {
        Collider2D hit = Physics2D.OverlapCircle(transform.position, interactionRange, npcLayer);

        if (hit != null)
        {
            nearbyNPC = hit.gameObject;
        }
        else
        {
            nearbyNPC = null;
        }
    }

    void Interact()
    {
        UIManager.Instance.ShowInteractionMenu(nearbyNPC);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactionRange);
    }
}