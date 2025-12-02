using UnityEngine;
using TMPro;
using System.Collections;

public class MessageWindow : MonoBehaviour
{
    public static MessageWindow Instance;

    public GameObject window;              // The panel object
    public TextMeshProUGUI messageText;    // Text inside the panel
    public float pickupMessageTime = 2f;   // How long to show "Picked up ..." message

    private bool showingPickup = false;    // To avoid the prompt fighting with the pickup message

    void Awake()
    {
        Instance = this;

        if (window == null)
            Debug.LogError("MessageWindow: 'window' is not assigned in the Inspector!");
        if (messageText == null)
            Debug.LogError("MessageWindow: 'messageText' is not assigned in the Inspector!");

        if (window != null)
            window.SetActive(false);   // Start hidden
    }

    // Called while you're close: show "Press E to pick up X"
    public void ShowPrompt(string itemName)
    {
        if (window == null || messageText == null) return;
        if (showingPickup) return;  // Don't override the "Picked up" message

        window.SetActive(true);
        messageText.text = $"Press E to pick up {itemName}";
    }

    // Called when you walk away from the item
    public void HidePrompt()
    {
        if (window == null) return;
        if (showingPickup) return;  // Don't hide if we're in the middle of a pickup message

        window.SetActive(false);
    }

    // Called when you actually pick up the item: show "Picked up X!"
    public void ShowPickup(string itemName)
    {
        if (window == null || messageText == null) return;

        StopAllCoroutines();
        StartCoroutine(ShowPickupRoutine(itemName));
    }

    private IEnumerator ShowPickupRoutine(string itemName)
    {
        showingPickup = true;

        window.SetActive(true);
        messageText.text = $"Picked up {itemName}!";

        yield return new WaitForSeconds(pickupMessageTime);

        showingPickup = false;
        window.SetActive(false);
    }
}
