using System;
using System.IO;
using System.Collections.Generic;
using ReportLibrary;
using System.Web;

namespace i18n
{
    /* Interface for accessing translated strings in PO files
     * The parsed PO files are cached. */
    public class I18n
    {
        public static readonly I18n Instance = new I18n();

        static I18n() { }
        I18n() { }

        string poDirectory = "";
        object catalogLock = new Object();
        Dictionary<string, Catalog> catalogs = new Dictionary<string, Catalog>();
        Catalog defaultCatalog = new Catalog();

        // dummy for marking up strings so they're catched by the extraction
        // script without translating them immediately
        public static string MarkString(string message)
        {
            return message;
        }

        // for convenience, this is static
        public static string GetString(string Message)
        {
            if (String.IsNullOrEmpty(Message))
                return "";

            var culture = (string)HttpContext.Current.Items["CurrentUICulture"];

            Catalog catalog = Instance.getCatalog(culture);

            if (catalog.messages.ContainsKey(Message) && catalog.messages[Message] != "")
                return catalog.messages[Message];
            else
                return Message;
        }

        public void Setup(string PoPath)
        {
            poDirectory = Path.GetFullPath(PoPath);
        }

        public void ClearCache()
        {
            lock (catalogLock)
            {
                catalogs = new Dictionary<string, Catalog>();
            }
        }

        Catalog getCatalog(string languageCode)
        {
            lock (catalogLock)
            {
                if (String.IsNullOrEmpty(languageCode))
                    languageCode = "da";

                string specific = languageCode;
                string generic = specific.Substring(0, 2);

                if (catalogs.ContainsKey(specific))
                    return catalogs[specific];

                string specificPath = Path.Combine(poDirectory, string.Format("{0}.po", specific));
                if (File.Exists(specificPath))
                {
                    catalogs[specific] = new Catalog(specificPath);
                    return catalogs[specific];
                }
                //Helpers.debug(specificPath + " doesn't exist");

                if (catalogs.ContainsKey(generic))
                    return catalogs[generic];

                string genericPath = Path.Combine(poDirectory, string.Format("{0}.po", generic));
                if (File.Exists(genericPath))
                {
                    catalogs[generic] = new Catalog(genericPath);
                    if (specific != generic)
                        catalogs[specific] = catalogs[generic]; // note this specific is handled by the generic
                    return catalogs[generic];
                }
                //Helpers.debug(genericPath + " doesn't exist");

                return defaultCatalog;
            }
        }
    }

    // contains the messages for a locale + PO file parser code to fetch it in
    class Catalog
    {
        public Dictionary<string, string> messages = new Dictionary<string, string>();
        public Dictionary<string, string> messagesPlural = new Dictionary<string, string>();

        public Catalog(string path)
        {
            parsePoFile(path);
        }

        public Catalog()
        {
        }

        string getPOLineContent(string line)
        {
            int start = line.IndexOf('"'), end = line.LastIndexOf('"');
            if (start == end) // catch if there's no " or only one
                return null;

            start += 1; // skip past the "

            return line.Substring(start, end - start).Replace("\\\"", "\"");
        }

        enum ParserState { SlurpingMsgId, SlurpingMsgIdPlural, SlurpingMsgStr, NextMessageIsFuzzy, InFuzzyMessage };

        void parsePoFile(string path)
        {
            Helpers.Debug("parsing file " + path);

            using (TextReader reader = new StreamReader(path, System.Text.Encoding.UTF8))
            {
                string line = "";
                string msgId = null, msgIdPlural = null;
                ParserState state = ParserState.SlurpingMsgId;
                while ((line = reader.ReadLine()) != null)
                {
                    line = line.Trim();

                    if (line.Length < 2)
                        continue;

                    if (line[0] == '#')
                    {
                        switch (line[1])
                        {
                            case ' ': // translator comment
                            case '.': // comment extracted from source code
                            case ':': // file references
                            case '|': // previous untranslated string
                            case '~': // commented, now obsolete string
                                break;

                            case ',': // flags
                                if (line.Contains("fuzzy"))
                                    state = ParserState.NextMessageIsFuzzy;
                                break;
                        }
                    }
                    else
                    {
                        if (line.StartsWith("msgid "))
                        {
                            if (state == ParserState.NextMessageIsFuzzy)
                            {
                                state = ParserState.InFuzzyMessage;
                                continue;
                            }

                            // if we're in state InFuzzyMessage, we're allowed to proceed,
                            // clearing the fuzzy state, since this must be the next msgid

                            msgId = "";
                            state = ParserState.SlurpingMsgId;
                        }

                        if (state == ParserState.InFuzzyMessage)
                            continue;

                        if (line.StartsWith("msgid_plural "))
                        {
                            msgIdPlural = "";
                            state = ParserState.SlurpingMsgIdPlural;
                        }

                        if (line.StartsWith("msgstr[")) // plural case, we don't handle that yet
                            continue;

                        if (line.StartsWith("msgstr"))
                        {
                            messages[msgId] = "";
                            state = ParserState.SlurpingMsgStr;
                        }

                        string content = getPOLineContent(line);
                        if (content == null || content == "")
                            continue;

                        switch (state)
                        {
                            case ParserState.SlurpingMsgId:
                                msgId += content;
                                break;

                            case ParserState.SlurpingMsgIdPlural:
                                msgIdPlural += content;
                                break;

                            case ParserState.SlurpingMsgStr:
                                messages[msgId] += content;
                                break;
                        }
                    }
                }
            }
        }
    }
}
