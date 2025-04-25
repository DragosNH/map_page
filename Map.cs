using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using TMPro;
using Unity.VisualScripting;

public class MapScene : MonoBehaviour
{
    public GameObject popupMenu;
    public PopupFader popupFader;
    // Drag handler
    public MapDragHandler mapDragHandler;
    public float horisontalNudge = -100f;

    //-- Map Variables --
    public RawImage mapImage;
    public int zoom = 15;
    public int tileX = 17184;
    public int tileY = 11646;

    //-- Tiles --
    public GameObject tilePrefab;
    public Transform tileContainer;

    //-- Buttons --
    public Button zoomInButton;
    public Button zoomOutButton;

    //-- User location mark --
    public GameObject userMarkerPrefab;
    public GameObject userMarkerInstance;

    void Start()
    {

    }

    void HandleDragFinished(Vector2 finalOffset)
    {
        // Calculate tile cooronates
        int dx = Mathf.RoundToInt(-finalOffset.x / 256f);
        int dy = Mathf.RoundToInt(finalOffset.y / 256f);

        tileX += dx;
        tileY += dy;

        LoadMapGrid(tileX, tileY, zoom);
    }

    // Recalculate center tile on zoom
    void ChangeZoom(int delta)
    {
        zoom += delta;
        zoom = Mathf.Clamp(zoom, 1, 19);

        float mulhouseLat = 47.7508f;
        float mulhouseLon = 7.3359f;

        tileX = (int)((mulhouseLon + 180.0f) / 360.0f * Mathf.Pow(2, zoom));
        tileY = (int)((1.0f - Mathf.Log(Mathf.Tan(mulhouseLat * Mathf.Deg2Rad) + 1.0f / Mathf.Cos(mulhouseLat * Mathf.Deg2Rad)) / Mathf.PI) / 2.0f * Mathf.Pow(2, zoom));

        tileX = Mathf.Clamp(tileX, 0, (int)Mathf.Pow(2, zoom) - 1);
        tileY = Mathf.Clamp(tileY, 0, (int)Mathf.Pow(2, zoom) - 1);

        foreach (Transform child in tileContainer)
        {
            Destroy(child.gameObject);
        }

        LoadMapGrid(tileX, tileY, zoom);
    }

    public void ShowPopUpMenu()
    {
        popupMenu.SetActive(true);
        popupFader.FadeIn();
        Debug.Log("Clicked");
    }

    public void HidePopupMenu()
    {
        popupFader.FadeOut();
    }

}