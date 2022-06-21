using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapController : MonoBehaviour
{
    [SerializeField] private int _mapCheckRadius = 10;
    private Texture2D _mapTexture;
    private GameObject _mapImage;
    private GameObject _map;
    private bool _isOpened = false;

    private void UpdateChunks()
    {
        for (int chunk = 0; chunk < World.chunkCount; chunk++)
            for (int i = 0; i < World.chunkSize; i++)
                for (int j = 0; j < World.height; j++)
                    _mapTexture.SetPixel(i + chunk * World.chunkSize, j, new(0, 0, 0, 1));

        _mapTexture.Apply();
    }

    private void UpdateBlock(int chunk, int x, int y, Block block)
    {
        if (block == null)
        {
            _mapTexture.SetPixel(x + chunk * World.chunkSize, y, new(1, 1, 1, 0));
            return;
        }

        _mapTexture.SetPixel(x + chunk * World.chunkSize, y, block.color);
    }

    public void UpdateMap(Vector2 position)
    {
        for (int i = (int)position.x - _mapCheckRadius; i < position.x + _mapCheckRadius; i++)
        {
            for (int j = (int)position.y - _mapCheckRadius; j < position.y + _mapCheckRadius; j++)
            {
                if (Mathf.Pow(i - position.x, 2) + Mathf.Pow(j - position.y, 2) < Mathf.Pow(_mapCheckRadius, 2))
                    UpdateBlock(i / World.chunkSize, i % World.chunkSize, j, World.GetBlock(i, j));
            }
        }
    }

    private void ZoomUpdate()
    {
        float value = 0.1f * Mathf.Sign(Input.GetAxis("Mouse ScrollWheel"));
        var imScale = _mapImage.transform.localScale;
        var mapScale = _map.transform.localScale;

        if (Input.GetAxis("Mouse ScrollWheel") == 0 || imScale.x + value < 0.5f)
            return;

        _mapImage.transform.localScale = new(imScale.x + value, imScale.y + value, 0);
        _map.transform.localScale = new(mapScale.x + value, mapScale.y + value, 0);
    }

    private void PositionUpdate()
    {
        if (!Input.GetMouseButton(0))
            return;

        var imPos = _mapImage.transform.position;
        var mapPos = _map.transform.position;

        _mapImage.transform.position = new(5 * Input.GetAxis("Mouse X") + imPos.x, 5 * Input.GetAxis("Mouse Y") + imPos.y);
        _map.transform.position = new(5 * Input.GetAxis("Mouse X") + mapPos.x, 5 * Input.GetAxis("Mouse Y") + mapPos.y);
    }

    public void ChangeVisibility()
    {
        _isOpened = !_isOpened;
        gameObject.SetActive(_isOpened);
        World.SetMapState(_isOpened);
        _mapTexture.Apply();
    }

    private void Start()
    {
        gameObject.SetActive(false);
        _mapTexture = new(World.width, World.height);
        UpdateChunks();
                
        _map = new(name = "Map");
        _mapImage = transform.GetChild(0).GetChild(0).gameObject;

        _map.transform.position = transform.GetChild(0).position;
        _map.transform.SetParent(transform.GetChild(0));

        _map.AddComponent<RawImage>().texture = _mapTexture;
        var tr = _map.GetComponent<RawImage>().rectTransform;
        tr.sizeDelta = new(World.width, World.height);
        var imageTr = _mapImage.GetComponent<Image>().rectTransform;
        imageTr.sizeDelta = 1.5f * tr.sizeDelta;
        tr.position = imageTr.position;
    }

    private void Update()
    {
        if (World.CanPlay())
            return;

        ZoomUpdate();
        PositionUpdate();
    }
}
