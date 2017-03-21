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

public class Game : MonoBehaviour {

	public struct AddedBall {  // structure for storing tile coordinates
		public int x;		  // goes from bottom left conner
		public int y;
		public int ballIndex;

		public AddedBall(int x, int y, int ballIndex) {
			this.x = x;
			this.y = y;
			this.ballIndex = ballIndex;
		}
	};

	public struct Tile {  // structure for storing tile coordinates
		public int x;		  // goes from bottom left conner
		public int y;

		public Tile(int x, int y) {
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
	private bool userMove;
	private int[] upcomingBalls = new int[3];
	private SetField.gridTile[] upcomingCoord = new SetField.gridTile[3];
	private int ballSize = (Screen.width / 9);
	private List<AddedBall> addedBalls = new List<AddedBall>();
	private List<Tile> freeTiles = new List<Tile>();
	public const int gridWidth = 9;
	public const int gridHeigth = 9;
	private IEnumerator coroutine;

	void Awake() {
		InstantiateFreeTiles();
	}

	void Start() {
		userMove = false;
		SetUpcomingBalls();
		AddBalls();
	}

	void Update() {
		if (!userMove) {
			SetUpcomingBalls();
			AddUpcomingBalls();
			userMove = true;
		}
		if ( Input.GetMouseButtonDown(0)) {
			Vector2 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			RaycastHit2D hit = Physics2D.Raycast(worldPoint, Vector2.zero);
			if ( hit.collider != null ) {
				if (coroutine != null) {
					StopCoroutine(coroutine);
					prevBall.anchoredPosition = startupPosition;
				}
				ballTransform = hit.collider.gameObject.transform as RectTransform;
				prevBall = ballTransform;
				startupPosition = ballTransform.anchoredPosition;
				coroutine = BallAnimation(0.35f, ballTransform);
				StartCoroutine(coroutine);
			}
		}
	}

	private void SetUpcomingBalls() {  // getting which balls will be added
		for (int i = 0; i < 3; i++) {
			int randInt = Random.Range(0, 6);
			upcomingBalls[i] = randInt;
			RawImage ball = Instantiate(balls[randInt], canvas.transform) as RawImage;
			ball.rectTransform.anchoredPosition3D = new Vector3(setField.upcomingBallsCoord[i].x, setField.upcomingBallsCoord[i].y, 1);
			ball.rectTransform.localScale = new Vector3(1, 1, 1);
			ball.rectTransform.sizeDelta = new Vector2(ballSize, ballSize);
		}
	}

	private void AddUpcomingBalls() {  // adding small presentations of upcoming balls
		GetUpcomingCoord();
		for (int i = 0; i < 3; i++) {
			RawImage ball = Instantiate(balls[upcomingBalls[i]], canvas.transform) as RawImage;
			ball.rectTransform.anchoredPosition3D = new Vector3(setField.gridPos[upcomingCoord[i].x,upcomingCoord[i].y].x, setField.gridPos[upcomingCoord[i].x,upcomingCoord[i].y].y, 1);
			ball.rectTransform.localScale = new Vector3(0.4f, 0.4f, 0.4f);
			ball.rectTransform.sizeDelta = new Vector2(ballSize, ballSize);
		}
	}

	private void AddBalls() {  // adding balls to the field
		GetUpcomingCoord();
		for (int i = 0; i < 3; i++) {
			RawImage ball = Instantiate(balls[upcomingBalls[i]], canvas.transform) as RawImage;
			ball.rectTransform.anchoredPosition3D = new Vector3(setField.gridPos[upcomingCoord[i].x,upcomingCoord[i].y].x, setField.gridPos[upcomingCoord[i].x,upcomingCoord[i].y].y, 1);
			ball.rectTransform.localScale = new Vector3(1, 1, 1);
			ball.rectTransform.sizeDelta = new Vector2(ballSize, ballSize);
			BoxCollider2D ballCollider = ball.GetComponent(typeof(BoxCollider2D)) as BoxCollider2D;
			ballCollider.size = new Vector2(ballSize, ballSize);
		}
	}

	private void GetUpcomingCoord() {  // getting coordinates to which balls will be added
		for (int i = 0; i < 3; i++) {
			int randIndex = Random.Range (0, freeTiles.Count); 
			addedBalls.Add (new AddedBall (freeTiles[randIndex].x, freeTiles[randIndex].y, upcomingBalls[i]));
			upcomingCoord[i].x = freeTiles[randIndex].x;
			upcomingCoord[i].y = freeTiles[randIndex].y;
			freeTiles.RemoveAt(randIndex);
		}
	}

	private void InstantiateFreeTiles() {
		for (int i = 0; i < gridWidth; i++) {
			for (int j = 0; j < gridHeigth; j++) {
				freeTiles.Add(new Tile (i, j));
			}
		}
	}
		
	private IEnumerator BallAnimation(float waitTime, RectTransform transform) {
		while (true) {
			transform.anchoredPosition = new Vector2(transform.anchoredPosition.x, transform.anchoredPosition.y + (Screen.height / 80));
			yield return new WaitForSeconds(waitTime);
			transform.anchoredPosition = new Vector2(transform.anchoredPosition.x, transform.anchoredPosition.y - (Screen.height / 80));
			yield return new WaitForSeconds(waitTime);
		}
	}
}
