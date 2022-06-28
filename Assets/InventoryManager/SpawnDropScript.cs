using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnDropScript : MonoBehaviour
{
    public LootData LootTable;

    public void Drop()
    {
        if( LootTable == null )
        {
            Debug.LogError("LootTable is missing.");
            return;
        }

        var drops = LootData.CalculateLoot(LootTable);

        foreach (var drop in drops)
        {
            var stackSize = drop.ItemData.Stackable ? drop.ItemData.StackSize : 1;
            int droppedAmount = 0;
            while(droppedAmount < drop.Amount)
            {
                GameObject newDrop = new GameObject();
                var meshRenderer = newDrop.AddComponent<MeshRenderer>();
                meshRenderer.material = drop.ItemData.Material;
                var meshFilter = newDrop.AddComponent<MeshFilter>();
                meshFilter.mesh = drop.ItemData.Mesh;

                newDrop.transform.position = this.transform.position + drop.ItemData.ItemIngameOffset + GetRandomEpsilonVector();
                newDrop.transform.localScale = drop.ItemData.ItemIngameScale;

                var rigidbody = newDrop.AddComponent<Rigidbody>();
                rigidbody.AddExplosionForce(2f, transform.position, 0.20f, 0.25f, ForceMode.Impulse);
                rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
                rigidbody.drag = 0.7f;

                newDrop.AddComponent<CapsuleCollider>();
                
                var droppedItemScript = newDrop.AddComponent<DropedItemsScript>();
                droppedItemScript.ItemAmount = new ItemAmountListing()
                {
                    Amount = Mathf.Min(drop.Amount, stackSize),
                    ItemData = drop.ItemData,
                };
                droppedAmount += droppedItemScript.ItemAmount.Amount;
            }
        }

    }

    private const float slack = 0.2f;
    private Vector3 GetRandomEpsilonVector()
    {

        return new Vector3(Random.Range(0f, slack), Random.Range(0f, slack), Random.Range(0f, slack));
    }
}
