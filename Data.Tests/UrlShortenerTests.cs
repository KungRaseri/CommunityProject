using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Engine.ClientProtocol;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Data.Tests
{
    [TestClass]
    public class UrlShortenerTests
    {
        [TestInitialize]
        public void Initialize()
        {
        }

        [TestMethod]
        public void Encode()
        {
            for (var i = 0; i < 10000; i++)
            {
                if (AlphabetTest.Decode(AlphabetTest.Encode(i)) != i)
                {
                    System.Console.WriteLine("{0} is not {1}", AlphabetTest.Encode(i), i);
                    break;
                }
            }
        }

        [TestMethod]
        public void Decode()
        {

        }
    }

    public class AlphabetTest
    {
        public static readonly string Alphabet = "abcdefghijklmnopqrstuvwxyz0123456789";
        public static readonly int Base = Alphabet.Length;

        public static string Encode(int i)
        {
            if (i == 0) return Alphabet[0].ToString();

            var s = string.Empty;

            while (i > 0)
            {
                s += Alphabet[i % Base];
                i = i / Base;
            }

            return string.Join(string.Empty, s.Reverse());
        }

        public static int Decode(string s)
        {
            var i = 0;

            foreach (var c in s)
            {
                i = (i * Base) + Alphabet.IndexOf(c);
            }

            return i;
        }
    }
}
