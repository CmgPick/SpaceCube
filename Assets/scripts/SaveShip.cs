using System.IO;
using System.Collections.Generic;
using UnityEngine;

public class SaveShip : MonoBehaviour {

    public Transform ship;
    public bool CreateEnemyShip = false;

    private string fileName = "Hangar1";

    public List<Transform> pieces;
    public List <string> lines;
    private string[] linesArray;
    public string pieceNumber;
    private string shipData = "";

    private PhpUpload phpUpload;
    private BuildUI buildUI;
    private Constructor constructor;

    private void Start(){

        constructor = FindObjectOfType<Constructor>();
        buildUI = FindObjectOfType<BuildUI>();
        phpUpload = FindObjectOfType<PhpUpload>();

        ChangeFileName("Hangar1");
    }

    public void ChangeFileName(string hangar) {

        fileName = SystemInfo.deviceUniqueIdentifier + "-" + hangar;
        lines.Clear();
    }



    // here we get all the ship pieces and make a list
    public void FindAllPieces() {

        Transform[] childs = ship.GetComponentsInChildren<Transform>();

        foreach (Transform child in childs) {

            if (child.gameObject.layer == 12)
                pieces.Add(child);
        }

        pieces.Sort(SortByName);

    }

    private int SortByName(Transform o1, Transform o2){

        return o1.name.CompareTo(o2.name);
    }

    public void SaveEnemyShip(){

        CreateEnemyShip = true;
        SaveFile();
    }

    // here we save a txt file formatted (number , type, position, rotation, parent, color)
    public void SaveFile() {

        buildUI.IsBusy(true);

        // FIND THE PIECES FIRST!
        FindAllPieces();

        lines.Clear();

        // ADD THE SHIP NAME AND OTHER INFO ON LINE 1
        lines.Add(constructor.shipName);

        for (int i = 0; i < pieces.Count; i++){

            Vector3 rot = pieces[i].rotation.eulerAngles;
            Vector3 pos = pieces[i].position;

            //get color
            Color32 col = pieces[i].GetComponentInChildren<Renderer>().material.color;
            string Scol = col.r.ToString("000") + col.g.ToString("000") + col.b.ToString("000");

            lines.Add (pieces[i].name + "_" + pos.x + "%" + pos.y + "%" + pos.z + "_" + rot.x + "%" + rot.y + "%" + rot.z + "_" + pieces[i].parent.name + "_" + Scol);
        }

        linesArray = new string[lines.Count];

        for (int i = 0; i < lines.Count; i++){

           //writer.WriteLine(lines[i]);

            linesArray[i] = lines[i];
            shipData += lines[i];
        }

        //writer.Close();

        Debug.Log("Saving file on server");

        if(CreateEnemyShip)
            phpUpload.CreateEnemy(constructor.shipName, linesArray);
        else
        phpUpload.UploadData(fileName, linesArray);

        pieces.Clear();

        CreateEnemyShip = false;
    }

    // download the data first
    public void DownloadFile() {

        buildUI.IsBusy(true);

        Debug.Log("Loading the file from server...");

        StartCoroutine(phpUpload.DownloadFromServer(fileName, isReady =>{

            if (isReady)
                //ReadFile();
                buildUI.IsBusy(false);


        }));  
    

    }

    // now read the local data 
    public void ReadData(string data){

        string [] readedLines = data.Split(new[] { '\r', '\n' });

        foreach (string line in readedLines)
            if (line != "")
            lines.Add(line);

        // Only build the ship if we got some data
        if (lines[0] != "" && lines[0] != "﻿<!DOCTYPE html>")
            CreateShip();
        else
            print("Corrupted data");

    }


    // now read the local data 
    // NOT USED
    private void ReadFile() {

        Debug.Log("reading local data");

        lines.Clear();

        string loadDataPath = Application.persistentDataPath + "/" + fileName + ".scs";
        StreamReader reader;

        if (!File.Exists(loadDataPath)) {

            Debug.LogWarning("No local file found, loading empty hangar");
            return;
        }
        else
            reader = new StreamReader(loadDataPath);

        while (!reader.EndOfStream){

            string newLine = reader.ReadLine();
            lines.Add(newLine);

        }

        reader.Close();
        Debug.Log("Local File Loaded" );

        CreateShip();

    }

    // here we create a new ship with the loaded Info
    private void CreateShip(){

        // keep a corelative piece number (-2 for the first info Line)
        constructor.PieceNumber = lines.Count -2;

        //Destroy the previous root to prevent doubles
        ship.GetChild(0).gameObject.SetActive(false);
        Destroy(ship.GetChild(0).gameObject);

// FIRST READ THE FIRST LINE (SHIP NAME & OTHER INFO)

        constructor.ShowShipName (lines[0]);

        // START AT LINE 2 
        for (int i = 1; i < lines.Count; i++){

            string[] value = lines[i].Split('_');

            pieceNumber = value[0];
            string prefabName = value[1];
            string newPieceName = value[0] +"_" + value[1] ;

            // POSITION
            string[] sPos = value[2].Split('%');
            float[] fPos = new float[sPos.Length];

            for (int x = 0; x < sPos.Length; x++){

                fPos[x] = float.Parse(sPos[x]);
            }

            Vector3 pos = new Vector3(fPos[0], fPos[1], fPos[2]);

            // ROTATION
            string[] sRot = value[3].Split('%');
            float[] fRot = new float[sRot.Length];

            for (int x = 0; x < sRot.Length; x++){

                fRot[x] = float.Parse(sRot[x]);
            }

            Quaternion rot = Quaternion.Euler(fRot[0], fRot[1], fRot[2]);

            //PARENT
            string PieceParent = value[4] + "_" + value[5];

            //COLOR
            byte colR = byte.Parse(value[6].Substring(0,3));
            byte colG = byte.Parse(value[6].Substring(3, 3));
            byte colB = byte.Parse(value[6].Substring(6, 3));

            Color32 col = new Color32(colR, colG, colB,255);


            GameObject spawnedPiece = Instantiate((GameObject)Resources.Load("Cubes/"+prefabName));

            spawnedPiece.transform.position = pos;
            spawnedPiece.transform.rotation = rot;
            spawnedPiece.name = newPieceName;

            spawnedPiece.transform.parent = GameObject.Find(PieceParent).transform;

            // give color to the new piece
            Renderer[] newPieceRenderers = spawnedPiece.GetComponentsInChildren<Renderer>();

            foreach (Renderer rend in newPieceRenderers)
                if (rend.gameObject.layer != 10) // snapoint layer
                    rend.material.color = col;
            

        }

        /*
        Debug.Log("Ship created");
        string loadDataPath = Application.persistentDataPath + "/" + fileName + ".scs";

        if (File.Exists(loadDataPath)) {

            File.Delete(loadDataPath);
            Debug.Log("Local file deleted");

        }
        */

        Debug.Log("Ship created");
        //buildUI.IsBusy(false);
        //lines = new List<string>(0);

    }


}
