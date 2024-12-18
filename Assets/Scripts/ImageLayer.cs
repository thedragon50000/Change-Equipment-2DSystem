using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImageLayer : MonoBehaviour
{
    // Note: ImageLayer 是圖片展示區
    RawImage rawImage;
    int pixelWidth;
    int pixelHeight;

    int unitWidth;
    int unitHeight;

    public int pixelsPerUnit = 32;

    public void SetVisible(bool visible)
    {
        rawImage = GetComponent<RawImage>();
        if (!rawImage || !rawImage.texture)
        {
            return;
        }

        rawImage.enabled = visible;
    }

    public void InitImageLayer(Texture texture)
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

        if (parentWidth == 0)
        {
            return;
        }

        pixelHeight = rawImage.texture.height;
        pixelWidth = rawImage.texture.width;

        unitWidth = pixelWidth / pixelsPerUnit;
        unitHeight = pixelHeight / pixelsPerUnit;

        // print($"parentWidth: ${parentWidth}, unitWidth: ${unitWidth}");

        // Note: 反直覺。假設我本來有10列，parentWidth不變的時候剛好能顯示10列
        // Note: 但我把內容放大卻不去改變相當於Mask的RectTransform時，顯示的就少了
        // Note: 比方說parentWidth * 10 那就只能看到1列了，這時我再/2 的話，就能看到2列

        float uiWidth = parentWidth * unitWidth / 8; // 想要顯示8列
        float uiHeight = uiWidth * pixelHeight / pixelWidth;

        transform.GetComponent<RectTransform>().sizeDelta = new Vector2(uiWidth, uiHeight);
    }

    private void OnValidate()
    {
        InitImageLayer(null);
    }

    public void SetOffset(int clothesIndex)
    {
        Debug.Log("SetOffset " + clothesIndex);

        float parentWidth = transform.parent.GetComponent<RectTransform>().rect.width;
        Debug.Log($"parent--width {parentWidth}");

        // Note: 每套裝備都有等量的圖，圖的寬度等於 parentWidth。
        // Note: 若我有第二套甚至第三套，把anchoredPosition 往左移動一個parentWidth的距離，就是對齊後的第二套
        
        float uiX = parentWidth * clothesIndex ;

        transform.GetComponent<RectTransform>().anchoredPosition = new Vector2(-uiX, 0);
    }
}