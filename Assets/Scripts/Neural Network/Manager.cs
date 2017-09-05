namespace Neuroevolution.NeuralNetwork
{
	public class Manager
	{
		public Generations generations = new Generations();


		public Network[] nextGenerationNetworks()
		{
			NetworkSaveState[] networks;

			if(generations.initialised)
			{
				networks = generations.nextGenerationNetworks();
			}
			else
			{
				networks = generations.firstGenerationNetworks();
			}

			Network[] neuralNets = new Network[networks.Length];

			for(int i = 0; i < networks.Length; i++)
			{
				Network net = new Network(networks[i]);

				neuralNets[i] = net;
			}

			//TODO remove older generations

			return neuralNets;
		}


		public void reset()
		{
			generations = new Generations();
		}


		public void score(Network net, float fitness)
		{
			generations.addGenome(new Genome(net.saveState(), fitness));
		}
	}
}
