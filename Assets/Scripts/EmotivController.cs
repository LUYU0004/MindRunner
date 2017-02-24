using System;
using Emotiv;
using System.Collections.Generic;

public class EmotivController
{
	public bool connected = false;
	public bool debug = false;

	private EmoEngine engine;
	private int userID = -1;
	
	//EEG Signal Processing Helper variables
	public double[] COUNTER, INTERPOLATED, RAW_CQ,
	AF3, F7, F3, FC5, T7,
	P7, O1, O2, P8, T8,
	FC6, F4, F8, AF4, GYROX, GYROY,
	TIMESTAMP, ES_TIMESTAMP,
	FUNC_ID, FUNC_VALUE, MARKER,
	SYNC_SIGNAL;
	
	public EmotivController ()
	{
		try
		{
			connected = false;
			engine = EmoEngine.Instance;
			
			try
			{
				engine.EmoEngineConnected += new EmoEngine.EmoEngineConnectedEventHandler(Engine_Connected);
				engine.UserAdded += new EmoEngine.UserAddedEventHandler(engine_UserAdded_Event);
				engine.Connect();
			}
			catch (Exception ex)
			{
				connected = false;				
			}			
			
			COUNTER = new double[64];
			INTERPOLATED = new double[64];
			RAW_CQ = new double[64];
			AF3 = new double[64];
			F7 = new double[64];
			F3 = new double[64];
			FC5 = new double[64];
			T7 = new double[64];
			P7 = new double[64];
			O1 = new double[64];
			O2 = new double[64];
			P8 = new double[64];
			T8 = new double[64];
			FC6 = new double[64];
			F4 = new double[64];
			F8 = new double[64];
			AF4 = new double[64];
			GYROX = new double[64];
			GYROY = new double[64];
			TIMESTAMP = new double[64];
			ES_TIMESTAMP = new double[64];
			FUNC_ID = new double[64];
			FUNC_VALUE = new double[64];
			MARKER = new double[64];
			SYNC_SIGNAL = new double[64];
			
		}
		catch (Exception ex)
		{
			connected = false;
			//throw ex;
		}
	}
	
	public void Engine_Connected(object sender, EmoEngineEventArgs e)
	{
		connected = true;
	}
	
	public void engine_UserAdded_Event(object sender, EmoEngineEventArgs e)
        {
            // record the user 
            userID = (int)e.userId;

            // enable data aquisition for this user.
            engine.DataAcquisitionEnable((uint)userID, true);
            
            // ask for up to 1 second of buffered data
            engine.EE_DataSetBufferSizeInSec(1); 

        }

	public bool isConnected() {
		return connected || debug;
	}

	public Boolean get32Samples()
	{
		// Handle any waiting events
		engine.ProcessEvents();
		// If the user has not yet connected, do not proceed
		if ((int)userID == -1) {
			return false;
		}
		
		Dictionary<EdkDll.EE_DataChannel_t, double[]> data = engine.GetData((uint)userID);
		
		if (data == null) {
			return false;
		}
		
		int _bufferSize = data[EdkDll.EE_DataChannel_t.TIMESTAMP].Length;
		// Write the data to a file
		for (int i = 0; i < _bufferSize; i++)
		{
			// now write the data
			foreach (EdkDll.EE_DataChannel_t channel in data.Keys)
			{
				if (i > 31)
					break;
				switch ((int)channel)
				{
				case 0:
					COUNTER[i] = data[channel][i];
					break;
					
				case 1:
					INTERPOLATED[i] = data[channel][i];
					break;
					
				case 2:
					RAW_CQ[i] = data[channel][i];
					break;
					
				case 3:
					AF3[i] = data[channel][i];
					break;
					
				case 4:
					F7[i] = data[channel][i];
					break;
					
				case 5:
					F3[i] = data[channel][i];
					break;
					
				case 6:
					FC5[i] = data[channel][i];
					break;
					
				case 7:
					T7[i] = data[channel][i];
					break;
					
				case 8:
					P7[i] = data[channel][i];
					break;
					
				case 9:
					O1[i] = data[channel][i];
					break;
					
				case 10:
					O2[i] = data[channel][i];
					break;
					
				case 11:
					P8[i] = data[channel][i];
					break;
					
				case 12:
					T8[i] = data[channel][i];
					break;
					
				case 13:
					FC6[i] = data[channel][i];
					break;
					
				case 14:
					F4[i] = data[channel][i];
					break;
					
				case 15:
					F8[i] = data[channel][i];
					break;
					
				case 16:
					AF4[i] = data[channel][i];
					break;
					
				case 17:
					GYROX[i] = data[channel][i];
					break;
					
				case 18:
					GYROY[i] = data[channel][i];
					break;
					
				case 19:
					TIMESTAMP[i] = data[channel][i];
					break;
					
				case 20:
					ES_TIMESTAMP[i] = data[channel][i];
					break;
					
				case 21:
					FUNC_ID[i] = data[channel][i];
					break;
					
				case 22:
					FUNC_VALUE[i] = data[channel][i];
					break;
					
				case 23:
					MARKER[i] = data[channel][i];
					break;
					
				case 24:
					SYNC_SIGNAL[i] = data[channel][i];
					break;
				}
			}
		}
		return true;
	}
	
	public void TimeToFrq32()
	{
		double[] tempData;
		tempData = new double[64];
		for (int ii = 0; ii < 32; ii++)
		{
			tempData[2 * ii] = AF3[ii];
			tempData[2 * ii + 1] = 0.0;
		}
		FFTCompute(tempData);
		ConjugateCompute(tempData, AF3);
		
		for (int ii = 0; ii < 32; ii++)
		{
			tempData[2 * ii] = F3[ii];
			tempData[2 * ii + 1] = 0.0;
		}
		FFTCompute(tempData);
		ConjugateCompute(tempData, F3);
		
		for (int ii = 0; ii < 32; ii++)
		{
			tempData[2 * ii] = F4[ii];
			tempData[2 * ii + 1] = 0.0;
		}
		FFTCompute(tempData);
		ConjugateCompute(tempData, F4);
		
		for (int ii = 0; ii < 32; ii++)
		{
			tempData[2 * ii] = AF4[ii];
			tempData[2 * ii + 1] = 0.0;
		}
		FFTCompute(tempData);
		ConjugateCompute(tempData, AF4);
	}
	
	public void FFTCompute(double[] inputData)
	{
		/* temp vector, used many times, */
		double[] computeTemp;
		computeTemp = new double[64];
		
		/* constants for FFT algorithm */
		double[] fftConstant;
		fftConstant = new double[34]{  
			1.0,      0.0, 0.980785, 0.19509,
			0.92388,  0.382683,  0.83147,  0.55557,
			0.707107, 0.707107,  0.55557,  0.83147,
			0.382683, 0.92388,   0.19509,  0.980785,
			0.0,      1.0,      -0.19509,  0.980785,
			-0.382683, 0.92388,  -0.55557,  0.83147,
			-0.707107, 0.707107, -0.83147,  0.55557,
			-0.92388,  0.382683, -0.980785, 0.19509,
			-1.0,      0.0};
		
		
		double Tre, Tim;
		int ii, jj, preJ, l, target;
		
		target = 16;
		l = 1;
		while (true)
		{
			preJ = 0;
			jj = l;
			ii = 0;
			while (true)
			{
				while (true)
				{
					/* computeTemp[ii+preJ] = inputData[ii] + inputData[m+ii]; complex */
					computeTemp[2 * (ii + preJ)] = inputData[2 * ii] + inputData[2 * (target + ii)];
					computeTemp[2 * (ii + preJ) + 1] = inputData[2 * ii + 1] + inputData[2 * (target + ii) + 1];
					
					/* computeTemp[ii+jj] = fftConstant[preJ] * (inputData[ii] - inputData[m+ii]); complex */
					Tre = inputData[2 * ii] - inputData[2 * (target + ii)];
					Tim = inputData[2 * ii + 1] - inputData[2 * (target + ii) + 1];
					computeTemp[2 * (ii + jj)] = fftConstant[2 * preJ] * Tre - fftConstant[2 * preJ + 1] * Tim;
					computeTemp[2 * (ii + jj) + 1] = fftConstant[2 * preJ] * Tim + fftConstant[2 * preJ + 1] * Tre;
					ii++;
					if (ii >= jj) break;
				}
				
				preJ = jj;
				jj = preJ + l;
				if (jj > target) break;
				
			}
			
			l = l + l;
			
			if (l > target)
			{
				for (ii = 0; ii < 4 * target; ii++)
					inputData[ii] = computeTemp[ii]; // odd power finish
				
				return;
			}
			
			/* work back other way without copying */
			preJ = 0;
			jj = l;
			ii = 0;
			
			while (true)
			{
				while (true)
				{
					/* inputData[ii+preJ] = computeTemp[ii] + computeTemp[m+ii]; complex */
					inputData[2 * (ii + preJ)] = computeTemp[2 * ii] + computeTemp[2 * (target + ii)];
					inputData[2 * (ii + preJ) + 1] = computeTemp[2 * ii + 1] + computeTemp[2 * (target + ii) + 1];
					
					/* inputData[ii+jj] = fftConstant[preJ] * (computeTemp[ii] - computeTemp[m+ii]); complex */
					Tre = computeTemp[2 * ii] - computeTemp[2 * (target + ii)];
					Tim = computeTemp[2 * ii + 1] - computeTemp[2 * (target + ii) + 1];
					inputData[2 * (ii + jj)] = fftConstant[2 * preJ] * Tre - fftConstant[2 * preJ + 1] * Tim;
					inputData[2 * (ii + jj) + 1] = fftConstant[2 * preJ] * Tim + fftConstant[2 * preJ + 1] * Tre;
					ii++;
					if (ii >= jj) break;
				}
				
				preJ = jj;
				jj = preJ + l;
				if (jj > target) break;
			}
			
			l = l + l;
			if (l > target) break; // result is in inputData
		}
	}
	
	public void ConjugateCompute(double[] inputData, double[] outputData)
	{
		for (int ii = 0; ii < 32; ii++)
		{
			outputData[ii] = inputData[ii * 2] * inputData[ii * 2] + inputData[ii * 2 + 1] * inputData[ii * 2 + 1];
		}
	}
	
	public double GetAttention()
	{
		get32Samples();
		TimeToFrq32 ();
		// int Fs = 128;
		// int N = 64;
		// theta : 4 ~ 8 Hz                    [2]~[4]                                 
		// alpha : 8 ~ 13Hz = num * Fs/N       [4]~[6]     
		// beta  : 13 ~ 20Hz                   [7]~[10]     
		// gama  : above 30 Hz                   [15+]
		double alpha, beta, theta;
		theta = F3[2] + F3[3] + F3[4] +
			F4[2] + F4[3] + F4[4] +
				AF4[2] + AF4[3] + AF4[4] +
				AF3[2] + AF3[3] + AF3[4];
		
		theta = theta / 4;
		
		//		alpha = O1[4] + O1[5] + O1[6] +
		//			O2[4] + O2[5] + O2[6] +
		//				AF3[4] + AF3[5] + AF3[6] +
		//				AF4[4] + AF4[5] + AF4[6];
		//		
		//		alpha = alpha / 4;
		
		beta = F3[7] + F3[8] + F3[9] + F3[10] +
			F4[7] + F4[8] + F4[9] + F4[10] +
				AF3[7] + AF3[8] + AF3[9] + AF3[10] +
				AF4[7] + AF4[8] + AF4[9] + AF4[10];
		
		beta = beta / 4;
		
		// Attention in dB
		double attention = 20 * Math.Log(theta / beta);
		return attention;
	}

	public double GetDummyValues(string type) {
		double[] attention = {26.09302541, -1.490327939, 25.17709992, 19.65112146, 16.53526721, 31.64517042, 12.59438133, 22.82437996, 28.38555452, -5.659316642, 37.62013855, 38.60899831, -3.983207018, 27.78377246, 33.3479441, 19.57826111, 27.19807665, 43.37901733, 54.12530927, 22.45602255, 22.47266276, 8.784611469, 15.40233882, -5.716990712, 18.10276115, 20.25600535, 26.20796309, 15.42327786, 24.87229573, 26.88882735, -1.001539619, 34.64724716, 20.63508676, 3.051166295, -6.046508024, 27.12676284, 14.36380109, 32.81352042, 18.50878416, 42.97511506, 37.39048704, 31.18063954, 12.63574199, 11.13589594, 17.23374757, -4.742302383, 20.21588935, 29.93220005, 4.457862697, 20.8716937, 37.43421558, -7.002750555, 5.599462515, 27.8692147, -0.832802799, 12.07978887, 25.26375293, 25.95519332, 8.828554386, 7.886554987, 40.12448416, 22.10049922};
		double[] relaxation = {42.68779315, 9.648395961, 39.57584153, 42.95742758, 51.85272711, 30.48313566, 33.46956758, 23.07005764, -1.75472411, 21.53441433, 51.29266751, 10.93677668, 56.61565523, 33.4120937, 50.0670143, 44.95732894, 39.91448814, 12.79344271, 42.8843357, 23.84753575, -3.282147115, 28.5727123, 22.02637573, 9.351487056, 28.56047755, 32.13432884, 64.53635825, 13.72126879, 42.72911522, 31.13444311, 8.81165464, 19.75321023, 33.51512495, -7.685967462, 8.315501102, 37.1948248, 34.28648552, 21.66774297, 41.61962743, 30.52738223, 24.07475934, 19.84333375, -8.724620288, 11.98853864, 33.85741774, 19.60753868, -4.838372296, 36.79729283, 22.28883948, 51.02131729, 41.07268638, 39.43692352, 35.27112338, 10.22192859, 38.14944046, 46.45290167, 57.36356765, 28.452323, 36.37667173, 22.06001047};
		return (type == "a") ? attention[new Random().Next(0,attention.Length)] : relaxation[new Random().Next(0,relaxation.Length)] ;
	}

	public double GetAttention(string type) {
		if (debug)
			return GetDummyValues (type);
		else
			return GetAttention ();
	}
	/*
	public int GetAttentionStrength()
	{
		if (BTconcentrationLevel < PlayerInfo.AttentionThreshold)
		{
			return 5;
		}
		else if (BTconcentrationLevel < PlayerInfo.RelaxationThreshold && BTconcentrationLevel > PlayerInfo.AttentionThreshold)
		{
			return 2;
		}
		else
		{
			return 1;
		}
	}
	*/
}

