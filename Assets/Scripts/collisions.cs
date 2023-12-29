using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class collisions : MonoBehaviour
{


    public float displayTime = 2.0f; // Czas, przez który informacja bêdzie wyœwietlana
    public GameObject infoPanel; // Panel z informacj¹

    // Start is called before the first frame update
    void Start()
    {
        if (infoPanel != null)
        {
            infoPanel.SetActive(false);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        // Wyœwietl informacjê przez okreœlony czas
        ShowInfoPanel();
    }

    void ShowInfoPanel()
    {
        if (infoPanel != null)
        {
            // W³¹cz panel z informacj¹
            infoPanel.SetActive(true);

            // Rozpocznij odliczanie czasu
            StartCoroutine(HideInfoPanel());
        }
    }

    System.Collections.IEnumerator HideInfoPanel()
    {
        // Poczekaj przez okreœlony czas
        yield return new WaitForSeconds(displayTime);

        // Wy³¹cz panel z informacj¹ po up³ywie czasu
        if (infoPanel != null)
        {
            infoPanel.SetActive(false);
        }
    }
}
