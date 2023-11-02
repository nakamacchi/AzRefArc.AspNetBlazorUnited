using System.Reflection;
using System.Text;
using System.Collections.Concurrent;

namespace AzRefArc.AspNetBlazorUnited
{
    public sealed class ExceptionFileLogger : ILogger
    {
        public IDisposable? BeginScope<TState>(TState state) where TState : notnull => default!;

        public bool IsEnabled(LogLevel logLevel)
        {
            return (logLevel == LogLevel.Warning || logLevel == LogLevel.Error);
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
            if (!IsEnabled(logLevel)) return;

            string fileName = string.Format("{0:yyyyMMdd}.txt", DateTime.Now);
            string folder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData, Environment.SpecialFolderOption.None) + @"\" + Assembly.GetEntryAssembly()!.GetName().Name;
            Directory.CreateDirectory(folder);
            string filePath = Path.Combine(folder, fileName);
            File.AppendAllText(filePath, $"{formatter(state, exception)}");
            if (exception != null) File.AppendAllText(filePath, ConvertExceptionToString(exception));
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

        public ExceptionFileLoggerProvider()
        {
            // initialization code
        }

        public ILogger CreateLogger(string categoryName)
        {
            var logger = _loggers.GetOrAdd(categoryName, new ExceptionFileLogger());
            return logger;
        }

        public void Dispose()
        {
            _loggers.Clear();
        }
    }
}