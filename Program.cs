using TurnitTest;
using TurnitTest.Methods;
class Program
{

    static void Main(string[] args)
    {
        // lists all breaks
        var breakList = new List<Break>();

        // check if text file is passed as an argument
        if (args.Length > 1 && args[0].ToLower() == "filename")
        {
            // get or generate entries
            try
            {

                // Generate random data
                // Methods.GenerateData(args[1], 100);

                breakList = Methods.ParseInitialEntries(args[1]);

                // initial calculation if file passed as arg
                Methods.GetBusiest(breakList);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }
        else
        {
            Console.WriteLine("Running without .txt file... \n");
        }

        
        while (true)
        {
            Console.WriteLine("Add time (example 13:1514:00): (Press Enter to exit): ");
            string input = Console.ReadLine();

            if (input == "") break; // exit when user enters nothing

            if (Methods.ValidEntry(input))
            {
                breakList.Add(Methods.ParseEntry(input));
            }

            Methods.GetBusiest(breakList);
        }
    }
}
