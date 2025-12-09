using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    public string itemName = "Water";
    public float pickupDistance = 3f;

    [Header("Survival Effects")]
    public bool restoresHunger = false;
    public float hungerAmount = 25f;
    public bool restoresThirst = false;
    public float thirstAmount = 25f;

    private Transform player;
    private bool inRange = false;
    private MessageWindow msgWindow;
    private PlayerNeeds playerNeeds;

    public bool isObjectiveItem = false;      // Is this part of the radio tower quest?
    public ObjectiveItemType objectiveType;   // Which objective item is it?


    void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj == null)
        {
            Debug.LogError("ItemPickup: No GameObject with tag 'Player' found!");
            enabled = false;
            return;
        }

        player = playerObj.transform;
        playerNeeds = playerObj.GetComponent<PlayerNeeds>();
        if (playerNeeds == null)
        {
            Debug.LogWarning("ItemPickup: Player has no PlayerNeeds component.");
        }

        msgWindow = MessageWindow.Instance;
        if (msgWindow == null)
        {
            Debug.LogError("ItemPickup: No MessageWindow.Instance found in scene!");
        }
    }

    void Update()
    {
        if (player == null || msgWindow == null) return;

        float distance = Vector3.Distance(transform.position, player.position);

        // Enter pickup range
        if (!inRange && distance <= pickupDistance)
        {
            inRange = true;
            msgWindow.ShowPrompt(itemName);
        }
        // Exit pickup range
        else if (inRange && distance > pickupDistance)
        {
            inRange = false;
            msgWindow.HidePrompt();
        }

        // Press E to pick up
        if (inRange && Input.GetKeyDown(KeyCode.E))
        {
            PickUpItem();
        }
    }

    void PickUpItem() 
    {
    Debug.Log("ItemPickup: PickUpItem called for " + itemName);

    // Apply survival effects
    if (playerNeeds != null)
    {
        if (restoresHunger)
            playerNeeds.Eat(hungerAmount);

        if (restoresThirst)
            playerNeeds.Drink(thirstAmount);
    }

    // Show pickup message in window
    if (msgWindow != null)
        msgWindow.ShowPickup(itemName);

    // ✅ NEW: update the checklist if this is a radio tower objective item
    if (isObjectiveItem && ObjectivesUIManager.Instance != null)
    {
        ObjectivesUIManager.Instance.MarkCollected(objectiveType);
    }

    // Destroy item from world
    Destroy(gameObject);
    }

}
