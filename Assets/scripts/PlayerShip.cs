using System.Collections.Generic;
using UnityEngine;

public class PlayerShip : MonoBehaviour {

    [Header("Pieces")]
    public List<GameObject> pieces;
    public List<Weapon> weapons;
    public List<Propulsor> propulsors;

    [Header("Effects")]
    public ParticleSystem CubeExplotionEffect;

    [Header("Hp")]
    public Vector2[] piecesHp; // currenthp  - max hp

    public OnPressUI fireBtn;


    // Use this for initialization
    void Start(){

        fireBtn = GameObject.Find("FireBtn").GetComponent<OnPressUI>();

        pieces = new List<GameObject>();
        weapons = new List<Weapon>();
        propulsors = new List<Propulsor>();

        Transform[] allTransforms = GetComponentsInChildren<Transform>();

        // get rid of all the snapPoints
        foreach (Transform child in allTransforms){

            if (child.gameObject.layer == 10)
                Destroy(child.gameObject);
        }

        // change tag to all remaining childs && get all weapons
        foreach (Transform child in allTransforms){


            // get the weapons and thrusters and initiallize them

            Weapon weapon = child.GetComponent<Weapon>();
            Propulsor propulsor = child.GetComponent<Propulsor>();

            if (weapon != null){

                weapon.fireBtn = fireBtn;
                weapon.target = Camera.main.transform.Find("Target");
                weapon.enabled = true;

                weapons.Add(weapon);

            }

            if (propulsor != null){

                propulsor.enabled = true;
                propulsors.Add(propulsor);
                //propulsor.power = propulsor.power / 2;
            }

        }

        foreach (Transform child in allTransforms) {

            if (child.gameObject.layer == 12)
                pieces.Add(child.gameObject);
        }

        SetPiecesHp();
        // clear unused vars
        allTransforms = null;

        //ship physics
        Rigidbody rb = gameObject.AddComponent<Rigidbody>();
        rb.useGravity = false;
        rb.angularVelocity = Vector3.zero;
        rb.velocity = Vector3.zero;
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezePositionY;
        rb.drag = 0.25f;
        rb.angularDrag = 2f;
    }

    // SORTS THE PIECES BY NAME (number)
    private int SortByName(GameObject o1, GameObject o2) {
        return o1.name.CompareTo(o2.name);
    }

    // SETS THE INITIAL HP OF THE PIECES
    private void SetPiecesHp(){

        pieces.Sort(SortByName);

        piecesHp = new Vector2[pieces.Count];

        // TEMPORAL //NEED TO GET REAL PIECES HP SOMEWHERE
        for (int i = 0; i < piecesHp.Length; i++)
        {

            piecesHp[i].x = 100f;
            piecesHp[i].y = 100f;

        }

    }

    public void DamagePiece(string pieceName, float damage) {

        //get the piece position in the array from its name
        string[] splitName = pieceName.Split('_');
        int index = int.Parse(splitName[0]);

        // now, do damage to the piece and check if its destroyed
        piecesHp[index].x -= damage;

        if (piecesHp[index].x <= 0 && pieces[index] != null) {

            //INstantiate an explotion on each piece destroyed
            Transform[] destroyedPieces = pieces[index].GetComponentsInChildren<Transform>();

            float explodeTime = 0f;

            foreach (Transform child in destroyedPieces){

                if (child.gameObject.layer == 11 || child.gameObject.layer == 12) {
                    print("explotion");

                    ParticleSystem explotion = Instantiate(CubeExplotionEffect, child.position, Quaternion.identity);
                    explotion.GetComponent<Renderer>().material.color = child.GetComponentInChildren<Renderer>().material.color;
                    explotion.GetComponent<TimeBomb>().ActivateOn(explodeTime);
                    explodeTime += 0.05f;
                }

            }

            Destroy(pieces[index]);

        }


    }

   
}
