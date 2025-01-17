/*
 *	This program is the CONFIDENTIAL and PROPRIETARY property 
 *	of Tomasello Software LLC. Any unauthorized use, reproduction or
 *	transfer of this computer program is strictly prohibited.
 *
 *      Copyright (c) 2004 Tomasello Software LLC.
 *	This is an unpublished work, and is subject to limited distribution and
 *	restricted disclosure only. ALL RIGHTS RESERVED.
 *
 *			RESTRICTED RIGHTS LEGEND
 *	Use, duplication, or disclosure by the Government is subject to
 *	restrictions set forth in subparagraph (c)(1)(ii) of the Rights in
 * 	Technical Data and Computer Software clause at DFARS 252.227-7013.
 *
 *	Angel Island UO Shard	Version 1.0
 *			Release A
 *			March 25, 2004
 */

/* Scripts/Engines/RemoteAdmin/PacketHandler.cs
 * Changelog:
 *  6/5/04, Pix
 *		Merged in 1.0RC0 code.
 */

using System;
using System.Collections;
using Server;
using Server.Network;
using Server.Accounting;
using Server.Items;
using Server.Mobiles;

namespace Server.Admin
{
	public class RemoteAdminHandlers
	{
		public enum AcctSearchType : byte
		{
			Username = 0,
			IP = 1,
		}

		private static OnPacketReceive[] m_Handlers = new OnPacketReceive[256];

		static RemoteAdminHandlers()
		{
			//0x02 = login request, handled by AdminNetwork
			Register(0x04, new OnPacketReceive(ServerInfoRequest));
			Register(0x05, new OnPacketReceive(AccountSearch));
			Register(0x06, new OnPacketReceive(RemoveAccount));
			Register(0x07, new OnPacketReceive(UpdateAccount));
		}

		public static void Register(byte command, OnPacketReceive handler)
		{
			m_Handlers[command] = handler;
		}

		public static bool Handle(byte command, NetState state, PacketReader pvSrc)
		{
			if (m_Handlers[command] == null)
			{
				Console.WriteLine("ADMIN: Invalid packet 0x{0:X2} from {1}, disconnecting", command, state);
				return false;
			}
			else
			{
				m_Handlers[command](state, pvSrc);
				return true;
			}
		}

		private static void ServerInfoRequest(NetState state, PacketReader pvSrc)
		{
			state.Send(AdminNetwork.Compress(new ServerInfo()));
		}

		private static void AccountSearch(NetState state, PacketReader pvSrc)
		{
			AcctSearchType type = (AcctSearchType)pvSrc.ReadByte();
			string term = pvSrc.ReadString();

			if (type == AcctSearchType.IP && !Utility.IsValidIP(term))
			{
				state.Send(new MessageBoxMessage("Invalid search term.\nThe IP sent was not valid.", "Invalid IP"));
				return;
			}
			else
			{
				term = term.ToUpper();
			}

			ArrayList list = new ArrayList();

			foreach (Account a in Accounts.Table.Values)
			{
				switch (type)
				{
					case AcctSearchType.Username:
						{
							if (a.Username.ToUpper().IndexOf(term) != -1)
								list.Add(a);
							break;
						}
					case AcctSearchType.IP:
						{
							for (int i = 0; i < a.LoginIPs.Length; i++)
							{
								if (Utility.IPMatch(term, a.LoginIPs[i]))
								{
									list.Add(a);
									break;
								}
							}
							break;
						}
				}
			}

			if (list.Count > 0)
			{
				if (list.Count <= 25)
					state.Send(AdminNetwork.Compress(new AccountSearchResults(list)));
				else
					state.Send(new MessageBoxMessage("There were more than 25 matches to your search.\nNarrow the search parameters and try again.", "Too Many Results"));
			}
			else
			{
				state.Send(new MessageBoxMessage("There were no results to your search.\nPlease try again.", "No Matches"));
			}
		}

		private static void RemoveAccount(NetState state, PacketReader pvSrc)
		{
			Account a = Accounts.GetAccount(pvSrc.ReadString());

			if (a == null)
			{
				state.Send(new MessageBoxMessage("The account could not be found (and thus was not deleted).", "Account Not Found"));
			}
			else if (a == state.Account)
			{
				state.Send(new MessageBoxMessage("You may not delete your own account.", "Not Allowed"));
			}
			else
			{
				for (int i = 0; i < 5; i++)
				{
					if (a[i] != null)
						a[i].Delete();
				}

				Accounts.Table.Remove(a.Username);
				state.Send(new MessageBoxMessage("The requested account (and all it's characters) has been deleted.", "Account Deleted"));
			}
		}

		private static void UpdateAccount(NetState state, PacketReader pvSrc)
		{
			string username = pvSrc.ReadString();
			string pass = pvSrc.ReadString();

			Account a = Accounts.GetAccount(username);

			if (a == null)
				a = Accounts.AddAccount(username, pass);
			else if (pass != "(hidden)")
				a.SetPassword(pass);

			if (a != state.Account)
			{
				a.AccessLevel = (AccessLevel)pvSrc.ReadByte();
				a.Banned = pvSrc.ReadBoolean();
			}
			else
			{
				pvSrc.ReadInt16();//skip both
				state.Send(new MessageBoxMessage("Warning: When editing your own account, account status and accesslevel cannot be changed.", "Editing Own Account"));
			}

			ArrayList list = new ArrayList();
			ushort length = pvSrc.ReadUInt16();
			bool invalid = false;
			for (int i = 0; i < length; i++)
			{
				string add = pvSrc.ReadString();
				if (Utility.IsValidIP(add))
					list.Add(add);
				else
					invalid = true;
			}

			if (list.Count > 0)
				a.IPRestrictions = (string[])list.ToArray(typeof(string));
			else
				a.IPRestrictions = new string[0];

			if (invalid)
				state.Send(new MessageBoxMessage("Warning: one or more of the IP Restrictions you specified was not valid.", "Invalid IP Restriction"));
			state.Send(new MessageBoxMessage("Account updated successfully.", "Account Updated"));
		}
	}
}
