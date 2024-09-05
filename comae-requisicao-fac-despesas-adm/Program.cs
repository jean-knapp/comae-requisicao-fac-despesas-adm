using comae_requisicao_fac_despesas_adm.Pages;

namespace comae_requisicao_fac_despesas_adm
{
    internal static class Program
    {
        static void Main(string[] args)
        {
            IO.LoadProgramData();
            IO.LoadLastRequesterData();

            Index.Print();
        }


    }
}
