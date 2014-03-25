using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Data;

using RyuaNerin;
using System.Text.RegularExpressions;

namespace AheuiCSharp
{
	class Program
	{
		static void PrintHelp()
		{
			Console.WriteLine("Usage: {0} <filename> [options]", Regex.Match("/([^\"]+)\"", Environment.CommandLine.Replace('\\', '/')).Groups[1].Value);
			Console.WriteLine("--help,     -h  : Prints help message");
			Console.WriteLine("--version,  -v  : Prints Version Information");
			Console.WriteLine("--stackmin, -sn : Set default stack size ( default : 64 )");
			Console.WriteLine("--stackmax, -sx : Set maximum stack size ( default : 4096 )");
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

			RyuaNerin.Aheui au = new RyuaNerin.Aheui();
			au.init(File.ReadAllText(parser.GetCommand(0), encoding));

			int i;

			if (!int.TryParse(parser.GetOptionValue("--stackmin"), out i))
			{
				PrintVersion();
				return 1;
			}
			au.StackMinSize = (i / 4);

			if (!int.TryParse(parser.GetOptionValue("--stackmax"), out i))
			{
				PrintVersion();
				return 1;
			}
			au.StackMaxSize = (i / 4);

			while (au.isRunning)
			{
				if (au.state == RyuaNerin.Aheui.State.WAITING_CHAR)
				{
					Console.WriteLine(au.GetResult());
					Console.Write("Input Char : ");
					au.SetInput(Console.ReadLine());
					Console.WriteLine("");
				}
				else if (au.state == RyuaNerin.Aheui.State.WAITING_NUMBER)
				{
					Console.Write("Input Number : ");
					Console.Write(au.GetResult());
					au.SetInput(Console.ReadLine());
				}

				au.Step();
			}

			Console.WriteLine(au.GetResult());

			return 0;
		}
	}
}
