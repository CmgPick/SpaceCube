using UnityEngine.UI;
using UnityEngine;
using UnityEngine.EventSystems;

public class Constructor : MonoBehaviour {

    public Transform ship; //root

    // to prevent placing cubes when not wished
    public bool isAlowed = true;
    public bool isCamMoving = false;
    
    public bool isDeleting = false;

    private Camera cam;
    private int snapPointLayer = 10;
    private int pieceLayer = 11;

    public int PieceNumber = 0; // the current piece index
    public string shipName;

    public Image SelectedPieceImg;
    public GameObject selectedPiece;

    // for the selected piece
    private string selectedPieceName;
    public GameObject placeHolder;

    private BuildUI buildUI;
    private ColorPicker colorPicker;
    public InputField shipNameText;

    // Use this for initialization
    void Start () {

        colorPicker = FindObjectOfType<ColorPicker>();
        buildUI = FindObjectOfType<BuildUI>();

        cam = Camera.main;

        SetStartPiece("SA");
    }

    // do this to get the initial piece selected
     void SetStartPiece(string name){

        selectedPieceName = name;
        selectedPiece = Resources.Load<GameObject>("PlaceHolders/" + selectedPieceName);
        DeleteMode(false);

        SelectedPieceImg.sprite = Resources.Load<Sprite>("Icons/" + selectedPieceName);

        buildUI.HideAllPannels();
    }

    #region SHIP NAME
    public void NewShipName() {

        shipName = shipNameText.text;
    }
    public void ShowShipName(string name){

        shipName = name;
        shipNameText.text = shipName;
    }
    #endregion

    // Update is called once per frame
    void Update () {

        /*
        //set this to true so u can build in editor
        #if UNITY_EDITOR
            isAlowed = true;
        #endif
        */

        if (Input.GetMouseButtonDown(0) && isAlowed) {

            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            // INSTAMTIATE THE PLACEHOLDER 

            if(Physics.Raycast(ray,out hit)){

                if(hit.transform.gameObject.layer == snapPointLayer && !isDeleting && !isCamMoving) {

                    Vector3 spawnPosition = hit.collider.transform.position;
                    GameObject newplaceholder = Instantiate(selectedPiece, spawnPosition, hit.transform.rotation);
                    newplaceholder.transform.parent = hit.transform.parent.parent;

                    placeHolder = newplaceholder;

                    buildUI.TooglePlacingButtons();

                    isAlowed = false;
                }


                if (hit.transform.gameObject.layer == pieceLayer && isDeleting){

                    // MAYBE ADD A CTRL Z FUNCTION FOR THIS?

                        Destroy(hit.transform.parent.gameObject);
                        PieceNumber--;

                }

            }

        }
		
	}

    public void ChangePiece() {

        string name = EventSystem.current.currentSelectedGameObject.name;

        selectedPieceName = name;
        selectedPiece = Resources.Load<GameObject>("PlaceHolders/" + selectedPieceName);
        DeleteMode(false);

        // load the selected piece sprite from resourses
        SelectedPieceImg.sprite = Resources.Load<Sprite>("Icons/" + selectedPieceName);

        //hide the pannels
        buildUI.HideAllPannels();
    }


    public void ToogleDelete() {

        isDeleting = !isDeleting;
        buildUI.ChangeEraseBtn(isDeleting);
    }
    public void DeleteMode(bool del)
    {
        isDeleting = del;
        buildUI.ChangeEraseBtn(isDeleting);
    }


    // place the actual piece in place
    public void PlacePiece() {

        GameObject piece = Resources.Load<GameObject>("Cubes/" + selectedPieceName);

        GameObject newPiece = Instantiate(piece, placeHolder.transform.position, placeHolder.transform.rotation);
        newPiece.transform.parent = placeHolder.transform.parent;

        PieceNumber++;
        string number = PieceNumber.ToString("000");
        newPiece.name = number + "_" + selectedPiece.name;

        // give color to the new piece

        Renderer [] newPieceRenderers = newPiece.GetComponentsInChildren<Renderer>();

        foreach(Renderer rend in newPieceRenderers) {

            if(rend.gameObject.layer != 10) // snapoint layer
            rend.material.color = colorPicker.color;
        }
                

        //clean the temporal variables
        Destroy(placeHolder);
        placeHolder = null;

        buildUI.HidePlacingButtons();

        isAlowed = true;
    }

    // delete the current placeholder
    public void CancelPiece() {

        if(placeHolder != null)
        Destroy(placeHolder);

        placeHolder = null;

        // dont make the string empty so you still have a selected piece
        //selectedPieceName = string.Empty;

        buildUI.HidePlacingButtons();

        isAlowed = true;
    }

    public void RotatePiece(bool right) {

        if (right)
            placeHolder.transform.Rotate(0,0,90f);
        else
            placeHolder.transform.Rotate(0, 0, -90f);

    }
}
