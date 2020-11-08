namespace EndConditions
{
	using Exiled.API.Enums;
	using Exiled.API.Features;
	using Exiled.Events.EventArgs;
	using System;
	using System.Collections.Generic;
	using System.Linq;
	
	using static EndConditions;
	
	public class Handler
	{ 
		internal static readonly Dictionary<string, List<string>> EndConditions
			= new Dictionary<string, List<string>>();
		
		public void OnCheckRoundEnd(EndingRoundEventArgs ev) 
		{
			if (!Instance.Config.AllowDefaultEndConditions) 
			{
				ev.IsAllowed = false;
				ev.IsRoundEnded = false;
			}
			
			// Check if the warhead has detonated && if the detonation winner is set
			if (Warhead.IsDetonated && Instance.Config.DetonationWinner != "none")
			{
				EndGame(ev, Instance.Config.DetonationWinner);
				return;
			}

			var escAdditions = new Dictionary<string, bool>
			{
				{ "-classd", RoundSummary.escaped_ds == 0 },
				{ "+classd", RoundSummary.escaped_ds != 0 },
				{ "-science", RoundSummary.escaped_scientists == 0 },
				{ "+science", RoundSummary.escaped_scientists != 0 },
			};
				
			// Shove all alive roles into the list
			List<string> list = new List<string>();
			foreach (Player ply in Player.List)
			{
				if (ply.Role == RoleType.Spectator || API.BlacklistedPlayers.Contains(ply)) 
					continue;

				if (API.ModifiedRoles.ContainsKey(ply))
				{
					list.Add(API.ModifiedRoles[ply]);
					continue;
				}
				
				if (ply.ReferenceHub.serverRoles.GetUncoloredRoleString().Contains("SCP-035"))
				{
					list.Add("scp035");
					continue;
				}
				
				if (ply.Role == RoleType.Tutorial && Instance.Config.IgnoreTutorials)
					continue;
				
				list.Add(ply.Role.ToString().ToLower());
			}
			
			// Pull all the lists from the core dictionary and check em
			foreach (var condition in EndConditions.Where(condition => !list.Except(condition.Value).Any()))
			{
				try 
				{
					// Get the key that contains the name and escape conditions
					string key = EndConditions.FirstOrDefault(x => x.Value == condition.Value).Key;
					Log.Debug($"Using conditions from condition name: '{key}'", Instance.Config.AllowDebug);
					// Check for escape conditions
					string[] splitKey = key.Split(' ');
					List<string> conds = splitKey.Where(str => escAdditions.Keys.Contains(str)).ToList();
					List<string> failedConds = conds.Where(x => !escAdditions[x]).ToList();
					if (failedConds.Count > 0)
					{
						Log.Debug($"Failed at: {string.Join(", ", failedConds)}", Instance.Config.AllowDebug);
						continue;
					}

					Log.Debug("Escape checks passed, ending round.", Instance.Config.AllowDebug);
					EndGame(ev, key);
					return;
				}
				catch (Exception e) 
				{
					Log.Error($"Exception during final checks: {e}");
				}
			}
		}

		// Central round end function
		private void EndGame(EndingRoundEventArgs ev, string team) 
		{			
			ev.LeadingTeam = ConvertTeam(team);
			ev.IsRoundEnded = true;
			ev.IsAllowed = true;
			
			API.BlacklistedPlayers.Clear();
			API.ModifiedRoles.Clear();
			
			if (Instance.Config.AllowVerbose)
				Log.Info($"Force ending with {ev.LeadingTeam} as the lead team.");
		}

		// Middle man to convert string into RoundSummary.LeadingTeam to account for dictionary keys.
		private LeadingTeam ConvertTeam(string arg) 
		{
			string team = arg.ToLower();
			if (team.StartsWith("facilityforces"))
				return LeadingTeam.FacilityForces;
			if (team.StartsWith("chaosinsurgency"))
				return LeadingTeam.ChaosInsurgency;
			if (team.StartsWith("anomalies"))
				return LeadingTeam.Anomalies;
			if (team.StartsWith("draw"))
				return LeadingTeam.Draw;

			Log.Debug($"Could not parse {arg} into a team, returning as a draw.", Instance.Config.AllowDebug);
			return LeadingTeam.Draw;
		}
	}
}