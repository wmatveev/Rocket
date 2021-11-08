using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class DeleteFogButton : MonoBehaviour
{
    [SerializeField] private GameObject fog;
    public void CallDeleteFogMenu()
    {
        UIMapMenu.Instance.CallDeleteFogMenu(gameObject.transform.position, fog, gameObject);
    }
}
