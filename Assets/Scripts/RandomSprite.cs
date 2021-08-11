using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomSprite : MonoBehaviour
{
    public List<Sprite> sprites = new List<Sprite>();
    public List<Sprite> childrenSprites = new List<Sprite>();
    public List<Sprite> secondChildrenSprites = new List<Sprite>();
    public SpriteRenderer spriteRenderer, childrenSpRenderer, children2SpRenderer;

    void OnEnable()
    {
        if (sprites.Count > 0)
        {
            int randIndex = Random.Range(0, sprites.Count);
            spriteRenderer.sprite = sprites[randIndex];
            //children sprites:
            if (childrenSprites.Count > randIndex && childrenSpRenderer)
                childrenSpRenderer.sprite = childrenSprites[randIndex];

            if (secondChildrenSprites.Count > randIndex && children2SpRenderer)
                children2SpRenderer.sprite = secondChildrenSprites[randIndex];
        }
    }
}
