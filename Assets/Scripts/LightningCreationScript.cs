using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LightningCreationScript : MonoBehaviour 
{
    public int iterationsNumber;
    public float difference;
    public BoxCollider2D spawnPos;
    public float xDif;
    public float minTimeDif;
    public float maxTimeDif;
    public float startWidth;
    public float endWidth;
    public float branchingChance;
    public float branchingDifference;
    public float branchingSmallerLength;
    public float branchingSmallerWidth;
    public float branchingSmallerDif;
    public float branchingSmallerIter;

    public GameObject lightningPrefab;
    public GameObject lightPrefab;

    float timer;

	void GenerateLightning(Vector3 start, Vector3 end, int iterationCount, float local_dif) 
    {
        List<Vector3> positions = new List<Vector3>();
        positions.Add(start);
        positions.Add(end);

        for (int i = 1; i <= iterationCount; i++)
        {
            for (int j = 0; j < positions.Count - 1; j += 2)
            {
                Vector2 oldVect = positions[j + 1] - positions[j];
                Vector2 normal = new Vector2(-oldVect.y, oldVect.x).normalized;
                float dif = Random.Range(-local_dif, local_dif);
                Vector2 newPos = (positions[j] + positions[j + 1]) / 2;
                newPos += normal * dif / i;
                positions.Insert(j + 1, new Vector3(newPos.x, newPos.y, 
                    (positions[j].z + positions[j + 1].z) / 2));
            }
        }

        for (int i = 0; i < positions.Count - 1; i++)
        {
            Vector2 posDif = positions[i + 1] - positions[i];

            if ((i != 0) &&
                (Random.value <= branchingChance))
            {
                float modifier = 1 - (float)i / positions.Count;
                float baseAngle = Mathf.Atan2(posDif.y, posDif.x);
                float rand = Random.Range(-branchingDifference, branchingDifference) * Mathf.PI;
                baseAngle += rand;

                Vector2 baseLightning = end - start;
                Vector2 newLightning = new Vector2(Mathf.Cos(baseAngle), Mathf.Sin(baseAngle)) *
                    baseLightning.magnitude * modifier * branchingSmallerLength;

                Vector3 newStart = new Vector3(positions[i].x, positions[i].y, positions[i].z * branchingSmallerWidth);
                Vector3 newEnd = newStart;
                newEnd.x += newLightning.x;
                newEnd.y += newLightning.y;
                newEnd.z = positions[positions.Count - 1].z;

                GenerateLightning(newStart, newEnd,
                    Mathf.CeilToInt(iterationCount * modifier * branchingSmallerIter),
                    local_dif * modifier * branchingSmallerDif);
            }

            Vector2 position = positions[i];
            position += posDif / 2;
            var segment = (GameObject)Instantiate(lightningPrefab,
                new Vector3(position.x, position.y, 0),
                Quaternion.Euler(0, 0, Mathf.Atan2(posDif.y, posDif.x) / Mathf.PI * 180));

            segment.transform.localScale = new Vector2(
                posDif.magnitude, (positions[i].z + positions[i + 1].z) / 2);
        }
	}

    void Update() 
    {
        var endPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        endPoint.z = 0;

        if (Time.time > timer)
        {
            Vector3 startPos = new Vector3(spawnPos.transform.position.x +
                Random.Range(-spawnPos.bounds.extents.x, spawnPos.bounds.extents.x),
                spawnPos.transform.position.y +
                Random.Range(-spawnPos.bounds.extents.y, spawnPos.bounds.extents.y),
                startWidth);
            GenerateLightning(startPos, new Vector3(
                startPos.x + Random.Range(-xDif, xDif), -5, endWidth),
                iterationsNumber, difference);
            Instantiate(lightPrefab, new Vector3(startPos.x, startPos.y, 1.5f), Quaternion.identity);
            timer = Time.time + Random.Range(minTimeDif, maxTimeDif);
        }
	}
}
