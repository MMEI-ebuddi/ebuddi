using UnityEngine;
using System.Collections;
using System.Security.Cryptography;
using System;
using System.Collections.Generic;
using System.Threading;


public static class ExtensionMethods {

	public static float Remap (this float value, float from1, float to1, float from2, float to2) {
	
	    return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
	
	}



	public static void Shuffle<T>(this IList<T> list)
	{
		RNGCryptoServiceProvider provider = new RNGCryptoServiceProvider();
		int n = list.Count;
		while (n > 1)
		{
			byte[] box = new byte[1];
			do provider.GetBytes(box);
			while (!(box[0] < n * (Byte.MaxValue / n)));
			int k = (box[0] % n);
			n--;
			T value = list[k];
			list[k] = list[n];
			list[n] = value;
		}
	}
}
