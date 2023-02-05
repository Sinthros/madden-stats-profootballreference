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
                    values = new object[] { p1.Name, p1.Completions, p1.GamesPlayed, p1.AttemptedPasses, p1.PassingYards, p1.PassingTouchdowns, p1.Interceptions, p1.SacksTaken, p1.Team };
                    break;
                case KickingPlayer p2:
                    values = new object[] { p2.Name, p2.PuntAttempts, p2.GamesPlayed, p2.ExtraPointsMade, p2.ExtraPointsAttempted, p2.FieldGoalsMade, p2.FieldGoalsAttempted, p2.PuntYards, p2.PuntsBlocked, p2.Team };
                    break;
                case DefensePlayer p3:
                    values = new object[] { p3.Name, p3.Interceptions, p3.GamesPlayed, p3.Sacks, p3.SoloTackles, p3.AssistedTackles, p3.TacklesForLoss, p3.InterceptionTouchdowns, p3.PassesDefended, p3.FumblesRecovered, p3.FumbleYards, p3.FumbleTouchdowns, p3.ForcedFumbles,p3.Safety, p3.Team };
                    break;
                case ReceivingPlayer p4:
                    values = new object[] { p4.Name, p4.Receptions, p4.GamesPlayed, p4.YardsReceived, p4.ReceivingTouchdowns, p4.Team };
                    break;
                case ReturningPlayer p5:
                    values = new object[] { p5.Name, p5.KickReturns, p5.GamesPlayed, p5.KickReturnYards, p5.KickReturnTouchdowns, p5.PuntReturnAttempts, p5.PuntReturnYards, p5.PuntReturnTouchdowns, p5.Team};
                    break;
                case RushingPlayer p6:
                    values = new object[] { p6.Name, p6.RushAttempts, p6.GamesPlayed, p6.RushingYards, p6.RushTouchdowns, p6.Fumbles, p6.Team };
                    break;
                case PuntingPlayer p7:
                    values = new object[] { p7.Name, p7.PuntAttempts, p7.GamesPlayed, p7.ExtraPointsMade, p7.ExtraPointsAttempted, p7.FieldGoalsMade, p7.FieldGoalsAttempted, p7.PuntYards, p7.PuntsBlocked, p7.Team };
                    break;
            }
            WriteFields(worksheet, row, values);
        }

        private static void WriteHeaders<T>(IXLWorksheet worksheet) where T : Player
        {
            worksheet.Cell("A1").Value = "Player Name";
            worksheet.Cell("C1").Value = "GAMESPLAYED";
            if (typeof(T) == typeof(PassingPlayer))
            {
                worksheet.Cell("B1").Value = "PASSCOMPLETED";
                worksheet.Cell("D1").Value = "PASSATTEMPTS";
                worksheet.Cell("E1").Value = "PASSYARDS";
                worksheet.Cell("F1").Value = "PASSTDS";
                worksheet.Cell("G1").Value = "PASSINTS";
                worksheet.Cell("H1").Value = "PASSSACKED";
                worksheet.Cell("I1").Value = "TEAM";
            }
            if (typeof(T) == typeof(KickingPlayer))
            {
                worksheet.Cell("B1").Value = "PUNTATTEMPTS";
                worksheet.Cell("D1").Value = "KICKEPMADE";
                worksheet.Cell("E1").Value = "KICKEPATTEMPTS";
                worksheet.Cell("F1").Value = "KICKFGMADE";
                worksheet.Cell("G1").Value = "KICKFGATTEMPTS";
                worksheet.Cell("H1").Value = "PUNTYARDS";
                worksheet.Cell("I1").Value = "PUNTBLOCKED";
                worksheet.Cell("J1").Value = "TEAM";
            }
            if (typeof(T) == typeof(PuntingPlayer))
            {
                worksheet.Cell("B1").Value = "PUNTATTEMPTS";
                worksheet.Cell("D1").Value = "KICKEPMADE";
                worksheet.Cell("E1").Value = "KICKEPATTEMPTS";
                worksheet.Cell("F1").Value = "KICKFGMADE";
                worksheet.Cell("G1").Value = "KICKFGATTEMPTS";
                worksheet.Cell("H1").Value = "PUNTYARDS";
                worksheet.Cell("I1").Value = "PUNTBLOCKED";
                worksheet.Cell("J1").Value = "TEAM";
            }
            if (typeof(T) == typeof(DefensePlayer))
            {
                worksheet.Cell("B1").Value = "DSECINTS";
                worksheet.Cell("D1").Value = "DLINESACKS";

                worksheet.Cell("E1").Value = "DEFTACKLES";
                worksheet.Cell("F1").Value = "ASSDEFTACKLES";
                worksheet.Cell("G1").Value = "DEFTACKLESFORLOSS";

                worksheet.Cell("H1").Value = "DSECINTTDS";
                worksheet.Cell("I1").Value = "DEFPASSDEFLECTIONS";

                worksheet.Cell("J1").Value = "DLINEFUMBLERECOVERIES";
                worksheet.Cell("K1").Value = "DLINEFUMBLERECOVERYYARDS";
                worksheet.Cell("L1").Value = "DLINEFUMBLETDS";
                worksheet.Cell("M1").Value = "DLINEFORCEDFUMBLES";
                worksheet.Cell("N1").Value = "DLINESAFETIES";
                worksheet.Cell("O1").Value = "TEAM";
            }
            if (typeof(T) == typeof(ReceivingPlayer))
            {
                worksheet.Cell("B1").Value = "RECEIVECATCHES";
                worksheet.Cell("D1").Value = "RECEIVEYARDS";
                worksheet.Cell("E1").Value = "RECEIVETDS";
                worksheet.Cell("F1").Value = "TEAM";
    
            }
            if (typeof(T) == typeof(ReturningPlayer))
            {

                worksheet.Cell("B1").Value = "KRETATTEMPTS";
                worksheet.Cell("D1").Value = "KRETYARDS";
                worksheet.Cell("E1").Value = "KRETTDS";
                worksheet.Cell("F1").Value = "PRETATTEMPTS";
                worksheet.Cell("G1").Value = "PRETYDS";
                worksheet.Cell("H1").Value = "PRETTDS";
                worksheet.Cell("I1").Value = "TEAM";
            }
            if (typeof(T) == typeof(RushingPlayer))
            {
                worksheet.Cell("B1").Value = "RUSHATTEMPTS";
                worksheet.Cell("D1").Value = "RUSHYARDS";
                worksheet.Cell("E1").Value = "RUSHTDS";
                worksheet.Cell("F1").Value = "RUSHFUMBLES";
                worksheet.Cell("G1").Value = "TEAM";
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
