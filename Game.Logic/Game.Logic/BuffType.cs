using System;
namespace Game.Logic
{
	public static class BuffType
	{
		public static readonly int PET_BUFF = 5;
		public static readonly int CARD_BUFF = 4;
		public static readonly int CONSORTIA = 3;
		public static readonly int Pay = 2;
		public static readonly int Local = 1;
		public static readonly int Turn = 0;
		public static readonly int LockState = 1000;
		public static readonly int Tired = 1;
		public static readonly int Firing = 2;
		public static readonly int LockAngel = 3;
		public static readonly int Weakness = 4;
		public static readonly int NoHole = 5;
		public static readonly int Defend = 6;
		public static readonly int Targeting = 7;
		public static readonly int DisenableFly = 8;
		public static readonly int LimitMaxForce = 9;
		public static readonly int ResolveHurt = 10;
		public static readonly int CustomAddGuard = 11;
		public static readonly int AddDamage = 12;
		public static readonly int TurnAddDander = 13;
		public static readonly int AddCritical = 14;
		public static readonly int ExemptEnergy = 15;
		public static readonly int AddDander = 16;
		public static readonly int AddProperty = 17;
		public static readonly int AddMaxBlood = 18;
		public static readonly int ReduceDamage = 19;
		public static readonly int AddPercentDamage = 20;
		public static readonly int SetDefaultDander = 21;
		public static readonly int ReduceContinueDamage = 22;
		public static readonly int DoNotMove = 23;
		public static readonly int AddPercentDefance = 24;
		public static readonly int ReducePoisoning = 25;
		public static readonly int AddBloodGunCount = 26;
		public static readonly int ResistAttack = 27;
		public static readonly int SACRED_BLESSING = 31;
		public static readonly int SACRED_SHIELD = 32;
		public static readonly int NIUTOU = 33;
		public static readonly int INDIAN = 34;
		public static readonly int ConsortionAddBloodGunCount = 101;
		public static readonly int ConsortionAddDamage = 102;
		public static readonly int ConsortionAddCritical = 103;
		public static readonly int ConsortionAddMaxBlood = 104;
		public static readonly int ConsortionAddProperty = 105;
		public static readonly int ConsortionReduceEnergyUse = 106;
		public static readonly int ConsortionAddEnergy = 107;
		public static readonly int ConsortionAddEffectTurn = 108;
		public static readonly int ConsortionAddOfferRate = 109;
		public static readonly int ConsortionAddPercentGoldOrGP = 110;
		public static readonly int ConsortionAddSpellCount = 111;
		public static readonly int ConsortionReduceDander = 112;
		public static readonly int WorldBossHP = 400;
		public static readonly int WorldBossHP_MoneyBuff = 402;
		public static readonly int WorldBossAttrack = 401;
		public static readonly int WorldBossAttrack_MoneyBuff = 403;
		public static readonly int WorldBossMetalSlug = 404;
		public static readonly int WorldBossAncientBlessings = 405;
		public static readonly int WorldBossAddDamage = 406;
		public static bool isContainerBuff()
		{
			return false;
		}
	}
}
