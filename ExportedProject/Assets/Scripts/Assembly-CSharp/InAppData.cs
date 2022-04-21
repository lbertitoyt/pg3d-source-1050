using System.Collections.Generic;
using System.Linq;

public static class InAppData
{
	public static Dictionary<int, KeyValuePair<string, string>> inAppData;

	public static Dictionary<string, string> inappReadableNames;

	static InAppData()
	{
		inAppData = new Dictionary<int, KeyValuePair<string, string>>();
		inappReadableNames = new Dictionary<string, string>();
		inAppData.Add(5, new KeyValuePair<string, string>(StoreKitEventListener.endmanskin, Defs.endmanskinBoughtSett));
		inAppData.Add(11, new KeyValuePair<string, string>(StoreKitEventListener.chief, Defs.chiefBoughtSett));
		inAppData.Add(12, new KeyValuePair<string, string>(StoreKitEventListener.spaceengineer, Defs.spaceengineerBoughtSett));
		inAppData.Add(13, new KeyValuePair<string, string>(StoreKitEventListener.nanosoldier, Defs.nanosoldierBoughtSett));
		inAppData.Add(14, new KeyValuePair<string, string>(StoreKitEventListener.steelman, Defs.steelmanBoughtSett));
		inAppData.Add(15, new KeyValuePair<string, string>(StoreKitEventListener.CaptainSkin, Defs.captainSett));
		inAppData.Add(16, new KeyValuePair<string, string>(StoreKitEventListener.HawkSkin, Defs.hawkSett));
		inAppData.Add(17, new KeyValuePair<string, string>(StoreKitEventListener.GreenGuySkin, Defs.greenGuySett));
		inAppData.Add(18, new KeyValuePair<string, string>(StoreKitEventListener.TunderGodSkin, Defs.TunderGodSett));
		inAppData.Add(19, new KeyValuePair<string, string>(StoreKitEventListener.GordonSkin, Defs.gordonSett));
		inAppData.Add(23, new KeyValuePair<string, string>(StoreKitEventListener.magicGirl, Defs.magicGirlSett));
		inAppData.Add(24, new KeyValuePair<string, string>(StoreKitEventListener.braveGirl, Defs.braveGirlSett));
		inAppData.Add(25, new KeyValuePair<string, string>(StoreKitEventListener.glamDoll, Defs.glamGirlSett));
		inAppData.Add(26, new KeyValuePair<string, string>(StoreKitEventListener.kittyGirl, Defs.kityyGirlSett));
		inAppData.Add(27, new KeyValuePair<string, string>(StoreKitEventListener.famosBoy, Defs.famosBoySett));
		for (int i = 0; i < 11; i++)
		{
			inAppData.Add(29 + i - 1, new KeyValuePair<string, string>("newskin_" + i, "newskin_" + i));
		}
		for (int j = 11; j < 19; j++)
		{
			inAppData.Add(29 + j - 1, new KeyValuePair<string, string>("newskin_" + j, "newskin_" + j));
		}
		inAppData.Add(47, new KeyValuePair<string, string>(StoreKitEventListener.skin810_1, Defs.skin810_1));
		inAppData.Add(48, new KeyValuePair<string, string>(StoreKitEventListener.skin810_2, Defs.skin810_2));
		inAppData.Add(49, new KeyValuePair<string, string>(StoreKitEventListener.skin810_3, Defs.skin810_3));
		inAppData.Add(50, new KeyValuePair<string, string>(StoreKitEventListener.skin810_4, Defs.skin810_4));
		inAppData.Add(51, new KeyValuePair<string, string>(StoreKitEventListener.skin810_5, Defs.skin810_5));
		inAppData.Add(52, new KeyValuePair<string, string>(StoreKitEventListener.skin810_6, Defs.skin810_6));
		inAppData.Add(53, new KeyValuePair<string, string>(StoreKitEventListener.skin931_1, Defs.skin931_1));
		inAppData.Add(54, new KeyValuePair<string, string>(StoreKitEventListener.skin931_2, Defs.skin931_2));
		for (int k = 0; k < StoreKitEventListener.Skins_11_040915.Length; k++)
		{
			inAppData.Add(55 + k, new KeyValuePair<string, string>(StoreKitEventListener.Skins_11_040915[k], Defs.Skins_11_040915[k]));
		}
		inAppData.Add(61, new KeyValuePair<string, string>("super_socialman", "super_socialman"));
		inAppData.Add(62, new KeyValuePair<string, string>(StoreKitEventListener.skin_tiger, Defs.skin_tiger));
		inAppData.Add(63, new KeyValuePair<string, string>(StoreKitEventListener.skin_pitbull, Defs.skin_pitbull));
		inAppData.Add(64, new KeyValuePair<string, string>(StoreKitEventListener.skin_santa, Defs.skin_santa));
		inAppData.Add(65, new KeyValuePair<string, string>(StoreKitEventListener.skin_elf_new_year, Defs.skin_elf_new_year));
		inAppData.Add(66, new KeyValuePair<string, string>(StoreKitEventListener.skin_girl_new_year, Defs.skin_girl_new_year));
		inAppData.Add(67, new KeyValuePair<string, string>(StoreKitEventListener.skin_cookie_new_year, Defs.skin_cookie_new_year));
		inAppData.Add(68, new KeyValuePair<string, string>(StoreKitEventListener.skin_snowman_new_year, Defs.skin_snowman_new_year));
		inAppData.Add(69, new KeyValuePair<string, string>(StoreKitEventListener.skin_jetti_hnight, Defs.skin_jetti_hnight));
		inAppData.Add(70, new KeyValuePair<string, string>(StoreKitEventListener.skin_startrooper, Defs.skin_startrooper));
		inAppData.Add(71, new KeyValuePair<string, string>(StoreKitEventListener.easter_skin1, Defs.easter_skin1));
		inAppData.Add(72, new KeyValuePair<string, string>(StoreKitEventListener.easter_skin2, Defs.easter_skin2));
		inAppData.Add(73, new KeyValuePair<string, string>(StoreKitEventListener.skin_rapid_girl, Defs.skin_rapid_girl));
		inAppData.Add(74, new KeyValuePair<string, string>(StoreKitEventListener.skin_silent_killer, Defs.skin_silent_killer));
		inAppData.Add(75, new KeyValuePair<string, string>(StoreKitEventListener.skin_daemon_fighter, Defs.skin_daemon_fighter));
		inAppData.Add(76, new KeyValuePair<string, string>(StoreKitEventListener.skin_scary_demon, Defs.skin_scary_demon));
		inAppData.Add(77, new KeyValuePair<string, string>(StoreKitEventListener.skin_orc_warrior, Defs.skin_orc_warrior));
		inAppData.Add(78, new KeyValuePair<string, string>(StoreKitEventListener.skin_kung_fu_master, Defs.skin_kung_fu_master));
		inAppData.Add(79, new KeyValuePair<string, string>(StoreKitEventListener.skin_fire_wizard, Defs.skin_fire_wizard));
		inAppData.Add(80, new KeyValuePair<string, string>(StoreKitEventListener.skin_ice_wizard, Defs.skin_ice_wizard));
		inAppData.Add(81, new KeyValuePair<string, string>(StoreKitEventListener.skin_storm_wizard, Defs.skin_storm_wizard));
		inAppData.Add(82, new KeyValuePair<string, string>(StoreKitEventListener.skin_may1, Defs.skin_may1));
		inAppData.Add(83, new KeyValuePair<string, string>(StoreKitEventListener.skin_may2, Defs.skin_may2));
		inAppData.Add(84, new KeyValuePair<string, string>(StoreKitEventListener.skin_may3, Defs.skin_may3));
		inAppData.Add(85, new KeyValuePair<string, string>(StoreKitEventListener.skin_may4, Defs.skin_may4));
		inappReadableNames.Add("bigammopack", "Big Pack of Ammo");
		inappReadableNames.Add("Fullhealth", "Full Health");
		inappReadableNames.Add(StoreKitEventListener.elixirID, "Elixir of Resurrection");
		inappReadableNames.Add(StoreKitEventListener.armor, "Armor");
		inappReadableNames.Add(StoreKitEventListener.armor2, "Armor2");
		inappReadableNames.Add(StoreKitEventListener.armor3, "Armor3");
		inappReadableNames.Add(StoreKitEventListener.endmanskin, "End Man Skin for 40 coins");
		inappReadableNames.Add(StoreKitEventListener.chief, "Chief Skin for 40 coins");
		inappReadableNames.Add(StoreKitEventListener.spaceengineer, "Space Engineer Skin for 40 coins");
		inappReadableNames.Add(StoreKitEventListener.nanosoldier, "Nano Soldier Skin for 40 coins");
		inappReadableNames.Add(StoreKitEventListener.steelman, "Steel Man Skin for 40 coins");
		inappReadableNames.Add(StoreKitEventListener.CaptainSkin, "Captain Skin for 40 coins");
		inappReadableNames.Add(StoreKitEventListener.HawkSkin, "Hawk Skin for 40 coins");
		inappReadableNames.Add(StoreKitEventListener.TunderGodSkin, "Thunder God Skin for 40 coins");
		inappReadableNames.Add(StoreKitEventListener.GreenGuySkin, "Green Guy Skin for 40 coins");
		inappReadableNames.Add(StoreKitEventListener.GordonSkin, "Gordon Skin for 40 coins");
		inappReadableNames.Add(StoreKitEventListener.magicGirl, "Magic Girl Skin for 40 coins");
		inappReadableNames.Add(StoreKitEventListener.braveGirl, "Brave Girl Skin for 40 coins");
		inappReadableNames.Add(StoreKitEventListener.glamDoll, "Glam Doll Skin for 40 coins");
		inappReadableNames.Add(StoreKitEventListener.kittyGirl, "Kitty Skin for 40 coins");
		inappReadableNames.Add(StoreKitEventListener.famosBoy, "Famos Boy Skin for 40 coins");
		inappReadableNames.Add(StoreKitEventListener.skin810_1, "skin810_1");
		inappReadableNames.Add(StoreKitEventListener.skin810_2, "skin810_2");
		inappReadableNames.Add(StoreKitEventListener.skin810_3, "skin810_3");
		inappReadableNames.Add(StoreKitEventListener.skin810_4, "skin810_4");
		inappReadableNames.Add(StoreKitEventListener.skin810_5, "skin810_5");
		inappReadableNames.Add(StoreKitEventListener.skin810_6, "skin810_6");
		inappReadableNames.Add(StoreKitEventListener.skin_santa, "skin_santa");
		inappReadableNames.Add(StoreKitEventListener.skin_elf_new_year, "skin_elf_new_year");
		inappReadableNames.Add(StoreKitEventListener.skin_girl_new_year, "skin_girl_new_year");
		inappReadableNames.Add(StoreKitEventListener.skin_cookie_new_year, "skin_cookie_new_year");
		inappReadableNames.Add(StoreKitEventListener.skin_snowman_new_year, "skin_snowman_new_year");
		inappReadableNames.Add(StoreKitEventListener.skin_jetti_hnight, FlurryPluginWrapper.ConvertFromBase64("c2tpbiBqZWRpIGtuaWdodA=="));
		inappReadableNames.Add(StoreKitEventListener.skin_startrooper, FlurryPluginWrapper.ConvertFromBase64("c2tpbiBzdGFyd2FycyBzdG9ybXRyb29wZXI="));
		inappReadableNames.Add(StoreKitEventListener.skin931_1, "skin931_1");
		inappReadableNames.Add(StoreKitEventListener.skin931_2, "skin931_2");
		for (int l = 0; l < StoreKitEventListener.Skins_11_040915.Length; l++)
		{
			inappReadableNames.Add(StoreKitEventListener.Skins_11_040915[l], StoreKitEventListener.Skins_11_040915[l]);
		}
		inappReadableNames.Add("super_socialman", "super_socialman");
		inappReadableNames.Add(StoreKitEventListener.skin_tiger, StoreKitEventListener.skin_tiger);
		inappReadableNames.Add(StoreKitEventListener.skin_pitbull, StoreKitEventListener.skin_pitbull);
		inappReadableNames.Add(StoreKitEventListener.easter_skin1, "easter_skin1");
		inappReadableNames.Add(StoreKitEventListener.easter_skin2, "easter_skin2");
		inappReadableNames.Add(StoreKitEventListener.skin_rapid_girl, StoreKitEventListener.skin_rapid_girl);
		inappReadableNames.Add(StoreKitEventListener.skin_daemon_fighter, StoreKitEventListener.skin_daemon_fighter);
		inappReadableNames.Add(StoreKitEventListener.skin_scary_demon, StoreKitEventListener.skin_scary_demon);
		inappReadableNames.Add(StoreKitEventListener.skin_silent_killer, StoreKitEventListener.skin_silent_killer);
		inappReadableNames.Add(StoreKitEventListener.skin_orc_warrior, StoreKitEventListener.skin_orc_warrior);
		inappReadableNames.Add(StoreKitEventListener.skin_kung_fu_master, StoreKitEventListener.skin_kung_fu_master);
		inappReadableNames.Add(StoreKitEventListener.skin_fire_wizard, StoreKitEventListener.skin_fire_wizard);
		inappReadableNames.Add(StoreKitEventListener.skin_ice_wizard, StoreKitEventListener.skin_ice_wizard);
		inappReadableNames.Add(StoreKitEventListener.skin_storm_wizard, StoreKitEventListener.skin_storm_wizard);
		inappReadableNames.Add(StoreKitEventListener.skin_may1, StoreKitEventListener.skin_may1);
		inappReadableNames.Add(StoreKitEventListener.skin_may2, StoreKitEventListener.skin_may2);
		inappReadableNames.Add(StoreKitEventListener.skin_may3, StoreKitEventListener.skin_may3);
		inappReadableNames.Add(StoreKitEventListener.skin_may4, StoreKitEventListener.skin_may4);
		inappReadableNames.Add("cape_Archimage", "Archimage Cape");
		inappReadableNames.Add("cape_BloodyDemon", "Bloody Demon Cape");
		inappReadableNames.Add("cape_RoyalKnight", "Royal Knight Cape");
		inappReadableNames.Add("cape_SkeletonLord", "Skeleton Lord Cape");
		inappReadableNames.Add("cape_EliteCrafter", "Elite Crafter Cape");
		inappReadableNames.Add("cape_Custom", "Custom Cape");
		inappReadableNames.Add("HitmanCape_Up1", "HitmanCape_Up1");
		inappReadableNames.Add("BerserkCape_Up1", "BerserkCape_Up1");
		inappReadableNames.Add("DemolitionCape_Up1", "DemolitionCape_Up1");
		inappReadableNames.Add("cape_Engineer", "EngineerCape");
		inappReadableNames.Add("cape_Engineer_Up1", "EngineerCape_Up1");
		inappReadableNames.Add("cape_Engineer_Up2", "EngineerCape_Up2");
		inappReadableNames.Add("SniperCape_Up1", "SniperCape_Up1");
		inappReadableNames.Add("StormTrooperCape_Up1", "StormTrooperCape_Up1");
		inappReadableNames.Add("HitmanCape_Up2", "HitmanCape_Up2");
		inappReadableNames.Add("BerserkCape_Up2", "BerserkCape_Up2");
		inappReadableNames.Add("DemolitionCape_Up2", "DemolitionCape_Up2");
		inappReadableNames.Add("SniperCape_Up2", "SniperCape_Up2");
		inappReadableNames.Add("StormTrooperCape_Up2", "StormTrooperCape_Up2");
		inappReadableNames.Add("hat_Adamant_3", "hat_Adamant_3");
		inappReadableNames.Add("hat_DiamondHelmet", "Diamond Helmet");
		inappReadableNames.Add("hat_Headphones", "Headphones");
		inappReadableNames.Add("hat_KingsCrown", "King's Crown");
		inappReadableNames.Add("hat_SeriousManHat", "Leprechaun's Hat");
		inappReadableNames.Add("hat_Samurai", "Samurais Helm");
		inappReadableNames.Add("league2_hat_cowboyhat", "league2_hat_cowboyhat");
		inappReadableNames.Add("league3_hat_afro", "league3_hat_afro");
		inappReadableNames.Add("league4_hat_mushroom", "league4_hat_mushroom");
		inappReadableNames.Add("league5_hat_brain", "league5_hat_brain");
		inappReadableNames.Add("league6_hat_tiara", "league6_hat_tiara");
		inappReadableNames.Add("hat_AlmazHelmet", "hat_AlmazHelmet");
		inappReadableNames.Add("hat_ArmyHelmet", "hat_ArmyHelmet");
		inappReadableNames.Add("hat_SteelHelmet", "hat_SteelHelmet");
		inappReadableNames.Add("hat_GoldHelmet", "hat_GoldHelmet");
		inappReadableNames.Add("hat_Army_1", "hat_Army_1");
		inappReadableNames.Add("hat_Almaz_1", "hat_Almaz_1");
		inappReadableNames.Add("hat_Steel_1", "hat_Steel_1");
		inappReadableNames.Add("hat_Royal_1", "hat_Royal_1");
		inappReadableNames.Add("hat_Army_2", "hat_Army_2");
		inappReadableNames.Add("hat_Almaz_2", "hat_Almaz_2");
		inappReadableNames.Add("hat_Steel_2", "hat_Steel_2");
		inappReadableNames.Add("hat_Royal_2", "hat_Royal_2");
		inappReadableNames.Add("hat_Army_3", "hat_Army_3");
		inappReadableNames.Add("hat_Almaz_3", "hat_Almaz_3");
		inappReadableNames.Add("hat_Steel_3", "hat_Steel_3");
		inappReadableNames.Add("hat_Royal_3", "hat_Royal_3");
		inappReadableNames.Add("hat_Army_4", "hat_Army_4");
		inappReadableNames.Add("hat_Almaz_4", "hat_Almaz_4");
		inappReadableNames.Add("hat_Steel_4", "hat_Steel_4");
		inappReadableNames.Add("hat_Royal_4", "hat_Royal_4");
		inappReadableNames.Add("hat_Rubin_1", "hat_Rubin_1");
		inappReadableNames.Add("hat_Rubin_2", "hat_Rubin_2");
		inappReadableNames.Add("hat_Rubin_3", "hat_Rubin_3");
		inappReadableNames.Add("Armor_Steel_1", "Armor_Steel_1");
		inappReadableNames.Add("Armor_Steel_2", "Armor_Steel_2");
		inappReadableNames.Add("Armor_Steel_3", "Armor_Steel_3");
		inappReadableNames.Add("Armor_Steel_4", "Armor_Steel_4");
		inappReadableNames.Add("Armor_Royal_1", "Armor_Royal_1");
		inappReadableNames.Add("Armor_Royal_2", "Armor_Royal_2");
		inappReadableNames.Add("Armor_Royal_3", "Armor_Royal_3");
		inappReadableNames.Add("Armor_Royal_4", "Armor_Royal_4");
		inappReadableNames.Add("Armor_Almaz_1", "Armor_Almaz_1");
		inappReadableNames.Add("Armor_Almaz_2", "Armor_Almaz_2");
		inappReadableNames.Add("Armor_Almaz_3", "Armor_Almaz_3");
		inappReadableNames.Add("Armor_Almaz_4", "Armor_Almaz_4");
		inappReadableNames.Add("Armor_Army_1", "Armor_Army_1");
		inappReadableNames.Add("Armor_Army_2", "Armor_Army_2");
		inappReadableNames.Add("Armor_Army_3", "Armor_Army_3");
		inappReadableNames.Add("Armor_Army_4", "Armor_Army_4");
		inappReadableNames.Add("Armor_Novice", "Armor_Novice");
		inappReadableNames.Add("Armor_Rubin_1", "Armor_Rubin_1");
		inappReadableNames.Add("Armor_Rubin_2", "Armor_Rubin_2");
		inappReadableNames.Add("Armor_Rubin_3", "Armor_Rubin_3");
		inappReadableNames.Add("Armor_Adamant_Const_1", "Armor_Adamant_Const_1");
		inappReadableNames.Add("Armor_Adamant_Const_2", "Armor_Adamant_Const_2");
		inappReadableNames.Add("Armor_Adamant_Const_3", "Armor_Adamant_Const_3");
		inappReadableNames.Add("hat_Adamant_Const_1", "hat_Adamant_Const_1");
		inappReadableNames.Add("hat_Adamant_Const_2", "hat_Adamant_Const_2");
		inappReadableNames.Add("hat_Adamant_Const_3", "hat_Adamant_Const_3");
		inappReadableNames.Add("Armor_Adamant_3", "Armor_Adamant_3");
		string[] potions = PotionsController.potions;
		foreach (string text in potions)
		{
			inappReadableNames.Add(text, text);
		}
		inappReadableNames.Add("boots_red", "boots_red");
		inappReadableNames.Add("boots_gray", "boots_gray");
		inappReadableNames.Add("boots_blue", "boots_blue");
		inappReadableNames.Add("boots_green", "boots_green");
		inappReadableNames.Add("boots_black", "boots_black");
		inappReadableNames.Add("boots_tabi", "boots ninja");
		inappReadableNames.Add("HitmanBoots_Up1", "HitmanBoots_Up1");
		inappReadableNames.Add("StormTrooperBoots_Up1", "StormTrooperBoots_Up1");
		inappReadableNames.Add("SniperBoots_Up1", "SniperBoots_Up1");
		inappReadableNames.Add("DemolitionBoots_Up1", "DemolitionBoots_Up1");
		inappReadableNames.Add("BerserkBoots_Up1", "BerserkBoots_Up1");
		inappReadableNames.Add("HitmanBoots_Up2", "HitmanBoots_Up2");
		inappReadableNames.Add("StormTrooperBoots_Up2", "StormTrooperBoots_Up2");
		inappReadableNames.Add("SniperBoots_Up2", "SniperBoots_Up2");
		inappReadableNames.Add("DemolitionBoots_Up2", "DemolitionBoots_Up2");
		inappReadableNames.Add("BerserkBoots_Up2", "BerserkBoots_Up2");
		inappReadableNames.Add("EngineerBoots", "EngineerBoots");
		inappReadableNames.Add("EngineerBoots_Up1", "EngineerBoots_Up1");
		inappReadableNames.Add("EngineerBoots_Up2", "EngineerBoots_Up2");
		foreach (string item in Wear.wear[ShopNGUIController.CategoryNames.MaskCategory].SelectMany((List<string> list) => list))
		{
			if (!(item == "hat_ManiacMask"))
			{
				inappReadableNames.Add(item, item);
			}
		}
		inappReadableNames.Add("hat_ManiacMask", "Maniac Mask");
		for (int n = 0; n < 11; n++)
		{
			inappReadableNames.Add("newskin_" + n, "newskin_" + n);
		}
		for (int num = 11; num < 19; num++)
		{
			inappReadableNames.Add("newskin_" + num, "newskin_" + num);
		}
		inappReadableNames.Add("InvisibilityPotion" + GearManager.UpgradeSuffix + 1, "InvisibilityPotion" + GearManager.UpgradeSuffix + 1);
		inappReadableNames.Add("InvisibilityPotion" + GearManager.UpgradeSuffix + 2, "InvisibilityPotion" + GearManager.UpgradeSuffix + 2);
		inappReadableNames.Add("InvisibilityPotion" + GearManager.UpgradeSuffix + 3, "InvisibilityPotion" + GearManager.UpgradeSuffix + 3);
		inappReadableNames.Add("InvisibilityPotion" + GearManager.UpgradeSuffix + 4, "InvisibilityPotion" + GearManager.UpgradeSuffix + 4);
		inappReadableNames.Add("InvisibilityPotion" + GearManager.UpgradeSuffix + 5, "InvisibilityPotion" + GearManager.UpgradeSuffix + 5);
		inappReadableNames.Add("GrenadeID" + GearManager.UpgradeSuffix + 1, "GrenadeID" + GearManager.UpgradeSuffix + 1);
		inappReadableNames.Add("GrenadeID" + GearManager.UpgradeSuffix + 2, "GrenadeID" + GearManager.UpgradeSuffix + 2);
		inappReadableNames.Add("GrenadeID" + GearManager.UpgradeSuffix + 3, "GrenadeID" + GearManager.UpgradeSuffix + 3);
		inappReadableNames.Add("GrenadeID" + GearManager.UpgradeSuffix + 4, "GrenadeID" + GearManager.UpgradeSuffix + 4);
		inappReadableNames.Add("GrenadeID" + GearManager.UpgradeSuffix + 5, "GrenadeID" + GearManager.UpgradeSuffix + 5);
		inappReadableNames.Add(GearManager.Turret + GearManager.UpgradeSuffix + 1, GearManager.Turret + GearManager.UpgradeSuffix + 1);
		inappReadableNames.Add(GearManager.Turret + GearManager.UpgradeSuffix + 2, GearManager.Turret + GearManager.UpgradeSuffix + 2);
		inappReadableNames.Add(GearManager.Turret + GearManager.UpgradeSuffix + 3, GearManager.Turret + GearManager.UpgradeSuffix + 3);
		inappReadableNames.Add(GearManager.Turret + GearManager.UpgradeSuffix + 4, GearManager.Turret + GearManager.UpgradeSuffix + 4);
		inappReadableNames.Add(GearManager.Turret + GearManager.UpgradeSuffix + 5, GearManager.Turret + GearManager.UpgradeSuffix + 5);
		inappReadableNames.Add(GearManager.Mech + GearManager.UpgradeSuffix + 1, GearManager.Mech + GearManager.UpgradeSuffix + 1);
		inappReadableNames.Add(GearManager.Mech + GearManager.UpgradeSuffix + 2, GearManager.Mech + GearManager.UpgradeSuffix + 2);
		inappReadableNames.Add(GearManager.Mech + GearManager.UpgradeSuffix + 3, GearManager.Mech + GearManager.UpgradeSuffix + 3);
		inappReadableNames.Add(GearManager.Mech + GearManager.UpgradeSuffix + 4, GearManager.Mech + GearManager.UpgradeSuffix + 4);
		inappReadableNames.Add(GearManager.Mech + GearManager.UpgradeSuffix + 5, GearManager.Mech + GearManager.UpgradeSuffix + 5);
		inappReadableNames.Add(GearManager.Jetpack + GearManager.UpgradeSuffix + 1, GearManager.Jetpack + GearManager.UpgradeSuffix + 1);
		inappReadableNames.Add(GearManager.Jetpack + GearManager.UpgradeSuffix + 2, GearManager.Jetpack + GearManager.UpgradeSuffix + 2);
		inappReadableNames.Add(GearManager.Jetpack + GearManager.UpgradeSuffix + 3, GearManager.Jetpack + GearManager.UpgradeSuffix + 3);
		inappReadableNames.Add(GearManager.Jetpack + GearManager.UpgradeSuffix + 4, GearManager.Jetpack + GearManager.UpgradeSuffix + 4);
		inappReadableNames.Add(GearManager.Jetpack + GearManager.UpgradeSuffix + 5, GearManager.Jetpack + GearManager.UpgradeSuffix + 5);
		inappReadableNames.Add("InvisibilityPotion" + GearManager.OneItemSuffix + 0, "InvisibilityPotion" + GearManager.OneItemSuffix + 0);
		inappReadableNames.Add("InvisibilityPotion" + GearManager.OneItemSuffix + 1, "InvisibilityPotion" + GearManager.OneItemSuffix + 1);
		inappReadableNames.Add("InvisibilityPotion" + GearManager.OneItemSuffix + 2, "InvisibilityPotion" + GearManager.OneItemSuffix + 2);
		inappReadableNames.Add("InvisibilityPotion" + GearManager.OneItemSuffix + 3, "InvisibilityPotion" + GearManager.OneItemSuffix + 3);
		inappReadableNames.Add("InvisibilityPotion" + GearManager.OneItemSuffix + 4, "InvisibilityPotion" + GearManager.OneItemSuffix + 4);
		inappReadableNames.Add("InvisibilityPotion" + GearManager.OneItemSuffix + 5, "InvisibilityPotion" + GearManager.OneItemSuffix + 5);
		inappReadableNames.Add("GrenadeID" + GearManager.OneItemSuffix + 0, "GrenadeID" + GearManager.OneItemSuffix + 0);
		inappReadableNames.Add("GrenadeID" + GearManager.OneItemSuffix + 1, "GrenadeID" + GearManager.OneItemSuffix + 1);
		inappReadableNames.Add("GrenadeID" + GearManager.OneItemSuffix + 2, "GrenadeID" + GearManager.OneItemSuffix + 2);
		inappReadableNames.Add("GrenadeID" + GearManager.OneItemSuffix + 3, "GrenadeID" + GearManager.OneItemSuffix + 3);
		inappReadableNames.Add("GrenadeID" + GearManager.OneItemSuffix + 4, "GrenadeID" + GearManager.OneItemSuffix + 4);
		inappReadableNames.Add("GrenadeID" + GearManager.OneItemSuffix + 5, "GrenadeID" + GearManager.OneItemSuffix + 5);
		inappReadableNames.Add(GearManager.Turret + GearManager.OneItemSuffix + 0, GearManager.Turret + GearManager.OneItemSuffix + 0);
		inappReadableNames.Add(GearManager.Turret + GearManager.OneItemSuffix + 1, GearManager.Turret + GearManager.OneItemSuffix + 1);
		inappReadableNames.Add(GearManager.Turret + GearManager.OneItemSuffix + 2, GearManager.Turret + GearManager.OneItemSuffix + 2);
		inappReadableNames.Add(GearManager.Turret + GearManager.OneItemSuffix + 3, GearManager.Turret + GearManager.OneItemSuffix + 3);
		inappReadableNames.Add(GearManager.Turret + GearManager.OneItemSuffix + 4, GearManager.Turret + GearManager.OneItemSuffix + 4);
		inappReadableNames.Add(GearManager.Turret + GearManager.OneItemSuffix + 5, GearManager.Turret + GearManager.OneItemSuffix + 5);
		inappReadableNames.Add(GearManager.Mech + GearManager.OneItemSuffix + 0, GearManager.Mech + GearManager.OneItemSuffix + 0);
		inappReadableNames.Add(GearManager.Mech + GearManager.OneItemSuffix + 1, GearManager.Mech + GearManager.OneItemSuffix + 1);
		inappReadableNames.Add(GearManager.Mech + GearManager.OneItemSuffix + 2, GearManager.Mech + GearManager.OneItemSuffix + 2);
		inappReadableNames.Add(GearManager.Mech + GearManager.OneItemSuffix + 3, GearManager.Mech + GearManager.OneItemSuffix + 3);
		inappReadableNames.Add(GearManager.Mech + GearManager.OneItemSuffix + 4, GearManager.Mech + GearManager.OneItemSuffix + 4);
		inappReadableNames.Add(GearManager.Mech + GearManager.OneItemSuffix + 5, GearManager.Mech + GearManager.OneItemSuffix + 5);
		inappReadableNames.Add(GearManager.Jetpack + GearManager.OneItemSuffix + 0, GearManager.Jetpack + GearManager.OneItemSuffix + 0);
		inappReadableNames.Add(GearManager.Jetpack + GearManager.OneItemSuffix + 1, GearManager.Jetpack + GearManager.OneItemSuffix + 1);
		inappReadableNames.Add(GearManager.Jetpack + GearManager.OneItemSuffix + 2, GearManager.Jetpack + GearManager.OneItemSuffix + 2);
		inappReadableNames.Add(GearManager.Jetpack + GearManager.OneItemSuffix + 3, GearManager.Jetpack + GearManager.OneItemSuffix + 3);
		inappReadableNames.Add(GearManager.Jetpack + GearManager.OneItemSuffix + 4, GearManager.Jetpack + GearManager.OneItemSuffix + 4);
		inappReadableNames.Add(GearManager.Jetpack + GearManager.OneItemSuffix + 5, GearManager.Jetpack + GearManager.OneItemSuffix + 5);
	}
}
