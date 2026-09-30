using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

public class DashboardUI : MonoBehaviour
{
    [Header("UI Elements (Drag from Hierarchy)")]
    public TextMeshProUGUI speedText;
    public TextMeshProUGUI brakeText;
    public TextMeshProUGUI gearText;
    public TextMeshProUGUI gearNumber;

    [Header("Car Data")]
    public Rigidbody carRigidbody;
    public Transform gearShift;
    public InputActionReference brakeButton;
    
    public SimKartController kartController; 

    [Header("Custom Colors")]
    public Color driveColor = Color.blue;
    public Color revColor = new Color(0.7f, 0.5f, 1f); // light purple
    public Color brakeOffColor = Color.green;
    public Color brakeOnColor = Color.red;

    private float displayedSpeed = 0f;

    void Update()
    {
        if (speedText == null || brakeText == null || gearText == null) return;

        // update speed
        float targetRawSpeed = carRigidbody.linearVelocity.magnitude * 3.6f;
        displayedSpeed = Mathf.Lerp(displayedSpeed, targetRawSpeed, Time.deltaTime * 5f);
        int finalSpeedUI = Mathf.RoundToInt(displayedSpeed);
        speedText.text = finalSpeedUI.ToString() + "\nKM/H";

        // update gear state and color
        float gearAngle = gearShift.localEulerAngles.x;
        if (gearAngle > 180) gearAngle -= 360;

        if (gearAngle >= -10f)
        {
            gearText.text = "Drive";
            gearText.color = driveColor;
            gearNumber.text = "Gear " + kartController.currentGear.ToString();
            gearNumber.color = driveColor;
        }
        else
        {
            gearText.text = "Rev";
            gearText.color = revColor;
            gearNumber.text = "R";
            gearNumber.color = revColor;
        }

        // update brake state and color
        float brakeInput = brakeButton.action.ReadValue<float>();
        if (brakeInput > 0.1f)
        {
            brakeText.text = "-";
            brakeText.color = brakeOnColor;
        }
        else
        {
            brakeText.text = "-";
            brakeText.color = brakeOffColor;
        }
    }
}