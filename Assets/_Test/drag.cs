using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class drag : MonoBehaviour
{
    private void OnMouseDrag()
    {
        Vector3 xy = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 position = transform.position;
        transform.position = new Vector3(xy.x, xy.y, position.z);
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }
}