using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour {

	public GameObject[] card;//card prefab
	private List<GameObject> cardList = new List<GameObject>();//all card obj on board
	private List<Card> pickCard = new List<Card>();//the card you click
	private bool animFinish = true;//Is flip ani finish?
	private int cardCount;

	// Use this for initialization
	void Start () {
		int[] order = new int[16]{ 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 };
		for (int i = 0; i < 16; i++) {
			int random = Random.Range (0, 16);
			int backup = order [i];
			order [i] = order [random];
			order [random] = backup;
		}
		for (int i = 0; i < 8; i++) {
			for (int j = 0; j < 2; j++) {
				GameObject newCard = GameObject.Instantiate (card [i]);
				newCard.AddComponent<Card> ().group = i;
				newCard.transform.parent = this.transform;

				//4*4 local cood
				float posX = order [(i * 2 + j)] % 4;
				float posY = order [(i * 2 + j)] / 4;
				newCard.transform.localPosition = new Vector2 (posX, posY);
				cardList.Add (newCard);
			}
		}
		cardCount = cardList.Count;
	}
	
	// Update is called once per frame
	void Update () {
		//Click the Card
		if (Input.GetKeyDown (KeyCode.Mouse0)) {
			RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition),Vector2.zero);
			if(hit){
					Card target = hit.transform.GetComponent<Card>();
					if(target != null){
						CheckCard(target);
					}
			}
		}
	}

	//if animating , do nothing , else check is the card fliped.Let the Card flip to front side.
	private void CheckCard(Card target){
		if (!animFinish)
			return;
		if (!pickCard.Contains (target)) {
			pickCard.Add (target);
			StartCoroutine (FlipAnim (target, pickCard));
		}
	}

	//Flip animate , turn the angle of y
	private IEnumerator FlipAnim(Card card,List<Card> pickList){
		animFinish = false;
		while (true) {
			float angle = Mathf.MoveTowards (card.transform.localEulerAngles.y, 180, 360 * Time.deltaTime);
			card.transform.localRotation = Quaternion.Euler (new Vector3 (0, angle, 0));
			if (angle == 180)
				break;
			yield return null;
		}

		//Check if the cards choosed are the same
		if (pickList.Count == 2) {
			yield return new WaitForSeconds (0.3f);
			//same cards
			if (pickList [0].group == pickList [1].group) {
				pickList [0].gameObject.SetActive (false);
				pickList [1].gameObject.SetActive (false);
				cardCount -= 2;
				if (cardCount <= 0) {
					Debug.Log ("GameOver");
				}
			}
			//different cards
		 	else {
			float angle = 180;
			while (true) {
				angle = Mathf.MoveTowards (angle, 0, 360 * Time.deltaTime);
				foreach (Card c in pickList)
					c.transform.localRotation = Quaternion.Euler (new Vector3 (0, angle, 0));
				if (angle == 0)
					break;
				yield return null;
			}
		}
		pickList.Clear ();//clear the record of choosing cards
	}
		animFinish = true;
	}
}
