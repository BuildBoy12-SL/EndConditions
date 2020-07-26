using System;
using System.Collections.Generic;
using System.Linq;
using Exiled.API.Features;
using Exiled.Events.EventArgs;

namespace EndConditions
{
	public class Handler
	{
		public Plugin plugin;
		public Handler(Plugin plugin) => this.plugin = plugin;

		public bool AllowDebug => plugin.Config.AllowDebug;
		public static Dictionary<string, List<string>> escapeConditions = new Dictionary<string, List<string>>();
		public void OnCheckRoundEnd(EndingRoundEventArgs ev) 
		{
			if (!plugin.Config.AllowDefaultEndConditions) 
			{
				ev.IsAllowed = false;
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
				foreach (Player hub in Player.List) 
				{
					if(hub.Role != RoleType.Spectator)
						list.Add(hub.Role.ToString().ToLower());
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
						Log.Debug("Check passed.", AllowDebug);
						try 
						{
							//Get the key that contains the name and escape conditions
							string key = escapeConditions.FirstOrDefault(x => x.Value == condition.Value).Key;
							Log.Debug($"Using conditions from condition name: '{key}'", AllowDebug);
							//Check for escape conditions
							if (key.Contains("+classd")) 
							{
								if (RoundSummary.escaped_ds != 0) 
								{
									EndGame(ev, key);
									return;
								}
								else 
								{
									Log.Debug("+C Second check failed" + " " + key, AllowDebug);
								}
							}
							else if (key.Contains("-classd")) 
							{
								if (RoundSummary.escaped_ds == 0) 
								{
									EndGame(ev, key);
									return;
								}
								else 
								{
									Log.Debug("-C Second check failed" + " " + key, AllowDebug);
								}
							}
							else if (key.Contains("+science")) 
							{
								if (RoundSummary.escaped_scientists != 0) 
								{
									EndGame(ev, key);
									return;
								}
								else 
								{
									Log.Debug("+S Second check failed" + " " + key, AllowDebug);
								}
							}
							else if (key.Contains("-science")) 
							{
								if (RoundSummary.escaped_scientists == 0) 
								{
									EndGame(ev, key);
									return;
								}
								else 
								{
									Log.Debug("-S Second check failed" + " " + key, AllowDebug);
								}
							}
							else 
							{
								EndGame(ev, key);
								return;
							}
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
		public void EndGame(EndingRoundEventArgs ev, string team) 
		{			
			ev.LeadingTeam = (RoundSummary.LeadingTeam)ConvertTeam(team);
			ev.IsRoundEnded = true;
			ev.IsAllowed = true;
			if (plugin.Config.AllowVerbose)
				Log.Info($"Force ending with {ev.LeadingTeam} as the lead team.");
		}

		//Middle man to convert string into RoundSummary.LeadingTeam to account for dictionary keys.
		public int ConvertTeam(string arg) 
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