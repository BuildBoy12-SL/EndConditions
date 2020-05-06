using System;
using System.Collections.Generic;
using System.Linq;
using EXILED;
using EXILED.Extensions;

namespace EndConditions
{
	public class EventHandlers
	{
		public Plugin plugin;
		public EventHandlers(Plugin plugin) => this.plugin = plugin;

		public bool escapedClassD = false;
		public bool escapedScientists = false;

		//Reset of values
		public void OnWaitingForPlayers() {
			escapedClassD = false;
			escapedScientists = false;
		}

		//Hold for escape condition checks
		public void CheckEscape(ref CheckEscapeEvent ev) {
			if (ev.Player.GetRole() == RoleType.ClassD)
				escapedClassD = true;
			else if (ev.Player.GetRole() == RoleType.Scientist)
				escapedScientists = true;
		}

		public void OnCheckRoundEnd(ref CheckRoundEndEvent ev) {
			if (!plugin.DefaultEndConditions) {
				ev.Allow = false;
			}
			//Check if the warhead has detonated && if the detonation winner is set
			bool flag = Map.IsNukeDetonated && plugin.DetonationWinner != "none";
			if (flag) { 
				EndGame(ev, plugin.DetonationWinner); 
			}
			else {
				//Shove all alive roles into the list
				List<string> list = new List<string>();
				foreach (ReferenceHub hub in Player.GetHubs()) {
					if(hub.GetRole() != RoleType.Spectator)
						list.Add(hub.GetRole().ToString().ToLower());
				}				
				//If ignoretut, do the thing.
				if (plugin.IgnoreTut)
					list.RemoveAll(item => item == "tutorial");
				//Put all the lists from the core dictionary and check em
				List<string> values = new List<string>();
				foreach (var v in plugin.dict) {
					values = v.Value;
					//The actual check
					bool existsCheck = !list.Except(values).Any();
					if (existsCheck) {
						if (plugin.debug)
							Log.Info("Check passed.");

						try {
							//Get the key that contains the name and escape conditions
							var key = plugin.dict.FirstOrDefault(x => x.Value == values).Key;
							if (plugin.debug)
								Log.Info($"Using conditions from condition name: '{key}'");

							//Check for escape conditions
							if (key.Contains("+classd")) {
								if (escapedClassD) {
									EndGame(ev, key);
								}
							}
							else if (key.Contains("-classd")) {
								if (!escapedClassD) {
									EndGame(ev, key);
								}
							}
							else if (key.Contains("+science")) {
								if (escapedScientists) {
									EndGame(ev, key);
								}
							}
							else if (key.Contains("-science")) {
								if (!escapedScientists) {
									EndGame(ev, key);
								}
							}
							else {
								EndGame(ev, key);
							}
						}
						catch (Exception e) {
							Log.Error($"Exception during final checks: {e}");
						}
					}				
				}				
			}
		}

		//Central round end function
		public void EndGame(CheckRoundEndEvent ev, string team) {
			ev.LeadingTeam = (RoundSummary.LeadingTeam)ConvertTeam(team);
			ev.ForceEnd = true;
			if (plugin.debug || plugin.verbose)
				Log.Info($"Force ending with {ev.LeadingTeam} as the lead team.");
		}

		//Middle man to convert string into RoundSummary.LeadingTeam to account for dictionary keys. Yes I know Enum.Parse exists, no it wouldn't work in this case. I think.
		public int ConvertTeam(string arg) {
			var team = arg.ToLower().StartsWith("facilityforces") ? 0
					 : arg.ToLower().StartsWith("chaosinsurgency") ? 1
					 : arg.ToLower().StartsWith("anomalies") ? 2
					 : arg.ToLower().StartsWith("draw") ? 3
					 : -1;

			if (team != -1)
				return team;
			else {
				if (plugin.debug)
					Log.Warn($"Could not parse {arg} into a team, returning as a draw.");

				return 3;
			}
		}
	}
}