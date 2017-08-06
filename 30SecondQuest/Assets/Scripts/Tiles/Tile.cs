using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{

    private float _moveSpeed = 0.25f, _scaleSpeed = 0.1f;
    private Coroutine _moveCoroutine;
    [SerializeField] private MeshFilter _tileModel;
    public bool isTraversable = true;

    private enum TileType
    {
        NOTSET, CROSSROAD, CORNER, STRAIGHT, THREEWAY, ONEWAY
    }
    private TileType _currType = TileType.NOTSET;
    private Mesh _currMesh;

    [Header("On Use")]
    [SerializeField]
    private GameObject _onUseEffect = null;
    [SerializeField] private Vector3 _effectOffset = Vector3.zero;

    protected virtual void Start()
    {
        Vector3 localScale = transform.localScale;
        transform.localScale = new Vector3(0, 0, 0);
        StartCoroutine(scaleUp(localScale));
    }


    void Update()
    {

    }

    public virtual void use(PlayerController player)
    {
        if (!isTraversable)
        {
            player.useBombs(1);
        }

        if (_onUseEffect != null)
        {
            GameObject effect = Instantiate(_onUseEffect);
            effect.transform.position = this.transform.position + _effectOffset;
        }
    }

    public virtual void updateTile(bool[] directions)
    {
        if (_tileModel == null)
            return;

        int paths = 0;
        for (int i = 0; i < directions.Length; ++i)
            if (directions[i])
                paths++;


        switch (paths)
        {
            case 4:
                {
                    if (_currType != TileType.CROSSROAD)
                        _tileModel.transform.rotation = Quaternion.Euler(new Vector3(0.0f, Random.Range(0, 4) * 90.0f, 0.0f));
                    _tileModel.mesh = pickCrossroadModel();
                    break;
                }
            case 3:
                {
                    Mesh tileMesh = pickThreewayModel();
                    _tileModel.mesh = tileMesh;

                    if (!directions[(int)Direction.UP])
                        _tileModel.transform.rotation = Quaternion.Euler(new Vector3(0.0f, 90.0f, 0.0f));

                    else if (!directions[(int)Direction.DOWN])
                        _tileModel.transform.rotation = Quaternion.Euler(new Vector3(0.0f, 270.0f, 0.0f));

                    else if (!directions[(int)Direction.RIGHT])
                        _tileModel.transform.rotation = Quaternion.Euler(new Vector3(0.0f, 180.0f, 0.0f));

                    else
                        _tileModel.transform.rotation = Quaternion.Euler(new Vector3(0.0f, 0.0f, 0.0f));

                    break;
                }
            case 2:
                {
                    if (directions[(int)Direction.UP] && directions[(int)Direction.DOWN])
                    {
                        Mesh tileMesh = pickStraightModel();
                        _tileModel.mesh = tileMesh;
                    }
                    else if (directions[(int)Direction.LEFT] && directions[(int)Direction.RIGHT])
                    {
                        Mesh tileMesh = pickStraightModel();
                        _tileModel.mesh = tileMesh;
                        _tileModel.transform.rotation = Quaternion.Euler(new Vector3(0.0f, 90.0f, 0.0f));
                    }
                    else if (directions[(int)Direction.UP] && directions[(int)Direction.RIGHT])
                    {
                        Mesh tileMesh = pickCornerModel();
                        _tileModel.mesh = tileMesh;
                    }
                    else if (directions[(int)Direction.RIGHT] && directions[(int)Direction.DOWN])
                    {
                        Mesh tileMesh = pickCornerModel();
                        _tileModel.mesh = tileMesh;
                        _tileModel.transform.rotation = Quaternion.Euler(new Vector3(0.0f, 90.0f, 0.0f));
                    }
                    else if (directions[(int)Direction.DOWN] && directions[(int)Direction.LEFT])
                    {
                        Mesh tileMesh = pickCornerModel();
                        _tileModel.mesh = tileMesh;
                        _tileModel.transform.rotation = Quaternion.Euler(new Vector3(0.0f, 180.0f, 0.0f));
                    }
                    else if (directions[(int)Direction.LEFT] && directions[(int)Direction.UP])
                    {
                        Mesh tileMesh = pickCornerModel();
                        _tileModel.mesh = tileMesh;
                        _tileModel.transform.rotation = Quaternion.Euler(new Vector3(0.0f, 270.0f, 0.0f));
                    }
                    else
                        _tileModel.transform.rotation = Quaternion.Euler(new Vector3(0.0f, 0.0f, 0.0f));

                    break;
                }
            case 1:
                {
                    Mesh tileMesh = pickOnewayModel();
                    _tileModel.mesh = tileMesh;

                    if (directions[(int)Direction.UP])
                        _tileModel.transform.rotation = Quaternion.Euler(new Vector3(0.0f, 270.0f, 0.0f));

                    else if (directions[(int)Direction.DOWN])
                        _tileModel.transform.rotation = Quaternion.Euler(new Vector3(0.0f, 90.0f, 0.0f));

                    else if (directions[(int)Direction.LEFT])
                        _tileModel.transform.rotation = Quaternion.Euler(new Vector3(0.0f, 180.0f, 0.0f));

                    else
                        _tileModel.transform.rotation = Quaternion.Euler(new Vector3(0.0f, 0.0f, 0.0f));

                    break;
                }
            default:
                {
                    break;
                }
        }
    }

    private Mesh pickCrossroadModel()
    {
        if (_currType == TileType.CROSSROAD)
            return _currMesh;

        List<string> tileOptions = new List<string>() { "crossroad", "crossroadruins", "crossroadhouse"};
        _currMesh = (Mesh)Resources.Load("Tiles/" + tileOptions[Random.Range(0, tileOptions.Count)], typeof(Mesh));

        _currType = TileType.CROSSROAD;
        return _currMesh;
    }

    private Mesh pickStraightModel()
    {
        if (_currType == TileType.STRAIGHT)
            return _currMesh;

        List<string> tileOptions = new List<string>() { "straight", "straightfarm", "straighttrees" };
        _currMesh = (Mesh)Resources.Load("Tiles/" + tileOptions[Random.Range(0, tileOptions.Count)], typeof(Mesh));

        _currType = TileType.STRAIGHT;
        return _currMesh;
    }

    private Mesh pickCornerModel()
    {
        if (_currType == TileType.CORNER)
            return _currMesh;

        List<string> tileOptions = new List<string>() { "corner", "cornertown", "cornercrops", "cornerfort" };
        _currMesh = (Mesh)Resources.Load("Tiles/" + tileOptions[Random.Range(0, tileOptions.Count)], typeof(Mesh));

        _currType = TileType.CORNER;
        return _currMesh;
    }

    private Mesh pickThreewayModel()
    {
        if (_currType == TileType.THREEWAY)
            return _currMesh;

        List<string> tileOptions = new List<string>() { "threeway", "threewayrice", "threewaypumpkin"};
        _currMesh = (Mesh)Resources.Load("Tiles/" + tileOptions[Random.Range(0, tileOptions.Count)], typeof(Mesh));

        _currType = TileType.THREEWAY;
        return _currMesh;
    }

    private Mesh pickOnewayModel()
    {
        if (_currType == TileType.ONEWAY)
            return _currMesh;

        List<string> tileOptions = new List<string>() { "oneway", "onewayrocks", "onewaytrees", "onewaywater" };
        _currMesh = (Mesh)Resources.Load("Tiles/" + tileOptions[Random.Range(0, tileOptions.Count)], typeof(Mesh));

        _currType = TileType.ONEWAY;
        return _currMesh;
    }

    public void moveTo(Vector3 newPos)
    {
        if (_moveCoroutine != null)
            StopCoroutine(_moveCoroutine);
        _moveCoroutine = StartCoroutine(lerpMoveTo(newPos));
    }

    private IEnumerator lerpMoveTo(Vector3 newPos)
    {
        while (Vector3.Distance(transform.position, newPos) > 0.01f)
        {
            transform.position = Vector3.Lerp(transform.position, newPos, _moveSpeed);
            yield return null;
        }
        transform.position = newPos;
    }

    private IEnumerator scaleUp(Vector3 newScale)
    {
        while (Mathf.Abs(transform.localScale.magnitude - newScale.magnitude) > 0.01f)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, newScale, _scaleSpeed);
            yield return null;
        }
        transform.localScale = newScale;
    }

}
