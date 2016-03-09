﻿using System.Threading;
using System.Threading.Tasks;
using SharpCaster.Controllers;
using SharpCaster.Models;

namespace SharpCaster
{
    public class ChromecastService
    {
        private static ChromecastService _instance;

        public static ChromecastService Current
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new ChromecastService();
                }
                return _instance;
            }
        }

        public DeviceLocator DeviceLocator { get; }
        public ChromeCastClient ChromeCastClient { get; }
        public Chromecast ConnectedChromecast { get; set; }
        public CastButton CastButton { get; set; }
        private CancellationTokenSource _cancellationTokenSource;

        public ChromecastService()
        {
            DeviceLocator = new DeviceLocator();
            DeviceLocator.DeviceFounded += DeviceLocator_DeviceFounded;
            ChromeCastClient = new ChromeCastClient();
            ChromeCastClient.Connected += ChromeCastClient_Connected;

        }

        private void DeviceLocator_DeviceFounded(object sender, Chromecast e)
        {
            CastButton?.GoToState(CastButtonVisualStates.InteractiveStates.Disconnected);
        }

        public void ConnectToChromecast(Chromecast chromecast)
        {
            CastButton?.GoToState(CastButtonVisualStates.InteractiveStates.Connecting);
            StopLocatingDevices();
            ConnectedChromecast = chromecast;
            ChromeCastClient.ConnectChromecast(chromecast.DeviceUri);
        }

        private void ChromeCastClient_Connected(object sender, System.EventArgs e)
        {
            CastButton?.GoToState(CastButtonVisualStates.InteractiveStates.Connected);
        }

        public void StopLocatingDevices()
        {
            _cancellationTokenSource.Cancel();
        }

        public async Task StartLocatingDevices()
        {
            _cancellationTokenSource = new CancellationTokenSource();
            await DeviceLocator.LocateDevicesAsync(_cancellationTokenSource.Token);
        }
    }
}
