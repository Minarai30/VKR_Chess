namespace Chess {
	using System.Collections.Generic;
	using static PrecomputedMoveData;
	using static BoardRepresentation;
    using System.Diagnostics;

	public class MoveGenerator {

		public enum PromotionMode { All, QueenOnly, QueenAndKnight } //режимы повышений пешки: все, только ферзь и ферзь и конь

		public PromotionMode promotionsToGenerate = PromotionMode.All; //устанавливаем режим доступности всех повышений

		List<Move> moves; //список ходов
		int friendlyColour; //цвет игрока
		int opponentColour; //цвет противника
		int friendlyKingSquare; //положение короля игрока
		int friendlyColourIndex; //бит цвета игрока

		bool inCheck; //индикатор шаха
		bool inDoubleCheck; //индикатор двойного шаха
		ulong checkRayBitmask; //проверка битовой маски шаха

        static int enemies;
        static int friendlies;

        bool genQuiets;
		Board board;

		public List<Move> GenerateMoves (Board board, bool includeQuietMoves = true) //сам метод генерации ходов
		{
			this.board = board; //забираем доску
			genQuiets = includeQuietMoves; //включаем безопасные ходы
			Init (); //инициализируем генератор

			GenerateKingMoves (); //генерируем ходы для короля

			if (inDoubleCheck) //если был объявлен двойной шах, то единственная фигура, которая может ходить - король
			{
				return moves;
			}

			GenerateSlidingMoves (); //генерация ходов дальнобойных фигур
			GenerateKnightMoves (); //генерация ходов коня
			GeneratePawnMoves (); //генерация ходов пешек

            return moves;
		}

		public bool InCheck () //метод проверки на шах
		{
			return inCheck;
		}

		void Init () //инициализация генератора
		{
			moves = new List<Move> (64); //объявляем массив ходов
			inCheck = false; //шаха пока нет
			inDoubleCheck = false; //двойного тоже
			checkRayBitmask = 0; //инициализируем маску шаха

			enemies = 0;
			friendlies = 0;

            friendlyColour = board.ColourToMove; //забираем цвет игрока
			opponentColour = board.OpponentColour; //цвет противника
			friendlyKingSquare = board.KingSquare[board.ColourToMoveIndex]; //определяем местонахождение дружественного короля
			friendlyColourIndex = (board.WhiteToMove) ? Board.WhiteIndex : Board.BlackIndex; // определяем бит цвета игрока
		}

		void GenerateKingMoves () //генерируем ходы короля
		{
			for (int i = 0; i < kingMoves[friendlyKingSquare].Length; i++) //идём по возможным ходам короля
			{
				int targetSquare = kingMoves[friendlyKingSquare][i]; //проходим по клеткам
				int pieceOnTargetSquare = board.Square[targetSquare]; //проверяем наличие фигур на клетках

				if (Piece.IsColour (pieceOnTargetSquare, friendlyColour)) //в случае наличия в клетке фигуры игрока пропускаем эту клетку
				{
					friendlies++;
					continue;
				}

				bool isCapture = false;
				if (Piece.IsColour (pieceOnTargetSquare, opponentColour)) //если в клетке стоит противник, отмечаем его как потенциальную жертву
                {
					isCapture = Piece.IsColour(pieceOnTargetSquare, opponentColour);
					enemies++;
                } 
				if (!isCapture)
				{
					if (!genQuiets || SquareIsInCheckRay (targetSquare)) //проверяем, не охраняется ли эта фигура противником. Если да, то пропускаем эту клетку
					{
						continue;
					}
				}

			}
		}

		void GenerateSlidingMoves () //распределитель генераторов ходов дальнобойных фигур
		{
			PieceList rooks = board.rooks[friendlyColourIndex]; //забираем список ладей
			for (int i = 0; i < rooks.Count; i++) //и генерируем для них ходы
			{
				GenerateSlidingPieceMoves (rooks[i], 0, 4);
			}

			PieceList bishops = board.bishops[friendlyColourIndex]; //офицеры
			for (int i = 0; i < bishops.Count; i++) 
			{
				GenerateSlidingPieceMoves (bishops[i], 4, 8);
			}

			PieceList queens = board.queens[friendlyColourIndex]; //ферзи
			for (int i = 0; i < queens.Count; i++) 
			{
				GenerateSlidingPieceMoves (queens[i], 0, 8);
			}

		}

		void GenerateSlidingPieceMoves (int startSquare, int startDirIndex, int endDirIndex) //генерируем ходы для дальнобойных фигур
		{
			for (int directionIndex = startDirIndex; directionIndex < endDirIndex; directionIndex++) //проходим по возможным направлениям хода
			{
				int currentDirOffset = directionOffsets[directionIndex]; //определяем смещение по направлениям

				for (int n = 0; n < numSquaresToEdge[startSquare][directionIndex]; n++) //проходим по клеткам направления до края доски
				{
					int targetSquare = startSquare + currentDirOffset * (n + 1);
					int targetSquarePiece = board.Square[targetSquare];

					if (Piece.IsColour (targetSquarePiece, friendlyColour)) //если находим дружественную фигуру, клетки за ней не смотрим
					{
						friendlies++; //и записываем в дружественные фигуры для связки
						break;
					}
					bool isCapture = false; //переменная для обозначения фигуры противника

                    if (targetSquarePiece != Piece.None) //если нашли фигуру противника
					{
                        enemies++; //записываем её в найденных противников для двойного удара
                        isCapture = true;
					} 

					bool movePreventsCheck = SquareIsInCheckRay (targetSquare); //проверяем, защищает ли этот ход от шаха
					if (movePreventsCheck || !inCheck) //если ход предотвращает шах или шаха нет
					{
						if (genQuiets || isCapture) //либо клетка пустая или можем срубить фигуру
						{
							moves.Add (new Move (startSquare, targetSquare)); //добавляем этот ход
                        }
					}
					if (isCapture || movePreventsCheck)  //не смотрим клетки за вражескими фигурами и в случае наличия шаха можно только защититься от него
					{
                        break;
					}
				}
			}
		}

		void GenerateKnightMoves () //ходы коня
		{
			PieceList myKnights = board.knights[friendlyColourIndex]; //забираем коней

			for (int i = 0; i < myKnights.Count; i++) //проходим по коням
			{
				int startSquare = myKnights[i];

				for (int knightMoveIndex = 0; knightMoveIndex < knightMoves[startSquare].Length; knightMoveIndex++) //идем по ходам коня
				{
					int targetSquare = knightMoves[startSquare][knightMoveIndex];
					int targetSquarePiece = board.Square[targetSquare];
					bool isCapture = false; 
					
					if (Piece.IsColour (targetSquarePiece, opponentColour))
					{
						isCapture = Piece.IsColour(targetSquarePiece, opponentColour);
						enemies++;
                    }
					if (genQuiets || isCapture) 
					{
						if (Piece.IsColour (targetSquarePiece, friendlyColour) || (inCheck && !SquareIsInCheckRay (targetSquare))) 
						{
							friendlies++;
							continue;
						}
						moves.Add (new Move (startSquare, targetSquare));
					}
				}
			}
		}

		void GeneratePawnMoves () //ходы пешек
		{
			PieceList myPawns = board.pawns[friendlyColourIndex]; //забираем пространственные данные пешек и формируем список пешек
			int pawnOffset = (friendlyColour == Piece.White) ? 8 : -8; //смещение пешки
			int startRank = (board.WhiteToMove) ? 1 : 6; //стандартное начальное положение пешки
			int finalRankBeforePromotion = (board.WhiteToMove) ? 6 : 1; //предпоследняя строка для пешки

			for (int i = 0; i < myPawns.Count; i++) //идём по пешкам
			{
				int startSquare = myPawns[i];
				int rank = RankIndex (startSquare); //забираем строку 
				bool oneStepFromPromotion = rank == finalRankBeforePromotion; //запоминаем, если пешка в одной строке от повышения

				if (genQuiets) //генерируем безопасные ходы
				{
					int squareOneForward = startSquare + pawnOffset; //сдвижение пешки на клетку

					if (board.Square[squareOneForward] == Piece.None) //если клетка перед пешкой пуста
					{
						if (!inCheck || SquareIsInCheckRay (squareOneForward)) //если нет шаха или пешка закрывает короля этим ходом
						{
							if (oneStepFromPromotion) //если пешка была в шаге от повышения
							{
								MakePromotionMoves (startSquare, squareOneForward); //повышаем её
							} 
							else 
							{
								moves.Add (new Move (startSquare, squareOneForward)); //если нет, то она просто идёт
							}
						}

						if (rank == startRank) //если пешка находится в стандартном начальном положении
						{
							int squareTwoForward = squareOneForward + pawnOffset; //то она может сходить на две клетки
							if (board.Square[squareTwoForward] == Piece.None) //аналогичные случаю с одной клеткой проверки
							{
								if (!inCheck || SquareIsInCheckRay (squareTwoForward)) 
								{
									moves.Add (new Move (startSquare, squareTwoForward, Move.Flag.PawnTwoForward));
								}
							}
						}
					}
				}

				for (int j = 0; j < 2; j++) //атака пешки
				{
					if (numSquaresToEdge[startSquare][pawnAttackDirections[friendlyColourIndex][j]] > 0) //проверяем две клетки по диагонали перед пешкой
                    {
						int pawnCaptureDir = directionOffsets[pawnAttackDirections[friendlyColourIndex][j]];
						int targetSquare = startSquare + pawnCaptureDir;
						int targetPiece = board.Square[targetSquare];

                        if (Piece.IsColour(targetPiece, friendlyColour)) //сначала проверим наличие дружественных фигур по направлениям атаки для связки
                        {
                            friendlies++;
                        }

                        if (Piece.IsColour (targetPiece, opponentColour)) //дальше ищем врагов
						{
							enemies++;
							if (inCheck && !SquareIsInCheckRay (targetSquare)) //если объявлен шах и пешка не защищает короля(либо нахождением в клетке, либо срубанием врага), то пропускаем такой ход
							{
								continue;
							}
							if (oneStepFromPromotion) //если противник находился на последней строке
							{
								MakePromotionMoves (startSquare, targetSquare); //празднуем победу над ним повышением пешки
							} 
							else //иначе просто идём
							{
								moves.Add (new Move (startSquare, targetSquare));
							}
						}

                    }
				}
			}
		}

		void MakePromotionMoves (int fromSquare, int toSquare) //повышения пешки
		{
			moves.Add (new Move (fromSquare, toSquare, Move.Flag.PromoteToQueen)); //стандартное повышение в ферзя
			if (promotionsToGenerate == PromotionMode.All) //добавляем остальные повышения, если указана эта опция
			{
				moves.Add (new Move (fromSquare, toSquare, Move.Flag.PromoteToKnight)); //конь
				moves.Add (new Move (fromSquare, toSquare, Move.Flag.PromoteToRook)); //ладья
				moves.Add (new Move (fromSquare, toSquare, Move.Flag.PromoteToBishop)); //офицер
			} 
			else if (promotionsToGenerate == PromotionMode.QueenAndKnight) //или добавляем только коня
			{
				moves.Add (new Move (fromSquare, toSquare, Move.Flag.PromoteToKnight));
			}

		}

		bool SquareIsInCheckRay (int square) //битовое определение шаха
		{
			return inCheck && ((checkRayBitmask >> square) & 1) != 0;
		}

		public bool LessonChecker(int Lesson)
		{
			if (friendlies >= 1 && Lesson == 2) return true;
			if (enemies >= 2 && Lesson == 3) return true;

			return false;
		}

	}

}