using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurveMaker : MonoBehaviour
{
    public GameObject prefab; // Prefab do utworzenia
    public Transform startPoint; // Pocz¹tkowy punkt krzywej
    public Transform endPoint; // Koñcowy punkt krzywej
    public int numberOfPrefabs = 10; // Liczba prefabów do utworzenia
    public float curvature = 5.0f; // Parametr kontroluj¹cy krzywiznê

    void Start()
    {
        SpawnPrefabsOnCurve();
    }

    void SpawnPrefabsOnCurve()
    {
        Vector3[] curvePoints = CalculateEvenlySpacedPointsOnCurve();

        foreach (Vector3 position in curvePoints)
        {
            // Dodaj przesuniêcie w osi Y
            Vector3 adjustedPosition = position;

            // Utwórz instancjê prefabu na obliczonym punkcie
            Instantiate(prefab, adjustedPosition, Quaternion.identity);
        }
    }

    // Funkcja obliczaj¹ca punkty na krzywej Bezier'a równomiernie roz³o¿one
    Vector3[] CalculateEvenlySpacedPointsOnCurve()
    {
        int resolution = numberOfPrefabs * 10; // Wartoœæ mo¿esz dostosowaæ
        Vector3[] points = new Vector3[resolution + 1];
        float totalLength = 0f;

        for (int i = 0; i < resolution; i++)
        {
            float t1 = i / (float)resolution;
            float t2 = (i + 1) / (float)resolution;

            Vector3 p1 = CalculateBezierPoint(t1, startPoint.position, startPoint.position + new Vector3(curvature, 0, 0), endPoint.position);
            Vector3 p2 = CalculateBezierPoint(t2, startPoint.position, startPoint.position + new Vector3(curvature, 0, 0), endPoint.position);

            totalLength += Vector3.Distance(p1, p2);
        }

        float stepSize = totalLength / numberOfPrefabs;
        float currentDistance = 0f;
        int currentPointIndex = 0;

        for (int i = 0; i < resolution; i++)
        {
            float t1 = i / (float)resolution;
            float t2 = (i + 1) / (float)resolution;

            Vector3 p1 = CalculateBezierPoint(t1, startPoint.position, startPoint.position + new Vector3(curvature, 0, 0), endPoint.position);
            Vector3 p2 = CalculateBezierPoint(t2, startPoint.position, startPoint.position + new Vector3(curvature, 0, 0), endPoint.position);

            float segmentLength = Vector3.Distance(p1, p2);

            if (currentDistance + segmentLength >= stepSize)
            {
                float overshoot = currentDistance + segmentLength - stepSize;
                Vector3 interpolatedPoint = Vector3.Lerp(p1, p2, 1 - overshoot / segmentLength);
                points[currentPointIndex] = interpolatedPoint;
                currentDistance = overshoot;
                currentPointIndex++;
            }
            else
            {
                currentDistance += segmentLength;
            }
        }

        points[numberOfPrefabs] = CalculateBezierPoint(1.0f, startPoint.position, startPoint.position + new Vector3(curvature, 0, 0), endPoint.position); // Dodaj ostatni punkt
        return points;
    }

    // Funkcja obliczaj¹ca punkt na krzywej Bezier'a dla danego parametru t
    Vector3 CalculateBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2)
    {
        float u = 1 - t;
        float tt = t * t;
        float uu = u * u;
        float uuu = uu * u;
        float ttt = tt * t;

        // Wzór punktu na krzywej Bezier'a
        Vector3 p = uuu * p0;
        p += 3 * uu * t * p1;
        p += 3 * u * tt * p2;
        p += ttt * endPoint.position;

        return p;
    }
}