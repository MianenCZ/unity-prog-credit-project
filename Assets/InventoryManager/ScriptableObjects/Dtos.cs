using UnityEngine;

/// <summary>
/// Contains ItemData with specified amount.
/// </summary>
[System.Serializable]
public class ItemAmountListing
{
    [Tooltip("Definition of item. Note: Should not be empty.")]
    public ItemData ItemData;
    [Tooltip("Amount of items in listing. Note: Should be positive number.")]
    public int Amount;

    public ItemAmountListing()
    {

    }

    public ItemAmountListing(ItemData source, int amount)
    {
        ItemData = source;
        Amount = amount;
    }

    public override string ToString()
    {
        return $"{ItemData?.InternalName} {Amount}x"; ;
    }

    /// <summary>
    /// Creates shallow clone of <see cref="ItemAmountListing"/>
    /// </summary>
    /// <remarks>Reference field <see cref="ItemData"/> is copied.</remarks>
    /// <returns>New instance of <see cref="ItemAmountListing"/></returns>
    public ItemAmountListing Clone()
    {
        return new ItemAmountListing(ItemData, Amount);
    }
}