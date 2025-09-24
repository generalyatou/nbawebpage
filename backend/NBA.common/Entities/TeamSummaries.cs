using System;

namespace NBA.common.Entities;

public sealed record TeamSummaries(
    string TeamName, string Stadium, string? Logo, string? MVP,
    int Played, int Won, int Lost, int PlayedHome, int PlayedAway,
    string BiggestWin, string BiggestLoss,
    string LastGameStadium, string LastGameDate);
