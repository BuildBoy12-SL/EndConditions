using System;
using System.Collections.Generic;
using System.Linq;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.Events.EventArgs;
using Exiled.API.Enums;

namespace EndConditions
{
	public class Handler
	{
		public Plugin plugin;
		public Handler(Plugin plugin) => this.plugin = plugin;

		private bool AllowDebug => plugin.Config.AllowDebug;

		internal static Dictionary<string, List<string>> EndConditions
			= new Dictionary<string, List<string>>();

		private readonly Dictionary<string, bool> EscAdditions = new Dictionary<string, bool>
		{
			{ "-classd", RoundSummary.escaped_ds == 0 },
			{ "+classd", RoundSummary.escaped_ds != 0 },
			{ "-science", RoundSummary.escaped_scientists == 0 },
			{ "+science", RoundSummary.escaped_scientists != 0 },
		}; 
		
		public void OnCheckRoundEnd(EndingRoundEventArgs ev) 
		{
			if (!plugin.Config.AllowDefaultEndConditions) 
			{
				ev.IsAllowed = false;
				ev.IsRoundEnded = false;
			}
			//Check if the warhead has detonated && if the detonation winner is set
			if (Warhead.IsDetonated && plugin.Config.DetonationWinner != "none") 
			{ 
				EndGame(ev, plugin.Config.DetonationWinner); 
			}
			else 
			{
				//Shove all alive roles into the list
				List<string> list = new List<string>();
				foreach (Player ply in Player.List)
				{
					if (ply.Role.Equals(RoleType.Spectator)) continue;
					if (ply.ReferenceHub.serverRoles.GetUncoloredRoleString().Contains("SCP-035"))
					{
						list.Add("scp035");
						continue;
					}

					list.Add(ply.Role.ToString().ToLower());
				}
				//If ignoretut, do the thing.
				if (plugin.Config.IgnoreTutorials)
					list.RemoveAll(item => item == "tutorial");
				//Put all the lists from the core dictionary and check em
				foreach (KeyValuePair<string, List<string>> condition in escapeConditions) 
				{
					//The actual check
					if (!list.Except(condition.Value).Any()) 
					{
						//Get the key that contains the name and escape conditions
						string key = EndConditions.FirstOrDefault(x => x.Value == condition.Value).Key;
						Log.Debug($"Using conditions from condition name: '{key}'", AllowDebug);
						//Check for escape conditions
						string[] splitKey = key.Split(' ');
						IEnumerable<string> conds = splitKey.Where(str => EscAdditions.Keys.Contains(str));
						foreach (string cond in conds)
						{
							Log.Debug($"Condition: {cond}, Result: {EscAdditions[cond]}", AllowDebug);
						}
						
						if (conds.Any(str => !EscAdditions[str]))
						{
							Log.Debug("Escape conditions check failed:" + " " + key, AllowDebug);
							continue;
						}
						catch (Exception e) 
						{
							Log.Error($"Exception during final checks: {e}");
						}
					}				
				}				
			}
		}

		//Central round end function
		private void EndGame(EndingRoundEventArgs ev, string team) 
		{			
			ev.LeadingTeam = (LeadingTeam)ConvertTeam(team);
			ev.IsRoundEnded = true;
			ev.IsAllowed = true;
			if (plugin.Config.AllowVerbose)
				Log.Info($"Force ending with {ev.LeadingTeam} as the lead team.");
		}

		//Middle man to convert string into RoundSummary.LeadingTeam to account for dictionary keys.
		private int ConvertTeam(string arg) 
		{
			var team = arg.ToLower().StartsWith("facilityforces") ? 0
					 : arg.ToLower().StartsWith("chaosinsurgency") ? 1
					 : arg.ToLower().StartsWith("anomalies") ? 2
					 : arg.ToLower().StartsWith("draw") ? 3
					 : -1;

			if (team == -1)
            {
				Log.Debug($"Could not parse {arg} into a team, returning as a draw.", AllowDebug);
				return 3;
			}
			return team;
		}
	}
}
