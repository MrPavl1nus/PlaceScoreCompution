namespace PersonPlacementSystem.Animation;

public static class ConsoleLineAnimation
{
    public enum AnimationType
    {
        Dots,
        Propeller
    }
    public static void PlayAnimation(string message, AnimationType type, int iterationsCount = 1, int frameMsecDuration = 500)
    {
        if (iterationsCount <= 0)
        {
            return;
        }

        if (frameMsecDuration <= 0)
        {
            return;
        }
        switch (type)
        {
            //Можно было сделать с помощью цикла, но пофиг
            case AnimationType.Dots:

                for (int i = 0; i < iterationsCount; i++)
                {
                    Console.Write($"\r {message}   ");
                    Thread.Sleep(frameMsecDuration);

                    Console.Write($"\r {message}.  ");
                    Thread.Sleep(frameMsecDuration);

                    Console.Write($"\r {message}.. ");
                    Thread.Sleep(frameMsecDuration);

                    Console.Write($"\r {message}...");
                    Thread.Sleep(frameMsecDuration);
                    Console.Write($"\r ");
                }
                Console.WriteLine();
                break;

            case AnimationType.Propeller:
                for (int i = 0; i < iterationsCount; i++)
                {
                    Console.Write($"\r {message}/");
                    Thread.Sleep(frameMsecDuration);

                    Console.Write($"\r {message}-");
                    Thread.Sleep(frameMsecDuration);

                    Console.Write($"\r {message}|");
                    Thread.Sleep(frameMsecDuration);

                    Console.Write($"\r {message}/");
                    Thread.Sleep(frameMsecDuration);
                    Console.Write($"\r ");
                }
                Console.WriteLine();
                break;
        }

    }


}