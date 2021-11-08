using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsteroidGenerator : MonoBehaviour
{
    public static AsteroidGenerator Instance { get; private set; }
    public float timeToReload;
    private float cooldown = 0f, startTimeToReload;
    public List<AsteroidMotion> asteroidsPool = new List<AsteroidMotion>();
    public List<GameObject> mineralsPool = new List<GameObject>();
    [SerializeField] private GameObject mineralProtorype;
    private void Start()
    {
        Instance = this;
        startTimeToReload = timeToReload;

        createPool("Mineral", 50);
        createPool("Asteroid", 50);
    }

    void FixedUpdate()
    {
        cooldown += Time.deltaTime;
        if (cooldown > timeToReload)
        {
            cooldown = 0f;
            timeToReload = startTimeToReload * (0.8f + Random.value / 2f);
            LaunchAsteroid();
        }
    }

    private void LaunchAsteroid()
    {
        if (asteroidsPool.Count == 0)
            return;
        AsteroidMotion asteroid = GetAsteroidFromPool();
        Vector2 startPos = new Vector2();
        Vector2 endPos = new Vector2();

        startPos.y = ScreenInfo.Instance.maxScreenEdge.y;
        endPos.y = ScreenInfo.Instance.minScreenEdge.y;
        startPos.x = Random.Range(ScreenInfo.Instance.minScreenEdge.x * 1.1f, ScreenInfo.Instance.maxScreenEdge.x * 0.9f);
        endPos.x = Random.Range(ScreenInfo.Instance.minScreenEdge.x * 1.1f, ScreenInfo.Instance.maxScreenEdge.x * 0.9f);
//Random.Range(ScreenInfo.Instance.minScreenEdge.y, 
            //ScreenInfo.Instance.minScreenEdge.y + (ScreenInfo.Instance.maxScreenEdge.y - ScreenInfo.Instance.minScreenEdge.y) * 0.3f); 
        asteroid.startPos = startPos;
        asteroid.endPos = endPos;
    }

    #region PoolManager
    private void createPool(string type, int count)
    {
        GameObject[] gameObjects = GameObject.FindGameObjectsWithTag(type);
        if (gameObjects.Length == 0)
            return;

        //for (int j = 0; j < gameObjects.Length; j++)
            //Debug.Log(gameObjects[j]);
        if (type == "Mineral")
        {
            mineralProtorype = Instantiate(gameObjects[0]);
            mineralProtorype.transform.position = GameManager.Instance.spawnPosition;
        }

        GameObject parent = new GameObject();
        parent.name = type + "s Pool";


        for (int i = 0; i < count / gameObjects.Length; i++)
        {
            for (int j = 0; j < gameObjects.Length; j++)
            {
                if (type == "Asteroid")
                {
                    var tmpAsteroid = Instantiate(gameObjects[j], parent.transform).GetComponent<AsteroidMotion>();
                    asteroidsPool.Add(tmpAsteroid);
                    tmpAsteroid.gameObject.SetActive(false);
                }
                if (type == "Mineral")
                {
                    GameObject tmpMineral = Instantiate(gameObjects[j], parent.transform);
                    tmpMineral.name = "Mineral№" + Random.Range(0, 10000).ToString();
                    mineralsPool.Add(tmpMineral);
                    tmpMineral.SetActive(false);
                }
            }
        }

        for (int j = 0; j < gameObjects.Length; j++)
            Destroy(gameObjects[j]);
    }

    public AsteroidMotion GetAsteroidFromPool()
    {        
        AsteroidMotion asteroidTmp = asteroidsPool[0];
        asteroidsPool.Remove(asteroidTmp);
        asteroidTmp.gameObject.transform.position = GameManager.Instance.spawnPosition;
        asteroidTmp.gameObject.SetActive(true);
        //asteroidTmp.gameObject.transform.parent = null;
        return asteroidTmp;
    }

    public void AsteroidBackToPool(AsteroidMotion asteroid)
    {
        //asteroid.gameObject.transform.SetParent(gameObjectsStorage.transform);
        asteroidsPool.Add(asteroid);
        asteroid.gameObject.SetActive(false);
    }

    public void SpawnMineralAt(Vector2 spawnPos)
    {
        GameObject tmpMineral = GetMineralFromPool();
        tmpMineral.transform.position = spawnPos;
    }

    public GameObject GetMineralFromPool()
    {
        GameObject mineralTmp;
        int i = 0;
        do
        {
            if (i >= mineralsPool.Count)
            {
                mineralTmp = Instantiate(mineralProtorype);
                mineralTmp.SetActive(false);
                break;
            }
            mineralTmp = mineralsPool[i];
            mineralsPool.Remove(mineralTmp);
            i++;
        } while (!mineralsPool.Contains(mineralTmp));
        
        //GameObject mineralTmp = mineralsPool[0];
        //mineralsPool.Remove(mineralTmp);
        mineralTmp.transform.position = GameManager.Instance.spawnPosition;
        mineralTmp.SetActive(true);
        return mineralTmp;
    }

    public void MineralBackToPool(GameObject mineral)
    {
        mineralsPool.Add(mineral);
        mineral.gameObject.SetActive(false);
    }

       
    //public ResourceTrigger GetMineralFromPool(Vector2 spawnPosition)
    //{
    //    if (mineralsPool.Count == 0)
    //        return null;
    //    ResourceTrigger mineralTmp = mineralsPool[0];
    //    if (!mineralTmp)
    //        return null;
    //    mineralsPool.Remove(mineralTmp);
    //    Debug.Log("Забрали минерал из пула. Он в пуле = " + mineralsPool.Contains(mineralTmp));
    //    mineralTmp.gameObject.transform.position = GameManager.Instance.spawnPosition;
    //    mineralTmp.gameObject.SetActive(true);
    //    mineralTmp.gameObject.transform.position = spawnPosition;
    //    //mineralTmp.gameObject.transform.parent = null;
    //    return mineralTmp;
    //}

    //public void MineralBackToPool(ResourceTrigger mineral)
    //{
    //    //mineral.gameObject.transform.SetParent(gameObjectsStorage.transform);
    //    mineralsPool.Add(mineral);
    //    if (mineral.isActiveAndEnabled)
    //        mineral.gameObject.SetActive(false);
    //}
    #endregion

}
