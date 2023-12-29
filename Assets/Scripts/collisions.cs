using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class collisions : MonoBehaviour
{


    public float displayTime = 2.0f; // Czas, przez kt�ry informacja b�dzie wy�wietlana
    public GameObject infoPanel; // Panel z informacj�

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
        // Wy�wietl informacj� przez okre�lony czas
        ShowInfoPanel();
    }

    void ShowInfoPanel()
    {
        if (infoPanel != null)
        {
            // W��cz panel z informacj�
            infoPanel.SetActive(true);

            // Rozpocznij odliczanie czasu
            StartCoroutine(HideInfoPanel());
        }
    }

    System.Collections.IEnumerator HideInfoPanel()
    {
        // Poczekaj przez okre�lony czas
        yield return new WaitForSeconds(displayTime);

        // Wy��cz panel z informacj� po up�ywie czasu
        if (infoPanel != null)
        {
            infoPanel.SetActive(false);
        }
    }
}
