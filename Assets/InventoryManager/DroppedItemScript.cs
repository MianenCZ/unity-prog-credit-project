using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class DropedItemsScript : MonoBehaviour
{
    public ItemAmountListing ItemAmount;

    private Collider assignedCollider;
    private Rigidbody m_Rigidbody;
    private Vector3 m_EulerAngleVelocity;

    void FixedUpdate()
    {
        Quaternion deltaRotation = Quaternion.Euler(m_EulerAngleVelocity * Time.fixedDeltaTime);
        m_Rigidbody.MoveRotation(m_Rigidbody.rotation * deltaRotation);
    }


    private void Awake()
    {
        //Fetch the Rigidbody from the GameObject with this script attached
        m_Rigidbody = GetComponent<Rigidbody>();

        //Set the angular velocity of the Rigidbody (rotating around the Y axis, 100 deg/sec)
        m_EulerAngleVelocity = new Vector3(0, 50, 0);


        assignedCollider = GetComponent<Collider>();
    }

    private void Start()
    {
        gameObject.transform.name = ItemAmount.ToString();
    }

    public override string ToString()
    {
        return ItemAmount.ToString();
    }


    public void OnCollisionEnter(Collision collision)
    {
        var otherScript = collision.transform.GetComponent<DropedItemsScript>();
        if (otherScript != null)
        {
            if(otherScript.ItemAmount.ItemData == this.ItemAmount.ItemData
                && this.ItemAmount.ItemData.Stackable
                && otherScript.ItemAmount.Amount + this.ItemAmount.Amount <= this.ItemAmount.ItemData.StackSize)
            {
                var newSize = otherScript.ItemAmount.Amount + this.ItemAmount.Amount;
                (DropedItemsScript toUpdate, GameObject toDestroy) = CalculateMerge(this, otherScript);
                Debug.Log($"Dropped items '{this}' and '{otherScript}' combined");
                toUpdate.ItemAmount.Amount = newSize;
                toUpdate.transform.name = toUpdate.ItemAmount.ToString();
                Destroy(toDestroy);
            }
            var isCollection= collision.transform.GetComponent<DropCollectionScript>() != null;
            if(!isCollection)
                Physics.IgnoreCollision(assignedCollider, collision.collider);
        }
    }

    private static (DropedItemsScript toUpdate, GameObject toDestroy) CalculateMerge(DropedItemsScript a, DropedItemsScript b)
    {
        if (a == null || b == null)
            throw new System.ArgumentNullException();

        if(a.ItemAmount.Amount != b.ItemAmount.Amount)
        {
            if (a.ItemAmount.Amount > b.ItemAmount.Amount)
                return (a, b.gameObject);
            return (b, a.gameObject);
        }

        if (a.GetInstanceID() > b.GetInstanceID())
            return (a, b.gameObject);
        return (b, a.gameObject);
    }

}
