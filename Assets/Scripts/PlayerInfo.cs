using UnityEngine;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using System.IO;

public static class PlayerInfo {

	public static string name = "";
	public static List<ThetaBetaRatios> attentionRatios = new List<ThetaBetaRatios>();
	public static List<ThetaBetaRatios> relaxationRatios = new List<ThetaBetaRatios>();
	public static double attentionThreshold = 0;
	public static double relaxationThreshold = 0;
	public static List<Score> scores = new List<Score>();

	public static string GetPath() {
		return Path.Combine(Application.persistentDataPath, name+".xml");
	}

	public static double CalculateThreshold(List<ThetaBetaRatios> ratios) {
		// Averaging all the medians.
		double median_sum = 0;
		int iterations = 0;
		foreach (ThetaBetaRatios ratio in ratios) {
			median_sum += ratio.median;
			iterations++;
		}
		return median_sum / iterations;
	}

	public static void ResetTraining() {
		attentionRatios = new List<ThetaBetaRatios> ();
		relaxationRatios = new List<ThetaBetaRatios> ();
		attentionThreshold = 0;
		relaxationThreshold = 0;
	}

	public static void Save() {
		XmlSerializer serializer = new XmlSerializer (typeof(PlayerData));
		Debug.Log("Saving PlayerInfo: " + GetPath ());
		using (FileStream stream = new FileStream(GetPath(), FileMode.Create)) {
			serializer.Serialize(stream, new PlayerData());
		}
	}

	public static void Load() {
		if (File.Exists(GetPath())) {
			XmlSerializer serializer = new XmlSerializer (typeof(PlayerData));
			Debug.Log("Loading PlayerInfo: " + GetPath ());
			using (FileStream stream = new FileStream(GetPath(), FileMode.Open)) {
				PlayerData data = (PlayerData) serializer.Deserialize(stream);
				data.LoadData ();
			}
		}
	}
}
[XmlRoot("PlayerInfo")]
public class PlayerData {
	public string name = "";
	public List<ThetaBetaRatios> attentionRatios = new List<ThetaBetaRatios>();
	public List<ThetaBetaRatios> relaxationRatios = new List<ThetaBetaRatios>();
	public double attentionThreshold = 0;
	public double relaxationThreshold = 0;
	public List<Score> scores = new List<Score>();

	public PlayerData() {
		name = PlayerInfo.name;
		attentionRatios = PlayerInfo.attentionRatios;
		relaxationRatios = PlayerInfo.relaxationRatios;
		attentionThreshold = PlayerInfo.attentionThreshold;
		relaxationThreshold = PlayerInfo.relaxationThreshold;
		scores = PlayerInfo.scores;
	}

	public void LoadData() {
		PlayerInfo.name = name;
		PlayerInfo.attentionRatios = attentionRatios;
		PlayerInfo.relaxationRatios = relaxationRatios;
		PlayerInfo.attentionThreshold = attentionThreshold;
		PlayerInfo.relaxationThreshold = relaxationThreshold;
		PlayerInfo.scores = scores;
	}
}

public class ThetaBetaRatios {
	public string name; // Relaxation or Attention
	public System.DateTime datetime;
	public List<double> values;
	public List<double> averages;
	public double median;
	public double mean;

	public ThetaBetaRatios() {
		this.name = "";
		datetime = System.DateTime.Now;
		values = new List<double>();
		averages = new List<double>();
		median = 0.0;
		mean = 0.0;
	}

	public ThetaBetaRatios(string name) {
		this.name = name;
		datetime = System.DateTime.Now;
		values = new List<double>();
		averages = new List<double>();
		median = 0.0;
		mean = 0.0;
	}

	public void CalculateAverages() {
		List<double> averages = new List<double> ();
		int iterations = 0;
		double average_sum = 0;
		foreach (double value in values) {
			iterations++;
			average_sum += value;
			if (iterations % 4 == 0) {
				averages.Add(average_sum / 4);
				average_sum = 0;
			}
			// We omit the rest of the data if there is not enough data for groups of 4.
		}
		this.averages = averages;
	}

	public void CalculateMedian() {
		if (this.averages.Count < 1) CalculateAverages();
		List<double> averages = new List<double> (this.averages);
		averages.Sort ();
		int size = averages.Count;
		if (size % 2 == 1) {
			median = averages[size/2];
		} else {
			median = (averages[size/2] + averages[size/2+1]) / 2.0;
		}
	}

	public void CalculateMean() {
		double mean_sum = 0;
		int iterations = 0;
		if (this.averages.Count < 1) CalculateAverages();
		foreach (double value in averages) { // Take only averages not all values
			mean_sum += value;
			iterations++;
		}
		mean = mean_sum / iterations;
	}
}

public class Score {
	public float score;
	public float attentiveTime;
	public float semiAttentiveTime;
	public float semiRelaxedTime;
	public float relaxedTime;
	public float totalTimeElapsed;
	public System.DateTime datetime;

	public Score() {
		score = 0;
		attentiveTime = 0;
		semiAttentiveTime = 0;
		semiRelaxedTime = 0;
		relaxedTime = 0;
		totalTimeElapsed = 0;
		datetime = System.DateTime.Now;
	}
}