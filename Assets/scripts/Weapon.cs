using UnityEngine;

public class Weapon : MonoBehaviour {

    public bool isPlayer = true; // is this weapon a player controlled one?

    [Header ("Shooting")]
    public OnPressUI fireBtn;
    public bool isShooting = false;
    private bool canShoot = true;
    private float timer;

    [Header("weapon values")]
    public float fireRate = 0.5f;
    public int range = 100;
    public float damage = 20f;

    [Header("graphics")]
    public Transform barrel;
    public Transform cannonTip; // used for sparks position on barrel
    public ParticleSystem shootSparks;
    public GameObject hitSparks;

    public Transform target;


    private void FixedUpdate(){

        // LookAt Target

            Vector3 targetPostition = target.position - barrel.position;

            // the second argument, upwards, defaults to Vector3.up
            Quaternion rotation = Quaternion.LookRotation(targetPostition, Vector3.up);
            barrel.rotation = rotation;

            //Debug.DrawRay(cannonTip.position, targetPostition, Color.blue,range);
            //Debug.DrawRay(cam.position, targetPostition, Color.cyan);
    }
    

    private void Update(){

        if (isPlayer){

            fireBtn.OnTouch.AddListener(delegate () { isShooting = true; });
            fireBtn.OnRelase.AddListener(delegate () { isShooting = false; });

        }

        if (isShooting){

            timer += Time.deltaTime;
        }
            

        if (isShooting && canShoot) {

            Fire();
            canShoot = false;
        }

        if(timer >= fireRate){

            canShoot = true;
            timer = 0f;

        }
    
    }

    public void Shoot( bool shoot){

        isShooting = shoot;
    }


    public void Fire(){


        //Debug.Log("shooting");

        Vector3 targetPostition = target.position - barrel.position;
        Debug.DrawRay(barrel.position, targetPostition, Color.red);

        //graphics
        if (shootSparks)
            shootSparks.Play();
        
        RaycastHit hit;

        if (Physics.Raycast(barrel.position, targetPostition, out hit, range)){

            GameObject sparks = Instantiate(hitSparks, hit.point, Quaternion.identity);
            sparks.transform.forward = hit.normal;

            // only if this is a player Weapon and hits an Enemy
            if (isPlayer && hit.collider.tag == "Enemy") {

                //get the enemyAI scrip for the enemy ship hitted           
                EnemyAI enemyAI = hit.transform.root.GetComponent<EnemyAI>();

                if (enemyAI != null) {

                    enemyAI.DamagePiece(hit.collider.transform.parent.name,damage);
                    Debug.Log("hitted " + hit.collider.transform.parent.name + " for " + damage + " damage");
                }
                
            }

            // For enemy controlled Weapons
            if (!isPlayer && hit.collider.tag == "Piece") {

                //get the playerShip script       
                PlayerShip playerShip = hit.transform.root.GetComponent<PlayerShip>();

                if (playerShip != null){

                    playerShip.DamagePiece(hit.collider.transform.parent.name, damage);
                    //Debug.Log("hitted " + hit.collider.transform.parent.name + " for " + damage + " damage");
                }

            }




        }

    }


}
