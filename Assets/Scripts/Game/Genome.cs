using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;

using Neuroevolution.NeuralNetwork;


namespace Neuroevolution.Generations
{
	[System.Serializable]
	public class Genome
	{
		public NetworkSaveState network;
		public float fitness;


		public Genome(NetworkSaveState network, float fitness)
		{
			this.network = network;
			this.fitness = fitness;
		}


		public Genome deepCopy()
		{
			using(var ms = new MemoryStream())
			{
				var formatter = new BinaryFormatter();
				formatter.Serialize(ms, this);
				ms.Position = 0;

				return (Genome) formatter.Deserialize(ms);
			}
		}
	}
}