using System;
using UnityEngine;
using Zenject;

public class PlaceableObject_sc : MonoBehaviour
{
    [Inject] private BuildingSystem_sc _buildingSystem;
    private bool Placed { get; set; }
    public Vector3Int Size { get; private set; }
    
    private Vector3[] _v3Vertices;

    public void GetColliderVertexPositionsLocal()
    {
        BoxCollider b = GetComponent<BoxCollider>();
        _v3Vertices = new Vector3[4];
        var center = b.center;
        Vector3 bSize = b.size;
        //只需要地上的四個角，因此y可以直接=0。看似三維，實際上還是二維操作，共2*2=4種情況
        _v3Vertices[0] = center + new Vector3(-bSize.x, -bSize.y, -bSize.z) * 0.5f;
        _v3Vertices[1] = center + new Vector3(bSize.x, -bSize.y, -bSize.z) * 0.5f;
        _v3Vertices[2] = center + new Vector3(bSize.x, -bSize.y, bSize.z) * 0.5f;
        _v3Vertices[3] = center + new Vector3(-bSize.x, -bSize.y, bSize.z) * 0.5f;
    }

    public void CalculateSizeInCells()
    {
        Vector3Int[] v3Int = new Vector3Int[_v3Vertices.Length];

        for (int i = 0; i < _v3Vertices.Length; i++)
        {
            //每個點的世界座標
            Vector3 worldPos = transform.TransformPoint(_v3Vertices[i]);

            //每個點的網格座標
            v3Int[i] = _buildingSystem.gridLayout.WorldToCell(worldPos);
        }

        Size = new Vector3Int(Mathf.Abs((v3Int[0] - v3Int[1]).x),
            Mathf.Abs((v3Int[0] - v3Int[3]).y), 1);
    }

    public Vector3 V3GetStartPosition()
    {
        return transform.TransformPoint(_v3Vertices[0]);
    }

    private void Start()
    {
        GetColliderVertexPositionsLocal();
        CalculateSizeInCells();
    }

    public virtual void Place()
    {
        DragObject_sc drag = GetComponent<DragObject_sc>();
        Destroy(drag);
        Placed = true;
    }
}