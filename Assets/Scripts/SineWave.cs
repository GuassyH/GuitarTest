using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SineWave
{



	//Creates a sinewave
	public static float CreateSine(int timeIndex, float frequency, float sampleRate)
	{
		float baseSine =  Mathf.Sin(2 * Mathf.PI * timeIndex * frequency / sampleRate);		
		return baseSine;
	}




}
