namespace Chess {
	using System.Collections.Generic;
    using System.Diagnostics;

    public class Board {
		//индексы
		public const int WhiteIndex = 0; // индекс белых
		public const int BlackIndex = 1; // индекс черных

		public int[] Square; // хранение фигуры в клетке. Фигура описывается следующим образом - код фигуры | код цвета

		//обработка игровых сторон
		public bool WhiteToMove; // индикатор хода белых
		public int ColourToMove; // указатель текущего цвета
		public int OpponentColour; // указатель цвета противника
		public int ColourToMoveIndex; // переменная, хранящая цвет, который ходит на данный момент

		//состояния игры
		Stack<uint> gameStateHistory; // хранилище состояний игры
		public uint currentGameState; // текущее состояние игры

		public int[] KingSquare; // индексы клеток, где находятся короли обоих цветов

		//списки фигур
		public PieceList[] rooks; // список ладей 
		public PieceList[] bishops; // список офицеров
		public PieceList[] queens; // список ферзей
		public PieceList[] knights; // список коней
		public PieceList[] pawns; // список пешек

		PieceList[] allPieceLists; // список для хранения списков всех фигур

		PieceList GetPieceList (int pieceType, int colourIndex) // метод получения списков фигур через саму фигуру
		{
			return allPieceLists[colourIndex * 8 + pieceType];
		}

		public void MakeMove (Move move) {
			currentGameState = 0; // инициализируем игру

			int opponentColourIndex = 1 - ColourToMoveIndex; // определяем цвет противника
			int moveFrom = move.StartSquare; // клетка, с которой начинается движение
			int moveTo = move.TargetSquare; // целевая клетка для передвижения

			int capturedPieceType = Piece.PieceType (Square[moveTo]); // тип срубленной фигуры
			int movePiece = Square[moveFrom]; // процесс передвижения фигуры
			int movePieceType = Piece.PieceType (movePiece); // тип передвигаемой фигуры

			int moveFlag = move.MoveFlag; // флаг передвижения
			bool isPromotion = move.IsPromotion; // проверка на превращение пешки

			// обработка срубания фигур
			currentGameState |= (ushort) (capturedPieceType << 8); // изменяем состояние игры на срубленную фигуру
			if (capturedPieceType != 0) // обрабатываем срубленную фигуру в массивах с фигурами и списке фигур
            { 
				GetPieceList (capturedPieceType, opponentColourIndex).RemovePieceAtSquare (moveTo);
			}

			// обработка передвижений в списках фигур
			if (movePieceType == Piece.King) // обрабатываем передвижение короля
            { 
				KingSquare[ColourToMoveIndex] = moveTo;
			}
            else // передвижение остальных фигур
            { 
				GetPieceList (movePieceType, ColourToMoveIndex).MovePiece (moveFrom, moveTo);
			}

			int pieceOnTargetSquare = movePiece; // сдвигаем фигуру

			// обработка особых ходов
			if (isPromotion) // превращения пешек
            {
				int promoteType = 0;
				switch (moveFlag) {
					case Move.Flag.PromoteToQueen: // превращение в ферзя
						promoteType = Piece.Queen;
						queens[ColourToMoveIndex].AddPieceAtSquare (moveTo);
						break;
					case Move.Flag.PromoteToRook: // в ладью
						promoteType = Piece.Rook;
						rooks[ColourToMoveIndex].AddPieceAtSquare (moveTo);
						break;
					case Move.Flag.PromoteToBishop: // в офицера
						promoteType = Piece.Bishop;
						bishops[ColourToMoveIndex].AddPieceAtSquare (moveTo);
						break;
					case Move.Flag.PromoteToKnight: // в коня
						promoteType = Piece.Knight;
						knights[ColourToMoveIndex].AddPieceAtSquare (moveTo);
						break;

				}
				pieceOnTargetSquare = promoteType | ColourToMove; // повышаем пешку
				pawns[ColourToMoveIndex].RemovePieceAtSquare (moveTo); // убираем пешку с поля
			} 

			// обновляем представление доски
			Square[moveTo] = pieceOnTargetSquare;
			Square[moveFrom] = 0;

			gameStateHistory.Push (currentGameState); // записываем в историю состояний

		}


		// загружаем стартовую позицию
		public void LoadStartPosition () 
		{
			LoadPosition (FenUtility.startFen);
		}

		// загрузка позиции из строки FEN
		public void LoadPosition (string fen) 
		{
			Initialize (); // инициализируем пустую доску
			var loadedPosition = FenUtility.PositionFromFen (fen); // загружаем позицию из строки

			// обрабатываем строку
			for (int squareIndex = 0; squareIndex < 64; squareIndex++) 
			{
				int piece = loadedPosition.squares[squareIndex]; // записываем фигуры в нужные биты клеток
				Square[squareIndex] = piece;

				if (piece != Piece.None) 
				{ // записываем фигуры
					int pieceType = Piece.PieceType (piece); // тип фигуры
					int pieceColourIndex = (Piece.IsColour (piece, Piece.White)) ? WhiteIndex : BlackIndex; // цвет фигуры
					if (Piece.IsSlidingPiece (piece)) // обработка конкретных типов фигур
                    { 
						if (pieceType == Piece.Queen) // ферзь
                        { 
							queens[pieceColourIndex].AddPieceAtSquare (squareIndex);
						} 
						else if (pieceType == Piece.Rook) // ладья
						{
							rooks[pieceColourIndex].AddPieceAtSquare (squareIndex);
						} 
						else if (pieceType == Piece.Bishop) // офицер
						{
							bishops[pieceColourIndex].AddPieceAtSquare (squareIndex);
						}
					} 
					else if (pieceType == Piece.Knight) // конь
					{
						knights[pieceColourIndex].AddPieceAtSquare (squareIndex);
					} 
					else if (pieceType == Piece.Pawn) // пешка
					{
						pawns[pieceColourIndex].AddPieceAtSquare (squareIndex);
					} 
					else if (pieceType == Piece.King) // король
					{
						KingSquare[pieceColourIndex] = squareIndex;
					}
				}
			}

			// определяем стороны
			WhiteToMove = loadedPosition.whiteToMove;
			ColourToMove = (WhiteToMove) ? Piece.White : Piece.Black;
			OpponentColour = (WhiteToMove) ? Piece.Black : Piece.White;
			ColourToMoveIndex = (WhiteToMove) ? 0 : 1;

        }

		void Initialize () // инициализируем доску
        { 
			Square = new int[64];
			KingSquare = new int[2];

			gameStateHistory = new Stack<uint> ();

			knights = new PieceList[] { new PieceList (10), new PieceList (10) };
			pawns = new PieceList[] { new PieceList (8), new PieceList (8) };
			rooks = new PieceList[] { new PieceList (10), new PieceList (10) };
			bishops = new PieceList[] { new PieceList (10), new PieceList (10) };
			queens = new PieceList[] { new PieceList (9), new PieceList (9) };
			PieceList emptyList = new PieceList (0);
			allPieceLists = new PieceList[] 
			{
				emptyList,
				emptyList,
				pawns[WhiteIndex],
				knights[WhiteIndex],
				emptyList,
				bishops[WhiteIndex],
				rooks[WhiteIndex],
				queens[WhiteIndex],
				emptyList,
				emptyList,
				pawns[BlackIndex],
				knights[BlackIndex],
				emptyList,
				bishops[BlackIndex],
				rooks[BlackIndex],
				queens[BlackIndex],
			};
		}
	}
}