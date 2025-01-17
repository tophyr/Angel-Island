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

/* Scripts\Engines\Craft\Core\OldSchoolCraft.cs
 * ChangeLog:
 *	06/21/11, Adam
 *		Begin work on old school craft system
 */

using System;
using System.Collections;
using Server.Gumps;
using Server.Network;
using Server.Items;
using Server.Commands;
using Server.Menus;
using Server.Menus.Questions;
using Server.Menus.ItemLists;
using Server.Targeting;
using Server.Targets;
using Server.Engines.Craft;
using Server.Text;

namespace Server.Engines.OldSchoolCraft
{
	public abstract class SkillCraft
	{
		protected OldSchoolCraft m_craft;
		protected Mobile m_from;
		protected CraftSystem m_craftSystem;
		protected BaseTool m_tool;
		protected Server.Engines.Craft.Repair.RepairDeed m_Deed = null;					// not used - can be removed

		public OldSchoolCraft Craft { get { return m_craft; } }
		public SkillCraft(OldSchoolCraft craft)
		{
			m_craft = craft;
			m_from = m_craft.From;
			m_craftSystem = m_craft.CraftSystem;
			m_tool = m_craft.Tool;
		}
		public abstract void OnMenuSelection(int context, int operation);
		public abstract void OnItemSelection(int context, int operation, object targeted);

		#region Ultilties: GetWeakenChance, CheckWeaken, GetRepairDifficulty
		public int GetWeakenChance(Mobile mob, SkillName skill, int curHits, int maxHits)
		{
			// 40% - (1% per hp lost) - (1% per 10 craft skill)
			return (40 + (maxHits - curHits)) - (int)(((m_Deed != null) ? m_Deed.SkillLevel : mob.Skills[skill].Value) / 10);
		}

		public bool CheckWeaken(Mobile mob, SkillName skill, int curHits, int maxHits)
		{
			return (GetWeakenChance(mob, skill, curHits, maxHits) > Utility.Random(100));
		}

		public int GetRepairDifficulty(int curHits, int maxHits)
		{
			return (((maxHits - curHits) * 1250) / Math.Max(maxHits, 1)) - 250;
		}

		public bool CheckRepairDifficulty(Mobile mob, SkillName skill, int curHits, int maxHits)
		{
			double difficulty = GetRepairDifficulty(curHits, maxHits) * 0.1;

			if (m_Deed != null)
			{
				double value = m_Deed.SkillLevel;
				double minSkill = difficulty - 25.0;
				double maxSkill = difficulty + 25;

				if (value < minSkill)
					return false; // Too difficult
				else if (value >= maxSkill)
					return true; // No challenge

				double chance = (value - minSkill) / (maxSkill - minSkill);

				return (chance >= Utility.RandomDouble());
			}
			else
			{
				return mob.CheckSkill(skill, difficulty - 25.0, difficulty + 25.0);
			}
		}
		public bool IsSpecialWeapon(BaseWeapon weapon)
		{
			// Weapons repairable but not craftable

			if (m_craftSystem is DefTinkering)
			{
				return (weapon is Cleaver)
					|| (weapon is Hatchet)
					|| (weapon is Pickaxe)
					|| (weapon is ButcherKnife)
					|| (weapon is SkinningKnife);
			}
			else if (m_craftSystem is DefCarpentry)
			{
				return (weapon is Club)
					|| (weapon is BlackStaff)
					|| (weapon is MagicWand);
			}
			else if (m_craftSystem is DefBlacksmithy)
			{
				return (weapon is Pitchfork);
			}

			return false;
		}
		#endregion

		#region item table
		public class itemTable
		{
			public int ItemID;
			public int GroupName;	// unused
			public int ItemName;
			public double MinSkill;
			public double MaxSkill;
			public Type TypeRes;
			public int NameRes;
			public int Amount;
			public int FailMessage;
			public string Name;
			public Type TypeItem;
			public CraftItem CraftItem;

			public itemTable(int itemID, int groupName, int itemName, double minSkill, double maxSkill, Type typeRes, int nameRes, int amount, int failMessage, string name, Type typeItem)
			{
				ItemID = itemID;
				GroupName = groupName;
				ItemName = itemName;
				MinSkill = minSkill;
				MaxSkill = maxSkill;
				TypeRes = typeRes;
				NameRes = nameRes;
				Amount = amount;
				FailMessage = failMessage;
				Name = name;
				TypeItem = typeItem;

				CraftItem = new CraftItem(typeItem, groupName, itemName);
				CraftItem.AddRes(typeRes, nameRes, amount, FailMessage);
				CraftItem.AddSkill(SkillName.Blacksmith, minSkill, maxSkill);
			}

			public static itemTable AddCraft(Type typeItem, int groupName, int itemName, double minSkill, double maxSkill, Type typeRes, int nameRes, int amount, int localizedMessage)
			{
				Item item = Activator.CreateInstance(typeItem) as Item;

				if (item == null)
					return null;

				// we switched from "TileData.ItemTable[item.ItemID].Name" to the actual Cliloc since the actual graphic sometimes has
				//	the wrong name :\
				//	For instance, accorting to TileData.ItemTable[item.ItemID].Name, the new style "tinker's tools" are called "tool kit"
				return new itemTable(item.ItemID, groupName, itemName, minSkill, maxSkill, typeRes, nameRes, amount, localizedMessage, 
					Cliloc.Lookup[itemName]/*TileData.ItemTable[item.ItemID].Name*/, item.GetType());
			}

			public static itemTable Find(itemTable[] table, int itemId)
			{
				foreach (itemTable i in table)
				{
					if (i.ItemID == itemId)
						return i;
				}

				return null;
			}

			public static itemTable Copy(itemTable[] table, int itemId)
			{
				itemTable f = Find(table, itemId);
				if (f == null)
					return null;

				// suitable for modification.
				return new itemTable(f.ItemID, f.GroupName, f.ItemName, f.MinSkill, f.MaxSkill, f.TypeRes, f.NameRes, f.Amount, f.FailMessage, f.Name, f.TypeItem);
			}
		}
		#endregion
	}

	public class OldSchoolCraft
	{
		private Mobile m_from;
		private CraftSystem m_craftSystem;
		private BaseTool m_tool;

		public Mobile From { get { return m_from; } }
		public CraftSystem CraftSystem { get { return m_craftSystem; } }
		public BaseTool Tool { get { return m_tool; } }

		public OldSchoolCraft(Mobile from, CraftSystem craftSystem, BaseTool tool, object notice)
		{
			m_from = from;
			m_craftSystem = craftSystem;
			m_tool = tool;

			// setup
			m_craftSystem.OldSchool = true;							// supresses gumps
			CraftContext context = craftSystem.GetContext(from);	// creates the context, even if not used here
			context.MarkOption = CraftMarkOption.DoNotMark;			// never prompt for makers mark using RunUO system. we will handle it POST item creation
		}

		public bool DoOldSchoolCraft()
		{
			switch (m_craftSystem.MainSkill)
			{
				case SkillName.Blacksmith: new BlacksmithCraft(this).DoCraft(); return true;
				case SkillName.Tinkering: new TinkeringCraft(this).DoCraft(); return true;
			}

			return false;
		}
	}


	#region Targets and Menus
	public class ItemTarget : Target
	{
		private SkillCraft m_craft;
		private int m_context;
		private int m_operation;

		public ItemTarget(SkillCraft craft, int context, int operation)
			: base(-1, false, TargetFlags.None)
		{
			m_craft = craft;
			m_context = context;
			m_operation = operation;
		}

		protected override void OnTarget(Mobile from, object targeted)
		{
			m_craft.OnItemSelection(m_context, m_operation, targeted);
		}
	}

	public class Selection
	{
		private ItemListEntry m_itemListEntry;
		public ItemListEntry ItemListEntry { get { return m_itemListEntry; } set { m_itemListEntry = value; } }
		private int m_context;
		public int Context { get { return m_context; } set { m_context = value; } }
		public Selection(ItemListEntry itemListEntry, int context)
		{
			m_itemListEntry = itemListEntry;
			m_context = context;
		}
	}

	public class ItemMenu : ItemListMenu
	{
		private Mobile m_Mobile;
		private SkillCraft m_craft;
		private Selection[] m_options;

		public ItemMenu(SkillCraft craft, Selection[] options, string title)
			: base(title, ParseOptions(options))
		{
			m_Mobile = craft.Craft.From;
			m_craft = craft;
			m_options = options;
		}

		public override void OnResponse(NetState state, int index)
		{
			if (index >= 0 && index < m_options.Length)
			{
				m_craft.OnMenuSelection(m_options[index].Context, Entries[index].ItemID);
			}
		}

		private static ItemListEntry[] ParseOptions(Selection[] options)
		{
			ItemListEntry[] entries = new ItemListEntry[options.Length];
			for (int index = 0; index < options.Length; index++)
				entries[index] = options[index].ItemListEntry;
			return entries;
		}
	}
	#endregion
}

