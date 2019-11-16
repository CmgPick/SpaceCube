using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using System.IO;

public class LoadEnemyShip : MonoBehaviour {

    static string enemyShipsURL = "http://sv1daheimstudio.orgfree.com/SpacecubeData/Enemies/";

    [Header("Download info")]
    public string defaultFileName = "RagDoll";

    [Header("Spawn info")]
    public GameObject emptyGO;
    public Vector3 spawnPos;
    private Transform ship;

    [Header("Build info")]
    public List<string> lines;
    public string pieceNumber;
    public List<Transform> pieces;

    public ParticleSystem CubeExplotionEffect; // SHOULD BE LOADING FROM RESOURSES

    private void Start(){
        ReadOrDownload("RagDoll");
    }

    // first check if the file exist. otherwise download it

    public void ReadOrDownload(string filename){

        StartCoroutine(ReadOrDown(filename));
    }

   IEnumerator ReadOrDown(string filename){

        if (filename == "")
            filename = defaultFileName;

        string loadDataPath = Application.persistentDataPath + "/Enemies/" + filename + ".scs";
        StreamReader reader;

        if (!File.Exists(loadDataPath)){

            Debug.Log("No local file found, Downloading from server");
            StartCoroutine(DownloadFromServer(filename));

        }else{

            Debug.Log("Local File found, loading");

            reader = new StreamReader(loadDataPath);

            while (!reader.EndOfStream){

                string newLine = reader.ReadLine();
                lines.Add(newLine);
                
            }

            yield return reader;

            reader.Close();

            // Only build the ship if we got some data
            if (lines.Count >0 && lines[0] != "" && lines[0] != "﻿<!DOCTYPE html>")
                CreateShip();
            else {

                print("Corrupted data, deleting");
                File.Delete(loadDataPath);
            }              
            
        }

    }

    IEnumerator DownloadFromServer(string filename){

        string completeUrl = enemyShipsURL + filename + ".scs";

        WWW www = new WWW(completeUrl);
        yield return www;

        if (www.isDone && string.IsNullOrEmpty(www.error)){

            Debug.Log("Downloaded " + www.bytesDownloaded + " bytes from server");

            // create local file
            string saveDataPath = Application.persistentDataPath + "/Enemies/";

            // Determine whether the directory exists.
            if (!Directory.Exists(saveDataPath)){

                Directory.CreateDirectory(saveDataPath);
                print(" the directory was created");
            }

            saveDataPath = Application.persistentDataPath + "/Enemies/" + filename + ".scs";

            StreamWriter writer = new StreamWriter(saveDataPath);
            writer.Write(www.text);

            yield return writer;

            Debug.Log("Local File created");
            writer.Close();

            ReadData(www.text);

        }
        else
        {
            Debug.Log("No data found on server");
            //isReady(false);
        }

        if (www.error != null)
            Debug.LogWarning(www.error);

    }

    void ReadData(string data){

        Debug.Log("reading data");

        string[] readedLines = data.Split('\n');

        foreach (string line in readedLines)
            if (line != "")
                lines.Add(line);

        // Only build the ship if we got some data
        if (lines.Count > 0 && lines[0] != "" && lines[0] != "﻿<!DOCTYPE html>")       
        CreateShip();
        else
            print("Corrupted data");

    }

    void CreateShip(){

        pieces.Clear();

        // spawn a new root for the ship
        string shipName = (lines[0]);

        GameObject root = Instantiate(emptyGO, spawnPos, Quaternion.identity);
        root.transform.name = shipName;
        ship = root.transform;

        pieces.Add(ship);

        // START AT LINE 2 
        for (int i = 1; i < lines.Count; i++){

            string[] value = lines[i].Split('_');

            pieceNumber = value[0];
            string prefabName = value[1];
            string newPieceName = value[0] + "_" + value[1];

            // POSITION
            string[] sPos = value[2].Split('%');
            float[] fPos = new float[sPos.Length];

            for (int x = 0; x < sPos.Length; x++)
            {

                fPos[x] = float.Parse(sPos[x]);
            }

            Vector3 pos = new Vector3(fPos[0] + ship.position.x, fPos[1] + ship.position.y, fPos[2] + ship.position.z);

            // ROTATION
            string[] sRot = value[3].Split('%');
            float[] fRot = new float[sRot.Length];

            for (int x = 0; x < sRot.Length; x++)
            {

                fRot[x] = float.Parse(sRot[x]);
            }

            Quaternion rot = Quaternion.Euler(fRot[0], fRot[1], fRot[2]);

            //PARENT
            string PieceParent = value[4] + "_" + value[5];

            //COLOR
            byte colR = byte.Parse(value[6].Substring(0, 3));
            byte colG = byte.Parse(value[6].Substring(3, 3));
            byte colB = byte.Parse(value[6].Substring(6, 3));

            Color32 col = new Color32(colR, colG, colB, 255);


            GameObject spawnedPiece = Instantiate((GameObject)Resources.Load("Cubes/" + prefabName));

            spawnedPiece.transform.position = pos;
            spawnedPiece.transform.rotation = rot;
            spawnedPiece.name = newPieceName;

            pieces.Add(spawnedPiece.transform);

            // set the first parent name to the root (same as ship name in file)
            if(i == 1)
                PieceParent = shipName;

            foreach (Transform piece in pieces)
                if(piece.name == PieceParent)
                    spawnedPiece.transform.parent = piece;

            // give color to the new piece
            Renderer[] newPieceRenderers = spawnedPiece.GetComponentsInChildren<Renderer>();

            foreach (Renderer rend in newPieceRenderers)
                if (rend.gameObject.layer != 10) // snapoint layer
                    rend.material.color = col;


        }


        Debug.Log("Ship created");

        //buildUI.IsBusy(false);
        //lines = new List<string>(0);

        root.AddComponent<EnemyAI>();
        root.GetComponent<EnemyAI>().CubeExplotionEffect = CubeExplotionEffect;
        
    }
}
