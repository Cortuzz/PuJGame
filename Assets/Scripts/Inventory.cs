using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    public GameObject inventoryUI;
    public GameObject inventorySlotPrefab;

    public Vector2 multiplierUI;
    public Vector2 offsetUI;

    public int width;
    public int height;
    public InventorySlot[,] slots;
    public GameObject[,] slotsUI;
    // Start is called before the first frame update
    private void Start()
    {
        slots = new InventorySlot[width, height];
        slotsUI = new GameObject[width, height];

        SetupUI();
        UpdateUI();
    }

    public void ChangeVisibility(bool visibility)
    {
        inventoryUI.SetActive(visibility);
    }

    private void SetupUI()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                GameObject slot = Instantiate(inventorySlotPrefab, inventoryUI.transform.GetChild(0).transform);
                slot.GetComponent<RectTransform>().localPosition = 
                    new Vector3(i * multiplierUI.x + offsetUI.x, j * multiplierUI.y + offsetUI.y, 0);

                slotsUI[i, j] = slot;
                slots[i, j] = null;
            }
        }
    }

    private void UpdateUI()
    {
        Sprite itemSprite;
        bool flag;

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {

                if (slots[i, j] == null)
                {
                    itemSprite = null;
                    flag = false;
                }
                else
                {
                    itemSprite = slots[i, j].tile.sprite;
                    flag = true;
                }

                Image itemImage = slotsUI[i, j].transform.GetChild(0).GetComponent<Image>();
                itemImage.enabled = flag;
                itemImage.sprite = itemSprite;
            }
        }
    }
}
