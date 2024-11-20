using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

public struct Pos
{
    public int x;
    public int y;

    public Pos(int x, int y)
    {
        this.x = x;
        this.y = y;
    }
}

public class LayerInfo
{
    public bool Enable;
    public RawImage Raw;
    public Pos UnitPos;
}

public class UIManager : MonoBehaviour
{
    // [Header("這是顯示區域的標題")]

    public static UIManager Instance { get; private set; }

    [Tooltip("命名用輸入框")] public InputField exportNameField;


    /// <summary>
    /// 預覽Panel
    /// </summary>
    [FormerlySerializedAs("picturePreviewParent")] [Tooltip("預覽Panel")]
    public Transform picturePreview;


    /// <summary>
    /// 選擇Panel
    /// </summary>
    [Tooltip("選擇Panel")] public Transform buttonsParent;

    /// <summary>
    /// 按下layerButtonsParent的按鈕後彈出的選擇框
    /// </summary>
    public UISelectClothesPanel selectPanel;

    private readonly List<LayerInfo> _layersInfo = new();

    private void Awake()
    {
        Instance = this;
    }

    private IEnumerator Start()
    {
        selectPanel.gameObject.SetActive(false);

        for (int i = 0; i < buttonsParent.childCount; i++) // 所有套件(衣服、褲子、眼鏡...等)
        {
            Transform child = buttonsParent.GetChild(i);
            RawImage raw = child.GetComponentInChildren<RawImage>();
            Toggle toggle = child.GetComponentInChildren<Toggle>();

            LayerInfo info = new LayerInfo
            {
                Raw = raw,
                UnitPos = new Pos(0, 0),
                Enable = toggle.isOn
            };
            _layersInfo.Add(info);
        }

        yield return null;

        for (int i = 0; i < _layersInfo.Count; i++)
        {
            LayerInfo info = _layersInfo[i];

            Transform child = picturePreview.GetChild(i);
            ImageLayer layer = child.GetComponent<ImageLayer>();
            RawImage raw = child.GetComponent<RawImage>();

            layer.InitImageLayer(info.Raw.texture);
            layer.SetVisible(_layersInfo[i].Enable);
        }
    }

    public void OnSelectClothes(BaseEventData baseEventData)
    {
        PointerEventData evt = (PointerEventData)baseEventData;
        Debug.Log("OnSelectClothes " + evt.pointerClick);

        Transform parent = evt.pointerClick.transform.parent; // H1, H2, H3
        int layerIndex = parent.GetSiblingIndex();
        string className = parent.GetChild(0).name;

        // switch (className)
        // {
        //     case "衣服":
        //         break;
        //     case "髮型":
        //         break;
        //     case "鞋子":
        //         break;
        // }

        RawImage raw = evt.pointerClick.GetComponent<RawImage>();
        ShowSelectPanel(layerIndex, className, raw);
    }

    public void ShowSelectPanel(int index, string className, RawImage raw)
    {
        selectPanel.InitSelectingPanel(index, className, raw);
        selectPanel.gameObject.SetActive(true);
    }

    public void ChangeClothes(RawImage raw, int layerIndex, int clothesIndex)
    {
        // 修改按鈕的rawImage
        Debug.Log($"修改層級 {layerIndex}   名稱 {raw.transform.parent.name}");
        PartImage partImage = raw.GetComponent<PartImage>();
        partImage.Init(8 * clothesIndex + 1, 0);

        // 修改layer
        Transform transLayer = picturePreview.GetChild(layerIndex);

        ImageLayer layer = transLayer.GetComponent<ImageLayer>();
        layer.InitImageLayer(raw.texture);
        layer.SetOffset(clothesIndex);

        // 修改LayerInfo
        _layersInfo[layerIndex].Raw = raw;
        _layersInfo[layerIndex].UnitPos = new Pos(clothesIndex * 8, 0);
    }

    public void OnToggleChange(Toggle toggle)
    {
        int index = toggle.transform.parent.GetSiblingIndex();
        _layersInfo[index].Enable = toggle.isOn;

        Transform transLayer = picturePreview.GetChild(index);
        ImageLayer layer = transLayer.GetComponent<ImageLayer>();
        layer.SetVisible(toggle.isOn);
    }


    // 保存文件
    public void SavePNG()
    {
        List<Color32[]> layers = new();
        // 貼圖變量Texture2D
        Texture2D tex2d = new(0, 0);

        // 遍歷所有圖層，根據偏移量，從RawImage中劃出需要的區域範圍，類似Photoshop的選框工具
        for (int i = 0; i < _layersInfo.Count; i++)
        {
            LayerInfo info = _layersInfo[i];

            if (!info.Enable)
            {
                continue;
            }

            RawImage raw = info.Raw;
            Pos pos = info.UnitPos;

            Rect rect = new Rect(pos.x * 32, 0, 8 * 32, raw.texture.height);

            tex2d = TextureToTexture2D(raw.texture, rect);
            layers.Add(tex2d.GetPixels32());
        }

        // 圖層合成
        // 這時已經拿到一系列的Texture2D，考慮像素透明度，將它們加到一起即可
        Color32[] pixels = new Color32[layers[0].Length];
        for (int index = 0; index < layers.Count; index++)
        {
            Color32[] d = layers[index];
            for (int i = 0; i < d.Length; i++)
            {
                // Note: 一層一層疊上去，如果a值(透明度)為0，就維持原樣
                pixels[i] = d[i].a == 0 ? pixels[i] : d[i];
            }
        }

        tex2d.SetPixels32(pixels);

#if UNITY_EDITOR
        string path = "Assets/ExportPNG"; //設置存儲路徑
#else
        string path = Application.persistentDataPath;//設置存儲路徑
#endif
        Debug.Log("保存路徑：" + path);

        FileStream stream = new FileStream(path + "/" + exportNameField.text + ".png", FileMode.OpenOrCreate);
        byte[] png = tex2d.EncodeToPNG(); // EncodeToPNG，這個API很好用
        stream.Write(png, 0, png.Length);
        stream.Close();
    }

    // 網上找到的Texture 轉 Texture2D 方法，要用RenderTexture轉一步，較慢但是能用
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