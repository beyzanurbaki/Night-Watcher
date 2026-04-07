using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Hareket Ayarlarý")]
    public float moveSpeed = 5f;

    [Header("Etkileþim Ayarlarý")]
    public float interactionRange = 1.5f;
    public LayerMask npcLayer;

    private Rigidbody2D rb;
    private Vector2 movement;
    private GameObject nearbyNPC; // Yakýndaki NPC

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // WASD giriþini al
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");

        // Yakýndaki NPC'yi kontrol et
        CheckNearbyNPC();

        // E tuþu ile etkileþim
        if (Input.GetKeyDown(KeyCode.E) && nearbyNPC != null)
        {
            Interact();
        }
    }

    void FixedUpdate()
    {
        // Fizik bazlý hareket
        rb.MovePosition(rb.position + movement.normalized * moveSpeed * Time.fixedDeltaTime);
    }

    void CheckNearbyNPC()
    {
        // Oyuncunun etrafýnda daire içinde NPC var mý?
        Collider2D hit = Physics2D.OverlapCircle(transform.position, interactionRange, npcLayer);

        if (hit != null)
        {
            nearbyNPC = hit.gameObject;
            Debug.Log("NPC yakýnda: " + nearbyNPC.name); // Console'da göster
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
}

    