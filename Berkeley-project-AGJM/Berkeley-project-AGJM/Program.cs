namespace Berkeley_project_AGJM
{
    internal class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 2)
            {
                Console.WriteLine("Uso: dotnet run <numeroProcessosIniciais> <portInicial>");
                return;
            }

            int numeroProcessosInicias = int.Parse(args[0]);
            int portInicial = int.Parse(args[1]);

            if (portInicial <= 0)
            {
                Console.WriteLine("<portInicial> precisa ser maior que 0");
                return;
            }

            NodeManager.StartNodes(numeroProcessosInicias, portInicial);
        }
    }
}
