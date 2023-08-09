public class Program
{
    static void Main(string[] args)
    {
        var height = 16;
        var star = "*";
        var starDecoration = "o*";
        var treeIntent = height - 2;
        var stemIntent = height - 3;
        var space = " ";
        for (int i = 0; i < height; i++)
        {
            for (int j = treeIntent; 0 <= j; j--)
            {
                Console.Write(space);
            }
            treeIntent--;
            Console.WriteLine(star);
            star += starDecoration;
        }
        for (int i = stemIntent; 0 <= i; i--)
        {
            Console.Write(space);
        }
        Console.WriteLine("| |");
    }
}