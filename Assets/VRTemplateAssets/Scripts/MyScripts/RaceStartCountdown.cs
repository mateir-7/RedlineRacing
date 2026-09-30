using UnityEngine;
using TMPro;
using System.Collections;

public class RaceStartCountdown : MonoBehaviour
{
    public TextMeshProUGUI countdownText;
    public SimpleAICar[] aiCars;
    public SimKartController playerController;
    public float countdownDuration = 10f;

    void Start()
    {
        StartCoroutine(RunCountdown());
    }

    IEnumerator RunCountdown()
    {
        // disable player movement
        if (playerController != null) playerController.inputLocked = true;

        // disable AI cars
        foreach (var ai in aiCars)
            if (ai != null) ai.enabled = false;

        float remaining = countdownDuration;
        while (remaining > 0)
        {
            if (countdownText != null)
                countdownText.text = Mathf.CeilToInt(remaining).ToString();
            remaining -= Time.deltaTime;
            yield return null;
        }

        // show "GO!" briefly, then hide
        if (countdownText != null)
            countdownText.text = "GO!";

        // re-enable AI cars
        foreach (var ai in aiCars)
            if (ai != null) ai.enabled = true;

        // re-enable player movement
        if (playerController != null) playerController.inputLocked = false;

        yield return new WaitForSeconds(1f);
        if (countdownText != null)
            countdownText.text = "";
    }
}