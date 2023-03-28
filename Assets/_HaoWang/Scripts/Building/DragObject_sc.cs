using System;
using UnityEngine;
using Zenject;

public class DragObject_sc : MonoBehaviour
{
    [Inject] public BuildingSystem_sc buildSystem;

    public Vector3 v3Offset;

    private void OnMouseDown()
    {
        // print("MouseDown");
        //避免滑鼠點在太邊邊導致還沒拖曳就直接位移的情形
        v3Offset = transform.position - buildSystem.V3GetMouseWorldPosition();
    }

    private void OnMouseDrag()
    {
        // print("OnMouseDrag");
        Vector3 pos = buildSystem.V3GetMouseWorldPosition() + v3Offset;
        transform.position = buildSystem.SnapCoordinateToGrid(pos);
    }
}