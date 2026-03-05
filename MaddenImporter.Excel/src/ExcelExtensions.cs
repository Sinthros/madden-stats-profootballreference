using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using MaddenImporter.Models.Player;

namespace MaddenImporter.Excel
{
    internal static class ExcelExtensions
    {
        private static Dictionary<Type, string> playerMapper = new Dictionary<Type, string>{
            { typeof(PassingPlayer), "PASSING" },
            { typeof(KickingPlayer), "KICKING" },
            { typeof(PuntingPlayer), "PUNTING" },
            { typeof(DefensePlayer), "DEFENSE" },
            { typeof(ReceivingPlayer), "RECEIVING" },
            { typeof(ReturningPlayer), "RETURN" },
            { typeof(RushingPlayer), "RUSHING" }
        };

        private static void WriteFields(IXLWorksheet worksheet, int row, object[] values)
        {
            for (int i = 0; i < values.Length; i++)
                worksheet.Cell(row, i + 1).Value = values[i];
        }

        private static void WritePlayer<T>(IXLWorksheet worksheet, int row, T player) where T : Player
        {
            object[] values = new object[1];
            switch (player)
            {
                case PassingPlayer p1:
                    values = new object[] { p1.Name, p1.Completions, p1.GamesPlayed, p1.GamesStarted, p1.AttemptedPasses, p1.PassingYards, p1.PassingTouchdowns, p1.Interceptions, p1.SacksTaken, p1.Team, p1.PlayerLink, p1.Age, p1.MinYear, p1.MaxYear, p1.DraftTeam, p1.DraftRound, p1.DraftPick };
                    break;
                case KickingPlayer p2:
                    values = new object[] { p2.Name, p2.PuntAttempts, p2.GamesPlayed, p2.GamesPlayed, p2.ExtraPointsMade, p2.ExtraPointsAttempted, p2.FieldGoalsMade, p2.FieldGoalsAttempted, p2.PuntYards, p2.PuntNetYards, p2.PuntsBlocked, p2.Team, p2.PlayerLink, p2.Age, p2.MinYear, p2.MaxYear, p2.DraftTeam, p2.DraftRound, p2.DraftPick  };
                    break;
                case DefensePlayer p3:
                    values = new object[] { p3.Name, p3.Interceptions, p3.GamesPlayed, p3.GamesStarted, p3.Sacks, p3.SoloTackles, p3.AssistedTackles, p3.TacklesForLoss, p3.InterceptionTouchdowns, p3.PassesDefended, p3.FumblesRecovered, p3.FumbleYards, p3.FumbleTouchdowns, p3.ForcedFumbles,p3.Safety, p3.Team, p3.PlayerLink, p3.Age, p3.MinYear, p3.MaxYear, p3.DraftTeam, p3.DraftRound, p3.DraftPick  };
                    break;
                case ReceivingPlayer p4:
                    values = new object[] { p4.Name, p4.Receptions, p4.GamesPlayed, p4.GamesStarted, p4.YardsReceived, p4.ReceivingTouchdowns, p4.Team, p4.PlayerLink, p4.Age, p4.MinYear, p4.MaxYear, p4.DraftTeam, p4.DraftRound, p4.DraftPick };
                    break;
                case ReturningPlayer p5:
                    values = new object[] { p5.Name, p5.KickReturns, p5.GamesPlayed, p5.GamesStarted, p5.KickReturnYards, p5.KickReturnTouchdowns, p5.PuntReturnAttempts, p5.PuntReturnYards, p5.PuntReturnTouchdowns, p5.Team, p5.PlayerLink, p5.Age, p5.MinYear, p5.MaxYear, p5.DraftTeam, p5.DraftRound, p5.DraftPick };
                    break;
                case RushingPlayer p6:
                    values = new object[] { p6.Name, p6.RushAttempts, p6.GamesPlayed, p6.GamesStarted, p6.RushingYards, p6.RushTouchdowns, p6.Fumbles, p6.Team, p6.PlayerLink, p6.Age, p6.MinYear, p6.MaxYear, p6.DraftTeam, p6.DraftRound, p6.DraftPick };
                    break;
                case PuntingPlayer p7:
                    values = new object[] { p7.Name, p7.PuntAttempts, p7.GamesPlayed, p7.GamesPlayed, p7.ExtraPointsMade, p7.ExtraPointsAttempted, p7.FieldGoalsMade, p7.FieldGoalsAttempted, p7.PuntYards, p7.PuntNetYards, p7.PuntsBlocked, p7.Team, p7.PlayerLink, p7.Age, p7.MinYear, p7.MaxYear, p7.DraftTeam, p7.DraftRound, p7.DraftPick };
                    break;
            }
            WriteFields(worksheet, row, values);
        }

        private static void WriteHeaders<T>(IXLWorksheet worksheet) where T : Player
        {
            worksheet.Cell("A1").Value = "PLAYERNAME";
            worksheet.Cell("C1").Value = "GAMESPLAYED";
            worksheet.Cell("D1").Value = "GAMESSTARTED";

            if (typeof(T) == typeof(PassingPlayer))
            {
                worksheet.Cell("B1").Value = "PASSCOMPLETED";
                worksheet.Cell("E1").Value = "PASSATTEMPTS";
                worksheet.Cell("F1").Value = "PASSYARDS";
                worksheet.Cell("G1").Value = "PASSTDS";
                worksheet.Cell("H1").Value = "PASSINTS";
                worksheet.Cell("I1").Value = "PASSSACKED";
                worksheet.Cell("J1").Value = "TEAM";
                worksheet.Cell("K1").Value = "PLAYERLINK";
                worksheet.Cell("L1").Value = "AGE";
                worksheet.Cell("M1").Value = "MINYEAR";
                worksheet.Cell("N1").Value = "MAXYEAR";
                worksheet.Cell("O1").Value = "DRAFTTEAM";
                worksheet.Cell("P1").Value = "DRAFTROUND";
                worksheet.Cell("Q1").Value = "DRAFTPICK";
            }
            if (typeof(T) == typeof(KickingPlayer))
            {
                worksheet.Cell("B1").Value = "PUNTATTEMPTS";
                worksheet.Cell("E1").Value = "KICKEPMADE";
                worksheet.Cell("F1").Value = "KICKEPATTEMPTS";
                worksheet.Cell("G1").Value = "KICKFGMADE";
                worksheet.Cell("H1").Value = "KICKFGATTEMPTS";
                worksheet.Cell("I1").Value = "PUNTYARDS";
                worksheet.Cell("J1").Value = "PUNTNETYARDS";
                worksheet.Cell("K1").Value = "PUNTBLOCKED";
                worksheet.Cell("L1").Value = "TEAM";
                worksheet.Cell("M1").Value = "PLAYERLINK";
                worksheet.Cell("N1").Value = "AGE";
                worksheet.Cell("O1").Value = "MINYEAR";
                worksheet.Cell("P1").Value = "MAXYEAR";
                worksheet.Cell("Q1").Value = "DRAFTTEAM";
                worksheet.Cell("R1").Value = "DRAFTROUND";
                worksheet.Cell("S1").Value = "DRAFTPICK";
            }
            if (typeof(T) == typeof(PuntingPlayer))
            {
                worksheet.Cell("B1").Value = "PUNTATTEMPTS";
                worksheet.Cell("E1").Value = "KICKEPMADE";
                worksheet.Cell("F1").Value = "KICKEPATTEMPTS";
                worksheet.Cell("G1").Value = "KICKFGMADE";
                worksheet.Cell("H1").Value = "KICKFGATTEMPTS";
                worksheet.Cell("I1").Value = "PUNTYARDS";
                worksheet.Cell("J1").Value = "PUNTNETYARDS";
                worksheet.Cell("K1").Value = "PUNTBLOCKED";
                worksheet.Cell("L1").Value = "TEAM";
                worksheet.Cell("M1").Value = "PLAYERLINK";
                worksheet.Cell("N1").Value = "AGE";
                worksheet.Cell("O1").Value = "MINYEAR";
                worksheet.Cell("P1").Value = "MAXYEAR";
                worksheet.Cell("Q1").Value = "DRAFTTEAM";
                worksheet.Cell("R1").Value = "DRAFTROUND";
                worksheet.Cell("S1").Value = "DRAFTPICK";
            }
            if (typeof(T) == typeof(DefensePlayer))
            {
                worksheet.Cell("B1").Value = "DSECINTS";
                worksheet.Cell("E1").Value = "DLINESACKS";
                worksheet.Cell("F1").Value = "DEFTACKLES";
                worksheet.Cell("G1").Value = "ASSDEFTACKLES";
                worksheet.Cell("H1").Value = "DEFTACKLESFORLOSS";
                worksheet.Cell("I1").Value = "DSECINTTDS";
                worksheet.Cell("J1").Value = "DEFPASSDEFLECTIONS";
                worksheet.Cell("K1").Value = "DLINEFUMBLERECOVERIES";
                worksheet.Cell("L1").Value = "DLINEFUMBLERECOVERYYARDS";
                worksheet.Cell("M1").Value = "DLINEFUMBLETDS";
                worksheet.Cell("N1").Value = "DLINEFORCEDFUMBLES";
                worksheet.Cell("O1").Value = "DLINESAFETIES";
                worksheet.Cell("P1").Value = "TEAM";
                worksheet.Cell("Q1").Value = "PLAYERLINK";
                worksheet.Cell("R1").Value = "AGE";
                worksheet.Cell("S1").Value = "MINYEAR";
                worksheet.Cell("T1").Value = "MAXYEAR";
                worksheet.Cell("U1").Value = "DRAFTTEAM";
                worksheet.Cell("V1").Value = "DRAFTROUND";
                worksheet.Cell("W1").Value = "DRAFTPICK";
            }
            if (typeof(T) == typeof(ReceivingPlayer))
            {
                worksheet.Cell("B1").Value = "RECEIVECATCHES";
                worksheet.Cell("E1").Value = "RECEIVEYARDS";
                worksheet.Cell("F1").Value = "RECEIVETDS";
                worksheet.Cell("G1").Value = "TEAM";
                worksheet.Cell("H1").Value = "PLAYERLINK";
                worksheet.Cell("I1").Value = "AGE";
                worksheet.Cell("J1").Value = "MINYEAR";
                worksheet.Cell("K1").Value = "MAXYEAR";
                worksheet.Cell("L1").Value = "DRAFTTEAM";
                worksheet.Cell("M1").Value = "DRAFTROUND";
                worksheet.Cell("N1").Value = "DRAFTPICK";
            }
            if (typeof(T) == typeof(ReturningPlayer))
            {
                worksheet.Cell("B1").Value = "KRETATTEMPTS";
                worksheet.Cell("E1").Value = "KRETYARDS";
                worksheet.Cell("F1").Value = "KRETTDS";
                worksheet.Cell("G1").Value = "PRETATTEMPTS";
                worksheet.Cell("H1").Value = "PRETYARDS";
                worksheet.Cell("I1").Value = "PRETTDS";
                worksheet.Cell("J1").Value = "TEAM";
                worksheet.Cell("K1").Value = "PLAYERLINK";
                worksheet.Cell("L1").Value = "AGE";
                worksheet.Cell("M1").Value = "MINYEAR";
                worksheet.Cell("N1").Value = "MAXYEAR";
                worksheet.Cell("O1").Value = "DRAFTTEAM";
                worksheet.Cell("P1").Value = "DRAFTROUND";
                worksheet.Cell("Q1").Value = "DRAFTPICK";
            }
            if (typeof(T) == typeof(RushingPlayer))
            {
                worksheet.Cell("B1").Value = "RUSHATTEMPTS";
                worksheet.Cell("E1").Value = "RUSHYARDS";
                worksheet.Cell("F1").Value = "RUSHTDS";
                worksheet.Cell("G1").Value = "RUSHFUMBLES";
                worksheet.Cell("H1").Value = "TEAM";
                worksheet.Cell("I1").Value = "PLAYERLINK";
                worksheet.Cell("J1").Value = "AGE";
                worksheet.Cell("K1").Value = "MINYEAR";
                worksheet.Cell("L1").Value = "MAXYEAR";
                worksheet.Cell("M1").Value = "DRAFTTEAM";
                worksheet.Cell("N1").Value = "DRAFTROUND";
                worksheet.Cell("O1").Value = "DRAFTPICK";
            }
        }

        public static void WritePlayerSheet<T>(IXLWorkbook workbook, List<T> players) where T : Player
        {
            playerMapper.TryGetValue(typeof(T), out string sheetName);
            var worksheet = workbook.AddWorksheet(sheetName);
            WriteHeaders<T>(worksheet);
            for (int i = 0; i < players.Count; i++)
                WritePlayer(worksheet, i + 2, players[i]);
            Console.WriteLine($"Wrote Excel sheet {typeof(T)}.");
        }
    }
}
