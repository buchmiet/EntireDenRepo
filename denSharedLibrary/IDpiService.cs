namespace denSharedLibrary;

public interface IDpiService
{
    int GetWidth(int resolution, int size);
    int GetHeight(int resolution, int size);
}