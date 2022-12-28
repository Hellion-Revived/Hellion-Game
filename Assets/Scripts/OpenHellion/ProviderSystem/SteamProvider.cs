using UnityEngine;
using System.Collections;
using Steamworks;
using System;
using ZeroGravity;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace OpenHellion.ProviderSystem
{
	public class SteamProvider : IProvider
	{
		private bool _currentStatsRequested;
		private bool _userStatsReceived;
		private bool _storeStats;
		private Callback<UserStatsReceived_t> _userStatsReceivedCallback;
		private ConcurrentQueue<Task> _pendingTasks = new ConcurrentQueue<Task>();

		protected SteamAPIWarningMessageHook_t _SteamAPIWarningMessageHook;

		bool IProvider.Initialise()
		{
			if (!Packsize.Test())
			{
				Dbg.Error("[Steamworks.NET] Packsize Test returned false, the wrong version of Steamworks.NET is being run in this platform.", this);
			}

			if (!DllCheck.Test())
			{
				Dbg.Error("[Steamworks.NET] DllCheck Test returned false, One or more of the Steamworks binaries seems to be the wrong version.", this);
			}

			try
			{
				// If Steam is not running or the game wasn't started through Steam, SteamAPI_RestartAppIfNecessary starts the
				// Steam client and also launches this game again if the User owns it. This can act as a rudimentary form of DRM.
				// See the Valve documentation for more information: https://partner.steamgames.com/doc/sdk/api#initialization_and_shutdown
				if (SteamAPI.RestartAppIfNecessary((AppId_t)588210u))
				{
					Application.Quit();
					return false;
				}
			}
			catch (System.DllNotFoundException e)
			{ // We catch this exception here, as it will be the first occurrence of it.
				Dbg.Error("[Steamworks.NET] Could not load [lib]steam_api.dll/so/dylib. It's likely not in the correct location. Refer to the README for more details.\n" + e, this);

				Application.Quit();
				return false;
			}

			// https://partner.steamgames.com/doc/sdk/api#initialization_and_shutdown
			bool success = SteamAPI.Init();
			if (success)
			{
				Dbg.Log("Steamworks API initialised.");
			}

			return success;
		}

		// This should only ever get called on first load and after an Assembly reload, You should never Disable the Steamworks Manager yourself.
		void IProvider.Enable()
		{
			if (_SteamAPIWarningMessageHook == null)
			{
				// Set up our callback to receive warning messages from Steam.
				// You must launch with "-debug_steamapi" in the launch args to receive warnings.
				_SteamAPIWarningMessageHook = new SteamAPIWarningMessageHook_t(SteamAPIDebugTextHook);
				SteamClient.SetWarningMessageHook(_SteamAPIWarningMessageHook);
			}

			if (_currentStatsRequested)
			{
				SteamUserStats.RequestCurrentStats();
			}
		}

		// OnApplicationQuit gets called too early to shutdown the SteamAPI.
		// Because the SteamManager should be persistent and never disabled or destroyed we can shutdown the SteamAPI here.
		// Thus it is not recommended to perform any Steamworks work in other OnDestroy functions as the order of execution can not be garenteed upon Shutdown. Prefer OnDisable().
		void IProvider.Destroy()
		{
			SteamAPI.Shutdown();
		}

		void IProvider.Update()
		{
			// Run Steam client callbacks
			SteamAPI.RunCallbacks();

			if (!_currentStatsRequested)
			{
				_userStatsReceivedCallback = Callback<UserStatsReceived_t>.Create(callback => {
					_userStatsReceived = true;
					foreach (object value3 in Enum.GetValues(typeof(ProviderStatID)))
					{
						if (!GetStat((ProviderStatID)value3, out int _) && !GetStat((ProviderStatID)value3, out int x))
						{}
					}
				});
				_currentStatsRequested = SteamUserStats.RequestCurrentStats();
			}
			else if (_userStatsReceived)
			{
				Task result;
				while (_pendingTasks.TryDequeue(out result))
				{
					result.RunSynchronously();
				}
				if (_storeStats)
				{
					SteamUserStats.StoreStats();
					_storeStats = false;
				}
			}
		}

		public bool IsInitialised()
		{
			return true;
		}

		public void UpdateStatus() { }

		public bool GetAchievement(AchievementID id, out bool achieved)
		{
			return SteamUserStats.GetAchievement(id.ToString(), out achieved);
		}

		public void SetAchievement(AchievementID id)
		{
			_pendingTasks.Enqueue(new Task(delegate
			{
				SteamUserStats.SetAchievement(id.ToString());
				_storeStats = true;
			}));
		}

		public bool GetStat(ProviderStatID id, out int value)
		{
			return SteamUserStats.GetStat(id.ToString(), out value);
		}

		public void SetStat(ProviderStatID id, int value)
		{
			_pendingTasks.Enqueue(new Task(delegate
			{
				SteamUserStats.SetStat(id.ToString(), value);
				_storeStats = true;
			}));
		}

		public void ResetStat(ProviderStatID id)
		{
			SetStat(id, 0);
		}

		// TODO: Fix this.
		public void ChangeStatBy<T>(ProviderStatID id, T value)
		{
			if (typeof(T) == typeof(int))
			{
				ChangeStatBy(id, (int)(object)value);
			}
			else if (typeof(T) == typeof(float))
			{
				ChangeStatBy(id, (int)(object)value);
			}
		}

		private void ChangeStatBy(ProviderStatID id, int value)
		{
			_pendingTasks.Enqueue(new Task(delegate
			{
				if (GetStat(id, out int value2))
				{
					SteamUserStats.SetStat(id.ToString(), value2 + value);
					_storeStats = true;
				}
			}));
		}

		public string GetUsername()
		{
			return SteamFriends.GetFriendPersonaName(SteamUser.GetSteamID());
		}

		// TODO: Custom ID generation.
		public string GetId()
		{
			return SteamUser.GetSteamID().ToString();
		}

		// TODO: Custom ID generation.
		public IProvider.Friend[] GetFriends()
		{
			List<IProvider.Friend> friends = new();

			// Get all friends.
			for (int i = 0; i < SteamFriends.GetFriendCount(EFriendFlags.k_EFriendFlagImmediate); i++)
			{
				// Get friend's id.
				CSteamID id = SteamFriends.GetFriendByIndex(i, EFriendFlags.k_EFriendFlagImmediate);
				EPersonaState friendPersonaState = SteamFriends.GetFriendPersonaState(id);

				// Add friend to list of friends.
				friends.Add(new IProvider.Friend
				{
					Id = id.ToString(),
					Name = SteamFriends.GetFriendPersonaName(id),
					Status = friendPersonaState == EPersonaState.k_EPersonaStateOnline || friendPersonaState == EPersonaState.k_EPersonaStateLookingToPlay ? IProvider.FriendStatus.ONLINE : IProvider.FriendStatus.OFFLINE
				});
			}

			return friends.ToArray();
		}

		public Texture2D GetAvatar(string id)
		{
			int largeFriendAvatar = SteamFriends.GetLargeFriendAvatar(new CSteamID(ulong.Parse(id)));
			uint pnWidth;
			uint pnHeight;
			if (SteamUtils.GetImageSize(largeFriendAvatar, out pnWidth, out pnHeight) && pnWidth != 0 && pnHeight != 0)
			{
				byte[] array = new byte[pnWidth * pnHeight * 4];
				Texture2D texture2D = new Texture2D((int)pnWidth, (int)pnHeight, TextureFormat.RGBA32, false, false);
				if (SteamUtils.GetImageRGBA(largeFriendAvatar, array, (int)(pnWidth * pnHeight * 4)))
				{
					texture2D.LoadRawTextureData(array);
					texture2D.Apply();
				}
				return texture2D;
			}

			return Resources.Load<Texture2D>("UI/default_avatar");
		}

		public void InviteUser(string id, string secret)
		{
			SteamFriends.InviteUserToGame(new CSteamID(UInt64.Parse(id)), secret);
		}

		[AOT.MonoPInvokeCallback(typeof(SteamAPIWarningMessageHook_t))]
		protected static void SteamAPIDebugTextHook(int nSeverity, System.Text.StringBuilder pchDebugText)
		{
			Dbg.Warning(pchDebugText);
		}
	}
}