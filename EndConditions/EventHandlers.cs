namespace EndConditions
{
    using Exiled.API.Enums;
    using Exiled.API.Features;
    using Exiled.Events.EventArgs;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class EventHandlers
    {
        public EventHandlers(Config config) => _config = config;
        private readonly Config _config;
        
        internal static readonly Dictionary<string, List<string>> EndConditions = new();

        public void OnRoundStart()
        {
            if (_config.RoundEndFf)
            {
                foreach (var player in Player.List)
                    player.IsFriendlyFireEnabled = ServerConsole.FriendlyFire;
            }
        }

        public void OnCheckRoundEnd(EndingRoundEventArgs ev)
        {
            if (!_config.AllowDefaultEndConditions)
            {
                ev.IsAllowed = false;
                ev.IsRoundEnded = false;
            }

            // Check if the warhead has detonated && if the detonation winner is set
            if (Warhead.IsDetonated && _config.DetonationWinner != "none")
            {
                EndGame(ev, _config.DetonationWinner);
                return;
            }

            var escAdditions = new Dictionary<string, bool>
            {
                {"-classd", RoundSummary.escaped_ds == 0},
                {"+classd", RoundSummary.escaped_ds != 0},
                {"-science", RoundSummary.escaped_scientists == 0},
                {"+science", RoundSummary.escaped_scientists != 0},
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

                if (ply.CustomInfo == "<color=#FF0000>SCP-035</color>")
                {
                    list.Add("scp035");
                    continue;
                }

                if (ply.Role == RoleType.Tutorial && _config.IgnoreTutorials)
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
                    Log.Debug($"Using conditions from condition name: '{key}'", _config.AllowDebug);
                    // Check for escape conditions
                    string[] splitKey = key.Split(' ');
                    List<string> conds = splitKey.Where(str => escAdditions.Keys.Contains(str)).ToList();
                    List<string> failedConds = conds.Where(x => !escAdditions[x]).ToList();
                    if (failedConds.Count > 0)
                    {
                        Log.Debug($"Failed at: {string.Join(", ", failedConds)}", _config.AllowDebug);
                        continue;
                    }

                    Log.Debug("Escape checks passed, ending round.", _config.AllowDebug);
                    EndGame(ev, key);
                    return;
                }
                catch (Exception e)
                {
                    Log.Error($"Exception during final checks: {e}");
                }
            }
        }

        private void EndGame(EndingRoundEventArgs ev, string team)
        {
            ev.LeadingTeam = ConvertTeam(team);
            Round.ForceEnd();
            ev.IsRoundEnded = true;

            API.BlacklistedPlayers.Clear();
            API.ModifiedRoles.Clear();

            Log.Debug($"Force ending with {ev.LeadingTeam} as the lead team.", _config.AllowDebug);

            if (_config.RoundEndFf)
            {
                foreach (var player in Player.List)
                    player.IsFriendlyFireEnabled = true;
            }
        }

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

            Log.Debug($"Could not parse {arg} into a team, returning as a draw.", _config.AllowDebug);
            return LeadingTeam.Draw;
        }
    }
}