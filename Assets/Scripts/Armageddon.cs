using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Armageddon : MonoBehaviour
{
    [SerializeField] private ParticleSystem armageddonPS;
    private List<ParticleSystem> armageddonPSPool = new List<ParticleSystem>();
    private int targetIndex = 0;
    private void Start()
    {
        createArmageddonPSPool();
    }

    public void launchArmageddon()
    {
        int countOfTargets = GameManager.Instance.currEnemyRockets.Count;
        for (int i = 0; i < countOfTargets; i++)
            ArmageddonLightning();

        targetIndex = 0;
    }

    private void ArmageddonLightning()
    {
        if (GameManager.Instance.currEnemyRockets.Count <= targetIndex)
            return;

        ThreeBezierScript target = GameManager.Instance.currEnemyRockets[targetIndex];
        Vector2 targetPos = target.gameObject.transform.position;
        Vector2 pos = armageddonPSPool[targetIndex].transform.position;
        armageddonPSPool[targetIndex].transform.rotation = Quaternion.LookRotation(targetPos - pos);
        armageddonPSPool[targetIndex].startSize =
            Vector2.Distance(armageddonPSPool[targetIndex].transform.position, targetPos) / 2;
        armageddonPSPool[targetIndex].Play();
        target.DestroyWithDelay(0.45f);
        targetIndex++;
    }

    private void createArmageddonPSPool()
    {
        armageddonPS.Stop();
        armageddonPSPool.Add(armageddonPS);
        for (int i = 1; i < GameManager.Instance.eRocketsToLaunch; i++)
        {
            var tmp = Instantiate(armageddonPS);
            tmp.Stop();
            armageddonPSPool.Add(tmp);
        }
    }
}
