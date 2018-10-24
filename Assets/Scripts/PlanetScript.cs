using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetScript : MonoBehaviour {

	public int Resource;
	private int initResource;
	public Vector3 initScale;

	public int planetID;

	public List<GameObject> attachedRockets = new List<GameObject>();

	// Use this for initialization
	void Start () {
		Resource = 3;
		initResource = 3;
		this.GetComponentInChildren<TextMesh> ().text = formatCoin(Resource);
		initScale = transform.GetChild (0).transform.localScale;
	}
	
	// Update is called once per frame
	void Update () {
		if (Resource <= 0)
			KillYourself ();
	}
		

	public int Load(int amount){
		if (Resource <= amount) {
			int temp = Resource;
			Resource = 0;
			return temp;
		} else {
			Resource -= amount;
			this.transform.GetComponentInChildren<TextMesh> ().text = formatCoin (Resource);
			transform.GetChild (0).transform.localScale = initScale * Resource / initResource;
			return amount;
		}
	}
		
	public void Attach(GameObject rocket){
		int temp = attachedRockets.Count;
		attachedRockets.Add (rocket);
		Debug.Log (planetID + " ID'li planete bir tane ekliyorum, " + temp + " idi " + attachedRockets.Count + " oldu");
	}

	public void Detach(GameObject rocket){
		int temp = attachedRockets.Count;
		attachedRockets.Remove (rocket);
		Debug.Log (planetID + " ID'li planetten bir tane cikariyorum, " + temp + " idi " + attachedRockets.Count + " oldu");

	}

	void KillYourself(){
		this.gameObject.SetActive (false);
		while (attachedRockets.Count != 0) {
			attachedRockets[0].GetComponent<RocketScript> ().LeavePlanet ();
		}
		Destroy (this.gameObject);
		GameController.controller.createPlanet ();
	}

	string formatCoin(float coin){
		if (coin >= 1000000) {
			coin /= 1000000;
			return (coin.ToString ("N1") + "M");
		} 
		else if (coin >= 1000) {
			coin /= 1000;
			return (coin.ToString ("N1") + "K");
		} 
		else {
			return (coin.ToString ("N0"));
		}

	}
}
