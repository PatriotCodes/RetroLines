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

public class SetField : MonoBehaviour {

	public struct gridTile {  // structure for storing tile coordinates
		public int x;		  // goes from bottom left conner
		public int y;
	};

	public const int gridWidth = 9;
	public const int gridHeigth = 9;
	public gridTile[] upcomingBallsCoord = new gridTile[3];
	public gridTile[,] gridPos = new gridTile[gridWidth,gridHeigth];  // coodrinates of the centers of tiles
	public RawImage tileSprite;
	public Transform canvas;
	public int centerWidth = ((Screen.width / 9) / 2);

	void Awake() {
		setGridPositions();
		instantiateTiles();
		getUpcomingCoord();
	}

	// ... ... ...
	// 2,0 2,1 ...
	// 1,0 1,1 ...
	// 0,0 0,1 ...
	private void setGridPositions() {
		int yOff = (Screen.height - Screen.width) / 2 + centerWidth;
		for (int i = 0; i < gridWidth; i++) {
			int xOff = centerWidth;
			for (int j = 0; j < gridHeigth; j++) {
				gridPos[i,j].x = xOff;
				gridPos[i,j].y = yOff;
				xOff = xOff + (2 * centerWidth);
			}
			yOff = yOff + (2 * centerWidth);
		}
	}

	private void instantiateTiles() {
		for (int i = 0; i < gridWidth; i++) {
			for (int j = 0; j < gridHeigth; j++) {
				RawImage tile = Instantiate(tileSprite, canvas.transform) as RawImage;
				tile.rectTransform.anchoredPosition3D = new Vector3(gridPos [i, j].x, gridPos [i, j].y, 0);
				tile.rectTransform.localScale = new Vector3(1, 1, 1);
				tile.rectTransform.sizeDelta = new Vector2(centerWidth * 2, centerWidth * 2);
			}
		}
	}

	private void getUpcomingCoord() {
		int centerX = (Screen.width / 2);
		int tileSize = (Screen.width / 9);
		int centerY = Screen.height - (tileSize / 2);
		upcomingBallsCoord [0].x = (centerX - tileSize);
		upcomingBallsCoord [0].y = centerY;
		upcomingBallsCoord [1].x = centerX;
		upcomingBallsCoord [1].y = centerY;
		upcomingBallsCoord [2].x = (centerX + tileSize);
		upcomingBallsCoord [2].y = centerY;
	}
}
