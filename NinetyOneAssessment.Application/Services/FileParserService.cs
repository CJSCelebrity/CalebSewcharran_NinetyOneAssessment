using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NinetyOneAssessment.Application.Interfaces;

namespace NinetyOneAssessment.Application.Services;

public class FileParserService(
    IFileProcessingService fileProcessingService,
    ILogger<FileParserService> logger,
    IHostApplicationLifetime applicationLifetime)
{
}