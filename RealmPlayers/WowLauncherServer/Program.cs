﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

using VF_RealmPlayersDatabase;
using UploaderCommunication = VF_RealmPlayersDatabase.UploaderCommunication;

namespace VF_WoWLauncherServer
{
    static class Program
    {
        public static string g_RPPDBFolder = VF_RealmPlayersDatabase.Utility.DefaultServerLocation + "VF_RealmPlayersData\\RPPDatabase\\";
        public static string g_RDDBFolder = VF_RealmPlayersDatabase.Utility.DefaultServerLocation + "VF_RealmPlayersData\\RDDatabase\\";

        public static RPPDatabaseHandler g_RPPDatabaseHandler;
        public static RDDatabaseHandler g_RDDatabaseHandler;
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            bool debugMode = false;
            if (System.IO.Directory.Exists(g_RPPDBFolder) == false)
            {
                g_RPPDBFolder = g_RPPDBFolder.Replace(VF_RealmPlayersDatabase.Utility.DefaultServerLocation, VF_RealmPlayersDatabase.Utility.DefaultDebugLocation);
                g_RDDBFolder = g_RDDBFolder.Replace(VF_RealmPlayersDatabase.Utility.DefaultServerLocation, VF_RealmPlayersDatabase.Utility.DefaultDebugLocation);
                AddonDatabaseService.g_AddonUploadDataFolder = AddonDatabaseService.g_AddonUploadDataFolder.Replace(VF_RealmPlayersDatabase.Utility.DefaultServerLocation, VF_RealmPlayersDatabase.Utility.DefaultDebugLocation);
                AddonDatabaseService.g_AddonUploadStatsFolder = AddonDatabaseService.g_AddonUploadStatsFolder.Replace(VF_RealmPlayersDatabase.Utility.DefaultServerLocation, VF_RealmPlayersDatabase.Utility.DefaultDebugLocation);
                RPPDatabaseHandler.g_AddonContributionsBackupFolder = RPPDatabaseHandler.g_AddonContributionsBackupFolder.Replace(VF_RealmPlayersDatabase.Utility.DefaultServerLocation, VF_RealmPlayersDatabase.Utility.DefaultDebugLocation);

                RDDatabaseHandler.g_AddonContributionsBackupFolder = RDDatabaseHandler.g_AddonContributionsBackupFolder.Replace(VF_RealmPlayersDatabase.Utility.DefaultServerLocation, VF_RealmPlayersDatabase.Utility.DefaultDebugLocation);
                debugMode = true;
            }
            AppDomain.CurrentDomain.ProcessExit += CurrentDomain_ProcessExit;

            VF_WoWLauncher.ConsoleUtility.CreateConsole();

            /*Some Testing*/
            //try
            {
                RealmDatabase newRealm = new RealmDatabase(WowRealm.Al_Akir);
                Logger.ConsoleWriteLine("Started Loading!!!");
                newRealm.LoadDatabase("D:\\VF_RealmPlayersData\\RPPDatabase\\Database\\Al_Akir", new DateTime(2012, 5, 1, 0, 0, 0));//new DateTime(2015, 9, 1, 0, 0, 0));//, 
                Logger.ConsoleWriteLine("Loading...");
                newRealm.WaitForLoad(RealmDatabase.LoadStatus.EverythingLoaded);
                Logger.ConsoleWriteLine("Everything Loaded!!!");
                if(false)
                {
                    Logger.ConsoleWriteLine("Starting Saving to SQL!!!");
                    SQLMigration.SaveFakeContributorData();
                    SQLMigration.SaveRealmDatabase(newRealm);
                    Logger.ConsoleWriteLine("Done Saving to SQL!!!");
                }
                else
                {
                    Logger.ConsoleWriteLine("Starting Load from SQL!!!");
                    RealmDatabase sqlRealm = SQLMigration.LoadRealmDatabase(WowRealm.Al_Akir);
                    Logger.ConsoleWriteLine("Done Loading from SQL!!!");

                    Logger.ConsoleWriteLine("Starting Comparing realm datas!!!");
                    foreach (var player in newRealm.Players)
                    {
                        try
                        {
                            var playerData = sqlRealm.Players[player.Key];
                            if (player.Value.Guild.IsSame(playerData.Guild) == false)
                            {
                                Logger.ConsoleWriteLine("\"" + player.Key + "\" Guild data was not same!");
                            }
                            if (player.Value.Character.IsSame(playerData.Character) == false)
                            {
                                Logger.ConsoleWriteLine("\"" + player.Key + "\" Character data was not same!");
                            }
                            if (player.Value.Honor.IsSame(playerData.Honor) == false)
                            {
                                Logger.ConsoleWriteLine("\"" + player.Key + "\" Honor data was not same!");
                            }
                            if (player.Value.Gear.IsSame(playerData.Gear) == false)
                            {
                                Logger.ConsoleWriteLine("\"" + player.Key + "\" Gear data was not same!");
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("EXCEPTION OCCURED\n----------------------\n" + ex.ToString());
                        }
                    }
                    Logger.ConsoleWriteLine("Done comparing realm datas!!!");
                }
                Logger.ConsoleWriteLine("Everything Done!!!");
            }
            //catch (Exception ex)
            {
                //Console.WriteLine("EXCEPTION OCCURED\n----------------------\n" + ex.ToString());
            }
            while(true)
            {
                System.Threading.Thread.Sleep(1000);
            }
            return;
            /*Some Testing*/

            if (debugMode == false)
            {
                Console.WriteLine("Waiting for ContributorDB to load");
                while (ContributorDB.GetMongoDB() == null)
                {
                    ContributorDB.Initialize();
                    System.Threading.Thread.Sleep(100);
                    Console.Write(".");
                }
                Console.WriteLine("ContributorDB loaded!");
            }

            //VF_RealmPlayersDatabase.Deprecated.ContributorHandler.Initialize(g_RPPDBFolder + "Database\\");
            g_RPPDatabaseHandler = new RPPDatabaseHandler(g_RPPDBFolder);
            g_RDDatabaseHandler = new RDDatabaseHandler(g_RDDBFolder, g_RPPDatabaseHandler);
            AddonDatabaseService.HandleUnhandledFiles("VF_RealmPlayers");
            AddonDatabaseService.HandleUnhandledFiles("VF_RaidDamage");
            AddonDatabaseService.HandleUnhandledFiles("VF_RealmPlayersTBC");
            AddonDatabaseService.HandleUnhandledFiles("VF_RaidStatsTBC");
            try
            {
                Application.Run(new MainWindow());
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
                g_RDDatabaseHandler.Shutdown();
                g_RPPDatabaseHandler.Shutdown();
            }
        }
        static void CurrentDomain_ProcessExit(object sender, EventArgs e)
        {
            g_RPPDatabaseHandler.Shutdown();
        }
    }
}
