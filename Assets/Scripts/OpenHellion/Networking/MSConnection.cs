// MSConnection.cs
//
// Copyright (C) 2023, OpenHellion contributors
//
// SPDX-License-Identifier: GPL-3.0-or-later
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
using System.Threading.Tasks;
using OpenHellion.Networking.Message.MainServer;
using OpenHellion.IO;
using System.Net.Http;
using System.Text;

namespace OpenHellion.Networking
{
	// TODO: Use Nakama instead.
	/// <summary>
	/// 	Handles connecting to the main server.
	/// </summary>
	[Obsolete]
	public class MSConnection
	{
		public const string IpAddress = "localhost";
		public const ushort Port = 6001;

		public static string Address {
			get {
				return IpAddress + ":" + Port;
			}
		}

		/// <summary>
		/// 	Send a request to get data from the main server.
		/// </summary>
		public static void Get<T>(string destination, Action<T> callback)
		{
			Task.Run(async () =>
			{
				try {
					// Create new client and request and load it with data.
					HttpClient httpClient = new HttpClient();
					HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, new UriBuilder("http", IpAddress, Port, "/api/" + destination).Uri);

					// Send message and get result.
					HttpResponseMessage result = await httpClient.SendAsync(request);

					// Read data as string.
					string str = await result.Content.ReadAsStringAsync();

					// Make object out of data.
					callback(JsonSerialiser.Deserialize<T>(str));

					// Clean up.
					httpClient.Dispose();
					request.Dispose();
					result.Dispose();
				}
				catch (Exception e)
				{
					Dbg.Warning("Exception caught when sending get request to main server.");
					Dbg.Warning("Message:", e.Message);
					callback(default);
				}
			});
		}

		/// <summary>
		/// 	Send a message to the main server.
		/// </summary>
		public static void Send(MSMessage message)
		{
			Task.Run(async () =>
			{
				try
				{
					HttpClient httpClient = new HttpClient();
					HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, new UriBuilder("http", IpAddress, Port, "/api/" + message.GetDestination()).Uri)
					{
						Content = new StringContent(message.ToString(), Encoding.UTF8, "application/json")
					};

					// Send message.
					HttpResponseMessage result = await httpClient.SendAsync(request);

					// Clean up.
					httpClient.Dispose();
					request.Dispose();
					result.Dispose();
				}
				catch (Exception e)
				{
					Dbg.Warning("Exception caught when sending post request to main server.");
					Dbg.Warning("Message: {0} ", e.Message);
				}
			});
		}

		/// <summary>
		/// 	Send a message to the server and get a callback.
		/// </summary>
		public static void Post<T>(MSMessage message, Action<T> callback)
		{
			Task.Run(async () =>
			{
				try
				{
					// Create new client and request and load it with data.
					HttpClient httpClient = new HttpClient();
					HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, new UriBuilder("http", IpAddress, Port, "/api/" + message.GetDestination()).Uri)
					{
						Content = new StringContent(message.ToString(), Encoding.UTF8, "application/json")
					};

					Dbg.Log("Sending data to:", request.RequestUri);

					// Send message and get result.
					HttpResponseMessage result = await httpClient.SendAsync(request);

					// Read data as string.
					string str = await result.Content.ReadAsStringAsync();

					Dbg.Log("Data:", str);

					// Make object out of data.
					callback(JsonSerialiser.Deserialize<T>(str));

					// Clean up.
					httpClient.Dispose();
					request.Dispose();
					result.Dispose();
				}
				catch (Exception e)
				{
					Dbg.Warning("Exception caught when sending post request to main server.");
					Dbg.Warning("Message: {0} ", e.Message);
					callback(default);
				}
			});
		}
	}
}
