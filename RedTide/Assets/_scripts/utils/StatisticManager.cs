using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

// 用于统计的总入口
public class StatisticManager
{   
	private static readonly string STATISTIC_DATA = "statistic.json";
	private static string STATISTIC_DATA_PATH
	{
		get { return Application.persistentDataPath + "/" + STATISTIC_DATA; }
	}

	private static StatisticManager _instance;

	private Dictionary<String, float> _dmgStatistic = new Dictionary<string, float>();
	private float _playerHeal;

	private StatisticManager()
	{
		
	}

	public static StatisticManager GetInstance()
	{
		if (_instance == null)
		{
			_instance = new StatisticManager();
		}
		
		return _instance;
	}


	public void RegisterDamage(string name, GameUnit target, float dmg)
	{
		{
			if (_dmgStatistic.ContainsKey(name))
			{
				_dmgStatistic[name] += dmg;
			}
			else
			{
				_dmgStatistic[name] = dmg;			
			}
		}
	}

	public void Save()
	{
//		var rootNode = new JSONClass();
//		rootNode.Add("damage", SaveDamageToJSON());
//		rootNode.SaveToFile(STATISTIC_DATA_PATH);

		var builder = new StringBuilder();
		SaveDamage(builder);
		builder.Append("TotalHeal,").Append(_playerHeal).Append(" \n");
		System.IO.File.WriteAllText(STATISTIC_DATA_PATH + DateTime.Now.ToBinary(), builder.ToString());
	}

	private void SaveDamage(StringBuilder builder)
	{
		var typeDictionary = new Dictionary<string, Vector2>();
		
		foreach (var entry in _dmgStatistic)
		{
//			builder.Append(entry.Key).Append("  ").Append(entry.Value.ToString() + " \n");
			var subString = entry.Key;
			var indexOf = entry.Key.IndexOf("(");
			if (indexOf > 0)
			{
				subString = entry.Key.Substring(0, indexOf);
			}
			if (typeDictionary.ContainsKey(subString))
			{
				var keyValuePair = typeDictionary[subString];
				keyValuePair.x += 1;
				keyValuePair.y += entry.Value;
				typeDictionary[subString] = keyValuePair;
			}
			else
			{
				typeDictionary[subString] = new Vector2(1, entry.Value);			
			}
		}

		foreach (var entry in typeDictionary)
		{
			builder.Append(entry.Key).Append(",").Append(entry.Value.x).Append(",").Append(entry.Value.y + " \n");
		}
		
	}

	public void RegisterPlayHeal(float healing)
	{
		_playerHeal += healing;
	}

	public void Analysis()
	{
		var dictPath = "D:\\tmp\\game04Log";
		var directoryInfo = new DirectoryInfo(dictPath);
		var typDictionary = new Dictionary<string, Vector2>();
		foreach (var fileInfo in directoryInfo.GetFiles())
		{
			if (fileInfo.Name.Contains("statistic"))
			{
				var readAllLines = File.ReadAllLines(dictPath + "\\" + fileInfo.Name);
				foreach (var line in readAllLines)
				{
					List<string> splits = new List<string>();
					int pos = 0;
					int start = 0;
					do
					{
						pos = line.IndexOf(",", start);
						if (pos > 0)
						{
							var subString = line.Substring(start, pos - start + 1);
							splits.Add(subString);
							start = pos + 1;
						}
						else
						{
							splits.Add(line.Substring(start));
						}
					} while (pos > 0);

					float count = 1;
					float value = 0;
					try
					{
						if (splits.Count > 2)
						{
							count = float.Parse(splits[1]);
							value = float.Parse(splits[2]);
						}
						else
						{
							count = 1;
							value = float.Parse(splits[1]);
						}
						var type = splits[0];
						if (typDictionary.ContainsKey(type))
						{
							var keyValuePair = typDictionary[type];
							keyValuePair.x += count;
							keyValuePair.y += value;
							typDictionary[type] = keyValuePair;
						}
						else
						{
							typDictionary[type] = new Vector2(count, value);			
						}
					}
					catch (Exception e)
					{
						Debug.Log(e.ToString());
						Debug.Log("		Parse line: " + line);
						foreach (var split in splits)
						{
							Debug.Log(split);
						}
						Debug.Log("!!");
					}

				}
			}
		}
		
		var builder = new StringBuilder();
		foreach (var entry in typDictionary)
		{
			builder.Append(entry.Key).Append(",  ").Append(entry.Value.x).Append(",  ").Append(entry.Value.y + " \n");
		}
		System.IO.File.WriteAllText(dictPath + "\\analysis-" + DateTime.Now.ToBinary(), builder.ToString());

		
	}
}
