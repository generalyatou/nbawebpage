using System;

namespace NBA.common.Entities;

public record TeamGameRow(
    int TeamID, string TeamName, string Stadium,
    int GameID, DateTime GameDateTime,
    int TeamScore, int OppScore, int IsHome, int OppTeamID,
    int HomeTeamID, int AwayTeamID, int MVPPlayerID,
    int IsWin, int IsLoss, int Margin);
