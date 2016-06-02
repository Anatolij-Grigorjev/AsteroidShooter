using System;
using UnityEngine;
using System.Collections;


public class Utils
{
	private Utils () {}

	/**
	 * Gets component in ACTUAL child, skipping the parent
	 * */
	public static T GetComponentInChild<T>(Component root) where T : Component {
		T[] components = root.GetComponentsInChildren<T> ();
		foreach (T comp in components) {
			if (comp.transform.parent != null) {
				return comp;
			}
		}
		return default(T);
	}

}


