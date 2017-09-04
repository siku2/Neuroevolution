using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Neuroevolution
{
	public static class Utils
	{
		public static float rangeAroundZero(float range)
		{
			return range * (2 * Random.value - 1);
		}


		public static bool chance(float chance)
		{
			return (chance <= Random.value);
		}
	}
}