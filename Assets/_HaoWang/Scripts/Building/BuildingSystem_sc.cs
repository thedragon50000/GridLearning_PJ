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
    //todo:既然有繼承，為什麼不直接用Grid就好？ (改為直接用Grid,目前沒有錯誤)
    // public GridLayout gridLayout;

    public Grid grid;

    [SerializeField] private Tilemap _tilemap;
    [SerializeField] private TileBase crossPipeTileBase;

    public GameObject preBox0, preChest0;
    PlaceableObject_sc _gameObject2Place;
    [Inject] private DiContainer _container;

    public void Awake()
    {
        // _grid = gridLayout.gameObject.GetComponent<Grid>();
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
        // Vector3Int cellPos = gridLayout.WorldToCell(v3);
        Vector3Int cellPos = grid.WorldToCell(v3);

        //一格取中間位置
        Vector3 centerWorld = grid.GetCellCenterWorld(cellPos);
        return centerWorld;
    }

    public void InitializeWithObject(GameObject prefab)
    {
        Vector3 position = SnapCoordinateToGrid(Vector3.zero);
        var obj = _container.InstantiatePrefab(prefab, position, Quaternion.identity, null);
        DragObject_sc o = obj.AddComponent<DragObject_sc>();
        _container.Inject(o);
        _gameObject2Place = obj.gameObject.GetComponent<PlaceableObject_sc>();
    }

    //todo: what is this?
    TileBase[] GetTilesBlock(BoundsInt area, Tilemap tilemap)
    {
        TileBase[] array = new TileBase[area.size.x * area.size.y * area.size.z];
        int counter = 0;
        foreach (var a in area.allPositionsWithin)
        {
            Vector3Int pos = new Vector3Int(a.x, a.y, 0);
            array[counter] = tilemap.GetTile(pos);
            counter++;
        }

        return array;
    }

    bool CanBePlaced(PlaceableObject_sc placeable)
    {
        BoundsInt area = new BoundsInt();
        area.position = grid.WorldToCell(_gameObject2Place.V3GetStartPosition());
        area.size = _gameObject2Place.v3Size;

        TileBase[] baseArray = GetTilesBlock(area, _tilemap);
        foreach (TileBase b in baseArray)
        {
            if (b == crossPipeTileBase)
            {
                return false;
            }
        }

        return true;
    }
}