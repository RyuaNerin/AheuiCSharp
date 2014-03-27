using System;
using System.Linq;
using System.Text;
using System.Collections;
using System.IO;

namespace RyuaNerin
{
	public class Aheui
	{
		internal class Storage
		{
			private static char[] stackName =
			{
				'아', '악', '앆', '앇', '안',
				'앉', '않', '앋', '알', '앍',
				'앎', '앏', '앐', '앑', '앒',
				'앓', '암', '압', '앖', '앗',
				'았', '앙', '앚', '앛', '앜',
				'앝', '앞', '앟'
			};
			private ExtendedArray[] Storages;
			private int id;

			private int _StackMinSize = 32;
			public int StackMinSize
			{
				get
				{
					return this._StackMinSize;
				}
				set
				{
					this._StackMinSize = value;

					for (int i = 0; i < 28; ++i)
						this.Storages[i].StackMinSize = value;
				}
			}

			private int _StackMaxSize = 2048;
			public int StackMaxSize
			{
				get
				{
					return this._StackMaxSize;
				}
				set
				{
					this._StackMaxSize = value;

					for (int i = 0; i < 28; ++i)
						this.Storages[i].StackMaxSize = value;
				}
			}

			public Storage()
			{
				this.id = 0;
				Storages = new ExtendedArray[28];
				for (int i = 0; i < 28; ++i)
					this.Storages[i] = new ExtendedArray(this.StackMinSize, this.StackMaxSize);
			}
			public Storage(int MinStackSize, int MaxStackSize)
			{
				this.id = 0;
				Storages = new ExtendedArray[28];
				for (int i = 0; i < 28; ++i)
					this.Storages[i] = new ExtendedArray(MinStackSize, MaxStackSize);
			}

			public void Clear()
			{
				this.id = 0;
				for (int i = 0; i < 28; ++i)
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

			public override string ToString()
			{
				StringBuilder sb = new StringBuilder();

				for (int i = 0; i < 28; ++i)
					sb.AppendFormat("{0} : {1}\n", Storage.stackName[i], this.Storages[i].ToString());

				return sb.ToString();
			}
		}
		internal class ExtendedArray
		{
			public int StackMinSize { get; set; }
			public int StackMaxSize { get; set; }

			private int[] stack;
			private int length = 0;
			private int capacity;

			public ExtendedArray(int StackMinSize, int StackMaxSize)
			{
				this.StackMinSize = StackMinSize;
				this.StackMaxSize = StackMaxSize;

				this.capacity = StackMinSize;
				this.length = 0;
				this.stack = new int[capacity];
			}

			public void Clear()
			{
				this.length = 0;
			}

			private void ReduceSize()
			{
				if ((this.length < this.capacity / 2) && (this.StackMinSize < this.capacity))
				{
					int newCapacity;

					if (this.capacity < 2048)
						newCapacity = this.capacity / 2;
					else
						newCapacity = this.capacity - 2048;

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
					int newCapacity;

					if (this.capacity < 2048)
						newCapacity = this.capacity * 2;
					else
						newCapacity = this.capacity + 2048;

					if ((this.StackMaxSize > 0) && (newCapacity > this.StackMaxSize))
						newCapacity = this.StackMaxSize;

					if (this.capacity == newCapacity)
						return;

					int[] stackTmp = new int[newCapacity];
					Array.Copy(this.stack, 0, stackTmp, 0, this.capacity);
					this.stack = stackTmp;

					this.capacity = newCapacity;
				}
			}

			public void Push(int value)
			{
				this.ExpandSize();

				this.stack[this.length] = value;

				this.length++;
			}

			public void Unshift(int value)
			{
				this.ExpandSize();

				for (int i = 0; i < this.length - 1; ++i)
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

				for (int i = 1; i < this.length; ++i)
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

			public override string ToString()
			{
				StringBuilder sb = new StringBuilder();

				for (int i = 0; i < this.length; ++i)
					sb.AppendFormat("{0}, ", this.stack[i]);

				if (sb.Length > 0)
					sb.Remove(sb.Length - 2, 2);

				return sb.ToString();
			}
		}
		public struct Token
		{
			public static readonly char[] INITIALS = 
			{
				'ㄱ', 'ㄲ', 'ㄴ', 'ㄷ', 'ㄸ',
				'ㄹ', 'ㅁ', 'ㅂ', 'ㅃ', 'ㅅ',
				'ㅆ', 'ㅇ', 'ㅈ', 'ㅉ', 'ㅊ',
				'ㅋ', 'ㅌ', 'ㅍ', 'ㅎ'
			};
			public static readonly char[] VOWELS =
			{
				'ㅏ', 'ㅐ', 'ㅑ', 'ㅒ', 'ㅓ',
				'ㅔ', 'ㅕ', 'ㅖ', 'ㅗ', 'ㅘ',
				'ㅙ', 'ㅚ', 'ㅛ', 'ㅜ', 'ㅝ',
				'ㅞ', 'ㅟ', 'ㅠ', 'ㅡ', 'ㅢ',
				'ㅣ'
			};
			public static readonly char[] UNDERS =
			{
				'　', 'ㄱ', 'ㄲ', 'ㄳ', 'ㄴ',
				'ㄵ', 'ㄶ', 'ㄷ', 'ㄹ', 'ㄺ',
				'ㄻ', 'ㄼ', 'ㄽ', 'ㄾ', 'ㄿ',
				'ㅀ', 'ㅁ', 'ㅂ', 'ㅄ', 'ㅅ',
				'ㅆ', 'ㅇ', 'ㅈ', 'ㅊ', 'ㅋ',
				'ㅌ', 'ㅍ', 'ㅎ'
			};

			public char charCode;
			public bool isComment;
			public int initial;
			public int vowel;
			public int under;

			public Token(char code)
			{
				this.charCode = code;
				this.isComment = false;

				int charCode = Convert.ToInt32(code) - 44032;
				if ((charCode < 0) || (charCode > 11171))
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

				return String.Format("[{0} = {0}, {1}, {2}]", this.charCode, Token.INITIALS[initial], Token.VOWELS[vowel], Token.UNDERS[under]);
			}
		}

		public struct Cursor
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

		private static readonly int[] strokeCounts =
		{
			0, 2, 4, 4, 2,	//　ㄱㄲㄳㄴ
			5, 5, 3, 5, 7,	//ㄵㄶㄷㄹㄺ
			9, 9, 7, 9, 9,	//ㄻㄼㄽㄾㄿ
			8, 4, 4, 6, 2,	//ㅀㅁㅂㅄㅅ
			4, 1, 3, 4, 3,	//ㅆㅇㅈㅊㅋ
			4, 4, 3			//ㅌㅍㅎ
		};

		public int StackMinSize { get { return this.storage.StackMinSize; } set { this.storage.StackMinSize = value; } }
		public int StackMaxSize { get { return this.storage.StackMaxSize; } set { this.storage.StackMaxSize = value; } }

		private Token[][] codeSpace;

		private string input;
		private StringBuilder output;
		public State state { get; internal set; }

		private Storage storage;
		private bool direction;
		private int speed;
		private Cursor _cursor;
		public Cursor NowCursor { get { return this._cursor; } }

		public Token NowCursorToken
		{
			get
			{
				return this.codeSpace[this._cursor.y][this._cursor.x];
			}
		}

		public Aheui()
		{
			this.codeSpace = null;
			this.storage = new Storage();
			this._cursor = new Cursor(0, 0);
			this.init();
		}
		public Aheui(int MinStackSize, int MaxStackSize)
		{
			this.codeSpace = null;
			this.storage = new Storage(MinStackSize, MaxStackSize);
			this._cursor = new Cursor(0, 0);
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
			this._cursor.x = this._cursor.y = 0;
		}
		public void init(string input)
		{
			this.init();

			//////////////////////////////////////////////////////////////////////////

			if ((input != null) && (input.Length == 0))
				this.codeSpace = null;

			string[] codeLines = input.Replace("\r\n", "\n").Replace("\r", "\n").Split('\n');
			
			this.codeSpace = new Token[codeLines.Length][];
			
			for (int i = 0; i < codeLines.Length; ++i)
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

			Token token = this.codeSpace[this._cursor.y][this._cursor.x];
			if (!token.isComment)
			{
				switch (token.vowel)
				{
					case 0:  // ㅏ
						this.direction = false; this.speed = 1; break;

					case 2:  // ㅑ
						this.direction = false; this.speed = 2; break;

					case 4:  // ㅓ
						this.direction = false; this.speed = -1; break;

					case 6:  // ㅕ
						this.direction = false; this.speed = -2; break;

					case 8:  // ㅗ
						this.direction = true; this.speed = -1; break;

					case 12: // ㅛ
						this.direction = true; this.speed = -2; break;

					case 13: // ㅜ
						this.direction = true; this.speed = 1; break;

					case 17: // ㅠ
						this.direction = true; this.speed = 2; break;

					case 18: // ㅡ
						if (this.direction) this.speed *= -1; break;

					case 19: // ㅢ
						this.speed *= -1; break;

					case 20: // ㅣ
						if (!this.direction) this.speed *= -1; break;
				}

				int a, b;

				switch (token.initial)
				{
					case 18: // ㅎ
						state = State.STOPPED;
						return;
					case 3:  // ㄷ
						a = storage.Pop();
						b = storage.Pop();
						storage.Push(b + a);
						break;
					case 4:  // ㄸ
						a = storage.Pop();
						b = storage.Pop();
						storage.Push(b * a);
						break;
					case 16: // ㅌ
						a = storage.Pop();
						b = storage.Pop();
						storage.Push(b - a);
						break;
					case 2:  // ㄴ
						a = storage.Pop();
						b = storage.Pop();
						storage.Push(b / a);
						break;
					case 5:  // ㄹ
						a = storage.Pop();
						b = storage.Pop();
						storage.Push(b % a);
						break;
					case 6:  // ㅁ
						switch (token.under)
						{
							case 21: // ㅇ
								output.Append(storage.Pop());
								break;
							case 27: // ㅎ
								output.Append(Convert.ToChar(storage.Pop()));
								break;
							default:
								storage.Pop();
								break;
						}
						break;
					case 7: // ㅂ
						switch (token.under)
						{
							case 21: // ㅇ
								state = State.WAITING_NUMBER;
								break;
							case 27: // ㅎ
								state = State.WAITING_CHAR;
								break;
							default:
								storage.Push(Aheui.strokeCounts[token.under]);
								break;
						}
						break;
					case 8:  // ㅃ
						storage.Duplicate();
						break;
					case 17: // ㅍ
						storage.Swap();
						break;
					case 9:  // ㅅ
						storage.Select(token.under);
						break;
					case 10: // ㅆ
						storage.Move(token.under);
						break;
					case 12: // ㅈ
						a = storage.Pop();
						b = storage.Pop();
						storage.Push((b >= a) ? 1 : 0);
						break;
					case 14: // ㅊ
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
				_cursor.x += speed;

				if (_cursor.x < 0)
					_cursor.x = codeSpace[_cursor.y].Length - 1;

				else if (_cursor.x >= codeSpace[_cursor.y].Length)
					_cursor.x = 0;
			}
			else
			{
				_cursor.y += speed;

				if (_cursor.y < 0)
					_cursor.y = codeSpace.Length - 1;

				else if (_cursor.y >= codeSpace.Length)
					_cursor.y = 0;
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
				"cursor - {2} - [{0}, {1}]\ndirection : {3}\nspeed : {4}\nStacks Info\n{5}",
				this._cursor.x,
				this._cursor.y,
				this.NowCursorToken.charCode,
				(!this.direction ? "Horizontal" : "Vertical"),
				this.speed,
				this.storage.ToString());
		}
	}
}
