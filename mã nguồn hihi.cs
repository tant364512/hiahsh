using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using AssemblyCSharp.Mod.Xmap;

// Token: 0x0200000F RID: 15
internal class AutoBroly
{
	// Token: 0x0600004E RID: 78
	private static void Wait(int time)
	{
		AutoBroly.IsWait = true;
		AutoBroly.TimeStartWait = mSystem.currentTimeMillis();
		AutoBroly.TimeWait = (long)time;
	}

	// Token: 0x0600004F RID: 79
	private static bool IsWaiting()
	{
		if (AutoBroly.IsWait && mSystem.currentTimeMillis() - AutoBroly.TimeStartWait >= AutoBroly.TimeWait)
		{
			AutoBroly.IsWait = false;
		}
		return AutoBroly.IsWait;
	}

	// Token: 0x06000050 RID: 80
	public static bool IsBoss()
	{
		for (int i = 0; i < GameScr.vCharInMap.size(); i++)
		{
			global::Char @char = (global::Char)GameScr.vCharInMap.elementAt(i);
			if (@char != null && @char.cName.Contains("Broly") && @char.cName.Contains("Super") && @char.cHPFull >= 16070777L)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06000051 RID: 81
	public static void SearchBoss()
	{
		int currentZone = TileMap.zoneID;
		int count = GameScr.gI().zones.Length;
		if (AutoBroly.IsBoss())
		{
			AutoBroly.visitedZones.Clear();
			return;
		}
		AutoBroly.visitedZones.Add(currentZone);
		List<int> list = (from z in Enumerable.Range(0, count)
		where z != currentZone && !AutoBroly.visitedZones.Contains(z)
		select z).ToList<int>();
		if (list.Count == 0)
		{
			AutoBroly.visitedZones.Clear();
			return;
		}
		int zoneId = list[AutoBroly.random.Next(list.Count)];
		Service.gI().requestChangeZone(zoneId, -1);
	}

	// Token: 0x06000052 RID: 82
	public static void FocusSuperBroly()
	{
		for (int i = 0; i < GameScr.vCharInMap.size(); i++)
		{
			global::Char @char = (global::Char)GameScr.vCharInMap.elementAt(i);
			if (@char != null && @char.cName.Contains("Broly") && @char.cName.Contains("Super") && @char.cHP > 0L && global::Char.myCharz().charFocus != @char)
			{
				global::Char.myCharz().npcFocus = null;
				global::Char.myCharz().mobFocus = null;
				global::Char.myCharz().charFocus = @char;
				return;
			}
		}
	}

	// Token: 0x06000053 RID: 83
	public static void Update()
	{
		if ((DataAccount.Type == 2 || DataAccount.Type == 3) && !AutoBroly.IsBoss())
		{
			AutoBroly.RemoveZoneOwner(TileMap.mapID, TileMap.zoneID);
		}
		if (DataAccount.Type == 2 && AutoBroly.IsBoss())
		{
			for (int i = 0; i < GameScr.vCharInMap.size(); i++)
			{
				global::Char @char = (global::Char)GameScr.vCharInMap.elementAt(i);
				if (@char != null && @char.cName.Contains("Broly") && @char.cName.Contains("Super") && ((@char.cHPFull < 16077777L && @char.cHP >= 1L) || @char.cHP <= 0L))
				{
					AutoBroly.OutAndCleanZone(TileMap.mapID, TileMap.zoneID);
					return;
				}
			}
		}
		if ((DataAccount.Type == 2 || DataAccount.Type == 3) && AutoBroly.IsBoss())
		{
			for (int j = 0; j < GameScr.vCharInMap.size(); j++)
			{
				global::Char char2 = (global::Char)GameScr.vCharInMap.elementAt(j);
				if (char2 != null && char2.cName.Contains("Broly") && char2.cName.Contains("Super") && ((char2.cHPFull < 16077777L && char2.cHP >= 1L) || char2.cHP <= 0L))
				{
					AutoBroly.RemoveZoneOwner(TileMap.mapID, TileMap.zoneID);
					AutoBroly.RemoveZoneControlled(TileMap.mapID, TileMap.zoneID);
					AutoBroly.AddToBlacklist(TileMap.mapID, TileMap.zoneID);
					string path = "C:\\Users\\Admin\\Desktop\\QLTK - SP ver 008\\Nro_244_Data\\Resources\\Data\\zone_tracking.txt";
					string zoneTrackKey = string.Format("{0}:{1}:{2}", TileMap.mapID, TileMap.zoneID, DataAccount.Account);
					if (File.Exists(path))
					{
						string[] source = File.ReadAllLines(path);
						File.WriteAllLines(path, (from line in source
						where line != zoneTrackKey
						select line).ToArray<string>());
					}
					AutoBroly.Map = -1;
					AutoBroly.Khu = -1;
					Service.gI().requestChangeZone(0, -1);
					return;
				}
			}
		}
		if (DataAccount.Type == 3)
		{
			if (AutoBroly.IsInHome())
			{
				if (AutoBroly.GetSoLuongDau() <= 20)
				{
					AutoPean.AutoThuDau();
				}
			}
			else
			{
				long cHP = global::Char.myCharz().cHP;
				long cHPFull = global::Char.myCharz().cHPFull;
				long cMP = global::Char.myCharz().cMP;
				long cMPFull = global::Char.myCharz().cMPFull;
				if ((cHPFull > 0L && cHP * 100L / cHPFull <= 5L) || (cMPFull > 0L && cMP * 100L / cMPFull <= 5L))
				{
					AutoBroly.AnDau();
				}
			}
		}
		if ((DataAccount.Type == 2 || DataAccount.Type == 3) && AutoBroly.IsBoss())
		{
			for (int k = 0; k < GameScr.vCharInMap.size(); k++)
			{
				global::Char char3 = (global::Char)GameScr.vCharInMap.elementAt(k);
				if (char3 != null && char3.cName.Contains("Broly") && char3.cName.Contains("Super") && ((char3.cHPFull <= 16077777L && char3.cHP >= 1L) || char3.cHP <= 0L))
				{
					AutoBroly.RemoveZoneOwner(TileMap.mapID, TileMap.zoneID);
					AutoBroly.RemoveZoneControlled(TileMap.mapID, TileMap.zoneID);
					AutoBroly.AddToBlacklist(TileMap.mapID, TileMap.zoneID);
					string path2 = "C:\\Users\\Admin\\Desktop\\QLTK - SP ver 008\\Nro_244_Data\\Resources\\Data\\zone_tracking.txt";
					string zoneTrackKey = string.Format("{0}:{1}:{2}", TileMap.mapID, TileMap.zoneID, DataAccount.Account);
					if (File.Exists(path2))
					{
						string[] source2 = File.ReadAllLines(path2);
						File.WriteAllLines(path2, (from line in source2
						where line != zoneTrackKey
						select line).ToArray<string>());
					}
					AutoBroly.Map = -1;
					AutoBroly.Khu = -1;
					Service.gI().requestChangeZone(0, -1);
					return;
				}
			}
		}
		if (DataAccount.Type == 3 && File.Exists(AutoBroly.SuperBrolyCallPath))
		{
			string[] array = File.ReadAllText(AutoBroly.SuperBrolyCallPath).Split(new char[]
			{
				':'
			});
			if (array.Length == 3)
			{
				int mapId = int.Parse(array[0]);
				int zoneId = int.Parse(array[1]);
				if (array[2] == DataAccount.Account)
				{
					AutoBroly.MoveToMapAndZone(mapId, zoneId);
					return;
				}
				return;
			}
		}
		if (DataAccount.Type == 1 && AutoBroly.IsBoss())
		{
			for (int l = 0; l < GameScr.vCharInMap.size(); l++)
			{
				global::Char char4 = (global::Char)GameScr.vCharInMap.elementAt(l);
				if (char4 != null && char4.cName.Contains("Broly") && char4.cName.Contains("Super") && char4.cHPFull >= 16077777L && char4.cHP > 0L)
				{
					AutoBroly.CallType3(TileMap.mapID, TileMap.zoneID, "ten_acc_type3");
					break;
				}
			}
		}
		if (DataAccount.Type == 3 && !AutoBroly.IsBoss())
		{
			AutoBroly.RemoveZoneOwner(TileMap.mapID, TileMap.zoneID);
			AutoBroly.RemoveZoneControlled(TileMap.mapID, TileMap.zoneID);
		}
		if (AutoBroly.Map != -1 && AutoBroly.Khu != -1 && TileMap.mapID == AutoBroly.Map && TileMap.zoneID == AutoBroly.Khu && !AutoBroly.IsBoss())
		{
			AutoBroly.Map = -1;
			AutoBroly.Khu = -1;
		}
		if (!AutoBroly.IsWaiting())
		{
			if (DataAccount.Type == 3)
			{
				AutoBroly.TryCheckSuperBroly();
			}
			if (global::Char.myCharz().cHP <= 0L || global::Char.myCharz().meDead)
			{
				if (AutoBroly.IsBoss() && DataAccount.Type != 1)
				{
					AutoBroly.Map = TileMap.mapID;
					AutoBroly.Khu = TileMap.zoneID;
				}
				Service.gI().returnTownFromDead();
				AutoBroly.Wait(3000);
				return;
			}
			if (AutoBroly.Map != -1 && AutoBroly.Khu != -1 && TileMap.mapID == AutoBroly.Map && TileMap.zoneID == AutoBroly.Khu && !AutoBroly.IsBoss())
			{
				AutoBroly.Map = -1;
				AutoBroly.Khu = -1;
			}
			if (AutoBroly.IsBoss())
			{
				if (DataAccount.Type != 1)
				{
					AutoBroly.Map = TileMap.mapID;
					AutoBroly.Khu = TileMap.zoneID;
				}
				AutoBroly.TrangThai = "SP: " + TileMap.mapNames[TileMap.mapID].ToString() + " - " + TileMap.zoneID.ToString();
				if (AutoBroly.visitedZones.Count > 0)
				{
					AutoBroly.visitedZones.Clear();
				}
			}
			else
			{
				AutoBroly.TrangThai = "Không có thông tin ";
			}
			if (AutoBroly.Map != -1 && TileMap.mapID != AutoBroly.Map && !Pk9rXmap.IsXmapRunning)
			{
				XmapController.StartRunToMapId(AutoBroly.Map);
				AutoBroly.Wait(3000);
				return;
			}
			if (TileMap.mapID == AutoBroly.Map && TileMap.zoneID != AutoBroly.Khu && AutoBroly.Khu != -1)
			{
				Service.gI().requestChangeZone(AutoBroly.Khu, -1);
				AutoBroly.Wait(2000);
				return;
			}
			if (TileMap.mapID == AutoBroly.Map && TileMap.zoneID == AutoBroly.Khu && AutoBroly.IsBoss())
			{
				AutoBroly.FocusSuperBroly();
			}
			if (!AutoBroly.IsBoss() && AutoBroly.isDoKhu)
			{
				AutoBroly.SearchBoss();
				AutoBroly.Wait(2000);
				return;
			}
			if (DataAccount.Type == 1 && !AutoBroly.IsBoss() && AutoBroly.NhayNe == 1)
			{
				AutoBroly.NhayNe = 0;
			}
			if (DataAccount.Type == 3)
			{
				if (AutoBroly.NhayNe == 0 && !AutoBroly.IsBoss())
				{
					AutoBroly.NhayNe = 1;
					AutoBroly.NhayCuoiMap();
					AutoBroly.Wait(1000);
					return;
				}
				if (!AutoBroly.IsBoss() && AutoBroly.NhayNe == 1)
				{
					AutoBroly.NhayNe = 0;
				}
			}
			if (DataAccount.Type == 1)
			{
				AutoBroly.AvoidNormalBroly();
			}
			AutoBroly.Wait(500);
		}
	}

	// Token: 0x06000054 RID: 84
	public static void NhayCuoiMap()
	{
		if (GameScr.getX(2) > 0 && GameScr.getY(2) > 0)
		{
			KsSupper.TelePortTo(GameScr.getX(2) - 50, GameScr.getY(2));
		}
	}

	// Token: 0x06000055 RID: 85
	public static bool IsBroly()
	{
		for (int i = 0; i < GameScr.vCharInMap.size(); i++)
		{
			global::Char @char = (global::Char)GameScr.vCharInMap.elementAt(i);
			if (@char != null && @char.cName.Contains("Broly") && !@char.cName.Contains("Super"))
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06000056 RID: 86
	public static void Painting(mGraphics g)
	{
		string str = TileMap.mapNames[TileMap.mapID];
		string str2 = " - " + TileMap.zoneID.ToString();
		string str3 = NinjaUtil.getMoneys(global::Char.myCharz().cHP).ToString() + " / " + NinjaUtil.getMoneys(global::Char.myCharz().cHPFull).ToString();
		string str4 = NinjaUtil.getMoneys(global::Char.myCharz().cMP).ToString() + " / " + NinjaUtil.getMoneys(global::Char.myCharz().cMPFull).ToString();
		if (AutoBroly.IsBoss())
		{
			for (int i = 0; i < GameScr.vCharInMap.size(); i++)
			{
				global::Char @char = (global::Char)GameScr.vCharInMap.elementAt(i);
				if (@char != null && @char.cName.Contains("Broly") && @char.cName.Contains("Super") && @char.cHPFull >= 16070777L)
				{
					string st = string.Concat(new string[]
					{
						@char.cName,
						" [ ",
						NinjaUtil.getMoneys(@char.cHP).ToString(),
						" / ",
						NinjaUtil.getMoneys(@char.cHPFull).ToString(),
						" ]"
					});
					mFont.tahoma_7b_yellow.drawString(g, st, 20, GameCanvas.h - (GameCanvas.h - GameCanvas.h / 3), 0);
				}
			}
		}
		if (AutoBroly.IsBroly())
		{
			for (int j = 0; j < GameScr.vCharInMap.size(); j++)
			{
				global::Char char2 = (global::Char)GameScr.vCharInMap.elementAt(j);
				if (char2 != null && char2.cName.Contains("Broly") && !char2.cName.Contains("Super"))
				{
					string st2 = string.Concat(new string[]
					{
						char2.cName,
						" [ ",
						NinjaUtil.getMoneys(char2.cHP).ToString(),
						" / ",
						NinjaUtil.getMoneys(char2.cHPFull).ToString(),
						" ]"
					});
					mFont.tahoma_7b_white.drawString(g, st2, 20, GameCanvas.h - (GameCanvas.h - GameCanvas.h / 3), 0);
				}
			}
		}
		mFont.tahoma_7b_white.drawString(g, "HP: " + str3, 30, GameCanvas.h - (GameCanvas.h - 25), 0);
		mFont.tahoma_7b_white.drawString(g, "MP: " + str4, 30, GameCanvas.h - (GameCanvas.h - 35), 0);
		mFont.tahoma_7b_white.drawString(g, str + " " + str2 + " ", 30, GameCanvas.h - (GameCanvas.h - 10), 0);
		mFont.tahoma_7b_white.drawString(g, AutoBroly.Map.ToString() + " " + AutoBroly.Khu.ToString() + " ", GameCanvas.w - 30, GameCanvas.h - (GameCanvas.h - 10), 0);
	}

	// Token: 0x06000058 RID: 88
	static AutoBroly()
	{
		AutoBroly.SuperBrolyCallPath = "C:\\Users\\Admin\\Desktop\\QLTK - SP ver 008\\Nro_244_Data\\Resources\\Data\\super_broly_call.txt";
		AutoBroly.SuperBrolyInfoPath = "C:\\Users\\Admin\\Desktop\\QLTK - SP ver 008\\Nro_244_Data\\Resources\\Data\\super_broly_info.txt";
		AutoBroly.SuperBrolyOwnerPath = "C:\\Users\\Admin\\Desktop\\QLTK - SP ver 008\\Nro_244_Data\\Resources\\Data\\super_broly_owners.txt";
		AutoBroly.SuperBrolyBlacklistPath = "C:\\Users\\Admin\\Desktop\\QLTK - SP ver 008\\Nro_244_Data\\Resources\\Data\\super_broly_blacklist.txt";
		AutoBroly.TrangThai = "Không có thông tin";
		AutoBroly.Map = -1;
		AutoBroly.Khu = -1;
		AutoBroly.isDoKhu = false;
		AutoBroly.visitedZones = new HashSet<int>();
		AutoBroly.random = new Random();
		AutoBroly.NhayNe = 0;
		try
		{
			if (DataAccount.Type == 3)
			{
				foreach (string path in new string[]
				{
					AutoBroly.AreaControlPath,
					AutoBroly.SuperBrolyBlacklistPath,
					AutoBroly.SuperBrolyOwnerPath,
					AutoBroly.SuperBrolyInfoPath,
					AutoBroly.SuperBrolyCallPath,
					"C:\\Users\\Admin\\Desktop\\QLTK - SP ver 008\\Nro_244_Data\\Resources\\Data\\zone_tracking.txt",
					AutoBroly.BlacklistPath
				})
				{
					try
					{
						if (!File.Exists(path))
						{
							File.WriteAllText(path, string.Empty);
						}
					}
					catch
					{
					}
				}
			}
		}
		catch
		{
		}
	}

	// Token: 0x06000059 RID: 89
	private static double MySqrt(double x)
	{
		if (x < 0.0)
		{
			return 0.0;
		}
		double num = x;
		double num2;
		do
		{
			num2 = num;
			num = (num + x / num) / 2.0;
		}
		while (System.Math.Abs(num - num2) > 0.0001);
		return num;
	}

	// Token: 0x0600005A RID: 90
	private static int MyMin(int a, int b)
	{
		if (a >= b)
		{
			return b;
		}
		return a;
	}

	// Token: 0x0600005B RID: 91
	private static int MyMax(int a, int b)
	{
		if (a <= b)
		{
			return b;
		}
		return a;
	}

	// Token: 0x0600005C RID: 92
	private static void AvoidNormalBroly()
	{
		if (DataAccount.Type != 1)
		{
			return;
		}
		if (!AutoBroly.IsBroly())
		{
			return;
		}
		global::Char @char = global::Char.myCharz();
		int pxw = TileMap.pxw;
		int num = 30;
		for (int i = 0; i < GameScr.vCharInMap.size(); i++)
		{
			global::Char char2 = (global::Char)GameScr.vCharInMap.elementAt(i);
			if (char2 != null && char2.cName.Contains("Broly") && !char2.cName.Contains("Super") && char2.cHP > 0L)
			{
				int num5 = @char.cx - char2.cx;
				int num2 = @char.cy - char2.cy;
				double num3 = AutoBroly.MySqrt((double)(num5 * num5 + num2 * num2));
				long num4 = mSystem.currentTimeMillis();
				if (num3 <= 100.0 && num4 - AutoBroly.lastAvoidBrolyTime >= 500L)
				{
					int x = @char.cx;
					if (@char.cx <= num)
					{
						x = pxw - num;
					}
					else if (@char.cx >= pxw - num)
					{
						x = num;
					}
					else if (@char.cx < char2.cx)
					{
						x = AutoBroly.MyMax(@char.cx - (int)(100.0 - num3), num);
					}
					else
					{
						x = AutoBroly.MyMin(@char.cx + (int)(100.0 - num3), pxw - num);
					}
					KsSupper.TelePortTo(x, @char.cy);
					AutoBroly.lastAvoidBrolyTime = num4;
					return;
				}
			}
		}
	}

	// Token: 0x0600005D RID: 93
	public static void TryCheckSuperBroly()
	{
		if (DataAccount.Type != 3)
		{
			return;
		}
		string superBrolyInfoPath = AutoBroly.SuperBrolyInfoPath;
		if (!File.Exists(superBrolyInfoPath))
		{
			return;
		}
		string[] array5 = File.ReadAllText(superBrolyInfoPath).Split(new char[]
		{
			'|'
		});
		int mapId = 0;
		int num = 0;
		List<int> source = new List<int>();
		List<int> checkedZones = new List<int>();
		foreach (string text in array5)
		{
			if (text.StartsWith("mapId:"))
			{
				mapId = int.Parse(text.Substring(6));
			}
			if (text.StartsWith("superCount:"))
			{
				num = int.Parse(text.Substring(11));
			}
			if (text.StartsWith("zones:"))
			{
				source = text.Substring(6).Split(new char[]
				{
					','
				}, StringSplitOptions.RemoveEmptyEntries).Select(new Func<string, int>(int.Parse)).ToList<int>();
			}
			if (text.StartsWith("checkedZones:"))
			{
				checkedZones = text.Substring(13).Split(new char[]
				{
					','
				}, StringSplitOptions.RemoveEmptyEntries).Select(new Func<string, int>(int.Parse)).ToList<int>();
			}
		}
		string zoneTrackingPath = "C:\\Users\\Admin\\Desktop\\QLTK - SP ver 008\\Nro_244_Data\\Resources\\Data\\zone_tracking.txt";
		string path = "C:\\Users\\Admin\\Desktop\\QLTK - SP ver 008\\Nro_244_Data\\Resources\\Data\\super_broly_found.txt";
		checkedZones = (from z in checkedZones
		where source.Contains(z)
		select z).ToList<int>();
		if (File.Exists(zoneTrackingPath) && !File.ReadAllLines(zoneTrackingPath).Any((string line) => line.StartsWith(mapId + ":")))
		{
			File.WriteAllText(zoneTrackingPath, string.Empty);
		}
		if (File.Exists(path) && !File.ReadAllText(path).StartsWith(mapId + ":"))
		{
			File.WriteAllText(path, string.Empty);
		}
		int num2 = AutoBroly.CountSuperBrolyInMap();
		File.WriteAllText(path, string.Format("{0}:{1}", mapId, num2));
		if (num2 >= num)
		{
			if (File.Exists(AutoBroly.SuperBrolyInfoPath))
			{
				File.Delete(AutoBroly.SuperBrolyInfoPath);
			}
			return;
		}
		List<int> list = (from z in source
		where !checkedZones.Contains(z) && !AutoBroly.IsBlacklisted(mapId, z) && !AutoBroly.IsZoneOwned(mapId, z)
		select z).Where(delegate(int z)
		{
			string key = string.Format("{0}:{1}:", mapId, z);
			if (File.Exists(AutoBroly.AreaControlPath) && File.ReadAllLines(AutoBroly.AreaControlPath).Any((string line) => line.StartsWith(key)))
			{
				return false;
			}
			string zoneTrackKeyInner = string.Format("{0}:{1}:{2}", mapId, z, DataAccount.Account);
			return !File.Exists(zoneTrackingPath) || !File.ReadAllLines(zoneTrackingPath).Any((string line) => line.StartsWith(zoneTrackKeyInner));
		}).ToList<int>();
		if (checkedZones.Count >= num || list.Count == 0)
		{
			return;
		}
		int num3 = list[0];
		if (AutoBroly.IsBlacklisted(mapId, num3))
		{
			return;
		}
		if (!AutoBroly.CanEnterZone(mapId, num3, DataAccount.Account))
		{
			return;
		}
		if (AutoBroly.IsZoneOwned(mapId, num3))
		{
			return;
		}
		if (AutoBroly.IsBlacklisted(mapId, num3))
		{
			return;
		}
		string zoneTrackKey = string.Format("{0}:{1}:{2}", mapId, num3, DataAccount.Account);
		if (File.Exists(zoneTrackingPath) && File.ReadAllLines(zoneTrackingPath).Contains(zoneTrackKey))
		{
			return;
		}
		if (AutoBroly.IsZoneOwnedByOther(mapId, num3, DataAccount.Account))
		{
			return;
		}
		if (!AutoBroly.TryReserveZone(mapId, num3, DataAccount.Account))
		{
			return;
		}
		File.AppendAllText(zoneTrackingPath, zoneTrackKey + Environment.NewLine);
		checkedZones.Add(num3);
		string format = "mapId:{0}|superCount:{1}|zones:{2}|checkedZones:{3}|time:{4:yyyyMMdd_HHmmss}";
		object[] array3 = new object[5];
		array3[0] = mapId;
		array3[1] = num;
		array3[2] = string.Join(",", source.Select(delegate(int z)
		{
			int num5 = z;
			return num5.ToString();
		}).ToArray<string>());
		array3[3] = string.Join(",", checkedZones.Select(delegate(int z)
		{
			int num5 = z;
			return num5.ToString();
		}).ToArray<string>());
		array3[4] = DateTime.Now;
		string contents = string.Format(format, array3);
		File.WriteAllText(superBrolyInfoPath, contents);
		int maxWaitMs = 3000;
		int waitStep = 150;
		int waited = 0;
		bool flag = false;
		while (waited < maxWaitMs)
		{
			Thread.Sleep(waitStep);
			waited += waitStep;
			if (!File.Exists(superBrolyInfoPath))
			{
				return;
			}
			if (AutoBroly.HasValidSuperBroly())
			{
				flag = true;
				break;
			}
		}
		if (flag)
		{
			for (int i = 0; i < GameScr.vCharInMap.size(); i++)
			{
				global::Char @char = (global::Char)GameScr.vCharInMap.elementAt(i);
				if (@char != null && @char.cName.Contains("Broly") && @char.cName.Contains("Super") && @char.cHPFull >= 16077777L && @char.cHP > 0L)
				{
					AutoBroly.SetZoneOwner(mapId, num3, DataAccount.Account);
					AutoBroly.UpdateZoneStatus(mapId, num3, DataAccount.Account, true);
					int num6 = 1;
					if (File.Exists(path))
					{
						string[] array4 = File.ReadAllText(path).Split(new char[]
						{
							':'
						});
						int num4;
						if (array4.Length == 2 && int.TryParse(array4[1], out num4))
						{
							num6 = num4 + 1;
						}
					}
					File.WriteAllText(path, string.Format("{0}:{1}", mapId, num6));
					return;
				}
			}
		}
		AutoBroly.UpdateZoneStatus(mapId, num3, DataAccount.Account, false);
		AutoBroly.AddToBlacklist(mapId, num3);
		AutoBroly.RemoveZoneOwner(mapId, num3);
		if (File.Exists(zoneTrackingPath))
		{
			string[] contents2 = (from line in File.ReadAllLines(zoneTrackingPath)
			where line != zoneTrackKey
			select line).ToArray<string>();
			File.WriteAllLines(zoneTrackingPath, contents2);
		}
		Service.gI().requestChangeZone(0, -1);
		string text2 = File.ReadAllText(superBrolyInfoPath).Split(new char[]
		{
			'|'
		}).FirstOrDefault((string x) => x.StartsWith("time:"));
		string newTime = ((text2 != null) ? text2.Substring(5) : null) ?? "";
		if (AutoBroly.lastInfoTime != newTime)
		{
			AutoBroly.ResetSuperBrolyTrackingFiles(mapId);
			AutoBroly.lastInfoTime = newTime;
			checkedZones.Clear();
		}
		if (AutoBroly.IsZoneOwned(mapId, num3))
		{
			string ownerLine = File.ReadAllLines(AutoBroly.SuperBrolyOwnerPath).FirstOrDefault((string l) => l.StartsWith(string.Format("{0}:{1}:", mapId, num3)));
			if (ownerLine != null && !ownerLine.EndsWith(DataAccount.Account))
			{
				checkedZones.Add(num3);
				string format2 = "mapId:{0}|superCount:{1}|zones:{2}|checkedZones:{3}|time:{4:yyyyMMdd_HHmmss}";
				object[] arr2 = new object[5];
				arr2[0] = mapId;
				arr2[1] = num;
				arr2[2] = string.Join(",", (from x in source
				select x.ToString()).ToArray<string>());
				arr2[3] = string.Join(",", (from x in checkedZones
				select x.ToString()).ToArray<string>());
				arr2[4] = DateTime.Now;
				string infoContent = string.Format(format2, arr2);
				File.WriteAllText(superBrolyInfoPath, infoContent);
				return;
			}
		}
		string bossKey = string.Format("{0}:{1}", mapId, num3);
		bool hasLegalBoss = false;
		for (int j = 0; j < GameScr.vCharInMap.size(); j++)
		{
			global::Char char2 = (global::Char)GameScr.vCharInMap.elementAt(j);
			if (char2 != null && char2.cName.Contains("Broly") && char2.cName.Contains("Super") && char2.cHPFull >= 16077777L && char2.cHP > 0L)
			{
				hasLegalBoss = true;
				break;
			}
		}
		if (!hasLegalBoss)
		{
			if (!AutoBroly.bossInvalidSince.ContainsKey(bossKey))
			{
				AutoBroly.bossInvalidSince[bossKey] = DateTime.Now;
			}
			if ((DateTime.Now - AutoBroly.bossInvalidSince[bossKey]).TotalSeconds > 5.0)
			{
				AutoBroly.RemoveZoneOwner(mapId, num3);
				AutoBroly.AddToBlacklist(mapId, num3);
				AutoBroly.RemoveZoneControlled(mapId, num3);
				if (File.Exists(zoneTrackingPath))
				{
					File.WriteAllLines(zoneTrackingPath, (from line in File.ReadAllLines(zoneTrackingPath)
					where line != zoneTrackKey
					select line).ToArray<string>());
				}
				AutoBroly.bossInvalidSince.Remove(bossKey);
				Service.gI().requestChangeZone(0, -1);
				return;
			}
		}
		else if (AutoBroly.bossInvalidSince.ContainsKey(bossKey))
		{
			AutoBroly.bossInvalidSince.Remove(bossKey);
		}
	}

	// Token: 0x0600005E RID: 94
	private static void MoveToMapAndZone(int mapId, int zoneId)
	{
		XmapController.StartRunToMapId(mapId);
		Service.gI().requestChangeZone(zoneId, -1);
	}

	// Token: 0x0600005F RID: 95
	private static int[] GetZonesOfMap(int mapId)
	{
		int num = GameScr.gI().zones.Length;
		List<int> list = new List<int>();
		for (int i = 0; i < num; i++)
		{
			if (i != 0 && i != 1)
			{
				list.Add(i);
			}
		}
		return list.ToArray();
	}

	// Token: 0x06000060 RID: 96
	public static void OnGameMessageReceived(string message)
	{
		if (message.Contains("Super Broly xuất hiện tại map"))
		{
			int num = 0;
			int superCount = 0;
			try
			{
				string[] array = message.Split(new char[]
				{
					' '
				});
				for (int i = 0; i < array.Length; i++)
				{
					if (array[i] == "map")
					{
						num = int.Parse(array[i + 1].Replace(",", ""));
					}
					if (array[i] == "có")
					{
						superCount = int.Parse(array[i + 1]);
					}
				}
			}
			catch
			{
			}
			int[] zonesOfMap = AutoBroly.GetZonesOfMap(num);
			AutoBroly.SaveSuperBrolyInfoToFile(num, superCount, zonesOfMap, new List<int>());
			if (File.Exists(AutoBroly.SuperBrolyBlacklistPath))
			{
				File.Delete(AutoBroly.SuperBrolyBlacklistPath);
			}
			if (File.Exists(AutoBroly.SuperBrolyOwnerPath))
			{
				File.Delete(AutoBroly.SuperBrolyOwnerPath);
			}
			if (File.Exists(AutoBroly.AreaControlPath))
			{
				string prefix = string.Format("{0}:", num);
				string[] contents = (from line in File.ReadAllLines(AutoBroly.AreaControlPath)
				where !line.StartsWith(prefix)
				select line).ToArray<string>();
				File.WriteAllLines(AutoBroly.AreaControlPath, contents);
			}
			string path = "C:\\Users\\Admin\\Desktop\\QLTK - SP ver 008\\Nro_244_Data\\Resources\\Data\\zone_tracking.txt";
			if (File.Exists(path))
			{
				string prefix = string.Format("{0}:", num);
				string[] contents2 = (from line in File.ReadAllLines(path)
				where !line.StartsWith(prefix)
				select line).ToArray<string>();
				File.WriteAllLines(path, contents2);
			}
			string path2 = "C:\\Users\\Admin\\Desktop\\QLTK - SP ver 008\\Nro_244_Data\\Resources\\Data\\super_broly_found.txt";
			if (File.Exists(path2))
			{
				File.Delete(path2);
			}
			AutoBroly.ResetSuperBrolyTrackingFiles(num);
		}
		if (message.Contains("Super Broly xuất hiện tại map"))
		{
			int mapId = 0;
			try
			{
				string[] arr = message.Split(new char[]
				{
					' '
				});
				for (int j = 0; j < arr.Length; j++)
				{
					if (arr[j] == "map")
					{
						mapId = int.Parse(arr[j + 1].Replace(",", ""));
					}
					if (arr[j] == "có")
					{
						int.Parse(arr[j + 1]);
					}
				}
			}
			catch
			{
			}
			AutoBroly.ResetSuperBrolyTrackingFiles(mapId);
			string infoPath = AutoBroly.SuperBrolyInfoPath;
			if (File.Exists(infoPath))
			{
				string text = File.ReadAllText(infoPath).Split(new char[]
				{
					'|'
				}).FirstOrDefault((string x) => x.StartsWith("time:"));
				AutoBroly.lastInfoTime = (((text != null) ? text.Substring(5) : null) ?? "");
			}
		}
	}

	// Token: 0x06000061 RID: 97
	public static void SaveSuperBrolyInfoToFile(int mapId, int superCount, int[] zones, List<int> checkedZones)
	{
		string format = "mapId:{0}|superCount:{1}|zones:{2}|checkedZones:{3}|time:{4:yyyyMMdd_HHmmss}";
		object[] array = new object[5];
		array[0] = mapId;
		array[1] = superCount;
		array[2] = string.Join(",", zones.Select(delegate(int z)
		{
			int num = z;
			return num.ToString();
		}).ToArray<string>());
		array[3] = string.Join(",", checkedZones.Select(delegate(int z)
		{
			int num = z;
			return num.ToString();
		}).ToArray<string>());
		array[4] = DateTime.Now;
		string content = string.Format(format, array);
		AutoBroly.SafeWriteAllText(AutoBroly.SuperBrolyInfoPath, content);
	}

	// Token: 0x06000062 RID: 98
	public static int CountSuperBrolyInMap()
	{
		int num = 0;
		for (int i = 0; i < GameScr.vCharInMap.size(); i++)
		{
			global::Char @char = (global::Char)GameScr.vCharInMap.elementAt(i);
			if (@char != null && @char.cName.Contains("Broly") && @char.cName.Contains("Super") && @char.cHPFull >= 16070777L)
			{
				num++;
			}
		}
		return num;
	}

	// Token: 0x06000063 RID: 99
	private static bool IsZoneOwned(int mapId, int zoneId)
	{
		if (!File.Exists(AutoBroly.SuperBrolyOwnerPath))
		{
			return false;
		}
		string value = string.Format("{0}:{1}:", mapId, zoneId);
		string[] array = File.ReadAllLines(AutoBroly.SuperBrolyOwnerPath);
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i].StartsWith(value))
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06000064 RID: 100
	private static void SetZoneOwner(int mapId, int zoneId, string accName)
	{
		string item = string.Format("{0}:{1}:{2}", mapId, zoneId, accName);
		List<string> list = File.Exists(AutoBroly.SuperBrolyOwnerPath) ? File.ReadAllLines(AutoBroly.SuperBrolyOwnerPath).ToList<string>() : new List<string>();
		list = (from l in list
		where !l.StartsWith(string.Format("{0}:{1}:", mapId, zoneId))
		select l).ToList<string>();
		list.Add(item);
		AutoBroly.SafeWriteAllLines(AutoBroly.SuperBrolyOwnerPath, list.ToArray());
	}

	// Token: 0x06000065 RID: 101
	private static bool IsBlacklisted(int mapId, int zoneId)
	{
		if (!File.Exists(AutoBroly.SuperBrolyBlacklistPath))
		{
			return false;
		}
		IEnumerable<string> source = File.ReadAllLines(AutoBroly.SuperBrolyBlacklistPath);
		string value = string.Format("{0}:{1}", mapId, zoneId);
		return source.Contains(value);
	}

	// Token: 0x06000066 RID: 102
	private static void AddToBlacklist(int mapId, int zoneId)
	{
		string item = string.Format("{0}:{1}", mapId, zoneId);
		List<string> list = File.Exists(AutoBroly.SuperBrolyBlacklistPath) ? File.ReadAllLines(AutoBroly.SuperBrolyBlacklistPath).ToList<string>() : new List<string>();
		if (!list.Contains(item))
		{
			list.Add(item);
			AutoBroly.SafeWriteAllLines(AutoBroly.SuperBrolyBlacklistPath, list.ToArray());
		}
	}

	// Token: 0x06000067 RID: 103
	private static bool HasValidSuperBroly()
	{
		for (int i = 0; i < GameScr.vCharInMap.size(); i++)
		{
			global::Char @char = (global::Char)GameScr.vCharInMap.elementAt(i);
			if (@char != null && @char.cName.Contains("Broly") && @char.cName.Contains("Super") && @char.cHPFull >= 16077777L && @char.cHP > 0L)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06000068 RID: 104
	public static void CallType3(int mapId, int zoneId, string accName)
	{
		string content = string.Format("{0}:{1}:{2}", mapId, zoneId, accName);
		AutoBroly.SafeWriteAllText(AutoBroly.SuperBrolyCallPath, content);
	}

	// Token: 0x06000069 RID: 105
	private static void RemoveZoneOwner(int mapId, int zoneId)
	{
		if (!File.Exists(AutoBroly.SuperBrolyOwnerPath))
		{
			return;
		}
		string key = string.Format("{0}:{1}:", mapId, zoneId);
		string[] lines = (from line in File.ReadAllLines(AutoBroly.SuperBrolyOwnerPath)
		where !line.StartsWith(key)
		select line).ToArray<string>();
		AutoBroly.SafeWriteAllLines(AutoBroly.SuperBrolyOwnerPath, lines);
	}

	// Token: 0x0600006A RID: 106
	private static bool IsZoneControlled(int mapId, int zoneId)
	{
		if (!File.Exists(AutoBroly.AreaControlPath))
		{
			return false;
		}
		string key = string.Format("{0}:{1}:", mapId, zoneId);
		return File.ReadAllLines(AutoBroly.AreaControlPath).Any((string line) => line.StartsWith(key));
	}

	// Token: 0x0600006B RID: 107
	private static void SetZoneControlled(int mapId, int zoneId, string accName)
	{
		string str = string.Format("{0}:{1}:{2}", mapId, zoneId, accName);
		AutoBroly.SafeAppendAllText(AutoBroly.AreaControlPath, str + Environment.NewLine);
	}

	// Token: 0x0600006C RID: 108
	private static void RemoveZoneControlled(int mapId, int zoneId)
	{
		if (!File.Exists(AutoBroly.AreaControlPath))
		{
			return;
		}
		string key = string.Format("{0}:{1}:", mapId, zoneId);
		string[] lines = (from line in File.ReadAllLines(AutoBroly.AreaControlPath)
		where !line.StartsWith(key)
		select line).ToArray<string>();
		AutoBroly.SafeWriteAllLines(AutoBroly.AreaControlPath, lines);
	}

	// Token: 0x0600006D RID: 109
	private static void ResetAreaControl(int mapId)
	{
		if (!File.Exists(AutoBroly.AreaControlPath))
		{
			return;
		}
		string[] contents = (from line in File.ReadAllLines(AutoBroly.AreaControlPath)
		where !line.StartsWith(string.Format("{0}:", mapId))
		select line).ToArray<string>();
		File.WriteAllLines(AutoBroly.AreaControlPath, contents);
	}

	// Token: 0x0600006E RID: 110
	private static bool IsZoneBlacklisted(int mapId, int zoneId)
	{
		if (!File.Exists(AutoBroly.BlacklistPath))
		{
			return false;
		}
		string key = string.Format("{0}:{1}", mapId, zoneId);
		return File.ReadAllLines(AutoBroly.BlacklistPath).Any((string line) => line == key);
	}

	// Token: 0x0600006F RID: 111
	private static void AddZoneBlacklist(int mapId, int zoneId)
	{
		string text = string.Format("{0}:{1}", mapId, zoneId);
		if (!File.Exists(AutoBroly.BlacklistPath) || !File.ReadAllLines(AutoBroly.BlacklistPath).Contains(text))
		{
			File.AppendAllText(AutoBroly.BlacklistPath, text + Environment.NewLine);
		}
	}

	// Token: 0x06000070 RID: 112
	private static void RemoveZoneBlacklist(int mapId, int zoneId)
	{
		if (!File.Exists(AutoBroly.BlacklistPath))
		{
			return;
		}
		string key = string.Format("{0}:{1}", mapId, zoneId);
		string[] contents = (from line in File.ReadAllLines(AutoBroly.BlacklistPath)
		where line != key
		select line).ToArray<string>();
		File.WriteAllLines(AutoBroly.BlacklistPath, contents);
	}

	// Token: 0x06000071 RID: 113
	private static void ResetBlacklist(int mapId)
	{
		if (!File.Exists(AutoBroly.BlacklistPath))
		{
			return;
		}
		string[] contents = (from line in File.ReadAllLines(AutoBroly.BlacklistPath)
		where !line.StartsWith(string.Format("{0}:", mapId))
		select line).ToArray<string>();
		File.WriteAllLines(AutoBroly.BlacklistPath, contents);
	}

	// Token: 0x06000072 RID: 114
	private static bool IsInHome()
	{
		int mapID = TileMap.mapID;
		return mapID == 21 || mapID == 22 || mapID == 23;
	}

	// Token: 0x06000073 RID: 115
	private static int GetSoLuongDau()
	{
		int num = 0;
		for (int i = 0; i < global::Char.myCharz().arrItemBag.Length; i++)
		{
			Item item = global::Char.myCharz().arrItemBag[i];
			if (item != null && item.template.type == 6)
			{
				num += item.quantity;
			}
		}
		return num;
	}

	// Token: 0x06000074 RID: 116
	private static void AnDau()
	{
		for (int i = 0; i < global::Char.myCharz().arrItemBag.Length; i++)
		{
			Item item = global::Char.myCharz().arrItemBag[i];
			if (item != null && item.template.type == 6)
			{
				Service.gI().useItem(1, 1, (sbyte)i, -1);
				return;
			}
		}
	}

	// Token: 0x06000075 RID: 117
	private static void UpdateZoneStatus(int mapId, int zoneId, string accName, bool foundBoss)
	{
		string key = string.Format("{0}:{1}:", mapId, zoneId);
		Type typeFromHandle = typeof(AutoBroly);
		lock (typeFromHandle)
		{
			if (File.Exists(AutoBroly.AreaControlPath))
			{
				List<string> list = File.ReadAllLines(AutoBroly.AreaControlPath).ToList<string>();
				list = (from l in list
				where !l.StartsWith(key)
				select l).ToList<string>();
				if (foundBoss)
				{
					list.Add(string.Format("{0}:{1}:{2}:holding", mapId, zoneId, accName));
				}
				AutoBroly.SafeWriteAllLines(AutoBroly.AreaControlPath, list.ToArray());
			}
		}
	}

	// Token: 0x06000076 RID: 118
	private static bool TryReserveZone(int mapId, int zoneId, string accName)
	{
		string key = string.Format("{0}:{1}:", mapId, zoneId);
		string text = string.Format("{0}:{1}:{2}:checking", mapId, zoneId, accName);
		Type typeFromHandle = typeof(AutoBroly);
		bool result;
		lock (typeFromHandle)
		{
			if (!File.Exists(AutoBroly.AreaControlPath))
			{
				AutoBroly.SafeWriteAllText(AutoBroly.AreaControlPath, text + Environment.NewLine);
				result = true;
			}
			else
			{
				List<string> list = File.ReadAllLines(AutoBroly.AreaControlPath).ToList<string>();
				if (list.Any((string l) => l.StartsWith(key)))
				{
					result = false;
				}
				else
				{
					list.Add(text);
					AutoBroly.SafeWriteAllLines(AutoBroly.AreaControlPath, list.ToArray());
					result = true;
				}
			}
		}
		return result;
	}

	// Token: 0x06000077 RID: 119
	private static void OutBossZone(int mapId, int zoneId)
	{
		AutoBroly.RemoveZoneOwner(mapId, zoneId);
		AutoBroly.RemoveZoneControlled(mapId, zoneId);
		AutoBroly.AddToBlacklist(mapId, zoneId);
		AutoBroly.Map = -1;
		AutoBroly.Khu = -1;
		Service.gI().requestChangeZone(0, -1);
	}

	// Token: 0x06000078 RID: 120
	private static bool IsZoneOwnedByOther(int mapId, int zoneId, string myAccName)
	{
		string path = "C:\\Users\\Admin\\Desktop\\QLTK - SP ver 008\\Nro_244_Data\\Resources\\Data\\super_broly_owners.txt";
		if (!File.Exists(path))
		{
			return false;
		}
		string value = string.Format("{0}:{1}:", mapId, zoneId);
		foreach (string text in File.ReadAllLines(path))
		{
			if (text.StartsWith(value) && !text.EndsWith(myAccName))
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06000079 RID: 121
	private static void SafeWriteAllText(string path, string content)
	{
		using (Mutex mutex = new Mutex(false, "Global\\" + Path.GetFileName(path)))
		{
			bool flag = false;
			try
			{
				flag = mutex.WaitOne(2000);
				if (flag)
				{
					File.WriteAllText(path, content);
				}
			}
			finally
			{
				if (flag)
				{
					try
					{
						mutex.ReleaseMutex();
					}
					catch
					{
					}
				}
			}
		}
	}

	// Token: 0x0600007A RID: 122
	private static void SafeWriteAllLines(string path, string[] lines)
	{
		using (Mutex mutex = new Mutex(false, "Global\\" + Path.GetFileName(path)))
		{
			bool flag = false;
			try
			{
				flag = mutex.WaitOne(2000);
				if (flag)
				{
					File.WriteAllLines(path, lines);
				}
			}
			finally
			{
				if (flag)
				{
					try
					{
						mutex.ReleaseMutex();
					}
					catch
					{
					}
				}
			}
		}
	}

	// Token: 0x0600007B RID: 123
	private static void SafeAppendAllText(string path, string content)
	{
		using (Mutex mutex = new Mutex(false, "Global\\" + Path.GetFileName(path)))
		{
			bool flag = false;
			try
			{
				flag = mutex.WaitOne(2000);
				if (flag)
				{
					File.AppendAllText(path, content);
				}
			}
			finally
			{
				if (flag)
				{
					try
					{
						mutex.ReleaseMutex();
					}
					catch
					{
					}
				}
			}
		}
	}

	// Token: 0x0600007C RID: 124
	private static void ResetSuperBrolyTrackingFiles(int mapId)
	{
		foreach (string path in new string[]
		{
			AutoBroly.SuperBrolyInfoPath,
			AutoBroly.SuperBrolyBlacklistPath,
			AutoBroly.SuperBrolyOwnerPath,
			AutoBroly.SuperBrolyCallPath,
			"C:\\Users\\Admin\\Desktop\\QLTK - SP ver 008\\Nro_244_Data\\Resources\\Data\\zone_tracking.txt",
			"C:\\Users\\Admin\\Desktop\\QLTK - SP ver 008\\Nro_244_Data\\Resources\\Data\\super_broly_found.txt"
		})
		{
			try
			{
				if (File.Exists(path))
				{
					File.WriteAllText(path, string.Empty);
				}
			}
			catch
			{
			}
		}
		AutoBroly.ResetBlacklist(mapId);
		AutoBroly.ResetAreaControl(mapId);
		AutoBroly.ResetOwnerOfMap(mapId);
	}

	// Token: 0x0600007D RID: 125
	private static bool CanEnterZone(int mapId, int zoneId, string accName)
	{
		if (AutoBroly.IsZoneOwnedByOther(mapId, zoneId, accName))
		{
			return false;
		}
		string.Format("{0}:{1}:{2}", mapId, zoneId, accName);
		string path = "C:\\Users\\Admin\\Desktop\\QLTK - SP ver 008\\Nro_244_Data\\Resources\\Data\\zone_tracking.txt";
		return !File.Exists(path) || !File.ReadAllLines(path).Any((string l) => l.StartsWith(string.Format("{0}:{1}:", mapId, zoneId)) && !l.EndsWith(accName));
	}

	// Token: 0x0600007E RID: 126
	private static void ResetOwnerOfMap(int mapId)
	{
		if (!File.Exists(AutoBroly.SuperBrolyOwnerPath))
		{
			return;
		}
		string[] contents = (from line in File.ReadAllLines(AutoBroly.SuperBrolyOwnerPath)
		where !line.StartsWith(mapId + ":")
		select line).ToArray<string>();
		File.WriteAllLines(AutoBroly.SuperBrolyOwnerPath, contents);
	}

	// Token: 0x0600007F RID: 127
	private static void OutAndCleanZone(int mapId, int zoneId)
	{
		AutoBroly.RemoveZoneOwner(mapId, zoneId);
		AutoBroly.RemoveZoneControlled(mapId, zoneId);
		AutoBroly.AddToBlacklist(mapId, zoneId);
		string path = "C:\\Users\\Admin\\Desktop\\QLTK - SP ver 008\\Nro_244_Data\\Resources\\Data\\zone_tracking.txt";
		string zoneTrackKey = string.Format("{0}:{1}:{2}", mapId, zoneId, DataAccount.Account);
		if (File.Exists(path))
		{
			string[] contents = (from line in File.ReadAllLines(path)
			where line != zoneTrackKey
			select line).ToArray<string>();
			File.WriteAllLines(path, contents);
		}
		AutoBroly.Map = -1;
		AutoBroly.Khu = -1;
		Service.gI().requestChangeZone(0, -1);
	}

	// Token: 0x0400004D RID: 77
	public static string TrangThai;

	// Token: 0x0400004E RID: 78
	public static int Map;

	// Token: 0x0400004F RID: 79
	public static int Khu;

	// Token: 0x04000050 RID: 80
	private static bool IsWait;

	// Token: 0x04000051 RID: 81
	private static long TimeStartWait;

	// Token: 0x04000052 RID: 82
	private static long TimeWait;

	// Token: 0x04000053 RID: 83
	public static bool isDoKhu;

	// Token: 0x04000054 RID: 84
	private static HashSet<int> visitedZones;

	// Token: 0x04000055 RID: 85
	private static Random random;

	// Token: 0x04000056 RID: 86
	public static int NhayNe;

	// Token: 0x04000057 RID: 87
	private static long lastAvoidBrolyTime;

	// Token: 0x04000058 RID: 88
	private const int avoidDistance = 100;

	// Token: 0x04000059 RID: 89
	private const int avoidInterval = 500;

	// Token: 0x0400005A RID: 90
	private static string SuperBrolyInfoPath;

	// Token: 0x0400005B RID: 91
	private static string SuperBrolyOwnerPath;

	// Token: 0x0400005C RID: 92
	private static string SuperBrolyBlacklistPath;

	// Token: 0x0400005D RID: 93
	private static string SuperBrolyCallPath;

	// Token: 0x0400005E RID: 94
	private static string AreaControlPath;

	// Token: 0x0400005F RID: 95
	private static string BlacklistPath;

	// Token: 0x04001730 RID: 5936
	private static string lastInfoTime = "";

	// Token: 0x04001731 RID: 5937
	private static Dictionary<string, DateTime> bossInvalidSince = new Dictionary<string, DateTime>();
}
kiẻm tra mã nguồn xem đã đúng yeu cầu sau của tôi chưa và càn bỏ sung gì không           
phần1: bỏ lưu map khi gặp boss super broly chỉ của riêng type == 1

phần2: thêm né boss broly cho type == 1 ( không phải super broly )
+ khi di chuyển khoảng cách giữa type == 1 và boss broly <= 100px thì thì type 1 tựa động dịch ra sao cho đủ 100px
+ khi boss tiến lại gần type 1 khoảng cách <= 100px thì type == 1 dịch chuyển sao cho đủ 100px
+ khi khoảng các >= 100 px type == 1 di chuyển tự do không bị kìm hãy hay kéo lại boss khi khoảng cách >= 100px 
+ thêm cái check time khi quá tgian 500ms mà khoảng cách <= 100px thì nhân vật tự dịch chuyển 100px
+ khi bị boss đuổi dồn vào cuối map ( bản đồ nhân vật tự dịch chuyển về phía bên kia của boss )

dựa vào mã nguồn toi vừa gửi code xử lí chỉ tôi từng phần và code thật kĩ các phần theo thứ tự sao cho các phần liên kết chặt chẽ với nhau hợp lí và logic và tự dộng 100% :  
phần 3: check thông báo, 
+ dựa vào thông báo của game: tôi đã làm auto dò boss theo thông báo của game để type ==3 tìm đến khu + map chứ super 
+hãy dựa voà thông báo của game tạo file dẫn: C:\Users\Admin\Desktop\QLTK - SP ver 008\Nro_244_Data\Resources\Data
+ để check map vừa thông báo có super xuất hiện xem có bao nhiêu con super trong map đó 
+ để tất cả acc type 3 dùng file đó xem có bao nhiêu con vừa ra cùng map để check đủ số lượng mà game thông báo trong map đó
+ khi check đủ số lượng rồi dừng 
+ các ví dụ trường hợp không check đủ số lượng
      - TH1: mếu ra 2 con chỉ check được 1 hay không con thì dừng, reset file chờ thông báo map mới có super 
      - TH2: néu đang check, được 5 khu mà con mới ra sẽ reset khu đã check và check lại
+ kết hợp: fix dò trùng khu,  check hp, kết hợp với giới hạn mỗi khu chỉa chứ 1 acc type == 3
+ type 2 chỉ vào map + khu khi type 1 gọi, cái này tôi đxa code rồi nên không càn,
+ khi được gọi vô sẽ auto lưu map + khu và luôn chọn boss thì cái này tôi cũng code rồi
+chỉ code thêm khi không đủ điều hp <= 16077777 và >= 1  kiện type 2 sẽ bỏ lưu khu + map sẽ out ra khu khác và chờ type 1 gọi lần tiếp theo.
lưu ý: tự động làm 100%

Phần 4: tôi muốn giới hạn mỗi khu chưa super chỉ được phép 1 acc type ==3:
+ giờ tôi muốn mõi khu có boss super broly thì chỉ được giới hạn tối đa là 1 acc type == 3
+khi có 1 acc type ==3 trong khu có boss super broly thì bọn type ==3 kia sẽ bỏ qua khu đã phát hiện boss và có type 3 khác dứng cùng
+còn acc type 2 và 1 sẽ vô khu bình thường
+làm cái file có đường dẫn:C:\Users\Admin\Desktop\QLTK - SP ver 008\Nro_244_Data\Resources\Data
+cho tất cả các acc type ==3, khi dựa voà thông báo của game tìm đến map, acc sẽ quét xem map có bao nhiêu khu để điển vào file, khu nào check rồi sẽ xoá khỏi file để các acc khác không chcek khu thuộc map đó nữa
+ acc nào dò được sẽ điền tên mình + id map + khu để các acc khác không vô khu thuộc map đó nữa vì tôi chỉ cần 1 acc trong khu + map đó
+khi boss chết, hay biến mất sẽ xoá dữ liệu ở file đó và các acc khác có thế vô lại khu đó bình thường
+khi 1 acc vô được khu có boss các acc khác sẽ dừng viêc dò và chờ thông báo mới của game để chạy đi dò super broly
 vd: nếu thông báo 1 map có 2 con super ,mà 1 acc giữ rồi thì bọn kia sẽ dò nốt khu còn lại 
     mõi khu chỉ 1 acc type 3 ( kết hợp phần 4: check thông báo + phần 5 )
lưu ý: tự động làm 100%

phần 5: type ==1 gọi type ==3 khi phát hiện boss đủ điều kiện
+ khi acc type == 1 phát hiện boss super broly ( đến map có Super broly hay super broly xuất hiện  tại map acc type == 1 dứng ) 
+với đièu kiện HP >= 16077777  acc type ==1  sẽ tự động gọi acc type == 1 qua map, 
+kết hợp với chức năng mỗi khu boss chỉ chứa 1 acc type == 3 thì type 1 chỉ gọi đúng 1 acc type 3,
+ sẽ tạo 1 file đường dẫn chung:C:\Users\Admin\Desktop\QLTK - SP ver 008\Nro_244_Data\Resources\Data
+cho tất cả các type 3, chỉ điền tên acc nào acc đó sẽ tự động chạy đến chỗ type nếu boss đủ điều kiện hp, các acc type 3 khác sẽ đứng im
+ kết hợp phần 3, check số lượng boss trên 1 map: nếu tb chỉ ra map ra suoer có đúng 1 con mà nằm cùng type 1, các acc type 3 không được gọi sẽ không dò
+ kết hợp phần 4:, chỉ được 1 acc type vô khu có bos các type 3 khác sẽ không dò
lưu ý: tự động làm 100%

phần 6:check hp boss: 
+gặp trường hợp boss không đủ điều kiện hp hoặc type 1 không cùng khu + map với boss tức là type 1 không gọi type 3 ở phần 5
+thì type == 3 tự đi dò theo thông báo như bình thường, sẽ check map có bao nhiêu con super đã nói ở phần trên
+nếu map chỉ có 1 con mà phát hiện con boss đó HP thảo mãn điều kiện hp là >= 16077777 và <= 1 thì acc phát hiện sẽ đứng đó và điền vào file như ở phần trên để các type 3 khác không dò nữa ,
+ nếu mà nếu gặp trường hợp super bị tấn công hay mới xuất hiện mà super không thuộc khoảng điều kiện của hp là >=16077777 và <=1 quá 5 giây nó sẽ out map hoặc out khu khác chờ thông báo mới để dò
+có dựa vào phàn 3 check thông báo dể làm + phàn 4: mỗi acc type ==3 chỉ được vô 1 khu có super),
+kể cả type 2 cũng vậy nhưng mà type 2 không dò theo thông báo của game mà chỉ dựa vào phím tắt gọi type 2 qua của type 1 tôi đã code để vô khu có boss
+ khi type 3 gặp boss không đủ điều kiện hp tự tắt lưu map +khu và chọn boss cũng sẽ out map hoặc khu như type 3 
lưu ý: tự động làm 100%

phần 7: fix dò trùng khu:
+ tạo 1 cái file có đường dẫn: C:\Users\Admin\Desktop\QLTK - SP ver 008\Nro_244_Data\Resources\Data
+ acc type 3 vô sẽ quét xem map có bao nhiêu khu rồi điền vào file, khu nào check rồi sẽ xoá khỏi file để type 3 khác không check lại 
+( kết hợp phần 4 + 3 + 6 ) acc nào vô sẽ điền tên vô file có nguônf là : C:\Users\Admin\Desktop\QLTK - SP ver 008\Nro_244_Data\Resources\Data
+đẻ các acc type == 3 khác check file đó biết map/khu super đó đã có người không vô nữa , 
+ nếu đang check mà có con super khác ra cùng map sẽ reset lại khu đã dò và dò lại , tránh khu đã có acc chờ ks super
lưu ý: tự động làm 100%

phần 8: blacklist khu map:
+làm cái file blacklist có nguồn là : C:\Users\Admin\Desktop\QLTK - SP ver 008\Nro_244_Data\Resources\Data
+dựa vào check hp của phần 6 nếu máu boss không đủ đièu kiện thoả mãn >=16077777 hay <= 1 sẽ out map và điền vào file khu + map boss không thảo mãn điều kiện hp  để các acc type 3 không vô khu
+ nếu có thông báo của game thấy có super mới ra tại map có khu vực chứa trong file blacklist sẽ reset lại file để check lại và nếu check lại vẫn không đủ đièu kiện thì lại thêm vô blacklist
+ còn biết mất khỏi khu thì thôi
lưu ý: tự động làm 100%

Phần 9: bỏ check super ở khu 1 2 khỏi tất cả các map vì game sẽ luôn luôn không cho boss xuất hiệ ở các khu này
lưu ý: tự động làm 100%

lưu ý cực kìg quan trọng là phải giữ nguyên các chức năng cũ 
lưu ý : tôi nghĩ bạn nên đọc kĩ phần cuói: toi có kết hợp logic các phần với nhau, và bạn cần code, vào 1 cái thôi tôi coppy cho dễ, và bạn nên giữ lại các chúc năng cũ mà tôi đã code không thay đỏi chúng                           
