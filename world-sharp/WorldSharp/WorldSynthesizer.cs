using System;
using System.Runtime.InteropServices;
using WorldSharp.Interop;

namespace WorldSharp
{
	public class WorldSynthesizer : IDisposable
	{
		Pointer<Interop.WorldSynthesizer> impl;
		
		public WorldSynthesizer (int fs, double framePeriod, int fftSize, int bufferSize, int numberOfPointers)
		{
			IntPtr handle = IntPtr.Zero;
			unsafe {
				void* ptr = &handle;
				Natives.InitializeSynthesizer (fs, framePeriod, fftSize, bufferSize, numberOfPointers, handle);
				impl = handle;
			}
		}

		public void AddParameters (IntPtr f0, int f0Length, IntPtr spectrogram, IntPtr aperiodicity)
		{
			Natives.AddParameters (f0, f0Length, spectrogram, aperiodicity, impl);
		}

		public void Refresh ()
		{
			Natives.RefreshSynthesizer (impl);
		}

		public void Dispose ()
		{
			Natives.DestroySynthesizer (impl);
		}
	}
}
