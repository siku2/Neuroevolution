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
					Genome[] children = breed(genomes[i], genomes[breederIndex], Settings.childAmount);

					for(int j = 0; j < children.Length; j++)
					{
						if(nextNetworks.Count < Settings.populationSize)
						{
							nextNetworks.Add(children[j].network);
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