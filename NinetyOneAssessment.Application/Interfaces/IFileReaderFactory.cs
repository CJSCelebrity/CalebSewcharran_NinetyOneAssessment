namespace NinetyOneAssessment.Application.Interfaces;

public interface IFileReaderFactory
{
    IFileReader CreateFileReader(string path);
    void RegisterReader(IFileReader reader);
}