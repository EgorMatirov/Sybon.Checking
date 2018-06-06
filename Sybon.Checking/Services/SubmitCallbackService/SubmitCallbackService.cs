using System;
using System.Linq;
using System.Text;
using Bacs.Problem.Single;
using Bacs.Process;
using Bunsan.Broker;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using NLog;
using NLog.Fluent;
using Sybon.Checking.Repositories.SubmitResultRepository;
using Sybon.Common;
using BuildResult = Sybon.Checking.Repositories.SubmitResultRepository.BuildResult;
using Result = Bunsan.Broker.Result;

namespace Sybon.Checking.Services.SubmitCallbackService
{
    [UsedImplicitly]
    public class SubmitCallbackService : ISubmitCallbackService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private ClientListener _clientListener;
        private readonly object lockObj = new object();

        public SubmitCallbackService(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
        }

        public SubmitCallbackService Listen(ConnectionParameters connectionParameters)
        {
            _clientListener?.Dispose();
            _clientListener = new ClientListener(connectionParameters);
            _clientListener.Listen(StatusCallback, ResultCallback, ErrorCallback);
            return this;
        }

        private static void StatusCallback(string id, Status status)
        {
            // todo: process intermediate results from bacs
            Console.WriteLine(Encoding.UTF8.GetString(status.Data.ToByteArray()));
            var result = IntermediateResult.Parser.ParseFrom(status.Data.ToByteArray());
            Console.WriteLine(result.State.ToString());
        }
        
        private static void ErrorCallback(string id, string error)
        {
            // todo: process intermediate errors from bacs
            Console.WriteLine(error);
        }

        private void ResultCallback(string id, Result result)
        {
            switch (result.Status)
            {
                case Result.Types.Status.Ok when result.Data != null:
                    GetFinalResult(id, result.Data.ToByteArray());
                    break;
                case Result.Types.Status.ExecutionError:
                    SetExecutionError(id);
                    LogManager.GetCurrentClassLogger().Error($"Checking failed. Log is {result.Log.ToBase64()}");
                    break;
                default:
                    LogManager.GetCurrentClassLogger().Error($"Checking failed. Status is {result.Status}. Log is {result.Log.ToBase64()}");
                    break;
            }
        }

        private void SetExecutionError(string id)
        {
            lock (lockObj)
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var repositoryUnitOfWork = scope.ServiceProvider.GetRequiredService<IRepositoryUnitOfWork>();

                    var submitResult = repositoryUnitOfWork.GetRepository<ISubmitResultRepository>()
                        .FindAsync(long.Parse(id))
                        .Result;

                    EnsureHasBuildResult(submitResult);
                    submitResult.BuildResult.Status = BuildResult.BuildStatus.SERVER_ERROR;

                    repositoryUnitOfWork.SaveChangesAsync().Wait();
                }
            }
        }

        private static void EnsureHasBuildResult(SubmitResult submitResult)
        {
            if (submitResult.BuildResult == null)
            {
                submitResult.BuildResult = new BuildResult();
            }
        }

        public void Dispose()
        {
            _clientListener?.Dispose();
        }

        private void GetFinalResult(string submitIdStr, byte[] submitResultBytes)
        {
            lock (lockObj)
            {
                var submitResultId = long.Parse(submitIdStr);
                // Logger.Log.InfoFormat("GetFinalResult for submitResult {0}", submitResultId);
                var result = Bacs.Problem.Single.Result.Parser.ParseFrom(submitResultBytes);
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var repositoryUnitOfWork = scope.ServiceProvider.GetRequiredService<IRepositoryUnitOfWork>();
                
                    var submitResult = repositoryUnitOfWork.GetRepository<ISubmitResultRepository>().FindAsync(submitResultId).Result;

                    EnsureHasBuildResult(submitResult);
                    
                    submitResult.BuildResult.Output = result.Build.Output.ToByteArray();
                    submitResult.BuildResult.Status = (BuildResult.BuildStatus) (int) result.Build.Status;
    
                    submitResult.TestGroupResults = result.TestGroup?.Select((tgr, i) => new Repositories.SubmitResultRepository.TestGroupResult
                    {
                        InternalId = tgr.Id,
                        Executed = tgr.Executed,
                        OrderNumber = i,
                        TestResults = tgr.Test?.Select((tr, j) => new Repositories.SubmitResultRepository.TestResult
                        {
                            Status = GetStatus(tr),
                            JudgeMessage = tr.Judge?.Message,
                            Input = tr.File.FirstOrDefault(f => f.Id == "stdin")?.Data.ToStringUtf8(),
                            ActualResult = tr.File.FirstOrDefault(f => f.Id == "stdout")?.Data.ToStringUtf8(),
                            ExpectedResult = tr.File.FirstOrDefault(f => f.Id == "hint")?.Data.ToStringUtf8(),
                            ResourceUsage =  tr.Execution == null ? null : new Repositories.SubmitResultRepository.ResourceUsage
                            {
                                TimeUsageMillis = (long) tr.Execution.ResourceUsage.TimeUsageMillis,
                                MemoryUsageBytes = (long) tr.Execution.ResourceUsage.MemoryUsageBytes
                            },
                            OrderNumber = j
                        }).ToList()
                    }).ToList();
    
                    repositoryUnitOfWork.SaveChangesAsync().Wait();
                }
            }
        }
        
        private static TestResultStatus GetStatus(Bacs.Problem.Single.TestResult testResult)
        {
            var judgeStatus = ConvertJudgeStatus(testResult.Judge?.Status);
            return judgeStatus != TestResultStatus.SKIPPED
                ? judgeStatus
                : ConvertExecutionStatus(testResult.Execution?.Status);
        }
        
#region StatusConverters
        private static TestResultStatus ConvertJudgeStatus(JudgeResult.Types.Status? status)
        {
            switch (status)
            {
                case JudgeResult.Types.Status.Ok: return TestResultStatus.OK;
                case JudgeResult.Types.Status.WrongAnswer: return TestResultStatus.WRONG_ANSWER;
                case JudgeResult.Types.Status.PresentationError: return TestResultStatus.PRESENTATION_ERROR;
                case JudgeResult.Types.Status.QueriesLimitExceeded: return TestResultStatus.QUERIES_LIMIT_EXCEEDED;
                case JudgeResult.Types.Status.IncorrectRequest: return TestResultStatus.INCORRECT_REQUEST;
                case JudgeResult.Types.Status.InsufficientData: return TestResultStatus.INSUFFICIENT_DATA;
                case JudgeResult.Types.Status.ExcessData: return TestResultStatus.EXCESS_DATA;
                case JudgeResult.Types.Status.OutputLimitExceeded: return TestResultStatus.OUTPUT_LIMIT_EXCEEDED;
                case JudgeResult.Types.Status.TerminationRealTimeLimitExceeded: return TestResultStatus.TERMINATION_REAL_TIME_LIMIT_EXCEEDED;
                case JudgeResult.Types.Status.CustomFailure: return TestResultStatus.CUSTOM_FAILURE;
                case JudgeResult.Types.Status.FailTest: return TestResultStatus.FAIL_TEST;
                case JudgeResult.Types.Status.Failed: return TestResultStatus.FAILED;
                case JudgeResult.Types.Status.Skipped:
                case null:
                    return TestResultStatus.SKIPPED;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private static TestResultStatus ConvertExecutionStatus(ExecutionResult.Types.Status? status)
        {
            switch (status)
            {
                case ExecutionResult.Types.Status.Ok: return TestResultStatus.OK;
                case ExecutionResult.Types.Status.AbnormalExit: return TestResultStatus.ABNORMAL_EXIT;
                case ExecutionResult.Types.Status.MemoryLimitExceeded: return TestResultStatus.MEMORY_LIMIT_EXCEEDED;
                case ExecutionResult.Types.Status.TimeLimitExceeded: return TestResultStatus.TIME_LIMIT_EXCEEDED;
                case ExecutionResult.Types.Status.OutputLimitExceeded: return TestResultStatus.OUTPUT_LIMIT_EXCEEDED;
                case ExecutionResult.Types.Status.RealTimeLimitExceeded: return TestResultStatus.REAL_TIME_LIMIT_EXCEEDED;
                case ExecutionResult.Types.Status.TerminatedBySystem: return TestResultStatus.TERMINATED_BY_SYSTEM;
                case ExecutionResult.Types.Status.Failed: return TestResultStatus.FAILED;
                case null:
                    return TestResultStatus.SKIPPED;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        #endregion
    }
}