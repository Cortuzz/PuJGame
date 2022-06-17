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

    public bool TryAdd(Item item)
    {
        for (int i = height - 1; i >= 0; i--)
        {
            for (int j = 0; j < width; j++)
            {
                if (slots[j, i] != null)
                    continue;

                slots[j, i] = new InventorySlot { item = item, position = new Vector2Int(j, i), quantity = 1 };
                return true;
            }
        }

        return false;
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
                Text amountText = slotsUI[i, j].transform.GetChild(1).GetComponent<Text>();

                if (slots[i, j] == null)
                {
                    itemSprite = null;
                    flag = false;
                }
                else
                {
                    itemSprite = slots[i, j].item.sprite;
                    amountText.text = slots[i, j].quantity.ToString();
                    flag = true;
                }

                Image itemImage = slotsUI[i, j].transform.GetComponent<Image>();

                amountText.enabled = flag;
                itemImage.enabled = flag;
                itemImage.sprite = itemSprite;
            }
        }
    }

    private void FixedUpdate()
    {
        UpdateUI();
    }
}
