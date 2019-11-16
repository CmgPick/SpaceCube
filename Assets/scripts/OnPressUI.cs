using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

//DAHEIMSTUDIO OnPressUI
// ON Press UI lets a button interact on press and on relase

public class OnPressUI : MonoBehaviour, IPointerDownHandler, IPointerUpHandler{

    [System.Serializable]
    public class InteractionEvent : UnityEvent{}

    public InteractionEvent OnTouch = new InteractionEvent();
    public InteractionEvent OnRelase = new InteractionEvent();

    //Detect current clicks on the GameObject (the one with the script attached)
    public void OnPointerDown(PointerEventData pointerEventData){

        OnTouch.Invoke();
    }

    //Detect if clicks are no longer registering
    public void OnPointerUp(PointerEventData pointerEventData){

        OnRelase.Invoke();
    }

}




