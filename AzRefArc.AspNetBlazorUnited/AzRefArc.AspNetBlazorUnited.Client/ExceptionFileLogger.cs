﻿using Microsoft.JSInterop;
using System.Collections.Concurrent;
using System.Reflection;
using System.Text;

namespace AzRefArc.AspNetBlazorUnited.Client
{
    public sealed class ExceptionFileLogger : ILogger
    {
        private IJSRuntime jSRuntime;
        public ExceptionFileLogger(IJSRuntime jSRuntime)
        {
            this.jSRuntime = jSRuntime;
        }

        public IDisposable? BeginScope<TState>(TState state) where TState : notnull => default!;

        public bool IsEnabled(LogLevel logLevel)
        {
            return (logLevel == LogLevel.Warning || logLevel == LogLevel.Error || logLevel == LogLevel.Critical);
        }

        public async void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
            if (!IsEnabled(logLevel)) return;

            string existingData = await jSRuntime.InvokeAsync<string>("localStorage.getItem", "exceptionData");
            await jSRuntime.InvokeVoidAsync("localStorage.setItem", "exceptionData",
                existingData + "\r\n" + $"{formatter(state, exception)}" + "\r\n" + (exception == null ? "" : ConvertExceptionToString(exception)));
        }

        private static string ConvertExceptionToString(Exception exception)
        {
            Dictionary<string, string> _generalInformation = new Dictionary<string, string>();

            if (exception == null) return "例外オブジェクト情報はありません。\r\n\r\n";
            StringBuilder strInfo = new StringBuilder("****** 一般情報 ******\r\n\r\n");

            // 一般情報を取得して文字列化
            // 発生時刻は上書き(なければ追加)
            _generalInformation["発生時刻"] = DateTimeOffset.Now.ToString();

            // 実行ランタイムの情報
            Assembly myAssembly = Assembly.GetEntryAssembly()!;
            // バージョンの設定
            _generalInformation["エントリアセンブリ情報"] = myAssembly.GetName().FullName;

            foreach (var key in _generalInformation.Keys)
            {
                strInfo.Append(key + ": " + _generalInformation[key] + "\r\n");
            }
            strInfo.Append("\r\n****** 例外情報 ******");

            // ネストされた例外を順次文字列化する
            Exception? currentException = exception!;
            int intExceptionCount = 1;
            do
            {
                strInfo.AppendFormat("\r\n\r\n{0}) 例外オブジェクト情報\r\n{1}", intExceptionCount.ToString(), "");
                strInfo.AppendFormat("\r\nException Type: {0}", currentException.GetType().FullName);

                try
                {
                    PropertyInfo[] aryPublicProperties = currentException.GetType().GetRuntimeProperties().ToArray();
                    foreach (PropertyInfo p in aryPublicProperties)
                    {
                        if (p.Name != "InnerException" && p.Name != "StackTrace")
                        {
                            try
                            {
                                if (p.GetValue(currentException, null) == null)
                                {
                                    strInfo.AppendFormat("\r\n{0}: NULL", p.Name);
                                }
                                else
                                {
                                    strInfo.AppendFormat("\r\n{0}: {1}", p.Name, p.GetValue(currentException, null));
                                }
                            }
                            catch (Exception)
                            {
                            }
                        }
                    }
                }
                catch (Exception)
                {
                }

                if (currentException.StackTrace != null)
                {
                    strInfo.AppendFormat("\r\n\r\nスタックトレース情報");
                    strInfo.AppendFormat("\r\n{0}\n", currentException.StackTrace);
                }
                currentException = currentException.InnerException;
                intExceptionCount++;
            } while (currentException != null);

            return strInfo.ToString();
        }
    }

    public class ExceptionFileLoggerProvider : ILoggerProvider
    {
        private readonly ConcurrentDictionary<string, ExceptionFileLogger> _loggers = new ConcurrentDictionary<string, ExceptionFileLogger>();

        private IJSRuntime jSRuntime;

        public ExceptionFileLoggerProvider(IJSRuntime jSRuntime)
        {
            // initialization code
            this.jSRuntime = jSRuntime;
        }

        public ILogger CreateLogger(string categoryName)
        {
            var logger = _loggers.GetOrAdd(categoryName, new ExceptionFileLogger(jSRuntime));
            return logger;
        }

        public void Dispose()
        {
            _loggers.Clear();
        }
    }
}
