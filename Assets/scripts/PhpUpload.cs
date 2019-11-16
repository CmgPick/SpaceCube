using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using System.IO;

public class PhpUpload : MonoBehaviour {

    private BuildUI buildUI;
    private SaveShip saveShip;

    // server 1
    static string uploadUrlSV1 = "http://sv1daheimstudio.orgfree.com/SpacecubeData/UploadFile.php";
    static string downloadUrlSV1 = "http://sv1daheimstudio.orgfree.com/SpacecubeData/";

    // enemyCreator
    static string enemySavingUrl = "http://sv1daheimstudio.orgfree.com/SpacecubeData/Enemies/UploadFile.php";

    public void Start(){

        buildUI = FindObjectOfType<BuildUI>();
        saveShip = FindObjectOfType<SaveShip>();
    }

    public void UploadData(string name, string [] data ){

        StartCoroutine(UploadToPhp(name, data));

    }

    #region ENEMY SHIP CREATOR

    public void CreateEnemy(string name, string[] data){

        StartCoroutine(SaveEnemyShip(name, data));

    }

    IEnumerator SaveEnemyShip(string name, string[] dataToSend){

        string data = "";

        for (int i = 0; i < dataToSend.Length; i++){

            if (i < dataToSend.Length)
                data += dataToSend[i] + "\n";
            else
                data += dataToSend[i];

        }

        WWWForm form = new WWWForm();
        form.AddField("name", name);
        form.AddField("data", data);

        UnityWebRequest www = UnityWebRequest.Post(enemySavingUrl, form);

        yield return www.SendWebRequest();

        if (www.isNetworkError)
            Debug.Log(www.error);

        Debug.Log(www.downloadHandler.text);
        Debug.Log("Uploaded " + www.uploadedBytes + " Bytes to server");

        buildUI.IsBusy(false);

    }
    #endregion

    IEnumerator UploadToPhp(string name, string [] dataToSend){

        string data = "";

        for (int i = 0; i < dataToSend.Length; i++){
            

            if (i < dataToSend.Length)
                data += dataToSend[i] + "\n";
            else
                data += dataToSend[i];

        }

        WWWForm form = new WWWForm();
        form.AddField("name", name);
        form.AddField("data", data);

        UnityWebRequest www = UnityWebRequest.Post(uploadUrlSV1, form);

        yield return www.SendWebRequest();

        if (www.isNetworkError)
            Debug.Log(www.error);

        Debug.Log(www.downloadHandler.text);
        Debug.Log("Uploaded " + www.uploadedBytes  + " Bytes to server");

        buildUI.IsBusy(false);

    }

    public IEnumerator DownloadFromServer(string filename,System.Action<bool> isReady){

        isReady(false);

        string completeUrl = downloadUrlSV1 + filename + ".scs";
        //string saveDataPath = Application.persistentDataPath +"/" + filename + ".scs";

        //StreamWriter writer;

        WWW www = new WWW(completeUrl);
        yield return www;

        if (www.isDone && string.IsNullOrEmpty(www.error)){

            Debug.Log("Downloaded " + www.bytesDownloaded + " bytes from server");

            //File.WriteAllText(saveDataPath, www.text);
            string data = www.text;
            //Debug.Log(data);

            saveShip.ReadData(data);

            //writer = new StreamWriter(saveDataPath);
            //writer.Write(www.text);
            //yield return writer;
           // writer.Close();

            //Debug.Log("LocalFile created");

            isReady(true);

        }
        else { 
            Debug.Log("No data found on server");
            isReady(false);
        }

        if (www.error != null)
            Debug.LogWarning(www.error);


    }

}
