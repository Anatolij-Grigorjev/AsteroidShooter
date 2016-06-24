using System;
using UnityEngine;
using System.Collections;
using AssemblyCSharp;

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

	public static String GetRandomName(bool junior = false) {
		String emotion = RandomNames.SENIOR_FEELINGS [UnityEngine.Random.Range (0, RandomNames.SENIOR_FEELINGS.Length)];
		String name = emotion + " " + RandomNames.SENIOR_NAMES[UnityEngine.Random.Range (0, RandomNames.SENIOR_NAMES.Length)];

		return junior ? name + " Jr." : name;
	}

}


