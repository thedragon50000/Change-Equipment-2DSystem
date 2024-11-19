using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public struct Pos
{
    public int x;
    public int y;

    public Pos(int x, int y) { this.x = x; this.y = y; }
}

public class LayerInfo
{
    public bool enable;
    public RawImage raw;
    public Pos unitPos;
}

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }
    public UISelectClothesPanel selectPanel;

    public Transform layersParent;
    public Transform layerButtonsParent;

    public InputField exportNameField;

    List<LayerInfo> layersInfo = new List<LayerInfo>();

    private void Awake()
    {
        Instance = this;
    }

    private IEnumerator Start()
    {
        selectPanel.gameObject.SetActive(false);
        
        for (int i=0; i<layerButtonsParent.childCount; i++)
        {
            Transform child = layerButtonsParent.GetChild(i);
            RawImage raw = child.GetComponentInChildren<RawImage>();
            Toggle toggle = child.GetComponentInChildren<Toggle>();

            LayerInfo info = new LayerInfo();
            info.raw = raw;
            info.unitPos = new Pos(0, 0);
            info.enable = toggle.isOn;
            layersInfo.Add(info);
        }

        yield return null;

        for (int i=0; i<layersInfo.Count; i++)
        {
            LayerInfo info = layersInfo[i];

            Transform child = layersParent.GetChild(i);
            ImageLayer layer = child.GetComponent<ImageLayer>();
            RawImage raw = child.GetComponent<RawImage>();

            layer.Init(info.raw.texture);
            layer.SetVisible(layersInfo[i].enable);
        }
    }

    public void OnSelectClothes(BaseEventData _evt)
    {
        PointerEventData evt = (PointerEventData)_evt;
        Debug.Log("OnSelectClothes " + evt.pointerClick);

        Transform parent = evt.pointerClick.transform.parent;      // H1, H2, H3
        int layerIndex = parent.GetSiblingIndex();
        string className = parent.GetChild(0).name;

        switch (className)
        {
            case "衣服":
                break;
            case "发型":
                break;
            case "鞋子":
                break;
        }

        RawImage raw = evt.pointerClick.GetComponent<RawImage>();
        ShowSelectPanel(layerIndex, className, raw);
    }

    public void ShowSelectPanel(int layerIndex, string className, RawImage raw)
    {
        selectPanel.Init(layerIndex, className, raw);
        selectPanel.gameObject.SetActive(true);
    }

    public void ChangeClothes(RawImage raw, int layerIndex, int clothesIndex)
    {
        // 修改按钮的rawImage
        Debug.Log($"修改层级 {layerIndex}   名称 {raw.transform.parent.name}" );
        PartImage partImage = raw.GetComponent<PartImage>();
        partImage.Init(8* clothesIndex + 1, 0);

        // 修改layer
        Transform transLayer = layersParent.GetChild(layerIndex);

        ImageLayer layer = transLayer.GetComponent<ImageLayer>();
        layer.Init(raw.texture);
        layer.SetOffset(clothesIndex);

        // 修改LayerInfo
        layersInfo[layerIndex].raw = raw;
        layersInfo[layerIndex].unitPos = new Pos(clothesIndex * 8, 0);
    }

    public void OnToggleChange(Toggle toggle)
    {
        int index = toggle.transform.parent.GetSiblingIndex();
        layersInfo[index].enable = toggle.isOn;

        Transform transLayer = layersParent.GetChild(index);
        ImageLayer layer = transLayer.GetComponent<ImageLayer>();
        layer.SetVisible(toggle.isOn);
    }


    // 保存文件
    public void SavePNG()
    {
        List<Color32[]> layers = new List<Color32[]>();
        // 贴图变量Texture2D
        Texture2D tex2d = new Texture2D(0,0);

        // 遍历所有图层，根据偏移量，从RawImage中划出需要的区域范围，类似Photoshop的选框工具
        for (int i=0; i<layersInfo.Count; i++)
        {
            LayerInfo info = layersInfo[i];

            if (!info.enable) { continue; }

            RawImage raw = info.raw;
            Pos pos = info.unitPos;

            Rect rect = new Rect(pos.x * 32, 0, 8 * 32, raw.texture.height);

            tex2d = TextureToTexture2D(raw.texture, rect);
            layers.Add(tex2d.GetPixels32());
        }

        // 图层合成
        // 这时已经拿到一系列的Texture2D，考虑像素透明度，将它们加到一起即可
        Color32[] pixels = new Color32[layers[0].Length];
        for (int index=0; index<layers.Count; index++)
        {
            Color32[] d = layers[index];
            for (int i=0; i<d.Length; i++)
            {
                pixels[i] = d[i].a != 0 ? d[i] : pixels[i];
            }
        }

        tex2d.SetPixels32(pixels);

#if UNITY_EDITOR
        string path = "ExportPNG";//设置存储路径
#else
        string path = Application.persistentDataPath;//设置存储路径
#endif
        Debug.Log("保存路径：" + path);

        FileStream stream = new FileStream(path + "/" + exportNameField.text + ".png", FileMode.OpenOrCreate);
        byte[] png = tex2d.EncodeToPNG();       // EncodeToPNG，这个API很好用
        stream.Write(png, 0, png.Length);
        stream.Close();
    }

    // 网上找到的Texture 转 Texture2D 方法，要用RenderTexture转一步，较慢但是能用
    private Texture2D TextureToTexture2D(Texture texture, Rect area)
    {
        Texture2D texture2D = new Texture2D(Mathf.RoundToInt(area.width), Mathf.RoundToInt(area.height), TextureFormat.RGBA32, false);
        RenderTexture currentRT = RenderTexture.active;
        RenderTexture renderTexture = RenderTexture.GetTemporary(texture.width, texture.height, 32);
        Graphics.Blit(texture, renderTexture);

        RenderTexture.active = renderTexture;
        texture2D.ReadPixels(area, 0, 0);
        texture2D.Apply();

        RenderTexture.active = currentRT;
        RenderTexture.ReleaseTemporary(renderTexture);

        return texture2D;
    }

}
