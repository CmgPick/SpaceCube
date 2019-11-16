using UnityEngine;

public class DragableUI : MonoBehaviour {

    RectTransform uiRect;
    Vector2 startPosition;

    void Start(){

        uiRect = this.GetComponent<RectTransform>();
        startPosition = uiRect.anchoredPosition;

    }

    public void StartDrag () {

        int touch = Input.touchCount == 1 ? 0 : 1;

        Vector2 touchposition = Input.GetTouch(touch).deltaPosition;
        uiRect.anchoredPosition = touchposition;

    }

    public void StopDrag(){

        uiRect.anchoredPosition = startPosition;

    }


}
