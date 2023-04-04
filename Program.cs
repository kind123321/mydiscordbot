using System;

namespace HelloWorld
{
    class Program
    {
        public static int GenerateRandomNumber()
        {
            Random random = new Random();
            return random.Next(1, 6);
        }

        public static bool PlayAgain()
        {
            Console.WriteLine("\nХотите сыграть еще раз? Напишите \"да\", чтобы сыграть еще раз, и \"нет\", чтобы закрыть программу.");

            string playAgainInput = Console.ReadLine();

            if (playAgainInput.ToLower() == "да")
            {
                return true;
            }
            else
            {
                Console.WriteLine("\nСпасибо за игру! До свидания!");
                return false;
            }
        }

        static void Main(string[] args)
        {
            bool playAgain = true;

            while (playAgain)
            {
                int number = 0, myNum, attempts = 2;
                Console.WriteLine("Я тинни, я люблю загадывать числа. Попробуйте угадать! (Я загадал число от 1 до 5)\nВведите свое число:");

                myNum = GenerateRandomNumber();

                while (attempts > 0)
                {
                    string input = Console.ReadLine();

                    if (!int.TryParse(input, out number))
                    {
                        Console.WriteLine("Неверный ввод! Введите число от 1 до 5:");
                        continue;
                    }

                    if (myNum == number)
                    {
                        Console.WriteLine("Вы угадали! Мое число: " + myNum);
                        Console.WriteLine("\nДавайте реванш!\n");
                        break;
                    }
                    else
                    {
                        Console.WriteLine("Вы не угадали! Осталось попыток: " + --attempts);
                    }
                }

                if (attempts == 0)
                {
                    Console.WriteLine("Вы проиграли! Мое число: " + myNum);
                }

                playAgain = PlayAgain();
            }
        }
    }

}
