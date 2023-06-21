namespace Chess {
	using System.Collections.Generic;
	using static System.Math;

	public static class PrecomputedMoveData {
		public static readonly int[] directionOffsets = { 8, -8, -1, 1, 7, -7, 9, -9 }; //направления: север, юг, запад, восток, северо-запад, юго-восток, северо-восток, юго-запад

		public static readonly int[][] numSquaresToEdge; //количество клеток до края. Первая скобка отвечает за направление, вторая - номер клетки. То есть: numSquaresToEdge[3][8] = 7 означает, что на восток от клетки a2 доступно 7 клеток до края доски

		public static readonly byte[][] knightMoves; //движения для коня
		public static readonly byte[][] kingMoves; //и короля

		public static readonly byte[][] pawnAttackDirections = //направления атаки для пешки - СЗ, СВ и ЮЗ, ЮВ
		{
			new byte[] { 4, 6 },
			new byte[] { 7, 5 }
		};

		//обработка атак
		public static readonly int[][] pawnAttacksWhite; //пешка атакует белых
		public static readonly int[][] pawnAttacksBlack; //пешка атакует черных
		public static readonly int[] directionLookup; //определение направления

		public static readonly ulong[] kingAttackBitboards; //короля
		public static readonly ulong[] knightAttackBitboards; //коня
		public static readonly ulong[][] pawnAttackBitboards; //пешки

		//обработка ходов
		public static readonly ulong[] rookMoves; //ладьи
		public static readonly ulong[] bishopMoves; //офицера
		public static readonly ulong[] queenMoves; //ферзя

		public static int[, ] orthogonalDistance; //мэнхеттенское измерение
		public static int[, ] kingDistance; //дистанция, измеряемая по методу Чебышева через короля
		public static int[] centreManhattanDistance; //центральное мэнхеттенское измерение

        public static int NumRookMovesToReachSquare (int startSquare, int targetSquare) //сколько ходов займет у ладьи перемещение в эту клетку, метод измерения для мэнхеттенского метода
		{
			return orthogonalDistance[startSquare, targetSquare];
		}

		public static int NumKingMovesToReachSquare (int startSquare, int targetSquare) //сколько ходов король будет добираться до этой клетки для метода Чебышева
		{
			return kingDistance[startSquare, targetSquare];
		}

		static PrecomputedMoveData () //конструктор для определения возможных ходов
		{
			pawnAttacksWhite = new int[64][]; //инициализируем хранилище атак пешки на белых
			pawnAttacksBlack = new int[64][]; //на черных
			numSquaresToEdge = new int[8][]; //массив клеток до края доски
			knightMoves = new byte[64][]; //ходы коня
			kingMoves = new byte[64][]; //короля
			numSquaresToEdge = new int[64][]; 

			//ходы дальнобойных фигур
			rookMoves = new ulong[64];
			bishopMoves = new ulong[64];
			queenMoves = new ulong[64];

			// Calculate knight jumps and available squares for each square on the board.
			// See comments by variable definitions for more info.
			int[] allKnightJumps = { 15, 17, -17, -15, 10, -6, 6, -10 }; //биты для прыжков коня
			knightAttackBitboards = new ulong[64]; //атакующие битборды коня
			kingAttackBitboards = new ulong[64]; //короля
			pawnAttackBitboards = new ulong[64][]; //пешки

			for (int squareIndex = 0; squareIndex < 64; squareIndex++) //идём по клеткам
			{
				int y = squareIndex / 8; //определяем строки
				int x = squareIndex - y * 8; //стоблцы

				int north = 7 - y; //определяем север доски. Перспективой по умолчанию является юг, поэтому север будет последней строкой
				int south = y; //юг
				int west = x; //запад
				int east = 7 - x; //восток
				numSquaresToEdge[squareIndex] = new int[8]; //определяем количество клеток на направлениях до конца доски
				numSquaresToEdge[squareIndex][0] = north; //север
				numSquaresToEdge[squareIndex][1] = south; //юг
				numSquaresToEdge[squareIndex][2] = west; //запад
				numSquaresToEdge[squareIndex][3] = east; //восток
				numSquaresToEdge[squareIndex][4] = System.Math.Min (north, west); //северо-запад. Определяется как наименьшее среди севера и запада
				numSquaresToEdge[squareIndex][5] = System.Math.Min (south, east); //юго-восток
				numSquaresToEdge[squareIndex][6] = System.Math.Min (north, east); //северо-восток
				numSquaresToEdge[squareIndex][7] = System.Math.Min (south, west); //юго-восток

				// Calculate all squares knight can jump to from current square
				var legalKnightJumps = new List<byte> (); //создаём список корректных ходов коня
				ulong knightBitboard = 0; //инициализируем битборд для коня
				foreach (int knightJumpDelta in allKnightJumps) //проходим по списку всех ходов коня
				{
					int knightJumpSquare = squareIndex + knightJumpDelta; //определяем, куда он приземлится
					if (knightJumpSquare >= 0 && knightJumpSquare < 64) //если его прыжок попал на доску
                    { 
						int knightSquareY = knightJumpSquare / 8; //определяем строку
						int knightSquareX = knightJumpSquare - knightSquareY * 8; //и столбец
						int maxCoordMoveDst = System.Math.Max (System.Math.Abs (x - knightSquareX), System.Math.Abs (y - knightSquareY)); //проверяем, чтобы он прыгал максимум на две клетки по какой-либо оси
						if (maxCoordMoveDst == 2) //если он прыгнул не больше, чем на 2
						{
							legalKnightJumps.Add ((byte) knightJumpSquare); //добавляем в список корректных ходов
							knightBitboard |= 1ul << knightJumpSquare; //корректируем битборд коня
						}
					}
				}
				knightMoves[squareIndex] = legalKnightJumps.ToArray (); //добавляем этот ход в список ходов коня
				knightAttackBitboards[squareIndex] = knightBitboard; //и в атакующий битборд тоже

				var legalKingMoves = new List<byte> (); //делаем список ходов короля
				foreach (int kingMoveDelta in directionOffsets) //сканируем направления
				{
					int kingMoveSquare = squareIndex + kingMoveDelta;
					if (kingMoveSquare >= 0 && kingMoveSquare < 64) 
					{
						int kingSquareY = kingMoveSquare / 8;
						int kingSquareX = kingMoveSquare - kingSquareY * 8;
						int maxCoordMoveDst = System.Math.Max (System.Math.Abs (x - kingSquareX), System.Math.Abs (y - kingSquareY)); //проверяем, что король сходил только на одну клетку
						if (maxCoordMoveDst == 1) 
						{
							legalKingMoves.Add ((byte) kingMoveSquare);
							kingAttackBitboards[squareIndex] |= 1ul << kingMoveSquare;
						}
					}
				}
				kingMoves[squareIndex] = legalKingMoves.ToArray ();

				List<int> pawnCapturesWhite = new List<int> (); //атаки пешек, атака на белых
				List<int> pawnCapturesBlack = new List<int> (); //на черных
				pawnAttackBitboards[squareIndex] = new ulong[2]; //создаём под эти списки два битборда
				if (x > 0) //если не крайний левый столбец
				{
					if (y < 7) //и не последняя строка
					{
						pawnCapturesWhite.Add (squareIndex + 7); //добавляем захваты
						pawnAttackBitboards[squareIndex][Board.WhiteIndex] |= 1ul << (squareIndex + 7);
					}
					if (y > 0) //для чёрных последней строкой будет первая
					{
						pawnCapturesBlack.Add (squareIndex - 9);
						pawnAttackBitboards[squareIndex][Board.BlackIndex] |= 1ul << (squareIndex - 9);
					}
				}
				if (x < 7) //если не крайний правый столбец
				{
					if (y < 7) 
					{
						pawnCapturesWhite.Add (squareIndex + 9);
						pawnAttackBitboards[squareIndex][Board.WhiteIndex] |= 1ul << (squareIndex + 9);
					}
					if (y > 0) 
					{
						pawnCapturesBlack.Add (squareIndex - 7);
						pawnAttackBitboards[squareIndex][Board.BlackIndex] |= 1ul << (squareIndex - 7);
					}
				}
				pawnAttacksWhite[squareIndex] = pawnCapturesWhite.ToArray (); //добавляем эти атаки
				pawnAttacksBlack[squareIndex] = pawnCapturesBlack.ToArray ();

				for (int directionIndex = 0; directionIndex < 4; directionIndex++) //пути ухода в закат для ладьи
				{
					int currentDirOffset = directionOffsets[directionIndex]; //отсчитываем расстояние
					for (int n = 0; n < numSquaresToEdge[squareIndex][directionIndex]; n++) //проходим до края доски
					{
						int targetSquare = squareIndex + currentDirOffset * (n + 1);
						rookMoves[squareIndex] |= 1ul << targetSquare; //добавляем в список ходов
					}
				}
				
				for (int directionIndex = 4; directionIndex < 8; directionIndex++) //то же самое для офицера
				{
					int currentDirOffset = directionOffsets[directionIndex];
					for (int n = 0; n < numSquaresToEdge[squareIndex][directionIndex]; n++) 
					{
						int targetSquare = squareIndex + currentDirOffset * (n + 1);
						bishopMoves[squareIndex] |= 1ul << targetSquare;
					}
				}
				queenMoves[squareIndex] = rookMoves[squareIndex] | bishopMoves[squareIndex]; //ходы ферзя получаем как соединение ходов ладьи и офицера
			}

			directionLookup = new int[127]; //метод для определения расстояния
			for (int i = 0; i < 127; i++) 
			{
				int offset = i - 63;
				int absOffset = System.Math.Abs (offset); //определяем отклонение
				int absDir = 1;
				if (absOffset % 9 == 0) //определяем направление
				{
					absDir = 9;
				} 
				else if (absOffset % 8 == 0) 
				{
					absDir = 8;
				} 
				else if (absOffset % 7 == 0) 
				{
					absDir = 7;
				}

				directionLookup[i] = absDir * System.Math.Sign (offset); //формируем расстояние по доске
			}

			orthogonalDistance = new int[64, 64]; //Мэнхеттенская метрика
			kingDistance = new int[64, 64]; //Чебышевская метрика
			centreManhattanDistance = new int[64]; //Центромэнхеттенская метрика
			for (int squareA = 0; squareA < 64; squareA++) //идём по клеткам
			{
				Coord coordA = BoardRepresentation.CoordFromIndex (squareA); //определяем координаты
				int fileDstFromCentre = Max (3 - coordA.fileIndex, coordA.fileIndex - 4); //определяем количество столбцов до центра
				int rankDstFromCentre = Max (3 - coordA.rankIndex, coordA.rankIndex - 4); //строк
				centreManhattanDistance[squareA] = fileDstFromCentre + rankDstFromCentre; //считаем центромэнхеттенскую метрику

				for (int squareB = 0; squareB < 64; squareB++) //проходим второй раз и считаем остальные метрики
				{
					Coord coordB = BoardRepresentation.CoordFromIndex (squareB);
					int rankDistance = Abs (coordA.rankIndex - coordB.rankIndex);
					int fileDistance = Abs (coordA.fileIndex - coordB.fileIndex);
					orthogonalDistance[squareA, squareB] = fileDistance + rankDistance; //Мэнхеттенская
					kingDistance[squareA, squareB] = Max (fileDistance, rankDistance); //Чебышев
				}
			}
		}
	}
}