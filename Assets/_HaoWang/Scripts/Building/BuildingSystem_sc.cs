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
    [Inject] private DiContainer _container;

    //todo:既然有繼承，為什麼不直接用Grid就好？
    public GridLayout gridLayout;

    public Grid grid;

    [SerializeField] private Tilemap mainTilemap;
    [SerializeField] private TileBase crossPipeTileBase;
    public GameObject preBox0, preChest0;

    PlaceableObject_sc _object2Place;

    #region Unity Methods

    public void Awake()
    {
        grid = gridLayout.gameObject.GetComponent<Grid>();
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

    private void Update()
    {
        if (!_object2Place)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (CanBePlaced(_object2Place))
            {
                print("yes.");
                _object2Place.Place();
                //todo: can't use grid?
                Vector3Int start = gridLayout.WorldToCell(_object2Place.V3GetStartPosition());
                TakeArea(start, _object2Place.Size);
            }
            else
            {
                Destroy(_object2Place.gameObject);
            }
        }
    }

    #endregion

    #region Utils

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
        Vector3 centerWorld = grid.GetCellCenterWorld(cellPos);
        return centerWorld;
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

    #endregion

    #region Building Placement

    private void InitializeWithObject(GameObject prefab)
    {
        Vector3 position = SnapCoordinateToGrid(Vector3.zero);
        var obj = _container.InstantiatePrefab(prefab, position, Quaternion.identity, null);
        DragObject_sc o = obj.AddComponent<DragObject_sc>();
        _container.Inject(o);
        _object2Place = obj.gameObject.GetComponent<PlaceableObject_sc>();
    }

    bool CanBePlaced(PlaceableObject_sc placeable)
    {
        BoundsInt area = new BoundsInt();
        //todo: 不能不用gridLayout了?
        area.position = gridLayout.WorldToCell(_object2Place.V3GetStartPosition());
        area.size = placeable.Size;
        //不+1的話會無條件捨去小數點，只佔一格的prefab就等於不佔空間了
        area.size = new Vector3Int(area.size.x + 1, area.size.y + 1, area.size.z);

        TileBase[] baseArray = GetTilesBlock(area, mainTilemap);
        foreach (TileBase b in baseArray)
        {
            if (b == crossPipeTileBase)
            {
                return false;
            }
        }

        return true;
    }

    #endregion

    //確定要把物品擺在那之後，直接把地佔據不給別人擺
    public void TakeArea(Vector3Int start, Vector3Int size)
    {
        //Collider大小會直接影響到佔地面積
        mainTilemap.BoxFill(start, crossPipeTileBase, start.x, start.y, start.x + size.x, start.y + size.y);
    }
}