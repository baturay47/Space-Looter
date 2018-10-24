using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketScript : MonoBehaviour {

	public GameObject attachedPlanet = null;

	public GameObject attachedBase = null;

	public IEnumerator activeCoroutine = null;

	public bool activeMode;  // true means planet, false means base

	public int capacity;

	public int collected;

	public float rocketSpeed;

	public float lootSpeed;

	public float unloadSpeed;

	public int rocketID;


	// Use this for initialization
	void Start () {
		collected = 0;
		attachedPlanet = FindNewPlanet ();
		attachedPlanet.GetComponent<PlanetScript> ().Attach (this.gameObject);
		activeMode = true;
		MovetoPoint (attachedPlanet.transform.position);


		// Rocket stats

		capacity = 2;
		rocketSpeed = 1.0f;
		lootSpeed = 1.0f;
		unloadSpeed = 1.0f;

	}

	
	// Update is called once per frame
	void Update () {
		if (collected == capacity && attachedBase == null) {
			gotoBase ();
		} 
		else if (collected == 0 && attachedBase != null) {
			LeaveBase ();
		}
	}

	void OnTriggerEnter(Collider col){
		if (col.CompareTag ("Planet")) {
			if (activeMode && attachedPlanet == col.gameObject) {
				if (activeCoroutine != null) {
					StopCoroutine (activeCoroutine);
				}
				activeCoroutine = LootPlanet (attachedPlanet);
				StartCoroutine (activeCoroutine);
			}
		} 
		else if (col.CompareTag ("Base")) {
			if (!activeMode) {
				attachedBase = col.gameObject;
				if (activeCoroutine != null) {
					StopCoroutine (activeCoroutine);
				}
				activeCoroutine = UnloadtoBase (attachedBase);
				StartCoroutine (activeCoroutine);
			}
		}
	}

	public void LeavePlanet(){
		StopCoroutine (activeCoroutine);
		activeCoroutine = null;
		attachedPlanet.GetComponent<PlanetScript> ().Detach (this.gameObject);
		attachedPlanet = null;
		if (collected == capacity) {
			attachedBase = FindBase ();
			activeMode = false;
			MovetoPoint (attachedBase.transform.position);
		} 
		else {
			attachedPlanet = FindNewPlanet ();
			activeMode = true;
			attachedPlanet.GetComponent<PlanetScript> ().Attach (this.gameObject);
			MovetoPoint (attachedPlanet.transform.position);
		}

	}

	void gotoBase(){
		StopCoroutine (activeCoroutine);
		attachedPlanet.GetComponent<PlanetScript> ().Detach (this.gameObject);
		attachedPlanet = null;
		activeMode = false;
		attachedBase = FindBase ();
		MovetoPoint (attachedBase.transform.position);
	}

	void LeaveBase(){
		attachedBase = null;
		attachedPlanet = FindNewPlanet ();
		activeMode = true;
		attachedPlanet.GetComponent<PlanetScript> ().Attach (this.gameObject);
		MovetoPoint (attachedPlanet.transform.position);
	}



	public void MovetoPoint(Vector3 goal){
		if (activeCoroutine != null){
			StopCoroutine (activeCoroutine);
		}
		activeCoroutine = MoveToPosition(goal);
		StartCoroutine (activeCoroutine);
	}

	public IEnumerator MoveToPosition(Vector3 goal){
		Vector3 pos;
		transform.up = goal - transform.position;
		while ((pos = transform.position )!= goal) {
			float step = rocketSpeed*Time.deltaTime;
			transform.position = Vector3.MoveTowards(pos, goal , step);
			yield return null;
		}
	}

	public IEnumerator LootPlanet(GameObject planet){
		PlanetScript script = planet.GetComponent<PlanetScript>();
		float toOne = 0.0f;
		while (script.Resource > 0) {
			float step = lootSpeed*Time.deltaTime;
			toOne += step;
			if (toOne >= 1.0f) {
				int FloortoOne = Mathf.FloorToInt (toOne);
				toOne = 0.0f;
				if (FloortoOne <= capacity - collected)
					collected += (script.Load (FloortoOne));
				else
					collected += (script.Load (capacity - collected));
			}

			yield return null;
		}
	}

	public IEnumerator UnloadtoBase(GameObject baz){
		BaseScript script = baz.GetComponent<BaseScript>();
		float toOne = 0.0f;
		while (collected > 0) {
			float step = unloadSpeed*Time.deltaTime;
			toOne += step;
			if (toOne >= 1.0f) {
				int FloortoOne = Mathf.FloorToInt (toOne);
				toOne = 0.0f;
				if (FloortoOne < collected) {
					script.Collect (FloortoOne);
					collected -= FloortoOne;
				} 
				else {
					script.Collect (collected);
					collected = 0;
				}
			}
			yield return null;
		}
	}


	GameObject FindNewPlanet(){
		GameObject[] allplanets = GameObject.FindGameObjectsWithTag ("Planet");
		float minDistance = Mathf.Infinity;
		GameObject selectedPlanet = null;
		float distance;
		foreach (GameObject planet in allplanets) {
			if ((distance = Vector3.Distance(this.transform.position,planet.transform.position)) < minDistance){
				selectedPlanet = planet;
				minDistance = distance;
			}
		}
		return selectedPlanet;
	}

	GameObject FindBase(){
		GameObject[] allbases = GameObject.FindGameObjectsWithTag ("Base");
		float minDistance = Mathf.Infinity;
		GameObject selectedBase = null;
		float distance;
		foreach (GameObject baz in allbases) {
			if ((distance = Vector3.Distance(this.transform.position,baz.transform.position)) < minDistance){
				selectedBase = baz;
				minDistance = distance;
			}
		}
		return selectedBase;
	}

	public void newPlanetCheck(GameObject newPlanet){
		if (activeMode && attachedPlanet != null) {
			if (Vector3.Distance (this.transform.position, newPlanet.transform.position) < Vector3.Distance (this.transform.position, attachedPlanet.transform.position)) {
				attachedPlanet.GetComponent<PlanetScript> ().Detach (this.gameObject);
				attachedPlanet = newPlanet;
				attachedPlanet.GetComponent<PlanetScript> ().Attach (this.gameObject);
				MovetoPoint (newPlanet.transform.position);
			}
		}
	}

	public void loadPrefs(int _capacity, float _rocketSpeed, float _lootSpeed, float _unloadSpeed){
		capacity = _capacity;
		rocketSpeed = _rocketSpeed;
		lootSpeed = _lootSpeed;
		unloadSpeed = _unloadSpeed;
	}
		
}
