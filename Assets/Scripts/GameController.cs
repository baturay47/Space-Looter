using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour {
	
	public static GameController controller = null;

	public Text coinText;

	// Player progress

	public int coins;

	public int capacity;

	public float rocketSpeed;

	public float lootSpeed;

	public float unloadSpeed;

	private GameObject[] rockets;

	private int lastrocketID;
	private int lastplanetID;



	void Awake(){
		if (controller == null) {
			controller = this;
		}
		else {
			Destroy (this.gameObject);
		}

	}

	// Use this for initialization
	void Start () {
		//Default values
		coins = 0;
		capacity = 2;
		rocketSpeed = 1.0f;
		lootSpeed = 1.0f;
		unloadSpeed = 1.0f;

		lastrocketID = 0;
		lastplanetID = 0;

		rockets = GameObject.FindGameObjectsWithTag ("Rocket");

		loadPrefs ();
		loadPrefstoRockets ();
		coinText.text = formatCoin (coins);

		assignIDs ();
			
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnApplicationQuit(){
		savePrefs ();
	}

	public void createPlanet(){
		GameObject newPlanet = Instantiate (Resources.Load ("Planet"), PointAllocator(2.0f,4.0f), Quaternion.identity) as GameObject;
		notifyRocketsofNewPlanet (newPlanet);
		newPlanet.GetComponent<PlanetScript> ().planetID = lastplanetID + 1;
		lastplanetID++;
	}

	Vector3 PointAllocator(float maxX, float maxY){
		while (true) {
			Vector3 found = new Vector3 (Random.Range (-maxX, maxX), Random.Range (-maxY, maxY), 0); 
			Collider[] colliders = Physics.OverlapSphere (found, 1.5f);
			if (colliders.Length == 0) {
				return found;
			}
		}
	}

	void notifyRocketsofNewPlanet(GameObject newPlanet){
		foreach (GameObject rocket in rockets) {
			rocket.GetComponent<RocketScript> ().newPlanetCheck (newPlanet);
		}
	}

	public void addCoin(int amount){
		coins += amount;
		coinText.text = formatCoin(coins);
	}

	string formatCoin(int coin){
		float coinF = (float)coin;
		if (coinF >= 1000000) {
			coinF /= 1000000;
			return (coinF.ToString ("N3") + "M");
		} 
		else if (coinF >= 1000) {
			coinF /= 1000;
			return (coinF.ToString ("N2") + "K");
		} 
		else {
			return (coinF.ToString ("N0"));
		}

	}

	void savePrefs(){
		PlayerPrefs.SetInt ("Coins", coins);
		PlayerPrefs.SetInt ("Capacity", capacity);
		PlayerPrefs.SetFloat ("rocketSpeed", rocketSpeed);
		PlayerPrefs.SetFloat ("lootSpeed", lootSpeed);
		PlayerPrefs.SetFloat ("unloadSpeed", unloadSpeed);
	}

	void loadPrefs(){
		coins = PlayerPrefs.GetInt ("Coins", 0);
		capacity = PlayerPrefs.GetInt ("Capacity", 5);
		rocketSpeed = PlayerPrefs.GetFloat ("rocketSpeed", 1);
		lootSpeed = PlayerPrefs.GetFloat ("lootSpeed", 1);
		unloadSpeed = PlayerPrefs.GetFloat ("unloadSpeed", 1);
	}

	void loadPrefstoRockets(){
		foreach (GameObject rocket in rockets) {
			rocket.GetComponent<RocketScript> ().loadPrefs (capacity, rocketSpeed, lootSpeed, unloadSpeed);
		}
	}

	void assignIDs(){
		foreach (GameObject rocket in rockets) {
			rocket.GetComponent<RocketScript> ().rocketID = lastrocketID + 1;
			lastrocketID++;
		}
		GameObject[] planets = GameObject.FindGameObjectsWithTag ("Planet");
		foreach (GameObject rocket in planets) {
			rocket.GetComponent<PlanetScript> ().planetID = lastplanetID + 1;
			lastplanetID++;
		}

	}

}
