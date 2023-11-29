using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurveMaker : MonoBehaviour
{
    public GameObject prefab;           // Przypisz prefab do tego pola w inspektorze
    public int iloscPrefabow = 10;      // Iloœæ prefabów do umieszczenia
    public Vector3 poczatekLuku = new Vector3(0, 0, 0);  // Wspó³rzêdne pocz¹tku ³uku
    public Vector3 koniecLuku = new Vector3(5, 0, 0);    // Wspó³rzêdne koñca ³uku
    public float wykrzywienie = 2.0f;   // Parametr kontroluj¹cy wykrzywienie


    void Start()
    {
        UstawPrefabyWLocie();
    }

    void UstawPrefabyWLocie()
    {
        for (int i = 0; i < iloscPrefabow; i++)
        {
            float progress = i / (float)(iloscPrefabow - 1); // Postêp od 0 do 1

            // U¿yj funkcji kwadratowej dla równomiernej krzywizny
            float curveAmount = Mathf.Pow(progress * 2 - 1, 2) * wykrzywienie;

            Vector3 pozycja = Vector3.Lerp(poczatekLuku, koniecLuku, progress); // Interpolacja wspó³rzêdnych
            pozycja.x += curveAmount; // Dodaj zakrzywienie wzd³u¿ osi Y

            Instantiate(prefab, pozycja, Quaternion.identity);
        }
    }
}
