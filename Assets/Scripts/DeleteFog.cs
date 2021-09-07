using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DeleteFog : MonoBehaviour
{
    Button fog;
    void Start()
    {
        fog = GetComponent<Button>();
    }

    public void onClickEvent()
    {
        //UIMapMenu.Instance.fogMenu.SetActive(true);
        fog.gameObject.SetActive(false);
    }
    public void deleteFog()
    {
        //save fog id
        fog.gameObject.SetActive(false);
    }
}
