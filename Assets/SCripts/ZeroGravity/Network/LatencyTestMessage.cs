using ProtoBuf;

namespace ZeroGravity.Network
{
	[ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
	public class LatencyTestMessage : NetworkData
	{
	}
}
