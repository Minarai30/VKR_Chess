using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Chess.Game
{
    public class TestGenerator
    {
        List<string> Test; //инициализируем список вопросов теста

        List<int> GenQuestionOrder() //метод для формирования варианта теста
        {
            int TestVariant = Random.Range(1, 6); //предусмотрено 6 вариантов сочетания всех вопросов
            List<int> QuestionOrder = new List<int>(); //список с порядком вопросов в тесте

            switch (TestVariant) //формируем порядок вопросов согласно варианту
            {
                case 1:
                    QuestionOrder.Add(1);
                    QuestionOrder.Add(2);
                    QuestionOrder.Add(3);
                    break;
                case 2:
                    QuestionOrder.Add(1);
                    QuestionOrder.Add(3);
                    QuestionOrder.Add(2);
                    break;
                case 3:
                    QuestionOrder.Add(2);
                    QuestionOrder.Add(1);
                    QuestionOrder.Add(3);
                    break;
                case 4:
                    QuestionOrder.Add(2);
                    QuestionOrder.Add(3);
                    QuestionOrder.Add(1);
                    break;
                case 5:
                    QuestionOrder.Add(3);
                    QuestionOrder.Add(1);
                    QuestionOrder.Add(2);
                    break;
                case 6:
                    QuestionOrder.Add(3);
                    QuestionOrder.Add(2);
                    QuestionOrder.Add(1);
                    break;
            }

            return QuestionOrder; //возвращаем порядок
        }

        public List<string> GenTest(int TestIndex) //генерация строк с вопросами
        {
            Test = new List<string>(); //инициализируем сырой тест
            List<int> TestOrder = GenQuestionOrder(); //получаем вариант

            switch (TestIndex) //генерируем вопросы в зависимости от выбранного игроком теста
            {
                case 1:
                    for (int i = 0; i <= 2; i++)
                    {
                        Test.Add(GenTest1Position(TestOrder[i]));
                    }

                    return Test;
                case 2:
                    for (int i = 0; i <= 2; i++)
                    {
                        Test.Add(GenTest2Position(TestOrder[i]));
                    }

                    return Test;
                case 3:
                    for (int i = 0; i <= 2; i++)
                    {
                        Test.Add(GenTest3Position(TestOrder[i]));
                    }

                    return Test;
                case 4:
                    for (int i = 0; i <= 2; i++)
                    {
                        Test.Add(GenTest4Position(TestOrder[i]));
                    }

                    return Test;
                case 5:
                    for (int i = 0; i <= 2; i++)
                    {
                        Test.Add(GenTest5Position(TestOrder[i]));
                    }

                    return Test;

            }
            for (int i = 0; i <= 2; i++) //если по какой-то причине не получилось сгенерировать тест, генерируем первый
            {
                Test.Add(GenTest1Position(TestOrder[i]));
            }

            return Test;
        }

        string GenTest1Position(int Question) //генерация первого теста
        {
            switch (Question)
            {
                case 1:
                    return "8/8/8/8/4r3/8/8/Q7 w KQkq - 0 1|Выбери правильные части строки FEN для этих фигур.|3r4, q7|4r3, Q7|3R4, Q7|4r3, q7|2";
                case 2:
                    return "8/8/8/8/b/8/8/8 w KQkq - 0 1|Какая фигура стоит в А4?|Ферзь|Король|Пешка|Слон|4";
                case 3:
                    return "8/8/8/PpPpPpPp/8/8/8/8 w KQkq - 0 1|Выбери правильную нотацию для 5 строки.|pPpPpPpP|PpPpPpPp|НЕТ|НЕТ|2";
                default:
                    Debug.Log("Unable to generate test position!");
                    break;
            }
            return "8/8/8/8/4r3/8/8/Q7 w KQkq - 0 1|Выбери правильные части строки FEN для этих фигур.|3r4, q7|4r3, Q7|3R4, Q7|4r3, q7|2"; //возвращаем первый вопрос в случае ошибки
        }

        string GenTest2Position(int Question) //второго
        {
            switch (Question)
            {
                case 1:
                    return "8/r3P3/3K3r/1p6/5q2/8/8/8 w KQkq - 0 1|Может ли тут король убежать от мата?|Да|Нет|НЕТ|НЕТ|1";
                case 2:
                    return "8/8/8/8/7q/5nP1/6R1/5B1K w KQkq - 0 1|Мне кажется, что здесь нет мата белым. Это так?|Да|Нет|НЕТ|НЕТ|2";
                case 3:
                    return "r1n5/8/8/K1q5/8/8/8/8 w KQkq - 0 1|Может ли король тут сходить так, чтобы не получить мат?|Да|Нет|НЕТ|НЕТ|1";
                default:
                    Debug.Log("Unable to generate test position!");
                    break;
            }
            return "8/r3P3/3K3r/1p6/5q2/8/8/8 w KQkq - 0 1|Может ли тут король убежать от мата?|Да|Нет|НЕТ|НЕТ|1";
        }

        string GenTest3Position(int Question) //третьего
        {
            switch (Question)
            {
                case 1:
                    return "8/1kp4p/1pp5/3p1R2/1P6/QPKPP3/2P3q1/5b1r w KQkq - 0 1|Какими фигурами белые могут поставить здесь мат в два хода?|Ладья и ферзь|Пешка и ферзь|НЕТ|НЕТ|1";
                case 2:
                    return "1k6/1p6/p5p1/5p2/1PQ2Pn1/8/P1P3P1/4qB1K w KQkq - 0 1|Какие два хода ты бы сделал, чтобы поставить мат белым?|Фe1-e2, Фe2-e1|Фe1-h4, Фh4-h3|Кg4-f2, Фе1-h4|Фе1-f2, Фf2xg2|2";
                case 3:
                    return "1k6/p4p2/6b1/Pp2p1p1/PQ1N4/2P1q3/1P6/3K4 w KQkq - 0 1|Чёрные сделали ход Фf3-e3, а белые Кре1-d1. Как чёрным поставить ему мат?|Фe3-d2|Сg6-h5|НЕТ|НЕТ|2";
                default:
                    Debug.Log("Unable to generate test position!");
                    break;
            }
            return "8/1kp4p/1pp5/3p1R2/1P6/QPKPP3/2P3q1/5b1r w KQkq - 0 1|Какими фигурами белые могут поставить здесь мат в два хода?|Ладья и ферзь|Пешка и ферзь|НЕТ|НЕТ|1";
        }

        string GenTest4Position(int Question) //четвёртого
        {
            switch (Question)
            {
                case 1:
                    return "8/k4p/8/7K/8/8/8/8 w KQkq - 0 1|Есть ли здесь ничья?|Да|Нет|НЕТ|НЕТ|2";
                case 2:
                    return "8/8/8/2KB4/8/3k5/8/8 w KQkq - 0 1|По-моему, тут можно победить. Как думаешь?|Можно|Нельзя|НЕТ|НЕТ|1";
                case 3:
                    return "8/8/2PK4/1PKP4/6B1/2P5/5B2/8 w KQkq - 0 1|Считаешь ли ты, что здесь патовая ситуация?|Да|Нет|НЕТ|НЕТ|2";
                default:
                    Debug.Log("Unable to generate test position!");
                    break;
            }
            return "8/k4p/8/7K/8/8/8/8 w KQkq - 0 1|Есть ли здесь ничья?|Да|Нет|НЕТ|НЕТ|2";
        }

        string GenTest5Position(int Question) //пятого
        {
            switch (Question)
            {
                case 1:
                    return "6k1/3q2p1/p2bp2p/3p1r2/1p1Pp3/3bQ1PP/PP1B1rB1/1N2R1RK w KQkq - 0 1|Как думаешь, является ли эта ситуация цугцвангом|Да|Нет|НЕТ|НЕТ|1";
                case 2:
                    return "8/8/8/3p5/2kPK3/8/8/8 w KQkq - 0 1|Эта ситуация цугцванг? Ходят белые.|Да|Нет|НЕТ|НЕТ|1";
                case 3:
                    return "5NbK/5p1P/8/7k/6p1/8/8/8 w KQkq - 0 1|Можно ли тут сделать ход, не потеряв фигуру?|Да|Нет|НЕТ|НЕТ|2";
                default:
                    Debug.Log("Unable to generate test position!");
                    break;
            }
            return "8/8/8/3p5/2kPK3/8/8/8 w KQkq - 0 1|Эта ситуация цугцванг? Ходят белые.|Да|Нет|НЕТ|НЕТ|1";
        }
    }
}

