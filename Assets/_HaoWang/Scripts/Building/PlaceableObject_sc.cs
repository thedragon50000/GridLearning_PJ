using System;
using UnityEngine;
using Zenject;

public class PlaceableObject_sc : MonoBehaviour
{
    [Inject] private BuildingSystem_sc _buildingSystem;
    
    private bool Placed { get; set; }
    public Vector3Int V3Size { get; private set; }

    private Vector3[] _v3Vertices;

    private void Start()
    {
        GetColliderVertexPositionsLocal();
        CalculateSizeInCells();
    }

    /// <summary>
    /// 
    /// </summary>
    private void GetColliderVertexPositionsLocal()
    {
        BoxCollider b = GetComponent<BoxCollider>();
        _v3Vertices = new Vector3[4];
        var center = b.center;
        Vector3 bSize = b.size;

        //只需要地上的四個角，因此y可以直接=0。看似三維，實際上還是二維操作，共2*2=4個座標
        _v3Vertices[0] = center + new Vector3(-bSize.x, -bSize.y, -bSize.z) * 0.5f;
        _v3Vertices[1] = center + new Vector3(bSize.x, -bSize.y, -bSize.z) * 0.5f;
        _v3Vertices[2] = center + new Vector3(bSize.x, -bSize.y, bSize.z) * 0.5f;
        _v3Vertices[3] = center + new Vector3(-bSize.x, -bSize.y, bSize.z) * 0.5f;
    }

    /// <summary>
    /// 計算prefab的Size
    /// </summary>
    private void CalculateSizeInCells()
    {
        Vector3Int[] v3Int = new Vector3Int[_v3Vertices.Length];

        for (int i = 0; i < _v3Vertices.Length; i++)
        {
            //每個點的世界座標
            Vector3 worldPos = transform.TransformPoint(_v3Vertices[i]);

            //每個點的網格座標
            // v3Int[i] = _buildingSystem.gridLayout.WorldToCell(worldPos);
            v3Int[i] = _buildingSystem.grid.WorldToCell(worldPos);
        }

        //四邊形的其中兩個點減掉原點(0,0,0)的就是長&寬
        V3Size = new Vector3Int(Mathf.Abs((v3Int[0] - v3Int[1]).x),
            Mathf.Abs((v3Int[0] - v3Int[3]).y), 1);
    }

    /// <summary>
    /// 取得對prefab來說是(0,0,0)的那個點的位置(旋轉的重心)
    /// </summary>
    /// <returns></returns>
    public Vector3 V3GetStartPosition()
    {
        return transform.TransformPoint(_v3Vertices[0]);
    }


    public virtual void Place()
    {
        DragObject_sc drag = GetComponent<DragObject_sc>();
        Destroy(drag);
        Placed = true;
    }

    // 旋轉prefab時也要轉
    public void RotatePrefab()
    {
        transform.Rotate(0, 90, 0);
        V3Size = new Vector3Int(V3Size.y, V3Size.x, 1);

        
        Vector3[] temp = new Vector3[_v3Vertices.Length];
        for (int i = 0; i < temp.Length; i++)
        {
            temp[i] = _v3Vertices[(i + 1) % _v3Vertices.Length];
        }
        
        _v3Vertices = temp;
    }
}