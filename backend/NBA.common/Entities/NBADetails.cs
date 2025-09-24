using System;

namespace NBA.common.Entities;

public sealed record NBADetails(
    List<TeamRow> Teams,
    List<TeamGameRow> Games,
    List<RosterRow> Roster);
