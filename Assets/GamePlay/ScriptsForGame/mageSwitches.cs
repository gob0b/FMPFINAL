using UnityEngine;
using System.Collections;

public class ImageSwitches : MonoBehaviour
{
    // Public variables to assign GameObject components
    public GameObject[] gameObjects; // Array to hold multiple GameObjects

    // Array of times for each GameObject to be active
    public float[] displayTimes; // Display times for each GameObject

    // Time for both GameObjects to be inactive before switching
    public float switchInterval = 1f;

    // Start is called before the first frame update
    void Start()
    {
        // Ensure the number of display times matches the number of GameObjects
        if (gameObjects.Length != displayTimes.Length)
        {
            Debug.LogError("The number of GameObjects and display times do not match!");
            return;
        }

        // Start the coroutine to switch GameObjects
        StartCoroutine(SwitchGameObjects());
    }

    // Coroutine to handle switching GameObjects
    IEnumerator SwitchGameObjects()
    {
        while (true)
        {
            // Loop through each GameObject and its corresponding display time
            for (int i = 0; i < gameObjects.Length; i++)
            {
                // Make the current GameObject active
                gameObjects[i].SetActive(true);

                // Wait for the display time for the current GameObject
                yield return new WaitForSeconds(displayTimes[i]);

                // Make the current GameObject inactive
                gameObjects[i].SetActive(false);
            }

            // After all GameObjects have been displayed, wait for the switch interval
            yield return new WaitForSeconds(switchInterval);
        }
    }
}
