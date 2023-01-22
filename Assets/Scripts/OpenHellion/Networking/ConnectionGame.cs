// ConnectionGame.cs
//
// Copyright (C) 2023, OpenHellion contributors
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <https://www.gnu.org/licenses/>.

using System;
using System.IO;
using OpenHellion.ProviderSystem;
using ZeroGravity;
using ZeroGravity.Network;

namespace OpenHellion.Networking
{
	/// <summary>
	/// 	Handles connecting to a game server.
	/// </summary>
	internal sealed class ConnectionGame
	{
		private string _serverIp;

		private int _serverPort;

		private string _serverId;

		private string _serverPassword;

		private Telepathy.Client _client;

		/// <summary>
		/// 	Establish a connection to a specified game server.
		/// </summary>
		internal void Connect(string ip, int port, string serverId, string password)
		{
			_serverIp = ip;
			_serverPort = port;
			_serverId = serverId;
			_serverPassword = password;

			_client = new(2000)
			{
				OnConnected = OnConnected,
				OnData = OnData,
				OnDisconnected = OnDisconnected
			};

			_client.Connect(ip, port);
		}

		internal int Tick()
		{
			return _client.Tick(50);
		}

		/// <summary>
		/// 	Send network data to the server.
		/// </summary>
		internal void Send(NetworkData data)
		{
			if (!_client.Connected)
			{
				EventSystem.Invoke(new EventSystem.InternalEventData(EventSystem.InternalEventType.OpenMainScreen));
				Dbg.Error("Tried to send data when not connected to any server.");
				return;
			}

			try
			{
				// Package data.
				ArraySegment<byte> binary = new(Serializer.Package(data));

				// Send data to server.
				_client.Send(binary);
				NetworkController.LogSentNetworkData(data.GetType());
			}
			catch (Exception ex)
			{
				Dbg.Error("Error when sending data", ex.Message, ex.StackTrace);
			}
		}

		/// <summary>
		/// 	Disconnect from server.
		/// </summary>
		internal void Disconnect()
		{
			_client.Disconnect();
		}

		// Executed when we connect to a server.
		private void OnConnected()
		{
			Dbg.Log("Client connected to server.");

			LogInRequest logInRequest = new LogInRequest
			{
				PlayerId = NetworkController.PlayerId,
				NativeId = ProviderManager.MainProvider.GetNativeId(),
				CharacterData = NetworkController.CharacterData,
				ServerID = _serverId,
				Password = _serverPassword,
				ClientHash = Client.CombinedHash
			};

			Send(logInRequest);
		}


		// Handles a network package.
		private void OnData(ArraySegment<byte> message)
		{
			try
			{
				NetworkData networkData = Serializer.Deserialize(new MemoryStream(message.Array));
				if (networkData != null && NetworkController.Instance != null)
				{
					EventSystem.Invoke(networkData);
					NetworkController.LogReceivedNetworkData(networkData.GetType());
				}
			}
			catch (Exception ex)
			{
				if (!Client.Instance.LogInResponseReceived)
				{
					EventSystem.Invoke(new EventSystem.InternalEventData(EventSystem.InternalEventType.ShowMessageBox, Localization.ConnectionError, Localization.TryAgainLater));
					Dbg.Error(ex.Message, ex.StackTrace);
				}
				else if (Client.Instance.LogoutRequestSent)
				{
					Dbg.Info("Tried to listen to data, but logout was requested.");
				}
				else
				{
					Dbg.Error("Game server listening thread exception", ex.Message, ex.StackTrace);
				}
			}
		}

		// Executed when we disconnect from a server.
		private void OnDisconnected()
		{
			if (Client.Instance.LogoutRequestSent)
			{
				EventSystem.Invoke(new EventSystem.InternalEventData(EventSystem.InternalEventType.OpenMainScreen));
			}

			Dbg.Log("Client disconnected from server.");
		}
	}
}