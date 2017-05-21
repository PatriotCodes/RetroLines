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

public class UIScaler : MonoBehaviour {

	public Text ScoreText;
	public Text BallsText;
	public Text GameOverScoreText;
	public Button RepeatButton;
	public Button QuitButton;

	private const int minScreenWidth = 240;
	private int offsetYDivider = 90;
	private int offsetXDivider = 120;

	void Start () {
		SetScale();
		SetOffset();
		ScoreText.text = "00000";
		BallsText.text = "00000";
	}
		
	private void SetScale() {
		float scaleMultiplier = (float)Screen.width / (float)minScreenWidth;
		ScoreText.rectTransform.localScale = new Vector2(scaleMultiplier, scaleMultiplier);
		BallsText.rectTransform.localScale = new Vector2(scaleMultiplier, scaleMultiplier);
		GameOverScoreText.rectTransform.localScale = new Vector2(scaleMultiplier, scaleMultiplier);
		RepeatButton.transform.localScale = new Vector2(scaleMultiplier, scaleMultiplier);
		QuitButton.transform.localScale = new Vector2(scaleMultiplier, scaleMultiplier);
	}

	private void SetOffset() {
		ScoreText.rectTransform.anchoredPosition = new Vector2((Screen.height / offsetXDivider), -(Screen.height / offsetYDivider));
		BallsText.rectTransform.anchoredPosition = new Vector2(-(Screen.height / offsetXDivider) - (BallsText.rectTransform.sizeDelta.x * BallsText.rectTransform.localScale.x), -(Screen.height / offsetYDivider));
		GameOverScoreText.rectTransform.anchoredPosition = new Vector2(((-GameOverScoreText.rectTransform.sizeDelta.x * 2) + (GameOverScoreText.rectTransform.sizeDelta.x / 6)),(0));
		RepeatButton.GetComponent<RectTransform>().anchoredPosition = new Vector2(RepeatButton.GetComponent<RectTransform>().sizeDelta.x / 4, -RepeatButton.GetComponent<RectTransform>().sizeDelta.y * 3); // check x variable
		QuitButton.GetComponent<RectTransform>().anchoredPosition = new Vector2(20, 0);
	}
}
