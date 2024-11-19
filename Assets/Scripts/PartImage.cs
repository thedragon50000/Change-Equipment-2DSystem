using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PartImage : MonoBehaviour
{
    // ʹ��UI RawImageԭ��ͼƬ�ؼ���ʾ
    RawImage rawImage;
    int pixelWidth;     // ��1����Ϊ��λ�Ŀ��
    int pixelHeight;    // ��1����Ϊ��λ�ĸ߶�

    int unitWidth;      // ��32����Ϊһ����λ�Ŀ��
    int unitHeight;     // ��32����Ϊһ����λ�ĸ߶�

    public int col = 0;     // ��ǰ��ʾ�ڼ��е�ͼ
    public int row = 0;     // ��ǰ��ʾ�ڼ��е�ͼ
    public int pixelsPerUnit = 32;      // 32������Ϊ1����λ

    void Start()
    {
        Init();
    }

    public void Init()
    {
        // ����һϵ�м��㣬����rawImage��UV���ꡣ
        // ˼·���ǰ�0~unitHeight�����أ�ӳ�䵽0.0 ~ 1.0�ķ�Χ��

        rawImage = GetComponent<RawImage>();

        pixelHeight = rawImage.texture.height;
        pixelWidth = rawImage.texture.width;

        unitWidth = pixelWidth / pixelsPerUnit;
        unitHeight = pixelHeight / pixelsPerUnit;

        // UV�����·���0����row�Ǵ����Ͻǿ�ʼ����
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

    // �ڱ༭������ʱˢ��ͼƬ
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
