using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PartImage : MonoBehaviour
{
    // 使用UI RawImage原生图片控件显示
    RawImage rawImage;
    int pixelWidth;     // 以1像素为单位的宽度
    int pixelHeight;    // 以1像素为单位的高度

    int unitWidth;      // 以32像素为一个单位的宽度
    int unitHeight;     // 以32像素为一个单位的高度

    public int col = 0;     // 当前显示第几列的图
    public int row = 0;     // 当前显示第几行的图
    public int pixelsPerUnit = 32;      // 32个像素为1个单位

    void Start()
    {
        Init();
    }

    public void Init()
    {
        // 经过一系列计算，设置rawImage的UV坐标。
        // 思路就是把0~unitHeight的像素，映射到0.0 ~ 1.0的范围。

        rawImage = GetComponent<RawImage>();

        pixelHeight = rawImage.texture.height;
        pixelWidth = rawImage.texture.width;

        unitWidth = pixelWidth / pixelsPerUnit;
        unitHeight = pixelHeight / pixelsPerUnit;

        // UV坐标下方是0，而row是从左上角开始数的
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

    // 在编辑器中随时刷新图片
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
