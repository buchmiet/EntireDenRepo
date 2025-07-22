using System.Text.RegularExpressions;

namespace denSharedLibrary;

public class SimpleSearchEngine : ISearchEngine
{
    private Dictionary<int, string> _data;
    private Dictionary<string, List<int>> _keywordIndex;

    public SimpleSearchEngine()
    {
        _data = new Dictionary<int, string>();
        _keywordIndex = new Dictionary<string, List<int>>();
    }

    // Inicjalizacja słownika
    public void Initialize(Dictionary<int, string> data)
    {
        _data = data;
        _keywordIndex.Clear();
        BuildIndex();
    }

    // Wyszukiwanie na podstawie terminu
    public List<int> Find(string searchTerm)
    {
        var searchTokens = Tokenize(searchTerm);

        // Lista potencjalnych dopasowań
        var potentialMatches = new List<int>();

        // Budujemy wzorzec do dopasowania oparty na kolejności tokenów
        string pattern = string.Join(".*", searchTokens);

        // Używamy Regex, aby znaleźć dopasowanie do wzorca
        Regex regex = new Regex(pattern, RegexOptions.IgnoreCase);

        // Iteracja po wszystkich danych, by znaleźć te, które pasują do wzorca
        foreach (var dataEntry in _data)
        {
            if (regex.IsMatch(dataEntry.Value))
            {
                potentialMatches.Add(dataEntry.Key);
            }
        }

        return potentialMatches;
    }




    // Odświeżanie wpisu
    public void Refresh(KeyValuePair<int, string> dictionaryEntry)
    {
        if (_data.ContainsKey(dictionaryEntry.Key))
        {
            RemoveFromIndex(dictionaryEntry.Key);
            _data[dictionaryEntry.Key] = dictionaryEntry.Value;
            AddToIndex(dictionaryEntry.Key, dictionaryEntry.Value);
        }
    }

    // Dodawanie wpisu
    public void Add(KeyValuePair<int, string> dictionaryEntry)
    {
        _data.Add(dictionaryEntry.Key, dictionaryEntry.Value);
        AddToIndex(dictionaryEntry.Key, dictionaryEntry.Value);
    }

    // Tokenizacja
    private HashSet<string> Tokenize(string input)
    {
        string cleaned = Regex.Replace(input, @"[^a-zA-Z0-9\s-]", "").ToLower();
        string[] tokensArray = cleaned.Split(new[] { ' ', '-' }, StringSplitOptions.RemoveEmptyEntries);
        return new HashSet<string>(tokensArray);
    }

    // Budowanie indeksu
    private void BuildIndex()
    {
        foreach (var entry in _data)
        {
            AddToIndex(entry.Key, entry.Value);
        }
    }

    // Dodawanie do indeksu
    private void AddToIndex(int id, string value)
    {
        foreach (var token in Tokenize(value))
        {
            if (!_keywordIndex.ContainsKey(token))
            {
                _keywordIndex[token] = new List<int>();
            }

            _keywordIndex[token].Add(id);
        }
    }

    // Usuwanie z indeksu
    private void RemoveFromIndex(int id)
    {
        var entryValue = _data[id];
        foreach (var token in Tokenize(entryValue))
        {
            _keywordIndex[token].Remove(id);
        }
    }
}