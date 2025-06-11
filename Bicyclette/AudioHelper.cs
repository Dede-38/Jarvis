using NAudio.Wave;

namespace Bicyclette
{
    public class AudioHelper
    {
        public static bool MicrophoneDisponible()
        {
            int deviceCount = WaveIn.DeviceCount;
            return deviceCount > 0;
        }
    }
}
