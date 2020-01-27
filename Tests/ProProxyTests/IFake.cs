namespace ProProxyTests
{
    public interface IFake
    {
        void Speak(string words);
        string Listen(string words);
        int Calculate(int a, int b, int c);
    }
}