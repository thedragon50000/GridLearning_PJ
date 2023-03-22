using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class collision : MonoBehaviour
{
    public Collider2D c;

    private void OnCollisionEnter2D(Collision2D col)
    {
        print(col);
        print(col.gameObject);

        if (col.collider == c)
        {
            print("hit c");
        }
    }
}