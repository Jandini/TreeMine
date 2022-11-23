namespace DirectoryNavigator
{
    internal interface IMain
    {
        void Count(string path);
        void Scan(string path);
        void Hash(string path);
    }
}