using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows;
using KBS2.Exceptions;
using KBS2.Util;

namespace KBS2.Console.Commands
{
    public class CommandSet : ICommand
    {
        private static Regex _vectorRegex = new Regex(@"^(?<posX>-?\d+|-?\d*\.\d+),\s?(?<posY>-?\d+|-?\d*\.\d+)$");
        private static Regex _floatRegex = new Regex(@"^(?<value>-?\d*\.\d+)f$");
        private static Regex _doubleRegex = new Regex(@"^(?:(?<value>-?\d*\.\d+)(?:d?)|(?<value>-?\d+)d)$");
        private static Regex _intRegex = new Regex(@"^(?<value>-?\d+)$");
        private static Regex _stringRegex = new Regex(@"^""(?<value>.*)""$");
        
        public IEnumerable<char> Run(params string[] args)
        {
            if (args.Length <= 1) throw new InvalidParametersException();
            
            var prop = args[0];
            var val = new string[args.Length - 1];
            Array.Copy(args, 1, val, 0, val.Length);

            // Testing for Vector
            var output = TestVector(ref _vectorRegex, prop, val);
            if (output != string.Empty) return output;
            // Testing for float
            output = TestFloat(ref _floatRegex, prop, val);
            if (output != string.Empty) return output;
            // Testing for double
            output = TestDouble(ref _doubleRegex, prop, val);
            if (output != string.Empty) return output;
            // Testing for int
            output = TestInt(ref _intRegex, prop, val);
            if (output != string.Empty) return output;
            // Testing for string
            output = TestString(ref _stringRegex, prop, val);
            if (output != string.Empty) return output;
            
            throw new InvalidParametersException($"Could not parse input");
        }


        /// <summary>
        /// Tests if the input can be set to a <see cref="Vector"/>
        /// </summary>
        /// <param name="regex"><see cref="Regex"/> to match</param>
        /// <param name="prop">Name of the <see cref="Property"/> to set</param>
        /// <param name="val">Value to parse</param>
        /// <returns>Output message</returns>
        private static string TestVector(ref Regex regex, string prop, string[] val)
        {
            var vectorMatches = regex.Match(string.Join(" ", val));
            if (!vectorMatches.Success) return string.Empty;
            
            Vector output;
            try
            {
                output = new Vector(
                    double.Parse(vectorMatches.Groups["posX"].Value),
                    double.Parse(vectorMatches.Groups["posY"].Value)
                );
            }
            catch (OverflowException oe)
            {
                throw new InvalidParametersException(oe.Message);
            }
            return TryModifyProperty(prop, output, $"{output.X}, {output.Y}");
        }

        /// <summary>
        /// Tests if the input can be set to a <see cref="Single"/>
        /// </summary>
        /// <param name="regex"><see cref="Regex"/> to match</param>
        /// <param name="prop">Name of the <see cref="Property"/> to set</param>
        /// <param name="val">Value to parse</param>
        /// <returns>Output message</returns>
        private static string TestFloat(ref Regex regex, string prop, string[] val)
        {
            var floatMatches = regex.Match(string.Join(" ", val));
            if (!floatMatches.Success) return string.Empty;

            float output;
            try
            {
                output = float.Parse(floatMatches.Groups["value"].Value);
            }
            catch (OverflowException oe)
            {
                throw new InvalidParametersException(oe.Message);
            }
            return TryModifyProperty(prop, output, output.ToString(CultureInfo.InvariantCulture));
        }

        /// <summary>
        /// Tests if the input can be set to a <see cref="Double"/>
        /// </summary>
        /// <param name="regex"><see cref="Regex"/> to match</param>
        /// <param name="prop">Name of the <see cref="Property"/> to set</param>
        /// <param name="val">Value to parse</param>
        /// <returns>Output message</returns>
        private static string TestDouble(ref Regex regex, string prop, string[] val)
        {
            var doubleMatches = regex.Match(string.Join(" ", val));
            if (!doubleMatches.Success) return string.Empty;

            double output;
            try
            {
                output = double.Parse(doubleMatches.Groups["value"].Value);
            }
            catch (OverflowException oe)
            {
                throw new InvalidParametersException(oe.Message);
            }
            return TryModifyProperty(prop, output, output.ToString(CultureInfo.InvariantCulture));
        }

        /// <summary>
        /// Tests if the input can be set to an <see cref="Int32"/>
        /// </summary>
        /// <param name="regex"><see cref="Regex"/> to match</param>
        /// <param name="prop">Name of the <see cref="Property"/> to set</param>
        /// <param name="val">Value to parse</param>
        /// <returns>Output message</returns>
        private static string TestInt(ref Regex regex, string prop, string[] val)
        {
            var intMatches = regex.Match(string.Join(" ", val));
            if (!intMatches.Success) return string.Empty;

            int output;
            try
            {
                output = int.Parse(intMatches.Groups["value"].Value);
            }
            catch (OverflowException oe)
            {
                throw new InvalidParametersException(oe.Message);
            }
            return TryModifyProperty(prop, output, output.ToString());
        }

        /// <summary>
        /// Tests if the input can be set to a <see cref="String"/>
        /// </summary>
        /// <param name="regex"><see cref="Regex"/> to match</param>
        /// <param name="prop">Name of the <see cref="Property"/> to set</param>
        /// <param name="val">Value to parse</param>
        /// <returns>Output message</returns>
        private static string TestString(ref Regex regex, string prop, string[] val)
        {
            var stringMatches = regex.Match(string.Join(" ", val));
            if (!stringMatches.Success) return string.Empty;

            var output = stringMatches.Groups["value"].Value;
            return TryModifyProperty(prop, output, output);
        }


        /// <summary>
        /// Attempts to modify a <see cref="Property"/>
        /// </summary>
        /// <param name="prop">Name of the <see cref="Property"/> to set</param>
        /// <param name="output">Value to set</param>
        /// <param name="outputStringRep"><see cref="String"/> representation of the value</param>
        /// <returns>Output message</returns>
        /// <exception cref="InvalidParametersException">Error message</exception>
        private static string TryModifyProperty(string prop, dynamic output, string outputStringRep)
        {
            try
            {
                CommandHandler.ModifyProperty(prop, output);
                return $"Property \"{prop}\" set as ({outputStringRep})";
            }
            catch (KeyNotFoundException)
            {
                throw new InvalidParametersException($"Unknown property \"{prop}\"");
            }
            catch (TypeMismatchException)
            {
                throw new InvalidParametersException($"Property \"{prop}\" is not of type {output.GetType().Name}");
            }
        }
    }
}