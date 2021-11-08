using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomSolarSystem : MonoBehaviour
{
    [SerializeField] private GameObject sun;
    [SerializeField] private List<GameObject> planetPrefabs;

    void Start()
    {
        int planetCount = Random.Range(3, 5);

        Camera.main.orthographicSize = 5f;
        Vector2 currMinScreenEdge = ScreenInfo.GetCurrMinScreenEdge();
        Vector2 currMaxScreenEdge = ScreenInfo.GetCurrMaxScreenEdge();
        float maxWidth = (currMaxScreenEdge.x - currMinScreenEdge.x) / 6.3f;
        float minWidth = (currMaxScreenEdge.x - currMinScreenEdge.x) / 6.5f;
        float maxHeight = (currMaxScreenEdge.y - currMinScreenEdge.y) / 6.3f;
        float minHeight = (currMaxScreenEdge.y - currMinScreenEdge.y) / 6.5f; 

        for (int i = 0; i < planetCount; i++)
        {
            GameObject planet = planetPrefabs[Random.Range(0, planetPrefabs.Count - 1)];
            planetPrefabs.Remove(planet);
            planet.gameObject.transform.position = sun.transform.position;
            //if (!planet.activeSelf) planet.SetActive(true);
            EllipticalMovement ellipse = planet.GetComponent<EllipticalMovement>();
            if (planet.activeSelf) ellipse.gameObject.SetActive(false);
            float angle = Random.Range(0, 360f / planetCount) + i * 360f / planetCount;
            
            float b = (180 - angle % 180) > 135 || (180 - angle % 180) < 45 ? Random.Range(minWidth, maxWidth) :
                Random.Range(minHeight, maxHeight);
            if (maxWidth - b < b - minWidth) // если у нас орбита ближе к максимуму, чем к минимуму
                if (angle % 180 > 45 && angle % 180 < 135) //если орбита больше вертикальная, чем горизонтальная
                    angle = angle % 180 <= 90 ? angle - 45 : angle + 45;
            float a = b * 2;
            ellipse.SetProperties(a, b, angle, Random.Range(-2.5f, 2.5f));
            ellipse.gameObject.SetActive(true);
            ellipse.Draw();
        }
    }
}
