using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// 讓你選擇服裝的彈窗
/// </summary>
public class UISelectClothesPanel : MonoBehaviour
{
    public int layerIndex;
    Text _title;
    RawImage _rawImage;
    GridLayoutGroup _grid;

    public RawImage prefabGridItem; //32*32

    public void InitSelectingPanel(int index, string sName, RawImage raw)
    {
        layerIndex = index;

        if (!_title)
        {
            _title = transform.GetChild(0).GetComponent<Text>();
        }

        if (!_grid)
        {
            _grid = transform.GetChild(1).GetComponent<GridLayoutGroup>();
        }

        _rawImage = raw;

        _title.text = sName;

        // Note: 除以32是因為每張圖都是32*32，除以8是因為一套衣服有8列，但彈窗只需要展示第一列第一件即可
        int n = raw.texture.width / (8 * 32);

        {
            // 歸零
            int k = _grid.transform.childCount;
            for (int i = 0; i < k; i++)
            {
                DestroyImmediate(_grid.transform.GetChild(0).gameObject);
            }
        }

        // 產生
        for (int i = 0; i < n; i++)
        {
            RawImage item = Instantiate(prefabGridItem, _grid.transform);
            item.texture = raw.texture;

            PartImage part = item.GetComponent<PartImage>();
            part.Init(i * 8, 0);

            EventTrigger et = item.GetComponent<EventTrigger>();
            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.PointerClick;
            entry.callback.AddListener(OnClickItem);
            et.triggers.Add(entry);
        }
    }

    private void OnClickItem(BaseEventData baseEventData)
    {
        PointerEventData evt = (PointerEventData)baseEventData;

        int clothesIndex = evt.pointerClick.transform.GetSiblingIndex();
        Debug.Log($"選中了第{clothesIndex}個");

        UIManager.Instance.ChangeClothes(_rawImage, layerIndex, clothesIndex);

        transform.gameObject.SetActive(false);
    }

    public void OnClose()
    {
        gameObject.SetActive(false);
    }
}