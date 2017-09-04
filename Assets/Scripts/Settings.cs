using System;


namespace Neuroevolution
{
	public static class Settings
	{
		public static float crossoverFactor = .5f;
		public static float mutationRate = .1f;
		public static float mutationMax = .5f;


		public static float randomClamped()
		{
			return Utils.rangeAroundZero(1f);
		}


		// Logistic activation function taken from https://en.wikipedia.org/wiki/Logistic_function
		public static float activation(float stimulate)
		{
			float ap = (-stimulate) / 1;
			return (float) (1 / (1 + Math.Exp(ap)));
		}
	}
}