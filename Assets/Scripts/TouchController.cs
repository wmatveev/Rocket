using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TouchController : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    public static TouchController Instance { get; private set; }

    public GameObject playerShip;
    private bool isDraged = false;

    private GraphicRaycaster raycaster;

    void Start()
    {
        Instance = this;

        raycaster = gameObject.GetComponent<GraphicRaycaster>();
        playerShip = GameObject.FindWithTag("AutoGun");
    }

    public virtual void OnPointerDown(PointerEventData eventData)
    {
        if (RocketLauncher.Instance)
        {
            RocketLauncher.Instance._OnPointerDown(eventData);
            OnDrag(eventData);
        }

        if (playerShip)
        {
            List<RaycastResult> results = new List<RaycastResult>();
            raycaster.Raycast(eventData, results);
            foreach (RaycastResult result in results)
            {
                if (result.gameObject.transform.tag == "AutoGun")
                {
                    isDraged = true;
                    OnDrag(eventData);
                    break;
                }
            }
        }
    }

    public virtual void OnDrag(PointerEventData eventData)
    {
        if (RocketLauncher.Instance)
            RocketLauncher.Instance._OnDrag(eventData);
        if (isDraged == true)
        {
            Vector3 newPosition = Camera.main.ScreenToWorldPoint(eventData.position);
            newPosition = new Vector2(newPosition.x, newPosition.y);
            playerShip.transform.position = newPosition;
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (RocketLauncher.Instance)
            RocketLauncher.Instance._OnPointerUp(eventData);
        isDraged = false;
    }
}
