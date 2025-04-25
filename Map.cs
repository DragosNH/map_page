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

    void LoadMapGrid(int centerX, int centerY, int z)
    {
        // Clear existing tiles
        foreach(transform child in tileContainer)
        {
            Destroy(child.gameObject);
        }

        // Get container dimesnions
        RectTransform containerRect = tileContainer.GetComponent<RectTransform>();
        float containerWidth = containerRect.rect.width;
        float containerHeight = containerRect.rech.height;
        float tileSize = 256f;

        // number of rows and cold to cover the container
        int columns = Mathf.CeilToInt(containerWidth / tileSize) + 5;
        int rows = Mathf.CeilToInt(containerHeight / tileSize) + 5;

        // Calculate base offset to center the grid
        float horizontalOffset = (columns * tileSize - containerWidth) / 2f;
        float verticalOffset = (rows * tileSize - containerHeight) / 2f;
        float adjustedHorizontalOffset = horizontalOffset + horisontalNudge;
        Debug.Log($"HorizontalOffset: {horizontalOffset}, horisontalNudge: {horisontalNudge}, adjustedHorizontalOffset: {adjustedHorizontalOffset}");

        // Debug log container info
        Debug.Log($"Container width: {containerWidth}, Height: {containerHeight}, Pivot: {containerRect.pivot}");
        Debug.Log($"Calculated horizontal offset: {horizontalOffset}, vertical offset: {verticalOffset}");

        // instantiate tiles
        for (int row = 0; row < rows; row++)
        {
            for(int col = 0; col < columns; col++)
            {
                // Compute tile's anchored position.
                float posX = col * tileSize - adjustedHorizontalOffset;
                float posY = row * tileSize - verticalOffset;

                // Compute tile grid coordinates.
                int tileGridX = centerX + col - columns / 2;
                int tileGridY = centerY - (row - rows / 2);

                // Instantiate tile.
                GameObject tileObj = Instantiate(tilePrefab, tileContainer);
                tileObj.transform.localScale = Vector3.one;

                // Force native scale.
                tileObj.transform.localScale = Vector3.one;

                // Set size and position.
                RectTransform rt = tileObj.GetComponent<RectTransform>();
                rt.sizeDelta = new Vector2(tileSize, tileSize);
                rt.anchoredPosition = new Vector2(posX, posY);

                // Load tile texture.
                RawImage rawImage = tileObj.GetComponent<RawImage>();
                StartCoroutine(LoadTile(tileGridX, tileGridY, z, rawImage));

                // Optional debug color.
                rawImage.color = new Color(0.95f, 1f, 0.95f);
            }
        }
    }

    IEnumerator LoadTile(int x, int y, int z, RawImage targetImage)
    {
        string url = $"https://tile.openstreetmap.org/{z}/{x}/{y}.png";
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(url);
        yield return request.SendWebRequest();

        if (targetImage == null)
            yield break;

#if UNITY_2020_1_OR_NEWER
        if (request.result == UnityWebRequest.Result.Success)
#else
        if (!request.isNetworkError && !request.isHttpError)
#endif
        {
            Texture2D tex = DownloadHandlerTexture.GetContent(request);
            if(tex != null)
            {
                tex.filterMode = filterMode.Point;
                tex.wrapMode = TextureWrapMode.Clamp;
                targetImage.texture = tex;
                targetImage.rectTransform.sizeDelta = new Vector2(tex.width, tex.height);
            }
            else
            {
                Debug.LogError($"Texture was null for tile {x}, {y}");
            }
        else
        {
            Debug.LogError($"Failed to load tile {x}, {y}: {request.error}");
        }
        }
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