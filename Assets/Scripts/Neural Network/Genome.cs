namespace Neuroevolution.NeuralNetwork
{
	public class Genome
	{
		public NetworkSaveState network;
		public float fitness;


		public Genome(NetworkSaveState network, float fitness)
		{
			this.network = network;
			this.fitness = fitness;
		}
	}
}