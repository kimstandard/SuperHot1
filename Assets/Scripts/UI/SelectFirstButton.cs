using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SelectFirstButton : MonoBehaviour
{
    public Button firstButton;

    void Start()
    {
        Cursor.visible = false;
        EventSystem.current.SetSelectedGameObject(firstButton.gameObject);
    }
}