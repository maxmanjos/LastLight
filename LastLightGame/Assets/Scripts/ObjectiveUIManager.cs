using UnityEngine;
using TMPro;

public class ObjectivesUIManager : MonoBehaviour
{
    public static ObjectivesUIManager Instance;

    [Header("Objective Lines")]
    public TMP_Text batteryLine;
    public TMP_Text radioLine;
    public TMP_Text circuitBoardLine;
    public TMP_Text wiresLine;

    // Track which objectives are collected
    private bool hasBattery = false;
    private bool hasRadio = false;
    private bool hasCircuitBoard = false;
    private bool hasWires = false;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        if (batteryLine != null)        batteryLine.text = "[ ] Battery";
        if (radioLine != null)          radioLine.text = "[ ] Radio";
        if (circuitBoardLine != null)   circuitBoardLine.text = "[ ] Circuit Board";
        if (wiresLine != null)          wiresLine.text = "[ ] Wires";
    }

    public void MarkCollected(ObjectiveItemType type)
    {
        switch (type)
        {
            case ObjectiveItemType.Battery:
                hasBattery = true;
                if (batteryLine != null)
                    batteryLine.text = "[X] Battery";
                break;

            case ObjectiveItemType.Radio:
                hasRadio = true;
                if (radioLine != null)
                    radioLine.text = "[X] Radio";
                break;

            case ObjectiveItemType.CircuitBoard:
                hasCircuitBoard = true;
                if (circuitBoardLine != null)
                    circuitBoardLine.text = "[X] Circuit Board";
                break;

            case ObjectiveItemType.Wires:
                hasWires = true;
                if (wiresLine != null)
                    wiresLine.text = "[X] Wires";
                break;
        }
    }

    public bool AllObjectivesComplete()
    {
        return hasBattery && hasRadio && hasCircuitBoard && hasWires;
    }
}
