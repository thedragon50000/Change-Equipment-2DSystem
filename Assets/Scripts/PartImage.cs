using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PartImage : MonoBehaviour
{
    // 使用UI RawImage原生圖片控件顯示
    RawImage rawImage;
    int pixelWidth;     // 以1像素為單位的寬度
    int pixelHeight;    // 以1像素為單位的高度

    int unitWidth;      // 以32像素為一個單位的寬度
    int unitHeight;     // 以32像素為一個單位的高度

    public int col = 0;     // 當前顯示第幾列的圖
    public int row = 0;     // 當前顯示第幾行的圖
    public int pixelsPerUnit = 32;      // 32個像素為1個單位

    void Start()
    {
        Init();
    }

    public void Init()
    {
        // 經過一系列計算，設置rawImage的UV坐標。
        // 思路就是把0~unitHeight的像素，映射到0.0 ~ 1.0的範圍。

        rawImage = GetComponent<RawImage>();

        pixelHeight = rawImage.texture.height;
        pixelWidth = rawImage.texture.width;

        unitWidth = pixelWidth / pixelsPerUnit;
        unitHeight = pixelHeight / pixelsPerUnit;

        // UV坐標下方是0，而row是從左上角開始數的
        int row2 = unitHeight - row - 1;
        Rect rect = new Rect((float)col/unitWidth, (float)row2/unitHeight, 1f/unitWidth, 1f/unitHeight);
        rawImage.uvRect = rect;
    }

    public void Init(int col, int row)
    {
        this.col = col;
        this.row = row;
        Init();
    }

    // 在編輯器中隨時刷新圖片
    private void OnValidate()
    {
        if (!rawImage || !rawImage.texture)
        {
            rawImage = GetComponent<RawImage>();
            if (!rawImage.texture)
            {
                return;
            }
        }

        Init();
    }
}