namespace EndConditions
{
    using Exiled.API.Enums;
    using System.Collections.Generic;

    public readonly struct Condition
    {
        public Condition(LeadingTeam leadingTeam, string name, List<string> roleConditions)
        {
            LeadingTeam = leadingTeam;
            Name = name;
            RoleConditions = roleConditions;
        }

        public LeadingTeam LeadingTeam { get; }
        public string Name { get; }
        public List<string> RoleConditions { get; }
    }
}