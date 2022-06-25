using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    public GameObject inventoryUI;
    public GameObject inventorySlotPrefab;

    public Vector2 multiplierUI;
    public Vector2 offsetUI;

    public Vector2 multiplierHotbarUI;
    public Vector2 offsetHotbarUI;

    public int width;
    public int height;
    public InventorySlot[,] slots;
    public GameObject[,] slotsUI;

    public int activeHotbarSlot = 0;
    public InventorySlot[] slotsHotbar;
    public GameObject[] slotsHotbarUI;
    
    private void Start()
    {
        slots = new InventorySlot[width, height];
        slotsUI = new GameObject[width, height];

        slotsHotbar = new InventorySlot[width];
        slotsHotbarUI = new GameObject[width];

        SetupUI();
        UpdateUI();
    }

    public void RemoveActiveItem(int amount = 1)
    {
        var slot = slotsHotbar[activeHotbarSlot];
        if (slot == null)
            return;

        if (slot.quantity - amount <= 0)
        {
            slotsHotbar[activeHotbarSlot] = null;
            return;
        }

        slot.quantity -= amount;
    }

    public Item GetActiveItem()
    {
        if (slotsHotbar[activeHotbarSlot] == null)
            return null;

        return slotsHotbar[activeHotbarSlot].item;
    }

    public void ChangeVisibility(bool visibility)
    {
        inventoryUI.transform.GetChild(0).gameObject.SetActive(visibility);
    }

    public bool TryAddToHotbar(Item item)
    {
        for (int i = 0; i < width; i++)
        {
            var slot = slotsHotbar[i];
            if (slot != null)
            {
                if (slot.item.name == item.name && slot.quantity < item.stackCount)
                {
                    slotsHotbar[i].quantity += 1;
                    return true;
                }
                continue;
            }

            slotsHotbar[i] = new InventorySlot { item = item, position = new Vector2Int(i, 0), quantity = 1 };
            return true;
        }

        return false;
    }

    public bool TryAdd(Item item)
    {
        if (TryAddToHotbar(item))
            return true;
        //test
        for (int i = height - 1; i >= 0; i--)
        {
            for (int j = 0; j < width; j++)
            {
                var slot = slots[j, i];
                if (slot != null)
                {
                    if (slot.item.name == item.name && slot.quantity < item.stackCount)
                    {
                        slots[j, i].quantity += 1;
                        return true;
                    }
                    continue;
                }
                    

                slots[j, i] = new InventorySlot { item = item, position = new Vector2Int(j, i), quantity = 1 };
                return true;
            }
        }

        return false;
    }

    public bool IsContains(Item item, int count = 1)
    {
        for (int i = 0; i < width; i++)
        {
            var slot = slotsHotbar[i];
            if (slot == null || slot.item.name != item.name)
                continue;
                
            if (slot.quantity < count)
            {
                count -= slot.quantity;
                continue;
            }
            return true;
        }

        for (int i = height - 1; i >= 0; i--)
        {
            for (int j = 0; j < width; j++)
            {
                var slot = slots[j, i];
                if (slot == null || slot.item.name != item.name)
                    continue;
                
                if (slot.quantity < count)
                {
                    count -= slot.quantity;
                    continue;
                }
                return true;
            }
        }

        return false;
    }
    
    public void Remove(Item item, int count = 1)
    {
        for (int i = 0; i < width; i++)
        {
            var slot = slotsHotbar[i];
            if (slot == null || slot.item.name != item.name)
                continue;
                
            if (slot.quantity <= count)
            {
                count -= slot.quantity;
                slotsHotbar[i] = null;
                continue;
            }
            slotsHotbar[i].quantity -= count;
            return;
        }
        
        for (int i = height - 1; i >= 0; i--)
        {
            for (int j = 0; j < width; j++)
            {
                var slot = slots[j, i];
                if (slot == null || slot.item.name != item.name)
                    continue;
                
                if (slot.quantity <= count)
                {
                        count -= slot.quantity;
                        slots[j, i] = null;
                        continue;
                }
                slots[j, i].quantity -= count;
                return;
            }
        }
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

                var script = slot.GetComponent<InventorySlotUI>();
                script.x = i;
                script.y = j;

                slotsUI[i, j] = slot;
                slots[i, j] = null;

                if (j != 0)
                    continue;

                GameObject hotbarSlot = Instantiate(inventorySlotPrefab, inventoryUI.transform.GetChild(1).transform);
                hotbarSlot.GetComponent<RectTransform>().localPosition =
                    new Vector3(i * multiplierHotbarUI.x + offsetHotbarUI.x, offsetHotbarUI.y, 0);
                
                var scriptHotBar = hotbarSlot.GetComponent<InventorySlotUI>();
                scriptHotBar.x = i;
                scriptHotBar.isHotBar = true;

                slotsHotbarUI[i] = hotbarSlot;
                slotsHotbar[i] = null;
            }
        }
    }

    private void UpdateHotBar()
    {
        Sprite itemSprite;
        bool flag;

        for (int i = 0; i < width; i++)
        {
            Text amountText = slotsHotbarUI[i].transform.GetChild(1).GetComponent<Text>();

            if (slotsHotbar[i] == null)
            {
                itemSprite = null;
                flag = false;
            }
            else
            {
                itemSprite = slotsHotbar[i].item.sprite;
                amountText.text = slotsHotbar[i].quantity.ToString();
                flag = true;
            }

            Image itemImage = slotsHotbarUI[i].transform.GetChild(0).GetComponent<Image>();
            slotsHotbarUI[i].GetComponent<Outline>().enabled = (i == activeHotbarSlot);

            amountText.enabled = flag;
            itemImage.enabled = flag;
            itemImage.sprite = itemSprite;
        }
    }

    private void UpdateUI()
    {
        UpdateHotBar();
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

                Image itemImage = slotsUI[i, j].transform.GetChild(0).GetComponent<Image>();

                amountText.enabled = flag;
                itemImage.enabled = flag;
                itemImage.sprite = itemSprite;
            }
        }
    }

    private void HandUpdate()
    {
        if (slotsHotbar[activeHotbarSlot] == null)
        {
            World.player.SetActiveItem(null, null);
            return;
        }
            
        World.player.SetActiveItem(slotsHotbar[activeHotbarSlot].item, slotsHotbarUI[activeHotbarSlot]);
    }

    private void FixedUpdate()
    {
        HandUpdate();
        UpdateUI();
    }
}
