using System;
using WorldSharp;

namespace WorldSharp.Samples
{
	public class WorldSample
	{
		public static int Main (string [] args)
		{
			// Memory allocation is carried out in advanse.
			// This is for compatibility with C language.
			int x_length = GetAudioLength (argv [1]);
			if (x_length <= 0) {
				if (x_length == 0) Console.WriteLine ("error: File not found.");
				else Console.WriteLine ("error: The file is not .wav format.");
				return -1;
			}

			double [] x = new double [sizeof (double) * (x_length)];
			// wavread() must be called after GetAudioLength().
			int fs, nbit;
			wavread (args [0], out fs, out nbit, x);
			DisplayInformation (fs, nbit, x_length);

			//---------------------------------------------------------------------------
			// Analysis part
			//---------------------------------------------------------------------------
			// A new struct is introduced to implement safe program.
			WorldParameters world_parameters = new WorldParameters ();
			// You must set fs and frame_period before analysis/synthesis.
			world_parameters.fs = fs;
			// 5.0 ms is the default value.
			world_parameters.frame_period = 5.0;

			// F0 estimation
			// DIO
			// F0EstimationDio(x, x_length, &world_parameters);

			// Harvest
			F0EstimationHarvest (x, x_length, world_parameters);

			// Spectral envelope estimation
			SpectralEnvelopeEstimation (x, x_length, world_parameters);

			// Aperiodicity estimation by D4C
			AperiodicityEstimation (x, x_length, world_parameters);

			// Note that F0 must not be changed until all parameters are estimated.
			ParameterModification (argc, argv, fs, world_parameters.f0_length,
			  world_parameters.fft_size, world_parameters.f0,
			  world_parameters.spectrogram);

			//---------------------------------------------------------------------------
			// Synthesis part
			// There are three samples in speech synthesis
			// 1: Conventional synthesis
			// 2: Example of real-time synthesis
			// 3: Example of real-time synthesis (Ring buffer is efficiently used)
			//---------------------------------------------------------------------------
			char [] filename  = new char [1000];
			// The length of the output waveform
			int y_length = (int)((world_parameters.f0_length - 1) *
			world_parameters.frame_period / 1000.0 * fs) + 1;
			double [] y = new double [sizeof (double) * (y_length)];

			// Synthesis 1 (conventional synthesis)
			for (int i = 0; i < y_length; ++i) y [i] = 0.0;
			WaveformSynthesis (world_parameters, fs, y_length, y);
			sprintf (filename, "01%s", argv [1]);
			wavwrite (y, y_length, fs, 16, filename);

			// Synthesis 2 (All frames are added at the same time)
			for (int i = 0; i < y_length; ++i) y [i] = 0.0;
			WaveformSynthesis2 (world_parameters, fs, y_length, y);
			sprintf (filename, "02%s", argv [1]);
			wavwrite (y, y_length, fs, 16, filename);

			// Synthesis 3 (Ring buffer is efficiently used.)
			for (int i = 0; i < y_length; ++i) y [i] = 0.0;
			WaveformSynthesis3 (world_parameters, fs, y_length, y);
			sprintf (filename, "03%s", argv [1]);
			wavwrite (y, y_length, fs, 16, filename);

			printf ("complete.\n");
			return 0;
		}
	}
}
