using System;
namespace Chess {
	public struct Coord : IComparable<Coord> {
		public readonly int fileIndex;
		public readonly int rankIndex;

		public Coord (int fileIndex, int rankIndex) 
		{
			this.fileIndex = fileIndex; //столбцы
			this.rankIndex = rankIndex; //строки
		}

		public bool IsLightSquare () //проверка на цвет клетки
		{
			return (fileIndex + rankIndex) % 2 != 0;
		}

		public int CompareTo (Coord other) //проверка на совпадение координат
		{
			return (fileIndex == other.fileIndex && rankIndex == other.rankIndex) ? 0 : 1;
		}
	}
}