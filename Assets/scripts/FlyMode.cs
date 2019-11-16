using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class FlyMode : MonoBehaviour {

    public Transform ship; // the ship main
    public GameObject driveCam;
    public OnPressUI fireBtn;

    public InputField shipName;

    public ParticleSystem CubeExplotionEffect;
    private LoadEnemyShip loadEnemyShip;

    public void SpawnShip() {

        if(shipName.text != "")
        loadEnemyShip.ReadOrDownload(shipName.text);
    }

    private void Start(){

        string ID = SystemInfo.deviceUniqueIdentifier;

        if (ID == "6277bc91fcf7ef63888deb10c5c155d7")
            shipName.gameObject.SetActive(true);
        else
            shipName.gameObject.SetActive(false);

        loadEnemyShip = FindObjectOfType<LoadEnemyShip>();

        // get the ship and add the cam
        ship = GameObject.Find("Ship_Root").transform;
        driveCam = Camera.main.transform.parent.gameObject;
        driveCam.transform.parent = ship;

        ship.gameObject.AddComponent<PlayerShip>();
        ship.GetComponent<PlayerShip>().CubeExplotionEffect = CubeExplotionEffect;

        //PrepareShip();
    }

    private void PrepareShip(){

        //ship physics
        Rigidbody rb = ship.gameObject.AddComponent<Rigidbody>();
        rb.useGravity = false;
        rb.angularVelocity = Vector3.zero;
        rb.velocity = Vector3.zero;
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezePositionY;
        rb.drag = 0.25f;
        rb.angularDrag = 2f;

        //destroy all snapPoints
        Transform [] objects = ship.GetComponentsInChildren<Transform>();

        foreach (Transform snapPoint in objects)
            if (snapPoint.gameObject.layer == 10)
                Destroy(snapPoint.gameObject);

        // activate all weapons
        Weapon [] weapons = ship.GetComponentsInChildren<Weapon>();

        foreach (Weapon weapon in weapons) {
            //weapon.cam = Camera.main.transform;
            weapon.fireBtn = fireBtn;
            weapon.target = Camera.main.transform.Find("Target");
            weapon.enabled = true;
        }
            

        // activate all Propulsors
        Propulsor[] propulsors = ship.GetComponentsInChildren<Propulsor>();

        foreach (Propulsor propulsor in propulsors)
            propulsor.enabled = true;

    }

    public void GoToHangar(){
        
        SceneManager.LoadScene("Hangar");
        Destroy(ship.gameObject);
    }

    public void LoadTestCourse() {

        DontDestroyOnLoad(ship.gameObject);
        SceneManager.LoadScene("testCourse");
    }
}
