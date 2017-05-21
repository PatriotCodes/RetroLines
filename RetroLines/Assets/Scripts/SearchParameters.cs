using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// https://github.com/mclift/SimpleAStarExample
/// </summary>

public class SearchParameters {

	public Point StartLocation { get; set; }

	public Point EndLocation { get; set; }

	public bool[,] Map { get; set; }

	public SearchParameters(Point startLocation, Point endLocation, bool[,] map) {
		this.StartLocation = startLocation;
		this.EndLocation = endLocation;
		this.Map = map;
	}
}
