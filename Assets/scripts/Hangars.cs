using UnityEngine;
using UnityEngine.UI;

public class Hangars : MonoBehaviour {

    public int numberOfhangars = 2; // MAKE THIS PRIVATE?
    private int currenthangar = 1;
    public GameObject ship;

    public GameObject nextHangarBtn;
    public GameObject prevHangarBtn;
    public Text hangarNameTxt;

    private Constructor constructor;
    private SaveShip saveShip;

    // Use this for initialization
    void Start () {

        ship = GameObject.Find("Ship_Root");
        saveShip = FindObjectOfType<SaveShip>();
        constructor = FindObjectOfType<Constructor>();
    }
	
	public void ChangeHangar (bool next) {

        if (next) {

            if (currenthangar < numberOfhangars)
                currenthangar ++;
        }
        else
        {

            if (currenthangar > 1)
                currenthangar --;

        }

        if (currenthangar == numberOfhangars) {
            nextHangarBtn.SetActive(false);
            prevHangarBtn.SetActive(true);
        }
            

        if (currenthangar == 1){
            prevHangarBtn.SetActive(false);
            nextHangarBtn.SetActive(true);
        }


        hangarNameTxt.text = "Hangar " + currenthangar;
        DestroyAllPieces();

        saveShip.ChangeFileName("Hangar" + currenthangar);

        // reset piece number in the constructor and destroy the placeholder
        constructor.CancelPiece();
        constructor.PieceNumber = 0;
    }

    private void DestroyAllPieces(){

        Transform[] pieces = ship.GetComponentsInChildren<Transform>();

        foreach (Transform piece in pieces)
            if(piece.transform.name != "Ship_Root")
            Destroy(piece.gameObject);

        GameObject core = Instantiate(Resources.Load<GameObject>("Cubes/CO"),Vector3.zero,Quaternion.identity);
        core.transform.parent = ship.transform;
        core.transform.name = "000_CO";


    }
}
