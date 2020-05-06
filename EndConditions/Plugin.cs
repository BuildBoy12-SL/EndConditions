using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using EndConditions.Properties;
using EXILED;
using Newtonsoft.Json.Linq;
using YamlDotNet.Serialization;

namespace EndConditions {
	public class Plugin : EXILED.Plugin {

		public EventHandlers EventHandlers;
		public Dictionary<string, List<string>> dict = new Dictionary<string, List<string>>();

		internal bool isGlobal;

		internal bool enabled;
		internal bool debug;
		internal bool verbose;
		internal JObject configs;
		
		internal bool DefaultEndConditions;
		internal string DetonationWinner;
		internal bool IgnoreTut;
		
		public static string PluginDirectory = Path.Combine(PluginManager.PluginsDirectory, "EndConditions");
		public static string FileDirectory = Path.Combine(PluginDirectory, "config.yml");

		public override void OnEnable() {
			ReloadConfigs();
			if (!enabled)
				return;

			EventHandlers = new EventHandlers(this);

			Events.WaitingForPlayersEvent += EventHandlers.OnWaitingForPlayers;
			Events.CheckEscapeEvent += EventHandlers.CheckEscape;
			Events.CheckRoundEndEvent += EventHandlers.OnCheckRoundEnd;
			Log.Info($"EndConditions Loaded.");
		}

		public override void OnDisable() {
			Events.WaitingForPlayersEvent -= EventHandlers.OnWaitingForPlayers;
			Events.CheckEscapeEvent -= EventHandlers.CheckEscape;
			Events.CheckRoundEndEvent -= EventHandlers.OnCheckRoundEnd;

			EventHandlers = null;
		}

		public override void OnReload() { }

		public override string getName => "EndConditions";

		public void ReloadConfigs() {
			try {
				//Is this thing global?
				isGlobal = Config.GetBool("ec_global", true);
				string path = isGlobal ? FileDirectory : Path.Combine(PluginDirectory, ServerConsole.Port.ToString(), "config.yml");

				if (!Directory.Exists(PluginDirectory))
					Directory.CreateDirectory(PluginDirectory);
				//If it doesn't exist, make it so it does
				if (!isGlobal) {
					if (!Directory.Exists(Path.Combine(PluginDirectory, ServerConsole.Port.ToString())))
						Directory.CreateDirectory(Path.Combine(PluginDirectory, ServerConsole.Port.ToString()));
				}
				if (!File.Exists(path))
					File.WriteAllText(path, Encoding.UTF8.GetString(Resources.config));

				//Read the file
				FileStream stream = File.OpenRead(path);
				IDeserializer deserializer = new DeserializerBuilder().Build();
				object yamlObject = deserializer.Deserialize(new StreamReader(stream));
				ISerializer serializer = new SerializerBuilder().JsonCompatible().Build();
				string jsonString = serializer.Serialize(yamlObject);
				JObject json = JObject.Parse(jsonString);

				//Get the base config options
				enabled = json.SelectToken("enabled").Value<bool>();
				debug = json.SelectToken("debug").Value<bool>();
				verbose = json.SelectToken("verbose").Value<bool>();
				configs = json.SelectToken("endconditions").Value<JObject>();
				DefaultEndConditions = json.SelectToken("default").Value<bool>();
				DetonationWinner = json.SelectToken("warheadwinner").Value<string>().ToLower();
				IgnoreTut = json.SelectToken("ignoretut").Value<bool>();

				//Get the EndConditions
				JProperty[] groups = configs.Properties().ToArray();
				foreach (JProperty group in groups) {
					foreach (JObject bundle in group.Value.Children()) {
						List<string> hold = new List<string>();
						JProperty minibundle = bundle.Properties().First();
						foreach (string classes in minibundle.Value as JArray) {
							hold.Add(classes.ToLower());
						}
						dict.Add($"{group.Name.ToLower()}-{minibundle.Name.ToLower()}", hold);
					}
				}
				Log.Info("EndConditions Configs loaded.");
			}
			catch (Exception e) {
				Log.Error($"Error loading configs for EndConditions: {e}");
			}				
		}
	}
}