using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PanelHoverManagerTMP : MonoBehaviour
{
    public List<PanelTextPair> panelTextPairs;
    public float hoverTimeout = 0.5f; // Time to reach the panel from the text

    [System.Serializable]
    public class PanelTextPair
    {
        public TextMeshProUGUI textObject;
        public GameObject panel;
    }

    private int activeIndex = -1;
    private Coroutine hoverDelayCoroutine;

    void Start()
    {
        foreach (var pair in panelTextPairs)
        {
            pair.panel.SetActive(false); // Panels start hidden
        }
    }

    void Update()
    {
        int hoveredTextIndex = GetHoveredTextIndex();

        // Hovering a different text
        if (hoveredTextIndex != -1 && hoveredTextIndex != activeIndex)
        {
            // Hide all panels first
            for (int i = 0; i < panelTextPairs.Count; i++)
            {
                panelTextPairs[i].panel.SetActive(false);
            }

            // Stop existing coroutine
            if (hoverDelayCoroutine != null)
                StopCoroutine(hoverDelayCoroutine);

            // Start timeout check for the hovered panel
            hoverDelayCoroutine = StartCoroutine(ShowPanelWithTimeout(hoveredTextIndex));
        }

        // If mouse has left both active panel and text, hide the active panel
        if (activeIndex != -1)
        {
            var activePair = panelTextPairs[activeIndex];
            bool stillHovering = IsMouseOverText(activePair.textObject) || IsMouseOverPanel(activePair.panel);

            if (!stillHovering)
            {
                activePair.panel.SetActive(false);
                activeIndex = -1;
            }
        }
    }

    IEnumerator ShowPanelWithTimeout(int index)
    {
        var pair = panelTextPairs[index];
        pair.panel.SetActive(true);
        float timer = hoverTimeout;

        while (timer > 0f)
        {
            if (IsMouseOverPanel(pair.panel))
            {
                activeIndex = index;
                yield break; // Keep panel visible
            }

            timer -= Time.deltaTime;
            yield return null;
        }

        // Didn't reach panel in time — hide it
        pair.panel.SetActive(false);
        activeIndex = -1;
    }

    int GetHoveredTextIndex()
    {
        for (int i = 0; i < panelTextPairs.Count; i++)
        {
            if (IsMouseOverText(panelTextPairs[i].textObject))
                return i;
        }
        return -1;
    }

    bool IsMouseOverText(TextMeshProUGUI textObject)
    {
        RectTransform rect = textObject.GetComponent<RectTransform>();
        return RectTransformUtility.RectangleContainsScreenPoint(rect, Input.mousePosition);
    }

    bool IsMouseOverPanel(GameObject panel)
    {
        RectTransform rect = panel.GetComponent<RectTransform>();
        return RectTransformUtility.RectangleContainsScreenPoint(rect, Input.mousePosition);
    }
}
