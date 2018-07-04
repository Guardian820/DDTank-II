using Bussiness;
using log4net;
using SqlDataProvider.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
namespace Game.Logic
{
	public class PetMgr
	{
		private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
		private static Dictionary<string, PetConfig> _configs;
		private static Dictionary<int, PetLevel> _levels;
		private static Dictionary<int, PetSkillElementInfo> _skillElements;
		private static Dictionary<int, PetSkillInfo> _skills;
		private static Dictionary<int, PetSkillTemplateInfo> _skillTemplates;
		private static Dictionary<int, PetTemplateInfo> _templateIds;
		private static ReaderWriterLock m_lock;
		private static int MaxStar = 3;
		private static ThreadSafeRandom rand;
		public static bool Init()
		{
			bool result;
			try
			{
				PetMgr._configs = new Dictionary<string, PetConfig>();
				PetMgr._levels = new Dictionary<int, PetLevel>();
				PetMgr._skillElements = new Dictionary<int, PetSkillElementInfo>();
				PetMgr._skills = new Dictionary<int, PetSkillInfo>();
				PetMgr._skillTemplates = new Dictionary<int, PetSkillTemplateInfo>();
				PetMgr._templateIds = new Dictionary<int, PetTemplateInfo>();
				PetMgr.m_lock = new ReaderWriterLock();
				PetMgr.rand = new ThreadSafeRandom();
				result = PetMgr.LoadPetMgr(PetMgr._configs, PetMgr._levels, PetMgr._skillElements, PetMgr._skills, PetMgr._skillTemplates, PetMgr._templateIds);
			}
			catch (Exception exception)
			{
				if (PetMgr.log.IsErrorEnabled)
				{
					PetMgr.log.Error("PetInfoMgr", exception);
				}
				result = false;
			}
			return result;
		}
		public static bool ReLoad()
		{
			try
			{
				Dictionary<string, PetConfig> dictionary = new Dictionary<string, PetConfig>();
				Dictionary<int, PetLevel> dictionary2 = new Dictionary<int, PetLevel>();
				Dictionary<int, PetSkillElementInfo> dictionary3 = new Dictionary<int, PetSkillElementInfo>();
				Dictionary<int, PetSkillInfo> dictionary4 = new Dictionary<int, PetSkillInfo>();
				Dictionary<int, PetSkillTemplateInfo> dictionary5 = new Dictionary<int, PetSkillTemplateInfo>();
				new Dictionary<int, PetTemplateInfo>();
				Dictionary<int, PetTemplateInfo> dictionary6 = new Dictionary<int, PetTemplateInfo>();
				if (PetMgr.LoadPetMgr(dictionary, dictionary2, dictionary3, dictionary4, dictionary5, dictionary6))
				{
					PetMgr.m_lock.AcquireWriterLock(15000);
					try
					{
						PetMgr._configs = dictionary;
						PetMgr._levels = dictionary2;
						PetMgr._skillElements = dictionary3;
						PetMgr._skills = dictionary4;
						PetMgr._skillTemplates = dictionary5;
						PetMgr._templateIds = dictionary6;
						return true;
					}
					catch
					{
					}
					finally
					{
						PetMgr.m_lock.ReleaseWriterLock();
					}
				}
			}
			catch (Exception exception)
			{
				if (PetMgr.log.IsErrorEnabled)
				{
					PetMgr.log.Error("PetMgr", exception);
				}
			}
			return false;
		}
		private static bool LoadPetMgr(Dictionary<string, PetConfig> Config, Dictionary<int, PetLevel> Level, Dictionary<int, PetSkillElementInfo> SkillElement, Dictionary<int, PetSkillInfo> Skill, Dictionary<int, PetSkillTemplateInfo> SkillTemplate, Dictionary<int, PetTemplateInfo> TemplateId)
		{
			using (PlayerBussiness playerBussiness = new PlayerBussiness())
			{
				PetConfig[] allPetConfig = playerBussiness.GetAllPetConfig();
				PetLevel[] allPetLevel = playerBussiness.GetAllPetLevel();
				PetSkillElementInfo[] allPetSkillElementInfo = playerBussiness.GetAllPetSkillElementInfo();
				PetSkillInfo[] allPetSkillInfo = playerBussiness.GetAllPetSkillInfo();
				PetSkillTemplateInfo[] allPetSkillTemplateInfo = playerBussiness.GetAllPetSkillTemplateInfo();
				PetTemplateInfo[] allPetTemplateInfo = playerBussiness.GetAllPetTemplateInfo();
				PetConfig[] array = allPetConfig;
				for (int i = 0; i < array.Length; i++)
				{
					PetConfig petConfig = array[i];
					if (!Config.ContainsKey(petConfig.Name))
					{
						Config.Add(petConfig.Name, petConfig);
					}
				}
				PetLevel[] array2 = allPetLevel;
				for (int j = 0; j < array2.Length; j++)
				{
					PetLevel petLevel = array2[j];
					if (!Level.ContainsKey(petLevel.Level))
					{
						Level.Add(petLevel.Level, petLevel);
					}
				}
				PetSkillElementInfo[] array3 = allPetSkillElementInfo;
				for (int k = 0; k < array3.Length; k++)
				{
					PetSkillElementInfo petSkillElementInfo = array3[k];
					if (!SkillElement.ContainsKey(petSkillElementInfo.ID))
					{
						SkillElement.Add(petSkillElementInfo.ID, petSkillElementInfo);
					}
				}
				PetSkillTemplateInfo[] array4 = allPetSkillTemplateInfo;
				for (int l = 0; l < array4.Length; l++)
				{
					PetSkillTemplateInfo petSkillTemplateInfo = array4[l];
					if (!SkillTemplate.ContainsKey(petSkillTemplateInfo.ID))
					{
						SkillTemplate.Add(petSkillTemplateInfo.ID, petSkillTemplateInfo);
					}
				}
				PetTemplateInfo[] array5 = allPetTemplateInfo;
				for (int m = 0; m < array5.Length; m++)
				{
					PetTemplateInfo petTemplateInfo = array5[m];
					if (!TemplateId.ContainsKey(petTemplateInfo.ID))
					{
						TemplateId.Add(petTemplateInfo.ID, petTemplateInfo);
					}
				}
				PetSkillInfo[] array6 = allPetSkillInfo;
				for (int n = 0; n < array6.Length; n++)
				{
					PetSkillInfo petSkillInfo = array6[n];
					if (!Skill.ContainsKey(petSkillInfo.ID))
					{
						Skill.Add(petSkillInfo.ID, petSkillInfo);
					}
				}
			}
			return true;
		}
		public static PetConfig FindConfig(string key)
		{
			if (PetMgr._configs == null)
			{
				PetMgr.Init();
			}
			PetMgr.m_lock.AcquireReaderLock(15000);
			try
			{
				if (PetMgr._configs.ContainsKey(key))
				{
					return PetMgr._configs[key];
				}
			}
			catch
			{
			}
			finally
			{
				PetMgr.m_lock.ReleaseReaderLock();
			}
			return null;
		}
		public static int GetPetCount()
		{
			if (PetMgr._templateIds == null)
			{
				PetMgr.Init();
			}
			return PetMgr._templateIds.Values.Count;
		}
		public static int GetSkillCount()
		{
			if (PetMgr._skillTemplates == null)
			{
				PetMgr.Init();
			}
			return PetMgr._skillTemplates.Values.Count;
		}
		public static PetLevel FindPetLevel(int level)
		{
			if (PetMgr._levels == null)
			{
				PetMgr.Init();
			}
			PetMgr.m_lock.AcquireReaderLock(15000);
			try
			{
				if (PetMgr._levels.ContainsKey(level))
				{
					return PetMgr._levels[level];
				}
			}
			catch
			{
			}
			finally
			{
				PetMgr.m_lock.ReleaseReaderLock();
			}
			return null;
		}
		public static PetSkillElementInfo FindPetSkillElement(int SkillID)
		{
			if (PetMgr._skillElements == null)
			{
				PetMgr.Init();
			}
			PetMgr.m_lock.AcquireReaderLock(15000);
			try
			{
				if (PetMgr._skillElements.ContainsKey(SkillID))
				{
					return PetMgr._skillElements[SkillID];
				}
			}
			catch
			{
			}
			finally
			{
				PetMgr.m_lock.ReleaseReaderLock();
			}
			return null;
		}
		public static List<PetSkillElementInfo> GameNeedPetSkill()
		{
			if (PetMgr._skillElements == null)
			{
				PetMgr.Init();
			}
			List<PetSkillElementInfo> list = new List<PetSkillElementInfo>();
			Dictionary<string, PetSkillElementInfo> dictionary = new Dictionary<string, PetSkillElementInfo>();
			foreach (PetSkillElementInfo current in PetMgr._skillElements.Values)
			{
				if (!dictionary.Keys.Contains(current.EffectPic) && !string.IsNullOrEmpty(current.EffectPic))
				{
					list.Add(current);
					dictionary.Add(current.EffectPic, current);
				}
			}
			return list;
		}
		public static PetSkillInfo FindPetSkill(int SkillID)
		{
			if (PetMgr._skills == null)
			{
				PetMgr.Init();
			}
			PetMgr.m_lock.AcquireReaderLock(15000);
			try
			{
				if (PetMgr._skills.ContainsKey(SkillID))
				{
					return PetMgr._skills[SkillID];
				}
			}
			catch
			{
			}
			finally
			{
				PetMgr.m_lock.ReleaseReaderLock();
			}
			return null;
		}
		public static PetSkillTemplateInfo GetPetSkillTemplate(int ID)
		{
			if (PetMgr._skillTemplates == null)
			{
				PetMgr.Init();
			}
			PetMgr.m_lock.AcquireReaderLock(15000);
			try
			{
				if (PetMgr._skillTemplates.ContainsKey(ID))
				{
					return PetMgr._skillTemplates[ID];
				}
			}
			catch
			{
			}
			finally
			{
				PetMgr.m_lock.ReleaseReaderLock();
			}
			return null;
		}
		public static PetTemplateInfo FindPetTemplate(int TemplateID)
		{
			for (int i = 1; i <= PetMgr.GetPetCount(); i++)
			{
				if (PetMgr.FindPetTemplateById(i).TemplateID == TemplateID)
				{
					return PetMgr.FindPetTemplateById(i);
				}
			}
			return null;
		}
		public static PetTemplateInfo FindPetTemplateById(int ID)
		{
			if (PetMgr._templateIds == null)
			{
				PetMgr.Init();
			}
			PetMgr.m_lock.AcquireReaderLock(15000);
			try
			{
				if (PetMgr._templateIds.ContainsKey(ID))
				{
					return PetMgr._templateIds[ID];
				}
			}
			catch
			{
			}
			finally
			{
				PetMgr.m_lock.ReleaseReaderLock();
			}
			return null;
		}
		public static List<int> GetPetTemplateByStar(int star)
		{
			List<int> list = new List<int>();
			int num = 0;
			for (int i = 1; i <= PetMgr.GetPetCount(); i++)
			{
				PetTemplateInfo petTemplateInfo = PetMgr.FindPetTemplateById(i);
				if (petTemplateInfo.StarLevel <= star)
				{
					int num2 = petTemplateInfo.TemplateID - num;
					if (num2 != 1 && num2 != -1 && num2 != 2 && num2 != -2)
					{
						list.Add(petTemplateInfo.TemplateID);
					}
					num = petTemplateInfo.TemplateID;
				}
			}
			return list;
		}
		public static List<int> GetPetSkillByKindID(int KindID, int lv)
		{
			int num = Convert.ToInt32(PetMgr.FindConfig("MaxLevel").Value);
			List<int> list = new List<int>();
			List<string> list2 = new List<string>();
			PetSkillTemplateInfo[] petSkillByKindID = PetMgr.GetPetSkillByKindID(KindID);
			int num2 = (lv > num) ? num : lv;
			for (int i = 1; i <= num2; i++)
			{
				PetSkillTemplateInfo[] array = petSkillByKindID;
				for (int j = 0; j < array.Length; j++)
				{
					PetSkillTemplateInfo petSkillTemplateInfo = array[j];
					if (petSkillTemplateInfo.MinLevel == i)
					{
						string[] array2 = petSkillTemplateInfo.DeleteSkillIDs.Split(new char[]
						{
							','
						});
						string[] array3 = array2;
						for (int k = 0; k < array3.Length; k++)
						{
							string item = array3[k];
							list2.Add(item);
						}
						list.Add(petSkillTemplateInfo.SkillID);
					}
				}
			}
			foreach (string current in list2)
			{
				if (!string.IsNullOrEmpty(current))
				{
					int item2 = int.Parse(current);
					list.Remove(item2);
				}
			}
			list.Sort();
			return list;
		}
		public static PetSkillTemplateInfo[] GetPetSkillByKindID(int KindID)
		{
			List<PetSkillTemplateInfo> list = new List<PetSkillTemplateInfo>();
			for (int i = 1; i <= PetMgr.GetSkillCount(); i++)
			{
				PetSkillTemplateInfo petSkillTemplate = PetMgr.GetPetSkillTemplate(i);
				if (petSkillTemplate.KindID == KindID)
				{
					list.Add(petSkillTemplate);
				}
			}
			return list.ToArray();
		}
		public static List<UsersPetinfo> CreateAdoptList(int userID)
		{
			int num = Convert.ToInt32(PetMgr.FindConfig("AdoptCount").Value);
			List<int> petTemplateByStar = PetMgr.GetPetTemplateByStar(PetMgr.MaxStar);
			List<UsersPetinfo> list = new List<UsersPetinfo>();
			using (PlayerBussiness playerBussiness = new PlayerBussiness())
			{
				playerBussiness.RemoveUserAdoptPet(userID);
				for (int i = 0; i < num; i++)
				{
					int index = PetMgr.rand.Next(petTemplateByStar.Count);
					PetTemplateInfo info = PetMgr.FindPetTemplate(petTemplateByStar[index]);
					UsersPetinfo usersPetinfo = PetMgr.CreatePet(info, userID, i);
					usersPetinfo.IsExit = true;
					playerBussiness.AddUserAdoptPet(usersPetinfo, false);
					list.Add(usersPetinfo);
				}
			}
			return list;
		}
		public static List<UsersPetinfo> CreateFirstAdoptList(int userID)
		{
			List<int> list = new List<int>
			{
				100601,
				110601,
				120601,
				130601
			};
			List<UsersPetinfo> list2 = new List<UsersPetinfo>();
			using (PlayerBussiness playerBussiness = new PlayerBussiness())
			{
				playerBussiness.RemoveUserAdoptPet(userID);
				for (int i = 0; i < list.Count; i++)
				{
					PetTemplateInfo info = PetMgr.FindPetTemplate(list[i]);
					UsersPetinfo usersPetinfo = PetMgr.CreatePet(info, userID, i);
					usersPetinfo.IsExit = true;
					playerBussiness.AddUserAdoptPet(usersPetinfo, false);
					list2.Add(usersPetinfo);
				}
			}
			return list2;
		}
		public static string ActiveEquipSkill(int Level)
		{
			string text = "0,0";
			int num = 1;
			if (Level >= 20 && Level < 30)
			{
				num++;
			}
			if (Level >= 30 && Level < 50)
			{
				num += 2;
			}
			if (Level >= 50 && Level < 60)
			{
				num += 3;
			}
			if (Level == 60)
			{
				num += 4;
			}
			for (int i = 1; i < num; i++)
			{
				text = text + "|0," + i;
			}
			return text;
		}
		public static int UpdateEvolution(int TemplateID, int lv)
		{
			int num = TemplateID;
			int num2 = Convert.ToInt32(PetMgr.FindConfig("EvolutionLevel1").Value);
			int num3 = Convert.ToInt32(PetMgr.FindConfig("EvolutionLevel2").Value);
			PetTemplateInfo petTemplateInfo = PetMgr.FindPetTemplate(num);
			PetTemplateInfo petTemplateInfo2 = PetMgr.FindPetTemplate(num + 1);
			PetTemplateInfo petTemplateInfo3 = PetMgr.FindPetTemplate(num + 2);
			if (petTemplateInfo3 != null)
			{
				if (lv >= num2 && lv < num3 && petTemplateInfo2.EvolutionID != 0)
				{
					num = petTemplateInfo.EvolutionID;
				}
				else
				{
					if (lv >= num3)
					{
						num = petTemplateInfo2.EvolutionID;
					}
				}
			}
			else
			{
				if (petTemplateInfo2 != null && lv >= num3)
				{
					num = petTemplateInfo.EvolutionID;
				}
			}
			return num;
		}
		public static int TemplateReset(int TemplateID)
		{
			int num = TemplateID;
			PetTemplateInfo petTemplateInfo = PetMgr.FindPetTemplate(num - 1);
			PetTemplateInfo petTemplateInfo2 = PetMgr.FindPetTemplate(num - 2);
			if (petTemplateInfo != null)
			{
				num = petTemplateInfo.TemplateID;
			}
			else
			{
				if (petTemplateInfo2 != null)
				{
					num = petTemplateInfo2.TemplateID;
				}
			}
			return num;
		}
		public static string UpdateSkillPet(int Level, int TemplateID)
		{
			PetTemplateInfo petTemplateInfo = PetMgr.FindPetTemplate(TemplateID);
			if (petTemplateInfo == null)
			{
				PetMgr.log.Error("Pet not found: " + TemplateID);
				return "";
			}
			List<int> petSkillByKindID = PetMgr.GetPetSkillByKindID(petTemplateInfo.KindID, Level);
			string text = petSkillByKindID[0] + ",0";
			for (int i = 1; i < petSkillByKindID.Count; i++)
			{
				object obj = text;
				text = string.Concat(new object[]
				{
					obj,
					"|",
					petSkillByKindID[i],
					",",
					i
				});
			}
			return text;
		}
		public static int GetLevel(int GP)
		{
			int num = Convert.ToInt32(PetMgr.FindConfig("MaxLevel").Value);
			if (GP >= PetMgr.FindPetLevel(num).GP)
			{
				return num;
			}
			int i = 1;
			while (i <= num)
			{
				if (GP < PetMgr.FindPetLevel(i).GP)
				{
					if (i - 1 != 0)
					{
						return i - 1;
					}
					return 1;
				}
				else
				{
					i++;
				}
			}
			return 1;
		}
		public static int GetGP(int level)
		{
			int num = Convert.ToInt32(PetMgr.FindConfig("MaxLevel").Value);
			for (int i = 1; i <= num; i++)
			{
				if (level == PetMgr.FindPetLevel(i).Level)
				{
					return PetMgr.FindPetLevel(i).GP;
				}
			}
			return 0;
		}
		public static void PlusPetProp(UsersPetinfo pet, int min, int max, ref int blood, ref int attack, ref int defence, ref int agility, ref int lucky)
		{
			double num = (double)(pet.BloodGrow / 10) * 0.1;
			double num2 = (double)(pet.AttackGrow / 10) * 0.1;
			double num3 = (double)(pet.DefenceGrow / 10) * 0.1;
			double num4 = (double)(pet.AgilityGrow / 10) * 0.1;
			double num5 = (double)(pet.LuckGrow / 10) * 0.1;
			double num6 = 0.0;
			double num7 = (double)pet.Blood;
			double num8 = (double)pet.Attack;
			double num9 = (double)pet.Defence;
			double num10 = (double)pet.Agility;
			double num11 = (double)pet.Luck;
			for (int i = min + 1; i <= max; i++)
			{
				num6 += (double)(min / 100);
				double x = 0.5;
				num7 += num + Math.Pow(x, (double)i);
				num8 += num2 + Math.Pow(x, (double)i);
				num9 += num3 + Math.Pow(x, (double)i);
				num10 += num4 + Math.Pow(x, (double)i);
				num11 += num5 + Math.Pow(x, (double)i);
			}
			blood = (int)(num * (num7 / (num + num6)));
			attack = (int)(num2 * (num8 / (num2 + num6)));
			defence = (int)(num3 * (num9 / (num3 + num6)));
			agility = (int)(num4 * (num10 / (num4 + num6)));
			lucky = (int)(num5 * (num11 / (num5 + num6)));
		}
		public static UsersPetinfo CreatePet(PetTemplateInfo info, int userID, int place)
		{
			UsersPetinfo usersPetinfo = new UsersPetinfo();
			int starLevel = info.StarLevel;
			int minValue = 200 + 100 * starLevel;
			int maxValue = 350 + 100 * starLevel;
			int minValue2 = 1700 + 1000 * starLevel;
			int maxValue2 = 2200 + 2500 * starLevel;
			usersPetinfo.BloodGrow = PetMgr.rand.Next(minValue2, maxValue2);
			usersPetinfo.AttackGrow = PetMgr.rand.Next(minValue, maxValue);
			usersPetinfo.DefenceGrow = PetMgr.rand.Next(minValue, maxValue);
			usersPetinfo.AgilityGrow = PetMgr.rand.Next(minValue, maxValue);
			usersPetinfo.LuckGrow = PetMgr.rand.Next(minValue, maxValue);
			usersPetinfo.DamageGrow = 0;
			usersPetinfo.GuardGrow = 0;
			double num = (double)PetMgr.rand.Next(54, 61) * 0.1;
			double num2 = (double)PetMgr.rand.Next(9, 13) * 0.1;
			usersPetinfo.Blood = (int)((double)(PetMgr.rand.Next(minValue2, maxValue2) / 10) * 0.1 * num);
			usersPetinfo.Attack = (int)((double)(PetMgr.rand.Next(minValue, maxValue) / 10) * 0.1 * num2);
			usersPetinfo.Defence = (int)((double)(PetMgr.rand.Next(minValue, maxValue) / 10) * 0.1 * num2);
			usersPetinfo.Agility = (int)((double)(PetMgr.rand.Next(minValue, maxValue) / 10) * 0.1 * num2);
			usersPetinfo.Luck = (int)((double)(PetMgr.rand.Next(minValue, maxValue) / 10) * 0.1 * num2);
			usersPetinfo.Damage = 0;
			usersPetinfo.Guard = 0;
			usersPetinfo.Hunger = 10000;
			usersPetinfo.TemplateID = info.TemplateID;
			usersPetinfo.Name = info.Name;
			usersPetinfo.UserID = userID;
			usersPetinfo.Place = place;
			usersPetinfo.Level = 1;
			usersPetinfo.Skill = PetMgr.UpdateSkillPet(1, info.TemplateID);
			usersPetinfo.SkillEquip = PetMgr.ActiveEquipSkill(1);
			return usersPetinfo;
		}
	}
}
