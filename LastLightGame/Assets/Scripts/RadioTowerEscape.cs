using UnityEngine;

public class RadioTowerEscape : MonoBehaviour
{
    public float interactDistance = 3f;   // how close player must be to use the tower

    private Transform player;
    private bool promptShown = false;

    void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj == null)
        {
            Debug.LogError("RadioTowerEscape: No GameObject with tag 'Player' found!");
            enabled = false;
            return;
        }

        player = playerObj.transform;
    }

    void Update()
    {
        if (player == null) return;
        if (ObjectivesUIManager.Instance == null) return;

        float distance = Vector3.Distance(transform.position, player.position);
        bool allItemsCollected = ObjectivesUIManager.Instance.AllObjectivesComplete();

        // Only do anything if all objectives are done
        if (!allItemsCollected)
        {
            // Hide prompt if we were showing it
            if (promptShown && MessageWindow.Instance != null)
            {
                MessageWindow.Instance.HidePrompt();
                promptShown = false;
            }
            return;
        }

        // Player close enough to interact
        if (distance <= interactDistance)
        {
            // Show prompt once
            if (!promptShown && MessageWindow.Instance != null)
            {
                MessageWindow.Instance.ShowPrompt("Press E to fix the radio tower and escape");
                promptShown = true;
            }

            // Press E to escape
            if (Input.GetKeyDown(KeyCode.E))
            {
                Escape();
            }
        }
        else
        {
            // Out of range – hide prompt
            if (promptShown && MessageWindow.Instance != null)
            {
                MessageWindow.Instance.HidePrompt();
                promptShown = false;
            }
        }
    }

    void Escape()
    {
        Debug.Log("RadioTowerEscape: Player escaped!");

        // Show victory message
        if (MessageWindow.Instance != null)
        {
            MessageWindow.Instance.ShowMessage("You fixed the radio tower and escaped!");
        }

        // Simple "freeze" for playtest so they know it's over
        Time.timeScale = 0f;

        // TODO later: load a win screen scene, show UI, etc.
    }
}

