using System;
using System.Linq;
using System.Text;
using System.Collections;
using System.IO;

namespace RyuaNerin
{
	public class Aheui
	{
		private static readonly int[] strokeCounts = { 0, 2, 4, 4, 2, 5, 5, 3, 5, 7, 9, 9, 7, 9, 9, 8, 4, 4, 6, 2, 4, 1, 3, 4, 3, 4, 4, 3 };

		internal class Storage
		{
			private ExtendedArray[] Storages;
			private int id;

			public Storage()
			{
				this.id = 0;
				Storages = new ExtendedArray[28];
				for (int i = 0; i < 28; i++)
					this.Storages[i] = new ExtendedArray();
			}

			public void Clear()
			{
				this.id = 0;
				for (int i = 0; i < 28; i++)
					this.Storages[i].Clear();
			}

			public void Select(int id)
			{
				this.id = id;
			}

			public void Move(int id)
			{
				int id_ = this.id;
				int date = this.Pop();
				Select(id);
				Push(date);
				Select(id_);
			}

			public int Pop()
			{
				if (this.id == 21)
					return this.Storages[this.id].Shift();
				else
					return this.Storages[this.id].Pop();
			}

			public void Push(int value)
			{
				this.Storages[this.id].Push(value);
			}

			public void Duplicate()
			{
				ExtendedArray stc = this.Storages[this.id];
				if ((id == 21) || (id == 27))
					stc.Unshift(stc[0]);
				else
					stc.Push(stc[stc.Length - 1]);
			}

			public void Swap()
			{
				if ((id == 21) || (id == 27))
				{
					ExtendedArray stc = this.Storages[this.id];
					stc[0] ^= stc[1];
					stc[1] ^= stc[0];
					stc[0] ^= stc[1];
				}
				else
				{
					int  a, b;
					a = Pop();
					b = Pop();
					Push(a);
					Push(b);
				}
			}
		}
		internal class ExtendedArray
		{
			private const int MinCapacity = 16;
			private const int buffer = 256;

			private int[] stack;
			private int length = 0;
			private int capacity;

			public ExtendedArray() : this(ExtendedArray.MinCapacity) { }
			public ExtendedArray(int capacity)
			{
				this.capacity = capacity;
				this.length = 0;
				this.stack = new int[capacity];
			}

			public void Clear()
			{
				this.length = 0;
			}

			private void ReduceSize()
			{
				if (this.length < this.capacity / 2)
				{
					int newCapacity = 0;

					if (this.capacity > ExtendedArray.buffer)
						newCapacity = this.capacity - ExtendedArray.buffer;
					else
						newCapacity = this.capacity / 2;

					if (newCapacity < ExtendedArray.MinCapacity)
						return;

					int[] stackTmp = new int[newCapacity];
					Array.Copy(this.stack, 0, stackTmp, 0, newCapacity);
					this.stack = stackTmp;

					this.capacity = newCapacity;
				}

			}

			private void ExpandSize()
			{
				if (this.length + 2 >= this.capacity)
				{
					int newCapacity = 0;

					if (this.capacity > ExtendedArray.buffer)
						newCapacity = this.capacity + ExtendedArray.buffer;
					else
						newCapacity = this.capacity * 2;

					int[] stackTmp = new int[newCapacity];
					Array.Copy(this.stack, 0, stackTmp, 0, this.capacity);
					this.stack = stackTmp;

					this.capacity = newCapacity;
				}
			}

			public void Push(int value)
			{
				this.ExpandSize();

				this.stack[this.length++] = value;
			}

			public void Unshift(int value)
			{
				this.ExpandSize();

				for (int i = 0; i < this.length - 1; i++)
					this.stack[i + 1] = this.stack[i];

				this.stack[0] = value;
			}

			public int Pop()
			{
				int i = this.stack[--this.length];

				ReduceSize();

				return i;
			}

			public int Shift()
			{
				int r = this.stack[0];

				for (int i = 1; i < this.length; i++)
					this.stack[i - 1] = this.stack[i];

				this.length--;

				ReduceSize();

				return r;
			}

			public int Length
			{
				get
				{
					return this.length;
				}
			}

			public int this[int index]
			{
				get
				{
					return this.stack[index];
				}
				set
				{
					this.stack[index] = value;
				}
			}
		}
		internal struct Token
		{
			public static readonly string INITIALS = "ㄱㄲㄴㄷㄸㄹㅁㅂㅃㅅㅆㅇㅈㅉㅊㅋㅌㅍㅎ";
			public static readonly string VOWELS = "ㅏㅐㅑㅒㅓㅔㅕㅖㅗㅘㅙㅚㅛㅜㅝㅞㅟㅠㅡㅢㅣ";
			public static readonly string UNDERS =  "　ㄱㄲㄳㄴㄵㄶㄷㄹㄺㄻㄼㄽㄾㄿㅀㅁㅂㅄㅅㅆㅇㅈㅊㅋㅌㅍㅎ";

			public char code;
			public bool isComment;
			public int initial;
			public int vowel;
			public int under;

			public Token(char code)
			{
				this.code = code;
				this.isComment = false;

				int charCode = Convert.ToInt32(code) - 44032;
				if ((charCode > 11171) || (charCode < 0))
				{
					this.initial = 0;
					this.vowel = 0;
					this.under = 0;
					this.isComment = true;
				}
				else
				{
					this.initial = charCode / 588;
					this.vowel = (charCode % 588) / 28;
					this.under = (charCode % 588) % 28;
					this.isComment = false;
				}

			}

			public override string ToString()
			{
				if (this.isComment) return "[ # ]";

				return String.Format("[{0} = {0}, {1}, {2}]", this.code, Token.INITIALS[initial], Token.VOWELS[vowel], Token.UNDERS[under]);
			}
		}

		private struct Cursor
		{
			public Cursor(int x, int y)
			{
				this.x = x;
				this.y = y;
			}
			public override string ToString()
			{
				return String.Format("[{0}, {1}]", this.x, this.y);
			}
			public int x;
			public int y;
		}

		public enum State : int
		{
			STOPPED = 0,
			RUNNING = 1,
			WAITING_NUMBER = 2,
			WAITING_CHAR = 3
		}

		private Token[][] codeSpace;
		private string input;
		private StringBuilder output;
		public State state { get; internal set; }

		private Storage storage;
		private bool direction;
		private int speed;
		private Cursor cursor;

		public Aheui()
		{
			this.codeSpace = null;
			this.storage = new Storage();
			this.cursor = new Cursor(0, 0);
			this.init();
		}

		public void init()
		{
			this.input = null;
			this.output = new StringBuilder();
			this.outputLength = 0;
			this.state = State.RUNNING;
			this.storage.Clear();
			this.direction = true;
			this.speed = 1;
			this.cursor.x = this.cursor.y = 0;
		}
		public void init(string input)
		{
			this.init();

			//////////////////////////////////////////////////////////////////////////

			if ((input != null) && (input.Length == 0))
				this.codeSpace = null;

			string[] codeLines = input.Replace("\r\n", "\n").Replace("\r", "\n").Split('\n');
			
			this.codeSpace = new Token[codeLines.Length][];
			
			for (int i = 0; i < codeLines.Length; i++)
			{
				this.codeSpace[i] = new Token[codeLines[i].Length];

				for (int j = 0; j < codeLines[i].Length; j++)
					this.codeSpace[i][j] = new Token(codeLines[i][j]);
			}
		}

		public void Step()
		{
			switch (this.state)
			{
				case State.STOPPED:
					return;

				case State.WAITING_NUMBER:
					storage.Push(int.Parse(this.input));
					break;

				case State.WAITING_CHAR:
					storage.Push((int)this.input[0]);
					break;
			}

			this.state = State.RUNNING;
			this.input = null;

			if ((this.codeSpace == null) || (codeSpace.Length == 0))
			{
				this.state = State.STOPPED;
				return;
			}

			Token token = this.codeSpace[this.cursor.y][this.cursor.x];
			if (!token.isComment)
			{
				switch (token.vowel)
				{
					case 0:
						this.direction = false;
						this.speed = 1;
						break;
					case 2:
						this.direction = false;
						this.speed = 2;
						break;
					case 4:
						this.direction = false;
						this.speed = -1;
						break;

					case 6:
						this.direction = false;
						this.speed = -2;
						break;

					case 8:
						this.direction = true;
						this.speed = -1;
						break;

					case 12:
						this.direction = true;
						this.speed = -2;
						break;

					case 13:
						this.direction = true;
						this.speed = 1;
						break;

					case 17:
						this.direction = true;
						this.speed = 2;
						break;

					case 18:
						if (this.direction)
							this.speed *= -1;
						break;

					case 19:
						this.speed *= -1;
						break;

					case 20:
						if (!this.direction)
							this.speed *= -1;
						break;

				}

				int a, b;

				switch (token.initial)
				{
					case 18:
						state = State.STOPPED;
						return;
					case 3:
						a = storage.Pop();
						b = storage.Pop();
						storage.Push(b + a);
						break;
					case 4:
						a = storage.Pop();
						b = storage.Pop();
						storage.Push(b * a);
						break;
					case 16:
						a = storage.Pop();
						b = storage.Pop();
						storage.Push(b - a);
						break;
					case 2:
						a = storage.Pop();
						b = storage.Pop();
						storage.Push(b / a);
						break;
					case 5:
						a = storage.Pop();
						b = storage.Pop();
						storage.Push(b % a);
						break;
					case 6:
						switch (token.under)
						{
							case 21:
								output.Append(storage.Pop());
								break;
							case 27:
								output.Append(Convert.ToChar(storage.Pop()));
								break;
							default:
								storage.Pop();
								break;
						}
						break;
					case 7:
						switch (token.under)
						{
							case 21:
								state = State.WAITING_NUMBER;
								break;
							case 27:
								state = State.WAITING_CHAR;
								break;
							default:
								storage.Push(Aheui.strokeCounts[token.under]);
								break;
						}
						break;
					case 8:
						storage.Duplicate();
						break;
					case 17:
						storage.Swap();
						break;
					case 9:
						storage.Select(token.under);
						break;
					case 10:
						storage.Move(token.under);
						break;
					case 12:
						a = storage.Pop();
						b = storage.Pop();
						storage.Push((b >= a) ? 1 : 0);
						break;
					case 14:
						if (storage.Pop() == 0)
							speed = -speed;
						break;
				}
			}

			this.MoveCursor();
		}
		private void MoveCursor()
		{
			if (!direction)
			{
				cursor.x += speed;

				if (cursor.x < 0)
					cursor.x = codeSpace[cursor.y].Length - 1;

				else if (cursor.x >= codeSpace[cursor.y].Length)
					cursor.x = 0;
			}
			else
			{
				cursor.y += speed;

				if (cursor.y < 0)
					cursor.y = codeSpace.Length - 1;

				else if (cursor.y >= codeSpace.Length)
					cursor.y = 0;
			}
		}



		private int outputLength = 0;
		public string GetResult()
		{
			string r = this.output.ToString(outputLength, this.output.Length - outputLength);
			this.outputLength = this.output.Length;

			return r;
		}
		public string GetTotalResult()
		{
			return this.output.ToString();
		}
		public int GetResultLength()
		{
			return this.output.Length;
		}

		public void SetInput(int data)
		{
			this.input = data.ToString();
		}
		public void SetInput(char data)
		{
			this.input = Convert.ToString(data);
		}
		public void SetInput(string data)
		{
			this.input = data;
		}

		public bool isRunning
		{
			get
			{
				return (this.state != State.STOPPED);
			}
		}

		public override string ToString()
		{
			return String.Format(
				"Status\ncursor : {0}, {1}\ndirection : {2}\nspeed : {3}",
				this.cursor.x,
				this.cursor.y,
				(!this.direction ? "Horizontal" : "Vertical"),
				this.speed);
		}
	}
}
