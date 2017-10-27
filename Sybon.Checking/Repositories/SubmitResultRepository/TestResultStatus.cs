using System.Diagnostics.CodeAnalysis;

namespace Sybon.Checking.Repositories.SubmitResultRepository
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public enum TestResultStatus
    {
        OK = 0,
        WRONG_ANSWER = 1,
        PRESENTATION_ERROR = 2,
        QUERIES_LIMIT_EXCEEDED = 3,
        INCORRECT_REQUEST = 4,
        INSUFFICIENT_DATA = 5,
        EXCESS_DATA = 6,
        OUTPUT_LIMIT_EXCEEDED = 7,
        TERMINATION_REAL_TIME_LIMIT_EXCEEDED = 8,
        ABNORMAL_EXIT = 9,
        MEMORY_LIMIT_EXCEEDED = 10,
        TIME_LIMIT_EXCEEDED = 11,
        REAL_TIME_LIMIT_EXCEEDED = 13,
        TERMINATED_BY_SYSTEM = 14,
        CUSTOM_FAILURE = 500,
        FAIL_TEST = 999,
        FAILED = 1000,
        SKIPPED = 2000,
    }
}