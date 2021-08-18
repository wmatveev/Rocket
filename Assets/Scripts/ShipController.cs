using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipController : MonoBehaviour
{
    public float timeToReload;
    private float cooldown = 0f;
    void Start()
    {
        gameObject.transform.position = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, 0);
    }
    void FixedUpdate()
    {
        cooldown += Time.deltaTime;
        if (cooldown > timeToReload)
        {
            cooldown = 0f;
            AutoGunRocketLaunch();
        }
    }
    private void AutoGunRocketLaunch()
    {
        ThreeBezierScript bezier = GameManager.Instance.GetRocketFromPool(RocketLauncher.Mode.autoGun);
        if (!bezier)
            return;
        bezier.currentMode = RocketLauncher.Mode.autoGun;
        Vector3 rayUp = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y + 5f, 0);
        Ray ray = new Ray(gameObject.transform.position, rayUp - gameObject.transform.position);
        bezier.SetPoints(gameObject.transform.position, ray.GetPoint(5f * 100), ray.GetPoint(5f * 500));
    }
}
