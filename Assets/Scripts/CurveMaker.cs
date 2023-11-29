using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurveMaker : MonoBehaviour
{
    public GameObject prefab;           // Przypisz prefab do tego pola w inspektorze
    public int iloscPrefabow = 10;      // Ilo�� prefab�w do umieszczenia
    public Vector3 poczatekLuku = new Vector3(0, 0, 0);  // Wsp�rz�dne pocz�tku �uku
    public Vector3 koniecLuku = new Vector3(5, 0, 0);    // Wsp�rz�dne ko�ca �uku
    public float wykrzywienie = 2.0f;   // Parametr kontroluj�cy wykrzywienie


    void Start()
    {
        UstawPrefabyWLocie();
    }

    void UstawPrefabyWLocie()
    {
        for (int i = 0; i < iloscPrefabow; i++)
        {
            float progress = i / (float)(iloscPrefabow - 1); // Post�p od 0 do 1

            // U�yj funkcji kwadratowej dla r�wnomiernej krzywizny
            float curveAmount = Mathf.Pow(progress * 2 - 1, 2) * wykrzywienie;

            Vector3 pozycja = Vector3.Lerp(poczatekLuku, koniecLuku, progress); // Interpolacja wsp�rz�dnych
            pozycja.x += curveAmount; // Dodaj zakrzywienie wzd�u� osi Y

            Instantiate(prefab, pozycja, Quaternion.identity);
        }
    }
}
