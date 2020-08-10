using System;
using System.Security.Cryptography.X509Certificates;

namespace TextTranslate
{
    public static class TextTranslatorExtensions
    {
        public static Translator From(this Translator translator, Func<Translator, Translator> from)
        {
            translator.AddFrom();
            return from(translator);
        }

        public static Translator ToPortugueseBR(this Translator translator)
        {
            translator.AddLanguage("pt-br");
            return translator;
        }

        public static Translator ToGerman(this Translator translator)
        {
            translator.AddLanguage("de");
            return translator;
        }

        public static Translator ToItalian(this Translator translator)
        {
            translator.AddLanguage("it");
            return translator;
        }

        public static Translator ToJaponese(this Translator translator)
        {
            translator.AddLanguage("ja");
            return translator;
        }

        public static Translator ToEnglish(this Translator translator)
        {
            translator.AddLanguage("en");
            return translator;
        }

        public static Translator To(this Translator translator, params Func<Translator, Translator>[] languages)
        {
            Array.ForEach(languages, language => language(translator));
            return translator;
        }
    }
}