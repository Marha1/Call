using System.Text.RegularExpressions;

namespace Infrastructure;

public class ProfanityFilterService
{
    private static readonly Dictionary<char, string[]> CharReplacements = new()
    {
        { 'а', new[] { "a", "@", "а" } }, { 'б', new[] { "6", "b", "б" } },
        { 'в', new[] { "b", "v", "в" } }, { 'г', new[] { "r", "g", "г" } },
        { 'д', new[] { "d", "g", "д" } }, { 'е', new[] { "e", "е" } },
        { 'ё', new[] { "e", "ё", "е" } }, { 'ж', new[] { "zh", "*", "ж" } },
        { 'з', new[] { "3", "z", "з" } }, { 'и', new[] { "u", "i", "и" } },
        { 'й', new[] { "u", "y", "i", "й" } }, { 'к', new[] { "k", "i{", "|{", "к" } },
        { 'л', new[] { "l", "ji", "л" } }, { 'м', new[] { "m", "м" } },
        { 'н', new[] { "h", "n", "н" } }, { 'о', new[] { "o", "0", "о" } },
        { 'п', new[] { "n", "p", "п" } }, { 'р', new[] { "r", "p", "р" } },
        { 'с', new[] { "c", "s", "с" } }, { 'т', new[] { "m", "t", "т" } },
        { 'у', new[] { "y", "u", "у" } }, { 'ф', new[] { "f", "ф" } },
        { 'х', new[] { "x", "h", "к", "k", "}{", "х" } }, { 'ц', new[] { "c", "u,", "ц" } },
        { 'ч', new[] { "ch", "ч" } }, { 'ш', new[] { "sh", "ш" } },
        { 'щ', new[] { "sch", "щ" } }, { 'ь', new[] { "b", "ь" } },
        { 'ы', new[] { "bi", "ы" } }, { 'ъ', new[] { "ъ" } },
        { 'э', new[] { "e", "е", "э" } }, { 'ю', new[] { "io", "ю" } },
        { 'я', new[] { "ya", "я" } }
    };

    private static readonly HashSet<string> ForbiddenWords = new()
    {
        "хуй", "далбаеб", "ебаный", "ебаная", "ебанный", "ебанная", "пизда", "ебать", "блять", "гондон", "гандон",
        "сука", "мразь", "чмо", "уебок", "шлюха",
        "залупа", "ебло", "мудак", "долбоеб", "ебанутый", "пидор", "пидорас", "пидр", "еблан",
        "пиздец", "нахуй", "пошелнахуй", "идинахуй", "хуесос", "хуила", "хуёвый", "хуёвина",
        "хуйнуть", "хуйло", "хуеверт", "хуяк", "хуячить", "хуебес", "хуеглот", "хуедрыга", "хуеплет",
        "хуесоска", "ебанат", "ебарь", "ебнуться", "ебануться", "ебанешься", "выебон",
        "доебаться", "доебаться", "заебись", "заебать", "заебенить", "заебениться", "наебать",
        "наебалово", "наебка", "наебщик", "наебщица", "наебнуться", "наебну", "переебать",
        "переебнуться", "пиздануть", "пиздануться", "пиздец", "пиздить", "пиздюк", "пиздячить",
        "пиздёныш", "пиздохаться", "пиздострадать", "пиздуй", "пиздуйка", "пиздюкать", "распиздяй",
        "распиздяйка", "распиздеться", "распиздяйство", "распиздяйствовать", "распиздон",
        "распиздохать", "разъебать", "разъебись", "разъебос", "разъебон", "разъебываться",
        "охуеть", "охуенный", "охуетькак", "охуевать", "охуевший", "охуел", "охуительно",
        "ахуенно", "ахуеть", "ахуенный", "нахер", "нахрен", "нахуй", "нахуйта", "нахуярить",
        "нахерачить", "нахреначить", "нахренеть", "нахерить", "нахера", "перепиздеть",
        "перепиздиться", "переебашить", "переебенить", "перехуярить", "переебашиться",
        "переебаться", "переебнуться", "переебашка", "переебнуть", "перехуяриваться",
        "перехуярился", "перехуяриться", "перехуячить", "перехуячиться", "перехуяривание",
        "перехуярка", "перехуярованный", "перехуяренный", "перехуяривание", "перехуяренный",
        "перехуяренный", "перехуяривший", "перехуярить", "перехуяриваться", "перехуяриться",
        "перехуярка", "перехуярочка", "перехуярованный", "перехуяренный", "перехуяривание",
        "перехуяренный", "перехуяривший", "перехуярить", "перехуяриваться", "перехуяриться",
        "перехуярка", "перехуярочка", "перехуярованный", "перехуяренный", "перехуяривание",
        "перехуяренный", "перехуяривший", "перехуярить", "перехуяриваться", "перехуяриться",
        "перехуярка", "перехуярочка", "перехуярованный", "перехуяренный", "перехуяривание",
        "перехуяренный", "перехуяривший", "перехуярить", "перехуяриваться", "перехуяриться",
        "перехуярка", "перехуярочка", "перехуярованный", "перехуяренный", "перехуяривание",
        "перехуяренный", "перехуяривший", "перехуярить", "перехуяриваться", "перехуяриться",
        "перехуярка", "перехуярочка", "перехуярованный", "перехуяренный", "перехуяривание",
        "перехуяренный", "перехуяривший", "перехуярить", "перехуяриваться", "перехуяриться",
        "перехуярка", "перехуярочка", "перехуярованный", "перехуяренный", "перехуяривание",
        "перехуяренный", "перехуяривший", "перехуярить", "перехуяриваться", "перехуяриться",
        "перехуярка", "перехуярочка", "перехуярованный", "перехуяренный", "перехуяривание",
        "перехуяренный", "перехуяривший", "перехуярить", "перехуяриваться", "перехуяриться",
        "перехуярка", "перехуярочка", "перехуярованный", "перехуяренный", "перехуяривание",
        "перехуяренный", "перехуяривший", "перехуярить", "перехуяриваться", "перехуяриться",
        "перехуярка", "перехуярочка", "перехуярованный", "перехуяренный", "перехуяривание"
    };

    public bool ContainsProfanity(string text)
    {
        if (string.IsNullOrWhiteSpace(text)) return false;

        text = text.ToLower();

        text = Regex.Replace(text, @"[\s\-_]+", "");

        // Заменяем похожие символы
        foreach (var (key, replacements) in CharReplacements)
        foreach (var replacement in replacements)
            text = text.Replace(replacement, key.ToString());

        // Проверяем на наличие запрещённых слов
        return ForbiddenWords.Any(word => text.Contains(word));
    }
}