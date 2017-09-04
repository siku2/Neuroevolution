using System.Collections;
using System.Collections.Generic;


namespace Neuroevolution.NeuralNetwork
{
	public class Neuron
	{
		public float value;
		public float[] weights;


		public Neuron(int inputAmount)
		{
			value = 0;

			weights = new float[inputAmount];

			for(int i = 0; i < inputAmount; i++)
			{
				weights[i] = Settings.randomClamped();
			}
		}
	}


	public class Layer
	{
		public int index;
		public Neuron[] neurons;


		public Layer(int index, int neuronAmount, int inputAmount)
		{
			this.index = index;
			neurons = new Neuron[neuronAmount];

			for(int i = 0; i < neuronAmount; i++)
			{
				Neuron newNeuron = new Neuron(inputAmount);

				neurons[i] = newNeuron;
			}
		}
	}

	public class NetworkSaveState
	{
		public List<int> neuronsPerLayer = new List<int>();
		public List<float> weights = new List<float>();
	}


	public class Network
	{
		Layer[] layers;


		public Network(int inputLayerNeurons, int[] hiddenLayers, int outputLayerNeurons)
		{
			int index = 0;
			int prevNeurons = 0;
			layers = new Layer[inputLayerNeurons + hiddenLayers.Length + outputLayerNeurons];

			Layer inputLayer = new Layer(index, inputLayerNeurons, prevNeurons);
			layers[index] = inputLayer;

			prevNeurons = inputLayerNeurons;
			index++;

			for(int i = 0; i < hiddenLayers.Length; i++)
			{
				Layer layer = new Layer(index, hiddenLayers[i], prevNeurons);
				layers[index] = layer;

				prevNeurons = inputLayerNeurons;
				index++;
			}
				
			Layer outputLayer = new Layer(index, outputLayerNeurons, prevNeurons);
			layers[index] = outputLayer;
		}


		public NetworkSaveState saveState()
		{
			NetworkSaveState save = new NetworkSaveState();

			for(int i = 0; i < layers.Length; i++)
			{
				save.neuronsPerLayer.Add(layers[i].neurons.Length);

				for(int j = 0; j < layers[i].neurons.Length; j++)
				{
					for(int k = 0; k < layers[i].neurons[j].weights.Length; k++)
					{
						save.weights.Add(layers[i].neurons[j].weights[k]);
					}
				}
			}

			return save;
		}


		public void loadState(NetworkSaveState save)
		{
			int prevNeurons = 0;
			int index = 0;
			int indexWeights = 0;

			layers = new Layer[save.neuronsPerLayer.Count];

			for(int i = 0; i < save.neuronsPerLayer.Count; i++)
			{
				Layer layer = new Layer(index, save.neuronsPerLayer[i], prevNeurons);

				for(int j = 0; j < layer.neurons.Length; j++)
				{
					for(int k = 0; k < layer.neurons[j].weights.Length; k++)
					{
						layer.neurons[j].weights[k] = save.weights[indexWeights];

						indexWeights++;
					}
				}

				prevNeurons = save.neuronsPerLayer[i];
				index++;

				layers[i] = layer;
			}
		}


		public float[] compute(float[] inputValues)
		{
			for(int i = 0; i < inputValues.Length; i++)
			{
				if(layers[0] != null && layers[0].neurons[i] != null)
				{
					layers[0].neurons[i].value = inputValues[i];
				}
			}

			Layer prevLayer = layers[0];

			for(int i = 1; i < layers.Length; i++)
			{
				for(int j = 0; j < layers[i].neurons.Length; j++)
				{
					float total = 0;

					for(int k = 0; k < prevLayer.neurons.Length; k++)
					{
						total += prevLayer.neurons[k].value * layers[i].neurons[j].weights[k];
					}

					layers[i].neurons[j].value = Settings.activation(total);
				}

				prevLayer = layers[i];
			}

			Layer lastLayer = layers[layers.Length - 1];

			float[] final = new float[lastLayer.neurons.Length];

			for(int i = 0; i < lastLayer.neurons.Length; i++)
			{
				final[i] = lastLayer.neurons[i].value;
			}

			return final;
		}
	}
}