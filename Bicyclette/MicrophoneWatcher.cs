using NAudio.CoreAudioApi;
using NAudio.CoreAudioApi.Interfaces;
using System.Windows;

namespace Bicyclette
{
    public class MicrophoneWatcher
    {
        private readonly MMDeviceEnumerator _deviceEnumerator;

        public MicrophoneWatcher()
        {
            _deviceEnumerator = new MMDeviceEnumerator();
            _deviceEnumerator.RegisterEndpointNotificationCallback(new DeviceNotificationClient());
        }

        private class DeviceNotificationClient : IMMNotificationClient
        {
            public void OnDeviceStateChanged(string deviceId, DeviceState newState)
            {
                if (newState == DeviceState.NotPresent || newState == DeviceState.Unplugged || newState == DeviceState.Disabled)
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        MessageBox.Show("Le micro système a été désactivé. Le micro de l'application a été désactivé automatiquement.", "Info");
                        // Ici : désactiver ton micro applicatif (fermer l'enregistrement, etc.)
                    });
                }
            }

            public void OnDeviceAdded(string pwstrDeviceId) { }
            public void OnDeviceRemoved(string deviceId) { }
            public void OnDefaultDeviceChanged(DataFlow flow, Role role, string defaultDeviceId) { }
            public void OnPropertyValueChanged(string pwstrDeviceId, PropertyKey key) { }
        }
    }
}
