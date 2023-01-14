using System;
using System.Collections.Concurrent;
using System.Net.Sockets;
using System.Threading;
using OpenHellion.ProviderSystem;
using ZeroGravity;
using ZeroGravity.Network;

namespace OpenHellion.Networking
{
	// TODO: Rewrite this.
	internal sealed class GameServerThread
	{
		private volatile bool gameSocketReady;

		private volatile bool runThread;

		public Thread connectThread;

		public Thread sendingThread;

		public Thread listeningThread;

		private string _serverAddress;

		private int _serverPort;

		private string _serverId;

		private string _serverPassword;

		private int retryAttempt;

		private Socket socket;

		private ConcurrentQueue<NetworkData> networkDataQueue;

		private EventWaitHandle waitHandle;

		public void Start(string address, int port, string serverId, string password, int retryAttempt = 3)
		{
			_serverAddress = address;
			_serverPort = port;
			_serverId = serverId;
			_serverPassword = password;
			this.retryAttempt = retryAttempt;
			runThread = true;
			networkDataQueue = new ConcurrentQueue<NetworkData>();
			waitHandle = new EventWaitHandle(false, EventResetMode.AutoReset);

			connectThread = new Thread(ConnectThread);
			connectThread.IsBackground = true;
			connectThread.Start();

			sendingThread = new Thread(SendingThread);
			sendingThread.IsBackground = true;
			sendingThread.Start();
		}

		public void Send(NetworkData data)
		{
			if (gameSocketReady)
			{
				networkDataQueue.Enqueue(data);
				waitHandle.Set();
			}
		}

		public void StopImmediate()
		{
			runThread = false;
			if (waitHandle != null)
			{
				waitHandle.Set();
			}
			if (socket != null)
			{
				socket.Close();
			}
			listeningThread.Interrupt();
			listeningThread.Abort();
			sendingThread.Interrupt();
			sendingThread.Abort();
		}

		public void Stop()
		{
			runThread = false;
			gameSocketReady = false;
			if (waitHandle != null)
			{
				waitHandle.Set();
				socket.Close();
				if (sendingThread != null)
				{
					sendingThread.Interrupt();
				}
				if (listeningThread != null)
				{
					listeningThread.Interrupt();
				}
			}
		}

		private void SendingThread()
		{
			while (Client.IsRunning && runThread)
			{
				waitHandle.WaitOne();
				if (!Client.IsRunning || !runThread)
				{
					break;
				}
				NetworkData result;
				while (networkDataQueue.Count > 0)
				{
					if (!networkDataQueue.TryDequeue(out result))
					{
						EventSystem.Invoke(new EventSystem.InternalEventData(EventSystem.InternalEventType.RemoveLoadingCanvas));
						Dbg.Info("Problem occured while dequeueing network data");
						socket.Close();
						gameSocketReady = false;
						EventSystem.Invoke(new EventSystem.InternalEventData(EventSystem.InternalEventType.OpenMainScreen));
						return;
					}
					try
					{
						socket.Send(Serializer.Package(result));
						Client.Instance.LogSentNetworkData(result.GetType());
					}
					catch (ArgumentNullException)
					{
						EventSystem.Invoke(new EventSystem.InternalEventData(EventSystem.InternalEventType.RemoveLoadingCanvas));
						Dbg.Error("Serialized data buffer is null", result.GetType().ToString(), result);
					}
					catch (Exception ex2)
					{
						EventSystem.Invoke(new EventSystem.InternalEventData(EventSystem.InternalEventType.RemoveLoadingCanvas));
						if (!(ex2 is SocketException) && !(ex2 is ObjectDisposedException))
						{
							Dbg.Error("SendToGameServer exception", ex2.Message, ex2.StackTrace);
						}
						socket.Close();
						gameSocketReady = false;
						EventSystem.Invoke(new EventSystem.InternalEventData(EventSystem.InternalEventType.OpenMainScreen));
						return;
					}
				}
			}
		}

		public void DisconnectImmediate()
		{
			if (gameSocketReady)
			{
				gameSocketReady = false;
				if (socket != null)
				{
					socket.Close();
				}
				networkDataQueue = null;
				waitHandle = null;
				if (connectThread != null)
				{
					connectThread.Abort();
				}
				if (sendingThread != null)
				{
					sendingThread.Abort();
				}
			}
			StopImmediate();
		}

		public void Disconnect()
		{
			if (gameSocketReady)
			{
				gameSocketReady = false;
				if (socket != null)
				{
					socket.Close();
				}
				networkDataQueue = null;
				waitHandle = null;
				if (connectThread != null)
				{
					connectThread.Abort();
				}
				if (sendingThread != null)
				{
					sendingThread.Abort();
				}
			}
			Stop();
		}

		private void ConnectThread()
		{
			socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
			while (retryAttempt > 0)
			{
				try
				{
					IAsyncResult asyncResult = socket.BeginConnect(_serverAddress, _serverPort, null, null);
					if (!asyncResult.AsyncWaitHandle.WaitOne(TimeSpan.FromSeconds(5.0), false))
					{
						socket.Close();
						throw new TimeoutException();
					}
					if (socket.Connected)
					{
						gameSocketReady = true;
						FinalizeConnecting();
						return;
					}
					retryAttempt--;
				}
				catch (TimeoutException)
				{
					retryAttempt--;
					gameSocketReady = false;
					if (retryAttempt == 0)
					{
						FinalizeConnecting();
						EventSystem.Invoke(new EventSystem.InternalEventData(EventSystem.InternalEventType.ConnectionFailed));
					}
					return;
				}
				catch (Exception ex2)
				{
					Dbg.Error("Error occured while attempting to connect to game server", ex2.Message, ex2.StackTrace);
					FinalizeConnecting();
					return;
				}
			}
			FinalizeConnecting();
			EventSystem.Invoke(new EventSystem.InternalEventData(EventSystem.InternalEventType.ConnectionFailed));
		}

		private void FinalizeConnecting()
		{
			if (gameSocketReady)
			{
				listeningThread = new Thread(Listen)
				{
					IsBackground = true
				};
				listeningThread.Start();
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
			else
			{
				Disconnect();
				EventSystem.Invoke(new EventSystem.InternalEventData(EventSystem.InternalEventType.ConnectionFailed));
			}
		}

		/// <summary>
		/// 	Thread code that handles listening for data.<br/>
		/// 	Calls <c>EventSystem.Invoke</c>, which in turn refers it to a listener function.
		/// </summary>
		private void Listen()
		{
			while (Client.IsRunning && gameSocketReady && runThread)
			{
				try
				{
					NetworkData networkData = Serializer.ReceiveData(socket);
					if (networkData != null && NetworkController.Instance != null)
					{
						EventSystem.Invoke(networkData);
						Client.Instance.LogReceivedNetworkData(networkData.GetType());
						continue;
					}
					break;
				}
				catch (Exception ex)
				{
					if (ex is SocketException || (ex.InnerException != null && ex.InnerException is SocketException))
					{
						if (!Client.Instance.LogInResponseReceived)
						{
							EventSystem.Invoke(new EventSystem.InternalEventData(EventSystem.InternalEventType.ShowMessageBox, Localization.ConnectionError, Localization.TryAgainLater));
						}
						else if (Client.Instance.LogoutRequestSent)
						{
							Dbg.Info("Tried to listen to data, but logout was requested.");
						}
						else
						{
							Dbg.Error("***", ex.Message, ex.StackTrace);
						}
					}
					else
					{
						Dbg.Error("Game server listening thread exception", ex.Message, ex.StackTrace);
					}
					break;
				}
			}
		}
	}
}
