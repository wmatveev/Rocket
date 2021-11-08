using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// This class makes transparent pixels unclickable.
/// For correct work set in image import settings -> advanced -> read/write enables = true
/// </summary>

public class ButtonClickMask : MonoBehaviour
{
    [Range(0f, 1f)] 
    public float AlphaLevel = 1f;
    private Image bt;

    void Start()
    {
        bt = gameObject.GetComponent<Image>();
        bt.alphaHitTestMinimumThreshold = AlphaLevel;
    }
}