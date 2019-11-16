using UnityEngine;

// this script activates the particle emmiter on given time to get a sequentialExplotion effect

public class TimeBomb : MonoBehaviour {

    private ParticleSystem ps;
    public float cleanTime;

	public void ActivateOn (float time) {

        ps = GetComponent<ParticleSystem>();
        Invoke("ActivateParticles", time);
	}
	

	void ActivateParticles () {
        ps.Play();
        Invoke("CleanUp", cleanTime);
    }
    
    void CleanUp() {

        Destroy(gameObject);
    }
}
