namespace HardwareKeyCreationTool
{
    public interface IHardwareKeyConfigurator
    {
        void CreateHiddenMemory(uint sizeInMBs);
        void DeleteHiddenMemory();
        void SaveInHiddenMemory(string[] licenses);
        void WipeHiddenMemory();
    }
}