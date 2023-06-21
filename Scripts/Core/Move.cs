namespace Chess {

	public readonly struct Move {

		public readonly struct Flag //флаги особых движений
		{
			public const int None = 0; //нет особого
			public const int PromoteToQueen = 1; //повышение в ферзя
			public const int PromoteToKnight = 2; //коня
			public const int PromoteToRook = 3; //ладью
			public const int PromoteToBishop = 4; //офицера
			public const int PawnTwoForward = 5; //ход пешки на две клетки
		}

		readonly ushort moveValue; //битовая запись хода

		const ushort startSquareMask = 0b0000000000111111; //битовая маска начальной клетки
		const ushort targetSquareMask = 0b0000111111000000; //битовая маска конечной клетки

		public Move (ushort moveValue) //метод оформления хода через битовую запись
		{
			this.moveValue = moveValue;
		}

		public Move (int startSquare, int targetSquare) //через начальную и конечную клетку
		{
			moveValue = (ushort) (startSquare | targetSquare << 6); //путем побитового сдвига
		}

		public Move (int startSquare, int targetSquare, int flag) //то же самое для особых ходов
		{
			moveValue = (ushort) (startSquare | targetSquare << 6 | flag << 12);
		}

		public int StartSquare //определение начальной клетки хода по битовой маске
		{
			get 
			{
				return moveValue & startSquareMask;
			}
		}

		public int TargetSquare //определение конечной клетки хода по битовой маске
		{
			get 
			{
				return (moveValue & targetSquareMask) >> 6;
			}
		}

		public bool IsPromotion //метод определения повышения
		{
			get 
			{
				int flag = MoveFlag; //забираем флаг передвижения
				return flag == Flag.PromoteToQueen || flag == Flag.PromoteToRook || flag == Flag.PromoteToKnight || flag == Flag.PromoteToBishop; //возвращаем это повышение
			}
		}

		public int MoveFlag //флаг передвижения
		{
			get 
			{
				return moveValue >> 12;
			}
		}

		public int PromotionPieceType 
		{
			get 
			{
				switch (MoveFlag) //смотрим по флагу, не произошло ли повышения
				{
					case Flag.PromoteToRook: //в ладью
						return Piece.Rook;
					case Flag.PromoteToKnight: //коня
						return Piece.Knight;
					case Flag.PromoteToBishop: //офицера
						return Piece.Bishop;
					case Flag.PromoteToQueen: //ферзя
						return Piece.Queen;
					default:
						return Piece.None; //если повышения не было, говорим об этом
				}
			}
		}

		public static Move InvalidMove //флаг неправильного хода
		{
			get 
			{
				return new Move (0); //отменяем этот ход
			}
		}

		public ushort Value //получение битового значения передвижения
		{
			get 
			{
				return moveValue;
			}
		}

		public bool IsInvalid //обнуление битовой маски передвижения при неправильном ходе
		{
			get 
			{
				return moveValue == 0;
			}
		}

		public string Name //генерация нотации для передвижения
		{
			get 
			{
				return BoardRepresentation.SquareNameFromIndex (StartSquare) + "-" + BoardRepresentation.SquareNameFromIndex (TargetSquare);
			}
		}
	}
}
