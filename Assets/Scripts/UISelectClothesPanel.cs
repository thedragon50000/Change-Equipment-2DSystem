using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UISelectClothesPanel : MonoBehaviour
{
    public int layerIndex;
    Text title;
    RawImage rawImage;
    GridLayoutGroup grid;

    public RawImage prefabGridItem;

    public void Init(int layerIndex, string name, RawImage raw)
    {
        this.layerIndex = layerIndex;

        if (title == null)
        {
            title = transform.GetChild(0).GetComponent<Text>();
        }
        if (grid == null)
        {
            grid = transform.GetChild(1).GetComponent<GridLayoutGroup>();
        }

        rawImage = raw;

        title.text = name;

        int n = raw.texture.width / (8 * 32);

        {
            int k = grid.transform.childCount;
            while (k > 0)
            {
                DestroyImmediate(grid.transform.GetChild(0).gameObject);
                k--;
            }
        }

        for (int i=0; i<n; i++)
        {
            RawImage item = Instantiate(prefabGridItem, grid.transform);
            item.texture = raw.texture;

            PartImage part = item.GetComponent<PartImage>();
            part.Init(i*8, 0);

            EventTrigger et = item.GetComponent<EventTrigger>();
            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.PointerClick;
            entry.callback.AddListener(OnClickItem);
            et.triggers.Add(entry);
        }
    }

    public void OnClickItem(BaseEventData _evt)
    {
        PointerEventData evt = (PointerEventData)_evt;

        int clothesIndex = evt.pointerClick.transform.GetSiblingIndex();
        Debug.Log($"選中了第{clothesIndex}個");

        UIManager.Instance.ChangeClothes(rawImage, layerIndex, clothesIndex);

        transform.gameObject.SetActive(false);
    }

    public void OnClose()
    {
        gameObject.SetActive(false);
    }
}