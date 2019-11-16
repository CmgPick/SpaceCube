using UnityEngine;
using System.Collections.Generic;

public class EnemyAI : MonoBehaviour {

    [Header("Settings")]
    public bool isDummy = false; //is this a dummy practice bot? (wont move or shoot)
    public float damageMultipier = 0.2f; // 0 no damage - 1 full damage

    [Header("Pieces")]
    public List<Transform> pieces;
    public List <Weapon> weapons;
    public List<Propulsor> propulsors;

    [Header("Effects")]
    public ParticleSystem CubeExplotionEffect;

    [Header("Hp")]
    public Vector2 [] piecesHp; // currenthp  - max hp

    [Header("Movement")]
    public Transform target;
    public float distance; // the distance from the player
    public int maxFollowDistance = 200;
    public int minFollowDistance = 50;
    public int shootDistance = 100;
    public float Speed;

    public void Start(){

        pieces = new List<Transform>();
        weapons = new List<Weapon>();
        propulsors = new List<Propulsor>();

        target = GameObject.Find("Ship_Root").transform;

        Transform[] childs = transform.GetComponentsInChildren<Transform>();

        foreach (Transform child in childs){

            child.tag = "Enemy";

            if (child.gameObject.layer == 12)
                pieces.Add(child);
        }

        // get rid of all the snapPoints
        foreach (Transform child in childs){

            if (child.gameObject.layer == 10)
                Destroy(child.gameObject);

            
        }

        // change tag to all remaining childs && get all weapons
        foreach (Transform child in pieces){

            // get the weapons and thrusters and initiallize them

            Weapon weapon = child.GetComponent<Weapon>();
            Propulsor propulsor = child.GetComponent<Propulsor>();

            if (weapon != null && !isDummy){

                weapons.Add(weapon);
                weapon.target = target;
                weapon.isPlayer = false;

                //reduce damage
                weapon.damage *= damageMultipier;
                weapon.enabled = true;
            }

            if (propulsor != null && !isDummy) {

                propulsor.enabled = true;

                propulsors.Add(propulsor);
                //propulsor.power = propulsor.power / 2;
            }
                
        }


        SetPiecesHp();
        // clear unused vars

        //Physics

        Rigidbody rb = gameObject.AddComponent<Rigidbody>();
        rb.useGravity = false;
        rb.angularVelocity = Vector3.zero;
        rb.velocity = Vector3.zero;
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezePositionY;
        rb.drag = 0.5f;
        rb.angularDrag = 4f;
    }

    // SORTS THE PIECES BY NAME (number)
    private int SortByName(Transform o1, Transform o2){
        return o1.name.CompareTo(o2.name);
    }

    // SETS THE INITIAL HP OF THE PIECES
    private void SetPiecesHp () {

        pieces.Sort(SortByName);

        piecesHp = new Vector2 [pieces.Count];

        // TEMPORAL //NEED TO GET REAL PIECES HP SOMEWHERE
        for (int i = 0; i < piecesHp.Length; i++){

            piecesHp[i].x = 100f;
            piecesHp[i].y = 100f;

        }

    }

    public void DamagePiece(string pieceName, float damage) {

        //get the piece position in the array from its name
        string [] splitName = pieceName.Split('_');
        int index = int.Parse(splitName[0]);

        // now, do damage to the piece and check if its destroyed
        piecesHp[index].x -= damage;

        if (piecesHp[index].x <= 0 && pieces[index] != null) {

            //INstantiate an explotion on each piece destroyed
            Transform [] destroyedPieces = pieces[index].GetComponentsInChildren<Transform>();

            float explodeTime = 0f;

            foreach (Transform child in destroyedPieces){

                if(child.gameObject.layer == 12) {

                    ParticleSystem explotion = Instantiate(CubeExplotionEffect, child.position, Quaternion.identity);
                    explotion.GetComponent<Renderer>().material.color = child.GetComponentInChildren<Renderer>().material.color;
                    explotion.GetComponent<TimeBomb>().ActivateOn(explodeTime);
                    explodeTime += 0.05f;
                }

            }

            Destroy(pieces[index].gameObject);

        }
            

    }

    // not using
    private int FrontOrBack (Transform target) {

        Vector3 local_pos = transform.InverseTransformPoint(target.transform.position);

        if (local_pos.z < 0)
            return -1; // "Back";
        else
            return 1; // "Front ";

    }

    private float LeftOrRight(Transform target){

        Vector3 local_pos = transform.InverseTransformPoint(target.transform.position);
        return Mathf.Clamp (local_pos.x,-1f, 1f);
    }

    private void FixedUpdate(){

        distance = Vector3.Distance (transform.position,target.position);

        // MOVEMENT
        foreach (Propulsor propulsor in propulsors){

            // FORWARD WITHING DISTANCES
            if (distance < maxFollowDistance && distance > minFollowDistance){

                if (propulsor != null && propulsor.direction == Direction.Forward)
                {

                    propulsor.Propulse((distance / maxFollowDistance));

                    print("propulsing @ " + ((distance / maxFollowDistance)));
                }

            }

            // TURNING (LLOKAT //ALWAYS)
            if (distance < maxFollowDistance){

                float direction = LeftOrRight(target);

                if (propulsor != null && direction < 0)
                {

                    if (propulsor != null && propulsor.direction == Direction.Left)
                        propulsor.Propulse(-direction);

                    //print("propulsing left @ " + direction);
                }
                else if (propulsor != null && direction >= 0)
                {

                    if (propulsor.direction == Direction.Right)
                        propulsor.Propulse(direction);

                    //print("propulsing right @ " + direction);
                }

            }

        }

        // SHOOTING
        foreach (Weapon weapon in weapons){

            if (distance <= shootDistance)
                weapon.Shoot(true);
            else
                weapon.Shoot(false);

        }
    }
}




