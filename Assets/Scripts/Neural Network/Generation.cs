using System.Collections.Generic;


namespace Neuroevolution.NeuralNetwork
{
	public class Generation
	{
		List<Genome> genomes = new List<Genome>();


		public void addGenome(Genome genome)
		{
			int suitableIndex = 0;

			for(int i = 0; i < genomes.Count; i++)
			{
				if(genomes[i].fitness < genome.fitness)
				{
					suitableIndex = i;
					break;
				}
			}

			genomes.Insert(suitableIndex, genome);
		}


		public NetworkSaveState[] breed(NetworkSaveState parent1, NetworkSaveState parent2, int children)
		{
			NetworkSaveState[] offspring = new NetworkSaveState[children];

			for(int childIndex = 0; childIndex < children; childIndex++)
			{
				NetworkSaveState child = parent1.deepCopy();

				for(int i = 0; i < parent2.weights.Count; i++)
				{
					if(Utils.chance(Settings.crossoverFactor))
					{
						child.weights[i] = parent2.weights[i];
					}
				}

				for(int i = 0; i < child.weights.Count; i++)
				{
					if(Utils.chance(Settings.mutationRate))
					{
						child.weights[i] += Utils.rangeAroundZero(Settings.mutationMax);
					}
				}

				offspring[childIndex] = child;
			}

			return offspring;
		}


		public NetworkSaveState[] breedNextGenerationNetworks()
		{
			List<NetworkSaveState> nextNetworks = new List<NetworkSaveState>();

			int survivorAmount = System.Math.Max((int) (Settings.leaveBestPercentage * Settings.populationSize), 1);
			int randomAmount = (int) (Settings.newcomerPercentage * Settings.populationSize);

			UnityEngine.Debug.Log(string.Format("Next generation with {0} from the last gen and {1} random ones", survivorAmount, randomAmount));

			for(int i = 0; i < survivorAmount; i++)
			{
				if(nextNetworks.Count < Settings.populationSize)
				{
					nextNetworks.Add(genomes[i].network);
				}
			}

			for(int i = 0; i < randomAmount; i++)
			{
				Network net = new Network(Settings.inputNeurons, Settings.hiddenNeurons, Settings.ouputNeurons);
				nextNetworks.Add(net.saveState());
			}

			int breederIndex = 0;

			while(true)
			{
				for(int i = 0; i < breederIndex; i++)
				{
					NetworkSaveState[] children = breed(genomes[i].network, genomes[breederIndex].network, Settings.childAmount);

					for(int j = 0; j < children.Length; j++)
					{
						if(nextNetworks.Count < Settings.populationSize)
						{
							nextNetworks.Add(children[j]);
						}
						else
						{
							return nextNetworks.ToArray();
						}
					}
				}

				breederIndex = (breederIndex + 1) % (Settings.populationSize);
			}
		}
	}
}