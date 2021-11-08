using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetSceneOrientation : MonoBehaviour
{
    public enum SceneOrientation
    {
        horizontal,
        vertical
    }
    public SceneOrientation sceneOrientation;

    void Start()
    {
        if (sceneOrientation == SceneOrientation.horizontal)
            Screen.orientation = ScreenOrientation.LandscapeLeft;
        if (sceneOrientation == SceneOrientation.vertical)
            Screen.orientation = ScreenOrientation.Portrait;
    }
}
