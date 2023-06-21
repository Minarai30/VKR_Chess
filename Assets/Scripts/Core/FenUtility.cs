namespace Chess {
	using System.Collections.Generic;
	public static class FenUtility {

		static Dictionary<char, int> pieceTypeFromSymbol = new Dictionary<char, int> () //словарь с сокращениями названий фигур
		{
			['k'] = Piece.King, ['p'] = Piece.Pawn, ['n'] = Piece.Knight, ['b'] = Piece.Bishop, ['r'] = Piece.Rook, ['q'] = Piece.Queen
		};

		public const string startFen = "1rbq1r1k/2pp2pp/p1n3p1/2b1p3/R3P3/1BP2N2/1P3PPP/1NBQ1RK1 w - - 0 1"; //строка FEN по умолчанию - выдаёт стандартное расположение фигур в начале партии

		public static LoadedPositionInfo PositionFromFen (string fen) //загрузка позиции из строки FEN
		{
			LoadedPositionInfo loadedPositionInfo = new LoadedPositionInfo (); // инициализация информации о загруженной позиции
			string[] sections = fen.Split (' '); //разделяем строку по пробелам

			//выставляем изначальные параметры строки и столбца
			int file = 0; 
			int rank = 7;

			foreach (char symbol in sections[0]) // проходим по каждому символу в разбитой строке
			{
				if (symbol == '/') //если обнаруживаем символ перехода на следующую строку
				{
					file = 0;
					rank--; // переходим на следующую строку
				} 
				else 
				{
					if (char.IsDigit (symbol)) //если нашли цифру
					{
						file += (int) char.GetNumericValue (symbol); //добавляем её в значение столбца
					} 
					else //если ничего необычного не было найдено
					{
						int pieceColour = (char.IsUpper (symbol)) ? Piece.White : Piece.Black; //определяем цвет по регистру
						int pieceType = pieceTypeFromSymbol[char.ToLower (symbol)]; //определяем тип фигуры по сокращению
						loadedPositionInfo.squares[rank * 8 + file] = pieceType | pieceColour; //назначаем фигуру в клетку
						file++; // сдвигаем столбец
					}
				}
			}

			loadedPositionInfo.whiteToMove = (sections[1] == "w"); //определяем, кто первый ходит

			return loadedPositionInfo; //возвращаем загруженную позицию
		}

		public static string CurrentFen (Board board) //функция определения текущей строки FEN
		{
			string fen = "";
			for (int rank = 7; rank >= 0; rank--) // идём по строкам
			{
				int numEmptyFiles = 0; //инициализируем счётчик пустых столбцов
				for (int file = 0; file < 8; file++) //идём по столбцам
				{
					int i = rank * 8 + file; //определяем текущую клетку
					int piece = board.Square[i]; // определяем фигуру в клетке
					if (piece != 0) //если есть фигура
					{
						if (numEmptyFiles != 0) //если до этого были пустые столбцы
						{
							fen += numEmptyFiles; //записываем, сколько их было в строку
							numEmptyFiles = 0; //обнуляем их счетчик
						}
						bool isBlack = Piece.IsColour (piece, Piece.Black); //определяем цвет фигуры
						int pieceType = Piece.PieceType (piece); //определяем тип фигуры
						char pieceChar = ' '; //инициализируем строку для назначения фигуры
						switch (pieceType) //назначаем текстовое сокращение фигуре
						{
							case Piece.Rook: //ладья
								pieceChar = 'R';
								break;
							case Piece.Knight: //конь
								pieceChar = 'N';
								break;
							case Piece.Bishop: //офицер
								pieceChar = 'B';
								break;
							case Piece.Queen: //ферзь
								pieceChar = 'Q';
								break;
							case Piece.King: //король
								pieceChar = 'K';
								break;
							case Piece.Pawn: //пешка
								pieceChar = 'P';
								break;
						}
						fen += (isBlack) ? pieceChar.ToString ().ToLower () : pieceChar.ToString (); //выставляем регистр в зависимости от цвета
					} 
					else //если не нашли фигуру
					{
						numEmptyFiles++; //увеличиваем количество пустых столбцов
					}

				}
				if (numEmptyFiles != 0) //если так и не нашли фигуру
				{
					fen += numEmptyFiles; //записываем количество пустых столбцов в строку
				}
				if (rank != 0) //если строка не последняя, а столбцы закончились
				{
					fen += '/'; //ставим индикатор конца строки
				}
			}

			fen += ' ';
			fen += (board.WhiteToMove) ? 'w' : 'b'; // определение ходящей стороны

			return fen;
		}

		public class LoadedPositionInfo  //класс для загрузки позиции
		{
			public int[] squares; //массив клеток
			public bool whiteToMove; //определение ходящей стороны

			public LoadedPositionInfo () //конструктор позиции
			{
				squares = new int[64]; //создаёт массив клеток
			}
		}
	}
}