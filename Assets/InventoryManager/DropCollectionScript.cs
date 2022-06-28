using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropCollectionScript : MonoBehaviour
{
    public StorageScript TargetStorage;
    public Collider collectionCollider;

    public void Awake()
    {
        TargetStorage.ItemRemoved.AddListener(OnStorageItemRemoved);
    }


    public void OnTriggerEnter(Collider other)
    {
        var dropScript = other.gameObject.GetComponent<DropedItemsScript>();
        if(dropScript != null)
        {
            if(TargetStorage.TryInsertExact(dropScript.ItemAmount))
            {
                Destroy(other.gameObject);
            }
        }
    }

    public void OnStorageItemRemoved()
    {

    }
}
