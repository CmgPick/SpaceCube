using UnityEngine;

public class ShipManager : MonoBehaviour {

    public GameObject [] pieces; // the ship pieces // change to private later
    public int [] piecesHp; // the current Hp of the ship pieces // change to private later

    // receives a piece name and a damage value, splits the name string to get the piece
    //index number, susbtracts the hp to corresponding piece and destroys it if needed

        //TODO populate the arrays somehow

    public void DamagePiece( string pieceName , float damage) {

        string[] splittedName = pieceName.Split('_');
        int piece = int.Parse(splittedName[0]);

        piecesHp[piece] -= (int)damage;

        if (piecesHp[piece] <= 0)
            Destroy(pieces[piece]);

    }
}
