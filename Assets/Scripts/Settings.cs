using System;


namespace Neuroevolution
{
	public static class Settings
	{
		public static readonly int inputNeurons = 4;
		public static readonly int[] hiddenNeurons = { 4, 4, 2 };
		public static readonly int ouputNeurons = 1;

		public static readonly int populationSize = 50;

		public static readonly float crossoverFactor = .5f;
		public static readonly float mutationRate = .1f;
		public static readonly float mutationMax = .5f;


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