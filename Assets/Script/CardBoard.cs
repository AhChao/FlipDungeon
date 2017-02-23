using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardBoard : MonoBehaviour {

	public GameObject[] card;//card prefab
	private List<GameObject> cardList = new List<GameObject>();//all card obj on board
	private List<GameCard> pickCard = new List<GameCard>();//the card you click
	private bool animFinish = true;//Is flip ani finish?
	private int cardCount;
	private int monsterPointTotal=10;
	private int weaponPointTotal=11;
	private int monsterPointNow=0;
	private int weaponPointNow=0;
	private int weaponOn=0;//1=on 0=off
	private int monsterOn=0;//1=on 0=off
	public GameCard lastMonster;//for unfilp last monster

	void guiUpdate(){
		Text weaponP_label;
		weaponP_label=GameObject.Find("weaponP").GetComponent<Text>();
		weaponP_label.text="Weapon Point\n"+weaponPointNow+"/"+weaponPointTotal+"(now/total)";
		Text monsterP_label;
		monsterP_label=GameObject.Find("monsterP").GetComponent<Text>();
		monsterP_label.text="Monster Point\n"+monsterPointNow+"/"+monsterPointTotal+"(now/total)";
	}
	IEnumerator unflipMonster(){
		while (true) {
			float angle = Mathf.MoveTowards (lastMonster.transform.localEulerAngles.y, 180, 360 * Time.deltaTime);
			lastMonster.transform.localRotation = Quaternion.Euler (new Vector3 (0, angle, 0));
			if (angle == 180)
				break;
			yield return null;
		}
	}

	// Use this for initialization
	void Start () {
		guiUpdate();
		//random array for setup card position
		int[] order = new int[16]{ 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 };
		for (int i = 0; i < 16; i++) {
			int random = Random.Range (0, 16);
			int backup = order [i];
			order [i] = order [random];
			order [random] = backup;
		}
		//4 monster cards
		for (int i = 0; i < 4; i++) {
			GameObject newCard = GameObject.Instantiate (card [i]);
			newCard.AddComponent<GameCard> ().group = i;
			newCard.GetComponent<GameCard>().point=i+1;
			newCard.GetComponent<GameCard>().cardType=1;//monster
			newCard.transform.parent = this.transform;

			//4*4 local cood
			float posX = order [i] % 4;
			float posY = order [i] / 4;
			newCard.transform.localPosition = new Vector2 (posX, posY);
			cardList.Add (newCard);
		}
		//6 weapon cards
		int count_pos=4;
		for (int i = 4; i < 10; i++) {
			for (int j = 1; j < 3; j++) {
				GameObject newCard = GameObject.Instantiate (card [i]);
				newCard.AddComponent<GameCard> ().group = i;
				newCard.GetComponent<GameCard> ().point = i;
				newCard.GetComponent<GameCard> ().cardType = 2;//monster
				newCard.transform.parent = this.transform;
			
				//4*4 local cood
				float posX = order [count_pos] % 4;
				float posY = order [count_pos] / 4;
				newCard.transform.localPosition = new Vector2 (posX, posY);
				cardList.Add (newCard);
				count_pos++;
			}
		}
		//set the weapon points
		cardList [4].GetComponent<GameCard>().point = 1;
		cardList [5].GetComponent<GameCard>().point = 1;
		cardList [6].GetComponent<GameCard>().point = 2;
		cardList [7].GetComponent<GameCard>().point = 2;
		cardList [8].GetComponent<GameCard>().point = 2;
		cardList [9].GetComponent<GameCard>().point = 2;
		cardList [10].GetComponent<GameCard>().point = 2;
		cardList [11].GetComponent<GameCard>().point = 2;
		cardList [12].GetComponent<GameCard>().point = 2;
		cardList [13].GetComponent<GameCard>().point = 2;
		cardList [14].GetComponent<GameCard>().point = 3;
		cardList [15].GetComponent<GameCard>().point = 3;
		cardCount = cardList.Count;

	}

	// Update is called once per frame
	void Update () {
		//Click the Card
		if (Input.GetKeyDown (KeyCode.Mouse0)) {
			RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition),Vector2.zero);
			if(hit){
				GameCard target = hit.transform.GetComponent<GameCard>();
				if(target != null){
					CheckCard(target);
				}
			}
		}
	}

	//if animating , do nothing , else check is the card fliped.Let the Card flip to front side.
	private void CheckCard(GameCard target){
		if (!animFinish)
			return;
		if (!pickCard.Contains (target)) {
			if (target.cardType == 1) {//monster card
				if (monsterOn != 1) {
					monsterPointNow = target.point;
					lastMonster = target;
					monsterOn = 1;
				} else {//a monster card is open now
					//unfilp last card
					unflipMonster();
					pickCard.Remove (lastMonster);
					monsterPointNow = target.point;
					lastMonster = target;
					monsterOn = 1;
				}
			}
			else if (target.cardType == 2) {//weapon card
				//doNothing
			}
			pickCard.Add (target);
			guiUpdate ();
			StartCoroutine (FlipAnim (target, pickCard));
		}
	}

	//Flip animate , turn the angle of y
	private IEnumerator FlipAnim(GameCard card,List<GameCard> pickList){
		animFinish = false;
		while (true) {
			float angle = Mathf.MoveTowards (card.transform.localEulerAngles.y, 180, 360 * Time.deltaTime);
			card.transform.localRotation = Quaternion.Euler (new Vector3 (0, angle, 0));
			if (angle == 180)
				break;
			yield return null;
		}

		//Check if the cards choosed are the same
		if (pickList.Count >= 2) {
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
					foreach (GameCard c in pickList)
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
