using System.Collections.Generic;


namespace Neuroevolution.NeuralNetwork
{
	public class Generation
	{
		List<Genome> genomes = new List<Genome>();


		public void addGenome(Genome genome)
		{
			genomes.Add(genome);
			//TODO sort for score
		}


		public Genome[] breed(Genome parent1, Genome parent2, int children)
		{
			Genome[] offspring = new Genome[children];

			for(int childIndex = 0; childIndex < children; childIndex++)
			{
				Genome child = parent1.deepCopy();

				for(int i = 0; i < parent2.network.weights.Count; i++)
				{
					if(Utils.chance(Settings.crossoverFactor))
					{
						child.network.weights[i] = parent2.network.weights[i];
					}
				}

				for(int i = 0; i < child.network.weights.Count; i++)
				{
					if(Utils.chance(Settings.mutationRate))
					{
						child.network.weights[i] += Utils.rangeAroundZero(Settings.mutationMax);
					}
				}

				offspring[childIndex] = child;
			}

			return offspring;
		}


		public NetworkSaveState[] breedNextGenerationNetworks()
		{
			//TODO yes, u gotta do dis
			return new NetworkSaveState[0];
		}
	}
}