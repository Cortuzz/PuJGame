using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapController : MonoBehaviour, IObserver
{
    private Texture2D _mapTexture;
    private bool _isOpened = false;
    private GameObject _mapImage;
    private GameObject _map;

    public void ObserverUpdate(IObservable observable)
    {
        throw new System.NotImplementedException();
    }

    private void UpdateChunks()
    {
        for (int chunk = 0; chunk < World.chunkCount; chunk++)
        {
            for (int i = 0; i < World.chunkSize; i++)
            {
                for (int j = 0; j < World.height; j++)
                {
                    Block block = World.blocks[chunk][i, j];
                    if (block == null)
                    {
                        _mapTexture.SetPixel(i + chunk * World.chunkSize, j, new Color(1, 1, 1, 0));
                        continue;
                    }

                    _mapTexture.SetPixel(i + chunk * World.chunkSize, j, block.color);
                }
            }

            _mapTexture.Apply();
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
