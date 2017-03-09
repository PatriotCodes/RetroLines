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
	public gridTile[,] gridPos = new gridTile[gridWidth,gridHeigth];  // coodrinates of the centers of tiles
	public RawImage tileSprite;
	public Transform canvas;

	void Start () {
		setGridPositions();
		instantiateTiles();
	}

	void Update () {
		
	}

	// ... ... ...
	// 2,0 2,1 ...
	// 1,0 1,1 ...
	// 0,0 0,1 ...
	private void setGridPositions() {
		int centerWidth = ((Screen.width / 9) / 2);
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

	private void instantiateTiles(){
		int centerWidth = ((Screen.width / 9) / 2);
		for (int i = 0; i < gridWidth; i++) {
			for (int j = 0; j < gridHeigth; j++) {
				RawImage tile = Instantiate(tileSprite, canvas) as RawImage;
				tile.rectTransform.anchoredPosition3D = new Vector3(gridPos [i, j].x, gridPos [i, j].y, 0);
				tile.rectTransform.localScale = new Vector3(1, 1, 1);
				tile.rectTransform.sizeDelta = new Vector2(centerWidth * 2, centerWidth * 2);
			}
		}
	}
}
