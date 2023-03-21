using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.Tilemaps;
using UniRx;
using Zenject;

public class BuildingSystem_sc : MonoBehaviour
{
    //todo:既然有繼承，為什麼不直接用Grid就好？
    public GridLayout gridLayout;

    Grid _grid;

    [SerializeField] private Tilemap tilemap;
    [SerializeField] private TileBase crossPipeTileBase;

    public GameObject preBox0, preChest0;
    PlaceableObject_sc _gameObject2Place;
    [Inject] private DiContainer _container;

    public void Awake()
    {
        _grid = gridLayout.gameObject.GetComponent<Grid>();
    }

    public void Start()
    {
        Observable.EveryUpdate().Subscribe(_ =>
            {
                if (Input.GetKeyUp(KeyCode.A))
                {
                    print("A");
                    InitializeWithObject(preChest0);
                }
                else if (Input.GetKeyUp(KeyCode.B))
                {
                    print("B");
                    InitializeWithObject(preBox0);
                }
            }
        );
    }

    public Vector3 V3GetMouseWorldPosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            return hit.point;
        }
        else
        {
            return Vector3.zero;
        }
    }

    public Vector3 SnapCoordinateToGrid(Vector3 v3)
    {
        //世界座標到最近的一格Grid
        Vector3Int cellPos = gridLayout.WorldToCell(v3);

        //一格取中間位置
        Vector3 centerWorld = _grid.GetCellCenterWorld(cellPos);
        return centerWorld;
    }

    public void InitializeWithObject(GameObject prefab)
    {
        Vector3 position = SnapCoordinateToGrid(Vector3.zero);
        var obj = _container.InstantiatePrefab(prefab, position, Quaternion.identity, null);
        // GameObject obj = Instantiate(prefab, position, Quaternion.identity);
        DragObject_sc o = obj.AddComponent<DragObject_sc>();
        _container.Inject(o);
        _gameObject2Place = obj.gameObject.GetComponent<PlaceableObject_sc>();
    }
}