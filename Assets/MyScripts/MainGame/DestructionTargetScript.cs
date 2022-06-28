using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class DestructionTargetScript : MonoBehaviour
{
    public float Dificulty;

    public Vector3 SoftPoint;

    public float MaxHP = 100;
    public float CurrentHP = 100;


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position + (transform.rotation * SoftPoint), 1);
    }

    public bool Damage(float hp)
    {
        CurrentHP -= hp;
        if(CurrentHP < 0)
            gameObject.SetActive(false);

        return CurrentHP < 0;
    }


}
