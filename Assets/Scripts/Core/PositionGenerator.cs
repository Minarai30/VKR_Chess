using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net;
using UnityEngine;

namespace Chess.Game
{
	public class PositionGenerator
	{
		static string PlayerPiece; //���������� ��� �������� ������, ������� ����� ����������� �����
		static Coord PlayerCoord; //���������� ������ ������

		bool LightSquare; //���� ������
		bool Horizontal; //����� ��������� ����� - �������������� ��� ������������

		static int AnotherPieceFile; //��������������� ���������� �� �������� ������� ������ ��� ��������� ����� �����

		static bool BindingLesson; //��������� ����� "������"
		static bool MovingLesson; //��������� ����� "������������ �����"

		static string GenPieceType(bool White) //��������� �����
		{
			int randPiece = Random.Range(0, 5);

			switch (randPiece)
            {
                case 0: //�����
					if (!White) return "p";
					else return "P";
				case 1: //�����
					if (!White) return "r";
					else return "R";
				case 2: //����
					if (!White) return "n";
					else return "N";
				case 3: //����
					if (!White) return "b";
					else return "B";
				case 4: //�����
					if (!White) return "q";
					else return "Q";
				default: //������
					if (!White) return "k";
					else return "K";
			}
		}

		string[] GenPieces(bool Position, string[] Rows, bool White) //��������� ������� �����
        {
			int Rank = Random.Range(0, 7); //������
			int File = Random.Range(0, 6); //�������
			int Interval = Random.Range(2, 6 - File); //�������� ����� ��������

			int RandomVariant;

			if (PlayerPiece == "N") //��������� ��������� ��� ����, ����� �� ��� ��������� ��� ������
			{
				RandomVariant = Random.Range(1, 3);

				if (RandomVariant == 1) Interval = 1;
				else Interval = 3;

				if (!Position) Interval++;
			}

			if (PlayerPiece == "P" || PlayerPiece == "p") Rank = Random.Range(0, 3); //����������� ��������� ���������� �� ������ ������� ���� �� �����

			if (BindingLesson) //��������� ������ ��� ����� ������
            {
				if (Rank == 7) Rows[7] = File + GenPieceType(!White);
				else Rows[Rank] = File + GenPieceType(!White);
			}

			else //��������� ��� �������� �����
            {
                if (!Position && PlayerPiece != "P") //���� ������� ������������ � ������ ������ �� �����
                {
                    if (PlayerPiece != "N") Interval = Random.Range(2, 6 - Rank); //���� � ������ �� ����, �� ���������� ��������

					if (PlayerPiece == "B") //���� � ������ ����
					{
						Interval = 2 * Random.Range(1, 3); //������������� ��������

                        if (File + Interval > 7) //���������, ����� ������ �� ���� �� ������� �����
						{
                            File = 1;
							Interval = 4;
						}
					}
                  
					if (Rank >= 7) //���� ������ ������ ����� �� ���� �����
                    {
						Rows[7] = "K" + File + GenPieceType(White);
                        Rows[5] = File + GenPieceType(White); //������ ������ �� ��� ������ ����
                    }

                    else if (Rank + Interval >= 7) //�������� ������� �����
                    {
                        Rows[5] = File + GenPieceType(White);
                        Rows[7] = File + GenPieceType(White);
                    }

                    else //����������� ��������� ��� ������ �������
                    {
						Rows[Rank] = File + GenPieceType(White);
						Rows[Rank + Interval] = File + GenPieceType(White);
                    }

                    LightSquare = BoardRepresentation.LightSquare(File, Rank); //���������� ���� ������

                }

                else //���� ������� �������������� ��� � ������ �����
				{
					if (PlayerPiece == "P") //������������� ������ ��� �����
					{
						if (White) Rank = 7 - Rank; //����������� ��������� �� �������� ��� �����
						Interval = 1;
					}

                    if (PlayerPiece == "B") //���� � ������ ����, ��������� ���������� ������ � ������������ ��������
                    {
                        Interval = 2 * Random.Range(1, 3) - 1;
						Debug.Log(Interval);

                        if (File + Interval > 7)
                        {
                            File = 1;
                            Interval = 3;
                        }
                    }

					Rows[Rank] = File + GenPieceType(White) + Interval + GenPieceType(White); //���������� ������

                    AnotherPieceFile = File; //���������� ������� ����� ������ ��� �����

                    LightSquare = BoardRepresentation.LightSquare(File, Rank);
                }
			}

			return Rows;
		}

		string[] GenPlayerPiece(string[] Rows, bool White) //��������� ��������� ����� ������
        {
			int Rank = Random.Range(0, 7);
			int File = Random.Range(0, 7);

			if (PlayerPiece == "P") //���� ���� ������������� �����
			{
				if (!MovingLesson) File = AnotherPieceFile + 1; //���� ��� �� ���� ������������, ������ ����� � ������� �� ������� ������ �� �������

				Rank = Random.Range(5, 7); //���������� ����� ����� ����� ����
				if (!White) Rank = Random.Range(0, 2); //� ������ ������
				if (Rank < 0) Rank = 0;
			}

			if (PlayerPiece == "P") //������������, ����� ����� �� ���������� �� ��������� �������
            {
                if (Rank == 7 && !White) Rank--;
                if (Rank == 0 && White) Rank++;
            }
                

			if (PlayerPiece == "B") //�������� ������� �� ������� � ������ ������������ ������ ������
			{
				if (LightSquare != BoardRepresentation.LightSquare(File, Rank)) File++;
			}

            PlayerCoord = new Coord(File, Rank); //���������� ��������� ������ ������

            if (!White) PlayerPiece = PlayerPiece.ToLower(); //������� ������� ��� ����������� ������ ������ � ������ ������, ���� ��� ���������

            if (Rows[Rank] == "8") Rows[Rank] = File + PlayerPiece; //��������� ������ ������ � ������
			else Rows[Rank + 1] = File + PlayerPiece; //���� �������� �� ������ ����

			return Rows;
		}


		public static bool CheckPlayerPosition(int Lesson) //�������� ������� ������ ��� ������ "������� ����" � "������"
        {
            MoveGenerator moveGenerator = new MoveGenerator(); //��������� ��������� �����

			if (moveGenerator.LessonChecker(Lesson) && !BindingLesson && !MovingLesson) return true; //���� ����� �������� ��� ���� � ������� ���� "������� ����", ����������� �������� ����������
			if (moveGenerator.LessonChecker(Lesson) && BindingLesson) return true; //���� ����� ������� ������ ������ ������ �����, ����������� �����

            return false; //����� ���������� ����
        }

		public Coord GetPlayerCoord() //����� ��������� ��������� ������ ������
		{
			return PlayerCoord;
		}

		public string GetPlayerPiece() //����� ��������� ���� ������ ������
		{
			return PlayerPiece;
		}

		void GenCompilator(int PlayerColor, string[] Rows)
		{
			PositionGenerator PosGen = new PositionGenerator();
            if (PlayerColor == 1) //����� �����
            {
                PlayerPiece = GenPieceType(true); //���������� ��� ������
                if (!MovingLesson) Rows = PosGen.GenPieces(Horizontal, Rows, false); //���������� ������� ������
                Rows = PosGen.GenPlayerPiece(Rows, true); //���������� ������ ������ �� �����
            }

            else //����� ������
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

		public string GeneratePosition(int PlayerColor, int Lesson) //����� ����� ������ FEN
		{
            BindingLesson = false; //������������� ����� �����
            MovingLesson = false;
            string ResultFEN = ""; //������������� ������ FEN
			PositionGenerator PosGen = new PositionGenerator(); //��������� ��������� �������
			int rand = Random.Range(3, 4); //���������� �������������� ��� ������������ ��������� ������� �����
			if (rand == 3) Horizontal = true;
			else Horizontal = false;

			switch (Lesson) //������������� ������� ���� �������� ���������� �� �������� ��������� ������
			{
                case 1: //���� ������������
                    MovingLesson = true;
					break;
				case 2: //������
					BindingLesson = true;
					break;
				case 3: //������� ����
					break;
				default: //������ ��������� �����
					Debug.Log("Error, cannot generate lesson!");
					break;
            }
				
            string[] Rows = { "8", "8", "8", "8", "8", "8", "8", "8" }; //��������� ������ ������� ��������

			PosGen.GenCompilator(PlayerColor, Rows); //���������� ������

			if (PositionGenerator.CheckPlayerPosition(Lesson)) PositionRegenerator(Lesson, PlayerColor, Rows); //�������������� ������, ���� ��� ��������� ���, ��� ������� ��� ���������

            for (int i = 0; i < 8; i++) //������������ ������� ������ � ������
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
			if (PlayerColor == 1) ResultFEN = ResultFEN + " w KQkq - 0 1"; //���������� ��������� ������ ��� ������ ������
			else ResultFEN = ResultFEN + " b KQkq - 0 1"; //��� �������

            return ResultFEN; //���������� ������������ ������

		}
	}

}