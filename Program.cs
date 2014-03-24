using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using RyuaNerin;
using System.Data;

namespace twitcalc
{
	class Program
	{
		static void Main(string[] args)
		{
			RyuaNerin.Aheui au = new RyuaNerin.Aheui();

			au.init("밤밣따빠밣밟따빠맣받밪밬따딴박다닥빠맣밠당빠빱맣맣받닫빠맣파빨받밤따다맣맣빠빠밣다맣맣빠밬다맣밬탕빠맣밣타맣발다밤타맣박발땋맣희");

			while (au.isRunning)
			{
				if (au.state == Aheui.State.WAITING_CHAR)
				{
					Console.WriteLine(au.GetResult());
					Console.Write("Input Char : ");
					au.SetInput(Console.ReadLine());
					Console.WriteLine("");
				}
				else if (au.state == Aheui.State.WAITING_NUMBER)
				{
					Console.Write("Input Number : ");
					Console.Write(au.GetResult());
					au.SetInput(Console.ReadLine());
				}

				au.Step();
			}

			Console.WriteLine(au.GetResult());
			Console.ReadKey();
		}
	}
}
