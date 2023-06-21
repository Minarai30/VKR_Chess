using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Chess.Game
{
    public class Player
    {
        public event System.Action<Move> onMoveChosen; //системное действие хода

        BoardUI boardUI; //инициализация интерфейса
        Camera cam; //камера
        Coord selectedPieceSquare; //координаты выбранной клетки
        Board board; //инициализация доски

        public enum InputState //состояния действий игрока
        {
            None, //нет
            PieceSelected, //выбор фигуры
            DraggingPiece //перетаскивание фигуры
        }

        InputState currentState; //текущее состояние

        public Player(Board board) //конструктор игрока
        {
            boardUI = GameObject.FindObjectOfType<BoardUI>(); //загружаем интерфейс
            cam = Camera.main; //назначаем камеру
            this.board = board; //получаем данные о доске
        }

        public void Update()
        {
            HandleInput(); //постоянно обрабатываем ввод игрока
        }

        void HandleInput() //обработка ввода игрока
        {
            Vector2 mousePos = cam.ScreenToWorldPoint(Input.mousePosition); //определяем положение курсора

            if (currentState == InputState.None) //если игрок ничего не делает
            {
                HandlePieceSelection(mousePos); //запускаем обработку выбора фигуры
            }
            else if (currentState == InputState.DraggingPiece) //если перетаскивает фигуру
            {
                HandleDragMovement(mousePos); //обрабатываем это перетаскивание
            }
            else if (currentState == InputState.PieceSelected) //если выбрал фигуру
            {
                HandlePointAndClickMovement(mousePos); //обрабатываем передвижение при помощи клика
            }

            if (Input.GetMouseButtonDown(1)) //если ткнул на пустую клетку при выбранной фигуре
            {
                CancelPieceSelection(); //сбрасываем выбор фигуры
            }
        }

        void HandlePointAndClickMovement(Vector2 mousePos) //обработка передвижения кликом
        {
            if (Input.GetMouseButton(0)) //игрок ткнул на другую клетку
            {
                HandlePiecePlacement(mousePos); //перемещаем фигуру туда
            }
        }

        void HandleDragMovement(Vector2 mousePos) //обработка перетаскивания
        {
            boardUI.DragPiece(selectedPieceSquare, mousePos); //запускаем анимацию перетаскивания
            if (Input.GetMouseButtonUp(0)) //если игрок отпустил ЛКМ
            {
                HandlePiecePlacement(mousePos); //пытаемся поставить фигуру на это место
            }
        }

        void HandlePiecePlacement(Vector2 mousePos) //установка фигур
        {
            Coord targetSquare; //получаем координаты целевой клетки
            if (boardUI.TryGetSquareUnderMouse(mousePos, out targetSquare)) //пробуем поставить фигуру
            {
                if (targetSquare.Equals(selectedPieceSquare)) //если целевая клетка и начальная клетка совпадают
                {
                    boardUI.ResetPiecePosition(selectedPieceSquare); //сбрасываем положение фигуры
                    if (currentState == InputState.DraggingPiece) //переводим фигуру в режим перемещения кликом
                    {
                        currentState = InputState.PieceSelected;
                    }
                    else //если этот режим и так стоял
                    {
                        currentState = InputState.None; //переводим в режим выбора фигуры
                        boardUI.DeselectSquare(selectedPieceSquare); //перестаём считать фигуру выбранной
                    }
                }
                else
                {
                    int targetIndex = BoardRepresentation.IndexFromCoord(targetSquare.fileIndex, targetSquare.rankIndex); //если не совпадают
                    if (Piece.IsColour(board.Square[targetIndex], board.ColourToMove) && board.Square[targetIndex] != 0) //если в целевой клетке находится дружественная фигура
                    {
                        CancelPieceSelection(); //возвращаем фигуру на исходную позицию
                        HandlePieceSelection(mousePos);
                    }
                    else //если никого нет или стоит вражеская фигура
                    {
                        TryMakeMove(selectedPieceSquare, targetSquare); //ходим
                    }
                }
            }
            else //если фигуру попытались поставить не на доску
            {
                CancelPieceSelection(); //возвращаем её на место
            }

        }

        void CancelPieceSelection() //возвращение фигуры
        {
            if (currentState != InputState.None) //если была попытка совершить ход
            {
                currentState = InputState.None; //отменяем передвижение
                boardUI.DeselectSquare(selectedPieceSquare); //снимаем выделение с клетки
                boardUI.ResetPiecePosition(selectedPieceSquare); //и фигуры
            }
        }

        void TryMakeMove(Coord startSquare, Coord targetSquare) //совершение хода
        {
            int startIndex = BoardRepresentation.IndexFromCoord(startSquare); //индекс начальной клетки
            int targetIndex = BoardRepresentation.IndexFromCoord(targetSquare); //конечной
            bool moveIsLegal = false; //флаг корректности хода
            Move chosenMove = new Move(); //инициализируем ход

            MoveGenerator moveGenerator = new MoveGenerator(); //запускаем генератор ходов
            bool wantsKnightPromotion = Input.GetKey(KeyCode.LeftAlt); //если хотим повышения в коня, то при ходе зажимаем левый Alt

            var legalMoves = moveGenerator.GenerateMoves(board); //генерируем корректные ходы

            for (int i = 0; i < legalMoves.Count; i++) //и идём по их списку
            {
                var legalMove = legalMoves[i]; //выбираем конкретный ход

                if (legalMove.StartSquare == startIndex && legalMove.TargetSquare == targetIndex) //если ход игрока является корректным
                {
                    if (legalMove.IsPromotion) //проверяем на повышение
                    {
                        if (legalMove.MoveFlag == Move.Flag.PromoteToQueen && wantsKnightPromotion) //если оно есть и не хотим коня
                        {
                            continue;
                        }
                        if (legalMove.MoveFlag != Move.Flag.PromoteToQueen && !wantsKnightPromotion) //если нет повышения
                        {
                            continue;
                        }
                    }
                    moveIsLegal = true; //говорим, что ход корректный
                    chosenMove = legalMove;
                    break; //прерываем проверку ходов
                }
            }

            if (moveIsLegal) //если ход корректный
            {
                ChoseMove(chosenMove); //совершаем ход
                currentState = InputState.None;
            }
            else //если нет
            {
                CancelPieceSelection(); //то отменяем его
            }
        }

        void HandlePieceSelection(Vector2 mousePos) //выбор фигуры
        {
            if (Input.GetMouseButtonDown(0)) //игрок сделал тык
            {
                if (boardUI.TryGetSquareUnderMouse(mousePos, out selectedPieceSquare)) //проверяем фигуру под тыком
                {
                    int index = BoardRepresentation.IndexFromCoord(selectedPieceSquare); //забираем индекс фигуры
                    if (Piece.IsColour(board.Square[index], board.ColourToMove)) //если в клетке есть фигура
                    {
                        boardUI.HighlightLegalMoves(board, selectedPieceSquare); //подсвечиваем правильные ходы
                        boardUI.SelectSquare(selectedPieceSquare); //выбираем клетку
                        currentState = InputState.DraggingPiece; //начинаем перетаскивание фигуры
                    }
                }
            }
        }

        protected void ChoseMove(Move move) //совершение хода
        {
            onMoveChosen?.Invoke(move); //через системное действие
        }

        public void NotifyTurnToMove() //метод для перевода хода на игрока
        {

        }
    }
}