using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Aim : MonoBehaviour
{
    #region Singleton
    public static Aim Instance { get; private set; }
    #endregion
    [SerializeField] private GameObject point;
    [SerializeField] private Transform leftStartPos;
    [SerializeField] private Transform rightStartPos;
    [SerializeField] private int countOfPoints;
    [SerializeField] private float distanceBetweenPoints;
    private Color32 startColor, endColor, currentColor;
    private Vector3 colorDifference;
    private GameObject pointParent;
    private List<SpriteRenderer> allMasks = new List<SpriteRenderer>();
    public Vector2 position;

    void Start()
    {
        Instance = this;
        startColor = new Color32(255, 0, 255, 255); //purple
        endColor = new Color32(247, 147, 30, 255); //orange
        colorDifference = new Vector3(-8, 147, -225);
        position = leftStartPos.position;
        position.x = leftStartPos.position.x + (rightStartPos.position.x - leftStartPos.position.x) / 2;

        pointParent = point.transform.parent.gameObject;
        for (int i = 0; i < countOfPoints * 2; i++)
        {
            var tmp_mask = Instantiate(point, pointParent.transform);
            var tmp_sprite = tmp_mask.GetComponent<SpriteRenderer>();
            allMasks.Add(tmp_sprite);
            tmp_mask.SetActive(false);
        }
        Destroy(point);
    }

    public void PutAimPoints(Vector2 destination)
    {
        Vector3 shootPos = destination;
        Ray rayLeft = new Ray(leftStartPos.position, shootPos - leftStartPos.position);
        Ray rayRight = new Ray(rightStartPos.position, shootPos - rightStartPos.position);
        float maxDistanceLeft = Vector2.Distance(leftStartPos.position, shootPos);
        float maxDistanceRight = Vector2.Distance(rightStartPos.position, shootPos);
        //maxDistance = Vector2.Distance(leftStartPos.position, shootPos) > maxDistance ? 
        //    Vector2.Distance(leftStartPos.position, shootPos) : maxDistance;

        int i = 0;
        for (; i < countOfPoints; i++)
        {
            float offset = distanceBetweenPoints * i;
            Color currentColor = GetColor(offset, maxDistanceLeft);
            if (offset < maxDistanceLeft)
            {
                allMasks[i].gameObject.transform.position = rayLeft.GetPoint(offset);
                allMasks[i].material.color = currentColor;
                allMasks[i].gameObject.SetActive(true);
            }
            else allMasks[i].gameObject.SetActive(false);
            if (offset > maxDistanceRight)
            {
                allMasks[i + countOfPoints - 1].gameObject.SetActive(false);
                break;
            }

            allMasks[i + countOfPoints - 1].transform.position = rayRight.GetPoint(offset);
            allMasks[i + countOfPoints - 1].color = currentColor;
            allMasks[i + countOfPoints - 1].gameObject.SetActive(true);
        }
        i++;
        allMasks[i].gameObject.transform.position = destination;
        allMasks[i].material.color = endColor;
        allMasks[i].gameObject.SetActive(true);
        allMasks[i + countOfPoints - 1].gameObject.SetActive(false);
        i++;
        for (; i < countOfPoints; i++)
        {
            allMasks[i].gameObject.SetActive(false);
            allMasks[i + countOfPoints - 1].gameObject.SetActive(false);
        }
    }

    private Color32 GetColor(float currentPos, float maxPos)
    {
        float percent = currentPos / maxPos;
        Color tmp = new Color();
        tmp.r = startColor.r + (int)((float)(colorDifference.x) * percent);
        tmp.g = startColor.g + (int)((float)(colorDifference.y) * percent);
        tmp.b = startColor.b + (int)((float)(colorDifference.z) * percent);
        tmp.a = startColor.a;
        tmp.r = tmp.r / 255;
        tmp.g = tmp.g / 255;
        tmp.b = tmp.b / 255;
        tmp.a = tmp.a / 255;
        return tmp;
    }

    public void DeactivateAim()
    {
        for (int i = 0; i < countOfPoints * 2; i++)
        {
            allMasks[i].gameObject.SetActive(false);
        }
    }
}
