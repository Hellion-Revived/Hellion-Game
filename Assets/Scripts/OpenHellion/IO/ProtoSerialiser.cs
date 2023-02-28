using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using ProtoBuf;
using ZeroGravity.Network;
using ZeroGravity.Objects;

namespace OpenHellion.IO
{
	/// <summary>
	/// 	Class for deserialisation of network messages. Handles safe deserialsation of packets from the server.
	/// </summary>
	public static class ProtoSerialiser
	{
		public class ZeroDataException : Exception
		{
			public ZeroDataException(string message)
				: base(message)
			{
			}
		}

		public class StatisticsHelper
		{
			public long ByteSum;

			public int PacketNubmer;

			public long BytesSinceLastCheck;

			public StatisticsHelper(long bytes)
			{
				ByteSum = bytes;
				PacketNubmer = 1;
				BytesSinceLastCheck = bytes;
			}
		}

		private static DateTime s_statisticUpdateResetTime = DateTime.UtcNow;

		private static DateTime s_lastStatisticUpdateTime;

		private static readonly double _statisticsLogUpdateTime = 1.0;

		private static readonly Dictionary<Type, StatisticsHelper> s_sentStatistics = new Dictionary<Type, StatisticsHelper>();

		private static readonly Dictionary<Type, StatisticsHelper> s_receivedStatistics = new Dictionary<Type, StatisticsHelper>();

		/// <summary>
		/// 	For deserialisation of data not sent through network.
		/// </summary>
		private static NetworkData Deserialize(MemoryStream ms)
		{
			NetworkData networkData = null;
			ms.Position = 0L;
			try
			{
				networkData = ProtoBuf.Serializer.Deserialize<NetworkDataTransportWrapper>(ms).data;
			}
			catch (Exception ex)
			{
				Dbg.Error("Failed to deserialize communication data", ex.Message, ex.StackTrace);
			}
			if (_statisticsLogUpdateTime > 0.0)
			{
				try
				{
					ProcessStatistics(networkData, ms, s_receivedStatistics);
					return networkData;
				}
				catch
				{
					return networkData;
				}
			}
			return networkData;
		}

		public static NetworkData ReceiveData(Socket soc)
		{
			if (soc == null || !soc.Connected)
			{
				return null;
			}
			return Unpackage(new NetworkStream(soc));
		}

		public static NetworkData Unpackage(Stream str)
		{
			byte[] bufferSize = new byte[4];
			int dataReadSize = 0;
			int size;
			do
			{
				size = str.Read(bufferSize, dataReadSize, bufferSize.Length - dataReadSize);
				if (size == 0)
				{
					throw new ZeroDataException("Received zero data message.");
				}
				dataReadSize += size;
			}
			while (dataReadSize < bufferSize.Length);
			uint bufferLength = BitConverter.ToUInt32(bufferSize, 0);
			byte[] buffer = new byte[bufferLength];
			dataReadSize = 0;
			do
			{
				size = str.Read(buffer, dataReadSize, buffer.Length - dataReadSize);
				if (size == 0)
				{
					throw new ZeroDataException("Received zero data message.");
				}
				dataReadSize += size;
			}
			while (dataReadSize < buffer.Length);
			MemoryStream ms = new MemoryStream(buffer, 0, buffer.Length);
			return Deserialize(ms);
		}

		public static byte[] Package(NetworkData data)
		{
			using MemoryStream outMs = new MemoryStream();
			using MemoryStream ms = new MemoryStream();
			try
			{
				NetworkDataTransportWrapper ndtw = new NetworkDataTransportWrapper
				{
					data = data
				};
				NetworkDataTransportWrapper instance = ndtw;
				ProtoBuf.Serializer.Serialize(ms, instance);
			}
			catch (Exception ex)
			{
				Dbg.Error("Failed to serialize communication data", ex.Message, ex.StackTrace);
				return null;
			}
			if (_statisticsLogUpdateTime > 0.0)
			{
				try
				{
					ProcessStatistics(data, ms, s_sentStatistics);
				}
				catch
				{
				}
			}
			outMs.Write(BitConverter.GetBytes((uint)ms.Length), 0, 4);
			outMs.Write(ms.ToArray(), 0, (int)ms.Length);
			outMs.Flush();
			return outMs.ToArray();
		}

		private static void ProcessStatistics(NetworkData data, MemoryStream ms, Dictionary<Type, StatisticsHelper> stat)
		{
			Type type = data.GetType();
			if (stat.TryGetValue(type, out var value))
			{
				value.ByteSum += ms.Length;
				value.PacketNubmer++;
				value.BytesSinceLastCheck += ms.Length;
			}
			else
			{
				stat[type] = new StatisticsHelper(ms.Length);
			}
			if (!(DateTime.UtcNow.Subtract(s_lastStatisticUpdateTime).TotalSeconds >= _statisticsLogUpdateTime))
			{
				return;
			}
			TimeSpan timeSpan = DateTime.UtcNow.Subtract(s_statisticUpdateResetTime);
			string text = (stat != s_sentStatistics) ? ("Received packets statistics (" + timeSpan.ToString("h':'mm':'ss") + "): \n") : ("Sent packets statistics (" + timeSpan.ToString("h':'mm':'ss") + "): \n");
			long num = 0L;
			string text2;
			foreach (KeyValuePair<Type, StatisticsHelper> item in stat.OrderBy((KeyValuePair<Type, StatisticsHelper> m) => m.Value.ByteSum).Reverse())
			{
				text2 = text;
				text = text2 + item.Key.Name + ": " + item.Value.PacketNubmer + " (" + ((float)item.Value.ByteSum / 1000f).ToString("##,0") + " kB), \n";
				item.Value.BytesSinceLastCheck = 0L;
				num += item.Value.ByteSum;
			}
			text2 = text;
			text = text2 + "-----------------------------------------\nTotal: " + ((float)num / 1000f).ToString("##,0") + " kB (avg: " + ((double)num / timeSpan.TotalSeconds / 1000.0).ToString("##,0") + " kB/s)";
			if (MyPlayer.Instance != null)
			{
				if (stat == s_sentStatistics)
				{
					MyPlayer.Instance.SentPacketStatistics = text;
				}
				else
				{
					MyPlayer.Instance.ReceivedPacketStatistics = text;
				}
			}
			s_lastStatisticUpdateTime = DateTime.UtcNow;
		}

		public static void ResetStatistics()
		{
			s_sentStatistics.Clear();
			s_receivedStatistics.Clear();
			s_statisticUpdateResetTime = DateTime.UtcNow;
			s_lastStatisticUpdateTime = DateTime.UtcNow;
		}
	}
}