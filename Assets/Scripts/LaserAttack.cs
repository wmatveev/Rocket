using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Old class to move masks and gradient, which made up the dotted line to the aim
/// </summary>
public class LaserAttack : MonoBehaviour
{
    #region Singleton
    public static LaserAttack Instance { get; private set; }
    #endregion
    [SerializeField] private GameObject mask;
    [SerializeField] private Transform leftStartPoint;
    [SerializeField] private Transform rightStartPoint;
    [SerializeField] private int countOfPoints;


    private GameObject maskParent;
    private List<GameObject> allMasks = new List<GameObject>();

    void Start()
    {
        Instance = this;
        maskParent = mask.transform.parent.gameObject;
        top = Vector2.Distance(GameManager.Instance.enemyPlanet.transform.position, gameObject.transform.position);
        maskSize = mask.GetComponent<SpriteMask>().sprite.bounds.size.y * 2;
        Debug.Log(maskSize);
        if (countOfPoints % 2 != 0)
            countOfPoints++;
        for (int i = 0; i < countOfPoints; i++)
        {
            var tmp_mask = Instantiate(mask, maskParent.transform);
            allMasks.Add(tmp_mask);
        }
        Destroy(mask);
        maskParent.SetActive(false);
    }
    float top; float maskSize;
    public void Aim(Vector2 destination)
    {
        //rotation
        Vector2 v1 = GameManager.Instance.enemyPlanet.transform.position - gameObject.transform.position;
        Vector2 pos = gameObject.transform.position;
        Vector2 v2 = destination - pos;
        var angle = Vector2.Angle(v1, v2);
        if (destination.x > gameObject.transform.position.x)
            angle = -angle;
        gameObject.transform.rotation = Quaternion.Euler(0, 0, angle);

        //bg size
        var originY = top;
        var newSizeY = Vector2.Distance(destination, gameObject.transform.position);
        var ratio = newSizeY / originY;
        Vector3 scale = transform.localScale;
        transform.localScale = new Vector3(scale.x, scale.y * ratio, scale.z);
        top = Vector2.Distance(destination, gameObject.transform.position);

        //set masks
        Vector3 shootPos = destination;
        Ray rayLeft = new Ray(leftStartPoint.position, shootPos - leftStartPoint.position);
        int currentLength = countOfPoints / 2;
        float lestDistanceBetweenMasks = Vector2.Distance(destination, leftStartPoint.position) / currentLength;
        Debug.Log(lestDistanceBetweenMasks);
        if (lestDistanceBetweenMasks < maskSize)
        {
            currentLength /= 2;
            lestDistanceBetweenMasks = Vector2.Distance(destination, leftStartPoint.position) / currentLength;
        }
        Ray rayRight = new Ray(rightStartPoint.position, shootPos - rightStartPoint.position);
        float rightDistanceBetweenMasks = Vector2.Distance(destination, rightStartPoint.position) / currentLength;
        Debug.Log(rightDistanceBetweenMasks);
        if (rightDistanceBetweenMasks < maskSize)
        {
            currentLength /= 2;
            rightDistanceBetweenMasks = Vector2.Distance(destination, rightStartPoint.position) / currentLength;
        }
        for (int i = 0; i < currentLength; i++)
        {
            allMasks[i].transform.position = rayLeft.GetPoint(lestDistanceBetweenMasks * i);
            allMasks[countOfPoints - 1 - i].transform.position = rayRight.GetPoint(rightDistanceBetweenMasks * i);
        }
        maskParent.SetActive(true);
    }

    public void AimGenerator(Vector2 destination)
    {

    }
}
