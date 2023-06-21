using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Text.RegularExpressions;
using UnityEngine.UI;
using System.Runtime.InteropServices.WindowsRuntime;
using System;
using System.Diagnostics;
using TMPro;

namespace Chess.Game {
	public class GameManager : MonoBehaviour {

		public event System.Action onPositionLoaded; //системное действие загрузки позиции
		public event System.Action<Move> onMoveMade; //системное действие хода

		public string LessonPosition; //строка FEN урока

        int PlayerColor; //цвет игрока
        //public Color[] colors; //цвета фигур

        public TMPro.TMP_Text TaskJournalUI; //журнал выполненных задач
        public TMPro.TMP_Text CompletedTasksUI; //журнал выполненных уроков
		static TMPro.TMP_Text[] Texts; //текстовые блоки интерфейса тестов
        static Toggle[] AnswersToggle; //кнопки ответов тестов

        public Button Test1; //кнопки запуска тестов
        public Button Test2;
        public Button Test3;
        public Button Test4;
        public Button Test5;

		public Image TestPanel; //диалоговое окно тестов

        public Toggle SequentalToggle;

		string CurrentTask; //текстовое обозначение целевой клетки для урока с передвижением
		int CompletedTasks; //количество выполненных задач
		bool FirstLessonComplete; //флаг выполнения задачи урока с передвижением
		int Lesson; //индекс урока

        int Question; //счетчик пройденных вопросов теста
        static int Test; //индекс теста
		static Tuple<string, string, string, string, string, string, string> GeneratedQuestion; //кортеж с вопросом теста
        static List<Tuple<string, string, string, string, string, string, string>> GeneratedTest; //сгенерированный тест
        int CorrectAnswers; //количество правильных ответов на вопросы теста
		bool EnableDialogue; //переключатель режима диалогового окна с вопросов на завершающий экран и наоборот

		Player Player; //экземпляр игрока
		List<Move> gameMoves; //список ходов игры
		BoardUI boardUI; //интерфейс доски
		PositionGenerator PosGen; //генератор строк FEN
		TestManager TestManager; //генератор тестов

		public Board board //получение доски
        { 
			get; 
			private set; 
		} 

		void Start () //при загрузке игры инициализируем доску
		{
			Application.targetFrameRate = 60; //устанавливаем 60 кадров в секунду в качестве максимума

			boardUI = FindObjectOfType<BoardUI> (); //инициализируем интерфейс доски
			gameMoves = new List<Move> (); //создаём список ходов
			board = new Board (); //генерируем образец доски
			PosGen = new PositionGenerator(); //включаем генератор строки FEN
			TestManager = new TestManager(); //и тестов

            TestPanel.gameObject.SetActive(true); //временно включаем диалоговое окно тестов
            Texts = TestPanel.GetComponentsInChildren<TMPro.TMP_Text>(); //забираем текстовые блоки
            AnswersToggle = TestPanel.GetComponentsInChildren<Toggle>(); //и кнопки
            TestPanel.gameObject.SetActive(false); //выключаем окно тестов

            CompletedTasks = 0; //инициализируем переменные уроков
			Lesson = 1;
			Question = 0;
			CorrectAnswers = 0;

			EnableDialogue = true;

			NewGame(Lesson); //запускаем сессии
		}

		void Update () //этот метод срабатывает каждый кадр
		{
			if (Lesson == 4) TestChecker(); //если игрок проходит тест, оцениваем тесты

            Player.Update (); //обеспечиваем возможность игроку постоянно взаимодействовать с доской

			if (Lesson != 4 && gameMoves.Count > 0) TaskCheck(board); //если игрок проходит практические задания, оцениваем их
        }

		void OnMoveChosen (Move move) //метод для обработки сделанного хода
		{
            board.MakeMove(move); //делаем ход
            gameMoves.Add(move); //добавляем этот ход в список ходов
            onMoveMade?.Invoke(move); //запускаем системное действие хода
            boardUI.OnMoveMade(board, move); //делаем ход на интерфейсе доски
			if (FirstLesson.PositionChecker(move) && Lesson == 1) FirstLessonComplete = true; //проверяем, не пришел ли игрок в целевую клетку в уроке передвижения фигур

            PlayerMove(); //перезапускаем управление фигурами для игрока
		}

        void PlayerMove() //обработка хода игрока
        {
            MoveGenerator moveGenerator = new MoveGenerator(); //запускаем генератор ходов
            var moves = moveGenerator.GenerateMoves(board); //генерируем ходы для игрока

            Player.NotifyTurnToMove(); //передаём управление игроку
        }

        void CreatePlayer() //создаём игрока
        {
            Player = new Player(board); //создаём новый образец игрока
            int randColor = UnityEngine.Random.Range(2, 4); //генерируем цвет игрока
            if (randColor == 3) PlayerColor = 1; //белый
            else PlayerColor = 2; //черный

            Player.onMoveChosen += OnMoveChosen; //пропускаем первый ход для корректной инициализации управления
        }

        void NewGame (int Lesson) //новая сессия
		{
            boardUI.SetPerspective(true); //выставляем перспективу доски со стороны белых
            gameMoves.Clear (); //очищаем массив ходов
            if (!SequentalToggle.isOn) CompletedTasksUI.text = ""; //убираем журнал выполненных уроков, если не включено последовательное прохождение
            FirstLessonComplete = false; //индикатор прохождения первого урока

			if (Lesson != 4) LessonPosition = PosGen.GeneratePosition(PlayerColor, Lesson); //генерируем практическое задание, если не были запущены тесты
			else //иначе генерируем тест
			{
				GeneratedQuestion = GeneratedTest[Question]; //забираем кортеж с вопросом
				LessonPosition = GeneratedQuestion.Item1; //забираем строку FEN вопроса
                TextProcessor(); //запускаем обработчик текста теста
			}

			board.LoadPosition(LessonPosition); //загружаем позицию
			onPositionLoaded?.Invoke (); //обновляем доску для соответствия строке FEN
			boardUI.UpdatePosition (board);
			boardUI.ResetSquareColours ();

			CreatePlayer (); //создаем игрока

            if (Lesson == 1) CurrentTask = BoardRepresentation.SquareNameFromCoordinate(FirstLesson.FirstLessonGenerator()); //генерируем целевую клетку для урока с передвижением

			TasksJournal(); //запускаем журнал выполненных задач

            PlayerMove (); //запускаем обработчики ходов игрока

		}

        void TaskCheck(Board board) //проверка задач
        {
            if (FirstLessonComplete || PositionGenerator.CheckPlayerPosition(Lesson)) //если задача была выполнена
            {
                TaskJournalUI.text += " Выполнено!\n"; //добавляем упоминание об этом в журнал
                CompletedTasks++; //увеличиваем количество выполненных задач
                if (CompletedTasks == 3) //если было выполнено 3 задачи
                {
                    if (Lesson <= 3) //и не пройдены все уроки либо не запущены тесты
                    {
                        if (Lesson <= 2 && SequentalToggle.isOn) CompletedTasks = 0; //если урок не последний, обнуляем счетчик выполненных задач
                        TaskJournalUI.text = "Текущая задача:\n"; //перезапускаем журнал задач
                        if (SequentalToggle.isOn) //если включено последовательное прохождение
                        {
                            CompletedTasksUI.text += "Урок " + Lesson + " пройден!\n"; //заносим выполненный урок в журнал выполненных уроков
                            if (Lesson != 3) Lesson++; //меняем урок
                        }
                    }

                }

                NewGame(Lesson); //запускаем новую сессию
            }
        }

        public void TestModeSwitch(bool EnableButtons) //переключение тестового режима
        {
            if (EnableButtons) //очищаем интерфейс от остатков практических заданий
            {
                TaskJournalUI.text = ""; //очищаем журналы
                CompletedTasksUI.text = "";
                string EmptyBoard = "8/8/8/8/8/8/8/8 w KQkq - 0 1"; //FEN-строка пустой доски
                board.LoadPosition(EmptyBoard); //очищаем доску
                onPositionLoaded?.Invoke();
                boardUI.UpdatePosition(board);
                boardUI.ResetSquareColours();
            }

            Test1.gameObject.SetActive(EnableButtons); //активируем кнопки выбора тестов
            Test2.gameObject.SetActive(EnableButtons);
            Test3.gameObject.SetActive(EnableButtons);
            Test4.gameObject.SetActive(EnableButtons);
            Test5.gameObject.SetActive(EnableButtons);

            TestPanel.gameObject.SetActive(EnableButtons); //включаем диалоговое окно тестов
            for (int i = 0; i < 4; i++) //включаем кнопки ответов
            {
                AnswersToggle[i].gameObject.SetActive(EnableDialogue);
            }
            for (int i = 1; i < 5; i++) //выводим текст вопросов
            {
                Texts[i].gameObject.SetActive(EnableDialogue);
            }
            Texts[5].gameObject.SetActive(!EnableDialogue); //переключатель итогового диалога после окончания теста
            Texts[6].gameObject.SetActive(!EnableDialogue);
        }

        void TestRestarter() //перезапуск теста
		{
            GeneratedTest = TestManager.StartTest(Test); //генерируем тест
            EnableDialogue = true; //включаем интерфейс
			TestModeSwitch(true);
			Question = 0; //обнуляем счетчик вопросов
			CorrectAnswers = 0; //и правильных ответов
		}

		void TextProcessor() //вставка вопросов теста в диалоговое окно тестов
		{
			Texts[0].text = GeneratedQuestion.Item2; //сам вопрос
			Texts[1].text = GeneratedQuestion.Item3; //первый ответ
			Texts[2].text = GeneratedQuestion.Item4; //второй
			Texts[3].text = GeneratedQuestion.Item5; //третий
			Texts[4].text = GeneratedQuestion.Item6; //четвертый
		}

		void TestChecker() //проверка тестов
		{
			for (int i = 0; i < 4; i++) //проходим по кнопкам ответа
			{
				if (AnswersToggle[i].isOn) //если одну нажали
				{
					
					if (Question <= 2) Question++; //меняем вопрос
					if (i + 1 == int.Parse(GeneratedQuestion.Item7)) //проверяем, правильный ли ответ
					{
                        CorrectAnswers++;
					}

                    AnswersToggle[0].isOn = false; //переводим все кнопки в неактивное состояние
                    AnswersToggle[1].isOn = false;
                    AnswersToggle[2].isOn = false;
                    AnswersToggle[3].isOn = false;

                    if (Question < 3) NewGame(Lesson); //если вопрос не последний, меняем положение на поле
                }
			}
			if (Question == 3) //если вопрос последний
			{
				EnableDialogue = false; //меняем диалоговое окно на окно завершения теста
				TestModeSwitch(true); //перезагружаем интерфейс тестов

				Texts[0].text = "Ты закончил тест! Давай посмотрим, насколько хорошо у тебя получилось:"; //сообщаем об окончании теста

				switch (CorrectAnswers) //в зависимости от правильных ответов ставим оценку
				{
					case 2:
                        Texts[5].text = "ХОРОШО";
						Texts[6].text = "Ты хорошо постарался!";
						break;
					case 3:
                        Texts[5].text = "ОТЛИЧНО";
						Texts[6].text = "Настоящий молодец!";
                        break;
					default:
                        Texts[5].text = "ПЛОХО";
                        Texts[6].text = "Попробуй ещё раз!";
                        break;
                }
            }
		}

        void TasksJournal() //журнал выполненных заданий
        {
            string text;
            switch (Lesson) //меняем его в зависимости от текущего урока
            {
                case 1:
                    text = "Перемести " + PlayerPieceLocalization() + " в клетку " + CurrentTask + "\n";
                    break;
                case 2:
                    text = "Защити одну фигуру при помощи другой!\n";
                    break;
                case 3:
                    text = "Напади " + PlayerColorLocalization(1) + " " + PlayerColorLocalization(2) + " " + PlayerPieceLocalization() + " на фигуры врага.\n";
                    break;
                case 4:
                    text = "";
                    break;
                default:
                    text = "Ой, что-то пошло не так...\n";
                    break;
            }
            if (CompletedTasks >= 3) TaskJournalUI.text = "Ты сделал урок, но можешь продолжать, если хочешь! Если интересно, ты выполнил задачу уже " + CompletedTasks + " раз.\n";
            TaskJournalUI.text += text;
        }

        string PlayerColorLocalization(int initializer) //локализация цветов и местоимений
        {
            if (initializer == 1) //местоимения
            {
                if (PosGen.GetPlayerPiece().ToUpper() == "P" || PosGen.GetPlayerPiece().ToUpper() == "R") return "своей";
                else return "своим";
            }

            if (initializer == 2) //цвета
            {
                if (PlayerColor == 1)
                {
                    if (PosGen.GetPlayerPiece().ToUpper() == "P" || PosGen.GetPlayerPiece().ToUpper() == "R") return "белой";
                    else return "белым";
                }

                else
                {
                    if (PosGen.GetPlayerPiece().ToUpper() == "P" || PosGen.GetPlayerPiece().ToUpper() == "R") return "черной";
                    else return "черным";
                }
            }
            return "Ой, ошибка!";
        }

        string PlayerPieceLocalization() //генерация локали типов фигур
        {
            switch (PosGen.GetPlayerPiece().ToUpper())
            {
                case "P":
                    if (Lesson == 1) return "пешку";
                    else return "пешкой";
                case "B":
                    if (Lesson == 1) return "офицера";
                    else return "офицером";
                case "R":
                    if (Lesson == 1) return "ладью";
                    else return "ладьей";
                case "N":
                    if (Lesson == 1) return "коня";
                    else return "конем";
                case "Q":
                    if (Lesson == 1) return "ферзя";
                    else return "ферзем";
                case "K":
                    if (Lesson == 1) return "короля";
                    else return "королем";
                default: return "(фигура не обнаружена)";
            }
        }

        //кнопки

        //практические задания
        public void NewMoving() //урок перемещения
        {
            Lesson = 1;
            TaskJournalUI.text = "Текущая задача:\n";
            TestModeSwitch(false);
            CompletedTasks = 0;
            NewGame(Lesson);
        }

        public void NewBinding() //урок связки
        {
            Lesson = 2;
            TaskJournalUI.text = "Текущая задача:\n";
            TestModeSwitch(false);
            CompletedTasks = 0;
            NewGame(Lesson);
        }

        public void NewDoubleStrike() //двойной удар
        {
            Lesson = 3;
            TaskJournalUI.text = "Текущая задача:\n";
            TestModeSwitch(false);
            CompletedTasks = 0;
            NewGame(Lesson);
        }

        //тесты

        public void NewTest() //определи фигуру
        {
            Lesson = 4;
            Test = 1;
            TestRestarter();

            NewGame(Lesson);
        }

        public void NewTest2() //мат королю
        {
            Lesson = 4;
            Test = 2;
            TestRestarter();

            NewGame(Lesson);
        }

        public void NewTest3() //мат в два хода
        {
            Lesson = 4;
            Test = 3;
            TestRestarter();

            NewGame(Lesson);
        }

        public void NewTest4() //ничейные ситуации
        {
            Lesson = 4;
            Test = 4;
            TestRestarter();

            NewGame(Lesson);
        }

        public void NewTest5() //цугцванг
        {
            Lesson = 4;
            Test = 5;
            TestRestarter();

            NewGame(Lesson);
        }

        public void StartNeuro() //интеграция с нейросетью
        {
            Process.Start(Environment.CurrentDirectory + @"\neuro\GUI\Arena.exe"); //запускаем графический интерфейс Arena
        }

        public void QuitGame() //выход из игры
        {
            Application.Quit();
        }
    }
}