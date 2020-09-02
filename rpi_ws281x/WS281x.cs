using rpi_ws281x.Native;
using System;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;

namespace rpi_ws281x
{
	/// <summary>
	/// Wrapper class to controll WS281x LEDs
	/// </summary>
	public class WS281x : IDisposable
	{
		private ws2811_t _ws2811;
		private GCHandle _ws2811Handle;
		private bool _isDisposingAllowed;

		/// <summary>
		/// Initialize the wrapper
		/// </summary>
		/// <param name="settings">Settings used for initialization</param>
		public WS281x(Settings settings)
		{
			_ws2811 = new ws2811_t();
			//Pin the object in memory. Otherwies GC will probably move the object to another memory location.
			//This would cause errors because the native library has a pointer on the memory location of the object.
			
			_ws2811Handle = GCHandle.Alloc(_ws2811, GCHandleType.Pinned);
			_ws2811.dmanum	= settings.DMAChannel;
			_ws2811.freq	= settings.Frequency;
			_ws2811.channel_1 = default(ws2811_channel_t);
			_ws2811.channel_2 = default(ws2811_channel_t);
			if (settings.Channel_1 != null)
				InitChannel(ref _ws2811.channel_1, settings.Channel_1);
			if (settings.Channel_2 != null)
				InitChannel(ref _ws2811.channel_2, settings.Channel_2);

			Settings = settings;
			var initResult = PInvoke.ws2811_init(_ws2811Handle.AddrOfPinnedObject());

			if (initResult != ws2811_return_t.WS2811_SUCCESS)
			{
				var returnMessage = GetMessageForStatusCode(initResult);
				throw new Exception($"Error while initializing.{Environment.NewLine}Error code: {initResult.ToString()}{Environment.NewLine}Message: {returnMessage}");
			}	

			//Disposing is only allowed if the init was successfull.
			//Otherwise the native cleanup function throws an error.
			_isDisposingAllowed = true;
		}

		/// <summary>
		/// Renders the content of the channels
		/// </summary>
		public void Render()
		{
			if (Settings.Channel_1 != null)
			{
				var ledColor = Settings.Channel_1.LEDs.Select(x => x.RGBValue).ToArray();
				Marshal.Copy(ledColor, 0, _ws2811.channel_1.leds, ledColor.Count());
			}

			if (Settings.Channel_2 != null)
			{
				var ledColor = Settings.Channel_2.LEDs.Select(x => x.RGBValue).ToArray();
				Marshal.Copy(ledColor, 0, _ws2811.channel_2.leds, ledColor.Count());
			}
			var result = PInvoke.ws2811_render(_ws2811Handle.AddrOfPinnedObject());
			if (result != ws2811_return_t.WS2811_SUCCESS)
			{
				var returnMessage = GetMessageForStatusCode(result);
				throw new Exception($"Error while rendering.{Environment.NewLine}Error code: {result.ToString()}{Environment.NewLine}Message: {returnMessage}");
			}
		}

		private object GetMessageForStatusCode(object result)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Sets the color of a given LED
		/// </summary>
		/// <param name="channelIndex">Channel which controls the LED</param>
		/// <param name="ledID">ID/Index of the LED</param>
		/// <param name="color">New color</param>
		public void SetLEDColor(int channelIndex, int ledID, Color color)
		{
			if (channelIndex == 0)
				Settings.Channel_1.LEDs[ledID].Color = color;
            else if (channelIndex == 1)
                Settings.Channel_2.LEDs[ledID].Color = color;
		}

		/// <summary>
		/// Returns the settings which are used to initialize the component
		/// </summary>
		public Settings Settings { get; private set; }

		/// <summary>
		/// Initialize the channel propierties
		/// </summary>
		/// <param name="channelIndex">Index of the channel tu initialize</param>
		/// <param name="channelSettings">Settings for the channel</param>
		private void InitChannel(ref ws2811_channel_t channel, Channel channelSettings)
		{
			channel.count = channelSettings.LEDCount;
			channel.gpionum = channelSettings.GPIOPin;
			channel.brightness = channelSettings.Brightness;
			channel.invert = Convert.ToInt32(channelSettings.Invert);

			if (channelSettings.StripType != StripType.Unknown)
			{
				//Strip type is set by the native assembly if not explicitly set.
				//This type defines the ordering of the colors e. g. RGB or GRB, ...
				channel.strip_type = (int)channelSettings.StripType;
			}
		}

		/// <summary>
		/// Returns the error message for the given status code
		/// </summary>
		/// <param name="statusCode">Status code to resolve</param>
		/// <returns></returns>
		private string GetMessageForStatusCode(ws2811_return_t statusCode)
		{
			var strPointer = PInvoke.ws2811_get_return_t_str((int)statusCode);
			return Marshal.PtrToStringAuto(strPointer);
		}

	#region IDisposable Support
		private bool disposedValue = false; // To detect redundant calls

		protected virtual void Dispose(bool disposing)
		{
			if (!disposedValue)
			{
				if (disposing)
				{
					// TODO: dispose managed state (managed objects).
				}

				// TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
				// TODO: set large fields to null.

				if(_isDisposingAllowed)
				{
					PInvoke.ws2811_fini(_ws2811Handle.AddrOfPinnedObject());
					if (_ws2811Handle.IsAllocated)
					{
						_ws2811Handle.Free();
					}
					_isDisposingAllowed = false;
				}
				
				disposedValue = true;
			}
		}

		// TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
		~WS281x()
		{
			// Do not change this code. Put cleanup code in Dispose(bool disposing) above.
			Dispose(false);
		}

		// This code added to correctly implement the disposable pattern.
		public void Dispose()
		{
			// Do not change this code. Put cleanup code in Dispose(bool disposing) above.
			Dispose(true);
			GC.SuppressFinalize(this);
			// TODO: uncomment the following line if the finalizer is overridden above.
			// GC.SuppressFinalize(this);
		}
	#endregion
	}
}
