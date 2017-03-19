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

	public Transform canvas;
	public SetField setField;
	public RawImage[] balls = new RawImage[7];
	private bool userMove;
	private int[] upcomingBalls = new int[3];
	private int ballSize = (Screen.width / 9);

	void Start () {
		userMove = false;
	}

	void Update () {
		if (!userMove) {
			SetUpcomingBalls();
			userMove = true;
		}
	}

	private void SetUpcomingBalls() {
		for (int i = 0; i < 3; i++) {
			int randInt = Random.Range(0, 6);
			upcomingBalls[i] = randInt;
			RawImage ball = Instantiate(balls[randInt], canvas.transform) as RawImage;
			ball.rectTransform.anchoredPosition3D = new Vector3(setField.upcomingBallsCoord[i].x, setField.upcomingBallsCoord[i].y, 1);
			ball.rectTransform.localScale = new Vector3(1, 1, 1);
			ball.rectTransform.sizeDelta = new Vector2(ballSize, ballSize);
		}
	}

	private void AddBalls() {
		for (int i = 0; i < 3; i++) {
			RawImage ball = Instantiate(balls[upcomingBalls[i]], canvas.transform) as RawImage;
			ball.rectTransform.anchoredPosition3D = new Vector3(setField.gridPos[i,i].x, setField.gridPos[i,i].y, 1);
			ball.rectTransform.localScale = new Vector3(1, 1, 1);
			ball.rectTransform.sizeDelta = new Vector2(ballSize, ballSize);
		}
	}
}
