using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Data;
using System.Text.RegularExpressions;
using System.Reflection;

using RyuaNerin;

namespace AheuiCSharp
{
	class Program
	{
		static void PrintHelp()
		{
			Version ver = Assembly.GetExecutingAssembly().GetName().Version;
			Console.WriteLine("Aheui C# v {0}.{1}.{2}", ver.Major, ver.Minor, ver.Build);
			Console.WriteLine();
			Console.WriteLine("Usage: {0} <filename> [options]", Regex.Match("/([^\"]+)\"", Environment.CommandLine.Replace('\\', '/')).Groups[1].Value);
			Console.WriteLine("--help,     -h  : Prints help message");
			Console.WriteLine("--version,  -v  : Prints Version Information");
			Console.WriteLine("--stackmin, -sn : Set default stack size ( default : 64 )");
			Console.WriteLine("--stackmax, -sx : Set maximum stack size ( default : 4096 )");
			Console.WriteLine("--debug,    -d  : DebugMode");
			Console.WriteLine("--encoding, -e  : Set the encoding of source code (default :  utf-8 )");
			Console.WriteLine("    ascii, utf-7, utf-8, utf-16, utf-32, unicode");
			Console.WriteLine();
			Console.WriteLine("Made by RyuaNerin, 2014");
			Console.WriteLine();
		}

		static void PrintVersion()
		{
			Console.WriteLine("Aheui C# v 1.0.0");
			Console.WriteLine("Made by RyuaNerin");
		}

		static int Main(string[] args)
		{
			if (args.Length == 0)
			{
				PrintHelp();
				return 0;
			}

			CommandLineParser parser = new CommandLineParser();
			parser.AddOptions("--help", "-h");
			parser.AddOptions("--version", "-v");
			parser.AddOptions("--debug", "-d");
			parser.AddOptionsDefault("utf-8", "--encoding", "-e");
			parser.AddOptionsDefault("64", "--stackmin", "-sn");
			parser.AddOptionsDefault("4096", "--stackmax", "-sx");

			if (!parser.Parse(args))
			{
				PrintHelp();
				return 0;
			}

			if (parser.GetOption("--help"))
			{
				PrintHelp();
				return 0;
			}

			if (parser.GetOption("--version"))
			{
				PrintVersion();
				return 0;
			}

			Encoding encoding;

			switch (parser.GetOptionValue("--encoding").ToLower())
			{
				case "ascii":
					encoding = Encoding.ASCII;
					break;
				case "utf-7":
					encoding = Encoding.UTF7;
					break;
				case "utf-8":
					encoding = Encoding.UTF8;
					break;
				case "utf-16":
					encoding = Encoding.BigEndianUnicode;
					break;
				case "utf-32":
					encoding = Encoding.UTF32;
					break;
				case "unicode":
					encoding = Encoding.Unicode;
					break;
				default:
					PrintVersion();
					return 1;
			}

			int min;
			if (!int.TryParse(parser.GetOptionValue("--stackmin"), out min) || (min < 0))
			{
				PrintVersion();
				return 1;
			}

			int max;
			if (!int.TryParse(parser.GetOptionValue("--stackmax"), out max) || (max < 0) || (max < min))
			{
				PrintVersion();
				return 1;
			}

			RyuaNerin.Aheui au = new RyuaNerin.Aheui(min, max);
			au.init(File.ReadAllText(parser.GetCommand(0), encoding));

			au.StackMinSize = min;
			au.StackMaxSize = max;

			bool debugMode = parser.GetOption("--debug");

			while (au.isRunning)
			{
				if (au.state == RyuaNerin.Aheui.State.WAITING_CHAR)
				{
					Console.WriteLine();
					Console.WriteLine(au.GetResult());
					Console.Write("Input Char : ");
					au.SetInput(Console.ReadLine());
					Console.WriteLine("");
				}
				else if (au.state == RyuaNerin.Aheui.State.WAITING_NUMBER)
				{
					Console.WriteLine();
					Console.Write("Input Number : ");
					Console.Write(au.GetResult());
					au.SetInput(Console.ReadLine());
				}

				au.Step();

				if (debugMode)
				{
					Console.Clear();
					Console.WriteLine(au.ToString());
					Console.WriteLine();
					Console.WriteLine(au.GetTotalResult());
					Console.ReadKey();
				}
			}

			if (!debugMode)
				Console.WriteLine(au.GetResult());

			return 0;
		}
	}
}
