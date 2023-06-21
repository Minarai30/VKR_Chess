namespace Chess {
	public static class Piece {
		//обозначения фигур в клетке
		public const int None = 0; //нет фигуры
		public const int King = 1; //король
		public const int Pawn = 2; //пешка
		public const int Knight = 3; //конь
		public const int Bishop = 5; //слон
		public const int Rook = 6; //ладья
		public const int Queen = 7; //ферзь

		public const int White = 8; //обозначение белого цвета
		public const int Black = 16; //обозначение черного цвета

		//битовые маски
		const int typeMask = 0b00111; //тип фигуры
		const int blackMask = 0b10000; //черный цвет
		const int whiteMask = 0b01000; //белый цвет
		const int colourMask = whiteMask | blackMask; //маска текущего цвета

		public static bool IsColour (int piece, int colour) //метод назначения конкретного цвета фигуре
		{
			return (piece & colourMask) == colour;
		}

		public static int Colour (int piece) //формирование маски цвета
		{
			return piece & colourMask;
		}

		public static int PieceType (int piece) //формирование маски типа фигуры
		{
			return piece & typeMask;
		}

		public static bool IsRookOrQueen (int piece) //определение ладьи или ферзя
		{
			return (piece & 0b110) == 0b110;
		}

		public static bool IsBishopOrQueen (int piece) //определение офицера или ферзя
		{
			return (piece & 0b101) == 0b101;
		}

		public static bool IsSlidingPiece (int piece) //определение дальнобойной фигуры
		{
			return (piece & 0b100) != 0;
		}
	}
}