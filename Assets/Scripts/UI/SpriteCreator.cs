using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class SpriteCreator : MonoBehaviour
{
    /// <summary>
    /// Top padding = 0.1f.
    /// </summary>
    /// <param name="points"></param>
    /// <param name="background"></param>
    /// <param name="graph"></param>
    /// <param name="rect"></param>
    /// <returns></returns>
    public static Sprite Create(float[] points, Color background, Color graph, Vector2Int rect)
    {
        // кол-во пикселей по горизонтали между ближайшими точками графика
        var topPadding = rect.y * 0.1f;
        float maxY = points.Max();
        float yK = (rect.y - topPadding) / maxY;
        int horizontalPixels = rect.x / points.Length;

        var height = rect.y;
        var width = (points.Length - 1) * horizontalPixels;
        var texture = new Texture2D(width, height, TextureFormat.RGBA32, false);

        for (int i = 0; i < (points.Length - 1); i++)
        {
            for (int j = 0; j < horizontalPixels; j++)
            {
                // текущий пиксель по X
                var hPoint = i * horizontalPixels + j;
                // точки графика для сравнения текущего пикселя (выше или ниже => какого цвета он будет)
                var pointGraph1 = new Vector2(i * horizontalPixels, points[i] * yK);
                var pointGraph2 = new Vector2((i + 1) * horizontalPixels, points[i + 1] * yK);

                for (int k = 0; k < height; k++)
                {
                    var pointInTexture = new Vector2Int(hPoint, k);

                    if (GreaterThan(pointGraph1, pointGraph2, pointInTexture))
                    {
                        texture.SetPixel(pointInTexture.x, pointInTexture.y, background);
                    }
                    else
                    {
                        texture.SetPixel(pointInTexture.x, pointInTexture.y, graph);
                    }
                }
            }
        }

        texture.Apply();

        var textureRect = new Rect(0, 0, width, height);
        var texturePivot = new Vector2(width / 2, height / 2);

        Sprite sprite = Sprite.Create(texture as Texture2D, textureRect, texturePivot);
        
        return sprite;
    }

    public static void SaveTexture(Texture2D texture)
    {
        var bytes = texture.EncodeToPNG();
        using (FileStream fileSave = new FileStream(Application.dataPath + "/Save/" + "Image_" + Random.Range(0, 500) + ".png", FileMode.Create))
        {
            BinaryWriter binary = new BinaryWriter(fileSave);

            binary.Write(bytes);
            fileSave.Close();
        }
    }

    public static bool GreaterThan(Vector2 pointGraph1, Vector2 pointGraph2, Vector2 point)
    {
        var k = (pointGraph2.y - pointGraph1.y) / (pointGraph2.x - pointGraph1.x);
        var m = pointGraph1.y - k * pointGraph1.x;

        return point.y > (point.x * k + m);
    }
}
