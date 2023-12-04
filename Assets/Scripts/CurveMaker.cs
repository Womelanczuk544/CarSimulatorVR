using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurveMaker : MonoBehaviour
{
    public GameObject prefab; // Prefab do utworzenia
    public Transform startPoint; // Pocz�tkowy punkt krzywej
    public Transform endPoint; // Ko�cowy punkt krzywej
    public int numberOfPrefabs = 10; // Liczba prefab�w do utworzenia
    public float curvature = 5.0f; // Parametr kontroluj�cy krzywizn�

    void Start()
    {
        SpawnPrefabsOnCurve();
    }

    void SpawnPrefabsOnCurve()
    {
        Vector3[] curvePoints = CalculateEvenlySpacedPointsOnCurve();

        foreach (Vector3 position in curvePoints)
        {
            // Dodaj przesuni�cie w osi Y
            Vector3 adjustedPosition = position;

            // Utw�rz instancj� prefabu na obliczonym punkcie
            Instantiate(prefab, adjustedPosition, Quaternion.identity);
        }
    }

    // Funkcja obliczaj�ca punkty na krzywej Bezier'a r�wnomiernie roz�o�one
    Vector3[] CalculateEvenlySpacedPointsOnCurve()
    {
        int resolution = numberOfPrefabs * 10; // Warto�� mo�esz dostosowa�
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

    // Funkcja obliczaj�ca punkt na krzywej Bezier'a dla danego parametru t
    Vector3 CalculateBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2)
    {
        float u = 1 - t;
        float tt = t * t;
        float uu = u * u;
        float uuu = uu * u;
        float ttt = tt * t;

        // Wz�r punktu na krzywej Bezier'a
        Vector3 p = uuu * p0;
        p += 3 * uu * t * p1;
        p += 3 * u * tt * p2;
        p += ttt * endPoint.position;

        return p;
    }
}