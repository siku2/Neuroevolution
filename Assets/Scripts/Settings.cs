using System;


namespace Neuroevolution
{
	public static class Settings
	{
		public static readonly int inputNeurons = 3;
		public static readonly int[] hiddenNeurons = {5, 5};
		public static readonly int ouputNeurons = 1;

		public static readonly int populationSize = 25;
		public static readonly int childAmount = 1;

		public static readonly float crossoverFactor = .5f;
		public static readonly float mutationRate = .05f;
		public static readonly float mutationMax = .5f;
		public static readonly float leaveBestPercentage = .3f;
		public static readonly float newcomerPercentage = .05f;


		public static float randomClamped()
		{
			return Utils.rangeAroundZero(1f);
		}


		// Logistic activation function taken from https://en.wikipedia.org/wiki/Logistic_function
		public static float activation(float stimulate)
		{
			return (float) (1f / (1f + Math.Exp(-stimulate)));
		}
	}
}