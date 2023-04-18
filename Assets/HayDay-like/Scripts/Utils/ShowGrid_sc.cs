using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[ExecuteInEditMode]
[RequireComponent(typeof(Grid))]
public class ShowGrid_sc : MonoBehaviour
{
    // void OnDrawGizmos()
    // {
    //     Grid grid = GetComponent<Grid>();
    //     Gizmos.color = Color.red;
    //     
    //     // for (int x = 0; x < grid.transform.lossyScale.x * grid.cellSize.x; x++)
    //     for (int x = 0; x < 100; x++)
    //     {
    //         Gizmos.DrawLine(
    //             grid.transform.position + new Vector3(grid.cellSize.x * x, 0, 0),
    //             grid.transform.position +
    //             new Vector3(grid.cellSize.x * x, 0, 100)
    //         );
    //     }
    //
    //     for (int y = 0; y < 100; y++)
    //     {
    //         Gizmos.DrawLine(
    //             grid.transform.position + new Vector3(0, 0, grid.cellSize.y * y),
    //             grid.transform.position +
    //             new Vector3(100, 0, grid.cellSize.y * y)
    //         );
    //     }
    // }
}