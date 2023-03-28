using UnityEngine;
using UnityEngine.Tilemaps;
using UniRx;
using Unity.VisualScripting;
using Zenject;

public class BuildingSystem_sc : MonoBehaviour
{
    [Inject] private DiContainer _container;

    // todo:既然有繼承，為什麼不直接用Grid就好？(目前用Grid，沒出錯)
    // public GridLayout gridLayout;

    public Grid grid;

    [SerializeField] private Tilemap mainTilemap;
    [SerializeField] private TileBase crossPipeTileBase;
    public GameObject preBox0, preChest0;

    PlaceableObject_sc _object2Place;

    #region Unity Methods

    public void Awake()
    {
        // grid = gridLayout.gameObject.GetComponent<Grid>();
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
                // print("yes.");
                _object2Place.Place();
                Vector3Int start = grid.WorldToCell(_object2Place.V3GetStartPosition());
                // Vector3Int start = gridLayout.WorldToCell(_object2Place.V3GetStartPosition());
                TakeArea(start, _object2Place.V3Size);
            }
            else
            {
                Destroy(_object2Place.gameObject);
            }
        }

        if (Input.GetKeyDown(KeyCode.Return))
        {
            _object2Place.RotatePrefab();
        }
    }

    #endregion

    #region Utils(工具)

    /// 滑鼠的位置轉為射線
    public Vector3 V3GetMouseWorldPosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        // RaycastHit[] hits = Physics.RaycastAll(ray);
        RaycastHit[] hits = new RaycastHit[5];
        var size = Physics.RaycastNonAlloc(ray, hits);

        bool b = false;
        if (size > 0)
        {
            for (int i = 0; i < size; i++)
            {
                print(hits[i].transform.gameObject);
                if (hits[i].transform.gameObject.CompareTag($"Plane"))
                {
                    print("In plane.");
                    b = true;
                }
            }
        }

        return b ? hits[0].point : Vector3.zero;
        // return Physics.Raycast(ray, out RaycastHit hit) ? hit.point : Vector3.zero;
    }

    /// 將座標捕捉到網格中點(只要在網格內，就回傳該網格正中間的位置)
    public Vector3 SnapCoordinateToGrid(Vector3 v3)
    {
        //世界座標到最近的一格Grid
        Vector3Int cellPos = grid.WorldToCell(v3);
        // Vector3Int cellPos = gridLayout.WorldToCell(v3);

        //取那一格的正中間位置的世界座標
        Vector3 centerWorld = grid.GetCellCenterWorld(cellPos);
        return centerWorld;
    }

    //取得指定區域的全部tilemap(為了判斷是否可以放置prefab)
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

    /// <summary>
    /// 將指定的prefab設為_object2Place
    /// </summary>
    /// <param name="prefab"></param>
    private void InitializeWithObject(GameObject prefab)
    {
        Vector3 position = SnapCoordinateToGrid(Vector3.zero);
        var obj = _container.InstantiatePrefab(prefab, position, Quaternion.identity, null);
        DragObject_sc o = obj.GetOrAddComponent<DragObject_sc>();
        _container.Inject(o);
        _object2Place = obj.gameObject.GetComponent<PlaceableObject_sc>();
    }

    bool CanBePlaced(PlaceableObject_sc placeable)
    {
        // BoundsInt是Unity中的一個結構體，用於表示一個整數邊界框。
        // BoundsInt包含一個位置向量position(Vector3Int)和一個大小向量size(Vector3Int)，分別表示邊界框的位置和大小。
        // BoundsInt可以用於在整數網格上進行碰撞檢測、物體選擇等操作。
        BoundsInt area = new BoundsInt
        {
            position = grid.WorldToCell(_object2Place.V3GetStartPosition()),
            // area.position = gridLayout.WorldToCell(_object2Place.V3GetStartPosition());
            size = placeable.V3Size
        };

        //不+1的話會無條件捨去小數點，這樣只佔一格的prefab會因為長寬都略小於1，就等於不佔空間了
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

    //確定要把物品擺在那之後，直接把空間佔據不給別人擺
    private void TakeArea(Vector3Int start, Vector3Int size)
    {
        // Collider大小會直接影響到佔地面積
        // BoxFill:從起點開始一長寬高填滿tile base
        mainTilemap.BoxFill(start, crossPipeTileBase, start.x, start.y, start.x + size.x, start.y + size.y);
        _object2Place = null;
    }
}