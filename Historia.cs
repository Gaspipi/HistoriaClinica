namespace WinFormsApp1
{
    internal class Historia
    {
        List<string> FichasDiarias = new List<string>() { };

        public void AddLista(string[] a)
        {
            foreach (string element in a)
            {

                FichasDiarias.Add(element);
            }
        }
    }
}
