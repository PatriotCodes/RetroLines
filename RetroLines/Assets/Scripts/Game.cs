/*  Retro Lines. Lines 98 clone done on unity.
    Copyright (C) 2017  Alexander Triukhan

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <http://www.gnu.org/licenses/>. */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Game : MonoBehaviour { 

	public struct Ball {
		public int x;
		public int y;
		public int ballIndex;
		public RawImage ballObject;

		public Ball(int x, int y, int ballIndex, RawImage ballObject) {
			this.x = x;
			this.y = y;
			this.ballIndex = ballIndex;
			this.ballObject = ballObject;
		}
	};

	public struct Tile {  // structure for storing tile coordinates
		public int i;		  // goes from bottom left conner
		public int j;
		public int x;
		public int y;

		public Tile(int i, int j, int x, int y) {
			this.i = i;
			this.j = j;
			this.x = x;
			this.y = y;
		}
	};

	public Transform canvas;
	private RectTransform ballTransform;
	private RectTransform prevBall;
	private Vector2 startupPosition;
	private bool firstClick;
	public SetField setField;
	public RawImage[] balls = new RawImage[7];
	public GameObject gameOverScreen;
	private bool userMove;
	private int[] upcomingBalls = new int[3];
	private RawImage[] upcomingBallsRaw = new RawImage[3];
	private int ballSize;
	private List<Tile> freeTiles = new List<Tile>();
	private List<Tile> allTiles = new List<Tile>();
	private Ball currBall;
	private Tile moveTile = new Tile();  // tile to which we move our ball during the turn
	private Tile[] upcomingTiles = new Tile[3];
	private RawImage[] upcomingObjects = new RawImage[3];
	private List<Ball> addedBalls = new List<Ball>();
	public const int gridWidth = 9;
	public const int gridHeigth = 9;
	private IEnumerator coroutine;
	private string ballTag = "Ball";
	private string tileTag = "Tile";
	public Text totalBallsText;
	public Text scoreText;
	public Text gameOverText;
	private int totalBalls = 0;
	private int totalScore = 0;
	private bool gameIsOver = false;

	void Awake() {
		InstantiateFreeTiles();
	}

	void Start() {
		ballSize = (Screen.width / 9);
		SetUpcomingBalls();
		AddUpcomingBalls();
		AddBalls();
		SetUpcomingBalls();
		AddUpcomingBalls();
	}

	void Update() {
		if (!gameIsOver) {
			if (Input.GetMouseButtonDown (0)) {
				Vector2 worldPoint = Camera.main.ScreenToWorldPoint (Input.mousePosition);
				RaycastHit2D[] hit = Physics2D.RaycastAll (worldPoint, Vector2.zero);
				if (hit.Length > 1) {  // we hit ball
					if (hit [1].collider.tag == ballTag) {
						if (coroutine != null) {
							StopCoroutine (coroutine);
							prevBall.anchoredPosition = startupPosition;
						}
						ballTransform = hit [1].collider.gameObject.transform as RectTransform;
						prevBall = ballTransform;
						startupPosition = ballTransform.anchoredPosition;
						coroutine = BallAnimation (0.35f, ballTransform);
						StartCoroutine (coroutine);
					}
				}
				if (hit.Length == 1) {  // we hit field tile
					if (hit [0].collider.tag == tileTag) {
						if (prevBall != null) {
							RectTransform tileTransform = hit [0].collider.gameObject.transform as RectTransform;
							if (IsTileFree (tileTransform)) {
								if (coroutine != null) {
									StopCoroutine (coroutine);
									coroutine = null;
									// TODO: FindPath
									MoveBall (tileTransform);
									if (!FindLineAndRemove (currBall)) {  // TODO: Check situation when line was formed after balls were added //Do FindLineAndRemove() for each added ball
										AddBalls ();
										SetUpcomingBalls ();
										AddUpcomingBalls ();
									}
								}
							}
						}
					}
				}
			}
		}
	}

	private void SetUpcomingBalls() {  // getting which balls will be added (what colour)
		for (int i = 0; i < 3; i++) {
			int randInt = Random.Range(0, 6);
			upcomingBalls[i] = randInt;
			if (upcomingBallsRaw[i] != null) {
				Destroy(upcomingBallsRaw[i].gameObject);
			}
			RawImage ball = Instantiate(balls[randInt], canvas.transform) as RawImage;
			upcomingBallsRaw[i] = ball;
			ball.rectTransform.anchoredPosition3D = new Vector3(setField.upcomingBallsCoord[i].x, setField.upcomingBallsCoord[i].y, 1);
			ball.rectTransform.localScale = new Vector3(1, 1, 1);
			ball.rectTransform.sizeDelta = new Vector2(ballSize, ballSize);
		}
	}

	private void AddUpcomingBalls() {  // adding small presentations of upcoming balls
		GetUpcomingCoord();
		if (!gameIsOver) {
			for (int i = 0; i < 3; i++) {
				RawImage ball = Instantiate (balls [upcomingBalls [i]], canvas.transform) as RawImage;
				ball.rectTransform.anchoredPosition3D = new Vector3 (upcomingTiles [i].x, upcomingTiles [i].y, 1);
				ball.rectTransform.localScale = new Vector3 (0.4f, 0.4f, 0.4f);
				ball.rectTransform.sizeDelta = new Vector2 (ballSize, ballSize);
				upcomingObjects [i] = ball;
			}
		}
	}

	private void AddBalls() {  // adding balls to the field
		for (int i = 0; i < 3; i++) {
			RawImage ball = Instantiate (balls [upcomingBalls [i]], canvas.transform) as RawImage; 
			if (!upcomingTiles[i].Equals(moveTile)) {
				ball.rectTransform.anchoredPosition3D = new Vector3 (upcomingTiles [i].x, upcomingTiles [i].y, 1);
				addedBalls.Add(new Ball(upcomingTiles[i].i,upcomingTiles[i].j,upcomingBalls[i],ball));
			} else {
				Tile newTile = GetFreeTile();
				ball.rectTransform.anchoredPosition3D = new Vector3 (newTile.x, newTile.y, 1);
				addedBalls.Add(new Ball(newTile.i,newTile.j,upcomingBalls[i],ball));
			}
			ball.rectTransform.localScale = new Vector3 (1, 1, 1);
			ball.rectTransform.sizeDelta = new Vector2 (ballSize, ballSize);
			ball.tag = ballTag;
			BoxCollider2D ballCollider = ball.GetComponent (typeof(BoxCollider2D)) as BoxCollider2D;
			ballCollider.size = new Vector2 (ballSize, ballSize);
			Destroy (upcomingObjects[i].gameObject);
			totalBalls++;
		}
		totalBallsText.text = BallsString(totalBalls);
	}

	private void GetUpcomingCoord() {  // getting coordinates to which balls will be added
		if (freeTiles.Count > 3) {
			for (int i = 0; i < 3; i++) {
				int randIndex = Random.Range (0, freeTiles.Count);
				upcomingTiles [i] = freeTiles [randIndex];
				freeTiles.RemoveAt (randIndex);
			}
		} else {
			GameOver();
		}
	}

	private Tile GetFreeTile() {
		int randIndex = Random.Range (0, freeTiles.Count);
		Tile res = freeTiles[randIndex];
		freeTiles.RemoveAt(randIndex);
		return res;
	}

	private void InstantiateFreeTiles() {
		for (int i = 0; i < gridWidth; i++) {
			for (int j = 0; j < gridHeigth; j++) {
				freeTiles.Add(new Tile (i, j, setField.gridPos[i,j].x, setField.gridPos[i,j].y));
				allTiles.Add(new Tile (i, j, setField.gridPos [i,j].x, setField.gridPos [i,j].y));
			}
		}
	}

	private IEnumerator BallAnimation(float waitTime, RectTransform transform) { // TODO: Jumping ball z = 2
		while (true) {
			transform.anchoredPosition = new Vector2(transform.anchoredPosition.x, transform.anchoredPosition.y + (Screen.height / 80));
			yield return new WaitForSeconds(waitTime);
			transform.anchoredPosition = new Vector2(transform.anchoredPosition.x, transform.anchoredPosition.y - (Screen.height / 80));
			yield return new WaitForSeconds(waitTime);
		}
	}

	private bool IsTileFree(RectTransform transform) { // TODO: Implement
		return true;
	}

	private void MoveBall(RectTransform finalPosition) {
		prevBall.anchoredPosition = startupPosition;
		prevBall.anchoredPosition = finalPosition.anchoredPosition;
		Tile startTile = GetIJTile((int)startupPosition.x, (int)startupPosition.y);
		Ball mBall = addedBalls[GetBallIndexByXY(startTile.i, startTile.j)];
		addedBalls.Remove(mBall);
		freeTiles.Add(startTile);
		moveTile = GetIJTile((int)finalPosition.anchoredPosition.x, (int)finalPosition.anchoredPosition.y);
		currBall = new Ball (moveTile.i, moveTile.j, mBall.ballIndex, mBall.ballObject);
		addedBalls.Add(currBall);
		freeTiles.Remove(moveTile);
	}

	private Tile GetIJTile(int x, int y) {
		for (int i = 0; i < allTiles.Count; i++) {
			if (allTiles[i].x == x && allTiles[i].y == y) {
				return allTiles[i];
			}
		}
		return new Tile();
	}

	private Tile GetXYTile(int x, int y) {
		for (int i = 0; i < allTiles.Count; i++) {
			if (allTiles[i].i == x && allTiles[i].j == y) {
				return allTiles[i];
			}
		}
		return new Tile();
	}

	private string BallsString(int number) {
		if (number >= 10) {
			return "000" + number.ToString();
		} else if (number >= 100) {
			return "00" + number.ToString();
		} else if (number >= 1000) {
			return "0" + number.ToString();
		} else {
			return "0000" + number.ToString();
		}
	}

	private bool FindLineAndRemove(Ball currentBall) { // TODO: Implement
		if (FindVertical (currentBall)) {
			return true;
		}
		if (FindHorizontal (currentBall)) {
			return true;
		}
		return false;
	}

	private bool FindVertical(Ball currentBall) {
		int counter = 1;
		int nextIndex;
		List<RawImage> ballsInLine = new List<RawImage>();
		List<Ball> ballsInAddedList = new List<Ball>();
		for (int i = currentBall.x + 1; i <= 9; i++) {
			nextIndex = GetBallIndexByXY (i, currentBall.y);
			if (nextIndex != -1) {
				if (addedBalls [nextIndex].ballIndex == currentBall.ballIndex) {
					counter++;
					ballsInLine.Add (addedBalls [nextIndex].ballObject);
					ballsInAddedList.Add (addedBalls [nextIndex]);
				} else
					break;
			} else {
				break;
			}
		}
		for (int i = currentBall.x - 1; i >= 0; i--) {
			nextIndex = GetBallIndexByXY (i, currentBall.y);
			if (nextIndex != -1) {
				if (addedBalls [nextIndex].ballIndex == currentBall.ballIndex) {
					counter++;
					ballsInLine.Add (addedBalls [nextIndex].ballObject);
					ballsInAddedList.Add (addedBalls [nextIndex]);
				} else
					break;
			} else {
				break;
			}
		}
		if (counter >= 5) {
			ballsInLine.Add (currentBall.ballObject);
			ballsInAddedList.Add (currentBall);
			for (int i = 0; i < ballsInLine.Count; i++) {
				Destroy (ballsInLine [i].gameObject);
				Tile tmp = (GetXYTile (ballsInAddedList [i].x, ballsInAddedList [i].y));
				freeTiles.Add (tmp);
				addedBalls.Remove (ballsInAddedList [i]);
			}
			totalBalls -= counter;
			totalScore += counter;
			totalBallsText.text = BallsString(totalBalls);
			scoreText.text = BallsString(totalScore);
			return true;
		}
		return false;
	}

	private bool FindHorizontal(Ball currentBall) {
		int counter = 1;
		int nextIndex;
		List<RawImage> ballsInLine = new List<RawImage>();
		List<Ball> ballsInAddedList = new List<Ball>();
		for (int i = currentBall.y + 1; i <= 9; i++) {
			nextIndex = GetBallIndexByXY (currentBall.x, i);
			if (nextIndex != -1) {
				if (addedBalls [nextIndex].ballIndex == currentBall.ballIndex) {
					counter++;
					ballsInLine.Add (addedBalls [nextIndex].ballObject);
					ballsInAddedList.Add (addedBalls [nextIndex]);
				} else
					break;
			} else {
				break;
			}
		}
		for (int i = currentBall.y - 1; i >= 0; i--) {
			nextIndex = GetBallIndexByXY (currentBall.x, i);
			if (nextIndex != -1) {
				if (addedBalls [nextIndex].ballIndex == currentBall.ballIndex) {
					counter++;
					ballsInLine.Add (addedBalls [nextIndex].ballObject);
					ballsInAddedList.Add (addedBalls [nextIndex]);
				} else
					break;
			} else {
				break;
			}
		}
		if (counter >= 5) {
			ballsInLine.Add (currentBall.ballObject);
			ballsInAddedList.Add (currentBall);
			for (int i = 0; i < ballsInLine.Count; i++) {
				Destroy (ballsInLine [i].gameObject);
				Tile tmp = (GetXYTile (ballsInAddedList [i].x, ballsInAddedList [i].y));
				freeTiles.Add (tmp);
				addedBalls.Remove (ballsInAddedList [i]);
			}
			totalBalls -= counter;
			totalScore += counter;
			totalBallsText.text = BallsString(totalBalls);
			scoreText.text = BallsString(totalScore);
			return true;
		}
		return false;
	}

		
	private void FindDiagonal() { // TODO: Implement
	}

	private int GetBallIndexByXY(int x, int y) { // return -1 if no ball with such x,y coords found
		for (int i = 0; i < addedBalls.Count; i++) {
			if (addedBalls [i].x == x && addedBalls [i].y == y) {
				return i;
			}
		}
		return -1;
	}

	private void GameOver() { // TODO: Implement
		gameIsOver = true;
		gameOverText.text = "Your Score: " + BallsString(totalScore);
		gameOverScreen.transform.SetParent(canvas);
		gameOverScreen.SetActive(true);
	}

	public void Restart() {
		SceneManager.LoadScene ("GameStage", LoadSceneMode.Single);
	}
}
