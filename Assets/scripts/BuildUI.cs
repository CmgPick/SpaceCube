using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class BuildUI : MonoBehaviour {

    private GameObject ship;

    [Header("buttons")]
    public Transform verticalPanels;
    public GameObject slidingBarContent; // where all the buttons are stored in the sliding btns bar

    public GameObject placingButtons; // yes/no,rotate btns
    public Image SelectedPieceImg;

    [Header("delete mode")]
    public Sprite eraseIcon;
    public Sprite erasingIcon;
    public Button eraseBtn;

    [Header("Server waiting")]
    public GameObject loadingPannel;
    public Text loadingText;
    public Image CircleLoadingImage;
    private float circleTime = 0;
    private bool add = true;
    public float serverWaitTime = 20f;
    private float waitTime = 0;
    public bool isWaiting = false;

    private Constructor constructor;
    private PhpUpload phpUpload;
    private SaveShip saveShip;

    //enemy creator security
    public GameObject createEnemyBtn;

    private void Awake(){

        string ID = SystemInfo.deviceUniqueIdentifier;

        if (ID == "6277bc91fcf7ef63888deb10c5c155d7")
            createEnemyBtn.SetActive(true);
        else
            createEnemyBtn.SetActive(false);

        saveShip = FindObjectOfType<SaveShip>();
        phpUpload = FindObjectOfType<PhpUpload>();
        ship = GameObject.Find("Ship_Root");
        constructor = FindObjectOfType<Constructor>();

    }


    public void ChangeEraseBtn(bool isErasing) {

        if (isErasing)
            eraseBtn.image.sprite = erasingIcon;
        else
            eraseBtn.image.sprite = eraseIcon;
    }

    public void TooglePlacingButtons () {

        placingButtons.SetActive(!placingButtons.activeSelf);
            
	}

    public void HidePlacingButtons(){

        placingButtons.SetActive(false);
    }

    public void ShowPanel () {

        string name = EventSystem.current.currentSelectedGameObject.name;
        HideAllPannels();
        verticalPanels.transform.Find(name).gameObject.SetActive(true);
    }

    public void HideAllPannels() {

        for (int i = 0; i < verticalPanels.childCount; i++)
            verticalPanels.GetChild(i).gameObject.SetActive(false);
    }

    public void IsBusy (bool busy) {

        if (busy){

            isWaiting = true;
            constructor.isAlowed = false;
            loadingPannel.SetActive(true);
            loadingText.text = "Connecting to server.";
        }

        else{


            isWaiting = false;
            constructor.isAlowed = true;
            loadingPannel.SetActive(false);
            loadingText.text = "";

            waitTime = 0f;
            circleTime = 0f;
        }

    }

    void ConnectionError() {

        isWaiting = false;
        phpUpload.StopAllCoroutines();
        saveShip.StopAllCoroutines();

        loadingText.text = "Couldnt connect with server";
        Invoke("NotBusy", 2f);

    }

    // used for the invoke
    void NotBusy() { IsBusy(false); }

    public void GoToFlyMode() {

        DontDestroyOnLoad(ship);

        // delete the green pieces if any before going to fly
        if (constructor.placeHolder != null)
            Destroy(constructor.placeHolder);

        SceneManager.LoadScene("TestCourse");

    }

    private void Update(){

        if (isWaiting) {

            // server wait time
            waitTime += Time.deltaTime;

            if(waitTime >= serverWaitTime)  // waited too long, show error
                ConnectionError();

            // loading circle
            if (add)
                circleTime += Time.deltaTime;
            else
                circleTime -= Time.deltaTime;

            CircleLoadingImage.fillAmount = circleTime;

            if(circleTime >= 1)
            {
                CircleLoadingImage.fillClockwise = false;
                add = false;
            } 
                

            if (circleTime <= 0)
            {
                CircleLoadingImage.fillClockwise = true;
                add = true;
            }

        }   
        
    }


}
