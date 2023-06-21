using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net;
using UnityEngine;

namespace Chess.Game
{
	public class PositionGenerator
	{
		static string PlayerPiece; //переменная для хранения фигуры, которой будут выполняться уроки
		static Coord PlayerCoord; //координаты фигуры игрока

		bool LightSquare; //цвет клетки
		bool Horizontal; //режим появления фигур - горизонтальный или вертикальный

		static int AnotherPieceFile; //вспомогательная переменная со столбцом целевой клетки для генерации задач пешки

		static bool BindingLesson; //индикатор урока "Связка"
		static bool MovingLesson; //индикатор урока "Передвижение фигур"

		static string GenPieceType(bool White) //генерация фигур
		{
			int randPiece = Random.Range(0, 5);

			switch (randPiece)
            {
                case 0: //пешка
					if (!White) return "p";
					else return "P";
				case 1: //ладья
					if (!White) return "r";
					else return "R";
				case 2: //конь
					if (!White) return "n";
					else return "N";
				case 3: //слон
					if (!White) return "b";
					else return "B";
				case 4: //ферзь
					if (!White) return "q";
					else return "Q";
				default: //король
					if (!White) return "k";
					else return "K";
			}
		}

		string[] GenPieces(bool Position, string[] Rows, bool White) //генерация целевых фигур
        {
			int Rank = Random.Range(0, 7); //строка
			int File = Random.Range(0, 6); //столбец
			int Interval = Random.Range(2, 6 - File); //интервал между фигурами

			int RandomVariant;

			if (PlayerPiece == "N") //изменение интервала для коня, чтобы он мог атаковать обе фигуры
			{
				RandomVariant = Random.Range(1, 3);

				if (RandomVariant == 1) Interval = 1;
				else Interval = 3;

				if (!Position) Interval++;
			}

			if (PlayerPiece == "P" || PlayerPiece == "p") Rank = Random.Range(0, 3); //обеспечение появления противника на другой стороне поля от пешки

			if (BindingLesson) //генерация фигуры для урока связки
            {
				if (Rank == 7) Rows[7] = File + GenPieceType(!White);
				else Rows[Rank] = File + GenPieceType(!White);
			}

			else //генерация для двойного удара
            {
                if (!Position && PlayerPiece != "P") //если позиция вертикальная и фигура игрока не пешка
                {
                    if (PlayerPiece != "N") Interval = Random.Range(2, 6 - Rank); //если у игрока не конь, то генерируем интервал

					if (PlayerPiece == "B") //если у игрока слон
					{
						Interval = 2 * Random.Range(1, 3); //устанавливаем интервал

                        if (File + Interval > 7) //проверяем, чтобы фигура не ушла за пределы доски
						{
                            File = 1;
							Interval = 4;
						}
					}
                  
					if (Rank >= 7) //если первая фигура стоит на краю доски
                    {
						Rows[7] = "K" + File + GenPieceType(White);
                        Rows[5] = File + GenPieceType(White); //ставим вторую на две клетки выше
                    }

                    else if (Rank + Interval >= 7) //проверка предела доски
                    {
                        Rows[5] = File + GenPieceType(White);
                        Rows[7] = File + GenPieceType(White);
                    }

                    else //стандартная генерация без особых случаев
                    {
						Rows[Rank] = File + GenPieceType(White);
						Rows[Rank + Interval] = File + GenPieceType(White);
                    }

                    LightSquare = BoardRepresentation.LightSquare(File, Rank); //записываем цвет клетки

                }

                else //если позиция горизонтальная или у игрока пешка
				{
					if (PlayerPiece == "P") //устанавливаем задачу для пешки
					{
						if (White) Rank = 7 - Rank; //инвертируем положение по столбцам для белых
						Interval = 1;
					}

                    if (PlayerPiece == "B") //если у игрока слон, поступаем аналогично случаю с вертикальной позицией
                    {
                        Interval = 2 * Random.Range(1, 3) - 1;
						Debug.Log(Interval);

                        if (File + Interval > 7)
                        {
                            File = 1;
                            Interval = 3;
                        }
                    }

					Rows[Rank] = File + GenPieceType(White) + Interval + GenPieceType(White); //генерируем фигуры

                    AnotherPieceFile = File; //запоминаем столбец левой фигуры для пешки

                    LightSquare = BoardRepresentation.LightSquare(File, Rank);
                }
			}

			return Rows;
		}

		string[] GenPlayerPiece(string[] Rows, bool White) //генерация положения игуры игрока
        {
			int Rank = Random.Range(0, 7);
			int File = Random.Range(0, 7);

			if (PlayerPiece == "P") //если была сгенерирована пешка
			{
				if (!MovingLesson) File = AnotherPieceFile + 1; //если это не урок передвижения, ставим пешку в сторону от целевой клетки на столбец

				Rank = Random.Range(5, 7); //генерируем белую пешку внизу поля
				if (!White) Rank = Random.Range(0, 2); //а черную вверху
				if (Rank < 0) Rank = 0;
			}

			if (PlayerPiece == "P") //контролируем, чтобы пешка не появлялась на последних строках
            {
                if (Rank == 7 && !White) Rank--;
                if (Rank == 0 && White) Rank++;
            }
                

			if (PlayerPiece == "B") //сдвигаем офицера на столбец в случае несовпадения цветов клеток
			{
				if (LightSquare != BoardRepresentation.LightSquare(File, Rank)) File++;
			}

            PlayerCoord = new Coord(File, Rank); //запоминаем положение фигуры игрока

            if (!White) PlayerPiece = PlayerPiece.ToLower(); //снижаем регистр для превращения фигуры игрока в чёрную фигуру, если это требуется

            if (Rows[Rank] == "8") Rows[Rank] = File + PlayerPiece; //вставляем фигуру игрока в строку
			else Rows[Rank + 1] = File + PlayerPiece; //либо сдвигаем на строку выше

			return Rows;
		}


		public static bool CheckPlayerPosition(int Lesson) //проверка позиции игрока для уроков "Двойной удар" и "Связка"
        {
            MoveGenerator moveGenerator = new MoveGenerator(); //запускаем генератор ходов

			if (moveGenerator.LessonChecker(Lesson) && !BindingLesson && !MovingLesson) return true; //если игрок атаковал две цели и включён урок "Двойной удар", засчитываем успешное выполнение
			if (moveGenerator.LessonChecker(Lesson) && BindingLesson) return true; //если игрок защитил другую фигуру своего цвета, засчитываем успех

            return false; //иначе продолжаем урок
        }

		public Coord GetPlayerCoord() //метод получения координат фигуры игрока
		{
			return PlayerCoord;
		}

		public string GetPlayerPiece() //метод получения типа фигуры игрока
		{
			return PlayerPiece;
		}

		void GenCompilator(int PlayerColor, string[] Rows)
		{
			PositionGenerator PosGen = new PositionGenerator();
            if (PlayerColor == 1) //игрок белый
            {
                PlayerPiece = GenPieceType(true); //генерируем его фигуру
                if (!MovingLesson) Rows = PosGen.GenPieces(Horizontal, Rows, false); //генерируем целевые фигуры
                Rows = PosGen.GenPlayerPiece(Rows, true); //выставляем фигуру игрока на доску
            }

            else //игрок чёрный
            {
                PlayerPiece = GenPieceType(true);
                if (!MovingLesson) Rows = PosGen.GenPieces(Horizontal, Rows, true);
                Rows = PosGen.GenPlayerPiece(Rows, false);
            }
        }

		void PositionRegenerator(int Lesson, int PlayerColor, string[] Rows)
		{
            PositionGenerator PosGen = new PositionGenerator();
            for (int i = 0; i < 100; i++)
            {
				for (int j = 0; j < Rows.Length; j++) Rows[j] = "8";
                PosGen.GenCompilator(PlayerColor, Rows);
				if (PositionGenerator.CheckPlayerPosition(Lesson) == false) break;
            }
        }

		public string GeneratePosition(int PlayerColor, int Lesson) //метод сбора строки FEN
		{
            BindingLesson = false; //инициализация типов урока
            MovingLesson = false;
            string ResultFEN = ""; //инициализация строки FEN
			PositionGenerator PosGen = new PositionGenerator(); //запускаем генератор позиции
			int rand = Random.Range(3, 4); //определяем горизонтальное или вертикальное положение целевых фигур
			if (rand == 3) Horizontal = true;
			else Horizontal = false;

			switch (Lesson) //устанавливаем текущий урок согласно полученным от игрового менеджера данным
			{
                case 1: //урок передвижения
                    MovingLesson = true;
					break;
				case 2: //связка
					BindingLesson = true;
					break;
				case 3: //двойной удар
					break;
				default: //ошибка генерации урока
					Debug.Log("Error, cannot generate lesson!");
					break;
            }
				
            string[] Rows = { "8", "8", "8", "8", "8", "8", "8", "8" }; //заполняем строку пустыми клетками

			PosGen.GenCompilator(PlayerColor, Rows); //генерируем фигуры

			if (PositionGenerator.CheckPlayerPosition(Lesson)) PositionRegenerator(Lesson, PlayerColor, Rows); //перегенерируем фигуры, если они появились так, что задание уже выполнено

            for (int i = 0; i < 8; i++) //формирование массива клеток в строке
			{
				if (i != 7)
				{
                    ResultFEN += Rows[i] + "/";
				}
				else
				{
					ResultFEN += Rows[i];
				}
			}
			if (PlayerColor == 1) ResultFEN = ResultFEN + " w KQkq - 0 1"; //добавление окончания строки для белого игрока
			else ResultFEN = ResultFEN + " b KQkq - 0 1"; //для чёрного

            return ResultFEN; //возвращаем получившуюся строку

		}
	}

}