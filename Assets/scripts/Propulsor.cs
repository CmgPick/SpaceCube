using UnityEngine;

public enum Direction { Left, Right, Forward}
public class Propulsor : MonoBehaviour {

    public float power = 10f;

    public FixedJoystick DriveJoy;
    private float currentpower;
    private Rigidbody rb;
    public Direction direction;
    public ParticleSystem particleSys;
    public ParticleSystem.MainModule main;

    // Use this for initialization
    void Start () {

        rb = transform.root.GetComponent<Rigidbody>();
        DriveJoy = FindObjectOfType<FixedJoystick>();

        particleSys = GetComponentInChildren<ParticleSystem>();
        main = particleSys.main;

        // Get the direction for this propulsor

        // Forward Impulse Propulsor
        if (transform.forward == -Vector3.forward)
            direction = Direction.Forward;

        // Right Impulse Propulsor
        if (transform.forward == Vector3.left)
            direction = Direction.Right;

        // Left Impulse Propulsor
        if (transform.forward == Vector3.right)
            direction = Direction.Left;

    }


        // Update is called once per frame
        void FixedUpdate () {

            if (rb != null && DriveJoy != null) { 

                Vector3 powerVector = -this.transform.forward * currentpower;
                rb.AddForceAtPosition(powerVector, this.transform.position);
            
                main.startSpeed = currentpower;

            }


        }

        // USED FROM AI
    public void Propulse(float force) {

        if (rb != null){

            currentpower = power * force;

            Vector3 powerVector = -this.transform.forward * currentpower;
            rb.AddForceAtPosition(powerVector, this.transform.position);

            main.startSpeed = currentpower * 4;

        } else
            rb = transform.root.GetComponent<Rigidbody>();


    }

        private void Update(){

            if (DriveJoy == null)
                return;

        // Forward Impulse Propulsor
            if (direction == Direction.Forward) {

                if (DriveJoy.Vertical > 0)
                    currentpower = power * DriveJoy.Vertical;
                else
                    currentpower = 0;

            }

         // Right Impulse Propulsor
            if (direction == Direction.Right) {

                if (DriveJoy.Horizontal > 0)
                    currentpower = power * DriveJoy.Horizontal;
                else
                    currentpower = 0;

                //Debug.Log("propulsing Right");
            }

        // Left Impulse Propulsor
            if (direction == Direction.Left) { 

                if(DriveJoy.Horizontal < 0)
                    currentpower = power * -DriveJoy.Horizontal;
                else
                    currentpower = 0;

                //Debug.Log("propulsing Left");

            }
        }
}
