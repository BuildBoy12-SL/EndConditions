namespace EndConditions
{
	using Exiled.API.Enums;
	using Exiled.API.Features;
	using Exiled.Events.EventArgs;
	using System;
	using System.Collections.Generic;
	using System.Linq;
	
	public class Handler
	{ 
		internal static readonly Dictionary<string, List<string>> EndConditions
			= new Dictionary<string, List<string>>();
		
		public void OnCheckRoundEnd(EndingRoundEventArgs ev) 
		{
			if (!Plugin.Instance.Config.AllowDefaultEndConditions) 
			{
				ev.IsAllowed = false;
				ev.IsRoundEnded = false;
			}
			//Check if the warhead has detonated && if the detonation winner is set
			if (Warhead.IsDetonated && Plugin.Instance.Config.DetonationWinner != "none") 
			{ 
				EndGame(ev, Plugin.Instance.Config.DetonationWinner); 
			}
			else 
			{
				Dictionary<string, bool> escAdditions = new Dictionary<string, bool>
				{
					{ "-classd", RoundSummary.escaped_ds == 0 },
					{ "+classd", RoundSummary.escaped_ds != 0 },
					{ "-science", RoundSummary.escaped_scientists == 0 },
					{ "+science", RoundSummary.escaped_scientists != 0 },
				};
				
				//Shove all alive roles into the list
				List<string> list = new List<string>();
				foreach (Player ply in Player.List)
				{
					if (ply.Role.Equals(RoleType.Spectator)) 
						continue;
					
					if (ply.ReferenceHub.serverRoles.GetUncoloredRoleString().Contains("SCP-035"))
					{
						list.Add("scp035");
						continue;
					}

					list.Add(ply.Role.ToString().ToLower());
				}
				//If ignoretut, do the thing.
				if (Plugin.Instance.Config.IgnoreTutorials)
					list.RemoveAll(item => item == "tutorial");
				//Put all the lists from the core dictionary and check em
				foreach (KeyValuePair<string, List<string>> condition in escapeConditions) 
				{
					try 
					{
						//Get the key that contains the name and escape conditions
						string key = EndConditions.FirstOrDefault(x => x.Value == condition.Value).Key;
						Log.Debug($"Using conditions from condition name: '{key}'", Plugin.Instance.Config.AllowDebug);
						//Check for escape conditions
						string[] splitKey = key.Split(' ');
						List<string> conds = splitKey.Where(str => escAdditions.Keys.Contains(str)).ToList();
						List<string> failedConds = conds.Where(x => !escAdditions[x]).ToList();
						if (failedConds.Count > 0)
						{
							Log.Debug($"Failed at: {string.Join(", ", failedConds)}", Plugin.Instance.Config.AllowDebug);
							continue;
						}

						Log.Debug("Escape checks passed, ending round.", Plugin.Instance.Config.AllowDebug);
						EndGame(ev, key);
						return;
					}
					catch (Exception e) 
					{
						Log.Error($"Exception during final checks: {e}");
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
			//_forceLock = true;
			if (Plugin.Instance.Config.AllowVerbose)
				Log.Info($"Force ending with {ev.LeadingTeam} as the lead team.");
		}

		//Middle man to convert string into RoundSummary.LeadingTeam to account for dictionary keys.
		private static int ConvertTeam(string arg) 
		{
			var team = arg.ToLower().StartsWith("facilityforces") ? 0
					 : arg.ToLower().StartsWith("chaosinsurgency") ? 1
					 : arg.ToLower().StartsWith("anomalies") ? 2
					 : arg.ToLower().StartsWith("draw") ? 3
					 : -1;

			if (team != -1) 
				return team;
			
			Log.Debug($"Could not parse {arg} into a team, returning as a draw.", Plugin.Instance.Config.AllowDebug);
			return 3;
		}
	}
}
