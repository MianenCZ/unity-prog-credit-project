using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class StorageScript : MonoBehaviour
{
    public int SlotCount = 16;
    public List<ItemAmountListing> Items = new List<ItemAmountListing>();

    [Header("Settings")]
    public bool AutoSort = false;
    public bool AllowUndropableItems = true;
    public bool RespectStackSize = true;

    public UnityEvent ItemRemoved;
    public UnityEvent ItemAdded;
    public UnityEvent ContentChanged;


    // Start is called before the first frame update
    void Start()
    {
        for (int i = Items.Count; i < SlotCount; i++)
        {
            Items.Add(new ItemAmountListing());
        }
    }

    /// <summary>
    /// Evaluates if 
    /// </summary>
    /// <param name="items"></param>
    /// <returns></returns>
    public bool CanInsertExact(ItemAmountListing items)
    {
        if (!AllowUndropableItems && !items.ItemData.Dropable)
            return false;


        var presentSlot = Items.FirstOrDefault(x => x?.ItemData?.InternalName == items.ItemData.InternalName);
        if (presentSlot != null) // If items.ItemData is already inside storage
        {
            if (!RespectStackSize || presentSlot.Amount + items.Amount <= presentSlot.ItemData.StackSize)
            {
                return true;
            }
            // Fallback to inserting inside new slot when there is not enough space in this slot
        }

        var firstEmptySlot = Items.FirstOrDefault(x => x.ItemData == null);
        return firstEmptySlot != null;
    }

    public bool CanRemoveExact(ItemAmountListing items)
    {
        var slot = Items.FirstOrDefault(x => x?.ItemData?.InternalName == items.ItemData.InternalName);
        return slot != null && slot.Amount >= items.Amount;
    }

    public bool TryInsertExact(ItemAmountListing items)
    {
        if (!AllowUndropableItems && !items.ItemData.Dropable)
            return false;


        var presentSlot = Items.FirstOrDefault(x => x?.ItemData?.InternalName == items.ItemData.InternalName);
        if(presentSlot != null) // If items.ItemData is already inside storage
        {
            if(!RespectStackSize || presentSlot.Amount + items.Amount <= presentSlot.ItemData.StackSize)
            {
                presentSlot.Amount += items.Amount;
                return true;
            }
            // Fallback to inserting inside new slot when there is not enough space in this slot
        }

        var firstEmptySlot = Items.FirstOrDefault(x => x.ItemData == null);

        if(firstEmptySlot != null) // If there is empty slot
        {
            firstEmptySlot.ItemData = items.ItemData;
            firstEmptySlot.Amount = items.Amount;
            return true;
        }
            
        // No space for new item
        return false;
    }

    public bool TryRemoveExact(ItemAmountListing items)
    {
        var slot = Items.FirstOrDefault(x => x?.ItemData?.InternalName == items.ItemData.InternalName);
        
        
        if(slot != null && slot.Amount >= items.Amount) // If there enough is items.ItemData stored
        {
            slot.Amount -= items.Amount;
            if(slot.Amount <= 0)
                slot.ItemData = null;
            return true;
        }

        return false;
    }

    protected virtual void OnItemRemoved()
    {
        ItemRemoved?.Invoke();
        ContentChanged?.Invoke();
    }

    protected virtual void OnItemAdded()
    {
        ItemAdded?.Invoke();
        ContentChanged?.Invoke();
    }
}
