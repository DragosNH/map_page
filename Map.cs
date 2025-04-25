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