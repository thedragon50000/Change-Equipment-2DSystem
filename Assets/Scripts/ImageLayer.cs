using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImageLayer : MonoBehaviour
{
    RawImage rawImage;
    int pixelWidth;
    int pixelHeight;

    int unitWidth;
    int unitHeight;

    public int pixelsPerUnit = 32;

    void Start()
    {
    }

    public void SetVisible(bool visible)
    {
        rawImage = GetComponent<RawImage>();
        if (!rawImage || !rawImage.texture)
        {
            return;
        }
        rawImage.enabled = visible;
    }

    public void Init(Texture texture)
    {
        rawImage = GetComponent<RawImage>();
        if (texture != null)
        {
            rawImage.texture = texture;
        }
        if (!rawImage || !rawImage.texture)
        {
            return;
        }
        float parentWidth = transform.parent.GetComponent<RectTransform>().rect.width;
        //Debug.Log($"parent width {parentWidth}");

        if (parentWidth == 0)
        {
            return;
        }

        pixelHeight = rawImage.texture.height;
        pixelWidth = rawImage.texture.width;

        unitWidth = pixelWidth / pixelsPerUnit;
        unitHeight = pixelHeight / pixelsPerUnit;

        float uiWidth = parentWidth * unitWidth / 8;
        float uiHeight = uiWidth * pixelHeight / pixelWidth;

        transform.GetComponent<RectTransform>().sizeDelta = new Vector2(uiWidth, uiHeight);
    }

    private void OnValidate()
    {
        Init(null);
    }

    public void SetOffset(int clothesIndex)
    {
        Debug.Log("SetOffset " + clothesIndex);

        float parentWidth = transform.parent.GetComponent<RectTransform>().rect.width;
        Debug.Log($"parent--width {parentWidth}");

        float uiX = parentWidth * clothesIndex * 8 / 8;

        transform.GetComponent<RectTransform>().anchoredPosition = new Vector2(-uiX, 0);
    }

}
