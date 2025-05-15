using UnityEngine;
using System.Collections;

public class GameObjectAndLightSwitcher : MonoBehaviour
{
    [Header("GameObject Switching")]
    public GameObject[] gameObjects;       // GameObjects to show/hide
    public float[] displayTimes;           // Duration for each GameObject to be active
    public float switchInterval = 1f;      // Time between cycles

    [Header("Light Timing")]
    public Light controlledLight;
    public float lightTurnOnTime = 3f;     // When the light should turn ON (in seconds after start)
    public float lightOnDuration = 2f;     // How long the light stays ON before turning OFF

    void Start()
    {
        if (gameObjects.Length != displayTimes.Length)
        {
            Debug.LogError("The number of GameObjects must match the number of display times.");
            return;
        }

        if (controlledLight != null)
        {
            controlledLight.enabled = false; // Start with light off
            StartCoroutine(HandleLight());
        }

        StartCoroutine(SwitchGameObjectsLoop());
    }

    IEnumerator HandleLight()
    {
        // Wait until it's time to turn on the light
        yield return new WaitForSeconds(lightTurnOnTime);

        controlledLight.enabled = true;

        // Wait for duration the light should be on
        yield return new WaitForSeconds(lightOnDuration);

        controlledLight.enabled = false;
    }

    IEnumerator SwitchGameObjectsLoop()
    {
        while (true)
        {
            for (int i = 0; i < gameObjects.Length; i++)
            {
                gameObjects[i].SetActive(true);
                yield return new WaitForSeconds(displayTimes[i]);
                gameObjects[i].SetActive(false);
            }

            yield return new WaitForSeconds(switchInterval);
        }
    }
}
