using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

public class TestUIFiller : MonoBehaviour
{
    public Color BackgroundColor = Color.white;
    public Color GraphColor = Color.blue;

    public Vector2Int rect = new Vector2Int(1080, 500);

    private void Start()
    {
        Image image = GetComponent<Image>();

        float[] points = new float[]
        {
            40, 15, 36, 48, 5, 27
        };

        image.sprite = SpriteCreator.Create(
            points,
            BackgroundColor,
            GraphColor,
            rect
        );
    }
}
