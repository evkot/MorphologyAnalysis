using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace Morphology
{
    /// <summary>
    /// Морфология с использованием mystem
    /// </summary>
    public class MystemMorphology
    {
        /// <summary>
        /// Путь к файлу mystem
        /// </summary>
        string MystemFile;

        /// <summary>
        /// Кодировка ввода/вывода. Возможные варианты: cp866, cp1251, koi8-r, utf-8 (по умолчанию).
        /// </summary>
        string TextEncoding;

        /// <summary>
        /// Построчный режим; каждое слово печатается на новой строке. Опция "-n"
        /// </summary>
        bool NewLine;

        /// <summary>
        /// Печатать грамматическую информацию. Опция "-i"
        /// </summary>
        bool PrintGrammar;

        /// <summary>
        /// Контекстное снятие омонимии. Опция "-d"
        /// </summary>
        bool Disambiguation;

        /// <summary>
        /// Конструктор
        /// </summary>
        public MystemMorphology(string _MystemFile, string _TextEncoding, bool _NewLine, bool _PrintGrammar, bool _Disambiguation)
        {
            MystemFile = _MystemFile;
            TextEncoding = _TextEncoding;
            NewLine = _NewLine;
            PrintGrammar = _PrintGrammar;
            Disambiguation = _Disambiguation;
        }

        /// <summary>
        /// Функция возвращает строку параметров для mystem
        /// </summary>
        private string GetParameterString()
        {
            // Проверка, есть ли параметры
            if (!(NewLine || PrintGrammar || Disambiguation || (TextEncoding != "")))
                return "";

            string parameters = "";

            if (NewLine) parameters += "-n ";
            if (PrintGrammar) parameters += "-i ";
            if (Disambiguation) parameters += "-d ";

            if (TextEncoding != "") parameters = parameters + "-e " + TextEncoding + " ";

            return parameters;
        }

        /// <summary>
        /// Морфологический анализ (на выходе - файл)
        /// </summary>
        public void MorphAnalysis(string FromFile, string ToFile)
        {
            // Формируем строку параметров
            string MystemParams = GetParameterString();
            string parameters = " " + MystemParams + " \"" + FromFile + "\" \"" + ToFile + "\"";

            try
            {
                ProcessStartInfo startInfo = new ProcessStartInfo(MystemFile);
                startInfo.WindowStyle = ProcessWindowStyle.Hidden;
                startInfo.CreateNoWindow = true;
                startInfo.Arguments = parameters;
                startInfo.UseShellExecute = false;

                Process mystem = Process.Start(startInfo);

                mystem.WaitForExit();   // Ожидание окончание выполнения mystem.exe
                mystem.Close();
            }
            catch { Console.WriteLine("Mystem is not found!"); }
        }

        /// <summary>
        /// Морфологический анализ (на выходе - список)
        /// </summary>
        public List<string> MorphAnalysis(string FromFile)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo(MystemFile);

            // Формируем строку параметров
            string MystemParams = GetParameterString();
            string parameters = " " + MystemParams + " \"" + FromFile;

            startInfo.WindowStyle = ProcessWindowStyle.Hidden;
            startInfo.CreateNoWindow = true;
            startInfo.Arguments = parameters;
            startInfo.UseShellExecute = false;
            startInfo.RedirectStandardOutput = true;
            startInfo.StandardOutputEncoding = Encoding.GetEncoding("windows-1251");
            Process mystem = Process.Start(startInfo);

            List<string> list = new List<string>();
            while (!mystem.StandardOutput.EndOfStream)
            {
                list.Add(mystem.StandardOutput.ReadLine());
            }

            mystem.WaitForExit();   // Ожидание окончание выполнения mystem.exe
            mystem.Close();
            return list;
        }

        /// <summary>
        /// Морфологический анализ (на входе и на выходе - список слов)
        /// </summary>
        /// <param name="Text">Строка, содержащая текст для анализа</param>
        public List<string> MorphAnalysis(string Text)
        {
            // Формируем структуру с информацией о процессе
            ProcessStartInfo startInfo = new ProcessStartInfo(MystemFile);

            // Формируем строку параметров
            string parameters = GetParameterString();

            // Формируем параметры процесса
            startInfo.WindowStyle = ProcessWindowStyle.Hidden;
            startInfo.CreateNoWindow = true;
            startInfo.Arguments = parameters;
            startInfo.UseShellExecute = false;
            startInfo.RedirectStandardInput = true;
            startInfo.RedirectStandardOutput = true;
            startInfo.StandardOutputEncoding = Encoding.GetEncoding(TextEncoding);

            // Настриваем входной консольный поток 
            Console.InputEncoding = System.Text.Encoding.GetEncoding(TextEncoding);

            // Запускаем процесс
            Process mystem = Process.Start(startInfo);

            // Отправляем текст на вход процесса
            mystem.StandardInput.WriteLine(Text);
            mystem.StandardInput.Close();

            // Формируем результаты mystem
            List<string> mystem_results = new List<string>();
            while (!mystem.StandardOutput.EndOfStream)
            {
                mystem_results.Add(mystem.StandardOutput.ReadLine());
            }

            // Ожидание окончание выполнения процесса
            mystem.WaitForExit();
            mystem.Close();

            return mystem_results;
        }
    }
}
