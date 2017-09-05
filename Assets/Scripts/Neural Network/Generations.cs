using System.Collections.Generic;


namespace Neuroevolution.NeuralNetwork
{
	public class Generations
	{
		public List<Generation> generations = new List<Generation>();


		public Generation lastGeneration {
			get
			{
				return generations[generations.Count - 1];
			}
		}


		public bool initialised {
			get
			{
				return generations.Count > 0;
			}
		}



		public NetworkSaveState[] firstGenerationNetworks()
		{
			NetworkSaveState[] gen = new NetworkSaveState[Settings.populationSize];

			for(int i = 0; i < Settings.populationSize; i++)
			{
				Network neuralNet = new Network(Settings.inputNeurons, Settings.hiddenNeurons, Settings.ouputNeurons);

				gen[i] = neuralNet.saveState();
			}

			generations.Add(new Generation());
			return gen;
		}


		public NetworkSaveState[] nextGenerationNetworks()
		{
			if(!initialised)
			{
				return null;
			}

			NetworkSaveState[] gen = lastGeneration.breedNextGenerationNetworks();

			generations.Add(new Generation());
			return gen;
		}


		public bool addGenome(Genome genome)
		{
			if(!initialised)
			{
				return false;
			}

			//TODO maybe some more checks
			lastGeneration.addGenome(genome);
			return true;
		}
	}
}
