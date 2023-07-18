﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using nfs.nets.layered;
using System.Linq;
using UnityEngine.UI;
using nfs.tools;


namespace nfs {

	public class GetReady : MonoBehaviour {

		[SerializeField] private Visualiser visualiser;
		[SerializeField] private Transform networkListView;
		[SerializeField] private GameObject networkItemPrefab;
		[SerializeField] private GameObject hiddenLayerItemPrefab;
		[SerializeField] private Transform hiddenLayerItemsListView;
		private int minNbHiddenLayer = 1;
		private int maxNbHiddenLayer = 7;

		[SerializeField] private int minHiddenLayerSize = 2;
		[SerializeField] private int maxHiddenLayerSize = 7;
		
		private int numberOfIntput = 3;
		private int numberOfOutput = 2;

		private GameObject[] networkItems;
		private List<GameObject> hiddenLayerItems = new List<GameObject>();

		private SrLife loadedData;		

		// Use this for initialization
		void Start () {
			LoadNetworksAndSetPanel();
			ChangeNumberOfHiddenLayerItem(1);
		}
		
		// Update is called once per frame
		void Update () {
			
		}

		private void LoadNetworksAndSetPanel() {
			//loadedData = Serializer.LoadAllSpecies(GetComponentInParent<MainScreen>().CurrentSim);
			loadedData = Serializer.LoadAllSpecies(MainScreen.CurrentSim);

			networkItems = new GameObject[loadedData.Species.Length];

			for (int i = 0; i < loadedData.Species.Length; i++) {
				GameObject newNetItem = Instantiate(networkItemPrefab);
				newNetItem.transform.SetParent(networkListView);
				newNetItem.transform.localScale = new Vector3(1,1,1);
				newNetItem.transform.localPosition = new Vector3(0,0,0);
				SrSpecies sp = loadedData.Species[i];
				newNetItem.GetComponent<NetListItem>().SetFields(sp.Name, sp.Lineage[0].Id, sp.Lineage[0].FitnessScore.ToString("F2"), i);
				newNetItem.GetComponent<NetListItem>().Selected += OnNetworkSelected;
				networkItems[i] = newNetItem;
			}
			
		}

		private void OnNetworkSelected (int index) {

			NeuralNetwork selectedNetwork = Serializer.DeserializeNetwork(loadedData.Species[index].Lineage[0]);
			Serializer.PreLoadedNetwork = selectedNetwork;
			selectedNetwork.DummyPingFwd();
			visualiser.SetFocusNetwork(selectedNetwork);

			for(int i = 0; i < networkItems.Length; i++) {
				if(i != index)
					networkItems[i].GetComponent<Image>().enabled = false;;
			}

		}

		public void RandomizeSynapses() {

		}

		public void CreateRandomNetwork() {

			ChangeNumberOfHiddenLayerItem(Random.Range(minNbHiddenLayer, maxNbHiddenLayer));

			foreach(GameObject hiddenLayerItem in hiddenLayerItems){
				hiddenLayerItem.GetComponent<InputField>().text = Random.Range(HiddenLayerListItem.MinLayerSize, HiddenLayerListItem.MaxLayerSize).ToString();
			}

			// int[] layerSizes = new int[2 + UnityEngine.Random.Range(1,maxNbHiddenLayer)]; // we need a minimum of one layer to avoid bug with ping forward
			// layerSizes[0] = 3;
			// layerSizes[layerSizes.Length-1] = 2;

			// Debug.Log("layer number: " + layerSizes.Length);

			// for(int i = 1; i < layerSizes.Length - 1; i++) {
			// 	layerSizes[i] = Random.Range(layerSizes[layerSizes.Length-1], maxHiddenLayerSize);
			// 	Debug.Log(layerSizes[i]);
			// }

			// //String.Join(",", layerSizes.Select(p=>p.ToString()).ToArray())
			// Debug.Log("layer sizes: " + string.Join(", ", layerSizes.Select(x=>x.ToString()).ToArray()));

			// NeuralNetwork randomNetwork = new NeuralNetwork(layerSizes);
			// Serializer.PreLoadedNetwork = randomNetwork;
			// randomNetwork.DummyPingFwd();
			// visualiser.SetFocusNetwork(randomNetwork);
		}

		private void UpdateNtworkDesign() {

			int numberOfLayer = hiddenLayerItems.Count + 2;
			int[] layerSizes = new int[numberOfLayer];
			layerSizes[0] = numberOfIntput;
			layerSizes[layerSizes.Length - 1] = numberOfOutput;


			for(int i = 1; i < numberOfLayer-1; i++) {
				layerSizes[i] = hiddenLayerItems[i-1].GetComponent<HiddenLayerListItem>().LayerSize;
			}
			
			NeuralNetwork newNet = new NeuralNetwork(layerSizes);

			if(visualiser.Focus != null) {
				Matrix[] synapseExtract = visualiser.Focus.GetSynapsesClone();
				newNet.InsertSynapses(synapseExtract, true);
			}

			newNet.DummyPingFwd();

			visualiser.SetFocusNetwork(newNet);
		}

		public void OnNumberOfLayerChanged(InputField input) {
			if(input.text == "") {
				return;
			}

			int number = System.Convert.ToInt32(input.text);
			ChangeNumberOfHiddenLayerItem(number);
		}

		private void ChangeNumberOfHiddenLayerItem(int number) {

			if(number < minNbHiddenLayer) {
				Debug.LogWarning("You can't have less than 1 hidden layer.");
				return;
			}
			else if(number > maxNbHiddenLayer) {
				Debug.LogWarning("You can't have less than 1 hidden layer.");
				return;
			}

			int currentNumber = hiddenLayerItems.Count;

			if(currentNumber < number)  {
				for(int i = currentNumber; i < number; i++) {
					GameObject newItem = Instantiate(hiddenLayerItemPrefab);
					newItem.transform.SetParent(hiddenLayerItemsListView);
					newItem.transform.localScale = new Vector3(1, 1, 1);
					Vector3 pos = newItem.transform.localPosition;
					pos.z = 0;
					//pos.x -= 17;
					newItem.transform.localPosition = pos;
					newItem.GetComponent<HiddenLayerListItem>().HiddenLayerUpdated += UpdateNtworkDesign;
					hiddenLayerItems.Add(newItem);
				}
			}
			else if(currentNumber > number) {
				for(int i = number; i < currentNumber; i++) {
					GameObject itemToRemove = hiddenLayerItems.Last();
					hiddenLayerItems.Remove(itemToRemove);
					Destroy(itemToRemove);
				}
			}

			UpdateNtworkDesign();

		}

	}

}
