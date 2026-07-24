using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;
using System.Windows.Forms;
using Microsoft.Win32;

namespace Good3dLauncher
{
	// Token: 0x02000003 RID: 3
	internal class Program
	{
		// Token: 0x06000004 RID: 4
		[DllImport("kernel32.dll", SetLastError = true)]
		private static extern bool CheckRemoteDebuggerPresent(IntPtr hProcess, ref bool isDebuggerPresent);

		// Token: 0x06000005 RID: 5 RVA: 0x0000236C File Offset: 0x0000056C
		[MethodImpl(MethodImplOptions.NoInlining)]
		private static string Xs(byte[] b)
		{
			char[] array = new char[b.Length];
			for (int i = 0; i < b.Length; i++)
			{
				array[i] = (char)((int)b[i] ^ (90 ^ (i & 31)));
			}
			return new string(array);
		}

		// Token: 0x17000001 RID: 1
		// (get) Token: 0x06000006 RID: 6 RVA: 0x000023B0 File Offset: 0x000005B0
		private static string SB
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return Program.Xs(Program._sb);
			}
		}

		// Token: 0x17000002 RID: 2
		// (get) Token: 0x06000007 RID: 7 RVA: 0x000023CC File Offset: 0x000005CC
		private static string GB
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return Program.Xs(Program._gb);
			}
		}

		// Token: 0x17000003 RID: 3
		// (get) Token: 0x06000008 RID: 8 RVA: 0x000023E8 File Offset: 0x000005E8
		private static string CB
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return Program.Xs(Program._cb);
			}
		}

		// Token: 0x17000004 RID: 4
		// (get) Token: 0x06000009 RID: 9 RVA: 0x00002404 File Offset: 0x00000604
		private static string PU
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return Program.Xs(Program._pu);
			}
		}

		// Token: 0x17000005 RID: 5
		// (get) Token: 0x0600000A RID: 10 RVA: 0x00002420 File Offset: 0x00000620
		private static string AD
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return Program.Xs(Program._ad);
			}
		}

		// Token: 0x17000006 RID: 6
		// (get) Token: 0x0600000B RID: 11 RVA: 0x0000243C File Offset: 0x0000063C
		private static string AA
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return Program.Xs(Program._aa);
			}
		}

		// Token: 0x17000007 RID: 7
		// (get) Token: 0x0600000C RID: 12 RVA: 0x00002458 File Offset: 0x00000658
		private static string LE
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return Program.Xs(Program._le);
			}
		}

		// Token: 0x17000008 RID: 8
		// (get) Token: 0x0600000D RID: 13 RVA: 0x00002474 File Offset: 0x00000674
		private static string PN
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				return Program.Xs(Program._pr);
			}
		}

		// Token: 0x0600000E RID: 14 RVA: 0x00002490 File Offset: 0x00000690
		[STAThread]
		private static void Main(string[] args)
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			try
			{
				Program.InitializeLogging(args);
				if (!Program.PassesBasicRuntimeProtection())
				{
					Program.ShowMsg("Security", Program.lastErrorMessage ?? "Check failed.");
				}
				else
				{
					string text = (args.Length > 0) ? args[0] : string.Empty;
					if (args.Length == 0 || string.IsNullOrEmpty(text) || string.Equals(text, "/install", StringComparison.OrdinalIgnoreCase) || string.Equals(text, "-install", StringComparison.OrdinalIgnoreCase))
					{
						Program.bootstrapForm = new BootstrapForm();
						Program.bootstrapForm.Show();
						Application.DoEvents();
						if (!Program.RunInstallFlow())
						{
							string message = string.IsNullOrEmpty(Program.lastErrorMessage) ? "Installation failed." : Program.lastErrorMessage;
							Program.UpdateBootstrapStatus("Install failed", 100);
							Program.ShowMsg("Error", message);
							Program.bootstrapForm.Close();
						}
						else
						{
							Thread.Sleep(1200);
							Program.bootstrapForm.Close();
						}
					}
					else
					{
						Dictionary<string, string> dictionary = Program.ParseLaunchURI(text);
						if (dictionary == null)
						{
							Program.ShowMsg("Error", "Invalid launch URI.");
						}
						else
						{
							Program.bootstrapForm = new BootstrapForm();
							Program.bootstrapForm.Show();
							Application.DoEvents();
							bool flag;
							Program.EnsureLauncherUpToDate(out flag);
							Program.UpdateBootstrapStatus("Preparing...", 25);
							string text2 = Program.EnsureClient();
							if (string.IsNullOrEmpty(text2))
							{
								Program.ShowMsg("Error", "Failed to prepare client.");
								Program.bootstrapForm.Close();
							}
							else
							{
								Program.EnsureProtocolHandlerRegistered();
								Program.UpdateBootstrapStatus("Launching...", 75);
								Thread.Sleep(300);
								if (!Program.LaunchClient(text2))
								{
									Program.ShowMsg("Error", "Failed to launch.");
									Program.bootstrapForm.Close();
								}
								else
								{
									Program.UpdateBootstrapStatus("Launched!", 100);
									Thread.Sleep(1500);
									Program.bootstrapForm.Close();
								}
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				Program.WriteLog("fatal: " + ex);
				Program.ShowMsg("Error", ex.Message);
				if (Program.bootstrapForm != null && !Program.bootstrapForm.IsDisposed)
				{
					Program.bootstrapForm.Close();
				}
			}
		}

		// Token: 0x0600000F RID: 15 RVA: 0x0000271C File Offset: 0x0000091C
		[MethodImpl(MethodImplOptions.NoInlining)]
		private static bool LaunchClient(string exePath)
		{
			try
			{
				Process process = Process.Start(new ProcessStartInfo(exePath)
				{
					UseShellExecute = true
				});
				if (process != null)
				{
					Thread.Sleep(2000);
					if (!process.HasExited)
					{
						Program.WriteLog("launched pid=" + process.Id);
						return true;
					}
					Program.WriteLog("exited early code=" + process.ExitCode);
				}
			}
			catch (Exception ex)
			{
				Program.WriteLog("launch: " + ex.Message);
			}
			return false;
		}

		// Token: 0x06000010 RID: 16 RVA: 0x000027D4 File Offset: 0x000009D4
		[MethodImpl(MethodImplOptions.NoInlining)]
		private static bool PassesBasicRuntimeProtection()
		{
			bool result;
			try
			{
				string str;
				if (Program.HasKnownReverseEngineeringProcess(out str))
				{
					Program.lastErrorMessage = "Reverse-engineering tool detected (" + str + "). Close it first.";
					Program.WriteLog(Program.lastErrorMessage);
					result = false;
				}
				else if (Debugger.IsAttached)
				{
					Program.lastErrorMessage = "Debugger detected.";
					Program.WriteLog(Program.lastErrorMessage);
					result = false;
				}
				else
				{
					bool flag = false;
					Program.CheckRemoteDebuggerPresent(Process.GetCurrentProcess().Handle, ref flag);
					if (flag)
					{
						Program.lastErrorMessage = "Remote debugger detected.";
						Program.WriteLog(Program.lastErrorMessage);
						result = false;
					}
					else
					{
						string a = Environment.GetEnvironmentVariable("COR_ENABLE_PROFILING") ?? "0";
						if (a == "1")
						{
							Program.lastErrorMessage = "Runtime profiler detected.";
							Program.WriteLog(Program.lastErrorMessage);
							result = false;
						}
						else
						{
							result = true;
						}
					}
				}
			}
			catch (Exception ex)
			{
				Program.WriteLog("protection check: " + ex.Message);
				result = true;
			}
			return result;
		}

		// Token: 0x06000011 RID: 17 RVA: 0x000028FC File Offset: 0x00000AFC
		[MethodImpl(MethodImplOptions.NoInlining)]
		private static bool RunInstallFlow()
		{
			bool result;
			try
			{
				Program.lastErrorMessage = null;
				Program.UpdateBootstrapStatus("Checking version...", 5);
				bool flag2;
				bool flag = Program.EnsureLauncherUpToDate(out flag2);
				if (flag2)
				{
					Program.UpdateBootstrapStatus("Update found. Restarting...", 100);
					result = true;
				}
				else
				{
					if (!flag)
					{
						Program.UpdateBootstrapStatus("Version check unavailable, continuing...", 10);
					}
					Program.UpdateBootstrapStatus("Installing...", 15);
					string value = Program.EnsureClient();
					if (string.IsNullOrEmpty(value))
					{
						if (string.IsNullOrEmpty(Program.lastErrorMessage))
						{
							Program.lastErrorMessage = "Client preparation failed.";
						}
						result = false;
					}
					else
					{
						Program.UpdateBootstrapStatus("Registering protocol...", 85);
						if (!Program.EnsureProtocolHandlerRegistered())
						{
							if (string.IsNullOrEmpty(Program.lastErrorMessage))
							{
								Program.lastErrorMessage = "Protocol registration failed. Try running as administrator.";
							}
							result = false;
						}
						else
						{
							Program.UpdateBootstrapStatus("Good3d installed", 100);
							Program.TryOpenPostInstallPage();
							result = true;
						}
					}
				}
			}
			catch
			{
				if (string.IsNullOrEmpty(Program.lastErrorMessage))
				{
					Program.lastErrorMessage = "Install failed.";
				}
				result = false;
			}
			return result;
		}

		// Token: 0x06000012 RID: 18 RVA: 0x00002A24 File Offset: 0x00000C24
		[MethodImpl(MethodImplOptions.NoInlining)]
		private static bool HasKnownReverseEngineeringProcess(out string processName)
		{
			processName = null;
			string[] array = new string[]
			{
				"dnspy",
				"dnspy-x86",
				"dnspy-x64",
				"ilspy",
				"de4dot",
				"x64dbg",
				"x32dbg",
				"ida",
				"ida64",
				"ghidra",
				"ollydbg",
				"processhacker",
				"processhacker2",
				"pe-bear",
				"detect-it-easy",
				"cheatengine",
				"cheatengine-x86_64",
				"cheatengine-i386",
				"megadumper",
				"justdecompile",
				"dotpeek",
				"reflector",
				"fiddler",
				"fiddlereverywhere",
				"wireshark",
				"charles",
				"protection_id",
				"exeinfope",
				"lordpe",
				"rfxswitch",
				"scylla",
				"scylla_x86",
				"scylla_x64",
				"hollows_hunter"
			};
			try
			{
				Process[] processes = Process.GetProcesses();
				int i = 0;
				while (i < processes.Length)
				{
					Process process = processes[i];
					string a = string.Empty;
					try
					{
						a = (process.ProcessName ?? string.Empty).ToLowerInvariant();
					}
					catch
					{
						goto IL_1CB;
					}
					goto IL_193;
					IL_1CB:
					i++;
					continue;
					IL_193:
					for (int j = 0; j < array.Length; j++)
					{
						if (a == array[j])
						{
							processName = process.ProcessName;
							return true;
						}
					}
					goto IL_1CB;
				}
			}
			catch (Exception ex)
			{
				Program.WriteLog("process scan: " + ex.Message);
			}
			return false;
		}

		// Token: 0x06000013 RID: 19 RVA: 0x00002C58 File Offset: 0x00000E58
		[MethodImpl(MethodImplOptions.NoInlining)]
		private static bool EnsureLauncherUpToDate(out bool relaunchScheduled)
		{
			relaunchScheduled = false;
			bool result;
			try
			{
				string text = Program.FetchManifest();
				if (string.IsNullOrEmpty(text))
				{
					result = true;
				}
				else
				{
					string text2 = Program.ExtractVersionHash(text);
					if (string.IsNullOrEmpty(text2))
					{
						result = true;
					}
					else
					{
						string address = Program.SB.TrimEnd(new char[]
						{
							'/'
						}) + "/version-" + text2 + "-good3dlauncher.exe";
						string location = Assembly.GetExecutingAssembly().Location;
						string folderPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
						string text3 = Path.Combine(folderPath, Program.AD, "Updater");
						Directory.CreateDirectory(text3);
						string text4 = Path.Combine(text3, "Good3dLauncher-" + text2 + ".exe");
						using (WebClient webClient = new WebClient())
						{
							webClient.Headers.Add("User-Agent", "Good3d-Launcher");
							webClient.DownloadFile(address, text4);
						}
						if (!File.Exists(text4))
						{
							result = true;
						}
						else
						{
							string text5 = Program.ComputeFileSha256(location);
							string text6 = Program.ComputeFileSha256(text4);
							if (!text6.StartsWith(text2, StringComparison.OrdinalIgnoreCase))
							{
								Program.lastErrorMessage = "Launcher integrity check failed.";
								Program.WriteLog(string.Concat(new string[]
								{
									Program.lastErrorMessage,
									" hash=",
									text6,
									" expected=",
									text2
								}));
								result = false;
							}
							else if (string.Equals(text5, text6, StringComparison.OrdinalIgnoreCase))
							{
								try
								{
									File.Delete(text4);
								}
								catch
								{
								}
								Program.WriteLog("up-to-date: " + text5);
								result = true;
							}
							else
							{
								Program.WriteLog("update: current=" + text5 + " new=" + text6);
								string text7 = Path.Combine(text3, "update.cmd");
								string contents = string.Concat(new string[]
								{
									"@echo off\r\nsetlocal\r\nping 127.0.0.1 -n 10 >nul\r\ncopy /Y \"",
									text4,
									"\" \"",
									location,
									"\" >nul\r\nstart \"\" \"",
									location,
									"\" /install\r\ndel \"",
									text4,
									"\" >nul 2>&1\r\ndel \"%~f0\"\r\n"
								});
								File.WriteAllText(text7, contents);
								Process process = Process.Start(new ProcessStartInfo("cmd.exe", "/c \"" + text7 + "\"")
								{
									CreateNoWindow = true,
									UseShellExecute = false
								});
								if (process == null)
								{
									result = false;
								}
								else
								{
									relaunchScheduled = true;
									result = true;
								}
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				Program.WriteLog("update check: " + ex.Message);
				Program.lastErrorMessage = "Update check failed. Continuing.";
				result = false;
			}
			return result;
		}

		// Token: 0x06000014 RID: 20 RVA: 0x00002F70 File Offset: 0x00001170
		private static void TryOpenPostInstallPage()
		{
			try
			{
				Process.Start(new ProcessStartInfo(Program.PU)
				{
					UseShellExecute = true
				});
			}
			catch
			{
				try
				{
					Process.Start("explorer.exe", Program.PU);
				}
				catch
				{
				}
			}
		}

		// Token: 0x06000015 RID: 21 RVA: 0x00002FD8 File Offset: 0x000011D8
		private static string ComputeFileSha256(string filePath)
		{
			string result;
			using (SHA256 sha = SHA256.Create())
			{
				using (FileStream fileStream = File.OpenRead(filePath))
				{
					byte[] value = sha.ComputeHash(fileStream);
					result = BitConverter.ToString(value).Replace("-", "").ToLowerInvariant();
				}
			}
			return result;
		}

		// Token: 0x06000016 RID: 22 RVA: 0x0000305C File Offset: 0x0000125C
		[MethodImpl(MethodImplOptions.NoInlining)]
		private static bool EnsureProtocolHandlerRegistered()
		{
			bool result;
			try
			{
				string location = Assembly.GetExecutingAssembly().Location;
				if (!string.IsNullOrEmpty(Program.preferredLauncherCommandPath) && File.Exists(Program.preferredLauncherCommandPath))
				{
					location = Program.preferredLauncherCommandPath;
				}
				string value = "\"" + location + "\" \"%1\"";
				using (RegistryKey registryKey = Registry.CurrentUser.CreateSubKey("Software\\Classes\\" + Program.PN))
				{
					if (registryKey == null)
					{
						return false;
					}
					registryKey.SetValue("", "URL:" + Program.PN + " Protocol");
					registryKey.SetValue("URL Protocol", "");
					using (RegistryKey registryKey2 = registryKey.CreateSubKey("DefaultIcon"))
					{
						if (registryKey2 != null)
						{
							registryKey2.SetValue("", "\"" + location + "\",0");
						}
					}
					using (RegistryKey registryKey3 = registryKey.CreateSubKey("shell\\open\\command"))
					{
						if (registryKey3 == null)
						{
							return false;
						}
						registryKey3.SetValue("", value);
					}
				}
				result = true;
			}
			catch (Exception ex)
			{
				Program.lastErrorMessage = "Protocol registration failed: " + ex.Message;
				Program.WriteLog(Program.lastErrorMessage);
				result = false;
			}
			return result;
		}

		// Token: 0x06000017 RID: 23 RVA: 0x0000324C File Offset: 0x0000144C
		private static void UpdateBootstrapStatus(string statusText, int progressValue)
		{
			if (Program.bootstrapForm != null && !Program.bootstrapForm.IsDisposed)
			{
				Program.bootstrapForm.UpdateStatus(statusText, progressValue);
			}
		}

		// Token: 0x06000018 RID: 24 RVA: 0x00003280 File Offset: 0x00001480
		[MethodImpl(MethodImplOptions.NoInlining)]
		private static Dictionary<string, string> ParseLaunchURI(string uri)
		{
			Dictionary<string, string> result;
			try
			{
				uri = uri.Trim();
				string text = Program.PN + "://";
				if (!uri.StartsWith(text, StringComparison.OrdinalIgnoreCase))
				{
					result = null;
				}
				else
				{
					string text2 = string.Empty;
					Uri uri2;
					if (Uri.TryCreate(uri, UriKind.Absolute, out uri2) && string.Equals(uri2.Scheme, Program.PN, StringComparison.OrdinalIgnoreCase))
					{
						text2 = uri2.Query;
					}
					else
					{
						text2 = uri.Substring(text.Length);
					}
					if (text2.StartsWith("/?"))
					{
						text2 = text2.Substring(2);
					}
					else if (text2.StartsWith("?"))
					{
						text2 = text2.Substring(1);
					}
					Dictionary<string, string> dictionary = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
					foreach (string text3 in text2.Split(new char[]
					{
						'&'
					}))
					{
						string[] array2 = text3.Split(new char[]
						{
							'='
						});
						if (array2.Length == 2)
						{
							string text4 = HttpUtility.UrlDecode(array2[0]);
							string value = HttpUtility.UrlDecode(array2[1]);
							if (!string.IsNullOrEmpty(text4))
							{
								text4 = text4.Trim().TrimStart(new char[]
								{
									'/',
									'?'
								});
							}
							dictionary[text4.ToLower()] = value;
						}
					}
					if (!dictionary.ContainsKey("placeid") || !dictionary.ContainsKey("accountcode"))
					{
						result = null;
					}
					else
					{
						result = dictionary;
					}
				}
			}
			catch
			{
				result = null;
			}
			return result;
		}

		// Token: 0x06000019 RID: 25 RVA: 0x00003460 File Offset: 0x00001660
		[MethodImpl(MethodImplOptions.NoInlining)]
		private static string EnsureClient()
		{
			string result;
			try
			{
				Program.UpdateBootstrapStatus("Fetching manifest...", 10);
				string folderPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
				string path = Path.Combine(folderPath, Program.AD);
				string text = Path.Combine(path, "Versions");
				Directory.CreateDirectory(text);
				string text2 = Program.FetchManifest();
				if (string.IsNullOrEmpty(text2))
				{
					Program.UpdateBootstrapStatus("Manifest fetch failed", 5);
					if (string.IsNullOrEmpty(Program.lastErrorMessage))
					{
						Program.lastErrorMessage = "Could not fetch manifest.";
					}
					result = null;
				}
				else
				{
					string text3 = Program.ExtractVersionHash(text2);
					if (string.IsNullOrEmpty(text3))
					{
						Program.UpdateBootstrapStatus("Invalid manifest", 5);
						Program.lastErrorMessage = "Invalid manifest format.";
						result = null;
					}
					else
					{
						Program.UpdateBootstrapStatus("Checking cache...", 15);
						string text4 = Path.Combine(text, text3);
						string text5 = Path.Combine(text4, "Client2009");
						Program.WriteLog(string.Concat(new object[]
						{
							"hash=",
							text3,
							" clientDir=",
							text5,
							" exists=",
							Directory.Exists(text5)
						}));
						Program.CleanupOldVersionDirectories(text, text4);
						Program.EnsureInstalledLauncherInVersion(text4);
						string text6 = Program.ResolveInstalledClientExecutable(text5);
						Program.WriteLog("cache=" + (text6 ?? "null"));
						if (!string.IsNullOrEmpty(text6) && File.Exists(text6))
						{
							Program.UpdateBootstrapStatus("Registering client...", 20);
							Program.EnsureClientRegistration(text4, text6);
							result = text6;
						}
						else
						{
							Program.UpdateBootstrapStatus("Downloading...", 30);
							Directory.CreateDirectory(text5);
							string text7 = Path.Combine(text4, "client.zip");
							if (!Program.DownloadClientZip(text3, text7))
							{
								Program.UpdateBootstrapStatus("Download failed", 30);
								result = null;
							}
							else
							{
								Program.UpdateBootstrapStatus("Extracting...", 50);
								if (File.Exists(text7))
								{
									try
									{
										Directory.CreateDirectory(text5);
										if (!Program.ExtractZipManaged(text7, text5))
										{
											if (string.IsNullOrEmpty(Program.lastErrorMessage))
											{
												Program.lastErrorMessage = "Extraction failed.";
											}
											return null;
										}
									}
									catch (Exception ex)
									{
										Program.lastErrorMessage = "Extraction: " + ex.Message;
										Program.WriteLog(Program.lastErrorMessage);
										Program.UpdateBootstrapStatus("Extraction failed", 50);
										return null;
									}
									finally
									{
										try
										{
											File.Delete(text7);
										}
										catch
										{
										}
									}
								}
								text6 = Program.ResolveInstalledClientExecutable(text5);
								if (string.IsNullOrEmpty(text6) || !File.Exists(text6))
								{
									Program.UpdateBootstrapStatus("Extraction failed", 50);
									Program.lastErrorMessage = "Client executable not found.";
									result = null;
								}
								else
								{
									Program.UpdateBootstrapStatus("Registering...", 60);
									Program.EnsureClientRegistration(text4, text6);
									Program.UpdateBootstrapStatus("Ready", 80);
									result = text6;
								}
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				Program.lastErrorMessage = "Client prep: " + ex.Message;
				Program.WriteLog(Program.lastErrorMessage);
				Program.UpdateBootstrapStatus("Error", 5);
				result = null;
			}
			return result;
		}

		// Token: 0x0600001A RID: 26 RVA: 0x00003808 File Offset: 0x00001A08
		[MethodImpl(MethodImplOptions.NoInlining)]
		private static string ResolveInstalledClientExecutable(string clientDir)
		{
			try
			{
				Program.WriteLog("resolve: " + clientDir);
				if (!Directory.Exists(clientDir))
				{
					return null;
				}
				string[] array = new string[]
				{
					"Client2009.exe",
					"NostroApp.exe",
					"RobloxPlayerBeta.exe"
				};
				for (int i = 0; i < array.Length; i++)
				{
					string[] files = Directory.GetFiles(clientDir, array[i], SearchOption.AllDirectories);
					if (files.Length > 0)
					{
						Program.WriteLog("found: " + files[0]);
						return files[0];
					}
				}
				string[] files2 = Directory.GetFiles(clientDir, "*.exe", SearchOption.AllDirectories);
				for (int i = 0; i < files2.Length; i++)
				{
					string fileName = Path.GetFileName(files2[i]);
					if (!string.Equals(fileName, Program.LE, StringComparison.OrdinalIgnoreCase))
					{
						Program.WriteLog("fallback: " + files2[i]);
						return files2[i];
					}
				}
			}
			catch (Exception ex)
			{
				Program.WriteLog("resolve warning: " + ex.Message);
			}
			return null;
		}

		// Token: 0x0600001B RID: 27 RVA: 0x00003944 File Offset: 0x00001B44
		[MethodImpl(MethodImplOptions.NoInlining)]
		private static void EnsureInstalledLauncherInVersion(string versionDir)
		{
			try
			{
				Directory.CreateDirectory(versionDir);
				string location = Assembly.GetExecutingAssembly().Location;
				string text = Path.Combine(versionDir, Program.LE);
				Program.preferredLauncherCommandPath = text;
				if (!string.Equals(Path.GetFullPath(location), Path.GetFullPath(text), StringComparison.OrdinalIgnoreCase))
				{
					bool flag = !File.Exists(text);
					if (!flag)
					{
						string a = Program.ComputeFileSha256(location);
						string b = Program.ComputeFileSha256(text);
						flag = !string.Equals(a, b, StringComparison.OrdinalIgnoreCase);
					}
					if (flag)
					{
						Program.CloseOldBootstrapperProcesses(text);
						File.Copy(location, text, true);
						Program.WriteLog("staged launcher: " + text);
					}
					string directoryName = Path.GetDirectoryName(location);
					string text2 = Path.Combine(directoryName, "logo.ico");
					string text3 = Path.Combine(versionDir, "logo.ico");
					if (File.Exists(text2) && !File.Exists(text3))
					{
						File.Copy(text2, text3, true);
					}
				}
			}
			catch (Exception ex)
			{
				Program.WriteLog("stage launcher: " + ex.Message);
			}
		}

		// Token: 0x0600001C RID: 28 RVA: 0x00003A70 File Offset: 0x00001C70
		private static void CloseOldBootstrapperProcesses(string keepPath)
		{
			try
			{
				string location = Assembly.GetExecutingAssembly().Location;
				string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(location);
				Process[] processesByName = Process.GetProcessesByName(fileNameWithoutExtension);
				int i = 0;
				while (i < processesByName.Length)
				{
					Process process = processesByName[i];
					try
					{
						if (process.Id != Process.GetCurrentProcess().Id)
						{
							string fileName = process.MainModule.FileName;
							if (string.Equals(fileName, keepPath, StringComparison.OrdinalIgnoreCase) || fileName.IndexOf(Path.Combine(Program.AD, "Versions"), StringComparison.OrdinalIgnoreCase) >= 0)
							{
								Program.WriteLog("stopping pid=" + process.Id);
								process.Kill();
								process.WaitForExit(3000);
							}
						}
					}
					catch (Exception ex)
					{
						Program.WriteLog("close old: " + ex.Message);
					}
					IL_DC:
					i++;
					continue;
					goto IL_DC;
				}
			}
			catch (Exception ex)
			{
				Program.WriteLog("close old: " + ex.Message);
			}
		}

		// Token: 0x0600001D RID: 29 RVA: 0x00003BAC File Offset: 0x00001DAC
		[MethodImpl(MethodImplOptions.NoInlining)]
		private static void CleanupOldVersionDirectories(string versionsDir, string currentVersionDir)
		{
			try
			{
				if (Directory.Exists(versionsDir))
				{
					string location = Assembly.GetExecutingAssembly().Location;
					string directoryName = Path.GetDirectoryName(location);
					foreach (string text in Directory.GetDirectories(versionsDir))
					{
						if (!string.Equals(Path.GetFullPath(text), Path.GetFullPath(currentVersionDir), StringComparison.OrdinalIgnoreCase))
						{
							if (string.IsNullOrEmpty(directoryName) || !directoryName.StartsWith(text, StringComparison.OrdinalIgnoreCase))
							{
								try
								{
									Program.DeleteDirectoryContentsBestEffort(text);
									Directory.Delete(text, true);
									Program.WriteLog("deleted old version: " + text);
								}
								catch (Exception ex)
								{
									Program.WriteLog("cleanup skip " + text + ": " + ex.Message);
								}
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				Program.WriteLog("cleanup: " + ex.Message);
			}
		}

		// Token: 0x0600001E RID: 30 RVA: 0x00003CD0 File Offset: 0x00001ED0
		private static void DeleteDirectoryContentsBestEffort(string dir)
		{
			try
			{
				string[] files = Directory.GetFiles(dir, "*", SearchOption.AllDirectories);
				for (int i = 0; i < files.Length; i++)
				{
					try
					{
						File.SetAttributes(files[i], FileAttributes.Normal);
						File.Delete(files[i]);
					}
					catch
					{
					}
				}
			}
			catch
			{
			}
		}

		// Token: 0x0600001F RID: 31 RVA: 0x00003D48 File Offset: 0x00001F48
		[MethodImpl(MethodImplOptions.NoInlining)]
		private static bool ExtractZipManaged(string zipPath, string destinationDirectory)
		{
			bool result;
			try
			{
				using (FileStream fileStream = new FileStream(zipPath, FileMode.Open, FileAccess.Read, FileShare.Read))
				{
					using (BinaryReader binaryReader = new BinaryReader(fileStream))
					{
						long num = Program.FindEndOfCentralDirectory(fileStream);
						if (num < 0L)
						{
							Program.lastErrorMessage = "ZIP EOCD not found.";
							return false;
						}
						fileStream.Position = num;
						uint num2 = binaryReader.ReadUInt32();
						if (num2 != 101010256U)
						{
							Program.lastErrorMessage = "Invalid ZIP EOCD.";
							return false;
						}
						binaryReader.ReadUInt16();
						binaryReader.ReadUInt16();
						binaryReader.ReadUInt16();
						ushort num3 = binaryReader.ReadUInt16();
						binaryReader.ReadUInt32();
						uint num4 = binaryReader.ReadUInt32();
						ushort num5 = binaryReader.ReadUInt16();
						if (num5 > 0)
						{
							binaryReader.ReadBytes((int)num5);
						}
						fileStream.Position = (long)((ulong)num4);
						for (int i = 0; i < (int)num3; i++)
						{
							uint num6 = binaryReader.ReadUInt32();
							if (num6 != 33639248U)
							{
								Program.lastErrorMessage = "Invalid central dir.";
								return false;
							}
							binaryReader.ReadUInt16();
							binaryReader.ReadUInt16();
							ushort num7 = binaryReader.ReadUInt16();
							ushort num8 = binaryReader.ReadUInt16();
							binaryReader.ReadUInt16();
							binaryReader.ReadUInt16();
							binaryReader.ReadUInt32();
							uint count = binaryReader.ReadUInt32();
							uint num9 = binaryReader.ReadUInt32();
							ushort count2 = binaryReader.ReadUInt16();
							ushort num10 = binaryReader.ReadUInt16();
							ushort num11 = binaryReader.ReadUInt16();
							binaryReader.ReadUInt16();
							binaryReader.ReadUInt16();
							binaryReader.ReadUInt32();
							uint num12 = binaryReader.ReadUInt32();
							string @string = Encoding.UTF8.GetString(binaryReader.ReadBytes((int)count2));
							if (num10 > 0)
							{
								binaryReader.ReadBytes((int)num10);
							}
							if (num11 > 0)
							{
								binaryReader.ReadBytes((int)num11);
							}
							if (!string.IsNullOrEmpty(@string))
							{
								string text = @string.Replace('/', Path.DirectorySeparatorChar).Replace('\\', Path.DirectorySeparatorChar);
								if (text.Contains(".."))
								{
									Program.lastErrorMessage = "ZIP path traversal.";
									return false;
								}
								string path = Path.Combine(destinationDirectory, text);
								bool flag = text.EndsWith("/") || text.EndsWith("\\");
								if (flag)
								{
									Directory.CreateDirectory(path);
								}
								else
								{
									string directoryName = Path.GetDirectoryName(path);
									if (!string.IsNullOrEmpty(directoryName))
									{
										Directory.CreateDirectory(directoryName);
									}
									long position = fileStream.Position;
									fileStream.Position = (long)((ulong)num12);
									uint num13 = binaryReader.ReadUInt32();
									if (num13 != 67324752U)
									{
										Program.lastErrorMessage = "Invalid local header.";
										return false;
									}
									binaryReader.ReadUInt16();
									ushort num14 = binaryReader.ReadUInt16();
									ushort num15 = binaryReader.ReadUInt16();
									binaryReader.ReadUInt16();
									binaryReader.ReadUInt16();
									binaryReader.ReadUInt32();
									binaryReader.ReadUInt32();
									binaryReader.ReadUInt32();
									ushort num16 = binaryReader.ReadUInt16();
									ushort num17 = binaryReader.ReadUInt16();
									if (num16 > 0)
									{
										binaryReader.ReadBytes((int)num16);
									}
									if (num17 > 0)
									{
										binaryReader.ReadBytes((int)num17);
									}
									if ((num14 & 1) != 0 || (num7 & 1) != 0)
									{
										Program.lastErrorMessage = "Encrypted ZIP not supported.";
										return false;
									}
									byte[] array = binaryReader.ReadBytes((int)count);
									using (FileStream fileStream2 = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None))
									{
										if (num15 == 0)
										{
											fileStream2.Write(array, 0, array.Length);
										}
										else
										{
											if (num15 != 8)
											{
												Program.lastErrorMessage = "Unsupported compression: " + num15;
												return false;
											}
											using (MemoryStream memoryStream = new MemoryStream(array))
											{
												using (DeflateStream deflateStream = new DeflateStream(memoryStream, CompressionMode.Decompress))
												{
													deflateStream.CopyTo(fileStream2);
												}
											}
										}
									}
									fileStream.Position = position;
								}
							}
						}
					}
				}
				Program.WriteLog("extracted: " + zipPath);
				result = true;
			}
			catch (Exception ex)
			{
				Program.lastErrorMessage = "ZIP extract: " + ex.Message;
				Program.WriteLog(Program.lastErrorMessage);
				result = false;
			}
			return result;
		}

		// Token: 0x06000020 RID: 32 RVA: 0x0000428C File Offset: 0x0000248C
		private static long FindEndOfCentralDirectory(FileStream fs)
		{
			long num = Math.Min(fs.Length, 65557L);
			byte[] array = new byte[num];
			fs.Position = fs.Length - num;
			fs.Read(array, 0, (int)num);
			for (int i = array.Length - 22; i >= 0; i--)
			{
				if (array[i] == 80 && array[i + 1] == 75 && array[i + 2] == 5 && array[i + 3] == 6)
				{
					return fs.Length - num + (long)i;
				}
			}
			return -1L;
		}

		// Token: 0x06000021 RID: 33 RVA: 0x0000432C File Offset: 0x0000252C
		[MethodImpl(MethodImplOptions.NoInlining)]
		private static bool DownloadClientZip(string versionHash, string zipPath)
		{
			string[] array = Program.ResolveClientZipURLs(versionHash);
			Exception ex = null;
			foreach (string text in array)
			{
				try
				{
					using (WebClient webClient = new WebClient())
					{
						webClient.Headers.Add("User-Agent", "Good3d-Launcher");
						webClient.Headers.Add("Accept", "application/octet-stream,*/*");
						webClient.DownloadFile(text, zipPath);
					}
					if (File.Exists(zipPath) && new FileInfo(zipPath).Length > 0L)
					{
						Program.WriteLog("dl from " + text);
						return true;
					}
				}
				catch (Exception ex2)
				{
					ex = ex2;
					Program.WriteLog("dl fail " + text + ": " + ex2.Message);
					try
					{
						if (File.Exists(zipPath))
						{
							File.Delete(zipPath);
						}
					}
					catch
					{
					}
				}
			}
			Program.lastErrorMessage = "Download failed." + ((ex != null) ? (" " + ex.Message) : "");
			return false;
		}

		// Token: 0x06000022 RID: 34 RVA: 0x000044A4 File Offset: 0x000026A4
		[MethodImpl(MethodImplOptions.NoInlining)]
		private static void EnsureClientRegistration(string versionDir, string clientExe)
		{
			try
			{
				string path = Path.Combine(versionDir, ".reg");
				if (File.Exists(path))
				{
					string a = string.Empty;
					try
					{
						a = File.ReadAllText(path).Trim();
					}
					catch
					{
					}
					if (a == "ok")
					{
						Program.UpdateBootstrapStatus("Client registered", 70);
						return;
					}
				}
				Program.UpdateBootstrapStatus("Registering client...", 65);
				try
				{
					using (Process process = Process.Start(new ProcessStartInfo(clientExe, "/regserver")
					{
						UseShellExecute = false,
						CreateNoWindow = true
					}))
					{
						process.WaitForExit(10000);
					}
					Program.WriteLog("/regserver done");
				}
				catch (Exception ex)
				{
					Program.WriteLog("/regserver: " + ex.Message);
				}
				try
				{
					string str = "$e=\"" + clientExe + "\"; $p=Start-Process -FilePath $e -ArgumentList '/regserver' -Verb RunAs -Wait -PassThru; exit $p.ExitCode";
					using (Process process = Process.Start(new ProcessStartInfo("powershell", "-NoProfile -Command \"" + str + "\"")
					{
						UseShellExecute = false,
						CreateNoWindow = true
					}))
					{
						process.WaitForExit(15000);
					}
					Program.WriteLog("/regserver elevated done");
				}
				catch (Exception ex)
				{
					Program.WriteLog("/regserver elevated: " + ex.Message);
				}
				File.WriteAllText(path, "ok\n");
				Program.UpdateBootstrapStatus("Client registered", 70);
			}
			catch (Exception ex)
			{
				Program.WriteLog("registration: " + ex.Message);
				Program.UpdateBootstrapStatus("Registration warning", 65);
			}
		}

		// Token: 0x06000023 RID: 35 RVA: 0x00004708 File Offset: 0x00002908
		[MethodImpl(MethodImplOptions.NoInlining)]
		private static string FetchManifest()
		{
			string result;
			try
			{
				using (WebClient webClient = new WebClient())
				{
					webClient.Headers.Add("User-Agent", "Good3d-Launcher");
					result = webClient.DownloadString(Program.SB.TrimEnd(new char[]
					{
						'/'
					}) + "/version");
				}
			}
			catch (Exception ex)
			{
				Program.WriteLog("manifest primary: " + ex.Message);
				try
				{
					using (WebClient webClient = new WebClient())
					{
						webClient.Headers.Add("User-Agent", "Good3d-Launcher");
						return webClient.DownloadString(Program.CB.TrimEnd(new char[]
						{
							'/'
						}) + "/version");
					}
				}
				catch (Exception ex2)
				{
					Program.lastErrorMessage = "Manifest fetch failed. " + ex.Message + " | CDN: " + ex2.Message;
					Program.WriteLog(Program.lastErrorMessage);
				}
				result = null;
			}
			return result;
		}

		// Token: 0x06000024 RID: 36 RVA: 0x00004860 File Offset: 0x00002A60
		[MethodImpl(MethodImplOptions.NoInlining)]
		private static string ExtractVersionHash(string manifest)
		{
			string result;
			try
			{
				if (manifest.Contains("version_hash"))
				{
					Match match = Regex.Match(manifest, "\"version_hash\"\\s*:\\s*\"([^\"]+)\"");
					if (match.Success)
					{
						return Program.NormalizeVersionHash(match.Groups[1].Value);
					}
				}
				string[] array = manifest.Trim().Split(new char[]
				{
					' ',
					'\t',
					'\n'
				}, StringSplitOptions.RemoveEmptyEntries);
				if (array.Length > 1)
				{
					result = Program.NormalizeVersionHash(array[1]);
				}
				else
				{
					result = ((array.Length > 0) ? Program.NormalizeVersionHash(array[0]) : null);
				}
			}
			catch
			{
				result = null;
			}
			return result;
		}

		// Token: 0x06000025 RID: 37 RVA: 0x00004918 File Offset: 0x00002B18
		private static string NormalizeVersionHash(string raw)
		{
			string result;
			if (string.IsNullOrEmpty(raw))
			{
				result = null;
			}
			else
			{
				string text = raw.Trim();
				Match match = Regex.Match(text, "version-([a-fA-F0-9]{8,64})-good3dlauncher\\.exe", RegexOptions.IgnoreCase);
				if (match.Success)
				{
					result = match.Groups[1].Value.ToLowerInvariant();
				}
				else
				{
					Match match2 = Regex.Match(text, "^version-([a-fA-F0-9]{8,64})$", RegexOptions.IgnoreCase);
					if (match2.Success)
					{
						result = match2.Groups[1].Value.ToLowerInvariant();
					}
					else
					{
						Match match3 = Regex.Match(text, "([a-fA-F0-9]{8,64})");
						if (match3.Success)
						{
							result = match3.Groups[1].Value.ToLowerInvariant();
						}
						else
						{
							result = text;
						}
					}
				}
			}
			return result;
		}

		// Token: 0x06000026 RID: 38 RVA: 0x000049EC File Offset: 0x00002BEC
		private static string[] ResolveClientZipURLs(string versionHash)
		{
			return new string[]
			{
				Program.SB.TrimEnd(new char[]
				{
					'/'
				}) + "/versions/" + versionHash + "/Client2009.zip",
				Program.CB.TrimEnd(new char[]
				{
					'/'
				}) + "/versions/" + versionHash + "/Client2009.zip"
			};
		}

		// Token: 0x06000027 RID: 39 RVA: 0x00004A5C File Offset: 0x00002C5C
		private static void WriteLog(string message)
		{
			try
			{
				if (Program.logTargets.Count == 0)
				{
					string folderPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
					string text = Path.Combine(folderPath, Program.AD, "logs");
					Directory.CreateDirectory(text);
					Program.logTargets.Add(Path.Combine(text, "launcher-fallback.log"));
				}
				string contents = DateTime.UtcNow.ToString("o") + " " + message + Environment.NewLine;
				for (int i = 0; i < Program.logTargets.Count; i++)
				{
					try
					{
						File.AppendAllText(Program.logTargets[i], contents);
					}
					catch
					{
					}
				}
			}
			catch
			{
			}
		}

		// Token: 0x06000028 RID: 40 RVA: 0x00004B3C File Offset: 0x00002D3C
		private static void InitializeLogging(string[] args)
		{
			try
			{
				Program.logTargets.Clear();
				Program.launchLogFileName = string.Concat(new object[]
				{
					"launcher-",
					DateTime.UtcNow.ToString("yyyyMMdd-HHmmss"),
					"-",
					Process.GetCurrentProcess().Id,
					".log"
				});
				string folderPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
				string[] array = new string[]
				{
					Path.Combine(folderPath, Program.AD, "logs"),
					Path.Combine(folderPath, Program.AA, "logs")
				};
				for (int i = 0; i < array.Length; i++)
				{
					try
					{
						Directory.CreateDirectory(array[i]);
						Program.logTargets.Add(Path.Combine(array[i], Program.launchLogFileName));
					}
					catch
					{
					}
				}
				Program.WriteLog("=== Launch ===");
				Program.WriteLog("v=" + Assembly.GetExecutingAssembly().GetName().Version);
				Program.WriteLog("exe=" + Process.GetCurrentProcess().MainModule.FileName);
				Program.WriteLog("args=" + ((args != null && args.Length > 0) ? string.Join(" ", args) : "<none>"));
			}
			catch
			{
			}
		}

		// Token: 0x06000029 RID: 41 RVA: 0x00004CD8 File Offset: 0x00002ED8
		private static void ShowMsg(string title, string message)
		{
			try
			{
				Process process = new Process();
				process.StartInfo.FileName = "powershell";
				process.StartInfo.Arguments = string.Concat(new string[]
				{
					"-NoProfile -Command \"[System.Windows.Forms.MessageBox]::Show('",
					message.Replace("'", "''"),
					"', '",
					title.Replace("'", "''"),
					"')\""
				});
				process.StartInfo.UseShellExecute = false;
				process.StartInfo.CreateNoWindow = true;
				process.Start();
				process.WaitForExit(5000);
			}
			catch
			{
				Console.WriteLine(message);
			}
		}

		// Token: 0x04000004 RID: 4
		private static readonly byte[] _sb = new byte[]
		{
			50,
			47,
			44,
			41,
			45,
			101,
			115,
			114,
			33,
			54,
			36,
			36,
			38,
			121,
			51,
			58,
			37,
			47,
			123,
			45,
			96,
			55,
			53,
			55
		};

		// Token: 0x04000005 RID: 5
		private static readonly byte[] _gb = new byte[]
		{
			50,
			47,
			44,
			41,
			100,
			112,
			115,
			58,
			61,
			60,
			52,
			98,
			50,
			121,
			44,
			44,
			48
		};

		// Token: 0x04000006 RID: 6
		private static readonly byte[] _cb = new byte[]
		{
			50,
			47,
			44,
			41,
			45,
			101,
			115,
			114,
			56,
			50,
			41,
			51,
			58,
			47,
			122,
			57,
			37,
			39,
			103,
			46,
			33,
			32,
			40,
			126,
			38
		};

		// Token: 0x04000007 RID: 7
		private static readonly byte[] _pu = new byte[]
		{
			50,
			47,
			44,
			41,
			45,
			101,
			115,
			114,
			53,
			60,
			63,
			53,
			101,
			51,
			122,
			45,
			51,
			49,
			103,
			14,
			47,
			34,
			41,
			62,
			108,
			34,
			51,
			49,
			62
		};

		// Token: 0x04000008 RID: 8
		private static readonly byte[] _ad = new byte[]
		{
			29,
			52,
			55,
			61,
			109,
			59
		};

		// Token: 0x04000009 RID: 9
		private static readonly byte[] _aa = new byte[]
		{
			29,
			52,
			55,
			61,
			60,
			51,
			51,
			37
		};

		// Token: 0x0400000A RID: 10
		private static readonly byte[] _le = new byte[]
		{
			29,
			52,
			55,
			61,
			109,
			59,
			16,
			60,
			39,
			61,
			51,
			57,
			51,
			37,
			122,
			48,
			50,
			46
		};

		// Token: 0x0400000B RID: 11
		private static readonly byte[] _pr = new byte[]
		{
			61,
			52,
			55,
			61,
			60,
			51,
			51,
			37
		};

		// Token: 0x0400000C RID: 12
		private static BootstrapForm bootstrapForm;

		// Token: 0x0400000D RID: 13
		private static string lastErrorMessage;

		// Token: 0x0400000E RID: 14
		private static readonly List<string> logTargets = new List<string>();

		// Token: 0x0400000F RID: 15
		private static string launchLogFileName;

		// Token: 0x04000010 RID: 16
		private static string preferredLauncherCommandPath;
	}
}
