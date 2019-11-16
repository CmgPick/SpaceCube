using UnityEngine;

public class DestroyOnTime : MonoBehaviour {

    public float destoyTime;
    private float elapsedTime;

	void FixedUpdate () {

        elapsedTime += Time.deltaTime;

        if (elapsedTime >= destoyTime)
            Destroy(this.gameObject);
	}
}
