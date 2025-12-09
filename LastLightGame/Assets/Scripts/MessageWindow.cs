using UnityEngine;
using TMPro;
using System.Collections;

public class MessageWindow : MonoBehaviour
{
    public static MessageWindow Instance;

    [Header("UI References")]
    public GameObject window;               // The panel object
    public TextMeshProUGUI messageText;     // The TMP text inside the panel
    public float fadeTime = 2f;             // How long the message stays up

    private Coroutine currentRoutine;

    void Awake()
    {
        // Singleton-style access
        Instance = this;

        if (window != null)
            window.SetActive(false);
    }

    // --------------------------
    // PICKUP MESSAGE ("Picked up Water!")
    // --------------------------
    public void ShowPickup(string itemName)
    {
        ShowMessage("Picked up " + itemName + "!");
    }

    // --------------------------
    // GENERIC MESSAGE ("You starved to death!")
    // --------------------------
    public void ShowMessage(string message)
    {
        if (currentRoutine != null)
            StopCoroutine(currentRoutine);

        currentRoutine = StartCoroutine(ShowRoutine(message));
    }

    private IEnumerator ShowRoutine(string msg)
    {
        if (messageText != null)
            messageText.text = msg;

        if (window != null)
            window.SetActive(true);

        yield return new WaitForSeconds(fadeTime);

        if (window != null)
            window.SetActive(false);

        currentRoutine = null;
    }

    // --------------------------
    // PROMPT ("Press E to pick up Water")
    // --------------------------
    public void ShowPrompt(string itemName)
    {
        ShowMessage("Press E to pick up " + itemName);
    }

    public void HidePrompt()
    {
        if (currentRoutine != null)
        {
            StopCoroutine(currentRoutine);
            currentRoutine = null;
        }

        if (window != null)
            window.SetActive(false);
    }
}
