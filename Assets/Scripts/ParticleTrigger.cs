using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ParticleTrigger : MonoBehaviour
{
    ParticleSystem ps;
    private void Start()
    {
        ps = GetComponent<ParticleSystem>();
       
    }

    private List<ParticleSystem.Particle> inside = new List<ParticleSystem.Particle>();
    private ParticleSystem.ColliderData insideData = new ParticleSystem.ColliderData();
    private int numInside;
    private void OnParticleTrigger()
    {
        numInside = ps.GetTriggerParticles(ParticleSystemTriggerEventType.Enter, inside, out insideData);

        for (int i = 0; i < numInside; i++)
        {
            if (insideData.GetColliderCount(i) == 1)
            {
                var other = insideData.GetCollider(i, 0);
                Debug.Log(other.name);
                if (other)
                    return;
            }
        }
    }

    public void SetTriggers()
    {
        if (!ps)
            return;
        for (int i = 0; i < GameManager.Instance.enemyRocketsPool.Count; i++)
            ps.trigger.SetCollider(i, GameManager.Instance.enemyRocketsPool[i].GetComponent<Collider2D>());
    } 
}
